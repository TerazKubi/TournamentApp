using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/Players")]
    [ApiController]
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PlayerController(IPlayerRepository playerRepository, IMapper mapper,
            IUserRepository userRepository, ITeamRepository teamRepository, IPostRepository postRepository)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _postRepository = postRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PlayerNoDetailsDto>))]
        public IActionResult GetPlayers()
        {
            var players = _mapper.Map<List<PlayerNoDetailsDto>>(_playerRepository.GetPlayers());


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
        public IActionResult CreatePlayer([FromBody] PlayerCreate playerCreate)
        {
            if (playerCreate == null || playerCreate.UserId == 0 || playerCreate.TeamId == 0)
            {
                ModelState.AddModelError("errorMessage", "No data or no userId or teamId");
                return BadRequest(ModelState);
            }

            if (!_userRepository.UserExists(playerCreate.UserId))
            {
                ModelState.AddModelError("errorMessage", "No user with given userId");
                return BadRequest(ModelState);
            }

            if (!_teamRepository.TeamExists(playerCreate.TeamId))
            {
                ModelState.AddModelError("errorMessage", "No team with given teamId");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var playerMap = _mapper.Map<Player>(playerCreate);

            var user = _userRepository.GetUser(playerCreate.UserId);

            playerMap.User = user;
            



            if (!_playerRepository.CreatePlayer(playerMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating Player");
                return StatusCode(500, ModelState);
            }

            user.Player = playerMap;
            user.PlayerId = playerMap.Id;

            if (!_userRepository.UpdateUser(user))
            {
                ModelState.AddModelError("", "Something went wrong while updating User while creating Player");
                return StatusCode(500, ModelState);
            }
            

            return Ok("Successfully created");
        }

        [HttpPut("{playerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePlayer(int playerId, [FromBody] PlayerDto updatePlayer)
        {
            if (updatePlayer == null)
                return BadRequest(ModelState);

            if (playerId != updatePlayer.Id)
                return BadRequest(ModelState);

            if (!_playerRepository.PlayerExists(playerId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var playerMap = _mapper.Map<Player>(updatePlayer);

            if (!_playerRepository.UpdatePlayer(playerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating player");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpGet("{playerId}/posts")]
        [ProducesResponseType(200, Type = typeof(List<PostDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetPlayerPostss(int playerId)
        {
            if (!_playerRepository.PlayerExists(playerId))
                return NotFound();

            var posts = _mapper.Map<List<PostDto>>(_postRepository.GetPlayerPosts(playerId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }
    }
}
