using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    
    public class GameRepository : IGameRepository
    {
        private readonly DataContext _context;
        public GameRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateGame(Game game)
        {
            _context.Games.Add(game);
            return Save();
        }

        public bool DeleteGame(Game game)
        {
            _context.Remove(game);
            return Save();
        }

        public bool GameExists(int id)
        {
            return _context.Games.Any(g => g.Id == id);
        }

        public Game GetGame(int id)
        {
            return _context.Games.Where(g => g.Id == id).Include(g => g.Team1).Include(g => g.Team2).Include(g => g.Tournament)
                .ThenInclude(t => t.Organizer).FirstOrDefault();
        }

        public List<Game> GetGames()
        {
            return _context.Games.Include(g => g.Team1).Include(g => g.Team2).ToList();
        }

        public bool UpdateGame(Game game)
        {
            _context.Update(game);
            return Save();
        }
        public void CreateWithoutSave(Game game)
        {
            _context.Games.Add(game);
        }
        public void Save2()
        {
            _context.SaveChanges();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool CreateGamesFromList(List<Game> gameList)
        {
            _context.AddRange(gameList);
            return Save();
        }

        public bool UpdateGamesFromList(List<Game> gamesList)
        {
            _context.UpdateRange(gamesList);
            return Save();
        }

        //public Game GetTournamentRootGame(int tournamentId)
        //{
        //    return _context.Games.Where(g => g.TournamentId == tournamentId).Include(g => g.Children).OrderByDescending(g => g.Round).FirstOrDefault();
        //}
    }
}
