namespace TournamentApp.Models
{
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public List<Tournament> Tournaments { get; set;} = new List<Tournament>();
    }
}
