using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prayingtopass.Models
{
    public class Actvty
    {
        [Key]
        public int ActvtyId {get; set;}

        [Required]
        [MinLength(2, ErrorMessage = "Title must be 2 characters or longer")]
        public string Title {get; set;}

        [Required]
        [MinLength(10, ErrorMessage = "Description must be 10 characters or longer")]
        public string Description {get; set;}

        [Required]
        [FutureDateTime]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate {get; set;}

        [Required]
        [Display(Name = "Duration")]
        public int ActDuration {get; set;}

        [Required]
        public string ActUnit {get; set;}

        public int PlannerId {get; set;} //the UserId of the User (in session) who created the activity
        public List<Participation> ActivityAttendees {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
    public class FutureDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(((DateTime)value) <= DateTime.Now)
            {
                return new ValidationResult("Only dates/times in the future are allowed");
            }
            return ValidationResult.Success;
        }
    }
}
