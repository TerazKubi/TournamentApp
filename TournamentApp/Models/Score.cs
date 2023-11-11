namespace TournamentApp.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int SetPoints { get; set; }
        public int Points { get; set; }


        //=========================================================================
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public Game Game { get; set; }
        public int GameId { get; set; }

    }
}
