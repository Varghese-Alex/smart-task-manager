using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Api.Data;
using SmartTaskManager.Api.Models;
using SmartTaskManager.Api.Repositories.Interfaces;

namespace SmartTaskManager.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetTasksAsync(
            int userId,
            TaskItemStatus? status,
            string? search,
            int page,
            int pageSize)
        {
            var query = BuildQuery(userId, status, search);

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTaskCountAsync(
            int userId,
            TaskItemStatus? status,
            string? search)
        {
            var query = BuildQuery(userId, status, search);

            return await query.CountAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id, int userId)
        {
            return await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TaskItem> AddAsync(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
            return taskItem;
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            _context.TaskItems.Update(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }

        // Reusable query builder
        private IQueryable<TaskItem> BuildQuery(
            int userId,
            TaskItemStatus? status,
            string? search)
        {
            var query = _context.TaskItems
                .Where(t => t.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    (t.Description != null && t.Description.Contains(search)));
            }

            return query;
        }
    }
}