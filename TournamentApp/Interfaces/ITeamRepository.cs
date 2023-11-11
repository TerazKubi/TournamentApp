using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ITeamRepository
    {
        List<Team> GetTeams();
        User GetById(int id);
        bool CreateTeam(Team team);
        bool TeamExists(int teamId);
    }
}
