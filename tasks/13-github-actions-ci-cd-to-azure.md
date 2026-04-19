# Task 13 - Automate Deployment With GitHub Actions

## Goal

Automate deployment of the Smart Task Manager API to Azure App Service using GitHub Actions.

By the end of this task, pushing code to GitHub should automatically:

```text
Restore packages -> Build the API -> Publish the API -> Deploy to Azure App Service
```

## Why This Is The Next Step

You already deployed the API manually to Azure.

Manual deployment is good for learning the cloud pieces. CI/CD is the next professional step because it makes deployment repeatable.

Instead of remembering every click in Visual Studio, the deployment steps live in the repository as a workflow file.

## Concepts Reinforced

This task reinforces:

- **CI/CD**: Continuous Integration and Continuous Deployment.
- **GitHub Actions**: Automation that runs when code changes.
- **Workflow file**: A YAML file stored under `.github/workflows`.
- **Build artifact**: The compiled output that gets deployed.
- **Publish profile**: Azure App Service deployment credentials.
- **GitHub secrets**: Encrypted values used by workflows.
- **App Service deployment**: Updating the cloud-hosted API.

## Important Mental Model

Your local computer should not be the only machine that can deploy.

The cloud deployment flow becomes:

```text
Developer pushes code -> GitHub Actions builds -> GitHub Actions deploys -> Azure App Service runs new version
```

## Step 1 - Confirm The Manual Azure Deployment Works

### What To Click

Open Postman and use your Azure environment.

Confirm:

```text
baseUrl = https://app-smart-task-manager-<yourname>.azurewebsites.net
```

Run:

1. `POST {{baseUrl}}/api/auth/login`
2. `GET {{baseUrl}}/api/tasks`

### What To Configure

Nothing new yet.

### Why

Do not automate a broken deployment. CI/CD should automate a known-good process.

## Step 2 - Download The Azure App Service Publish Profile

### What To Click

In the Azure Portal:

1. Search for `App Services`.
2. Open your App Service:

```text
app-smart-task-manager-<yourname>
```

3. Go to `Overview`.
4. Click `Download publish profile`.
5. Save the downloaded `.PublishSettings` file somewhere temporary.

### What To Configure

Do not commit this file.

The publish profile contains deployment credentials. Treat it like a password.

### Why

GitHub Actions needs permission to deploy to Azure App Service.

For a learning project, publish profile deployment is the simplest option. Later, you can learn Microsoft Entra ID and OpenID Connect for a more secure production-style setup.

## Step 3 - Add The Publish Profile To GitHub Secrets

### What To Click

On GitHub:

1. Open your repository.
2. Click `Settings`.
3. In the left menu, open `Secrets and variables`.
4. Click `Actions`.
5. Click `New repository secret`.

Create this secret:

```text
Name: AZURE_WEBAPP_PUBLISH_PROFILE
Value: paste the full contents of the downloaded publish profile file
```

6. Click `Add secret`.

### What To Configure

The secret name must be exactly:

```text
AZURE_WEBAPP_PUBLISH_PROFILE
```

### Why

GitHub secrets store sensitive values encrypted. Your workflow can use the secret, but the secret value is not exposed in the repository.

## Step 4 - Create The GitHub Actions Workflow File

### What To Click

In Visual Studio:

1. In Solution Explorer, find the repository root.
2. Open `.github`.
3. Open `workflows`.
4. Right-click `workflows`.
5. Choose `Add -> New Item`.
6. Create a YAML file named:

```text
azure-app-service-deploy.yml
```

If Visual Studio does not offer a YAML item template, create a text file and rename it to `.yml`.

### What To Configure

Paste this workflow:

```yaml
name: Build and deploy API to Azure App Service

on:
  push:
    branches:
      - main
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: app-smart-task-manager-<yourname>
  DOTNET_VERSION: '10.0.x'
  PROJECT_PATH: SmartTaskManager.Api/SmartTaskManager.Api/SmartTaskManager.Api.csproj
  PUBLISH_DIR: ./publish

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore ${{ env.PROJECT_PATH }}

      - name: Build
        run: dotnet build ${{ env.PROJECT_PATH }} --configuration Release --no-restore

      - name: Publish
        run: dotnet publish ${{ env.PROJECT_PATH }} --configuration Release --no-build --output ${{ env.PUBLISH_DIR }}

      - name: Upload build artifact
        uses: actions/upload-artifact@v4
        with:
          name: smart-task-manager-api
          path: ${{ env.PUBLISH_DIR }}

  deploy:
    runs-on: ubuntu-latest
    needs: build
    environment: production

    steps:
      - name: Download build artifact
        uses: actions/download-artifact@v4
        with:
          name: smart-task-manager-api
          path: ${{ env.PUBLISH_DIR }}

      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ${{ env.PUBLISH_DIR }}
```

Replace:

```text
app-smart-task-manager-<yourname>
```

with your real Azure App Service name.

### Why

This workflow has two jobs:

- `build`: compiles and packages the API.
- `deploy`: sends the published output to Azure App Service.

Separating build and deploy teaches the normal CI/CD shape.

## Step 5 - Check The Project Path

### What To Click

In Visual Studio Solution Explorer, confirm your `.csproj` path.

For this project, it should be:

