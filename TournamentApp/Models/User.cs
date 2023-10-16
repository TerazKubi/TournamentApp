namespace TournamentApp.Models
{

    public enum UserRole { User, Player, Team};

    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        public UserRole Role { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordHash { get; set; }

        public DateTime LastLogin {  get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
