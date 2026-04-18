# Task 03 - Entity Framework Core Migrations

## Goal

Create and apply the first database migration for the Smart Task Manager backend.

By the end of this task, SQL Server should contain the `Users` and `TaskItems` tables.

---

## Why This Task Matters

Migrations are version control for your database schema.

Instead of manually creating tables, EF Core allows you to:

* Define schema using C# models
* Generate database changes automatically
* Track schema changes over time

This is important because your application will evolve, and your database must evolve with it.

---

## Prerequisites

Before starting, ensure:

* Task 02 is completed
* Project builds successfully
* SQL Server / LocalDB is available
* Connection string is configured

---

## Steps

---

### 1. Install EF Core Design Package

Use Visual Studio:

1. Right-click `SmartTaskManager.Api`
2. Click `Manage NuGet Packages`
3. Search: `Microsoft.EntityFrameworkCore.Design`
4. Install

#### Why:

This package enables migration generation from your models and DbContext.

---

### 2. Open Package Manager Console

EF Core migrations are executed using commands, and in Visual Studio the correct place to run them is the **Package Manager Console**.

#### Open it from:

```text
Tools → NuGet Package Manager → Package Manager Console
```

This will open a terminal panel at the bottom of Visual Studio that looks like:

```text
PM>
```

#### Configure the Console (Important)

At the top of the console, you will see a dropdown called:

```text
Default project
```

Set it to:

```text
SmartTaskManager.Api
```

#### Set Startup Project

In Solution Explorer:

1. Right-click on `SmartTaskManager.Api`
2. Click:

```text
Set as Startup Project
```

#### Why These Settings Are Required

EF Core needs to know:

* **Where your DbContext and models are** → Default Project
* **Where your app configuration exists (Program.cs, appsettings.json)** → Startup Project

In this project, both are the same:

```text
SmartTaskManager.Api
```

#### What Happens If This Is Wrong?

You may see errors like:

```text
No DbContext was found
```

or migrations may fail to generate.



Once this is set correctly, you are ready to run migration commands in the next step.


---

### 3. Create Initial Migration

Run:

```powershell id="8ql7wz"
Add-Migration InitialCreate
```

---

### What This Does

EF Core:

* Reads your models (`User`, `TaskItem`)
* Reads `AppDbContext`
* Generates SQL instructions

---

### Expected Result

A new folder appears:

```text id="i9cmcd"
Migrations/
```

With files like:

```text id="m9jvxr"
2026xxxx_InitialCreate.cs
AppDbContextModelSnapshot.cs
```

---

### 4. Review the Migration

Open the generated migration file.

Check:

* `Users` table
* `TaskItems` table
* Unique index on `Email`
* Foreign key (`UserId` → `Users.Id`)

#### Why:

Never blindly trust generated code.
This is what will be executed on your database.

---

### 5. Apply Migration

Run:

```powershell id="0mk2c2"
Update-Database
```

---

### What This Does

* Creates the database (if not exists)
* Creates tables
* Applies constraints
* Tracks migration history

---

### Expected Output

You should see logs indicating:

* Database created
* Tables created
* Migration applied successfully

---

## 6. Verify Database (SSMS)

Open SQL Server Management Studio (SSMS).

Try connecting to:

```text id="r3qmbp"
(localdb)\MSSQLLocalDB
```

---

### Check:

1. Expand **Databases**
2. Find:

```text id="pl1c3z"
SmartTaskManagerDb
```

3. Expand **Tables**

You should see:

```text id="r2zzf3"
Users
TaskItems
__EFMigrationsHistory
```

---

### What Each Table Means

* `Users` → user data
* `TaskItems` → tasks
* `__EFMigrationsHistory` → tracks applied migrations

---

## Completion Criteria

You are done when:

* Migration file is created
* Database update succeeds
* Database exists in SQL Server
* Tables are present
* Migration is reviewed

---

## Common Problems

---

### ❌ Connection String Issues

Check:

* Server name
* SQL Server running
* Correct connection string

---

### ❌ DbContext Not Found

Ensure:

* `AppDbContext` is public
* Registered in `Program.cs`

---

### ❌ Startup Project Issue

Fix:

1. Right-click project → Set as Startup Project
2. In Package Manager Console → set Default Project
3. Run again

---

## Learning Notes

* Migrations = version control for database
* Models define structure
* Migrations apply structure

A good habit:

👉 Always read migrations before applying them

---

## Commit Your Work

```bash id="8o3o1v"
git add .
git commit -m "feat(task-03): add initial migration and create database schema"
git push
```