using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Prayingtopass.Models {
    public class User {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Display (Name = "First Name")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "First Name must only contain letters")]
        public string FirstName { get; set; }

        [Required]
        [Display (Name = "Last Name")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Last Name must only contain letters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength (8, ErrorMessage = "Password Must be 8 character or longer!")]
        [RegularExpression (@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must contain at least 1 letter, 1 number, and 1 special character!")]
        [DataType (DataType.Password)]
        public string Password { get; set; }

        public List<Participation> AttendedActs { get; set; } //the list of activities the user is attending
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        [Required]
        [Compare ("Password")]
        [DataType (DataType.Password)]
        [Display (Name = "Password Confirmation")]
        public string Confirm { get; set; }

    }
}