# Task 04 - DTOs And Validation

## Goal

Create request and response DTOs for authentication and task operations, then add basic validation rules.

By the end of this task, the API should have clear input and output shapes without exposing database entities directly.

## Why This Task Matters

DTO means Data Transfer Object.

DTOs protect your application in two ways:

- They prevent clients from sending fields they should not control.
- They prevent the API from exposing internal database details.

For example, a user should never send `PasswordHash`. They should send `Password`, and the backend should create the hash internally.

## Steps

### 1. Create authentication DTOs

In Solution Explorer, right-click `DTOs/Auth`, choose `Add -> Class`, and create `RegisterDto.cs`:

```csharp
public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

Then create `LoginDto.cs` in the same folder:

```csharp
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

Then create `AuthResponseDto.cs` in the same folder:

```csharp
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

Why:

- Register and login inputs are similar but separate so they can evolve independently.
- The response contains the JWT token and basic user identity.

### 2. Create task DTOs

In Solution Explorer, right-click `DTOs/Task`, choose `Add -> Class`, and create `TaskCreateDto.cs`:

```csharp
public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}
```

Then create `TaskUpdateDto.cs` in the same folder:

```csharp
public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Status { get; set; }
}
```

Then create `TaskResponseDto.cs` in the same folder:

```csharp
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Status { get; set; }
}
```

Why:

- Create DTO does not contain `Id`, `UserId`, or `CreatedAt`.
- Update DTO allows changing task fields but still does not expose ownership.
- Response DTO returns only the fields the client needs.

### 3. Create a paged response DTO

For `GET /api/tasks`, create a reusable response shape:

```csharp
public class PagedResponseDto<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
}
```

You can place this in `DTOs/PagedResponseDto.cs`.

In Visual Studio, right-click the `DTOs` folder and use `Add -> Class`.

Why:

The design says the list endpoint should return `data` and `totalCount`. A generic DTO lets you reuse this pattern later.

### 4. Add validation attributes

Use data annotations for simple validation:

```csharp
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
```

Apply similar rules:

- `LoginDto.Email`: required, email address.
- `LoginDto.Password`: required.
- `TaskCreateDto.Title`: required, reasonable max length.
- `TaskUpdateDto.Title`: required, reasonable max length.
- `TaskUpdateDto.Status`: restrict to known status values in service logic.

Why:

Validation catches bad input before it reaches business logic or database code.

### 5. Think about nullability

Use nullable types only when a value is truly optional:

- `Description` can be nullable.
- `DueDate` can be nullable.
- `Title` should not be nullable.
- `Email` should not be nullable.
- `Password` should not be nullable.

Why:

Good nullability decisions make bugs easier to see during development.

## Completion Criteria

You are done when:

- Auth DTOs exist.
- Task DTOs exist.
- A paged response DTO exists.
- Required fields have validation rules.
- Database entities are not used directly as API request or response bodies.

## Learning Notes

DTOs are part of your API contract.

Changing an entity is an internal database decision. Changing a DTO is a client-facing API decision. Separating them gives you control over both.
