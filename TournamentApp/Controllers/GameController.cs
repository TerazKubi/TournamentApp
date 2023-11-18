using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public GameController(IMapper mapper, IGameRepository gameRepository, 
            ITournamentRepository tournamentRepository, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _gameRepository = gameRepository;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<GameDto>))]
        public IActionResult GetGames()
        {
            var games = _mapper.Map<List<GameDto>>(_gameRepository.GetGames());


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

            if(!_teamRepository.TeamExists(gameCreate.Team1Id) || !_teamRepository.TeamExists(gameCreate.Team2Id))
                return BadRequest(ModelState);




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

            if (game.State == "Finished")
                return BadRequest();

            //ultra walidacja TODO: do zmiany na ładniejszy kod
            if(game.Team1Id == teamId)
            {
                game.Team1Points++;
                if (game.Team1Points == 2)
                {
                    game.Team1Points=0;
                    game.Team2Points=0;
                    game.Team1Sets++;
                }
            } else
            {
                game.Team2Points++;
                if(game.Team2Points == 2)
                {
                    game.Team2Points=0;
                    game.Team1Points=0;
                    game.Team2Sets++;
                }
            }


            //check if win
            if (game.Team1Sets == 3 || game.Team2Sets == 3) game.State = "Finished";

            //handle win TODO
            // progress in bracket or something ...

            if (!_gameRepository.UpdateGame(game))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
