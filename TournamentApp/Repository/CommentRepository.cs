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

        public bool CreateComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Comment GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            throw new NotImplementedException();
        }
    }
}
