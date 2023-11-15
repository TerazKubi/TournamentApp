using Microsoft.EntityFrameworkCore.Diagnostics;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly DataContext _context;
        public TournamentRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateTournament(Tournament tournament)
        {
            _context.Tournaments.Add(tournament);
            return Save();
        }

        public bool DeleteTournament(Tournament tournament)
        {
            _context.Remove(tournament);
            return Save();
        }

        public Tournament GetTournament(int id)
        {
            return _context.Tournaments.Where(t => t.Id == id).FirstOrDefault();
        }

        public List<Tournament> GetTournaments()
        {
            return _context.Tournaments.ToList();
        }

        public bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(t => t.Id == id);
        }

        public bool UpdateTournament(Tournament tournament)
        {
            _context.Update(tournament);
            return Save();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
