using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ISwissEliminationRepository
    {
        bool CreateSwissTableFromList(List<SwissElimination> swissTable); 
    }
}
