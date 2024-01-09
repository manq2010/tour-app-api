using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Touring.api.Models;
using Touring.api.Data;

// using Touring.api.Models;
// using Touring.api.DBModels;
// using Touring.api.Authentication;
// using Touring.api.Services;

namespace Touring.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    // api/students
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public StudentsController(
            AppDbContext context,
            IConfiguration configuration
            )
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> GetStudents()
        {
            var students = await _context.Students.AsNoTracking().ToListAsync();

            return students;
        }

    /*    [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _context.Students
                .AsNoTracking()
                .Where(x => x.Name != "name")
                .ToListAsync();

                return Ok(students);
            }
            catch (Exception err)
            {
                string message = err.Message;
                return BadRequest(new Response { Status = "Error", Message = err.Message });
            }
        }*/

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

                /* return BadRequest(ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList()); */
            }

            await _context.AddAsync(student);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                // return Ok(result);
                return Ok();
            }

            return BadRequest();
            // return BadRequest(ModelState);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            /*if (student is null) return NotFound(new { response = "Student not found" });*/

            if (student is null) return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Failed to return candidate" });

            /*return Ok(student);*/
            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Successfully returned candidate", DetailDescription = student });

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student is null) return NotFound();

            _context.Remove(student);

            var result = await _context.SaveChangesAsync();

            // if (result > 0) return Ok("Student deleted successfully");
            if (result > 0) return Ok();

            return BadRequest();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditStudent(int id, Student student)
        {
            var studentFromDb = await _context.Students.FindAsync(id);

           /* if (studentFromDb is null) return BadRequest("Student not found");*/
            if (studentFromDb is null) return BadRequest(new Response { Status = "Error", Message = "Student not found" });

            studentFromDb.Name = student.Name;
            studentFromDb.Email = student.Email;
            studentFromDb.Address = student.Address;
            studentFromDb.PhoneNumber = student.PhoneNumber;

            var result = await _context.SaveChangesAsync();

            if (result > 0) return Ok();

            return BadRequest();
        }
    }

}
