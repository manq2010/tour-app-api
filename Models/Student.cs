﻿using System;
using System.ComponentModel.DataAnnotations;


namespace Touring.api.Models
{
	public class Student
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; } = string.Empty;
		public string? Address { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Email { get; set; }
	}
}
