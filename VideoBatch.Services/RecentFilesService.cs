using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace VideoBatch.Services
{
    public class RecentFilesService : IRecentFilesService
    {
        private readonly string _recentFilesPath;
        private readonly int _maxFiles = 5;
        private List<string> _recentFiles;
        private readonly ILogger<RecentFilesService> _logger;

        public RecentFilesService(ILogger<RecentFilesService> logger)
        {
            _logger = logger;
            // Store recent files list in user's local app data
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDir = Path.Combine(appDataPath, "VideoBatch");
            Directory.CreateDirectory(appDir); // Ensure directory exists
            _recentFilesPath = Path.Combine(appDir, "recent_projects.txt");
            _recentFiles = LoadRecentFiles();
            _logger.LogInformation("RecentFilesService initialized. Storage path: {Path}", _recentFilesPath);
        }

        public IList<string> GetRecentFiles()
        {
            // Return a copy to prevent external modification
            return new List<string>(_recentFiles);
        }

        public void AddRecentFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) // Basic validation
            {
                _logger.LogWarning("Attempted to add invalid or non-existent path to recent files: {Path}", path);
                return;
            }

            // Remove if already exists
            _recentFiles.Remove(path);

            // Add to top
            _recentFiles.Insert(0, path);

            // Trim list
            if (_recentFiles.Count > _maxFiles)
            {
                _recentFiles = _recentFiles.Take(_maxFiles).ToList();
            }

            SaveRecentFiles();
            _logger.LogDebug("Added recent file: {Path}. Current count: {Count}", path, _recentFiles.Count);
        }

        private List<string> LoadRecentFiles()
        {
            try
            {
                if (File.Exists(_recentFilesPath))
                {
                    // Read paths, filter out any that no longer exist
                    var paths = File.ReadAllLines(_recentFilesPath)
                                     .Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
                                     .Distinct() // Ensure uniqueness
                                     .ToList();
                    _logger.LogDebug("Loaded {Count} valid recent files from {Path}", paths.Count, _recentFilesPath);
                    return paths;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recent files from {Path}", _recentFilesPath);
            }
            return new List<string>(); // Return empty list on error or if file doesn't exist
        }

        private void SaveRecentFiles()
        {
            try
            {
                File.WriteAllLines(_recentFilesPath, _recentFiles);
                _logger.LogDebug("Saved {Count} recent files to {Path}", _recentFiles.Count, _recentFilesPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving recent files to {Path}", _recentFilesPath);
            }
        }
    }
} 