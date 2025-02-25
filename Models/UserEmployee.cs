using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserEmployee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string EmployeeId { get; set; } // Employee ID

        [Required]
        public required string UserName { get; set; } // Assignee Name
    }

}
