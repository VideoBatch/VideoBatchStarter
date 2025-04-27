using VideoBatch.Model;

namespace VideoBatch.Services
{
    public interface IProjectServices
    {
        Task DeleteJobAsync(Job j);
        Task DeleteTaskAsync(JobTask t);
        Task<Team> GetTeamAsync();
        void Update();
    }
}