using Microsoft.Extensions.Configuration;
using Touring.api.Data;
using Touring.api.Models;
using System;
using System.Collections.Generic;
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

         public void InsertUpdateUserProfile(UserProfile user)
        {
            // user.updated_at = DateTime.Now;
            user.isdeleted = false;
            user.deleted_at = null;
            bool insertMode = user.UserProfileId == 0;
            try
            {
                if (user != null)
                {
                    if (insertMode)
                    {
                        //check if user status is set
                        //if (user.userstatusid >0 )
                        //{
                        //    user.userstatusid_moddatetime = DateTime.Now;
                        //}
                        user.created_at = DateTime.Now;
                        user.updated_at = DateTime.Now;
                       
                        _context.UserProfiles.Add(user);
                    }
                    else
                    {
                        user.updated_at = DateTime.Now;

                        var local = _context.Set<UserProfile>()
                        .Local
                        .FirstOrDefault(f => f.UserProfileId == user.UserProfileId);

                      
                        if (local != null)
                        {
                            _context.Entry(local).State = EntityState.Detached;
                        }
                        _context.Entry(user).State = EntityState.Modified;
                    }

                    _context.SaveChanges();
                }



            }
            catch (Exception err)
            {
                string errMessage = err.Message;
                // Write to log

                //throw;
            }
        }

         public void DeleteUserProfileById(int id)
        {

            bool insertMode = id == 0;

            var record = _context.UserProfiles.Where(d => d.UserProfileId == id).FirstOrDefault();

            try
            {
                if (record != null)
                {
                    if (insertMode)
                    {
                        record.created_at = DateTime.Now;
                        //_context.manualrequests.Add(manrequest);
                    }
                    else
                    {
                        record.isdeleted = true;
                        record.deleted_at = DateTime.Now;
                        var local = _context.Set<UserProfile>()
                    .Local
                    .FirstOrDefault(f => f.UserProfileId == id);
                        if (local != null)
                        {
                            _context.Entry(local).State = EntityState.Detached;
                        }
                        _context.Entry(record).State = EntityState.Modified;
                    }

                    _context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                string errMessage = err.Message;
                // Write to log
                throw;
            }
        }



    }
}
