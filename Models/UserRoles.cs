using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Touring.api.Models
{
    public class UserRoles
    {
        public const string SystemAdmin = "SystemAdmin";
        public const string BasicUser = "BasicUser";
    }
}
