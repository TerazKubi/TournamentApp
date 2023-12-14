using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class SwissEliminationRepository : ISwissEliminationRepository
    {
        private readonly DataContext _context;
        public SwissEliminationRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateSwissTableFromList(List<SwissElimination> swissTable)
        {
            _context.AddRange(swissTable);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
