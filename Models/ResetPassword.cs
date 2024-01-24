using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Touring.api.Models
{
    [NotMapped]
    public class ResetPasswordModel
   {
        public string email { get; set; }
        public string token { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
    }
}
