namespace TournamentApp.Input
{
    public class ResetPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
