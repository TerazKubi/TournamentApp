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

        public SwissElimination GetSwissElimination(int tournamentId, int teamId)
        {
            return _context.SwissEliminations.Where(e => e.TournamentId == tournamentId &&  e.TeamId == teamId).FirstOrDefault();
        }

        public List<SwissElimination> GetSwissEliminationList(int tournamentId)
        {
            return _context.SwissEliminations.Where(e => e.TournamentId == tournamentId).OrderByDescending(e => e.Points).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateSwissTable(SwissElimination swissElimination)
        {
            _context.Update(swissElimination);
            return Save();
        }
    }
}
