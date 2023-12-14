using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult GetTournamentRootGame(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            var rootGame = _mapper.Map<GameNode>(_tournamentRepository.GetTournamentRootGame(tournamentId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rootGame);
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
            tournamentMap.Teams = _teamRepository.GetTeamsFromList(tournamentCreate.teamsIdList);

            if (!_tournamentRepository.CreateTournament(tournamentMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating tournament");
                return StatusCode(500, ModelState);
            }

            
            if(tournamentMap.EliminationAlgorithm == EliminationTypes.SingleElimination)
            {
                if(!InitSingleEliminationTournament(tournamentMap, tournamentCreate.teamsIdList))
                {
                    ModelState.AddModelError("errorMessage", "Error while updating game while creating a bracket.");
                    return BadRequest(ModelState);
                }
            } 
            else if (tournamentMap.EliminationAlgorithm == EliminationTypes.DoubleElimination)
            {

            }
            else if (tournamentMap.EliminationAlgorithm == EliminationTypes.SwissElimination)
            {
                if(!InitSwissEliminationTournament(tournamentMap, (int)tournamentCreate.SwissRounds))
                {
                    ModelState.AddModelError("errorMessage", "Error while initializing swiss elimination tournamnet.");
                    return BadRequest(ModelState);
                }
            }


            //var depth = (int)Math.Ceiling(Math.Log2(tournamentMap.Teams.ToList().Count));

            //var rootGame = BuildTreeRecursive(depth, tournamentMap.Id);

            //AssignTeamsForLeafNodes(rootGame, tournamentCreate.teamsIdList);

            ////PrintTree(rootGame, 0);


            //if (rootGame != null)
            //{
            //    Console.WriteLine("Controller: Saving game...");
            //    Console.WriteLine("Parent: " + rootGame.Parent);
            //    _gameRepository.CreateGame(rootGame);
            //}

            ////check for early advance if team numbers were odd
            //var gameWithOneTeam = _tournamentRepository.GetTournamentGameWithOneTeamAsigned(tournamentMap.Id);
            //if(gameWithOneTeam != null)
            //{
            //    var teamId = gameWithOneTeam.Team1Id ?? gameWithOneTeam.Team2Id;
            //    if (_gameRepository.GameExists((int)gameWithOneTeam.ParentId))
            //    {
            //        var parentGame = _gameRepository.GetGame((int)gameWithOneTeam.ParentId);
            //        parentGame.Team1Id = teamId;
            //        if (!_gameRepository.UpdateGame(parentGame))
            //        {
            //            ModelState.AddModelError("errorMessage", "Error while updating game while creating a bracket. Team id with error: "+parentGame.Id);
            //            return BadRequest(ModelState);
            //        }
            //    }
            //}


            return Ok("Successfully created");
        }

        

        private bool InitSwissEliminationTournament(Tournament tournament, int rounds)
        {
            //prepare swiss table
            List<SwissElimination> swissTable = new List<SwissElimination>();
            foreach (var team in tournament.Teams)
            {
                swissTable.Add(new SwissElimination
                {
                    TournamentId = tournament.Id,
                    TeamId = team.Id,
                });
            }


            return _swissRepository.CreateSwissTableFromList(swissTable);
        }
        private bool InitSingleEliminationTournament(Tournament tournament, List<int> teamsIdList)
        {
            var depth = (int)Math.Ceiling(Math.Log2(tournament.Teams.ToList().Count));

            var rootGame = BuildTreeRecursive(depth, tournament.Id);

            AssignTeamsForLeafNodes(rootGame, teamsIdList);

            //PrintTree(rootGame, 0);


            if (rootGame != null)
            {
                Console.WriteLine("Controller: Saving game...");
                Console.WriteLine("Parent: " + rootGame.Parent);
                _gameRepository.CreateGame(rootGame);
            }

            //check for early advance if team numbers were odd
            var gameWithOneTeam = _tournamentRepository.GetTournamentGameWithOneTeamAsigned(tournament.Id);
            if (gameWithOneTeam != null)
            {
                var teamId = gameWithOneTeam.Team1Id ?? gameWithOneTeam.Team2Id;
                if (_gameRepository.GameExists((int)gameWithOneTeam.ParentId))
                {
                    var parentGame = _gameRepository.GetGame((int)gameWithOneTeam.ParentId);
                    parentGame.Team1Id = teamId;
                    if (!_gameRepository.UpdateGame(parentGame))
                    {
                        //ModelState.AddModelError("errorMessage", "Error while updating game while creating a bracket. Team id with error: " + parentGame.Id);
                        //return BadRequest(ModelState);
                        return false;
                    }
                }
            }

            return true;
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
        private void PrintTree(Game node, int depth)
        {
            if (node == null)
                return;

            // Print right child
            PrintTree(node.Children.ToList()[0], depth + 1);

            // Print current node
            Console.WriteLine($"{new string(' ', depth * 4)}{node.Round}");

            // Print left child
            PrintTree(node.Children.ToList()[1], depth + 1);
        }

        private void AssignTeamsForLeafNodes(Game node, List<int> teamIdList)
        {
            if (node != null)
            {
                if (node.Children.ToList().Count == 0)
                {
                    if(teamIdList.Count > 0)
                    {
                        int randomTeam1IdPosition = randomPositionFromList(teamIdList.Count);
                        node.Team1Id = teamIdList[randomTeam1IdPosition];
                        teamIdList.RemoveAt(randomTeam1IdPosition);
                    }
                    
                    if(teamIdList.Count > 0)
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

        private int randomPositionFromList(int listCount)
        {
            var random = new Random();
            int low = 0;
            int high = listCount - 1;
            return random.Next(low, high);
        }
    }



    
}
