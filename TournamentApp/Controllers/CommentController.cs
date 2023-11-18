using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
            
        public CommentController(ICommentRepository commentRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
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
        public IActionResult CreateComment([FromBody] CommentCreate commentCreate)
        {
            if (commentCreate == null || commentCreate.AuthorId == 0 || commentCreate.PostId == 0)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_postRepository.PostExists(commentCreate.PostId)) 
                return BadRequest(ModelState);

            if(!_userRepository.UserExists(commentCreate.AuthorId))
                return BadRequest(ModelState);

            var comment = _mapper.Map<Comment>(commentCreate);


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

        [HttpPut("{commentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int commentId, [FromBody] CommentDto updateComment)
        {
            if (updateComment == null)
                return BadRequest(ModelState);

            if (commentId != updateComment.Id)
                return BadRequest(ModelState);

            if (!_commentRepository.CommentExist(commentId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var commentMap = _mapper.Map<Comment>(updateComment);

            if (!_commentRepository.UpdateComment(commentMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{commentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteComment(int commentId)
        {
            if (!_commentRepository.CommentExist(commentId))
            {
                return NotFound();
            }

            var commentToDelte = _commentRepository.GetCommentById(commentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_commentRepository.DeleteComment(commentToDelte))
            {
                ModelState.AddModelError("", "Something went wrong deleting comment");
            }

            return NoContent();
        }

    }
}
