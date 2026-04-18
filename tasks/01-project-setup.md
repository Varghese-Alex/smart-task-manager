# Task 01 - Project Setup

## Goal

Create the ASP.NET Core Web API project structure for the Smart Task Manager backend.

By the end of this task, you should have a runnable Web API project with the folder layout needed for controllers, services, repositories, data access, models, DTOs, and helpers.

---

## Why This Task Matters

Project setup is not just ceremony. A clean structure makes the rest of the backend easier to understand, test, and change.

The design follows a simplified Clean Architecture approach:

```text
Controller -> Service -> Repository -> DbContext -> Database
```

Creating folders for these responsibilities early helps you avoid putting all logic inside controllers.

---

## Steps

### 1. Create the Web API project using Visual Studio

Open Visual Studio and choose:

```text
Create a new project -> ASP.NET Core Web API
```

Use these settings:

* Project name: `SmartTaskManager.Api`
* Location: the repository folder
* Solution name: `SmartTaskManager`
* Framework: .NET 8 or later (including .NET 10)
* Authentication type: `None`
* Configure for HTTPS: enabled
* Enable OpenAPI support: enabled
* Use controllers: enabled

---

### 2. Confirm the solution structure

```text
Solution 'SmartTaskManager'
  SmartTaskManager.Api
```

(Optional but recommended):

```text
SmartTaskManager
 └── src
     └── SmartTaskManager.Api
```

---

### 3. Add the planned folders

Create the following folders:

```text
Controllers

Services
 └── Interfaces

Repositories
 └── Interfaces

Data

Models

DTOs
 ├── Auth
 └── Tasks

Helpers

Middleware
```

---

## Folder Responsibilities

* `Controllers` → Handles HTTP requests
* `Services` → Business logic
* `Repositories` → Data access logic
* `Data` → EF Core configuration (DbContext later)
* `Models` → Database entities
* `DTOs` → Request/response contracts
* `Helpers` → Utility classes (e.g., JWT later)
* `Middleware` → Cross-cutting concerns

---

### 4. Remove sample files

Delete:

```text
WeatherForecast.cs
Controllers/WeatherForecastController.cs
```

---

### 5. Install Swagger (Important for .NET 8+ / 10)

Newer templates may not include Swagger UI by default.

Install via Package Manager Console:

```powershell
Install-Package Swashbuckle.AspNetCore
```

---

### 6. Configure Program.cs (Swagger + OpenAPI)

Update your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger (UI for humans)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OpenAPI (machine-readable JSON)
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();

    // OpenAPI JSON
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

### 7. Run the project

* Press `F5` or `Ctrl + F5`
* Open:

Swagger UI:

```text
https://localhost:xxxx/swagger
```

OpenAPI JSON:

```text
https://localhost:xxxx/openapi/v1.json
```

---

## Expected Behavior

* Swagger UI opens successfully
* It may show:

  ```text
  No operations defined in spec!
  ```

  ✔ This is expected (no controllers yet)

---

## Completion Criteria

You are done when:

* Solution file exists
* Web API project runs
* Swagger UI opens
* OpenAPI JSON endpoint works
* Folder structure is created
* Sample files are removed
* No business logic is added yet

---

## Learning Notes

* Swagger = for developers (UI testing)
* OpenAPI = for tools (clients, integrations)

Most important habit:

> Do not ask "Where can I put this code?"
> Ask "Which layer should own this responsibility?"