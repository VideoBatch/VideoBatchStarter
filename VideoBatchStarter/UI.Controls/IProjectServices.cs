using VideoBatch.Model;

namespace VideoBatch.UI.Controls
{
    internal interface IProjectServices
    {
        Task DeleteJobAsync(Job j);
        Task DeleteTaskAsync(JobTask t);
        Task<Team> GetTeamAsync();
        void Update();
    }
}