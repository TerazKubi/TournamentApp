using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context) {
            _context = context;
        }

        public bool CommentExist(int commentId)
        {
            return _context.Comments.Any(comment => comment.Id == commentId);
        }

        public bool CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            return Save();
        }

        public bool DeleteComment(Comment comment)
        {
            _context.Remove(comment);
            return Save();
        }


        public Comment GetCommentById(int id)
        {
            return _context.Comments.Where(comment => comment.Id == id).FirstOrDefault();
        }

        public List<Comment> GetComments()
        {
            return _context.Comments.ToList();
        }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            return _context.Comments
                .Include(c => c.Author).ThenInclude(a => a.Organizer)
                .Include(c => c.Author).ThenInclude(a => a.Team)
                .Include(c => c.Author).ThenInclude(a => a.Player)
                .Where(comment => comment.PostId == postId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateComment(Comment comment)
        {
            _context.Update(comment);
            return Save();
        }
    }
}
