# Task 14 - Build React Frontend For Smart Task Manager

## Goal

Build a React frontend for the Smart Task Manager backend.

By the end of this task, you should have a browser-based app where a user can:

- Register
- Log in
- Store the JWT for API calls
- View tasks
- Create tasks
- Update tasks
- Delete tasks
- Use the Azure-hosted backend instead of only testing through Postman

## Why This Is The Next Step

So far, Postman has proven that the backend works.

The frontend proves that real users can use it.

This task moves the project from:

```text
Backend API tested by developer
```

to:

```text
Full-stack app used in the browser
```

## Concepts Reinforced

This task teaches:

- React components
- Client-side routing
- Forms
- API calls
- JWT authentication from the browser
- Protected pages
- Browser storage
- Environment variables
- CORS
- Azure Static Web Apps
- End-to-end full-stack testing

## Important Mental Model

Postman and browsers are different.

Postman can call your API directly.

A browser-based React app must follow browser security rules. If the React app and API are on different domains, the backend must allow that frontend origin through CORS.

This is why CORS becomes important now.

## Recommended Folder Structure

Create the frontend as a sibling to the backend project:

```text
Smart_Task_Manager
|
|-- SmartTaskManager.Api
|-- smart-task-manager-ui
|-- tasks
```

Why:

- Backend and frontend stay in the same repository.
- Each app keeps its own dependencies.
- GitHub Actions can later build/deploy each part separately.

## Step 1 - Install Node.js

### What To Click

1. Go to the official Node.js website.
2. Download the LTS version.
3. Run the installer.
4. Keep the default options unless you know you need something different.
5. Finish installation.

### What To Configure

Restart Visual Studio or Visual Studio Code after installing Node.js.

### Why

React tooling runs on Node.js. Even though your backend is C#, the frontend build system is JavaScript-based.

## Step 2 - Create The React App

### What To Click

Use Visual Studio Code for the React frontend if possible. It has smoother support for JavaScript, TypeScript, npm, and React.

1. Open Visual Studio Code.
2. Click `File -> Open Folder`.
3. Open the repository root:

```text
d:\Smart_Task_Manager
```

4. Open the integrated terminal:

```text
Terminal -> New Terminal
```

### What To Configure

Create a React app using Vite:

```powershell
npm create vite@latest smart-task-manager-ui -- --template react
```

Then install dependencies:

```powershell
cd smart-task-manager-ui
npm install
```

Start the frontend locally:

```powershell
npm run dev
```

Open the local URL shown by Vite. It is usually:

```text
http://localhost:5173
```

### Why A Command Here

This is one of the places where the command line is the correct tool.

React project creation and package installation are normally done through npm commands. The rest of the task stays GUI-first where possible.

## Step 3 - Clean The Starter App

### What To Click

In Visual Studio Code Explorer, open:

```text
smart-task-manager-ui/src
```

Edit:

```text
App.jsx
App.css
index.css
```

### What To Configure

Remove the default Vite/React starter content.

Replace it with a simple shell:

```text
Smart Task Manager
Login
Register
Tasks
```

### Why

Starter content is useful only to prove the React app runs. Once it runs, remove it so your project becomes about your domain.

## Step 4 - Add Frontend Environment Variables

### What To Click

In the `smart-task-manager-ui` folder:

1. Right-click in Explorer.
2. Choose `New File`.
3. Create:

```text
.env.development
.env.production
```

### What To Configure

For local development:

```text
VITE_API_BASE_URL=https://localhost:xxxx
```

Replace `xxxx` with your backend HTTPS port.

For production:

```text
VITE_API_BASE_URL=https://app-smart-task-manager-<yourname>.azurewebsites.net
```

### Why

The frontend needs to know where the backend API lives.

Local development points to your local backend.

Production points to your Azure App Service backend.

Vite only exposes environment variables to frontend code when they start with:

```text
VITE_
```

Do not put secrets in frontend environment variables. Frontend values are bundled into browser code and can be seen by users.

## Step 5 - Create A Small API Client

### What To Click

In `src`, create a folder:

```text
api
```

Create:

```text
src/api/client.js
```

### What To Configure

This file should:

