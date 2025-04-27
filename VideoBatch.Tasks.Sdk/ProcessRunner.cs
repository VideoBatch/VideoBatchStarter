using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoBatch.Model; // Assuming context or progress reporting might use model types later

namespace VideoBatch.Tasks.Sdk
{
    /// <summary>
    /// Represents the result of a process execution.
    /// </summary>
    public class ProcessResult
    {
        public int ExitCode { get; }
        public string OutputFilePath { get; }
        public string ErrorFilePath { get; }
        public bool TimedOut { get; }
        public Exception? Exception { get; } // Any exception during process start/management

        public bool Success => ExitCode == 0 && Exception == null && !TimedOut;

        public ProcessResult(int exitCode, string outputFilePath, string errorFilePath, bool timedOut, Exception? exception = null)
        {
            ExitCode = exitCode;
            OutputFilePath = outputFilePath;
            ErrorFilePath = errorFilePath;
            TimedOut = timedOut;
            Exception = exception;
        }
    }

    /// <summary>
    /// Helper class to run external processes, capture output/error streams,
    /// and provide real-time feedback.
    /// </summary>
    public class ProcessRunner
    {
        /// <summary>
        /// Runs an external process asynchronously, capturing and teeing stdout/stderr.
        /// </summary>
        /// <param name="executablePath">Path to the executable file.</param>
        /// <param name="arguments">Command-line arguments for the executable.</param>
        /// <param name="workingDirectory">The working directory for the process.</param>
        /// <param name="outputProgress">Callback for reporting standard output lines.</param>
        /// <param name="errorProgress">Callback for reporting standard error lines.</param>
        /// <param name="cancellationToken">Token to cancel the process execution.</param>
        /// <param name="timeout">Optional timeout for the process execution.</param>
        /// <param name="outputFilePath">Optional path to write the standard output to (tee functionality).</param>
        /// <param name="errorFilePath">Optional path to write the standard error to (tee functionality).</param>
        /// <returns>A Task representing the asynchronous operation, yielding a ProcessResult.</returns>
        public static async Task<ProcessResult> RunProcessAsync(
            string executablePath,
            string arguments,
            string workingDirectory,
            IProgress<string>? outputProgress = null,
            IProgress<string>? errorProgress = null,
            CancellationToken cancellationToken = default,
            TimeSpan? timeout = null,
            string? outputFilePath = null,
            string? errorFilePath = null)
        {
            int exitCode = -1;
            bool timedOut = false;
            Exception? runException = null;
            string actualOutputFilePath = outputFilePath ?? Path.GetTempFileName(); // Use temp if not specified
            string actualErrorFilePath = errorFilePath ?? Path.GetTempFileName();   // Use temp if not specified

            // Ensure directory exists for output files if specified
            if (outputFilePath != null) 
            {
                 var outputDir = Path.GetDirectoryName(outputFilePath);
                 if (!string.IsNullOrEmpty(outputDir)) 
                 {
                     Directory.CreateDirectory(outputDir);
                 }
            }
            if (errorFilePath != null)
            {
                var errorDir = Path.GetDirectoryName(errorFilePath);
                if (!string.IsNullOrEmpty(errorDir))
                {
                    Directory.CreateDirectory(errorDir);
                }
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = false, // Standard input redirection can be added later if needed
                UseShellExecute = false,       // Required for redirection
                CreateNoWindow = true,         // Don't show the console window
                StandardOutputEncoding = Encoding.UTF8, // Specify encoding
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
            using var outputWriter = new StreamWriter(actualOutputFilePath, append: false, Encoding.UTF8);
            using var errorWriter = new StreamWriter(actualErrorFilePath, append: false, Encoding.UTF8);

            var outputReadTaskCompletionSource = new TaskCompletionSource<bool>();
            var errorReadTaskCompletionSource = new TaskCompletionSource<bool>();
            var processExitCompletionSource = new TaskCompletionSource<int>();

            process.OutputDataReceived += async (sender, e) =>
            {
                if (e.Data == null)
                {
                    outputReadTaskCompletionSource.TrySetResult(true); // Signal completion
                    return;
                }
                outputProgress?.Report(e.Data);
                await outputWriter.WriteLineAsync(e.Data);
                // Consider adding flush logic if immediate writing is critical
                 // await outputWriter.FlushAsync(); // Uncomment if needed, might impact performance
            };

            process.ErrorDataReceived += async (sender, e) =>
            {
                if (e.Data == null)
                {
                    errorReadTaskCompletionSource.TrySetResult(true); // Signal completion
                    return;
                }
                errorProgress?.Report(e.Data);
                await errorWriter.WriteLineAsync(e.Data);
                // Consider adding flush logic
                 // await errorWriter.FlushAsync(); // Uncomment if needed
            };

            process.Exited += (sender, e) =>
            {
                 // Ensure we attempt to get the exit code only after the process has truly exited
                try
                {
                     // Adding a small delay might help ensure streams are flushed before checking ExitCode
                     // Task.Delay(100).Wait(cancellationToken); // Optional delay, test if needed
                    processExitCompletionSource.TrySetResult(process.ExitCode);
                }
                catch (InvalidOperationException ioEx) when (ioEx.Message.Contains("exited"))
                {
                    // Handle race condition where Exited event fires but ExitCode isn't available yet
                    processExitCompletionSource.TrySetResult(-999); // Indicate potential issue getting exit code
                    runException ??= new InvalidOperationException("Process exited but ExitCode could not be retrieved immediately.", ioEx);
                }
                 catch (Exception ex) when (!(ex is OperationCanceledException))
                 {
                     processExitCompletionSource.TrySetException(ex);
                 }
            };

            try
            {
                if (!File.Exists(executablePath))
                {
                    throw new FileNotFoundException($"Executable not found at '{executablePath}'.");
                }
                 if (!Directory.Exists(workingDirectory))
                {
                    throw new DirectoryNotFoundException($"Working directory not found: '{workingDirectory}'.");
                }

                if (!process.Start())
                {
                    throw new InvalidOperationException($"Failed to start process: {executablePath}");
                }

                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Process Started: {process.Id} - {executablePath} {arguments}");

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // --- Cancellation and Timeout Handling ---
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                if (timeout.HasValue)
                {
                    linkedCts.CancelAfter(timeout.Value);
                }

                try
                {
                     // Wait for process exit OR cancellation/timeout
                    exitCode = await processExitCompletionSource.Task.WaitAsync(linkedCts.Token);

                    // Wait for stream readers to finish *after* process exit signal
                    // Give them a grace period to process remaining buffered data.
                     await Task.WhenAll(
                         outputReadTaskCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), CancellationToken.None), // Use separate timeout
                         errorReadTaskCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5), CancellationToken.None)
                     ).ConfigureAwait(false); // Allow continuation on any thread

                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Process Exited: {process.Id}, ExitCode: {exitCode}");

                }
                catch (TaskCanceledException) // Catches cancellation from linkedCts (original token or timeout)
                {
                    timedOut = !cancellationToken.IsCancellationRequested; // It's a timeout if the original token wasn't cancelled
                    runException ??= new TimeoutException($"Process execution timed out after {timeout?.TotalSeconds ?? 0} seconds.");
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Process {(timedOut ? "Timed Out" : "Cancelled")}: {process.Id}");

                    // Attempt to kill the process if cancelled or timed out
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill(entireProcessTree: true); // Kill process and potential children
                            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Process Killed: {process.Id}");
                             exitCode = -99; // Indicate forced termination
                        }
                    }
                    catch (Exception killEx) when (!(killEx is OperationCanceledException))
                    {
                         Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error killing process {process.Id}: {killEx.Message}");
                        runException ??= new InvalidOperationException("Failed to kill process after cancellation/timeout.", killEx);
                    }
                     // Still wait briefly for streams even after kill attempt
                      await Task.WhenAll(
                          outputReadTaskCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(2)),
                          errorReadTaskCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(2))
                      ).ConfigureAwait(false);
                }
                finally
                {
                      // Ensure writers are flushed and disposed properly
                      await outputWriter.FlushAsync();
                      await errorWriter.FlushAsync();
                      outputWriter.Close(); // Explicitly close before returning paths
                      errorWriter.Close(); // Explicitly close before returning paths
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                 Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error running process {executablePath}: {ex.Message}");
                 runException = ex;
                 exitCode = -1; // Indicate failure to start or manage process
            }

            // --- Cleanup temporary files if they were used ---
            string finalOutputFilePath = outputFilePath ?? actualOutputFilePath;
            string finalErrorFilePath = errorFilePath ?? actualErrorFilePath;

            if (outputFilePath == null && File.Exists(actualOutputFilePath))
            {
                 // Optionally delete temp file or decide behavior
                 // File.Delete(actualOutputFilePath);
                 // finalOutputFilePath = string.Empty; // Indicate no persistent file
                 Debug.WriteLine($"[ProcessRunner] Note: Output was written to temp file: {actualOutputFilePath}");
            }
             if (errorFilePath == null && File.Exists(actualErrorFilePath))
            {
                 // Optionally delete temp file
                 // File.Delete(actualErrorFilePath);
                 // finalErrorFilePath = string.Empty;
                 Debug.WriteLine($"[ProcessRunner] Note: Error output was written to temp file: {actualErrorFilePath}");
            }


            return new ProcessResult(exitCode, finalOutputFilePath, finalErrorFilePath, timedOut, runException);
        }
    }
} 