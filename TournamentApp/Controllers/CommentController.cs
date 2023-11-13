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
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        
            
        public CommentController(ICommentRepository commentRepository,
            IPostRepository postRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        [HttpGet("comments")]
        [ProducesResponseType(200, Type = typeof(List<CommentDto>))]
        public IActionResult GetComments()
        {
            var comments = _mapper.Map<List<CommentDto>>(_commentRepository.GetComments());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comments);
        }

        [HttpGet("comments/{postId}")]
        [ProducesResponseType(200, Type = typeof(List<Comment>))]
        [ProducesResponseType(400)]
        public IActionResult GetPostComments(int postId)
        {
            if (!_postRepository.PostExists(postId))
                return NotFound();
                
         
            var comments = _mapper.Map<List<CommentDto>>(_commentRepository.GetCommentsByPostId(postId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comments);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateComment([FromBody] Comment comment)
        {
            if (comment == null)
                return BadRequest(ModelState);

            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);




            if (!_commentRepository.CreateComment(comment))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpGet("byCommentId/{commentId}")]
        [ProducesResponseType(200, Type = typeof(Comment))]
        [ProducesResponseType(400)]
        public IActionResult GetPostById(int commentId)
        {
            if (!_commentRepository.CommentExist(commentId))
                return NotFound();

            var comment = _mapper.Map<CommentDto>(_commentRepository.GetCommentById(commentId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(comment);
        }

    }
}
