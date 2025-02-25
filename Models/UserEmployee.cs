using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserEmployee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } // Employee ID

        [Required]
        public string UserName { get; set; } // Assignee Name
    }

}
