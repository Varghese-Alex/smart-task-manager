# Task 10 - Testing And Manual Verification

## Goal

Verify that the backend works correctly by testing all APIs end-to-end.

By the end of this task, you should be confident that:

* Authentication works
* Task operations work
* Security rules are enforced

---

## Why This Task Matters

Code running ≠ Code working correctly

```text
Testing = verifying real user behavior
```

This ensures:

```text
Auth → Works
Tasks → Work
Security → Works
```

---

## Steps

---

### 1. Run the Application

Start the API:

```text
F5 → Debug
or
Ctrl + F5 → Run
```

---

### 2. Set Up Postman

Use your collection:

```text
Smart Task Manager API
```

Set variables:

```text
baseUrl = https://localhost:xxxx
token   = (auto-filled after login)
```

---

### 3. Test Registration

📌 `POST /api/auth/register`

```json
{
  "email": "test@mail.com",
  "password": "123456"
}
```

---

### ✅ Expected

```text
200 OK
```

Response:

```json
{
  "email": "test@mail.com",
  "token": "..."
}
```

---

### 🔍 Verify

* User created in DB
* Password is hashed

---

### 4. Test Duplicate Registration

Send same request again.

---

### ✅ Expected

```text
400 Bad Request
```

```json
{
  "message": "Email already registered."
}
```

---

### 5. Test Login

📌 `POST /api/auth/login`

```json
{
  "email": "test@mail.com",
  "password": "123456"
}
```

---

### ✅ Expected

```text
200 OK
```

```json
{
  "email": "test@mail.com",
  "token": "..."
}
```

---

### 🔥 Token Handling

Token should be automatically saved:

```text
{{token}}
```

---

### 6. Test Invalid Login

```json
{
  "email": "test@mail.com",
  "password": "wrong"
}
```

---

### ✅ Expected

```text
401 Unauthorized
```

```json
{
  "message": "Invalid email or password."
}
```

---

### 7. Test Task Creation

📌 `POST /api/tasks`

```json
{
  "title": "Finish project",
  "description": "Complete backend",
  "dueDate": "2026-04-20"
}
```

---

### ✅ Expected

```text
201 Created
```

```json
{
  "id": 1,
  "title": "Finish project"
}
```

---

### 🔍 Verify

* Task belongs to logged-in user
* Status defaults to `Pending`

---

### 8. Test Task Listing

📌 `GET /api/tasks?page=1&pageSize=10`

---

### ✅ Expected

```json
{
  "data": [...],
  "totalCount": 1
}
```

---

### 9. Test Filtering

📌 Status filter:

```text
GET /api/tasks?status=0
```

📌 Search:

```text
GET /api/tasks?search=project
```

---

### ✅ Expected

* Correct filtered results

---

### 10. Test Task Update

📌 `PUT /api/tasks/{id}`

```json
{
  "title": "Finish project",
  "description": "Backend completed",
  "dueDate": "2026-04-20",
  "status": 1
}
```

---

### ✅ Expected

```text
200 OK
```

* Task updated
* Status = Completed

---

### 11. Test Task Deletion

📌 `DELETE /api/tasks/{id}`

---

### ✅ Expected

```text
204 No Content
```

* Task removed

---

### 12. Test Unauthorized Access

Call any task API **without token**

---

### ✅ Expected

```text
401 Unauthorized
```

---

### 13. Test Ownership Protection

1. Register second user
2. Login as second user
3. Try to access first user's task

---

### ✅ Expected

```text
404 Not Found
```

---

### 🔥 Why

```text
Tasks are scoped by userId
```

---

## 🔥 Important Concepts

---

### Full Flow

```text
Register → Login → Get Token → Access Tasks
```

---

### Security Flow

```text
JWT → Controller → Service → Repository
```

---

### Data Isolation

```text
User A ≠ User B data
```

---

## Completion Criteria

* Registration works
* Duplicate email handled
* Login works
* Invalid login returns 401
* Tasks can be created
* Tasks can be listed
* Tasks can be updated
* Tasks can be deleted
* Unauthorized requests fail
* Cross-user access is blocked

---

## Optional Next Step (Advanced)

After manual testing:

* Add unit tests
* Add integration tests

---

## Commit Your Work

```bash
git add .
git commit -m "feat(task-10): add manual testing verification for auth and task APIs"
git push
```

---

## Final Result

Your API is now:

```text
✔ Functional
✔ Secure
✔ Tested
✔ Production-ready (for learning project)
```

---

## 🎉 Project Milestone

You have built:

```text
Full Backend API with:
- Authentication (JWT)
- Authorization
- CRUD operations
- Security rules
- Error handling
- Testing
```

---

## Next Step

👉 Deploy the API (Azure / Render / Railway)
👉 Or build frontend (React)

---
