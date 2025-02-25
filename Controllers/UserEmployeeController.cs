using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/user-employee")]
    [ApiController]
    public class UserEmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserEmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUserEmployees()
        {
            var userEmployees = await _context.UserEmployees.ToListAsync();
            return Ok(userEmployees);
        }

    }
}
