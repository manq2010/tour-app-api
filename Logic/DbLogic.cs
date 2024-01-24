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
                var user = _context.User.FirstOrDefault(a => a.Id == id && !a.isdeleted);

                if (user != null)
                {
                    user.isdeleted = true;
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

        public List<Gender> GetAllGenders()
        {
            IQueryable<Gender> toReturn;

            try
            {
                toReturn = _context.Genders.AsQueryable();
            }
            catch (Exception err)
            {
                throw;
            }
            return toReturn.ToList();
        }

        public bool LeaderExist(Leader leader)
        {
            try
            {
                var cnt = _context.Leaders.Where(d => d.Email == leader.Email && leader.isdeleted == false).Count();

                if (cnt > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }

        public string PostInsertNewLeader(Leader leader)
        {
            var message = "";

            if (leader.Id == 0)
            {
                try
                {
                    leader.created_at = DateTime.Now;
                    leader.updated_at = DateTime.Now;
                    leader.isdeleted = false;
                    _context.Leaders.Add(leader);
                    _context.SaveChanges();
                    message = "Success";
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                leader.updated_at = DateTime.Now;
                leader.isdeleted = false;
                _context.Leaders.Update(leader);
                _context.SaveChanges();
                message = "Success";
            }

            return message;
        }

        public string DeleteLeader(int id)
        {
            var message = "";

            try
            {
                var leader = _context.Leaders.First(a => a.Id == id);

                leader.isdeleted = true;
                leader.deleted_at = DateTime.Now;

                _context.SaveChanges();
                message = "Success";
            }
            catch (Exception e)
            {
                throw e;
            }

            return message;
        }

        public DocumentLeader InsertUpdateDocLeader(DocumentLeader item)
        {
            bool insertMode = item.Id == 0;

            try
            {
                if (item != null)
                {
                    var clpExist = _context.DocumentsLeader.FirstOrDefault(f => (f.LeaderId == item.LeaderId) && (f.DocTypeName == item.DocTypeName));

                    if (clpExist != null)
                        insertMode = item.Id == 0;

                    if (insertMode)
                    {

                        _context.DocumentsLeader.Add(item);
                    }
                    else
                    {
                        var local = _context.Set<DocumentLeader>()
                    .Local
                    .FirstOrDefault(f => (f.LeaderId == item.LeaderId) && (f.DocTypeName == item.DocTypeName));
                        if (local != null)
                        {
                            _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                        }
                        _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

            return (item);
        }

        public DocumentLeader GetDocLeaderById(int Id)
        {
            DocumentLeader item = new DocumentLeader();

            try 
            {
                item = _context.DocumentsLeader.Where(d => d.Id == Id).FirstOrDefault();
            } 
            catch (Exception err) 
            { 

            }            

            return (item);
        }

        public List<DocumentLeader> GetLeaderFilesListById(int Id)
        {
            List<DocumentLeader> items = new List<DocumentLeader>();

            try
            {
                items = _context.DocumentsLeader.Where(d => d.LeaderId == Id && d.isdeleted==false).ToList();
            }
            catch (Exception err)
            {

            }
            return (items);
        }

        public string DeleteDocLeader(int id)
        {
            var message = "";

            try
            {
                var doc = _context.DocumentsLeader.First(a => a.Id == id);

                doc.isdeleted = true;
                doc.deleted_at = DateTime.Now;

                _context.SaveChanges();
                message = "Success";
            }
            catch (Exception e)
            {
                throw e;
            }

            return message;
        }



    }
}
