using Microsoft.Extensions.Configuration;
using Touring.api.Data;
using Touring.api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Touring.api.Logic
{
    public class DbLogic
    {
        public AppDbContext _context;
        public IConfiguration _configuration { get; }

        public DbLogic(AppDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public DbLogic(AppDbContext applicationDbContext, IConfiguration configuration)
        {
            _context = applicationDbContext;
            _configuration = configuration;
        }

        public string DeleteUser(string id)
        {
            var message = "";
            try
            {
                var user = _context.User.FirstOrDefault(a => a.Id == id && !a.deleted);

                if (user != null)
                {
                    user.deleted = true;
                    user.deleted_at = DateTime.Now;

                    _context.SaveChanges();
                    message = "Success";
                }
                else
                {
                    message = "User not found.";
                }
            }
            catch (Exception e)
            {
                // Log or handle the exception appropriately
                message = "An error occurred while deleting the user.";
                // throw e;
            }
            return message;
        }

    }
}
