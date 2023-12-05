using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
    [Route("api/Games")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IGameCommentRepository _gameCommentRepository;
        private readonly IMapper _mapper;

        public GameController(IMapper mapper, IGameRepository gameRepository,
            ITournamentRepository tournamentRepository, ITeamRepository teamRepository, IGameCommentRepository gameCommentRepository)
        {
            _mapper = mapper;
            _gameRepository = gameRepository;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
            _gameCommentRepository = gameCommentRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<GameNoDetailsDto>))]
        public IActionResult GetGames()
        {
            var games = _mapper.Map<List<GameNoDetailsDto>>(_gameRepository.GetGames());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(games);
        }

        [HttpGet("{gameId}")]
        [ProducesResponseType(200, Type = typeof(GameDto))]
        [ProducesResponseType(400)]
        public IActionResult GetGameById(int gameId)
        {
            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            var game = _mapper.Map<GameDto>(_gameRepository.GetGame(gameId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(game);
        }

        [HttpGet("{gameId}/gameComments")]
        [ProducesResponseType(200, Type = typeof(List<GameCommentDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetGameCommentByGameId(int gameId)
        {
            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            var gameComments = _mapper.Map<List<GameCommentDto>>(_gameCommentRepository.GetGameCommentsByGameId(gameId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(gameComments);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateGame([FromBody] GameCreate gameCreate)
        {

            if (gameCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_tournamentRepository.TournamentExists(gameCreate.TournamentId))
                return BadRequest(ModelState);


            // validate here like in updategame
            if (gameCreate.Team1Id != null && !_teamRepository.TeamExists((int)gameCreate.Team1Id))
            {
                ModelState.AddModelError("errorMessage", "team 1 doesnt exists");
                return BadRequest(ModelState);
            }

            if (gameCreate.Team2Id != null && !_teamRepository.TeamExists((int)gameCreate.Team2Id))
            {
                ModelState.AddModelError("errorMessage", "team 2 doesnt exists");
                return BadRequest(ModelState);
            }




            var gameMap = _mapper.Map<Game>(gameCreate);

            if (!_gameRepository.CreateGame(gameMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating game");
                return StatusCode(500, ModelState);
            }


            return Ok("Successfully created");
        }

        [HttpPut("{gameId}/Addpoint/{teamId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult AddPoint(int gameId, int teamId)
        {
            if(!_gameRepository.GameExists(gameId) || !_teamRepository.TeamExists(teamId)) 
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest();

            var game = _gameRepository.GetGame(gameId);
            if (game.Team1Id != teamId && game.Team2Id != teamId)
                return BadRequest();

            if (game.State == "finished")
                return BadRequest();

            if(game.Team1Id == null || game.Team2Id == null)
                return BadRequest();

            if (game.State == "awaited") game.State = "ongoing";

            var tournament = _tournamentRepository.GetTournament(game.TournamentId);
            if(tournament != null)
            {
                if (tournament.State == "awaited") tournament.State = "ongoing";
                if(!_tournamentRepository.UpdateTournament(tournament))
                {
                    ModelState.AddModelError("", "Something went wrong updating tournament");
                    return StatusCode(500, ModelState);
                }
            }

            switch (game.CurrentSet)
            {
                case 1:
                    game.Set1Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set1Team2Points += (game.Team2Id == teamId) ? 1 : 0;

                    break;
                case 2:
                    game.Set2Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set2Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 3:
                    game.Set3Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set3Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 4:
                    game.Set4Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set4Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 5:
                    game.Set5Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set5Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                default:
                    return BadRequest();
            }

            //increment current set
            if (IsSetComplete(game))
            {
                game.CurrentSet++;
                game.Team1Sets += (game.Team1Id == teamId) ? 1 : 0;
                game.Team2Sets += (game.Team2Id == teamId) ? 1 : 0;
            }
            //check if game complete
            if (IsGameComplete(game)) 
            {
                game.State = "finished";
                game.WinnerId = teamId;
                
                if(game.ParentId != null)
                {
                    if (!_gameRepository.GameExists((int)game.ParentId))
                    {
                        ModelState.AddModelError("", "No parent game with id: "+(int)game.ParentId);
                        return BadRequest(ModelState);
                    }

                    var parentGame = _gameRepository.GetGame((int)game.ParentId);

                    if (parentGame.Team1Id == null)
                    {
                        parentGame.Team1Id = game.WinnerId;
                    }
                    else if (parentGame.Team2Id == null)
                    {
                        parentGame.Team2Id = game.WinnerId;
                    }

                    //update parent game
                    if (!_gameRepository.UpdateGame(parentGame))
                    {
                        ModelState.AddModelError("", "Something went wrong updating parent game");
                        return StatusCode(500, ModelState);
                    }
                }
            }

            


            

            

            if (!_gameRepository.UpdateGame(game))
            {
                ModelState.AddModelError("", "Something went wrong updating game");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        


        [HttpPut("{gameId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateGame(int gameId, [FromBody] GameUpdate updateGame)
        {
            if (updateGame == null)
                return BadRequest(ModelState);

            if (gameId != updateGame.Id)
                return BadRequest(ModelState);

            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var gameToUpdate = _gameRepository.GetGame(gameId);
         
            gameToUpdate.StartDate = updateGame.StartDate;
            
            

            //var gameMap = _mapper.Map<Game>(updateGame);

            if (!_gameRepository.UpdateGame(gameToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating game");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private bool IsSetComplete(Game game)
        {
            int team1Points, team2Points;
            switch (game.CurrentSet)
            {
                case 1:
                    team1Points = game.Set1Team1Points;
                    team2Points = game.Set1Team2Points;
                    break;
                case 2:
                    team1Points = game.Set2Team1Points;
                    team2Points = game.Set2Team2Points;
                    break;
                case 3:
                    team1Points = game.Set3Team1Points;
                    team2Points = game.Set3Team2Points;
                    break;
                case 4:
                    team1Points = game.Set4Team1Points;
                    team2Points = game.Set4Team2Points;
                    break;
                case 5:
                    team1Points = game.Set5Team1Points;
                    team2Points = game.Set5Team2Points;
                    break;
                default:
                    return false;
            }

            return (team1Points >= 5 || team2Points >= 5) && Math.Abs(team1Points - team2Points) >= 2;
        }

        private bool IsGameComplete(Game game)
        {
            return game.Team1Sets == 3 || game.Team2Sets == 3;
        }

        private void AdvanceInBracket(Game game)
        {
            if (game.ParentId != null) return;

            if (!_gameRepository.GameExists((int)game.ParentId)) return;

            var parentGame = _gameRepository.GetGame((int)game.ParentId);

            if (parentGame.Team1Id == null)
            {
                parentGame.Team1Id = game.WinnerId;              
            }
            else if (parentGame.Team2Id == null)
            {
                parentGame.Team2Id = game.WinnerId;              
            }

            if(!_gameRepository.UpdateGame(parentGame)) return;

        }
    }
}
