using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class GameDto
    {
        public int Id { get; set; }
        public string KeyCode { get; set; }
        public string State { get; set; }
        public DateTime StartDate { get; set; }

        public int Round { get; set; }
        public int Team1Points { get; set; }
        public int Team2Points { get; set; }
        public int Team1Sets { get; set; }
        public int Team2Sets { get; set; }

        //=========================================================================
        //public List<Team> Teams { get; set; } = new List<Team>();
        //public List<Score> Scores { get; set; } = new List<Score>();
        
        public int TournamentId { get; set; }

        
        public int? Team1Id { get; set; }
        
        public int? Team2Id { get; set; }
    }
}
