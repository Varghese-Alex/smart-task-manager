# Task 07 - Task Service Layer

## Goal

Create the task service that contains business logic for creating, reading, updating, and deleting tasks.

By the end of this task, task operations should be handled through `ITaskService` and `TaskService`.

## Why This Task Matters

The service layer is where application rules live.

Controllers should not decide how tasks are created. Repositories should not decide whether a user is allowed to update a task. The service layer coordinates those decisions.

## Steps

### 1. Create ITaskService

In Solution Explorer, right-click `Services/Interfaces`, choose `Add -> New Item`, select `Interface`, and name it `ITaskService.cs`.

Suggested methods:

```csharp
public interface ITaskService
{
    Task<PagedResponseDto<TaskResponseDto>> GetTasksAsync(
        int userId,
        int? status,
        string? search,
        int page,
        int pageSize);

    Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto dto);
    Task<TaskResponseDto?> UpdateTaskAsync(int userId, int taskId, TaskUpdateDto dto);
    Task<bool> DeleteTaskAsync(int userId, int taskId);
}
```

Why include `userId`:

The service must enforce that every operation happens for the authenticated user only.

### 2. Create TaskService

In Solution Explorer, right-click `Services`, choose `Add -> Class`, and name it `TaskService.cs`.

Inject:

- `ITaskRepository`

Why:

The service uses the repository to access data, but the service owns the business rules.

### 3. Implement task listing

The service should:

1. Normalize pagination values.
2. Ask repository for filtered tasks.
3. Ask repository for total count.
4. Map entities to response DTOs.
5. Return `PagedResponseDto<TaskResponseDto>`.

Example rules:

```text
If page < 1, use page = 1.
If pageSize < 1, use pageSize = 10.
If pageSize > 100, use pageSize = 100.
```

Why:

The API should protect itself from bad or expensive pagination requests.

### 4. Implement task creation

The service should:

1. Receive `userId` from the controller.
2. Receive task data from `TaskCreateDto`.
3. Create a `TaskItem` entity.
4. Set `UserId` from the authenticated user, not from the request body.
5. Set default status to pending.
6. Save using repository.
7. Return `TaskResponseDto`.

Why:

The client should never decide task ownership. Ownership comes from the authenticated token.

### 5. Implement task update

The service should:

1. Get the task by `taskId` and `userId`.
2. Return `null` if not found.
3. Update allowed fields only.
4. Validate status value.
5. Save using repository.
6. Return response DTO.

Why:

Fetching by both `taskId` and `userId` prevents cross-user updates.

### 6. Implement task deletion

The service should:

1. Get the task by `taskId` and `userId`.
2. Return `false` if not found.
3. Delete using repository.
4. Return `true`.

Why:

Delete must also respect ownership.

### 7. Add mapping helpers

You can map manually:

```csharp
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
```

Why manual mapping:

For a small learning project, manual mapping is explicit and easy to understand. Later, you can learn tools like AutoMapper if mapping becomes repetitive.

### 8. Register TaskService

In `Program.cs`:

```csharp
builder.Services.AddScoped<ITaskService, TaskService>();
```

## Completion Criteria

You are done when:

- `ITaskService` exists.
- `TaskService` exists.
- The service lists tasks with pagination.
- The service creates tasks for the current user.
- The service updates only tasks owned by the current user.
- The service deletes only tasks owned by the current user.
- Entities are mapped to response DTOs.
- Task service is registered in dependency injection.

## Learning Notes

The service layer is the center of the application.

If you are unsure where code belongs, ask:

- Is it HTTP-specific? Put it in the controller.
- Is it a business rule? Put it in the service.
- Is it database querying? Put it in the repository.
- Is it data shape for clients? Put it in DTOs.
