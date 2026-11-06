
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using TaskManagementApi.DTOs;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET LOGGED-IN USER ID
        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
        }

        // CHECK ADMIN ROLE
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // CREATE TASK
        [HttpPost]
        public IActionResult CreateTask(TaskDto request)
        {
            var task = _taskService.CreateTask(
                request, GetUserId()
            );

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.Id },
                task
            );
        }

        // GET ALL TASKS
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _taskService.GetAllTasks(
                GetUserId(), IsAdmin()
            );

            return Ok(tasks);
        }

        // GET TASK BY ID
        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var task = _taskService.GetTaskById(
                id, GetUserId(), IsAdmin()
            );

            if (task == null)
            {
                return NotFound("Task not found");
            }

            return Ok(task);
        }

        // UPDATE TASK
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskDto request)
        {
            var task = _taskService.UpdateTask(
                id, request, GetUserId(), IsAdmin()
            );

            if (task == null)
            {
                return NotFound("Task not found");
            }

            return Ok(task);
        }

        // DELETE TASK - ADMIN ONLY
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTask(int id)
        {
            var deleted = _taskService.DeleteTask(id);

            if (!deleted)
            {
                return NotFound("Task not found");
            }

            return Ok("Task deleted successfully");
        }
    }
}
