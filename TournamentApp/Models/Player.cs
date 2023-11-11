namespace TournamentApp.Models
{
    public class Player
    {
        public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public int Number {  get; set; }
        public string Position { get; set; }

        //=========================================================================
        public Team Team { get; set; }
        public int TeamId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
    }
}
