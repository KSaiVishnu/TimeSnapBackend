using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/timesheet")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimesheetController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Timesheet>> CreateTimesheet([FromBody] TimesheetDto timesheetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Retrieve the task and employee (AppUser) using the provided taskId and empId
            var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
            var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

            if (task == null)
                return NotFound("Task not found");
            if (employee == null)
                return NotFound("Employee not found");

            // Create and save the timesheet entry
            var timesheet = new Timesheet
            {
                EmpId = timesheetDto.EmpId,
                TaskId = timesheetDto.TaskId,
                Date = DateTime.SpecifyKind(timesheetDto.Date ?? DateTime.Now, DateTimeKind.Local),
                //EndTime = timesheetDto.EndTime,
                TotalMinutes = timesheetDto.TotalMinutes
            };

            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
        }

        
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEndTime(int id, [FromBody] EndTimeRequest request)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }

            // Update the endTime
            //timesheet.EndTime = request.EndTime;
            timesheet.Date = request.Date;
            timesheet.TotalMinutes = request.TotalMinutes;
            

            // Save changes to the database
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(new { message = "Timesheet Updated successfully" }); // ✅ Return data

        }


        [AllowAnonymous]
        [HttpPost("addlog")]
        public async Task<ActionResult<Timesheet>> AddTimeLog([FromBody] TimesheetDto timesheetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Retrieve the task and employee (AppUser) using the provided taskId and empId
            var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
            var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

            if (task == null)
                return NotFound("Task not found");
            if (employee == null)
                return NotFound("Employee not found");

            // Create and save the timesheet entry
            var timesheet = new Timesheet
            {
                EmpId = timesheetDto.EmpId,
                TaskId = timesheetDto.TaskId,
                Date = timesheetDto.Date.Value,
                //EndTime = timesheetDto.EndTime.Value,
                TotalMinutes = timesheetDto.TotalMinutes
            };

            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
        }


        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<Timesheet>> GetTimesheets()
        {
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
                      })
                .ToListAsync();

            if (!timesheets.Any())
            {
                return NotFound(new { message = "No timesheets found!" });
            }

            return Ok(timesheets);
        }


        [AllowAnonymous]
        [HttpDelete("{timeSheetId}")]
        public async Task<IActionResult> DeleteTask(int timeSheetId)
        {
            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
            var timeSheet = await _context.Timesheets.Where(t => t.Id == timeSheetId).ToListAsync();
            if (!timeSheet.Any())
            {
                return NotFound("Time Sheet Not Found");
            }
            else
            {
                _context.Timesheets.Remove(timeSheet[0]);
            }

            await _context.SaveChangesAsync();
            //return Ok(taskRecords);
            return Ok(new { message = "Time Sheet Deleted Successfully" });
        }



    }
}
