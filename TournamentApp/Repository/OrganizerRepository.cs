using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly DataContext _context;
        public OrganizerRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateOrganizer(Organizer organizer)
        {
            _context.Organizers.Add(organizer);
            return Save();
        }

        public bool DeleteOrganizer(Organizer organizer)
        {
            _context.Remove(organizer);
            return Save();
        }

        public Organizer GetOrganizerById(int organizerId)
        {
            return _context.Organizers.Where(o => o.Id == organizerId).Include(o => o.User).FirstOrDefault();
        }

        public List<Organizer> GetOrganizers()
        {
            return _context.Organizers.ToList();
        }

        public bool OrganizerExists(int organizerId)
        {
            return _context.Organizers.Any(o => o.Id == organizerId);
        }

        public bool UpdateOrganizer(Organizer organizer)
        {
            _context.Update(organizer);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
