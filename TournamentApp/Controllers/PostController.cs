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
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public PostController(IPostRepository postRepository, 
            IUserRepository userRepository,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PostDto>))]
        public IActionResult GetPosts()
        {
            var posts = _mapper.Map<List<PostDto>>(_postRepository.GetPosts());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePost([FromBody] Post post)
        {
            if (post == null)
                return BadRequest(ModelState);

            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);




            if (!_postRepository.CreatePost(post))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpGet("byPostId/{postId}")]
        [ProducesResponseType(200, Type = typeof(PostDto))]
        [ProducesResponseType(400)]
        public IActionResult GetPostById(int postId)
        {
            if (!_postRepository.PostExists(postId))
                return NotFound();

            var user = _mapper.Map<PostDto>(_postRepository.GetPostById(postId));
            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }


        [HttpGet("byUserId/{userId}")]
        [ProducesResponseType(200, Type = typeof(List<Post>))]
        [ProducesResponseType(400)]
        public IActionResult GetPostsByUserId(int userId)
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
