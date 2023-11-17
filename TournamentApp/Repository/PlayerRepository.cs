using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DataContext _context;
        public PlayerRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreatePlayer(Player player)
        {
            _context.Players.Add(player);
            return Save();
        }

        public bool DeletePlayer(Player player)
        {
            _context.Remove(player);
            return Save();
        }

        public Player GetById(int id)
        {
            return _context.Players.Where(p => p.Id == id).FirstOrDefault();
        }

        public List<Player> GetPlayers()
        {
            return _context.Players.Include(p => p.User).ToList();
        }

        public bool PlayerExists(int playerId)
        {
            return _context.Players.Any(p => p.Id == playerId);
        }

        public bool UpdatePlayer(Player player)
        {
            _context.Update(player);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
