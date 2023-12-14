using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ITournamentRepository
    {
        List<Tournament> GetTournaments();
        List<Tournament> GetTournamentsByOrganizerId(int organizerId);
        List<Tournament> GetTournamentsByTeamId(int teamId);
        Tournament GetTournament(int id);
        Game GetTournamentRootGame(int id);
        Game GetTournamentGameWithOneTeamAsigned(int id);
        bool CreateTournament(Tournament tournament);
        bool UpdateTournament(Tournament tournament);
        bool DeleteTournament(Tournament tournament);
        bool TournamentExists(int id);
    }
}
