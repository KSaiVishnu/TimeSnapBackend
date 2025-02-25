using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Timesheet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string? EmpId { get; set; }  // FK to AppUser.EmpId

        [ForeignKey("EmpId")]
        [JsonIgnore]
        public virtual AppUser? Employee { get; set; }

        [Required]
        public int TaskId { get; set; } // FK to TaskModel.Id

        [ForeignKey("TaskId")]
        public virtual TaskModel? Task { get; set; }

        public DateTime? Date { get; set; }
        //public DateTime? EndTime { get; set; }
        public double TotalMinutes { get; set; }
    }
}
