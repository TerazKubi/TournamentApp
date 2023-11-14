using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IOrganizerRepository
    {
        List<Organizer> GetOrganizers();
        Organizer GetOrganizerById(int organizerId);    
        bool CreateOrganizer(Organizer organizer);
        bool DeleteOrganizer(Organizer organizer);
        bool UpdateOrganizer(Organizer organizer);
        bool OrganizerExists(int organizerId);
    }
}
