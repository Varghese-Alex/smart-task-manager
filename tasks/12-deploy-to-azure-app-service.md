# Task 12 - Deploy To Azure App Service

## Goal

Deploy the Smart Task Manager backend to Azure using a GUI-first workflow.

By the end of this task, your ASP.NET Core Web API should run in Azure App Service and connect to an Azure SQL Database using cloud configuration instead of local settings.

## AZ-900 Concepts Reinforced

This task reinforces these cloud fundamentals:

- **Resource Group**: A logical container for related Azure resources.
- **App Service**: A managed platform for hosting web apps and APIs.
- **App Service Plan**: The compute pricing and scale unit behind an App Service.
- **Azure SQL Database**: A managed relational database service.
- **Connection Strings**: Configuration values that tell the app where the database is.
- **Environment Variables / App Settings**: Cloud-hosted configuration values injected into the app at runtime.
- **Cloud Hosting**: Running your application on managed infrastructure instead of your local computer.

## Important Deployment Idea

Your local `appsettings.json` is for local development.

In Azure, production values should be configured in the App Service resource:

```text
Azure App Service -> Settings -> Environment variables
```

For ASP.NET Core apps, App Service configuration values override values in local config files at runtime. This lets the same code run locally with local settings and in Azure with cloud settings.

## Resources You Will Create

Use names like these, replacing `<yourname>` with something unique:

```text
Resource group: rg-smart-task-manager-dev
App Service: app-smart-task-manager-<yourname>
App Service plan: asp-smart-task-manager-dev
SQL server: sql-smart-task-manager-<yourname>
SQL database: sqldb-smart-task-manager-dev
```

Why these names:

- Prefixes make the resource type easy to identify.
- `dev` reminds you this is a learning environment.
- App Service and SQL Server names must be globally unique in Azure.

## Step 1 - Prepare The Project

### What To Click

In Visual Studio:

1. Open `SmartTaskManager.Api`.
2. Right-click the `SmartTaskManager.Api` project.
3. Select `Build`.
4. Confirm the build succeeds.

### What To Configure

Make sure the app works locally before deploying.

Check:

- Register works.
- Login works.
- JWT-protected task APIs work.
- Database migrations have been applied locally.

### Why

Azure deployment should not be the first time you test the app. Cloud hosting adds networking and configuration concerns, so the app should already work on your machine.

## Step 2 - Create A Resource Group

### What To Click

In the Azure Portal:

1. Search for `Resource groups`.
2. Click `Create`.
3. Select your subscription.
4. Enter resource group name:

```text
rg-smart-task-manager-dev
```

5. Choose a nearby region.
6. Click `Review + create`.
7. Click `Create`.

### What To Configure

Use the same region later for App Service and Azure SQL when possible.

### Why

A resource group lets you manage related resources together. For learning projects, it also makes cleanup easier because you can delete the resource group to remove everything inside it.

## Step 3 - Create Azure SQL Database

### What To Click

In the Azure Portal:

1. Search for `SQL databases`.
2. Click `Create`.
3. Select your subscription.
4. Select resource group:

```text
rg-smart-task-manager-dev
```

5. Enter database name:

```text
sqldb-smart-task-manager-dev
```

6. Under `Server`, click `Create new`.
7. Enter a unique server name:

```text
sql-smart-task-manager-<yourname>
```

8. Choose the same region as the resource group.
9. Choose `Use SQL authentication`.
10. Enter an admin login and password.
11. Save the password somewhere safe for this learning project.
12. For workload environment, choose `Development` if available.
13. Choose a low-cost compute option suitable for learning.
14. Click `Review + create`.
15. Click `Create`.

### What To Configure

After the SQL database is created:

1. Open the SQL server resource, not just the database.
2. Go to `Security -> Networking`.
3. Enable access for Azure services if the option is shown.
4. Add your current client IP if you want to connect from Visual Studio or SQL Server Management Studio.
5. Save changes.

### Why

Your API needs a cloud database. Azure SQL Database gives you SQL Server behavior without managing a full virtual machine.

The firewall step matters because Azure SQL blocks connections that are not explicitly allowed.

## Step 4 - Get The Azure SQL Connection String

### What To Click

In the Azure Portal:

1. Open your SQL database:

```text
sqldb-smart-task-manager-dev
```

2. Go to `Settings -> Connection strings`.
3. Select the `ADO.NET` connection string.
4. Copy it.
5. Replace the placeholder password with your SQL admin password.

### What To Configure

The final connection string should look conceptually like this:

```text
Server=tcp:sql-smart-task-manager-<yourname>.database.windows.net,1433;
Initial Catalog=sqldb-smart-task-manager-dev;
Persist Security Info=False;
User ID=<admin-user>;
Password=<admin-password>;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

Keep the connection string out of source code.

### Why

The deployed API needs to know where the Azure SQL database is. This is cloud configuration, so it belongs in Azure App Service settings, not in `appsettings.json`.

## Step 5 - Create The App Service From Visual Studio

### What To Click

In Visual Studio:

1. Right-click the `SmartTaskManager.Api` project.
2. Click `Publish`.
3. Choose `Azure`.
4. Click `Next`.
5. Choose `Azure App Service (Windows)`.
6. Click `Next`.
7. Sign in to your Azure account if prompted.
8. Click the green plus button or `Create new`.

Fill in:

```text
Name: app-smart-task-manager-<yourname>
Subscription: your Azure subscription
Resource group: rg-smart-task-manager-dev
Hosting plan: create new
App Service plan name: asp-smart-task-manager-dev
Region: same region as your database
Pricing tier: a low-cost dev/test tier
```

9. Click `Create`.
10. Select the newly created App Service.
11. Click `Finish`.

### What To Configure

After the publish profile is created:

1. Review the target App Service name.
2. Confirm the runtime is compatible with your project target framework.
3. Do not publish yet if you have not configured cloud settings.

### Why

App Service is the managed hosting environment. Visual Studio can create the hosting resource and remember the publish profile so future deployments are easier.

## Step 6 - Configure App Service Connection String

### What To Click

In the Azure Portal:

1. Search for `App Services`.
2. Open your app:

```text
app-smart-task-manager-<yourname>
```

3. Go to `Settings -> Environment variables`.
4. Open the `Connection strings` tab.
5. Click `Add`.

Configure:

```text
Name: DefaultConnection
Value: <your Azure SQL connection string>
Type: SQLAzure
Deployment slot setting: unchecked for now
```

6. Click `Apply`.
7. Click `Apply` again on the Environment variables page if Azure asks.

### What To Configure

The name must be exactly:

```text
DefaultConnection
```

Why:

Your code reads:

```csharp
builder.Configuration.GetConnectionString("DefaultConnection")
```

If the Azure setting name does not match, the deployed app will not find the database connection.

## Step 7 - Configure JWT Environment Values

### What To Click

Still in:

```text
App Service -> Settings -> Environment variables
```

Open the `App settings` tab and add these values:

```text
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpiresInMinutes
```

Suggested values:

```text
Jwt__Key: use a long random secret, not the sample value
Jwt__Issuer: SmartTaskManager
Jwt__Audience: SmartTaskManagerUsers
Jwt__ExpiresInMinutes: 60
```

Click `Apply`.

### What To Configure

Use double underscores:

```text
Jwt__Key
```

Do not use:

```text
Jwt:Key
```

### Why

In ASP.NET Core environment variables, double underscore maps to nested configuration. So `Jwt__Key` maps to:

```json
{
  "Jwt": {
    "Key": "..."
  }
}
```

This keeps secrets out of source code and teaches the AZ-900 idea that cloud apps usually receive configuration from the hosting platform.

## Step 8 - Apply Migrations To Azure SQL

### Recommended Learning Approach

Use Visual Studio's Package Manager Console.

This is one of the few command-based steps because EF Core migrations are normally applied through EF tooling.

### What To Click

In Visual Studio:

1. Open `Tools`.
2. Go to `NuGet Package Manager`.
3. Click `Package Manager Console`.
4. Set `Default project` to `SmartTaskManager.Api`.

### What To Configure

Temporarily make your local project use the Azure SQL connection string, then run the migration.

Safer learning approach:

1. Open `appsettings.Development.json`.
2. Temporarily add the Azure SQL `DefaultConnection`.
3. In Package Manager Console, run:

```powershell
Update-Database
```

4. Remove the Azure SQL connection string from `appsettings.Development.json` after the migration succeeds.

### Why

Your Azure SQL database starts empty. The migration creates the `Users`, `TaskItems`, and EF migration history tables.

### Better Future Approach

Later, use one of these safer production approaches:

- EF Core migration bundle.
- CI/CD pipeline migration step.
- Manual DBA-reviewed SQL script.

For learning, Package Manager Console is acceptable as long as you understand what database you are pointing at.

## Step 9 - Publish The API

### What To Click

In Visual Studio:

1. Right-click `SmartTaskManager.Api`.
2. Click `Publish`.
3. Select the Azure publish profile you created.
4. Click `Publish`.

### What To Configure

Wait for Visual Studio to finish deployment.

After publish, Visual Studio usually opens the deployed site URL:

```text
https://app-smart-task-manager-<yourname>.azurewebsites.net
```

You will use this deployed base URL in Postman.

### Why

Publishing copies your compiled API to App Service. App Service then runs it using the cloud settings you configured.

## Step 10 - Test The Cloud API

### What To Click

Open Postman and create or update an environment for the Azure deployment.

```text
baseUrl = https://app-smart-task-manager-<yourname>.azurewebsites.net
token   = empty for now
```

Test in this order:

1. `POST {{baseUrl}}/api/auth/register`
2. `POST {{baseUrl}}/api/auth/login`
3. Copy the JWT token from the login response.
4. Save the token into the Postman environment variable named `token`.
5. For task requests, use authorization type `Bearer Token`.
6. Set the bearer token value to `{{token}}`.
7. `POST {{baseUrl}}/api/tasks`
8. `GET {{baseUrl}}/api/tasks`
9. `PUT {{baseUrl}}/api/tasks/{id}`
10. `DELETE {{baseUrl}}/api/tasks/{id}`

### What To Configure

For authenticated task APIs, Postman must send this header:

```text
Authorization: Bearer <token>
```

In Postman, the easiest way is:

```text
Authorization tab -> Type: Bearer Token -> Token: {{token}}
```

### Why

This confirms that:

- App Service is hosting your API.
- App settings are being read.
- Connection string points to Azure SQL.
- Authentication works in the cloud.
- The API can create and read cloud database records.

## Step 11 - View Logs

### What To Click

In the Azure Portal:

1. Open your App Service.
2. Go to `Monitoring -> Log stream`.
3. Refresh your API in the browser or call an endpoint.
4. Watch for startup or runtime errors.

Optional:

1. Go to `Monitoring -> App Service logs`.
2. Turn on application logging for temporary debugging.
3. Save changes.

### Why

Logs help you debug cloud-only issues like missing environment variables, failed database connections, or runtime mismatch.

Turn verbose logging back off when you are done because logs can contain sensitive information and can increase noise.

## Common Mistakes To Avoid

### Mistake 1 - Putting Azure secrets in `appsettings.json`

Do not commit production connection strings or JWT secrets.

Use:

```text
App Service -> Settings -> Environment variables
```

### Mistake 2 - Wrong connection string name

The connection string must be named:

```text
DefaultConnection
```

If you name it `Default`, `Database`, or `SmartTaskManagerDb`, the code will not find it.

### Mistake 3 - Wrong connection string type

For Azure SQL, choose:

```text
SQLAzure
```

### Mistake 4 - Forgetting Azure SQL firewall rules

If the app cannot connect to SQL, check:

- Azure services are allowed to reach the SQL server.
- Your local IP is allowed if you are applying migrations from Visual Studio.
- The SQL username and password are correct.

### Mistake 5 - Forgetting to apply migrations

Publishing code does not automatically create database tables.

If the app starts but register/login fails with database errors, check whether the Azure SQL database has these tables:

```text
Users
TaskItems
__EFMigrationsHistory
```

### Mistake 6 - Using a weak JWT key

Do not use the sample JWT key in Azure.

Use a long random value.

### Mistake 7 - Publishing before configuring App Service settings

If settings are missing, the app may start but fail during authentication or database access.

Configure connection strings and JWT app settings before final testing.

### Mistake 8 - Choosing expensive resources for a learning project

Use low-cost dev/test tiers.

After testing, stop or delete resources you do not need.

The easiest cleanup path is deleting:

```text
rg-smart-task-manager-dev
```

Deleting the resource group deletes the resources inside it.

## Completion Criteria

You are done when:

- Resource group exists.
- Azure SQL Database exists.
- App Service exists.
- App Service has `DefaultConnection` configured as a connection string.
- App Service has JWT values configured as app settings.
- Azure SQL migrations have been applied.
- API is published to App Service.
- Postman environment points to the deployed App Service base URL.
- Register, login, and task APIs work against Azure SQL.
- You understand which values are local config and which values are cloud config.

## Learning Notes

This task is where local backend development becomes cloud hosting.

The mental model is:

```text
Your code -> App Service
Your data -> Azure SQL Database
Your secrets -> App Service Environment variables
Your resources -> Resource Group
Your cost/scale -> App Service Plan
```

That is a very AZ-900-friendly way to understand Azure application hosting.

## References

- Microsoft Learn: Configure App Service app settings and connection strings  
  https://learn.microsoft.com/en-us/azure/app-service/configure-common
- Microsoft Learn: Deploy ASP.NET Core and Azure SQL Database app to Azure App Service  
  https://learn.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app
- Microsoft Learn: Publish an ASP.NET Core app to Azure with Visual Studio  
  https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vs
