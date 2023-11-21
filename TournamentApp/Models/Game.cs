namespace TournamentApp.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string KeyCode { get; set; }
        public string State { get; set; }
        public DateTime? StartDate { get; set; }

        public int Round { get; set; }
        public int Team1Points { get; set; }
        public int Team2Points { get; set; }
        public int Team1Sets { get; set; }
        public int Team2Sets { get; set; }

        //=========================================================================
        //public List<Team> Teams { get; set; } = new List<Team>();
        //public List<Score> Scores { get; set; } = new List<Score>();
        public Tournament Tournament { get; set; }
        public int TournamentId { get; set; }

        public Team Team1 { get; set; }
        public int? Team1Id { get; set;}
        public Team Team2 { get; set; }
        public int? Team2Id { get; set;}

    }
}
