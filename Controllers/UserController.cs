using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Context;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context) => _context = context;

        /// <summary>
        /// Retrieves a list of all users from the database.
        /// </summary>
        /// <remarks>
        /// This method asynchronously retrieves all user records from the database and returns them as a list.
        /// If there is an error during the retrieval process, a bad request response is returned with the error message.
        /// </remarks>
        /// <returns>
        /// An IActionResult containing the list of users if successful, or a BadRequest with an error message if failed.
        /// </returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="400">If there is an error retrieving the users, returns a BadRequest with the error message.</response>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving users", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new user if the ID is 0, or updates an existing user if the ID is greater than 0.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object containing user details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created or updated user.
        /// </returns>
        /// <response code="200">Returns the created or updated user successfully.</response>
        /// <response code="400">If there was an error while saving the user.</response>
        [HttpPost]
        public async Task<IActionResult> UpsertUser(User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    _context.Users.Add(user);
                }
                else
                {
                    _context.Users.Update(user);
                }
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error saving user", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user from the database by their unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the user to be deleted.</param>
        /// <remarks>
        /// This method attempts to find and delete the user with the specified ID from the database.
        /// If the user is not found, it returns a 404 (Not Found) response with an appropriate message.
        /// If an error occurs during the deletion process, it returns a 400 (Bad Request) response with the error message.
        /// </remarks>
        /// <returns>
        /// An IActionResult containing the deleted user if successful, or a BadRequest with the error message if failed.
        /// </returns>
        /// <response code="200">Returns the deleted user object if the operation is successful.</response>
        /// <response code="400">If there is an error deleting the user, returns a BadRequest with the error message.</response>
        /// <response code="404">If the user with the specified ID is not found, returns a NotFound response.</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                var user = await _context.Users.FindAsync(Id);
                if (user == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error deleting user", error = ex.Message });
            }
        }
    }
}
