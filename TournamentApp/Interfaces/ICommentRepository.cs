using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByPostId(int postId);
        Comment GetById(int id);
        bool CreateComment(Comment comment);
        
    }
}
