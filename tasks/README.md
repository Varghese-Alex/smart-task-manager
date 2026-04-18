# Smart Task Manager Backend - Task Breakdown

This folder breaks the high-level backend design into practical implementation tasks.

The original design document explains the target architecture. These task files explain how to build it step by step and why each step matters.

## Recommended Order

1. [Project setup](01-project-setup.md)
2. [Domain models and database context](02-domain-models-and-dbcontext.md)
3. [Entity Framework Core migrations](03-ef-core-migrations.md)
4. [DTOs and validation](04-dtos-and-validation.md)
5. [Repository layer](05-repository-layer.md)
6. [Authentication service and JWT](06-authentication-service-and-jwt.md)
7. [Task service layer](07-task-service-layer.md)
8. [Controllers and API endpoints](08-controllers-and-api-endpoints.md)
9. [Security, ownership, and error handling](09-security-ownership-and-error-handling.md)
10. [Testing and manual verification](10-testing-and-manual-verification.md)
11. [Future enhancements](11-future-enhancements.md)

## How To Use These Files

Work through the files in order. Each task builds on the previous one.

These tasks assume a GUI-first workflow, mainly using Visual Studio, SQL Server Management Studio, Azure Data Studio, and Swagger UI. Use command-line steps only when the tooling itself expects a command, such as Entity Framework Core migrations in Visual Studio's Package Manager Console.

For each task:

- Read the goal first.
- Understand why the task exists.
- Follow the steps in sequence.
- Check the completion criteria before moving on.
- Use the learning notes to connect implementation choices back to backend design principles.

## Overall Backend Shape

The backend follows this simplified Clean Architecture flow:

```text
Controller -> Service -> Repository -> DbContext -> Database
```

Each layer has a job:

- Controllers handle HTTP input and output.
- Services hold business rules.
- Repositories isolate database access.
- DbContext translates C# entity operations into SQL queries.
- DTOs protect the API contract from database implementation details.

The main learning goal is not only to make the backend work, but to understand where each responsibility belongs.
