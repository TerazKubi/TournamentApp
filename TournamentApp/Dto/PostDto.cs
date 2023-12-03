using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        //=========================================================================
        //public int AuthorId { get; set; }
        public UserNoDetailsDto Author {  get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
