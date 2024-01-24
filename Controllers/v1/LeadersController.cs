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
using Microsoft.Extensions.Logging;

using Touring.api.Models;
using Touring.api.Data;
using Touring.api.Logic;
using Touring.api.Services;
using Touring.api.Filters;
using Touring.api.Helpers;

namespace Touring.api.Controllers
{
    //  [Authorize]
    [EnableCors("DefaultCorsPolicy")]
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class LeadersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IOptions<SmptSetting> _appSMTPSettings;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IUriService _uriService;

          public LeadersController(UserManager<ApplicationUser> userManager,
                                      RoleManager<IdentityRole> roleManager,
                                      IConfiguration configuration,
                                      AppDbContext dbContext,
                                      IOptions<SmptSetting> appSMTPSettings,
                                      ILogger<AuthenticateController> logger,
                                      IUriService uriService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = dbContext;
            _appSMTPSettings = appSMTPSettings;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet]
        [Route("GetPagedAllLeaders")]
        [MapToApiVersion("1")]
        [AllowAnonymous]
        
        public IActionResult GetPagedAllLeaders([FromQuery] PaginationFilter filter)
        {
            DbLogic logic = new DbLogic(_context);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var route = Request.Path.Value;
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var pagedData = _context.Leaders
                    .Where(d => d.isdeleted == false)
                    .Include(d => d.Gender)
                    .OrderByDescending(d => d.Id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize)
               .ToList();

                var totalRecords = _context.Leaders.Where(d => d.isdeleted == false).Count();

                //return Ok(new PagedResponse<List<cbbuser>>(pagedData, validFilter.PageNumber, validFilter.PageSize));
                var pagedReponse = PaginationHelper.CreatePagedReponse<Leader>(pagedData, validFilter, totalRecords, _uriService, route);
                return Ok(pagedReponse);

            }
            catch (Exception err)
            {
                string message = err.Message;
                //return BadRequest();
                //throw;
                return BadRequest(new Response { Status = "Error", Message = err.Message });
            }
        }


        [HttpGet]
        [Route("GetAllLeaders")]
        [MapToApiVersion("1")]
        [AllowAnonymous]
         public IActionResult GetAllLeaders()
        {
            try
            {
                var alldata = _context.Leaders
                    .Where(d => d.isdeleted == false)
                    .Include(d => d.Gender)
                    // .Include(d => d.Region)
                    // .Include(d => d.CandidateStatus)
                    .ToList();
               
                return Ok(alldata);

            }
            catch (Exception err)
            {
                string message = err.Message;
                return BadRequest(new Response { Status = "Error", Message = err.Message });
            }
        }

        [HttpPost]
        [Route("PostInsertNewLeader")]
        [MapToApiVersion("1")]
         public ActionResult<string> PostInsertNewLeader(Leader leader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            ObjectResult response = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            
            if (ModelState.IsValid)
            {

                DbLogic logic = new DbLogic(_context, _configuration);
                var leaderExist = logic.LeaderExist(leader);
                if ((!leaderExist && leader.Id==0)||(leaderExist && leader.Id != 0) || (!leaderExist && leader.Id != 0))
                {

                    var DBResponse = logic.PostInsertNewLeader(leader);
                    if (DBResponse == "Success")
                    { response = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Successfully added new leader", DetailDescription = leader }); }
                    else
                    {
                        response = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Failed to add new leader" });
                    }
                }
                else
                {
                    response = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "DuplicateLeaderAddition", Message = "Leader already exist" });
                }
            }

            else
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Model State Is Invalid" });
            }

            return response;
        }

        [HttpPost("DeleteLeader")]
        [MapToApiVersion("1")]
        public ActionResult<string> DeleteLeader(int id)
        {
            ObjectResult response = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });

            if (ModelState.IsValid)
            {
                DbLogic logic = new DbLogic(_context);
                var DBResponse = logic.DeleteLeader(id);
                if (DBResponse == "Success")
                { response = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Successfully deleted leader" }); }
                else
                {
                    response = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Failed to delete leader" });
                }
            }

            else
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Model State Is Invalid" });
            }

            return response;
        }


    }
}
