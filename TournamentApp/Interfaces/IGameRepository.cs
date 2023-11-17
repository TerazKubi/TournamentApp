using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IGameRepository
    {
        List<Game> GetGames();
        Game GetGame(int id);
        bool CreateGame(Game game);
        bool UpdateGame(Game game);
        bool DeleteGame(Game game);
        bool GameExists(int id);
    }
}
