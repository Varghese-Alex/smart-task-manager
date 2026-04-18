using SmartTaskManager.Api.DTOs;
using SmartTaskManager.Api.DTOs.Tasks;
using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResponseDto<TaskResponseDto>> GetTasksAsync(
            int userId,
            TaskItemStatus? status,
            string? search,
            int page,
            int pageSize);

        Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto dto);

        Task<TaskResponseDto?> UpdateTaskAsync(
            int userId,
            int taskId,
            TaskUpdateDto dto);

        Task<bool> DeleteTaskAsync(int userId, int taskId);
    }
}