# Task 02 - Domain Models And DbContext

## Goal

Create the core database entities and configure Entity Framework Core through `AppDbContext`.

By the end of this task, the application should know about users and task items as database-backed entities.

## Why This Task Matters

Models represent the data your application stores. The DbContext represents the bridge between C# objects and SQL Server tables.

The design has two main tables:

- `Users`
- `TaskItems`

The relationship is:

```text
One User -> Many TaskItems
```

This relationship is important because each user should only see and manage their own tasks.

## Steps

### 1. Add the User model

In Solution Explorer, right-click the `Models` folder and choose:

```text
Add -> Class
```

Name the file `User.cs`.

Suggested properties:

```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
```

Why:

- `Id` is the primary key.
- `Email` identifies the user and should be unique.
- `PasswordHash` stores a hashed password, never the plain password.
- `CreatedAt` helps with auditing.
- `Tasks` defines the one-to-many relationship.

### 2. Add the TaskItem model

In Solution Explorer, right-click the `Models` folder, choose `Add -> Class`, and name the file `TaskItem.cs`.

Suggested properties:

```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
```

Why:

- `Title` is required because a task needs a name.
- `Description` can be optional.
- `DueDate` can be optional because not every task has a deadline.
- `Status` starts simple as an integer matching the design.
- `UserId` is the foreign key.
- `User` is the navigation property back to the owner.

### 3. Consider using an enum for task status

Instead of using magic numbers everywhere, add a class file through Solution Explorer and replace the class with an enum:

```csharp
public enum TaskItemStatus
{
    Pending = 0,
    Completed = 1
}
```

Why:

Enums make the code easier to read. `TaskItemStatus.Completed` communicates more than `1`.

Avoid naming it `TaskStatus`, because that can be confused with `System.Threading.Tasks.TaskStatus`.

### 4. Add AppDbContext

In Solution Explorer, right-click the `Data` folder, choose `Add -> Class`, and name the file `AppDbContext.cs`.

Suggested structure:

```csharp
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
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(user => user.Tasks)
            .WithOne(task => task.User)
            .HasForeignKey(task => task.UserId);
    }
}
```

Why:

- `DbSet<User>` maps to a users table.
- `DbSet<TaskItem>` maps to a task items table.
- The unique email index prevents duplicate accounts.
- The relationship configuration makes ownership explicit.

### 5. Register DbContext in Program.cs

Install the EF Core SQL Server package through Visual Studio:

1. Right-click the `SmartTaskManager.Api` project.
2. Choose `Manage NuGet Packages`.
3. Open the `Browse` tab.
4. Search for `Microsoft.EntityFrameworkCore.SqlServer`.
5. Select the package from Microsoft.
6. Click `Install`.

Why this package:

It gives Entity Framework Core the SQL Server provider, which is what lets `AppDbContext` talk to SQL Server.

Then register the DbContext in `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 6. Add the connection string

In Solution Explorer, open `appsettings.json` and add:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartTaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Adjust the connection string based on your SQL Server setup.

## Completion Criteria

You are done when:

- `User` model exists.
- `TaskItem` model exists.
- `AppDbContext` exists.
- `Users` and `TaskItems` DbSets are configured.
- User email is configured as unique.
- User-to-task relationship is configured.
- DbContext is registered in dependency injection.

## Learning Notes

Models are not DTOs.

Models describe how data is stored. DTOs describe how data enters and leaves the API. Keeping those separate prevents your database shape from leaking directly into your public API.
