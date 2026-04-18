# Task 05 - Repository Layer

## Goal

Create a repository abstraction for task data access.

By the end of this task, task-related database queries should live in `TaskRepository`, not in controllers or services directly.

## Why This Task Matters

The repository layer gives the service layer a clean way to ask for data without knowing the details of Entity Framework Core queries.

This keeps responsibilities clear:

```text
Service: decides what should happen
Repository: knows how to fetch or save data
```

This project uses a repository because the high-level design explicitly includes one.

## Steps

### 1. Create the repository interface

In Solution Explorer, right-click `Repositories/Interfaces`, choose `Add -> New Item`, select `Interface`, and name it `ITaskRepository.cs`.

Suggested methods:

```csharp
public interface ITaskRepository
{
    Task<List<TaskItem>> GetTasksAsync(
        int userId,
        int? status,
        string? search,
        int page,
        int pageSize);

    Task<int> GetTaskCountAsync(
        int userId,
        int? status,
        string? search);

    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<TaskItem> AddAsync(TaskItem taskItem);
    Task UpdateAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
}
```

Why include `userId`:

Every task query must be scoped to the authenticated user. This prevents one user from reading or changing another user's tasks.

### 2. Implement TaskRepository

In Solution Explorer, right-click `Repositories`, choose `Add -> Class`, and name it `TaskRepository.cs`.

The repository should accept `AppDbContext` through constructor injection:

```csharp
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }
}
```

Why:

Dependency injection lets ASP.NET Core create the repository with the correct database context.

### 3. Build a reusable query

For list and count operations, start from:

```csharp
var query = _context.TaskItems
    .Where(task => task.UserId == userId);
```

Then apply optional filters:

```csharp
if (status.HasValue)
{
    query = query.Where(task => task.Status == status.Value);
}

if (!string.IsNullOrWhiteSpace(search))
{
    query = query.Where(task =>
        task.Title.Contains(search) ||
        (task.Description != null && task.Description.Contains(search)));
}
```

Why:

Filtering starts with ownership. Optional filters are added only when the client supplies them.

### 4. Add pagination

Use `Skip` and `Take`:

```csharp
var tasks = await query
    .OrderByDescending(task => task.CreatedAt)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

Why:

Pagination prevents the API from loading every task at once. This matters as data grows.

### 5. Add create, update, and delete methods

For create:

```csharp
_context.TaskItems.Add(taskItem);
await _context.SaveChangesAsync();
return taskItem;
```

For update:

```csharp
_context.TaskItems.Update(taskItem);
await _context.SaveChangesAsync();
```

For delete:

```csharp
_context.TaskItems.Remove(taskItem);
await _context.SaveChangesAsync();
```

Why:

These methods centralize database persistence.

### 6. Register the repository

In `Program.cs`:

```csharp
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
```

Why scoped:

Repositories use `AppDbContext`, and EF Core DbContext is normally scoped to one HTTP request.

## Completion Criteria

You are done when:

- `ITaskRepository` exists.
- `TaskRepository` exists.
- List queries filter by user.
- List queries support status, search, page, and page size.
- Count query matches the list filters.
- Create, update, and delete methods save changes.
- Repository is registered in dependency injection.

## Learning Notes

A repository should not contain business decisions like "is this password valid?" or "can this status transition happen?"

Those decisions belong in services. The repository's job is data access.
