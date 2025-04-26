using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Context;
using TaskManagement.Models;


namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TaskController(AppDbContext context) => _context = context;

        /// <summary>
        /// Retrieves a list of all tasks with their assigned users.
        /// </summary>
        /// <returns>
        /// A list of <see cref="TaskItem"/> objects wrapped in an <see cref="ActionResult"/>.
        /// </returns>
        /// <response code="200">Returns the list of tasks successfully.</response>
        /// <response code="400">If there was an error retrieving the tasks.</response>
        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetTasks()
        {
            try
            {
                var tasks = await _context.Tasks.Include(t => t.User).ToListAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving tasks", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new task or updates an existing task based on the task ID.
        /// </summary>
        /// <param name="task">The <see cref="TaskItem"/> object containing task details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created or updated task.
        /// </returns>
        /// <response code="200">Returns the created or updated task successfully.</response>
        /// <response code="400">If there was an error creating or updating the task.</response>
        [HttpPost]
        public async Task<IActionResult> UpsertTask(TaskItem task)
        {
            try
            {
                if (task.Id == 0)
                {
                    _context.Tasks.Add(task);
                }
                else
                {
                    _context.Tasks.Update(task);
                }
                await _context.SaveChangesAsync();
                return Ok(task);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "Error creating task", error = ex.Message });
            }
        }


        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="Id">The ID of the <see cref="TaskItem"/> to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// </returns>
        /// <response code="200">Returns the deleted task if successful.</response>
        /// <response code="404">If the task with the specified ID is not found.</response>
        /// <response code="400">If there was an error while deleting the task.</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int Id)
        {
            try
            {
               var task = await _context.Tasks.FindAsync(Id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "Error deleting task", error = ex.Message });
            }
        }

    }
}
