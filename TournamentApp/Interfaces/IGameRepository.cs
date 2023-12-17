using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IGameRepository
    {
        List<Game> GetGames();
        Game GetGame(int id);
        List<Game> GetRoundOneGames(int tournamentId);
        //Game GetTournamentRootGame(int tournamentId);
        bool CreateGame(Game game);
        bool CreateGamesFromList(List<Game> gameList);
        bool UpdateGame(Game game);
        bool UpdateGamesFromList(List<Game> gamesList);
        bool DeleteGame(Game game);
        bool GameExists(int id);


        void CreateWithoutSave(Game game);
        void Save2();
    }
}
