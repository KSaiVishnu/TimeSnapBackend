using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string? FullName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string? EmpId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Timesheet>? Timesheets { get; set; }
    }
}
