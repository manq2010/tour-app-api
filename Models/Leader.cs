using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touring.api.Models
{
    public class Leader
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string CellNumber { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }

        [Required]
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }

        public virtual List<DocumentLeader> DocumentsLeader { get; set; } = new List<DocumentLeader>();
  
    }
}
