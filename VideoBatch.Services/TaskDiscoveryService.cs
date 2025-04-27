using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using VideoBatch.Tasks.Interfaces;

namespace VideoBatch.Services 
{
    public class TaskDiscoveryService : ITaskDiscoveryService 
    {
        private readonly ILogger<TaskDiscoveryService> _logger;
        private List<Type> _discoveredTaskTypes = new List<Type>();

        public IEnumerable<Type> DiscoveredTaskTypes => _discoveredTaskTypes.AsReadOnly();

        public TaskDiscoveryService(ILogger<TaskDiscoveryService> logger)
        {
            _logger = logger;
        }

        public void DiscoverTasks(string pluginDirectory = "Tasks")
        {
            _discoveredTaskTypes.Clear();
            var taskInterfaceType = typeof(IJobTask);
            var currentAppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(currentAppPath))
            {
                _logger.LogError("Could not determine application execution path.");
                return;
            }

            var fullPluginPath = Path.Combine(currentAppPath, pluginDirectory);

            if (!Directory.Exists(fullPluginPath))
            {
                _logger.LogWarning("Task directory not found: {PluginPath}. Attempting to create it.", fullPluginPath);
                try
                {
                    Directory.CreateDirectory(fullPluginPath);
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, "Failed to create task directory: {PluginPath}", fullPluginPath);
                     return; 
                }
            }

            _logger.LogInformation("Scanning for tasks in: {PluginPath}", fullPluginPath);

            foreach (var dllFile in Directory.GetFiles(fullPluginPath, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);

                    var taskTypesInAssembly = assembly.GetTypes()
                        .Where(t => taskInterfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .ToList();

                    if (taskTypesInAssembly.Any())
                    {
                        _logger.LogInformation("Found {TaskCount} task type(s) in {FileName}", taskTypesInAssembly.Count, Path.GetFileName(dllFile));
                        _discoveredTaskTypes.AddRange(taskTypesInAssembly);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    _logger.LogError("Error loading types from {FileName}: {ErrorMessage}", Path.GetFileName(dllFile), ex.Message);
                    foreach (var loaderEx in ex.LoaderExceptions.Where(l => l != null))
                    {
                        _logger.LogError(" - LoaderException: {LoaderMessage}", loaderEx.Message);
                    }
                }
                catch (BadImageFormatException)
                {
                     _logger.LogWarning("Skipping file {FileName} as it's not a valid .NET assembly.", Path.GetFileName(dllFile));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading assembly {FileName}", Path.GetFileName(dllFile));
                }
            }
            _logger.LogInformation("Total task types found: {TotalCount}", _discoveredTaskTypes.Count);
        }

        public IJobTask? InstantiateTask(Type taskType)
        {
            if (!typeof(IJobTask).IsAssignableFrom(taskType) || taskType.IsAbstract || taskType.IsInterface)
            {
                _logger.LogWarning("Attempted to instantiate invalid task type: {TaskTypeName}", taskType.FullName);
                return null; 
            }
            try
            {
                return (IJobTask?)Activator.CreateInstance(taskType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error instantiating task {TaskTypeName}", taskType.FullName);
                return null;
            }
        }
    }

    public interface ITaskDiscoveryService
    {
        IEnumerable<Type> DiscoveredTaskTypes { get; }
        void DiscoverTasks(string pluginDirectory = "Tasks");
        IJobTask? InstantiateTask(Type taskType);
    }

} 