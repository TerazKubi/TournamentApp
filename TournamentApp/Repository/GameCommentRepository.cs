using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class GameCommentRepository : IGameCommentRepository
    {
        private readonly DataContext _context;
        public GameCommentRepository(DataContext context)
        {
            _context = context;
        }

        public bool GameCommentExist(int gameCommentId)
        {
            return _context.GameComments.Any(gameComment => gameComment.Id == gameCommentId);
        }

        public bool CreateGameComment(GameComment gameComment)
        {
            _context.GameComments.Add(gameComment);
            return Save();
        }

        public bool DeleteGameComment(GameComment gameComment)
        {
            _context.Remove(gameComment);
            return Save();
        }

        public List<GameComment> GetGameCommentsByGameId(int gameId)
        {
            return _context.GameComments
                .Where(gameComment => gameComment.GameId == gameId).Include(gameComment => gameComment.Author).ToList();
        }

        public bool UpdateGameComment(GameComment gameComment)
        {
            _context.Update(gameComment);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public GameComment GetGameCommentById(int gameCommentId)
        {
            return _context.GameComments
                .Where(gameComment => gameComment.Id == gameCommentId).FirstOrDefault();
        }
    }
}
