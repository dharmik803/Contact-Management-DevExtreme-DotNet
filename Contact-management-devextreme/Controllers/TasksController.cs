using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contact_management_devextreme.Models;
using Microsoft.AspNetCore.Authorization;

namespace Contact_management_devextreme.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ContactDbContext _context;

        public TasksController(ContactDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.Include(t => t.AssignedTo).ToListAsync();
            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }

            // var task = await _context.Tasks.FindAsync(id);
            var task = await _context.Tasks
                             .Include(t => t.AssignedTo)
                             .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, Models.Task task)
        {
            if (id != task.TaskId)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Task>> PostTask(Models.Task task)
        {


            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ContactDbContext.Tasks'  is null.");
            }
            try
            {
                if (task.AssignedToId != null)
                {
                    var contact = await _context.Contacts.FindAsync(task.AssignedToId);
                    if (contact == null)
                    {
                        return NotFound("No contact found with that ID");
                    }
                    task.AssignedTo = contact;

                }

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                // return Ok();
                return CreatedAtAction("GetTask", new { id = task.TaskId }, task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