1. Read the API base URL from `import.meta.env.VITE_API_BASE_URL`.
2. Add `Content-Type: application/json`.
3. Add `Authorization: Bearer <token>` when a token exists.
4. Parse JSON responses.
5. Throw readable errors when the API returns an error.

Suggested functions:

```text
apiGet(path)
apiPost(path, body)
apiPut(path, body)
apiDelete(path)
```

### Why

Centralizing API calls prevents duplicated fetch logic across every component.

If token handling changes later, you update one place.

## Step 6 - Create Auth Pages

### What To Click

Create folders:

```text
src/pages
src/components
```

Create:

```text
src/pages/LoginPage.jsx
src/pages/RegisterPage.jsx
```

### What To Configure

Register page:

- Email input
- Password input
- Submit button
- Calls `POST /api/auth/register`
- Saves returned token
- Redirects to tasks page

Login page:

- Email input
- Password input
- Submit button
- Calls `POST /api/auth/login`
- Saves returned token
- Redirects to tasks page

### Why

Authentication is the entry point of the app.

The frontend should use the same flow you already validated in Postman:

```text
Register/Login -> receive token -> use token for task APIs
```

## Step 7 - Decide Where To Store The JWT

### Recommended Learning Choice

For this learning project, store the token in:

```text
localStorage
```

### What To Configure

Create helper functions:

```text
saveToken(token)
getToken()
clearToken()
```

### Why

`localStorage` is simple and survives page refreshes.

Important security note:

`localStorage` is vulnerable if your app has a cross-site scripting bug. In production systems, many teams prefer HttpOnly secure cookies or a more advanced token strategy.

For this learning app, `localStorage` is acceptable as long as you understand the tradeoff.

## Step 8 - Add Client-Side Routing

### What To Click

Install React Router:

```powershell
npm install react-router-dom
```

Create:

```text
src/router.jsx
src/components/ProtectedRoute.jsx
```

### What To Configure

Routes:

```text
/login
/register
/tasks
/
```

Expected behavior:

- `/login` shows login.
- `/register` shows register.
- `/tasks` requires a token.
- `/` redirects to `/tasks` if logged in, otherwise `/login`.

### Why

Protected routing prevents unauthenticated users from seeing task screens.

This is frontend convenience, not backend security. The backend still enforces real security with JWT authorization.

## Step 9 - Build The Tasks Page

### What To Click

Create:

```text
src/pages/TasksPage.jsx
src/components/TaskForm.jsx
src/components/TaskList.jsx
src/components/TaskItem.jsx
```

### What To Configure

Tasks page should call:

```text
GET /api/tasks?page=1&pageSize=10
```

Show:

- Loading state
- Empty state
- Error state
- Task title
- Description
- Due date
- Status
- Edit button
- Delete button

### Why

This is the main user experience. The page should handle success, failure, and empty data.

## Step 10 - Add Create Task

### What To Click

Open:

```text
src/components/TaskForm.jsx
```

### What To Configure

Fields:

- Title
- Description
- Due date

Submit should call:

```text
POST /api/tasks
```

Body:

```json
{
  "title": "Finish project",
  "description": "Complete backend",
  "dueDate": "2026-04-20"
}
```

After success:

- Clear the form.
- Refresh the task list.

### Why

Create is the first real authenticated browser-to-backend write operation.

If this fails in the browser but works in Postman, suspect CORS or missing Authorization header.

## Step 11 - Add Update Task

### What To Click

Open:

```text
src/components/TaskItem.jsx
```

### What To Configure

Support:

- Mark as completed
- Edit title/description/due date

Call:

```text
PUT /api/tasks/{id}
```

Body:

```json
{
  "title": "Updated task",
  "description": "Updated details",
  "dueDate": "2026-04-20",
  "status": 1
}
```

### Why

Update confirms that the frontend correctly sends route parameters, JSON body, and bearer token together.

## Step 12 - Add Delete Task

### What To Click

Open:

```text
src/components/TaskItem.jsx
```

### What To Configure

Delete button should:

1. Ask for confirmation.
2. Call `DELETE /api/tasks/{id}`.
3. Remove the task from the UI or refresh the list.

### Why

Delete completes the CRUD workflow.

