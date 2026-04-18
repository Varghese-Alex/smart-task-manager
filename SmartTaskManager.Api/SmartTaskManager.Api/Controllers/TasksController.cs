using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SmartTaskManager.Api.DTOs.Tasks;
using SmartTaskManager.Api.Models;
using SmartTaskManager.Api.Services.Interfaces;

namespace SmartTaskManager.Api.Controllers
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

        // 🔥 Helper: Get user id from JWT
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // =====================================
        // GET: api/tasks
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetTasks(
            [FromQuery] TaskItemStatus? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();

            var result = await _taskService.GetTasksAsync(
                userId, status, search, page, pageSize);

            return Ok(result);
        }

        // =====================================
        // POST: api/tasks
        // =====================================
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
        {
            var userId = GetCurrentUserId();

            var created = await _taskService.CreateTaskAsync(userId, dto);

            return CreatedAtAction(
                nameof(GetTasks),
                new { id = created.Id },
                created);
        }

        // =====================================
        // PUT: api/tasks/{id}
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id,
            [FromBody] TaskUpdateDto dto)
        {
            var userId = GetCurrentUserId();

            var updated = await _taskService.UpdateTaskAsync(userId, id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // =====================================
        // DELETE: api/tasks/{id}
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();

            var deleted = await _taskService.DeleteTaskAsync(userId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}