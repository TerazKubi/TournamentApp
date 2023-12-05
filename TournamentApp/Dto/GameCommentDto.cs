using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class GameCommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorId { get; set; }
        public UserNoDetailsDto Author { get; set; }
        public int GameId { get; set; }
    }
}
