# Task 06 - Authentication Service And JWT

## Goal

Implement registration, login, password hashing, and JWT token generation.

By the end of this task, users should be able to register, log in, and receive a token that can be used for authenticated task APIs.

## Why This Task Matters

Authentication answers two questions:

- Who is the user?
- Can this request prove that identity?

The backend should never store plain-text passwords. It should store password hashes and issue signed JWTs after successful login.

## Steps

### 1. Add JWT settings to configuration

In `appsettings.json`, add a JWT section:

```json
{
  "Jwt": {
    "Key": "replace-this-with-a-long-secret-key",
    "Issuer": "SmartTaskManager",
    "Audience": "SmartTaskManagerUsers",
    "ExpiresInMinutes": 60
  }
}
```

Why:

JWT generation needs a signing key and token metadata. Configuration keeps those values out of code.

For real production deployments, the secret key should come from environment variables or a secret manager.

### 2. Create IAuthService

In Solution Explorer, right-click `Services/Interfaces`, choose `Add -> New Item`, select `Interface`, and name it `IAuthService.cs`.

Suggested methods:

```csharp
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
```

Why:

The controller should not know how passwords are hashed or tokens are generated. It should call a service.

### 3. Create AuthService

In Solution Explorer, right-click `Services`, choose `Add -> Class`, and name it `AuthService.cs`.

Inject:

- `AppDbContext`
- configuration or a JWT helper

Responsibilities:

- Check whether email already exists.
- Hash password on registration.
- Save new user.
- Verify password on login.
- Generate JWT token.
- Return `AuthResponseDto`.

Why:

Authentication is business logic. It belongs in a service, not directly in a controller.

### 4. Hash passwords

Use a proper password hashing approach.

For learning projects, ASP.NET Core's `PasswordHasher<TUser>` is a good option:

```csharp
var passwordHasher = new PasswordHasher<User>();
var hash = passwordHasher.HashPassword(user, dto.Password);
```

To verify:

```csharp
var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
```

Why:

Password hashing protects users if the database is exposed. Never store plain passwords.

### 5. Create JwtHelper

In Solution Explorer, right-click `Helpers`, choose `Add -> Class`, and name it `JwtHelper.cs`.

It should generate a signed token with claims such as:

- User id
- Email

Important claims:

```csharp
new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
new Claim(ClaimTypes.Email, user.Email)
```

Why:

The task APIs need the authenticated user's id so they can filter tasks by owner.

### 6. Configure JWT authentication

Install the JWT bearer package through Visual Studio:

1. Right-click the `SmartTaskManager.Api` project.
2. Choose `Manage NuGet Packages`.
3. Open the `Browse` tab.
4. Search for `Microsoft.AspNetCore.Authentication.JwtBearer`.
5. Select the package from Microsoft.
6. Click `Install`.

In `Program.cs`, add authentication and authorization:

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
```

Also add middleware in the correct order:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Why:

Authentication reads and validates the token. Authorization enforces access rules.

### 7. Register AuthService

In `Program.cs`:

```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
```

## Completion Criteria

You are done when:

- JWT settings exist.
- `IAuthService` exists.
- `AuthService` exists.
- Passwords are hashed.
- Login verifies hashed passwords.
- JWT token includes user id and email claims.
- JWT authentication is configured.
- Auth service is registered in dependency injection.

## Learning Notes

Authentication should be boring and careful.

The main security rules are:

- Never store plain passwords.
- Never trust user input.
- Never let the client decide its own `UserId`.
- Always derive the current user from the validated token.