```text
SmartTaskManager.Api/SmartTaskManager.Api/SmartTaskManager.Api.csproj
```

### What To Configure

If your folder structure changes, update this workflow value:

```yaml
PROJECT_PATH: SmartTaskManager.Api/SmartTaskManager.Api/SmartTaskManager.Api.csproj
```

### Why

GitHub Actions runs from the repository root. If the project path is wrong, restore/build will fail.

## Step 6 - Commit And Push The Workflow

### What To Click

In Visual Studio:

1. Open `Git Changes`.
2. Review the new workflow file.
3. Enter a commit message:

```text
ci: deploy api to azure app service
```

4. Click `Commit All`.
5. Click `Push`.

### What To Configure

Make sure you do not commit:

```text
*.PublishSettings
```

If you downloaded the publish profile inside the repository by accident, move it outside the repo or delete it before committing.

### Why

GitHub Actions only sees workflow files after they are pushed to GitHub.

## Step 7 - Watch The Workflow Run

### What To Click

On GitHub:

1. Open your repository.
2. Click the `Actions` tab.
3. Click the workflow named:

```text
Build and deploy API to Azure App Service
```

4. Open the latest run.
5. Watch the `build` job.
6. Watch the `deploy` job.

### What To Configure

No new configuration unless the workflow fails.

### Why

The Actions tab is your CI/CD dashboard. It tells you whether GitHub could build and deploy your code.

## Step 8 - Validate The Deployment In Postman

### What To Click

Open Postman and use your Azure environment:

```text
baseUrl = https://app-smart-task-manager-<yourname>.azurewebsites.net
```

Run:

1. `POST {{baseUrl}}/api/auth/login`
2. Copy or save the token.
3. `GET {{baseUrl}}/api/tasks`
4. `POST {{baseUrl}}/api/tasks`

### What To Configure

For task APIs, set:

```text
Authorization tab -> Type: Bearer Token -> Token: {{token}}
```

### Why

The workflow can succeed even if the deployed app has a runtime configuration issue. Postman confirms the app actually works from the outside.

## Step 9 - Make A Small Safe Change And Re-Test

### What To Click

In Visual Studio:

1. Make a tiny safe change, such as updating a comment or a harmless response message.
2. Commit the change.
3. Push to GitHub.
4. Watch GitHub Actions run again.
5. Test in Postman again.

### What To Configure

Nothing new.

### Why

This proves the full loop:

```text
Code change -> Push -> Build -> Deploy -> Validate
```

That loop is the heart of CI/CD.

## Common Mistakes To Avoid

### Mistake 1 - Committing The Publish Profile

The publish profile is a secret.

Do not commit files ending with:

```text
.PublishSettings
```

### Mistake 2 - Wrong Secret Name

The workflow expects:

```text
AZURE_WEBAPP_PUBLISH_PROFILE
```

If the GitHub secret name is different, deployment fails.

### Mistake 3 - Wrong App Service Name

This value must match the Azure App Service name exactly:

```yaml
AZURE_WEBAPP_NAME: app-smart-task-manager-<yourname>
```

Do not use the resource group name or App Service plan name here.

### Mistake 4 - Wrong Project Path

If restore or build says the project file does not exist, fix:

```yaml
PROJECT_PATH
```

### Mistake 5 - Forgetting Azure App Settings

GitHub Actions deploys code. It does not automatically configure your App Service environment variables.

Confirm these still exist in Azure:

```text
DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpiresInMinutes
```

### Mistake 6 - Expecting Migrations To Run Automatically

This workflow deploys the API only.

It does not run:

```text
Update-Database
```

If you add a new migration later, you need a deliberate database deployment strategy.

For learning, apply migrations manually first. Later, learn migration bundles or a controlled database pipeline.

### Mistake 7 - Using The Wrong Branch

The workflow runs on pushes to:

```yaml
main
```

If your default branch is named `master` or something else, update the workflow.

### Mistake 8 - Not Checking The Actions Logs

If the deployment fails, open the failed step in GitHub Actions.

The logs usually tell you exactly whether the problem is:

- Restore.
- Build.
- Publish.
- Missing secret.
- Invalid publish profile.
- Wrong App Service name.

## Completion Criteria

You are done when:

- GitHub secret `AZURE_WEBAPP_PUBLISH_PROFILE` exists.
- Workflow file exists under `.github/workflows`.
- Workflow runs on push to `main`.
- Build job succeeds.
- Deploy job succeeds.
- Azure App Service receives the new deployment.
- Postman confirms login and task APIs still work after deployment.
- You understand the difference between manual deployment and CI/CD.

## Learning Notes

Manual deployment teaches where the cloud pieces are.

CI/CD teaches how teams deploy repeatedly without relying on one person's machine.

The new mental model is:

```text
GitHub is now part of your deployment system.
```

Azure hosts the API, but GitHub Actions delivers updates to it.

## Production Note

Publish profiles are simple and useful for learning.

For more production-grade security, learn OpenID Connect with Microsoft Entra ID. That approach avoids storing long-lived Azure credentials in GitHub secrets.

## References

- Microsoft Learn: Deploy to App Service using GitHub Actions  
  https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions
- Microsoft Learn: Deploy ASP.NET Core apps to Azure App Service  
  https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/
- GitHub Docs: Using secrets in GitHub Actions  
  https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions

