# Task 11 - Future Enhancements

## Goal

Plan future improvements after the core backend is working.

This task is not required for the first version. It helps you understand how production systems grow over time.

## Why This Task Matters

The high-level design mentions future enhancements:

- Role-based authorization.
- Redis caching.
- Background jobs.
- Docker support.
- CI/CD pipelines.

These are valuable, but they should come after the core application works. Adding them too early can make learning harder because there are too many moving parts.

## Enhancement 1 - Role-Based Authorization

### What It Adds

Different permissions for different user types.

Example roles:

- User
- Admin

### Why Add It Later

The first version only needs users to manage their own tasks. Roles become useful when admins need to view system-wide data or manage users.

### Suggested Steps

1. Add a `Role` property to `User`.
2. Include role claims in JWT.
3. Use `[Authorize(Roles = "Admin")]` on admin endpoints.
4. Add tests for role-restricted access.

## Enhancement 2 - Redis Caching

### What It Adds

Redis can store frequently requested data in memory.

### Why Add It Later

Caching makes systems faster but also more complex. You need to handle stale data and cache invalidation.

### Suggested Steps

1. Identify slow or frequently used reads.
2. Cache list responses or summary data.
3. Invalidate cache when tasks are created, updated, or deleted.
4. Measure performance before and after.

## Enhancement 3 - Background Jobs

### What It Adds

Background jobs run work outside the normal HTTP request flow.

Possible examples:

- Send reminder emails before due dates.
- Clean up old completed tasks.
- Generate daily task summaries.

### Why Add It Later

The first version has no long-running work. Background jobs become useful when you need scheduled or delayed processing.

### Suggested Steps

1. Add Hangfire or a similar job library.
2. Configure persistent job storage.
3. Create a reminder job.
4. Add retry and failure handling.

## Enhancement 4 - Docker Support

### What It Adds

Docker makes the API easier to run consistently across machines.

### Why Add It Later

You should understand how the app runs normally before containerizing it.

### Suggested Steps

1. Add a `Dockerfile` for the API.
2. Add `docker-compose.yml` for API and SQL Server.
3. Move configuration into environment variables.
4. Test from a clean machine or clean container environment.

## Enhancement 5 - CI/CD Pipeline

### What It Adds

CI/CD automatically builds, tests, and deploys the backend.

### Why Add It Later

Automation is most useful once there are tests and a stable build process.

### Suggested Steps

1. Add a build workflow.
2. Run tests on every pull request.
3. Publish build artifacts.
4. Add deployment only after build and tests are reliable.

## Recommended Priority

After the core backend is complete, improve in this order:

1. Automated tests.
2. Docker support.
3. CI build pipeline.
4. Background jobs.
5. Role-based authorization.
6. Redis caching.

Why this order:

- Tests protect behavior.
- Docker improves repeatability.
- CI catches problems early.
- Background jobs add useful product behavior.
- Roles matter when there is an admin use case.
- Caching should be driven by measured performance needs.

## Completion Criteria

You are done when:

- Future enhancements are understood.
- Each enhancement has a reason to exist.
- You know which improvements should wait until the core backend works.

## Learning Notes

Production readiness is not one feature.

It is the result of many small improvements: security, tests, observability, repeatable deployment, clear architecture, and careful data ownership.

