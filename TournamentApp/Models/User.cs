using Microsoft.AspNetCore.Identity;

namespace TournamentApp.Models
{

    //public enum UserRole { User, Player, Team};

    public class User : IdentityUser
    {
        //public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        //public string Email { get; set; }
        //public string PasswordHash { get; set; }
        //public DateTime LastLogin {  get; set; }A

        //=========================================================================
        public Team Team { get; set; }
        public int? TeamId { get; set; }
        public Player Player { get; set; }
        public int? PlayerId { get; set; }
        public Organizer Organizer{ get; set; }
        public int? OrganizerId { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<GameComment> GameComments { get; set; } = new List<GameComment>();
    }
}
