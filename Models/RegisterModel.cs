using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Touring.api.Models
{
    [NotMapped]
    public class RegisterModel
    {
       [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
      
        public string UserRole { get; set; }
    }
}
