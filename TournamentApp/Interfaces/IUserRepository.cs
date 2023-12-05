using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetUser(string id);
        bool CreateUser(User user);
        bool DeleteUser(User user);
        bool UpdateUser(User user);
        bool UserExists(string userId);
        bool ValidateUser(string email);
    }
}
