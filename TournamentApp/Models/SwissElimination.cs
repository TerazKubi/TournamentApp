namespace TournamentApp.Models
{
    public class SwissElimination
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int Points { get; set; }
        public bool HasPause { get; set; }
    }
}
