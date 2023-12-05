using Microsoft.AspNetCore.Identity;
using TournamentApp.Data;
using TournamentApp.Models;

namespace TournamentApp
{
    public class Seed
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public Seed(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedDataContextAsync()
        {
            if (_dataContext.Users.Any()) return;

            User user = new()
            {
                Email = "testuser@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "testuser",
                FirstName = "test",
                LastName = "user"
            };
            var result = await _userManager.CreateAsync(user, "TestUser123!");
            if (result.Succeeded)
            {
                Console.WriteLine("SEEDED ONE USER =====================================================================");
            }

        }

    }
}