The browser app should now cover the same API behavior you already tested in Postman.

## Step 13 - Configure CORS In The Backend

### What To Click

Open the backend project in Visual Studio.

Open:

```text
SmartTaskManager.Api/SmartTaskManager.Api/Program.cs
```

### What To Configure

Add a CORS policy to allow the React frontend origin.

For local development, the frontend origin is usually:

```text
http://localhost:5173
```

For production, the frontend origin will be your Azure Static Web Apps URL, which looks like:

```text
https://<your-static-web-app-name>.azurestaticapps.net
```

Use a named CORS policy such as:

```text
AllowFrontend
```

The policy should:

- Allow only your known frontend origins.
- Allow any header.
- Allow any method.

Middleware order matters. Place CORS after HTTPS redirection and before authentication/authorization:

```text
UseHttpsRedirection
UseCors
UseAuthentication
UseAuthorization
MapControllers
```

### Why

The browser blocks cross-origin API calls unless the backend explicitly allows the frontend origin.

Do not use `AllowAnyOrigin` for the deployed app. It is too broad for a real API.

## Step 14 - Add CORS Settings In Azure

### What To Click

In Azure Portal:

1. Open your backend App Service.
2. Go to `Settings -> Environment variables`.
3. Open `App settings`.
4. Add app settings for allowed frontend origins.

Example:

```text
Cors__AllowedOrigins__0 = http://localhost:5173
Cors__AllowedOrigins__1 = https://<your-static-web-app-name>.azurestaticapps.net
```

5. Click `Apply`.
6. Restart the App Service if Azure does not restart it automatically.

### What To Configure

Use double underscores for nested configuration:

```text
Cors__AllowedOrigins__0
```

### Why

This keeps frontend origin configuration outside the backend code.

Local development and production can allow different origins without changing source code every time.

## Step 15 - Test Locally In The Browser

### What To Click

Run backend from Visual Studio:

```text
F5
```

Run frontend from Visual Studio Code terminal:

```powershell
npm run dev
```

Open:

```text
http://localhost:5173
```

### What To Validate

Test:

1. Register
2. Login
3. Create task
4. List tasks
5. Update task
6. Delete task
7. Logout
8. Try opening `/tasks` without token

### Why

This proves the browser app works before deploying it.

If something fails locally, deployment will not fix it.

## Step 16 - Deploy React To Azure Static Web Apps

### What To Click

In Azure Portal:

1. Click `Create a resource`.
2. Search for `Static Web App`.
3. Click `Create`.
4. Select your subscription.
5. Select the existing resource group:

```text
rg-smart-task-manager-dev
```

6. Enter name:

```text
swa-smart-task-manager-<yourname>
```

7. Select a region.
8. Choose plan type `Free` for learning if available.
9. Source: `GitHub`.
10. Sign in to GitHub if prompted.
11. Select your organization.
12. Select your repository.
13. Select branch:

```text
main
```

### Build Details

Configure:

```text
Build preset: React
App location: smart-task-manager-ui
Api location: leave blank
Output location: dist
```

Click:

```text
Review + create -> Create
```

### Why

Azure Static Web Apps is a good fit for React because React builds into static files: HTML, CSS, and JavaScript.

Azure creates a GitHub Actions workflow that builds and deploys the frontend when you push changes.

## Step 17 - Configure Frontend Production API URL

### What To Click

In GitHub:

1. Open your repository.
2. Go to `Settings`.
3. Open `Secrets and variables`.
4. Click `Actions`.
5. Add a repository variable or secret depending on your workflow.

### What To Configure

For Vite production builds, the workflow needs:

```text
VITE_API_BASE_URL=https://app-smart-task-manager-<yourname>.azurewebsites.net
```

If Azure Static Web Apps generated the workflow, open the workflow file and confirm this variable is available during build.

### Why

Vite environment variables are baked into the frontend at build time.

Changing Azure Static Web Apps settings after the app is already built will not automatically change JavaScript that was already bundled.

If the API URL changes, rebuild and redeploy the frontend.

## Step 18 - Add The Deployed Frontend URL To Backend CORS

### What To Click

After Azure Static Web Apps deployment finishes:

