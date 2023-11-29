namespace TournamentApp.Input
{
    public class UserCreate
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
    }
    public class UserCreateWrapper
    {
        public UserCreate UserCreate { get; set; }
        public string Password { get; set; }
    }
}
