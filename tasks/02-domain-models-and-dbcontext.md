# Task 02 - Domain Models And DbContext

## Goal

Create the core database entities and configure Entity Framework Core through `AppDbContext`.

By the end of this task, the application should know about users and task items as database-backed entities.

---

## Why This Task Matters

Models represent the data your application stores. The DbContext represents the bridge between C# objects and SQL Server tables.

The design has two main tables:

* `Users`
* `TaskItems`

The relationship is:

```text
One User -> Many TaskItems
```

Each user should only see and manage their own tasks.

---

## Steps

---

### 0. Install Required Packages

Install the following packages using Visual Studio (Manage NuGet Packages):

* `Microsoft.EntityFrameworkCore.SqlServer`
* `Microsoft.EntityFrameworkCore.Tools` (optional but recommended)

Why:

These packages allow your application to communicate with SQL Server and enable migrations later.

---

### 1. Add the User model

📁 Create file: `Models/User.cs`

```csharp
namespace SmartTaskManager.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
```

#### Why:

* `Id` is the primary key.
* `Email` uniquely identifies a user.
* `PasswordHash` ensures passwords are not stored in plain text.
* `CreatedAt` helps track when the user was created.
* `Tasks` defines the one-to-many relationship.

---

### 2. Add the TaskItem model

📁 Create file: `Models/TaskItem.cs`

```csharp
namespace SmartTaskManager.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatus Status { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
```

#### Why:

* `Title` is required because every task needs a name.
* `Description` is optional.
* `DueDate` is optional since not all tasks have deadlines.
* `Status` uses an enum for readability.
* `UserId` is the foreign key linking to the user.
* `User` is the navigation property back to the owner.

---

### 3. Add Task Status Enum

📁 Create file: `Models/TaskItemStatus.cs`

```csharp
namespace SmartTaskManager.Api.Models
{
    public enum TaskItemStatus
    {
        Pending = 0,
        Completed = 1
    }
}
```

#### Why:

Enums improve readability and avoid using magic numbers like 0 and 1.

---

### 4. Add AppDbContext

📁 Create file: `Data/AppDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
```

---

### What AppDbContext Does

`AppDbContext` is the central class that connects your application to the database using Entity Framework Core.

It is responsible for:

* Mapping C# models to database tables

  * `DbSet<User>` → Users table
  * `DbSet<TaskItem>` → TaskItems table

* Tracking changes to entities

* Translating LINQ queries into SQL

* Managing relationships between entities

* Applying constraints like unique indexes and foreign keys

In simple terms:

```text
AppDbContext = Bridge between your code and the database
```

---

### 5. Register DbContext in Program.cs

📁 File: `Program.cs`

#### Add at the TOP:

```csharp
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Api.Data;
```

---

#### Add BELOW this line:

```csharp
builder.Services.AddOpenApi();
```

👉 Add:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
```

---

### 6. Add the connection string

📁 File: `appsettings.json`

Replace with:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SmartTaskManagerDb;Trusted_Connection=True;"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*"
}
```

---

## Validation (Light Check)

After completing this task:

* Build the project
* Ensure no compilation errors

Do NOT expect:

* Database creation
* Tables
* API responses

These will happen in the next task.

---

## Completion Criteria

You are done when:

* `User` model exists
* `TaskItem` model exists
* Enum is used for status
* `AppDbContext` exists
* DbSets are configured
* Email is configured as unique
* Relationship is configured
* DbContext is registered
* Connection string is added

---

## Learning Notes

* Models are NOT DTOs
* Models define database structure
* DTOs define API input/output

Keeping them separate avoids tight coupling between your database and API.

---

## Note

At this stage, no database or tables are created yet.

The models and DbContext only define how the database should look.

The actual database will be created in the next task using EF Core migrations.