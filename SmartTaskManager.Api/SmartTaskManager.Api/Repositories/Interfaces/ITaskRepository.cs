using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetTasksAsync(
            int userId,
            TaskItemStatus? status,
            string? search,
            int page,
            int pageSize);

        Task<int> GetTaskCountAsync(
            int userId,
            TaskItemStatus? status,
            string? search);

        Task<TaskItem?> GetByIdAsync(int id, int userId);

        Task<TaskItem> AddAsync(TaskItem taskItem);

        Task UpdateAsync(TaskItem taskItem);

        Task DeleteAsync(TaskItem taskItem);
    }
}