namespace TournamentApp.Input
{
    public class GameCreate
    {
        public string KeyCode { get; set; }
        public DateTime? StartDate { get; set; }



        //=========================================================================
        //public List<Team> Teams { get; set; } = new List<Team>();
        //public List<Score> Scores { get; set; } = new List<Score>();

        public int TournamentId { get; set; }


        public int? Team1Id { get; set; }

        public int? Team2Id { get; set; }
    }
}
