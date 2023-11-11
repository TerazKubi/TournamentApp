namespace TournamentApp.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ShortTeamName { get; set; }
        public string City{ get; set; }
        public string CoachFullName { get; set; }

        //=========================================================================
        public List<Tournament> Tournaments { get; set;} = new List<Tournament>();
        public List<Game> Team1Games { get; set;} = new List<Game>();
        public List<Game> Team2Games { get; set;} = new List<Game>();
        public List<Player > Players { get; set; } = new List<Player>();
        //public List<Score > Scores { get; set; } = new List<Score>();
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
