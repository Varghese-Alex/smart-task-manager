# Task 03 - Entity Framework Core Migrations

## Goal

Create and apply the first database migration for the Smart Task Manager backend.

By the end of this task, SQL Server should contain the `Users` and `TaskItems` tables.

## Why This Task Matters

Migrations are version control for your database schema.

Instead of manually creating tables in SQL Server, EF Core migrations let the application describe the schema in code and generate repeatable database changes.

This matters because the backend will evolve. Migrations help you track how the database changes over time.

## Steps

### 1. Install EF Core design package

Use Visual Studio's NuGet Package Manager:

1. Right-click the `SmartTaskManager.Api` project.
2. Choose `Manage NuGet Packages`.
3. Open the `Browse` tab.
4. Search for `Microsoft.EntityFrameworkCore.Design`.
5. Select the package from Microsoft.
6. Click `Install`.

Why:

The design package helps EF Core create migrations from your `DbContext` and models.

### 2. Open Package Manager Console

Entity Framework migrations do not have a normal point-and-click screen built into Visual Studio. The standard Visual Studio GUI workflow is to use Package Manager Console.

Open it from:

```text
Tools -> NuGet Package Manager -> Package Manager Console
```

In the console toolbar:

- Set `Default project` to `SmartTaskManager.Api`.
- Make sure `SmartTaskManager.Api` is also the startup project in Solution Explorer.

Why this is acceptable even in a GUI-first workflow:

The console is part of Visual Studio, and EF Core migrations are normally created through short commands. This is one of the places where a command is the correct tool.

### 3. Create the initial migration

In Package Manager Console, run:

```powershell
Add-Migration InitialCreate
```

This should create a `Migrations` folder.

Why:

- The migration captures table creation.
- It records primary keys, foreign keys, indexes, and column definitions.

### 4. Review the generated migration

Open the generated migration file.

Check for:

- `Users` table.
- `TaskItems` table.
- Unique index on `Users.Email`.
- Foreign key from `TaskItems.UserId` to `Users.Id`.

Do not blindly trust generated code. Reading migrations helps you understand what will happen to the database.

### 5. Apply the migration

In Package Manager Console, run:

```powershell
Update-Database
```

This applies the migration to SQL Server.

### 6. Verify the database

Use SQL Server Management Studio, Azure Data Studio, or another database tool.

Confirm that:

- The database exists.
- `Users` table exists.
- `TaskItems` table exists.
- `__EFMigrationsHistory` table exists.

The `__EFMigrationsHistory` table is how EF Core remembers which migrations have already been applied.

## Completion Criteria

You are done when:

- EF Core tooling works.
- Initial migration exists.
- Database update succeeds.
- SQL Server contains the expected tables.
- The migration has been reviewed, not just generated.

## Common Problems

### Connection string error

If SQL Server cannot be reached, check:

- Server name.
- Authentication mode.
- Database permissions.
- `TrustServerCertificate=True` for local development.

### DbContext not found

Make sure `AppDbContext` is public and registered correctly.

### Startup project error

If EF cannot find the startup project:

1. Right-click `SmartTaskManager.Api`.
2. Choose `Set as Startup Project`.
3. In Package Manager Console, set `Default project` to `SmartTaskManager.Api`.
4. Run `Add-Migration InitialCreate` again.

## Learning Notes

Migrations are a contract between your code and your database.

A good habit is to read each migration before applying it. This catches accidental schema changes early.
