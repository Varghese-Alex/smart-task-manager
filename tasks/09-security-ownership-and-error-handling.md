# Task 09 - Security, Ownership, And Error Handling

## Goal

Strengthen the backend by enforcing ownership rules and handling errors in a clean and secure way.

By the end of this task, the API should:

* Prevent users from accessing others’ data
* Return clean error responses
* Avoid exposing internal details

---

## Why This Task Matters

Even if APIs work, they can still be unsafe.

```text
Correct logic ≠ Secure system
```

This task ensures:

```text
User → can ONLY access their own tasks
```

---

## Steps

---

### 1. Enforce Ownership Everywhere

Ensure all task queries include `userId`.

📁 `Repositories/TaskRepository.cs`

```csharp
// ✅ Correct
.Where(task => task.UserId == userId)

// ❌ Wrong
.Where(task => task.Id == id)
```

---

### 2. Do NOT Accept UserId from Client

📁 `DTOs/Tasks/*`

```csharp
// ❌ DO NOT include
public int UserId { get; set; }
```

---

### 3. Handle Duplicate Email Registration

📁 `Exceptions/BadRequestException.cs`

```csharp
namespace SmartTaskManager.Api.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
```

---

📁 `Services/AuthService.cs`

```csharp
var existingUser = await _context.Users
    .FirstOrDefaultAsync(u => u.Email == dto.Email);

if (existingUser != null)
{
    throw new BadRequestException("Email already registered.");
}
```

---

### 4. Handle Invalid Login (401 Unauthorized)

📁 `Exceptions/UnauthorizedException.cs`

```csharp
namespace SmartTaskManager.Api.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
```

---

📁 `Services/AuthService.cs`

```csharp
if (user == null)
    throw new UnauthorizedException("Invalid email or password.");

var result = _passwordHasher.VerifyHashedPassword(
    user, user.PasswordHash, dto.Password);

if (result == PasswordVerificationResult.Failed)
    throw new UnauthorizedException("Invalid email or password.");
```

```text
Do NOT reveal whether email or password is incorrect
```

---

### 5. Validate Task Status (Service Layer)

📁 `Services/TaskService.cs`

Inside `UpdateTaskAsync`:

```csharp
if (!Enum.IsDefined(typeof(TaskItemStatus), dto.Status))
{
    throw new BadRequestException("Invalid task status.");
}
```

---

### 6. Add Global Exception Handling Middleware

📁 `Middleware/ExceptionHandlingMiddleware.cs`

```csharp
using System.Net;
using System.Text.Json;
using SmartTaskManager.Api.Exceptions;

namespace SmartTaskManager.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message
                }));
            }
            catch (UnauthorizedException ex)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message
                }));
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = "An unexpected error occurred."
                }));
            }
        }
    }
}
```

---

### 7. Register Middleware

📁 `Program.cs`

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
```

---

## 🔥 Important Concepts

---

### Ownership Security

```text
task.Id + userId → ensures data isolation
```

---

### Exception Flow

```text
Service → throws exception
Middleware → catches it
Client → receives clean response
```

---

### Clean API Errors

```json
{
  "message": "Invalid email or password."
}
```

---

### HTTP Status Codes

| Scenario         | Status |
| ---------------- | ------ |
| Success          | 200    |
| Create           | 201    |
| Delete           | 204    |
| Validation Error | 400    |
| Unauthorized     | 401    |
| Not Found        | 404    |
| Server Error     | 500    |

---

## Completion Criteria

* Ownership enforced in all queries
* DTOs do not accept UserId
* Duplicate email handled
* Invalid login returns 401
* Task status validated
* Middleware handles exceptions globally
* Clean error responses returned

---

## Commit Your Work

```bash
git add .
git commit -m "feat(task-09): add security checks, ownership validation, and structured exception handling"
git push
```

---

## Next Step

👉 Task 10 - Final Improvements and Production Readiness
