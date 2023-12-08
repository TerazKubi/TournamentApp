using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
    [Route("api/GameComments")]
    [ApiController]
    public class GameCommentController: Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameCommentRepository _gameCommentRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GameCommentController(IUserRepository userRepository, IGameCommentRepository gameCommentRepository, IMapper mapper, IGameRepository gameRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _gameCommentRepository = gameCommentRepository;
            _mapper = mapper;
            _gameRepository = gameRepository;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateGameComment([FromBody] GameCommentCreate gameCommentCreate)
        {
            if (gameCommentCreate == null || gameCommentCreate.GameId == 0)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_gameRepository.GameExists(gameCommentCreate.GameId))
                return BadRequest(ModelState);

            if (!_userRepository.UserExists(gameCommentCreate.AuthorId))
                return BadRequest(ModelState);

            var comment = _mapper.Map<GameComment>(gameCommentCreate);


            if (!_gameCommentRepository.CreateGameComment(comment))
            {
                ModelState.AddModelError("", "Something went wrong while game comment");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("{commentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteGameComment(int gameCommentId)
        {
            if (!_gameCommentRepository.GameCommentExist(gameCommentId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var gameCommentToDelte = _gameCommentRepository.GetGameCommentById(gameCommentId);

            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);

            if (!(gameCommentToDelte.AuthorId.Equals(currentUserId) || isAdmin)) return Forbid();

            if (!_gameCommentRepository.DeleteGameComment(gameCommentToDelte))
            {
                ModelState.AddModelError("", "Something went wrong deleting game comment");
            }

            return Ok();
        }
    }
}
