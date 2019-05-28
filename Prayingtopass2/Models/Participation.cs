using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prayingtopass.Models
{
    public class Participation
    {
        [Key]
        public int ParticipationId {get; set;}
        public int UserId {get; set;}
        public int ActivityId {get; set;}
        public User User {get; set;}
        public Actvty Activity {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
    }
}