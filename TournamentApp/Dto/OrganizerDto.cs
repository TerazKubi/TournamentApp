using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserDto User { get; set; }
        public string UserId { get; set; }
    }

    public class OrganizerAuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
