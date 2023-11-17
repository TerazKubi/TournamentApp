using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ITournamentRepository
    {
        List<Tournament> GetTournaments();
        List<Tournament> GetTournamentsByOrganizerId(int organizerId);
        Tournament GetTournament(int id);
        bool CreateTournament(Tournament tournament);
        bool UpdateTournament(Tournament tournament);
        bool DeleteTournament(Tournament tournament);
        bool TournamentExists(int id);
    }
}
