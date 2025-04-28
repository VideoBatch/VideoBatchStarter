using Microsoft.Extensions.Logging;
using VideoBatch.Model;

namespace VideoBatch.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly ILogger<ProjectServices> _logger;
        private Team _currentTeam;

        public ProjectServices(ILogger<ProjectServices> logger)
        {
            _logger = logger;
            _currentTeam = new Team("Default Team");
            
            // Add some sample data for testing
            var project = new Project("Sample Project");
            var job = new Job("Sample Job");
            project.Jobs.Add(job);
            _currentTeam.Projects.Add(project);
        }

        public Task DeleteJobAsync(Job j)
        {
            _logger.LogInformation($"Deleting job: {j.Name}");
            var project = _currentTeam.Projects.FirstOrDefault(p => p.Jobs.Contains(j));
            if (project != null)
            {
                project.Jobs.Remove(j);
            }
            return Task.CompletedTask;
        }

        public Task DeleteTaskAsync(JobTask t)
        {
            _logger.LogInformation($"Deleting task: {t.Name}");
            foreach (var project in _currentTeam.Projects)
            {
                foreach (var job in project.Jobs)
                {
                    if (job.Tasks.Contains(t))
                    {
                        job.Tasks.Remove(t);
                        return Task.CompletedTask;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task<Team> GetTeamAsync()
        {
            _logger.LogInformation($"Getting team: {_currentTeam.Name}");
            return Task.FromResult(_currentTeam);
        }

        public void Update()
        {
            _logger.LogInformation("Updating project state");
            // In a real implementation, this would persist changes to a database or file
        }

        // Method removed during rollback
        // public Task AddTaskToJobAsync(Job parentJob, JobTask newTask)
        // { ... }
    }
} 