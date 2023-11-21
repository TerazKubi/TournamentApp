using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ITeamRepository
    {
        List<Team> GetTeams();
        List<Team> GetTeamsFromList(List<int> teamIdList);
        List<Player> GetPlayers(int teamId);
        Team GetById(int id);
        bool CreateTeam(Team team);
        bool UpdateTeam(Team team);
        bool DelteTeam(Team team);
        bool TeamExists(int teamId);
        bool AllTeamsExists(List<int> teamIdList);
    }
}
