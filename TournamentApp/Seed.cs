using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Identity;
using TournamentApp.Data;
using TournamentApp.Input;
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

            Console.WriteLine("SEEDING DATA ...");

            //SEED ROLES ============================================================================
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Organizer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Organizer));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Team))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Team));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Player))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Player));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Referee))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Referee));

            // create test user
            await _userManager.CreateAsync(new User()
            {
                Email = "testuser@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "testuser",
                FirstName = "test",
                LastName = "user"
            }, "TestUser123!"); // <--- Pasword for test user


            // create Admin
            var admin = new User()
            {
                Email = "admin@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "AdminUser",
                FirstName = "admin",
                LastName = "adminn"
            };
            await _userManager.CreateAsync(admin, "AdminPassword123!"); // <-- admin password
            await _userManager.AddToRolesAsync(admin, new List<string>(){ UserRoles.Admin, UserRoles.Organizer, UserRoles.User, UserRoles.Team, UserRoles.Player, UserRoles.Referee });

            //create ref
            var referee = new User()
            {
                Email = "referee@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "RefUser1",
                FirstName = "ref",
                LastName = "referee"
            };
            await _userManager.CreateAsync(referee, "RefPassword123!"); // <-- ref password
            await _userManager.AddToRolesAsync(referee, new List<string>() { UserRoles.Referee, UserRoles.User });


            // create 2 organizers
            var organizerNames = new List<string>() { "Lays", "Pepsi" };

            foreach(var organizerName in organizerNames)
            {
                var organizerUser = new User()
                {
                    Email = $"organizer{organizerName}@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = $"Organizer{organizerName}",
                    FirstName = "organizer",
                    LastName = organizerName
                };

                await _userManager.CreateAsync(organizerUser, $"Organizer{organizerName}123!"); // <-- organizer password
                await _userManager.AddToRolesAsync(organizerUser, new List<string>() { UserRoles.Organizer, UserRoles.User });

                var organizer = new Organizer()
                {
                    Name = organizerName,
                    UserId = organizerUser.Id
                };
                _dataContext.Organizers.Add(organizer);
                _dataContext.SaveChanges();

                organizerUser.OrganizerId = organizer.Id;
                organizerUser.Organizer = organizer;
                _dataContext.Update(organizerUser);
                _dataContext.SaveChanges();
            }





            //// create Teams
            var teamsList = new List<(string TeamName, string ShortTeamName, string City, string CoachFullName)> {
                ("Uniwersytet Warszawski", "UW", "Warszawa", "Jan Kowalski"),
                ("Politechnika Warszawska", "PW", "Warszawa", "Maria Nowak"),
                ("Uniwersytet Jagielloński w Krakowie", "UJ", "Kraków", "Piotr Wiśniewski"),
                ("Akademia Górniczo-Hutnicza w Krakowie", "AGH", "Kraków", "Anna Dąbrowska"),
                ("Uniwersytet Adama Mickiewicza w Poznaniu", "UAM", "Poznań", "Krzysztof Jankowski"),
                ("Politechnika Poznańska", "PP", "Poznań", "Agnieszka Kowalczyk"),
                ("Uniwersytet Wrocławski", "UWr", "Wrocław", "Tomasz Lewandowski"),
                ("Politechnika Wrocławska", "PWr", "Wrocław", "Joanna Kamińska"),
                ("Uniwersytet Łódzki", "UŁ", "Łódź", "Mateusz Kowalik"),
                ("Politechnika Łódzka", "PŁ", "Łódź", "Monika Zielińska"),
                ("Uniwersytet Śląski w Katowicach", "US", "Katowice", "Grzegorz Szymański"),
                ("Politechnika Śląska", "PŚ", "Gliwice", "Dorota Woźniak"),
                ("Uniwersytet Technologiczno-Przyrodniczy w Bydgoszczy", "UTP", "Bydgoszcz", "Marcin Kozłowski"),
                ("Uniwersytet Mikołaja Kopernika w Toruniu", "UMK", "Toruń", "Magdalena Jóźwiak"),
                ("Uniwersytet Warmińsko-Mazurski w Olsztynie", "UWM", "Olsztyn", "Andrzej Kwiatkowski"),
                ("Akademia Sztuk Pięknych w Warszawie", "ASP", "Warszawa", "Ewa Kaczmarek"),
                ("Akademia Muzyczna im. Fryderyka Chopina w Warszawie", "AMFC", "Warszawa", "Rafał Mazur"),
                ("Uniwersytet Ekonomiczny w Krakowie", "UEK", "Kraków", "Marta Kowalik"),
                ("Szkoła Główna Handlowa w Warszawie", "SGH", "Warszawa", "Łukasz Pawlak"),
                ("Uniwersytet Medyczny im. Karola Marcinkowskiego w Poznaniu", "UMP", "Poznań", "Gabriela Sikora"),
                ("Uniwersytet Medyczny w Białymstoku", "UMB", "Białystok", "Robert Krajewski"),
                ("Politechnika Białostocka", "PB", "Białystok", "Julia Sosnowska"),
                ("Uniwersytet Przyrodniczy w Poznaniu", "UPP", "Poznań", "Artur Głowacki"),
                ("Uniwersytet Rolniczy im. Hugona Kołłątaja w Krakowie", "UR", "Kraków", "Patrycja Jastrzębska"),
                ("Uniwersytet Opolski", "UO", "Opole", "Daniel Piotrowski"),
                ("Uniwersytet Rzeszowski", "URz", "Rzeszów", "Aleksandra Białek"),
                ("Uniwersytet Kazimierza Wielkiego w Bydgoszczy", "UKW", "Bydgoszcz", "Adrian Sokołowski"),
                ("Uniwersytet Jana Kochanowskiego w Kielcach", "UJK", "Kielce", "Monika Krajewska"),
                ("Akademia Techniczno-Humanistyczna w Bielsku-Białej", "ATH", "Bielsko-Biała", "Radosław Wróbel"),
                ("Uniwersytet Trzeciego Wieku w Rzeszowie", "UTW", "Rzeszów", "Joanna Malinowska"),
                ("Uniwersytet Gdański", "UG", "Gdańsk", "Wojciech Nowicki"),
                ("Politechnika Gdańska", "PG", "Gdańsk", "Małgorzata Kowalczyk"),
            };

            for (int i = 0; i< teamsList.Count; i++)
            {
                var teamUser = new User()
                {
                    Email = $"TeamUser{i}@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = $"TeamUser{i}",
                    FirstName = $"Team{i}{teamsList[i].ShortTeamName}",
                    LastName = $"Team{i}{teamsList[i].ShortTeamName}"
                };

                await _userManager.CreateAsync(teamUser, $"Team{teamsList[i].ShortTeamName}123!");
                await _userManager.AddToRoleAsync(teamUser, UserRoles.Team);

                var team = new Team()
                {
                    TeamName = teamsList[i].TeamName,
                    ShortTeamName = teamsList[i].ShortTeamName,
                    City = teamsList[i].City,
                    CoachFullName = teamsList[i].CoachFullName,
                    UserId = teamUser.Id,
                };

                _dataContext.Teams.Add( team );
                _dataContext.SaveChanges();

                teamUser.TeamId = team.Id;
                teamUser.Team = team;
                _dataContext.Update(teamUser);
                _dataContext.SaveChanges();
            }





            List<string> names = new List<string>()
            {
                "Adam", "Adrian", "Aleksander", "Bartłomiej", "Dominik", "Filip", "Grzegorz", "Hubert", "Jacek", "Kamil",
                "Krzysztof", "Łukasz", "Marcin", "Michał", "Paweł", "Piotr", "Robert", "Sebastian", "Tomasz", "Wojciech", "Jakub", "Patryk", "Mateusz"
            };
            List<string> surnames = new List<string>()
            {
                "Nowak", "Kowalski", "Wiśniewski", "Dąbrowski", "Jankowski", "Kowalczyk", "Lewandowski", "Kamiński", "Kowalik", "Zieliński",
                "Szymański", "Woźniak", "Kozłowski", "Jóźwiak", "Kwiatkowski", "Kaczmarek", "Mazur", "Kowalik", "Pawlak", "Sikora",
                "Krajewski", "Sosnowski", "Głowacki", "Jastrzębski", "Piotrowski", "Białek", "Sokołowski", "Krajewski", "Wróbel", "Malinowski"
            };
            List<string> positions = new List<string>()
            {
                "Przyjmujący","Atakujący", "Środkowy", "Atakujący", "Rozgrywający", "Libero"
            };

            // create players

            var teams = _dataContext.Teams.ToList();
            Random random = new Random();
            foreach (var team in teams)
            {
                for(int i = 0; i < 6; i++)
                {
                    var playerUser = new User()
                    {
                        Email = $"PlayerUser{i}@gmail.com",
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = $"PlayerUser{i}{team.Id}",
                        FirstName = names[random.Next(names.Count)],
                        LastName = surnames[random.Next(surnames.Count)]
                    };

                    await _userManager.CreateAsync(playerUser, $"Player{team.ShortTeamName}123!");
                    await _userManager.AddToRoleAsync(playerUser, UserRoles.Player);

                    var player = new Player()
                    {
                        Number = random.Next(0, 99),
                        Position = positions[i],
                        TeamId = team.Id,
                        UserId = playerUser.Id
                    };

                    _dataContext.Players.Add(player);
                    _dataContext.SaveChanges();

                    playerUser.PlayerId = player.Id;
                    playerUser.Player = player;
                    _dataContext.Update(playerUser);
                    _dataContext.SaveChanges();
                }
                
                
            }

            // create Posts

            var teamPostTexts = new List<string>() { 
                "Drużyna odbyła właśnie bardzo cięzki trening.", "Zawodnicy w formie, oby tak dalej!", "Finał będzie nasz!!", "Następnym razem wygramy.."
            };
            
            var teamsToPost = _dataContext.Teams.Take(teamPostTexts.Count).ToList();
            for (int i = 0; i < teamPostTexts.Count; i++)
            {
                var post = new Post()
                {
                    Text = teamPostTexts[i],
                    AuthorId = teamsToPost[i].UserId
                };
                _dataContext.Posts.Add(post);
                
            }
            _dataContext.SaveChanges();
            

            // create Comments
            var posts = _dataContext.Posts.ToList();
            var usersToComment = _dataContext.Users.Take(posts.Count).ToList();

            var commentTexts = new List<string>() { ":)", "Oby tak dalej", "Powodzenia", "Pozdrawiam", "c:", "Znakomita gra", "#TEAMWORK", "Idziemy po puchar!",
                "Dzisiejszy mecz był super", "Niezły mecz", "Wspaniałe zwycięstwo"};

            for (int i = 0;i < posts.Count;i++)
            {
                var post = posts[i];
                for (int j = 0; j < 3; j++)
                {
                    var author = usersToComment[random.Next(usersToComment.Count)];
                    var comment = new Comment()
                    {
                        Text = commentTexts[random.Next(commentTexts.Count)],
                        AuthorId = author.Id,
                        PostId = post.Id,
                    };

                    _dataContext.Comments.Add(comment);
                    
                }

            }
            _dataContext.SaveChanges();


            // create tournament

            var organizerT = _dataContext.Organizers.First();
            int teamCount = 8;
            var tournament = new Tournament()
            {
                Name = organizerT.Name + " tournament",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                City = "Poznań",
                TeamCount = teamCount,
                EliminationAlgorithm = EliminationTypes.SingleElimination,
                OrganizerId = organizerT.Id,
            };

            tournament.Teams = _dataContext.Teams.Take(teamCount).ToList();

            _dataContext.Tournaments.Add(tournament);
            _dataContext.SaveChanges();

            var depth = (int)Math.Ceiling(Math.Log2(teamCount));
            var teamIdList = new List<int>();
            foreach (var team in tournament.Teams)
            {
                teamIdList.Add(team.Id);
            }
            var rootGame = BuildTreeRecursive(depth, tournament.Id);

            AssignTeamsForLeafNodes(rootGame, teamIdList);

            _dataContext.Games.Add(rootGame);
            _dataContext.SaveChanges();


        }

        private Game BuildTreeRecursive(int depth, int tournamentId, Game parentGame = null)
        {
            if (depth == 0)
            {
                return null;
            }

            var game = new Game
            {
                KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                TournamentId = tournamentId,
                Parent = parentGame,
                Round = depth

            };


            var left = BuildTreeRecursive(depth - 1, tournamentId, game);
            if (left != null) game.Children.Add(left);

            var right = BuildTreeRecursive(depth - 1, tournamentId, game);
            if (right != null) game.Children.Add(right);

            return game;
        }
        private int randomPositionFromList(int listCount)
        {
            var random = new Random();
            int low = 0;
            int high = listCount - 1;
            return random.Next(low, high);
        }
        private void AssignTeamsForLeafNodes(Game node, List<int> teamIdList)
        {
            if (node != null)
            {
                if (node.Children.ToList().Count == 0)
                {
                    if (teamIdList.Count > 0)
                    {
                        int randomTeam1IdPosition = randomPositionFromList(teamIdList.Count);
                        node.Team1Id = teamIdList[randomTeam1IdPosition];
                        teamIdList.RemoveAt(randomTeam1IdPosition);
                    }

                    if (teamIdList.Count > 0)
                    {
                        int randomTeam2IdPosition = randomPositionFromList(teamIdList.Count);
                        node.Team2Id = teamIdList[randomTeam2IdPosition];
                        teamIdList.RemoveAt(randomTeam2IdPosition);
                    }


                }

                foreach (var game in node.Children.ToList())
                {
                    AssignTeamsForLeafNodes(game, teamIdList);
                }



            }
        }

    }
}
