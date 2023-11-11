using Microsoft.AspNetCore.Mvc;
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
        public PostController(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Post>))]
        public IActionResult GetPosts()
        {
            var posts = _postRepository.GetPosts();

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

            //check if already exists

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
        [ProducesResponseType(200, Type = typeof(Post))]
        [ProducesResponseType(400)]
        public IActionResult GetPostById(int postId)
        {
            if (!_postRepository.PostExists(postId))
                return NotFound();

            //var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            var user = _postRepository.GetPostById(postId);

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

            var posts = _postRepository.GetPostsByUserId(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }
    }
}
