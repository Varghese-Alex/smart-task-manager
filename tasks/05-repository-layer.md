# Task 05 - Repository Layer

## Goal

Create a repository abstraction for task data access.

By the end of this task, all task-related database queries should live in `TaskRepository`, not in controllers or services.

---

## Why This Task Matters

The repository layer separates **data access** from **business logic**.

```text
Service → decides what should happen
Repository → fetches and saves data
```

This keeps your code:

* Cleaner
* Easier to test
* Easier to maintain

---

## Steps

---

### 1. Create Repository Interface

📁 File: `Repositories/Interfaces/ITaskRepository.cs`

```csharp
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
```

---

### Why include `userId`

Every query is scoped to the logged-in user.

👉 Prevents users from accessing others' data

---

### 2. Implement TaskRepository

📁 File: `Repositories/TaskRepository.cs`

```csharp
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

        // 🔥 Reusable query builder
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
```

---

## 🔥 Important Concept: Query Composition

Instead of repeating filters:

```text
GetTasks → filter
GetCount → filter
```

We extract:

```text
BuildQuery() → reusable logic
```

👉 This avoids duplication and keeps logic consistent

---

### 3. Register Repository

📁 File: `Program.cs`

#### Add BELOW this line:

```csharp
builder.Services.AddDbContext<AppDbContext>(...)
```

👉 Add:

```csharp
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
```

---

### Why Scoped?

* One DbContext per request
* Safe and efficient
* Recommended by EF Core

---

## Completion Criteria

You are done when:

* Interface is created
* Repository is implemented
* Queries filter by user
* Status filter works
* Search filter works
* Pagination works
* Count matches filters
* CRUD methods work
* Repository is registered

---

## Learning Notes

A repository:

* ✔ Handles database queries
* ❌ Does NOT contain business logic

Examples of what should NOT be here:

* Password validation
* Business rules
* Status transitions

👉 Those belong in **Services**

---

## Commit Your Work

```bash
git add .
git commit -m "feat(task-05): implement task repository with filtering, search, and pagination"
git push
```

---

## Next Step

👉 Proceed to:

**Task 06 - Authentication Service and JWT**
