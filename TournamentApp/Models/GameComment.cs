namespace TournamentApp.Models
{
    public class GameComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
