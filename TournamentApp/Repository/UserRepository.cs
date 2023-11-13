using Microsoft.EntityFrameworkCore.Diagnostics;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateUser(User user)
        {
            //var newUser = new User()
            //{
            //    FirstName = "Test",
            //    LastName = "Test",
            //    Email = "test@gmail.com",
            //    PasswordHash = "pswdhash"
            //};
            Console.WriteLine(user);

            _context.Users.Add(user);
            return Save();
        }

        public User GetUser(int id)
        {
            return _context.Users.Where(user => user.Id == id).FirstOrDefault();
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UserExists(int userId)
        {
            return _context.Users.Any(user => user.Id == userId);
        }

        public bool ValidateUser(string email)
        {
            return _context.Users.Any(user => user.Email == email);
        }
    }
}
