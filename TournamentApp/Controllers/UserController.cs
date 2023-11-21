using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper, IPostRepository postRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _postRepository = postRepository;
            
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<UserDto>))]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(users);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserCreate userCreate)
        {
            if (userCreate == null)
                return BadRequest(ModelState);

            //check if already exists
            if(_userRepository.ValidateUser(userCreate.Email))
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userMap = _mapper.Map<User>(userCreate);

            //testing creating with DTO
            userMap.PasswordHash = "test";


            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }


        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _mapper.Map<UserDto>(_userRepository.GetUser(userId));
            //var user = _userRepository.GetById(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpGet("{userId}/posts")]
        [ProducesResponseType(200, Type = typeof(List<PostDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUserPosts(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();


            var posts = _mapper.Map<List<PostDto>>(_postRepository.GetPostsByUserId(userId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }

        
    }
}
