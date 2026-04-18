# Task 09 - Security, Ownership, And Error Handling

## Goal

Strengthen the backend by handling common security and error cases consistently.

By the end of this task, the API should avoid obvious security mistakes and return understandable errors.

## Why This Task Matters

An API can be functionally correct but still unsafe.

For this project, the most important security rule is:

```text
Users can only access their own tasks.
```

This task makes that rule explicit and checks the places where it can accidentally be broken.

## Steps

### 1. Verify ownership filtering everywhere

Check each task operation:

- List tasks.
- Get task by id, if implemented.
- Update task.
- Delete task.

Every task lookup should include `userId`.

Good:

```csharp
task.Id == taskId && task.UserId == userId
```

Risky:

```csharp
task.Id == taskId
```

Why:

Task ids are often predictable integers. Without user filtering, one user could modify another user's task by guessing an id.

### 2. Do not accept UserId in task DTOs

Confirm that `TaskCreateDto` and `TaskUpdateDto` do not include `UserId`.

Why:

The backend should derive ownership from the JWT, not from client input.

### 3. Handle duplicate registration emails

During registration, if email already exists, return a clear error.

Suggested behavior:

```text
400 Bad Request
Email already registered.
```

Why:

The database has a unique index, but the service should still detect the case and return a friendly API error.

### 4. Handle invalid login

If email or password is wrong, avoid revealing which one failed.

Suggested response:

```text
401 Unauthorized
Invalid email or password.
```

Why:

Returning "email not found" can help attackers discover registered accounts.

### 5. Validate task status

Only allow known status values:

```text
0 = Pending
1 = Completed
```

If using an enum, validate with:

```csharp
Enum.IsDefined(typeof(TaskItemStatus), dto.Status)
```

Why:

Without validation, clients can store invalid states like `99`.

### 6. Add consistent exception handling

For a learning project, you can start with simple try/catch behavior in services or controllers.

A cleaner production-style approach is custom middleware:

```text
Middleware/ExceptionHandlingMiddleware.cs
```

Middleware can catch unhandled exceptions and return:

```json
{
  "message": "An unexpected error occurred."
}
```

Why:

Unhandled exceptions should not expose stack traces or internal details to API clients.

### 7. Use proper HTTP status codes

Recommended status codes:

```text
200 OK - successful read or update
201 Created - successful create
204 No Content - successful delete
400 Bad Request - validation or business rule failure
401 Unauthorized - missing or invalid authentication
404 Not Found - task does not exist or does not belong to user
500 Internal Server Error - unexpected server failure
```

Why:

HTTP status codes are part of your API's communication contract.

### 8. Protect secrets

Do not commit real JWT secrets or production connection strings.

For local learning, `appsettings.json` can contain local values. For real deployment, use:

- Environment variables.
- User secrets.
- Cloud secret manager.

Why:

Secrets in source control can leak and compromise the system.

## Completion Criteria

You are done when:

- Every task query is scoped by user id.
- Task DTOs do not accept `UserId`.
- Duplicate registration is handled.
- Invalid login returns a generic error.
- Invalid task status is rejected.
- Unexpected errors do not expose sensitive details.
- HTTP status codes are intentional and consistent.

## Learning Notes

Security is mostly about refusing to trust the wrong thing.

For this app:

- Trust the validated JWT for identity.
- Do not trust request bodies for ownership.
- Do not trust ids without checking the owner.
- Do not trust client-provided status values without validation.

