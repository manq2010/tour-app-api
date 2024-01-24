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
using Touring.api.Logic;
using Touring.api.Services;

namespace Touring.api.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors("DefaultCorsPolicy")]
    [ApiVersion("1")]
    [ApiVersion("2")]

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

        [AllowAnonymous]
        [HttpPost("Login")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> Login(LoginModel appUser)
        {
            ApplicationUser user = null;

            user = _context.User.Where(user => user.UserName == appUser.Username).SingleOrDefault();

            ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            if (user != null)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                var userRoles = await _userManager.GetRolesAsync(user);
                List<string> roles = (List<string>)userRoles;
                string rolesList = string.Join(",", roles.ToArray());

                // User toReturn = new User();

                //toReturn.Id = user.Id;
                //toReturn.EmailConfirmed = user.EmailConfirmed;
                // toReturn.Email = user.Email;
                // toReturn.UserName = user.UserName;
                // toReturn.created_at = user.created_at;
                // toReturn.deleted_at = user.deleted_at;
                // toReturn.updated_at = user.updated_at;
                //toReturn.PhoneNumber = user.PhoneNumber;
                //toReturn.PasswordHash = user.PasswordHash;
                //toReturn.Fullname = user.Fullname;
                //toReturn.IDNumber = user.IDNumber;
                // toReturn.token = new JwtSecurityTokenHandler().WriteToken(token);
                //toReturn.Gender = user.Gender;
                //sign in

                var signInResult = await _signInManager.PasswordSignInAsync(user, appUser.Password, false, false);
                //userObj = _context.users.Where(d => d.id == usermap.userid).SingleOrDefault();
                if (signInResult.Succeeded)
                {
                    // statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "200", Message = "Successfully Signed In", DetailDescription = toReturn });
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                        ,expiration = token.ValidTo
                        ,aspUserID  = user.Id.ToString()
                        ,rolesList  = rolesList
                    });
                }
                else
                {
                    // statusCode = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "500", Message = "Please check your password and username" });
                    return Unauthorized(signInResult);
                }
            }
            else if(user == null) {

                statusCode = StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "401", Message = "Please check your password and username" });
            }
            else
            {
                statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "400", Message = "Bad request was made" });
            }
            return statusCode;

        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
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
                    created_at = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, appUser.Password);

                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await _roleManager.RoleExistsAsync(UserRoles.Subscriber))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Subscriber));

                if (result.Succeeded)
                {
                    // await _userManager.AddToRoleAsync(user, UserRoles.SystemAdmin);
                    await _userManager.AddToRoleAsync(user, appUser.UserRole);
                    statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User created successfully", Detail = result });
                }

                if (!result.Succeeded)
                {
                    statusCode = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation unsuccessful:", Detail = result });
                }
            } else
            {
                statusCode = StatusCode(StatusCodes.Status422UnprocessableEntity, new Response { Status = "Error", Message = "Model state is invalid , please check model" });

            }
            return statusCode;
        }

        [AllowAnonymous]
        [HttpPost("RegisterSubscriber")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> RegisterSubscriber(RegisterSubscriberModel appUser)
        {
            ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            //register functionality  
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,

                };

                var result = await _userManager.CreateAsync(user, appUser.Password);

                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!await _roleManager.RoleExistsAsync("Subscriber"))
                    await _roleManager.CreateAsync(new IdentityRole("Subscriber"));

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, appUser.UserRole);

                    statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Subscriber created successfully", Detail = result });

                    //create 
                    try
                    {
                        UserProfile userProfile = new UserProfile();
                        userProfile.UserProfileId = 0;
                        userProfile.FullName = appUser.FullName;
                        userProfile.Email = appUser.Email;
                        //userProfile.mobilenumber = appUser.mo
                        userProfile.AspuId = user.Id;
                        userProfile.UserRole = appUser.UserRole;
                        userProfile.isdeleted = true;
                        userProfile.UserRole = appUser.UserRole;
                        DbLogic logic = new DbLogic(_context);
                        logic.InsertUpdateUserProfile(userProfile);
                    }
                    catch (Exception err)
                    { 
                      //error creating user profile, but login was successfull
                    }
                }

                if (!result.Succeeded)
                {
                    statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Subscriber creation unsuccessful:", Detail = result });
                    // return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
                }
            }
            if (!ModelState.IsValid)
            {
                statusCode = StatusCode(StatusCodes.Status422UnprocessableEntity, new Response { Status = "Error", Message = "Model state is invalid , please check model" });
            }
            return statusCode;
        }

        // [Authorize]
        [HttpPost("InsertUpdateUserProfile")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> InsertUpdateUserProfile(UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            try
            {
                DbLogic logic = new DbLogic(_context);
                logic.InsertUpdateUserProfile(userProfile);

                return Ok(userProfile);

            }
            catch (Exception err)
            { 
            
            }

            return BadRequest();

        }

        [Authorize]
        [HttpDelete("DeleteUserProfileById")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> DeleteUserProfileById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                DbLogic logic = new DbLogic(_context);
                logic.DeleteUserProfileById(id);
                return Ok();
            }
            catch (Exception err)
            {
                string message = err.Message;
                throw err;
            }
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> LogOut(string username, string password)
        {
            try
            {

                await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Login");

            }
            catch (Exception e)
            {

                return Ok(false);
            } 
        }

        [HttpGet("GetAllUserLogins")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetAllUserLogins()
        {
            var users = _userManager.Users.Where(d => d.deleted == false);
            if (users == null)
            {
                return Unauthorized(new { response = "Invalid users" });
            } else {

                return Ok(users);
            }
        }

        [HttpDelete("DeleteUser")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            // Check if userId is provided
            if (string.IsNullOrEmpty(id))
            {
                statusCode = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User Id not provided" });
                return statusCode;
            }

            DbLogic logic = new DbLogic(_context);
            var DBResponse = logic.DeleteUser(id);

            // Check if the user deletion was successful
            if (DBResponse == "Success")
            {
                statusCode = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User deleted successfully" });
            }
            else
            {
                statusCode = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User deletion unsuccessful" });
            }
        
            return statusCode;
        }

        [HttpPost("PostUpdateIdentityUser")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        // [Authorize]
        public async Task<IActionResult> UpdateIdentityUser([FromBody] ApplicationUser userToUpdate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            // Get the existing user from the db
            var user = await _userManager.FindByIdAsync(userToUpdate.Id);

            if (user != null)
            {
                user.UserName = userToUpdate.UserName;
                user.Email = userToUpdate.Email;
                user.updated_at = DateTime.Now;
                user.IsActive = userToUpdate.IsActive;
                user.IsAdminUser = userToUpdate.IsAdminUser;
  
                // Apply the changes if any to the db
                try
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    return Ok(updateResult);
                }
                catch (Exception err)
                {
                    return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
                    // throw new HttpException((int)HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest(new { response = "User with provided not found Id= " + userToUpdate.Id });
            }
        }

        [HttpGet("LoginUserNameExist")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> LoginUserNameExist(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpPost("RequestPasswordReset")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                return Unauthorized(new { response = "Invalid email" });
            }

            if (user != null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                //Generate email with the new token
                byte[] resetTokenGeneratedBytes = Encoding.UTF8.GetBytes(resetToken);
                var validResetToken = Uri.EscapeDataString(WebEncoders.Base64UrlEncode(resetTokenGeneratedBytes));

                //var appUrl = _configuration["AppURL"]; //"http://localhost:4200/#/reset-password";//get URL from config
                // var appUrl = _configuration["AppURL"];

                var appUrl = "http://localhost:4200/";

                string resetUrl = appUrl + @"#/reset-password?email=" + email + "&token=" + validResetToken;

                string resetEmailBody = $"<h1>Test App</h1>" + $"<p>Please confirm your email by <a href='{resetUrl}'>Clicking here</a></p>";

                try
                {
                    EmailService emailService = new EmailService(_configuration);
                    emailService.SendPasswordResetEmail(user.Email, resetEmailBody);
                }
                catch (Exception err)
                {

                }

                return Ok(new
                {
                    resetToken = validResetToken,
                    resetUrl = resetUrl
                });
            }

            return Unauthorized(new { response = "Invalid email" });
        }

        [HttpPost("ResetPassword")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPassword)
        {
            var user = await _userManager.FindByNameAsync(resetPassword.email);

            if (user == null)
            {
                return Unauthorized(new { response = "Invalid email" });
            }

            if (resetPassword.newPassword != resetPassword.confirmPassword)
            {
                return BadRequest(new { response = "Password match failed" });
            }

            if (user != null)
            {

                //get number of times the password hash has been used.
                var decodedResetToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPassword.token));

                var result = await _userManager.ResetPasswordAsync(user, decodedResetToken, resetPassword.newPassword);

                if (result.Succeeded)
                {
                    return Ok(new { response = "Password reset successful." });
                }
                else
                {
                    var erros = result.Errors.Select(e => e.Description);
                    return Unauthorized(new { response = erros });
                    // return Unauthorized(new { response = "Invalid token" });
                }
            }

            return Unauthorized(new { response = "Invalid email" });
        }

        [HttpGet("GetRoles")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> GetRoles()
        {         

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (!await _roleManager.RoleExistsAsync(UserRoles.Subscriber))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Subscriber));

            return Ok(_roleManager.Roles.ToList());
        }
    }
}
