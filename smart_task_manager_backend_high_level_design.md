# Smart Task Manager Backend - High Level Design

## 📌 Overview

This document describes the high-level design, architecture, folder structure, database schema, and API design for the **Smart Task Manager** backend application.

The goal is to build a clean, scalable, and production-ready backend using ASP.NET Core Web API.

---

## 🧱 Tech Stack

- Backend: ASP.NET Core Web API
- Language: C#
- Database: SQL Server
- ORM: Entity Framework Core
- Authentication: JWT

---

## 🧭 Architecture

The application follows a **Clean Architecture (simplified)** approach:

```
Controller → Service → Repository → DbContext → Database
```

### Responsibilities

- **Controller**: Handles HTTP requests and responses
- **Service**: Business logic
- **Repository**: Data access abstraction
- **DbContext**: Database interaction

---

## 📁 Folder Structure

```
/SmartTaskManager.Api
│
├── Controllers
│   ├── AuthController.cs
│   ├── TasksController.cs
│
├── Services
│   ├── Interfaces
│   │   ├── IAuthService.cs
│   │   ├── ITaskService.cs
│   │
│   ├── AuthService.cs
│   ├── TaskService.cs
│
├── Repositories
│   ├── Interfaces
│   │   ├── ITaskRepository.cs
│   │
│   ├── TaskRepository.cs
│
├── Data
│   ├── AppDbContext.cs
│
├── Models
│   ├── User.cs
│   ├── TaskItem.cs
│
├── DTOs
│   ├── Auth
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   ├── AuthResponseDto.cs
│   │
│   ├── Task
│       ├── TaskCreateDto.cs
│       ├── TaskUpdateDto.cs
│       ├── TaskResponseDto.cs
│
├── Helpers
│   ├── JwtHelper.cs
│
├── Middleware (optional)
│
├── appsettings.json
├── Program.cs
```

---

## 🗄️ Database Design

### Users Table

| Column        | Type        | Description          |
|--------------|------------|----------------------|
| Id           | int (PK)   | Primary key          |
| Email        | nvarchar   | Unique email         |
| PasswordHash | nvarchar   | Hashed password      |
| CreatedAt    | datetime   | Created timestamp    |

---

### TaskItems Table

| Column     | Type      | Description                |
|-----------|----------|----------------------------|
| Id        | int (PK) | Primary key                |
| Title     | nvarchar | Task title                 |
| Description | nvarchar | Task details            |
| DueDate   | datetime | Optional due date          |
| Status    | int      | 0 = Pending, 1 = Completed |
| UserId    | int (FK) | Linked user               |
| CreatedAt | datetime | Created timestamp         |

---

### Relationships

- One User → Many Tasks
- Foreign Key: TaskItems.UserId → Users.Id

---

## 🔐 Authentication Flow

1. User registers
2. Password is hashed and stored
3. User logs in
4. JWT token is generated
5. Token is sent with each request
6. Backend validates token and extracts UserId

---

## 📡 API Design

### Auth APIs

#### POST /api/auth/register

**Request**
```json
{
  "email": "test@mail.com",
  "password": "123456"
}
```

#### POST /api/auth/login

**Response**
```json
{
  "token": "JWT_TOKEN",
  "email": "test@mail.com"
}
```

---

### Task APIs (Authenticated)

#### GET /api/tasks

**Query Parameters**
- status (optional)
- search (optional)
- page (default: 1)
- pageSize (default: 10)

**Response**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Finish project",
      "description": "Complete backend",
      "dueDate": "2026-04-20",
      "status": 0
    }
  ],
  "totalCount": 25
}
```

---

#### POST /api/tasks

```json
{
  "title": "New Task",
  "description": "Details",
  "dueDate": "2026-04-20"
}
```

---

#### PUT /api/tasks/{id}

```json
{
  "title": "Updated Task",
  "description": "Updated",
  "status": 1
}
```

---

#### DELETE /api/tasks/{id}

---

## 🧠 Key Design Decisions

- Use DTOs to avoid exposing database entities
- Use async/await for scalability
- Filter data by authenticated user
- Implement pagination for performance

---

## 🔧 Future Enhancements

- Role-based authorization
- Redis caching
- Background jobs (Hangfire)
- Docker support
- CI/CD pipelines

---

## 🚀 Outcome

This backend structure demonstrates:

- Clean architecture principles
- Secure authentication
- Scalable API design
- Production-ready practices

---

## 📎 Notes

- Keep controllers thin
- Move all logic to services
- Validate inputs properly
- Use proper HTTP status codes

---

**End of Document**

