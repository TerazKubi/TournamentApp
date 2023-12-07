using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
    [Route("api/Posts")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public PostController(IPostRepository postRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ICommentRepository commentRepository,
            UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _commentRepository = commentRepository;
            _userManager = userManager;
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
        public IActionResult CreatePost([FromBody] PostCreate postCreate)
        {
            if (postCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_userRepository.UserExists(postCreate.AuthorId)) 
                return BadRequest(ModelState);


            var postMap = _mapper.Map<Post>(postCreate);



            if (!_postRepository.CreatePost(postMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpGet("{postId}")]
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

        [HttpGet("{postId}/comments")]
        [ProducesResponseType(200, Type = typeof(List<PostDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetPostComments(int postId)
        {
            if (!_postRepository.PostExists(postId))
                return NotFound();


            var comments = _mapper.Map<List<CommentDto>>(_commentRepository.GetCommentsByPostId(postId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comments);
        }


        [HttpPut("{postId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePost(int postId, [FromBody] PostDto updatePost)
        {
            if (updatePost == null)
                return BadRequest(ModelState);

            if (postId != updatePost.Id)
                return BadRequest(ModelState);

            if (!_postRepository.PostExists(postId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var postMap = _mapper.Map<Post>(updatePost);

            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);

            if (currentUserId != postMap.AuthorId || !isAdmin) return Forbid();

            if (!_postRepository.UpdatePost(postMap))
            {
                ModelState.AddModelError("", "Something went wrong updating post");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{postId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePostAsync(int postId)
        {
            if (!_postRepository.PostExists(postId))
            {
                return NotFound();
            }

            var postToDelete = _postRepository.GetPostById(postId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);
            Console.WriteLine("\n CUrent user id: " + currentUserId + " IS ADMIN: "+isAdmin+"\n");

            if (currentUserId != postToDelete.AuthorId || !isAdmin) return Forbid();
            
            if (!_postRepository.DeletePost(postToDelete))
            {
                ModelState.AddModelError("error", "something went wrong deleting post");
                return StatusCode(500, ModelState);
            }

            return NoContent();
            

          
        }
    }
}
