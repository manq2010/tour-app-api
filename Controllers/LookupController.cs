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
    // [Route("api/[controller]")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors("DefaultCorsPolicy")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class LookupController : ControllerBase
    {
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOptions<SmptSetting> _appSMTPSettings;

        public LookupController(
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

        [HttpGet("GetAllGenders")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [AllowAnonymous]
        public IActionResult GetAllGenders()
        {
            DbLogic logic = new DbLogic(_context, _configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                List<Gender> records = new List<Gender>();
                records = logic.GetAllGenders().ToList();

                return Ok(records);
            }
            catch (Exception err)
            {
                string message = err.Message;
                throw err;
            }
        }
    }
}
