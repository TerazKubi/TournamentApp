using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ITeamRepository
    {
        List<Team> GetTeams();
        Team GetById(int id);
        bool CreateTeam(Team team);
        bool UpdateTeam(Team team);
        bool DelteTeam(Team team);
        bool TeamExists(int teamId);
    }
}
