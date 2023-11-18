namespace TournamentApp.Input
{
    public class CommentCreate
    {
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int PostId { get; set; }
    }
}
