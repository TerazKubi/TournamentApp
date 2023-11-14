using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public PlayerController(IPlayerRepository playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PlayerDto>))]
        public IActionResult GetPlayers()
        {
            var players = _mapper.Map<List<PlayerDto>>(_playerRepository.GetPlayers());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(players);
        }

        [HttpGet("{playerId}")]
        [ProducesResponseType(200, Type = typeof(PlayerDto))]
        [ProducesResponseType(400)]
        public IActionResult GetPlayerById(int playerId)
        {
            if (!_playerRepository.PlayerExists(playerId))
                return NotFound();

            var player = _mapper.Map<PlayerDto>(_playerRepository.GetById(playerId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(player);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePost([FromBody] PlayerDto playerCreate)
        {
            if (playerCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //validate player somehow c:
            //if (!_userRepository.UserExists(postCreate.AuthorId))
            //    return BadRequest(ModelState);


            var playerMap = _mapper.Map<Player>(playerCreate);



            if (!_playerRepository.CreatePlayer(playerMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating Player");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
    }
}