1. Open the Static Web App in Azure Portal.
2. Copy its URL.
3. Open the backend App Service.
4. Go to `Settings -> Environment variables`.
5. Add or update:

```text
Cors__AllowedOrigins__1 = https://<your-static-web-app-name>.azurestaticapps.net
```

6. Apply changes.
7. Restart the backend App Service.

### Why

The final production browser flow is:

```text
React frontend on Azure Static Web Apps -> API on Azure App Service -> Azure SQL Database
```

The backend must allow the frontend origin.

## Step 19 - Test The Deployed Full-Stack App

### What To Click

Open the Azure Static Web Apps URL:

```text
https://<your-static-web-app-name>.azurestaticapps.net
```

Use the app in the browser:

1. Register a new user.
2. Log in.
3. Create a task.
4. Refresh the page.
5. Confirm the task still appears.
6. Mark the task complete.
7. Delete the task.
8. Log out.

### What To Verify

Open browser developer tools:

```text
F12 -> Network
```

Confirm:

- API calls go to your Azure App Service backend.
- Authenticated calls include `Authorization: Bearer ...`.
- No CORS errors appear in the Console.
- Responses match what you saw in Postman.

### Why

This proves the complete cloud system works from a real browser.

## Common Mistakes To Avoid

### Mistake 1 - Forgetting CORS

Symptom:

```text
Works in Postman, fails in browser
```

Cause:

The browser enforces CORS. Postman does not.

Fix:

Allow the React frontend origin in the backend CORS policy.

### Mistake 2 - Using The Wrong API Base URL

Local frontend should use:

```text
https://localhost:xxxx
```

Production frontend should use:

```text
https://app-smart-task-manager-<yourname>.azurewebsites.net
```

### Mistake 3 - Missing `VITE_` Prefix

This will not work in Vite frontend code:

```text
API_BASE_URL=...
```

This will:

```text
VITE_API_BASE_URL=...
```

### Mistake 4 - Putting Secrets In React Env Variables

Never put database strings, JWT signing keys, or passwords in React environment variables.

React runs in the user's browser. Anything bundled into React can be inspected.

### Mistake 5 - Forgetting Bearer Token Header

Task APIs require:

```text
Authorization: Bearer <token>
```

Without it, the backend should return:

```text
401 Unauthorized
```

### Mistake 6 - Thinking Protected Routes Are Real Security

Frontend protected routes improve user experience.

Backend `[Authorize]` is the real security boundary.

Users can bypass frontend code, but they cannot bypass backend authorization if the API is implemented correctly.

### Mistake 7 - Not Rebuilding After Env Changes

Vite production environment variables are applied during build.

If `VITE_API_BASE_URL` changes, rebuild and redeploy the frontend.

### Mistake 8 - Date Formatting Problems

HTML date inputs produce dates like:

```text
2026-04-20
```

Your API accepts `DateTime?`, so check that the frontend sends a format the backend can parse.

### Mistake 9 - Leaving Logout Incomplete

Logout should:

- Clear token from storage.
- Redirect to login.
- Prevent task page access until logging in again.

## Completion Criteria

You are done when:

- React app exists in `smart-task-manager-ui`.
- Frontend has login and register pages.
- Frontend stores JWT after login.
- Frontend sends bearer token to task APIs.
- Frontend can create, list, update, and delete tasks.
- Protected routes work.
- Backend CORS allows the local frontend origin.
- Backend CORS allows the deployed frontend origin.
- React app is deployed to Azure Static Web Apps.
- Deployed frontend calls the Azure App Service backend.
- Browser testing succeeds end to end.

## Learning Notes

This task connects everything:

```text
React -> Azure App Service API -> Azure SQL Database
```

Postman helped you verify the backend.

React helps you verify the product.

Once this is complete, your project is no longer just an API. It is a deployed full-stack application.

## References

- Vite documentation: Environment variables and modes  
  https://vite.dev/guide/env-and-mode
- Microsoft Learn: Enable Cross-Origin Requests in ASP.NET Core  
  https://learn.microsoft.com/en-us/aspnet/core/security/cors
- Microsoft Learn: Deploy React app with Azure Static Web Apps  
  https://learn.microsoft.com/en-us/azure/static-web-apps/deploy-react

