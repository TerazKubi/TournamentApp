namespace TournamentApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        //public string Title { get; set; }
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public User Author { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
