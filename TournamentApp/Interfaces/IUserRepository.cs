using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetUser(int id);
        bool CreateUser(User user);
        bool DeleteUser(User user);
        bool UpdateUser(User user);
        bool UserExists(int userId);
        bool ValidateUser(string email);
    }
}
