using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ISwissEliminationRepository
    {
        bool CreateSwissTableFromList(List<SwissElimination> swissTable);
        bool UpdateSwissTable(SwissElimination swissElimination);
        SwissElimination GetSwissElimination(int tournamentId, int teamId);
    }
}
