using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
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


        //[HttpPost]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //public IActionResult CreateUser([FromBody] UserCreateWrapper userCreate)
        //{
        //    if (userCreate == null)
        //        return BadRequest(ModelState);

        //    //check if already exists
        //    if(_userRepository.ValidateUser(userCreate.UserCreate.Email))
        //        return BadRequest(ModelState);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var userMap = _mapper.Map<User>(userCreate.UserCreate);

        //    //testing creating with DTO
        //    userMap.PasswordHash = HashPassword(userCreate.Password);


        //    if (!_userRepository.CreateUser(userMap))
        //    {
        //        ModelState.AddModelError("", "Something went wrong while savin");
        //        return StatusCode(500, ModelState);
        //    }

        //    return Ok("Successfully created");
        //}

        
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(string userId)
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
        public IActionResult GetUserPosts(string userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();


            var posts = _mapper.Map<List<PostDto>>(_postRepository.GetPostsByUserId(userId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }

        //private string HashPassword(string password)
        //{
        //    using SHA256 sha256 = SHA256.Create();
        //    // Convert the password string to bytes
        //    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        //    // Compute the hash value of the password bytes
        //    byte[] hashBytes = sha256.ComputeHash(passwordBytes);

        //    // Convert the hashed bytes to a base64-encoded string
        //    string hashedPassword = Convert.ToBase64String(hashBytes);

        //    return hashedPassword;
        //}


    }
}
