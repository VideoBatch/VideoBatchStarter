using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VideoBatch.Model; // Reference the model project
// using NodaTime; // Uncomment if used in Model
// using NodaTime.Serialization.SystemTextJson; // Uncomment if used in Model

namespace VideoBatch.Services // Namespace reflects location
{
    /// <summary>
    /// Concrete implementation of IDataService using System.Text.Json.
    /// </summary>
    public class JsonDataService : IDataService // Implement IDataService
    {
        private readonly ILogger<JsonDataService> _logger;
        private Account? _cachedAccount; // Field is nullable
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService(ILogger<JsonDataService> logger)
        {
            _logger = logger;
            _cachedAccount = null;

            // Configure JSON options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            // Uncomment if NodaTime is used in VideoBatch.Model
            // _jsonOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            _logger.LogInformation("JsonDataService initialized.");
        }

        /// <inheritdoc />
        public async Task<Account> LoadDataAsync(string filePath)
        {
            _logger.LogInformation("Attempting to load data from {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogError("Data file not found: {FilePath}", filePath);
                throw new FileNotFoundException("Data file not found.", filePath);
            }

            try
            {
                await using FileStream fs = File.OpenRead(filePath);
                _cachedAccount = await JsonSerializer.DeserializeAsync<Account>(fs, _jsonOptions);
                _logger.LogInformation("Successfully loaded and deserialized data from {FilePath}. Account: {AccountName}", filePath, _cachedAccount?.Name);
                return _cachedAccount ?? throw new JsonException("Deserialization resulted in a null Account object.");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize JSON data from {FilePath}", filePath);
                _cachedAccount = null; // Ensure cache is clear on error
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while loading data from {FilePath}", filePath);
                _cachedAccount = null; // Ensure cache is clear on error
                throw; // Re-throw the exception
            }
        }

        /// <inheritdoc />
        public Task<Account> GetAccountAsync()
        {
            if (_cachedAccount == null)
            {
                _logger.LogWarning("GetAccountAsync called before data was loaded.");
                throw new InvalidOperationException("Data has not been loaded. Call LoadDataAsync first.");
            }
            return Task.FromResult(_cachedAccount);
        }

        /// <inheritdoc />
        public Task<Primitive?> GetPrimitiveByIdAsync(Guid id) // Nullable return type
        {
            if (_cachedAccount == null)
            {
                _logger.LogWarning("GetPrimitiveByIdAsync called before data was loaded.");
                throw new InvalidOperationException("Data has not been loaded. Call LoadDataAsync first.");
            }

            _logger.LogDebug("Searching for Primitive with ID: {PrimitiveId}", id);

            // Check Account level
            if (_cachedAccount.ID == id)
            {
                _logger.LogDebug("Found Primitive ID {PrimitiveId} at Account level: {AccountName}", id, _cachedAccount.Name);
                return Task.FromResult<Primitive?>(_cachedAccount);
            }

            // Search Teams
            var teamQuery = from team in _cachedAccount.Teams ?? Enumerable.Empty<Team>()
                            where team.ID.Equals(id)
                            select team as Primitive;

            var result = teamQuery.FirstOrDefault();
            if (result != null)
            {
                _logger.LogDebug("Found Primitive ID {PrimitiveId} at Team level: {TeamName}", id, result.Name);
                return Task.FromResult<Primitive?>(result);
            }

            // Search Projects
            var projectQuery = from team in _cachedAccount.Teams ?? Enumerable.Empty<Team>()
                               from project in team.Projects ?? Enumerable.Empty<Project>()
                               where project.ID.Equals(id)
                               select project as Primitive;

            result = projectQuery.FirstOrDefault();
            if (result != null)
            {
                _logger.LogDebug("Found Primitive ID {PrimitiveId} at Project level: {ProjectName}", id, result.Name);
                return Task.FromResult<Primitive?>(result);
            }

            // Search Jobs
            var jobQuery = from team in _cachedAccount.Teams ?? Enumerable.Empty<Team>()
                           from project in team.Projects ?? Enumerable.Empty<Project>()
                           from job in project.Jobs ?? Enumerable.Empty<Job>()
                           where job.ID.Equals(id)
                           select job as Primitive;

            result = jobQuery.FirstOrDefault();
            if (result != null)
            {
                _logger.LogDebug("Found Primitive ID {PrimitiveId} at Job level: {JobName}", id, result.Name);
                return Task.FromResult<Primitive?>(result);
            }

            // Search JobTasks
            var jobTaskQuery = from team in _cachedAccount.Teams ?? Enumerable.Empty<Team>()
                               from project in team.Projects ?? Enumerable.Empty<Project>()
                               from job in project.Jobs ?? Enumerable.Empty<Job>()
                               from jobTask in job.Tasks ?? Enumerable.Empty<JobTask>()
                               where jobTask.ID.Equals(id)
                               select jobTask as Primitive;

            result = jobTaskQuery.FirstOrDefault();
            if (result != null)
            {
                _logger.LogDebug("Found Primitive ID {PrimitiveId} at JobTask level: {JobTaskName}", id, result.Name);
                return Task.FromResult<Primitive?>(result);
            }

            // TODO: Adapt Extension searching from Fulfilled if/when Extensions are added to VideoBatch.Model.Account

            _logger.LogWarning("Primitive with ID {PrimitiveId} not found in cached data.", id);
            return Task.FromResult<Primitive?>(null); // Return null Task if not found anywhere
        }
    }
} 