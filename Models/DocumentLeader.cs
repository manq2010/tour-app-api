﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Touring.api.Models
{
    public class DocumentLeader
    {
        [Key]
        public int Id { get; set; }
        public int LeaderId { get; set; }
        [Required]
        public string DocTypeName { get; set; }
        
        [NotMapped]
        public IFormFile file { get; set; }

        public string file_origname { get; set; }
        public string file_seqname { get; set; }
        public string file_url { get; set; }
        public string file_mimetype { get; set; }
        public long? file_size { get; set; }
        public string file_extention { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? isdeleted { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
