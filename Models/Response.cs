using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Touring.api.Models
{
    [NotMapped]
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public IdentityResult Detail { get; set; }
        public object DetailDescription { get; set; }
    }
}
