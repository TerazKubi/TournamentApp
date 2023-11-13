using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByPostId(int postId);
        List<Comment> GetComments();
        Comment GetCommentById(int id);
        bool CreateComment(Comment comment);
        bool DeleteComment(Comment comment);

        bool CommentExist(int commentId);


    }
}
