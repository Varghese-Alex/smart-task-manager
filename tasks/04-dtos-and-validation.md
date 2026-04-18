# Task 04 - DTOs And Validation

## Goal

Create request and response DTOs for authentication and task operations, then add basic validation rules.

By the end of this task, the API should have clear input and output shapes without exposing database entities directly.

---

## Why This Task Matters

DTO means Data Transfer Object.

DTOs protect your application in two ways:

* Prevent clients from sending fields they should not control
* Prevent exposing internal database structure

Example:

A user should send:

```text
Email + Password
```

NOT:

```text
PasswordHash
```

---

## Steps

---

### 1. Create Authentication DTOs

📁 Folder: `DTOs/Auth`

---

#### 📄 RegisterDto.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace SmartTaskManager.Api.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
```

---

#### 📄 LoginDto.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace SmartTaskManager.Api.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
```

---

#### 📄 AuthResponseDto.cs

```csharp
namespace SmartTaskManager.Api.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
```

---

### Why

* Separate DTOs allow flexibility
* Input and output are controlled independently
* Sensitive data is never exposed

---

### 2. Create Task DTOs

📁 Folder: `DTOs/Tasks`

---

#### 📄 TaskCreateDto.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace SmartTaskManager.Api.DTOs.Tasks
{
    public class TaskCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
```

---

#### 📄 TaskUpdateDto.cs

```csharp
using System.ComponentModel.DataAnnotations;
using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.DTOs.Tasks
{
    public class TaskUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatus Status { get; set; }
    }
}
```

---

#### 📄 TaskResponseDto.cs

```csharp
using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.DTOs.Tasks
{
    public class TaskResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatus Status { get; set; }
    }
}
```

---

### Why

* Create DTO excludes system-controlled fields
* Update DTO allows controlled updates
* Response DTO returns only needed data

---

### 3. Create Paged Response DTO

📁 File: `DTOs/PagedResponseDto.cs`

```csharp
namespace SmartTaskManager.Api.DTOs
{
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();

        public int TotalCount { get; set; }
    }
}
```

---

### Why

* Standardizes list responses
* Makes pagination reusable
* Keeps API consistent

---

### 4. Validation Rules

Using Data Annotations:

* Email → `[Required]`, `[EmailAddress]`
* Password → `[Required]`, `[MinLength(6)]`
* Title → `[Required]`, `[MaxLength(200)]`

---

### Why Validation Matters

Validation ensures:

* Bad input is rejected early
* Business logic stays clean
* Database errors are avoided

---

### 5. Nullability Decisions

Use nullable only when necessary:

* `Description` → nullable ✔
* `DueDate` → nullable ✔
* `Title` → NOT nullable ❌
* `Email` → NOT nullable ❌
* `Password` → NOT nullable ❌

---

## Important Concept

### Models vs DTOs

* Models → database structure
* DTOs → API contract

👉 Never expose models directly in APIs

---

## Completion Criteria

You are done when:

* Auth DTOs exist
* Task DTOs exist
* Paged response DTO exists
* Validation attributes are applied
* Models are NOT used directly in API requests/responses

---

## Commit Your Work

```bash
git add .
git commit -m "feat(task-04): add DTOs and validation for auth and tasks"
git push
```

---

## Next Step

👉 Proceed to:

**Task 05 - Repository Layer**
