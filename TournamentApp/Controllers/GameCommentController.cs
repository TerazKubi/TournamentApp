using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/GameComments")]
    [ApiController]
    public class GameCommentController: Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameCommentRepository _gameCommentRepository;
        private readonly IMapper _mapper;

        public GameCommentController(IUserRepository userRepository, IGameCommentRepository gameCommentRepository, IMapper mapper, IGameRepository gameRepository)
        {
            _userRepository = userRepository;
            _gameCommentRepository = gameCommentRepository;
            _mapper = mapper;
            _gameRepository = gameRepository;
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateGameComment([FromBody] GameCommentCreate gameCommentCreate)
        {
            if (gameCommentCreate == null || gameCommentCreate.AuthorId == 0 || gameCommentCreate.GameId == 0)
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
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteGameComment(int gameCommentId)
        {
            if (!_gameCommentRepository.GameCommentExist(gameCommentId))
            {
                return NotFound();
            }

            var gameCommentToDelte = _gameCommentRepository.GetGameCommentById(gameCommentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_gameCommentRepository.DeleteGameComment(gameCommentToDelte))
            {
                ModelState.AddModelError("", "Something went wrong deleting game comment");
            }

            return NoContent();
        }
    }
}
