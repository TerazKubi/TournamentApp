using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        //=========================================================================
        
        public UserAsAuthorDto Author { get; set; }
        
        //public int PostId { get; set; }
    }
}
