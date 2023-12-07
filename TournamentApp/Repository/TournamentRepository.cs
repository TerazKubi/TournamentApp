using Microsoft.EntityFrameworkCore;
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
            return _context.Tournaments.Where(t => t.Id == id)
                .Include(t => t.Teams).Include(t => t.Games).Include(t => t.Organizer).ThenInclude(o => o.User).FirstOrDefault();
        }

        public List<Tournament> GetTournaments()
        {
            return _context.Tournaments.Include(t => t.Organizer).OrderByDescending(t => t.StartDate).ToList();
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

        public List<Tournament> GetTournamentsByOrganizerId(int organizerId)
        {
            return _context.Tournaments.Where(t => t.OrganizerId == organizerId).ToList();
        }

        public Game GetTournamentRootGame(int id)
        {
            var tournament = GetTournament(id);
            var games = tournament.Games;
            return games.OrderByDescending(g => g.Round).FirstOrDefault();
        }

        public Game GetTournamentGameWithOneTeamAsigned(int id)
        {
            var tournament = GetTournament(id);
            var games = tournament.Games;
            return games.Where(g => (g.Team1Id == null && g.Team2Id != null) || (g.Team1Id != null && g.Team2Id == null)).FirstOrDefault();
        }
    }
}
