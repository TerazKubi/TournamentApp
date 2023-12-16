using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Models;

namespace TournamentApp.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthenticateController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user = _mapper.Map<UserDto>(user),
                roles = userRoles
            });
            
            
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            // Retrieve the user based on the username or email (customize as needed)
            var user = await _userManager.FindByNameAsync(model.Username);

            // Check if the user exists
            if (user == null) 
                return NotFound("User not found");
            

            // Check if the old password is correct
            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                return BadRequest("Old password is incorrect");
            
            // Change the password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
                return Ok("Password changed successfully");            
            else
                return BadRequest(result.Errors);

        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("AddRole/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddRole(string userId, string role)
        {

            if (!IsValidRole(role))
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Invalid role!" });
            

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User doesn't exist!" });

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
                return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"Failed to add user to the {role} role." });
        }



        //[Authorize(Roles = UserRoles.Admin)]
        //[HttpPost]
        //[Route("AddOrganizerRole/{userId}")]
        //public async Task<IActionResult> AddOrganizerRole(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Organizer))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Organizer));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Organizer))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Organizer);
        //    }

        //    return Ok();
        //}

        //[Authorize(Roles = UserRoles.Admin)]
        //[HttpPost]
        //[Route("AddRefereeRole/{userId}")]
        //public async Task<IActionResult> AddRefereeRole(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Referee))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Referee));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Referee))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Referee);
        //    }

        //    return Ok();
        //}

        //[Authorize(Roles = UserRoles.Admin)]
        //[HttpPost]
        //[Route("AddPlayerRole/{userId}")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> AddPlayerRole(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User doesn't exists!" });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Player))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Player));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Player))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Player);
        //    }

        //    return Ok();
        //}

        //[Authorize(Roles = UserRoles.Admin)]
        //[HttpPost]
        //[Route("AddTeamRole/{userId}")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> AddTeamRole(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User doesn't exists!" });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Team))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Team));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Team))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Team);
        //    }

        //    return Ok();
        //}

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Organizer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Organizer));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Organizer);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }


        private bool IsValidRole(string role)
        {
            var validRoles = new[] { UserRoles.Admin, UserRoles.Organizer, UserRoles.Referee, UserRoles.Player, UserRoles.Team, UserRoles.User };
            return validRoles.Contains(role);
        }
    }
}
