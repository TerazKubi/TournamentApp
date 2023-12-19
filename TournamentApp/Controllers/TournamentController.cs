using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
    [Route("api/Tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISwissEliminationRepository _swissRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public TournamentController(IMapper mapper, ITournamentRepository tournamentRepository,
            ITeamRepository teamRepository, IOrganizerRepository organizerRepository, IGameRepository gameRepository, UserManager<User> userManager, ISwissEliminationRepository swissRepository)
        {
            _mapper = mapper;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
            _organizerRepository = organizerRepository;
            _gameRepository = gameRepository;
            _userManager = userManager;
            _swissRepository = swissRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TournamentNoDetailsDto>))]
        public IActionResult GetTournaments()
        {
            var tournaments = _mapper.Map<List<TournamentNoDetailsDto>>(_tournamentRepository.GetTournaments());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tournaments);
        }

        [HttpGet("{tournamentId}")]
        [ProducesResponseType(200, Type = typeof(TournamentDto))]
        [ProducesResponseType(400)]
        public IActionResult GetTournamnetById(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            var tournament = _mapper.Map<TournamentDto>(_tournamentRepository.GetTournament(tournamentId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tournament);
        }

        [HttpGet("{tournamentId}/rootGame")]
        [ProducesResponseType(200, Type = typeof(GameNode))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetTournamentRootGame(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            var tournament = _tournamentRepository.GetTournament(tournamentId);
            if(tournament.EliminationAlgorithm == EliminationTypes.SwissElimination)
            {
                ModelState.AddModelError("error", "This tournament doesn't support root game");
                return BadRequest(ModelState);
            }

            var rootGame = _mapper.Map<GameNode>(_tournamentRepository.GetTournamentRootGame(tournamentId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rootGame);
        }

        [HttpGet("{tournamentId}/swissTable")]
        [ProducesResponseType(200, Type = typeof(List<SwissElimination>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetTournamentSwissTable(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tournament = _tournamentRepository.GetTournament(tournamentId);
            if (tournament.EliminationAlgorithm != EliminationTypes.SwissElimination)
            {
                ModelState.AddModelError("error", "This tournament doesn't support root game");
                return BadRequest(ModelState);
            }


            var swissTable =_mapper.Map<List<SwissEliminationDto>>(_swissRepository.GetSwissEliminationList(tournamentId));

            return Ok(swissTable);
        }

        [Authorize(Roles = UserRoles.Organizer)]
        [HttpDelete("{tournamentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTournament(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tournamentToDelete = _tournamentRepository.GetTournament(tournamentId);

            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);

            if (!(tournamentToDelete.Organizer.UserId.Equals(currentUserId) || isAdmin)) return Forbid();

            if (!_tournamentRepository.DeleteTournament(tournamentToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting tournament");
            }

            return NoContent();
        }




        [Authorize(Roles = UserRoles.Organizer)]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTournament([FromBody] TournamentCreateWrapper tournamentCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("errorMessage", "modelState error");
                return BadRequest(ModelState);
            }

            if (tournamentCreate.Tournament == null || tournamentCreate.teamsIdList == null)
            {
                ModelState.AddModelError("errorMessage", "tournament or teamIdlist is null");
                return BadRequest(ModelState);
            }

            if(tournamentCreate.teamsIdList.Count == 0)
            {
                ModelState.AddModelError("errorMessage", "Empty teamIdList");
                return BadRequest(ModelState);
            }

            if(tournamentCreate.teamsIdList.Count != tournamentCreate.Tournament.TeamCount)
            {
                ModelState.AddModelError("errorMessage", "Team count is not equal to teamIdList count");
                return BadRequest(ModelState);
            }

            if (!AllUnique(tournamentCreate.teamsIdList))
            {
                ModelState.AddModelError("errorMessage", "Team id list must be all unique");
                return BadRequest(ModelState);
            }

            if(tournamentCreate.Tournament.OrganizerId == 0 ||
                !_organizerRepository.OrganizerExists(tournamentCreate.Tournament.OrganizerId))
            {
                ModelState.AddModelError("errorMessage", "no organizer with given id");
                return BadRequest(ModelState);
            }

            if(tournamentCreate.Tournament.EliminationAlgorithm == EliminationTypes.SwissElimination && tournamentCreate.SwissRounds == null)
            {
                ModelState.AddModelError("errorMessage", "No rounds given for swiss elimination");
                return BadRequest(ModelState);
            }

            if (!_teamRepository.AllTeamsExists(tournamentCreate.teamsIdList))
            {
                ModelState.AddModelError("errorMessage", "One of the team from teamIdList doesnt exist");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("errorMessage", "modelState error");
                return BadRequest(ModelState);
            }

            var tournamentMap = _mapper.Map<Tournament>(tournamentCreate.Tournament);
            var teamList = _teamRepository.GetTeamsFromList(tournamentCreate.teamsIdList);          
            tournamentMap.Teams = teamList;

            if (!_tournamentRepository.CreateTournament(tournamentMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating tournament");
                return StatusCode(500, ModelState);
            }

            

            
            if(tournamentMap.EliminationAlgorithm == EliminationTypes.SingleElimination)
            {
                if (!InitSingleEliminationTournament(tournamentMap, tournamentCreate.teamsIdList))
                {
                    ModelState.AddModelError("errorMessage", "Error while initializing single elimination tournamnet.");
                    return BadRequest(ModelState);
                }
            } 
            else if (tournamentMap.EliminationAlgorithm == EliminationTypes.DoubleElimination)
            {
                if (!InitDoubleEliminationTournament(tournamentMap, tournamentCreate.teamsIdList))
                {
                    ModelState.AddModelError("errorMessage", "Error while initializing double elimination tournamnet.");
                    return BadRequest(ModelState);
                }
            }
            else if (tournamentMap.EliminationAlgorithm == EliminationTypes.SwissElimination)
            {
                if (!InitSwissEliminationTournament(tournamentMap, (int)tournamentCreate.SwissRounds))
                {
                    ModelState.AddModelError("errorMessage", "Error while initializing swiss elimination tournamnet.");
                    return BadRequest(ModelState);
                }
            }


            


            return Ok("Successfully created");
        }

        

        private bool InitSwissEliminationTournament(Tournament tournament, int rounds)
        {
            //prepare swiss table
            List<SwissElimination> swissTable = new List<SwissElimination>();
            var teamList = tournament.Teams.ToList();
            foreach (var team in teamList)
            {
                swissTable.Add(new SwissElimination
                {
                    TournamentId = tournament.Id,
                    TeamId = team.Id,
                });
            }

            if (!_swissRepository.CreateSwissTableFromList(swissTable)) return false;



            //prepare games
            var gameList = new List<Game>();
            int gamesForRound = (int)Math.Floor((double)tournament.Teams.Count / 2);
            for (int i=1; i<= rounds; i++)
            {
                for(int j=0; j<gamesForRound; j++)
                {
                    gameList.Add(new Game
                    {
                        KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                        TournamentId = tournament.Id,
                        Round = i
                    });
                }
            }

            if(!_gameRepository.CreateGamesFromList(gameList)) return false;


            //first round team assignment
            
            Shuffle(teamList);

            var games = tournament.Games.Where(g => g.Round == 1).ToList();

            foreach (var game in games)
            {
                if (!(teamList.Count >= 2)) break;
                
                game.Team1 = teamList[0];
                game.Team1.Id = teamList[0].Id;
                game.Team2 = teamList[1];
                game.Team2.Id = teamList[1].Id;

                teamList.RemoveAt(0);
                teamList.RemoveAt(0);
                
            }
            if (!_gameRepository.UpdateGamesFromList(games)) return false;

            if(teamList.Count == 1)
            {
                var lastTeam = teamList[0];
                var swissTableRow = _swissRepository.GetSwissElimination(tournament.Id, lastTeam.Id);
                swissTableRow.Points += 3;
                swissTableRow.HasPause = true;

                if(!_swissRepository.UpdateSwissTable(swissTableRow)) return false;
            }

            return true;
        }
        private bool InitSingleEliminationTournament(Tournament tournament, List<int> teamsIdList)
        {
            var depth = (int)Math.Ceiling(Math.Log2(tournament.Teams.ToList().Count));

            var rootGame = BuildTreeRecursive(depth, tournament.Id);

            //add root game and all leaf nodes to data base
            if (rootGame != null)
            {
                if (!_gameRepository.CreateGame(rootGame)) return false;
            }


            Shuffle(teamsIdList);

            if (!AssignTeamsForLeafNodes(tournament.Id, teamsIdList)) return false;


            if(!CheckForEarlyAdvance(tournament.Id)) return false;
            

            return true;
        }

        private bool InitDoubleEliminationTournament(Tournament tournament, List<int> teamsIdList)
        {
            var depth = (int)Math.Ceiling(Math.Log2(tournament.Teams.ToList().Count));

            var rootGame = BuildTreeRecursive(depth, tournament.Id);

            var levels = CalculateLevelCounts(teamsIdList.Count);

            var loserRoot = BuildLosersTree(levels, tournament.Id);

            var mainRoot = new Game
            {
                KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                TournamentId = tournament.Id,
                Round = levels.Count + 3
            };

            mainRoot.Children.Add(rootGame);
            mainRoot.Children.Add(loserRoot);


            if (mainRoot != null)
            {
                if (!_gameRepository.CreateGame(mainRoot)) return false;
            }

            //===========================================================

            Shuffle(teamsIdList);

            if (!AssignTeamsForLeafNodes(tournament.Id, teamsIdList)) return false;


            if (!CheckForEarlyAdvance(tournament.Id)) return false;









            Console.WriteLine("\n\n\n Funkcja:-----------------------------------------------------------------");
            PrintTree(mainRoot);
            Console.WriteLine("\n-----------------------------------------------------------------");
            return true;
        }

        private List<int> CalculateLevelCounts(int n)
        {
            int N = n;
            int maxRounds = (int)Math.Ceiling(Math.Log2(N));
            List<int> levels = new List<int>();

            levels.Add(N - (int)Math.Pow(2, maxRounds - 1));

            for (int i = 0; i < maxRounds - 1; i++)
            {
                int los1 = (int)Math.Ceiling(levels.Last() / 2.0);
                int los2 = (int)Math.Pow(2, maxRounds - i - 2);
                levels.Add(los1 + los2);
            }

            while (levels.Last() > 2)
            {
                int los1 = (int)Math.Ceiling(levels.Last() / 2.0);
                levels.Add(los1);
            }

            for (int i = 0; i < levels.Count; i++)
            {
                levels[i] = (int)Math.Floor(levels[i] / 2.0);
            }

            levels.Reverse();
            levels.RemoveAt(0);

            // Print the result
            Console.WriteLine("Levels: [" + string.Join(", ", levels) + "]");
            return levels;
        }

        private Game BuildLosersTree(List<int> levels, int tournamentId)
        {
            var root = new Game
            {
                KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                TournamentId = tournamentId,
                Round = levels.Count + 2,
                IsWinnerTree = 0
            };

            List<Game> currentLevel = new List<Game>();
            currentLevel.Add(root);

            foreach (var x in levels)
            {
                List<Game> nextLevel = new List<Game>();

                var levelCount = x;

                foreach (var parent in currentLevel)
                {
                    if(levelCount > 0)
                    {
                        var child = new Game
                        {
                            KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                            TournamentId = tournamentId,
                            Round = parent.Round - 1,
                            IsWinnerTree = 0
                        };
                        parent.Children.Add(child);
                        nextLevel.Add(child);

                        levelCount -= 1;
                    }
                    if (levelCount > 0)
                    {
                        var child = new Game
                        {
                            KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                            TournamentId = tournamentId,
                            Round = parent.Round - 1,
                            IsWinnerTree = 0
                        };
                        parent.Children.Add(child);
                        nextLevel.Add(child);

                        levelCount -= 1;
                    }



                }

                currentLevel = nextLevel;


            }

            return root;



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
            if(right != null) game.Children.Add(right);

            return game;
        }
        private void PrintTree(Game node)
        {
            if (node == null)
                return;

            Console.WriteLine($"{node.Id}");
            

            if(node.Children.ToList().Count >= 1)
                PrintTree(node.Children.ToList()[0]);

            if (node.Children.ToList().Count == 2)
                PrintTree(node.Children.ToList()[1]);
        }

        private bool AssignTeamsForLeafNodes(int tournamentId, List<int> teamIdList)
        {
            var roundOneGames = _gameRepository.GetRoundOneGames(tournamentId);

            //asign teams to round one games as Team1
            foreach (var game in roundOneGames)
            {
                int position = randomPositionFromList(teamIdList.Count);

                game.Team1Id = teamIdList[position];
                teamIdList.RemoveAt(position);
            }

            //asign teams to round one games as Team2
            foreach (var game in roundOneGames)
            {
                if (teamIdList.Count == 0) break;

                int position = randomPositionFromList(teamIdList.Count);

                game.Team2Id = teamIdList[position];
                teamIdList.RemoveAt(position);
            }

            if(!_gameRepository.UpdateGamesFromList(roundOneGames)) return false;

            return true;
            
           
        }
        private bool CheckForEarlyAdvance(int tournamentId)
        {
            var roundOneGames = _gameRepository.GetRoundOneGames(tournamentId);

            foreach (var game in roundOneGames)
            {
                if ((game.Team1Id.HasValue && game.Team2Id.HasValue)) continue;
                

                var teamId = game.Team1Id ?? game.Team2Id;

                if (!game.ParentId.HasValue) return false;

                var parentGame = game.Parent;

                if (parentGame.Team1Id == null)
                    parentGame.Team1Id = teamId;
                else
                    parentGame.Team2Id = teamId;

                game.State = "finished";

                if(!_gameRepository.UpdateGame(parentGame)) return false;


                
            }
            return true;
        }
        private int randomPositionFromList(int listCount)
        {
            var random = new Random();
            int low = 0;
            int high = listCount - 1;
            //if (low == high) return low;
            return random.Next(low, high);
        }

        private void Shuffle(List<Team> list)
        {
            Random random = new Random();

            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                // Swap elements at positions i and j
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
        private void Shuffle(List<int> list)
        {
            Random random = new Random();

            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                // Swap elements at positions i and j
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private bool AllUnique(List<int> list)
        {
            return list.Distinct().Count() == list.Count; 
        }
    }



    
}
