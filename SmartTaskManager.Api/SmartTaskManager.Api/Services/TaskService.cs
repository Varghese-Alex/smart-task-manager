using SmartTaskManager.Api.DTOs;
using SmartTaskManager.Api.DTOs.Tasks;
using SmartTaskManager.Api.Exceptions;
using SmartTaskManager.Api.Models;
using SmartTaskManager.Api.Repositories.Interfaces;
using SmartTaskManager.Api.Services.Interfaces;

namespace SmartTaskManager.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponseDto<TaskResponseDto>> GetTasksAsync(
            int userId,
            TaskItemStatus? status,
            string? search,
            int page,
            int pageSize)
        {
            // Normalize pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var tasks = await _repository.GetTasksAsync(
                userId, status, search, page, pageSize);

            var totalCount = await _repository.GetTaskCountAsync(
                userId, status, search);

            return new PagedResponseDto<TaskResponseDto>
            {
                Data = tasks.Select(MapToResponse).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<TaskResponseDto> CreateTaskAsync(
            int userId,
            TaskCreateDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = TaskItemStatus.Pending,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(task);

            return MapToResponse(created);
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(
            int userId,
            int taskId,
            TaskUpdateDto dto)
        {
            var task = await _repository.GetByIdAsync(taskId, userId);

            if (task == null)
                return null;

            if (!Enum.IsDefined(typeof(TaskItemStatus), dto.Status))
            {
                throw new BadRequestException("Invalid task status.");
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Status = dto.Status;

            await _repository.UpdateAsync(task);

            return MapToResponse(task);
        }

        public async Task<bool> DeleteTaskAsync(int userId, int taskId)
        {
            var task = await _repository.GetByIdAsync(taskId, userId);

            if (task == null)
                return false;

            await _repository.DeleteAsync(task);

            return true;
        }

        // Mapping helper
        private static TaskResponseDto MapToResponse(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status
            };
        }
    }
}