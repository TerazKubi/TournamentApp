using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        public PostRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreatePost(Post post)
        {
            _context.Posts.Add(post);
            return Save();
        }

        public bool DeletePost(Post post)
        {
            _context.Remove(post);
            return Save();
        }

        public Post GetPostById(int id)
        {
            return _context.Posts.Where(post => post.Id == id).FirstOrDefault();
        }

        public List<Post> GetPosts()
        {
            
            return _context.Posts.Include(p => p.Comments).ToList();
        }

        public List<Post> GetPostsByUserId(int userId)
        {
            return _context.Posts.Where(post => post.AuthorId == userId).ToList();
        }

        public bool PostExists(int postId)
        {
            return _context.Posts.Any(post => post.Id == postId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
