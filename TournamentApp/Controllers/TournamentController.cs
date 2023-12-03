using AutoMapper;
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
    [Route("api/Tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public TournamentController(IMapper mapper, ITournamentRepository tournamentRepository,
            ITeamRepository teamRepository, IOrganizerRepository organizerRepository, IGameRepository gameRepository)
        {
            _mapper = mapper;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
            _organizerRepository = organizerRepository;
            _gameRepository = gameRepository;
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
        [ProducesResponseType(200, Type = typeof(GameNoDetailsDto))]
        [ProducesResponseType(400)]
        public IActionResult GetTournamentRootGame(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            var rootGame = _mapper.Map<GameNoDetailsDto>(_tournamentRepository.GetTournamentRootGame(tournamentId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rootGame);
        }

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

            //check if all teams exists
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

            

            

            var depth = (int)Math.Ceiling(Math.Log2(tournamentMap.Teams.ToList().Count));

            var rootGame = BuildTreeRecursive(depth, tournamentMap.Id);

            AssignTeamsForLeafNodes(rootGame, tournamentCreate.teamsIdList);

            //PrintTree(rootGame, 0);

            
            if (rootGame != null)
            {
                Console.WriteLine("Controller: Saving game...");
                Console.WriteLine("Parent: " + rootGame.Parent);
                _gameRepository.CreateGame(rootGame);
            }

            //check for early advance if team numbers were odd
            var gameWithOneTeam = _tournamentRepository.GetTournamentGameWithOneTeamAsigned(tournamentMap.Id);
            if(gameWithOneTeam != null)
            {
                var teamId = gameWithOneTeam.Team1Id ?? gameWithOneTeam.Team2Id;
                if (_gameRepository.GameExists((int)gameWithOneTeam.ParentId))
                {
                    var parentGame = _gameRepository.GetGame((int)gameWithOneTeam.ParentId);
                    parentGame.Team1Id = teamId;
                    if (!_gameRepository.UpdateGame(parentGame))
                    {
                        ModelState.AddModelError("errorMessage", "Error while updating game while creating a bracket. Team id with error: "+parentGame.Id);
                        return BadRequest(ModelState);
                    }
                }
            }


            return Ok("Successfully created");
        }

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

            var tournamentToDelete = _tournamentRepository.GetTournament(tournamentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_tournamentRepository.DeleteTournament(tournamentToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting tournament");
            }

            return NoContent();
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
