using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Touring.api.Models
{
	public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public bool IsAdminUser { get; set; }

        public bool isdeleted { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }

    public class User
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string token { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public bool deleted { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}
