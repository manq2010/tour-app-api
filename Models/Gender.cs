using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touring.api.Models
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }
}
