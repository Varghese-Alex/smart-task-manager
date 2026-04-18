# Task 06 - Authentication Service And JWT

## Goal

Implement registration, login, password hashing, and JWT token generation.

By the end of this task, users should be able to:

* Register
* Log in
* Receive a JWT token

---

## Why This Task Matters

Authentication answers:

* Who is the user?
* Can we trust this request?

---

## Steps

---

### 1. Add JWT Settings

📁 `appsettings.json`

```json
"Jwt": {
  "Key": "THIS_IS_A_SUPER_SECRET_KEY_12345",
  "Issuer": "SmartTaskManager",
  "Audience": "SmartTaskManagerUsers",
  "ExpiresInMinutes": 60
}
```

---

### 2. Create IAuthService

📁 `Services/Interfaces/IAuthService.cs`

```csharp
using SmartTaskManager.Api.DTOs.Auth;

namespace SmartTaskManager.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
```

---

### 3. Create JwtHelper

📁 `Helpers/JwtHelper.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartTaskManager.Api.Models;

namespace SmartTaskManager.Api.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

---

### 4. Create AuthService

📁 `Services/AuthService.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Api.Data;
using SmartTaskManager.Api.DTOs.Auth;
using SmartTaskManager.Api.Helpers;
using SmartTaskManager.Api.Models;
using SmartTaskManager.Api.Services.Interfaces;

namespace SmartTaskManager.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new Exception("Email already exists");

            var user = new User
            {
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token
            };
        }
    }
}
```

---

### 5. Install JWT Package

Install:

```text
Microsoft.AspNetCore.Authentication.JwtBearer
```

---

### 6. Configure JWT in Program.cs

#### 🔹 Add using:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
```

---

#### 🔹 Add BEFORE builder.Build():

```csharp
builder.Services.AddScoped<JwtHelper>();

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

---

### 🔥 VERY IMPORTANT (Middleware Order)

AFTER `app = builder.Build();`

```csharp
app.UseAuthentication();   // MUST come first
app.UseAuthorization();
```

---

### 7. Register AuthService

```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
```

---

## Completion Criteria

* Registration works
* Login works
* Passwords are hashed
* JWT token is generated
* Token contains user id + email
* Authentication middleware is configured

---

## Commit Your Work

```bash
git add .
git commit -m "feat(task-06): implement authentication service with JWT"
git push
```

---

## Next Step

👉 Task 07 - Task Service Layer
