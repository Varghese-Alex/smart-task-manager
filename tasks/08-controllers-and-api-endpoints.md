# Task 08 - Controllers And API Endpoints

## Goal

Create API controllers for authentication and task management.

By the end of this task, the API should expose the endpoints described in the design document.

## Why This Task Matters

Controllers are the HTTP boundary of the application.

They should be thin. Their job is to:

- Accept requests.
- Call services.
- Return HTTP responses.

They should not contain password hashing, database queries, or business rules.

## Steps

### 1. Create AuthController

In Solution Explorer, right-click `Controllers`, choose `Add -> Controller`, and select an empty API controller if Visual Studio offers controller templates.

Name it `AuthController.cs`.

Base attributes:

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
}
```

With this route, the controller name `AuthController` maps to:

```text
/api/auth
```

### 2. Add register endpoint

Endpoint:

```text
POST /api/auth/register
```

Controller action:

```csharp
[HttpPost("register")]
public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
{
    var result = await _authService.RegisterAsync(dto);
    return Ok(result);
}
```

Why:

The controller delegates registration logic to `IAuthService`.

### 3. Add login endpoint

Endpoint:

```text
POST /api/auth/login
```

Controller action:

```csharp
[HttpPost("login")]
public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
{
    var result = await _authService.LoginAsync(dto);
    return Ok(result);
}
```

Why:

The controller should not know how login is verified. It only returns the service result.

### 4. Create TasksController

In Solution Explorer, right-click `Controllers`, choose `Add -> Controller`, and select an empty API controller.

Name it `TasksController.cs`.

Base attributes:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
}
```

Why `[Authorize]`:

All task APIs require a valid JWT.

### 5. Add a helper to get current user id

Inside `TasksController`, add a private method:

```csharp
private int GetCurrentUserId()
{
    return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
```

Why:

The authenticated user's id comes from the validated JWT claims.

### 6. Add GET /api/tasks

Endpoint:

```text
GET /api/tasks?status=0&search=project&page=1&pageSize=10
```

Controller action should:

1. Read optional query parameters.
2. Get current user id.
3. Call `ITaskService.GetTasksAsync`.
4. Return `Ok(result)`.

Why:

Filtering and pagination are part of the API contract.

### 7. Add POST /api/tasks

Endpoint:

```text
POST /api/tasks
```

Controller action should:

1. Accept `TaskCreateDto`.
2. Get current user id.
3. Call service.
4. Return `CreatedAtAction` or `Ok`.

Prefer `CreatedAtAction` because a resource was created.

Why:

HTTP status code `201 Created` communicates the result more accurately than always returning `200 OK`.

### 8. Add PUT /api/tasks/{id}

Endpoint:

```text
PUT /api/tasks/{id}
```

Controller action should:

1. Accept `id`.
2. Accept `TaskUpdateDto`.
3. Get current user id.
4. Call service.
5. Return `NotFound` if service returns null.
6. Return `Ok(updatedTask)` otherwise.

Why:

If a task does not exist or does not belong to the user, the API should not update it.

### 9. Add DELETE /api/tasks/{id}

Endpoint:

```text
DELETE /api/tasks/{id}
```

Controller action should:

1. Accept `id`.
2. Get current user id.
3. Call service.
4. Return `NotFound` if false.
5. Return `NoContent` if true.

Why:

`204 No Content` is a common successful delete response.

## Completion Criteria

You are done when:

- `AuthController` exists.
- Register endpoint exists.
- Login endpoint exists.
- `TasksController` exists.
- Task endpoints require authorization.
- Current user id is read from JWT claims.
- Controllers call services instead of repositories directly.
- API uses appropriate HTTP status codes.

## Learning Notes

Thin controllers are easier to test and easier to change.

If your controller action becomes long, it is usually a sign that logic belongs in a service.
