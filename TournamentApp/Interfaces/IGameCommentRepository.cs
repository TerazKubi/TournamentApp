using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IGameCommentRepository
    {
        List<GameComment> GetGameCommentsByGameId(int gameId);
        GameComment GetGameCommentById(int gameCommentId);
        bool CreateGameComment(GameComment gameComment);
        bool DeleteGameComment(GameComment gameComment);
        bool UpdateGameComment(GameComment gameComment);

        bool GameCommentExist(int gameCommentId);
    }
}
