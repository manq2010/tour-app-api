using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

using Touring.api.Models;
using Touring.api.Data;

namespace Touring.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthenticateController : ControllerBase
	{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOptions<SmptSetting> _appSMTPSettings;

        public AuthenticateController(
                AppDbContext context,
                IOptions<SmptSetting> appSMTPSettings,
                IConfiguration configuration,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSMTPSettings = appSMTPSettings;
            _configuration = configuration;
        }

        // [AllowAnonymous]
        // [HttpPost("Login")]
        // public async Task<IActionResult> Login(LoginModel appUser)
        // {
        //     /*ApplicationUser user = null;*/

        //     /*await _context.AddAsync(student);*/

        //     /*var result = await _context.SaveChangesAsync();*/

        //     ApplicationUser user = null;

        //     user = _context.User.Where(user => user.UserName == appUser.Username).SingleOrDefault();

        //     ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

        //     if (user != null)
        //     {
        //         var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        //         var authClaims = new List<Claim>
        //             {
        //                 new Claim(ClaimTypes.Name, user.UserName),
        //                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //             };

        //         var token = new JwtSecurityToken(
        //             issuer: _configuration["JWT:ValidIssuer"],
        //             audience: _configuration["JWT:ValidAudience"],
        //             expires: DateTime.Now.AddHours(3),
        //             claims: authClaims,
        //             signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //             );

        //         User toReturn = new User();

        //         toReturn.Id = user.Id;
        //         toReturn.Email = user.Email;
        //         toReturn.UserName = user.UserName;
        //         toReturn.PasswordHash = user.PasswordHash;
        //         toReturn.Fullname = user.Fullname;
        //         toReturn.token = new JwtSecurityTokenHandler().WriteToken(token);
        //         //sign in  
        //         var signInResult = await signInManager.PasswordSignInAsync(user, appUser.Password, false, false);
        //         //userObj = _context.users.Where(d => d.id == usermap.userid).SingleOrDefault();
        //         if (signInResult.Succeeded)
        //         {

        //             statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "200", Message = "Successfully Signed In", DetailDescription = signInResult });
        //         }
        //         else
        //         {
        //             statusCode = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "500", Message = "Please check your password and username" });
        //         }
        //     }
        //     else
        //     {
        //         statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "400", Message = "Bad request was made" });
        //     }
        //     return statusCode;
        // }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel appUser)
        {
            ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            //register functionality  
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = appUser.Username,
                    Email = appUser.Email,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    created_at = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, appUser.Password);

                if (!await _roleManager.RoleExistsAsync(UserRoles.SystemAdmin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.SystemAdmin));

                if (!await _roleManager.RoleExistsAsync(UserRoles.BasicUser))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.BasicUser));

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.SystemAdmin);
                    statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User created successfully", Detail = result });
                }

                if (!result.Succeeded)
                {
                    statusCode = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation unsuccessful:", Detail = result });
                }
            }
            if (!ModelState.IsValid)
            {
                statusCode = StatusCode(StatusCodes.Status422UnprocessableEntity, new Response { Status = "Error", Message = "Model state is invalid , please check model" });
            }
            return statusCode;
        }

    }
}
