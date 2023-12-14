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


        //[HttpPost]
        //[Route("ResetPassword")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user == null)
        //    {
        //        // User not found
        //        return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found!" });
        //    }

        //    // Validate the token
        //    var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", model.Token);
        //    if (!isTokenValid)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Invalid reset token!" });
        //    }

        //    // Check if the reset token has not expired
        //    var tokenExpiration = await _userManager.GetTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword");
        //    var t = _userManager.
        //    if (string.IsNullOrEmpty(tokenExpiration) || DateTimeOffset.Parse(tokenExpiration) <= DateTimeOffset.UtcNow)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Reset token has expired!" });
        //    }

        //    // Reset the user's password
        //    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        //    if (!resetPasswordResult.Succeeded)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Password reset failed! Please try again." });
        //    }

        //    return Ok(new Response { Status = "Success", Message = "Password reset successfully!" });
        //}

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("AddOrganizerRole/{userId}")]
        public async Task<IActionResult> AddOrganizerRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Organizer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Organizer));

            if (await _roleManager.RoleExistsAsync(UserRoles.Organizer))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Organizer);
            }

            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("AddRefereeRole/{userId}")]
        public async Task<IActionResult> AddRefereeRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Referee))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Referee));

            if (await _roleManager.RoleExistsAsync(UserRoles.Referee))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Referee);
            }

            return Ok();
        }

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
    }
}
