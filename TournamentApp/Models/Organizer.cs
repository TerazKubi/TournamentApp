namespace TournamentApp.Models
{
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public List<Tournament> Tournaments { get; set;} = new List<Tournament>();

        public User User { get; set; }
        public string UserId { get; set; }
    }
}
