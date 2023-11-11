using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetById(int id);
        bool CreateUser(User user);
        bool UserExists(int userId);
    }
}
