using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
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
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;
        public UserController(IUserRepository userRepository, IMapper mapper, IPostRepository postRepository, IWebHostEnvironment environment, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _postRepository = postRepository;
            _environment = environment;
            _userManager = userManager;
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

        [HttpPost("{userId}/ProfilePicture")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile, string userId)
        {
            if(!_userRepository.UserExists(userId)) return BadRequest();


            
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);
            
            if (!(userId.Equals(currentUserId) || isAdmin)) return Forbid();


            try
            {
                //string FilePath = _environment.WebRootPath + "\\Upload\\UserImages\\";
                string FilePath = "/app/wwwroot/Upload/UserImages/";
                Console.WriteLine($"POST: FilePath {FilePath}");
                if (!System.IO.Directory.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }

                string ImagePath = FilePath + userId + ".png";
                if (!System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
                using(FileStream stream=System.IO.File.Create(ImagePath))
                {
                    using (var image = Image.Load(imageFile.OpenReadStream()))
                    {
                        // Resize to a specific width and height (e.g., 300x300)
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(800, 800),
                            Mode = ResizeMode.Max
                        }));

                        // Save the resized image
                        image.Save(stream, new JpegEncoder());
                    }
                    //await imageFile.CopyToAsync(stream);
                }

                var user = _userRepository.GetUser(userId);

                user.ImageURL = "/Upload/UserImages/" + user.Id + ".png";

                if(!_userRepository.UpdateUser(user))
                {
                    ModelState.AddModelError("error", "problem with updating user");
                    return BadRequest(ModelState);
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Ok();
        }

        [HttpGet("{userId}/ProfilePicture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetImage(string userId)
        {
            if (!_userRepository.UserExists(userId)) return BadRequest();

            string ImageUrl = String.Empty;
            string HostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            try
            {
                string FilePath = _environment.WebRootPath + "/Upload/UserImages/";
                Console.WriteLine($"GET: FilePath {FilePath}");
                string ImagePath = FilePath + userId + ".png";
                Console.WriteLine("ImagePath: " + ImagePath);


                if (System.IO.File.Exists(ImagePath))
                {
                    ImageUrl = HostUrl + "/Upload/UserImages/" + userId + ".png";
                    
                } else
                {
                    ImageUrl = HostUrl + "/Upload/UserImages/default.png";
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Ok(ImageUrl);
        }

        [HttpDelete("{userId}/ProfilePicture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveImage(string userId)
        {
            if (!_userRepository.UserExists(userId)) return BadRequest();

            
            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);

            if (!(userId.Equals(currentUserId) || isAdmin)) return Forbid();


            try
            {
                string FilePath = _environment.WebRootPath + "/Upload/UserImages/";

                string ImagePath = FilePath + userId + ".png";
                


                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                    
                }
                else
                {
                    return NotFound();
                }

                var user = _userRepository.GetUser(userId);
                user.ImageURL = "/Upload/UserImages/default.png";

                if (!_userRepository.UpdateUser(user))
                {
                    ModelState.AddModelError("error", "problem with updating user");
                    return BadRequest(ModelState);
                }

                return Ok();

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }



    }
}
