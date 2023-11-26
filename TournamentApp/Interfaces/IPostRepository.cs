using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IPostRepository
    {
        List<Post> GetPosts();
        List<Post> GetPostsByUserId(int userId);
        List<Post> GetTeamPosts(int teamId);
        List<Post> GetPlayerPosts(int playerId);
        Post GetPostById(int id);
        bool CreatePost(Post post);
        bool DeletePost(Post post);
        bool UpdatePost(Post post);
        bool PostExists(int postId);
    }
}
