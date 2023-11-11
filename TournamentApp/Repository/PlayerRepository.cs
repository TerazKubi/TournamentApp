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
            throw new NotImplementedException();
        }

        public Player GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Player> GetPlayers()
        {
            throw new NotImplementedException();
        }

        public bool PlayerExists(int playerId)
        {
            throw new NotImplementedException();
        }
    }
}
