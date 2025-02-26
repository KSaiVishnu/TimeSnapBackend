using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class TaskStatus
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
    }
    public class Assignee
{
    public string? AssigneeName { get; set; }
    public string? EmpId { get; set; }
}

public class UpdateTaskRequest
{
    public List<Assignee>? Assignees { get; set; }
}



    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadTasks([FromBody] List<TaskModel> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return BadRequest(new { message = "Invalid file data" });
            }

            foreach (var task in tasks)
            {
                //task.AssigneeNames = string.Join(",", task.AssigneeNames); // Convert array to string
                //task.StartDate = DateTime.FromOADate(Convert.ToDouble(task.StartDate));
                //task.DueDate = DateTime.FromOADate(Convert.ToDouble(task.DueDate));
                _context.Tasks.Add(task);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Data inserted successfully" });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [AllowAnonymous]
        [HttpGet("status/{empId}")]
        public async Task<IActionResult> GetTaskStatus(string empId)
        {
            var filteredTasks = await _context.Tasks.Where(t => t.EmpId == empId).ToListAsync();

            var taskStatusCounts = filteredTasks
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(), // Convert Enum to string
                    Count = g.Count()
                })
                .ToList();

            return Ok(taskStatusCounts);
        }

        [AllowAnonymous]
        [HttpGet("{empId}")]
        public async Task<IActionResult> GetTasksByEmpId(string empId)
        {
            if (string.IsNullOrEmpty(empId))
            {
                return BadRequest(new { message = "Employee Id is Required" });
            }
            //var tasks = await _context.Tasks.ToListAsync();
            var tasks = await _context.Tasks.Where(t => t.EmpId!.Equals(empId)).ToListAsync();
            var groupedTasks = tasks.GroupBy(t => t.Status).Select(g => new
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                Tasks = g.ToList(),
            }).ToList();


            return Ok(groupedTasks);
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateTaskById(int Id, [FromBody] TaskModel updatedTask)
        {
            if (Id != updatedTask.Id)
                return BadRequest("Task ID mismatch");

            var existingTask = await _context.Tasks.FindAsync(Id);
            if (existingTask == null)
                return NotFound();

            existingTask.Status = updatedTask.Status;

            await _context.SaveChangesAsync();
            return Ok(existingTask);
        }


        [AllowAnonymous]
        [HttpGet("{taskId}/timesheets")]
        public async Task<IActionResult> GetTimesheetsByTaskId(int taskId)
        {
            //var timesheets = await _context.Timesheets
            //                               .Where(t => t.TaskId == taskId)
            //                               .ToListAsync();

            var timesheets = await _context.Timesheets
    .Join(_context.Tasks,
          ti => ti.TaskId,
          ta => ta.Id,
          (ti, ta) => new
          {
              ti.Id,
              ti.EmpId,
              ti.TaskId,
              ti.Date,
              //t.EndTime,
              ti.TotalMinutes,
              UserName = ta.Assignee,  // Fetch username from AspNetUsers
              TaskName = ta.Task,
              ta.BillingType,
              ta.StartDate,
              ta.DueDate
          })
    .Where(ti => ti.TaskId == taskId)
    .ToListAsync();

            if (timesheets == null || !timesheets.Any())
            {
                return NotFound(new { message = "No timesheets found for this task!" });
            }

            return Ok(timesheets);
        }

        [AllowAnonymous]
        [HttpPut("update-task/{taskID}")]
        public async Task<IActionResult> UpdateTaskAssignees(string taskID, [FromBody] UpdateTaskRequest request)
        {
            var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();

            if (!taskRecords.Any()) return NotFound("Task not found.");

            var existingAssignees = taskRecords.Select(t => new { t.Assignee, t.EmpId }).ToHashSet();

            var newAssignees = request.Assignees!
                .Where(a => !existingAssignees.Any(e => e.EmpId == a.EmpId))
                .ToList();

            foreach (var assignee in newAssignees)
            {
                //if (string.IsNullOrEmpty(assignee.EmpId))
                //{
                //    var employee = await _context.UserEmployees.FirstOrDefaultAsync(u => u.UserName == assignee.AssigneeName);
                //    if (employee != null)
                //        assignee.EmpId = employee.EmployeeId;
                //}

                if (!string.IsNullOrEmpty(assignee.EmpId))
                {
                    _context.Tasks.Add(new TaskModel
                    {
                        TaskID = taskID,
                        Task = taskRecords.First().Task,
                        Assignee = assignee.AssigneeName,
                        EmpId = assignee.EmpId,
                        StartDate = taskRecords.First().StartDate,
                        DueDate = taskRecords.First().DueDate,
                        BillingType = taskRecords.First().BillingType,
                        Status = 0
                    });
                }
            }

            var assigneesToRemove = existingAssignees
                .Where(e => request.Assignees!.Any(a => a.EmpId == e.EmpId))
                .ToList();

            foreach (var assignee in assigneesToRemove)
            {
                var taskToRemove = taskRecords.FirstOrDefault(t => t.EmpId == assignee.EmpId);
                if (taskToRemove != null)
                    _context.Tasks.Remove(taskToRemove);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Task Updated Successfully" });
        }


        [AllowAnonymous]
        [HttpDelete("delete-task/{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
            var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskId).ToListAsync();
            foreach(var task in taskRecords)
            {
                _context.Tasks.Remove(task);
            }


            await _context.SaveChangesAsync();
            //return Ok(taskRecords);
            return Ok(new { message = "Task Deleted Successfully" });
        }

    }




}