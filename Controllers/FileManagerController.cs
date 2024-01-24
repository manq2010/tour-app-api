using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
using System.IO;
using Microsoft.AspNetCore.Hosting;

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
    public class FileManagerController : ControllerBase
    {
        private IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOptions<SmptSetting> _appSMTPSettings;

        public FileManagerController(
                IWebHostEnvironment environment,
                AppDbContext context,
                IOptions<SmptSetting> appSMTPSettings,
                IConfiguration configuration,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager
            )
        {
            _environment = environment;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSMTPSettings = appSMTPSettings;
            _configuration = configuration;
        }

        [HttpPost("Upload", Name = "Upload")]
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

        [HttpPost]
        [Route("PostDocsForLeader")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public IActionResult PostDocsForLeader([FromForm] IList<DocumentLeader> files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            var toReturn = new List<DocumentLeader>();
            DbLogic logic = new DbLogic(_context);

            foreach (var file in files)
            {
                var dbItem = new DocumentLeader();
                try
                {
                    var folderId = Convert.ToString(file.LeaderId);
                    var rootPath = Path.Combine(_environment.ContentRootPath, "Uploads"); ;
                    //Create the Directory.
                    string path = Path.Combine(rootPath, rootPath + "\\" + folderId + "\\Leader\\");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Fetch the File.
                    IFormFile postedFile = file.file;

                    //extract file detains
                    string fileName = postedFile.FileName;
                    string fileUrl = Path.Combine(path, fileName);
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    long filesize = postedFile.Length;
                    string mimeType = postedFile.ContentType;

                    //Save the File.
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    //populate dbFileObject less the raw file
                    dbItem.Id = file.Id;
                    dbItem.LeaderId = file.LeaderId;
                    dbItem.DocTypeName = file.DocTypeName;
                    dbItem.file_origname = fileName;
                    dbItem.file_url = fileUrl;
                    dbItem.file_size = filesize;
                    dbItem.file_mimetype = mimeType;
                    dbItem.file_extention = fileExtension;
                    if (file.Id == 0)
                    {
                        dbItem.created_at = DateTime.Now;
                        dbItem.updated_at = DateTime.Now;
                        dbItem.isdeleted = false;
                    }
                    else
                    {
                        dbItem.updated_at = DateTime.Now;
                        dbItem.isdeleted = file.isdeleted;
                    }


                    logic.InsertUpdateDocLeader(dbItem);


                    toReturn.Add(dbItem);
                }
                catch (Exception err)
                {
                    string err0rMsg = err.Message;
                }
            }

            //Send OK Response to Client.
            return Ok(toReturn);
        }

        [HttpGet]
        [Route("GetDocLeaderFileById")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public IActionResult GetDocLeaderFileById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            DbLogic logic = new DbLogic(_context);

            var item = logic.GetDocLeaderById(id);

            var net = new System.Net.WebClient();
            byte[] fileBytes = System.IO.File.ReadAllBytes(item.file_url);
            var contentType = item.file_mimetype;
            var fileName = item.file_origname;

            return File(fileBytes, contentType, fileName);

            //return Ok();
        }

        [HttpGet]
        [Route("GetLeaderFilesListById")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public IActionResult GetLeaderFilesListById(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            DbLogic logic = new DbLogic(_context);

            var filesList = logic.GetLeaderFilesListById(Id);

            return Ok(filesList);
        }

        [HttpDelete]
        [Route("DeleteDocLeader")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public ActionResult<string> DeleteDocLeader(int Id)
        {
            ObjectResult response = StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "", Message = "" });
            if (ModelState.IsValid)
            {
                DbLogic logic = new DbLogic(_context);
                var DBResponse = logic.DeleteDocLeader(Id);
                if (DBResponse == "Success")
                { response = StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Successfully deleted leader document" }); }
                else
                {
                    response = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Failed to delete leader document" });
                }
            }
            else
            {
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList());
            }

            return response;
        }


    }
}
