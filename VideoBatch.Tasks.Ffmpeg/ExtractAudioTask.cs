using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VideoBatch.Model;
using VideoBatch.Tasks.Interfaces;
using VideoBatch.Tasks.Sdk; // Reference the SDK project
using System.Collections.Generic; // Added for IEnumerable
using System.Linq; // Added for Enumerable.Empty

namespace VideoBatch.Tasks.Ffmpeg
{
    /// <summary>
    /// Task to extract the audio stream from a video file using ffmpeg.
    /// </summary>
    public class ExtractAudioTask : IBatchTask // <<< IMPLEMENT NEW INTERFACE
    {
        // Unique ID for this task type (Generate a new GUID for each task type)
        public Guid ID => new Guid("a1b2c3d4-e5f6-7788-9900-aabbccddeeff"); // <<< TODO: Replace with a REAL new GUID!

        public string Name => "Extract Audio (ffmpeg)";
        public string Description => "Extracts the audio stream from the input video file without re-encoding (copies the codec). Outputs an AAC file by default.";
        public string Version => "1.0";

        // TODO: Make ffmpeg path configurable
        private const string FfmpegExecutable = "ffmpeg.exe"; // Assume ffmpeg is in PATH or alongside the app

        /// <summary>
        /// Defines configurable properties for this task.
        /// TODO: Implement actual properties (e.g., output format/codec, bitrate)
        /// </summary>
        public IEnumerable<TaskProperty> GetPropertyDefinitions()
        {
            // Return an empty list for now, indicating no configurable properties yet.
            return Enumerable.Empty<TaskProperty>();
        }

        public Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context, CancellationToken cancellationToken)
        {
            context.Messages.Add($"[{Name} v{Version}] Starting execution.");

            if (string.IsNullOrEmpty(context.InputFilePath))
            {
                context.Messages.Add("[ERROR] Input file path is missing.");
                context.HasError = true;
                return Task.FromResult(context);
            }

            if (!File.Exists(context.InputFilePath))
            {
                context.Messages.Add($"[ERROR] Input file not found: {context.InputFilePath}");
                context.HasError = true;
                return Task.FromResult(context);
            }

            // --- Prepare Paths ---
            string inputFile = context.InputFilePath;
            string outputDirectory = Path.GetTempPath(); // Or derive from context/settings
            string outputFileName = Path.GetFileNameWithoutExtension(inputFile) + "_audio.aac"; // Default to AAC container
            string outputFilePath = Path.Combine(outputDirectory, outputFileName);

            // --- Prepare Arguments for ffmpeg ---
            // -i: input file
            // -vn: disable video recording (audio only)
            // -acodec copy: copy the audio codec without re-encoding
            // -y: overwrite output file without asking
            string ffmpegArgs = $"-i \"{inputFile}\" -vn -acodec copy -y \"{outputFilePath}\"";

            context.Messages.Add($"Input: {inputFile}");
            context.Messages.Add($"Output: {outputFilePath}");
            context.Messages.Add($"Command: {FfmpegExecutable} {ffmpegArgs}");

            // --- Setup Progress Reporting --- 
            var outputProgress = new Progress<string>(line =>
            {
                context.Messages.Add($"[FFMPEG_OUT] {line}");
                Debug.WriteLine($"[FFMPEG_OUT] {line}"); // Also write to debug
            });

            var errorProgress = new Progress<string>(line =>
            {
                // ffmpeg often reports progress and info to stderr
                context.Messages.Add($"[FFMPEG_ERR] {line}");
                Debug.WriteLine($"[FFMPEG_ERR] {line}"); // Also write to debug
            });

            // --- Execute Process --- 
            context.Messages.Add("Starting ffmpeg process...");
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                // Using Path.GetTempPath() as working directory for simplicity
                Task<ProcessResult> runTask = ProcessRunner.RunProcessAsync(
                    FfmpegExecutable,
                    ffmpegArgs,
                    Path.GetTempPath(), // Working directory
                    outputProgress,
                    errorProgress,
                    cancellationToken
                    // Consider adding a default timeout from configuration later
                    // timeout: TimeSpan.FromMinutes(30) 
                );

                // Await the result
                ProcessResult result = runTask.Result; // Block and wait here for simplicity in this example
                                                  // For UI responsiveness, consider making ExecuteAsync truly async 
                                                  // and awaiting runTask properly.

                stopwatch.Stop();
                context.Messages.Add($"ffmpeg process finished in {stopwatch.ElapsedMilliseconds} ms.");
                context.Messages.Add($"Exit Code: {result.ExitCode}");

                if (result.Success)
                {
                    if (File.Exists(outputFilePath))
                    {
                        context.Messages.Add($"Successfully extracted audio to: {outputFilePath}");
                        context.OutputFilePath = outputFilePath; // Set the output path for the next task
                        context.HasError = false;
                    }
                    else
                    {
                        context.Messages.Add("[ERROR] ffmpeg reported success (ExitCode 0), but the output file was not found!");
                        context.Messages.Add($"Expected file: {outputFilePath}");
                        context.HasError = true;
                    }
                }
                else
                {
                    context.Messages.Add("[ERROR] ffmpeg process failed.");
                    if (result.TimedOut)
                    {
                        context.Messages.Add("Reason: Process timed out.");
                    }
                    if (result.Exception != null)
                    {
                        context.Messages.Add($"Reason: Exception - {result.Exception.GetType().Name}: {result.Exception.Message}");
                        Debug.WriteLine($"[TASK ERROR] Exception during process run: {result.Exception}");
                    }
                    if (result.ExitCode != 0)
                    {
                         context.Messages.Add($"Check ffmpeg error output above for details (Exit Code: {result.ExitCode}).");
                    }
                    context.HasError = true;
                     // Clean up potentially incomplete output file
                    if (File.Exists(outputFilePath))
                    {
                        try { File.Delete(outputFilePath); }
                        catch (Exception ex) { context.Messages.Add($"[WARN] Could not delete potentially incomplete output file {outputFilePath}: {ex.Message}"); }
                    }
                }
            }
            catch (AggregateException aggEx) when (aggEx.InnerException is TaskCanceledException)
            {
                stopwatch.Stop();
                context.Messages.Add($"[CANCELLED] Task execution was cancelled after {stopwatch.ElapsedMilliseconds} ms.");
                context.HasError = true; // Treat cancellation as an error state for the chain
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                context.Messages.Add($"[FATAL ERROR] An unexpected error occurred during task execution: {ex.GetType().Name} - {ex.Message}");
                Debug.WriteLine($"[FATAL ERROR] {ex}");
                context.HasError = true;
            }

            context.Messages.Add($"[{Name}] Execution finished.");
            return Task.FromResult(context);
        }
    }
} 