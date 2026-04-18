# Task 10 - Testing And Manual Verification

## Goal

Verify that the backend works correctly through manual API testing and, optionally, automated tests.

By the end of this task, you should have confidence that registration, login, and task operations work together.

## Why This Task Matters

Code that compiles is not automatically correct.

Testing checks whether the API behaves as expected from a user's point of view. It also catches mistakes in authentication, ownership, validation, and database logic.

## Steps

### 1. Run the application

Start the API from Visual Studio:

1. Set `SmartTaskManager.Api` as the startup project.
2. Select the HTTPS launch profile.
3. Press `F5` to debug or `Ctrl+F5` to run without debugging.

Visual Studio should open Swagger automatically. If it does not, open:

```text
https://localhost:xxxx/swagger
```

### 2. Test registration

Call:

```text
POST /api/auth/register
```

Body:

```json
{
  "email": "test@mail.com",
  "password": "123456"
}
```

Expected:

- Status code is `200 OK` or `201 Created`.
- Response contains a token.
- Response contains the email.
- Database contains a new user.
- Password is stored as a hash, not plain text.

### 3. Test duplicate registration

Send the same registration request again.

Expected:

- Request fails.
- Error message is understandable.
- Database still has only one user with that email.

### 4. Test login

Call:

```text
POST /api/auth/login
```

Body:

```json
{
  "email": "test@mail.com",
  "password": "123456"
}
```

Expected:

- Status code is `200 OK`.
- Response contains a JWT token.

### 5. Authorize Swagger

In Swagger:

1. Click the authorize button.
2. Enter the JWT token.
3. If Swagger expects a bearer value, enter:

```text
Bearer YOUR_TOKEN_HERE
```

Why:

Task endpoints require authentication.

### 6. Test task creation

Call:

```text
POST /api/tasks
```

Body:

```json
{
  "title": "Finish project",
  "description": "Complete backend",
  "dueDate": "2026-04-20"
}
```

Expected:

- Status code is `201 Created` or `200 OK`.
- Response contains task id.
- Task belongs to the authenticated user.
- Status defaults to pending.

### 7. Test task listing

Call:

```text
GET /api/tasks?page=1&pageSize=10
```

Expected:

- Response contains `data`.
- Response contains `totalCount`.
- Created task appears in `data`.

### 8. Test filtering

Try:

```text
GET /api/tasks?status=0
GET /api/tasks?search=project
```

Expected:

- Status filter returns tasks with matching status.
- Search returns tasks with matching title or description.

### 9. Test task update

Call:

```text
PUT /api/tasks/{id}
```

Body:

```json
{
  "title": "Finish project",
  "description": "Backend completed",
  "dueDate": "2026-04-20",
  "status": 1
}
```

Expected:

- Status code is `200 OK`.
- Response shows updated values.
- Status changed to completed.

### 10. Test task deletion

Call:

```text
DELETE /api/tasks/{id}
```

Expected:

- Status code is `204 No Content`.
- Task no longer appears in list results.

### 11. Test unauthorized access

Call a task endpoint without a token.

Expected:

- Status code is `401 Unauthorized`.

Why:

This confirms `[Authorize]` is working.

### 12. Test ownership protection

Create a second user.

Try to access or update the first user's task while authenticated as the second user.

Expected:

- The task should not be visible.
- Update or delete should return `404 Not Found`.

Why:

This confirms user data isolation.

## Optional Automated Tests

After manual testing works, consider adding automated tests.

Useful test categories:

- Auth service registration succeeds.
- Auth service rejects duplicate email.
- Auth service rejects invalid login.
- Task service creates tasks for the correct user.
- Task service prevents cross-user updates.
- Task service applies pagination.

## Completion Criteria

You are done when:

- Registration works.
- Duplicate registration fails safely.
- Login works.
- Authenticated task creation works.
- Task listing, filtering, updating, and deleting work.
- Unauthenticated task requests fail.
- Cross-user task access is blocked.

## Learning Notes

Manual tests teach the API flow.

Automated tests protect that flow from future changes. Start manually so you understand the behavior, then automate the important cases.
