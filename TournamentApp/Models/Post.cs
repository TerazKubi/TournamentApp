namespace TournamentApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        //=========================================================================
        public User Author { get; set; }
        public string AuthorId { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
