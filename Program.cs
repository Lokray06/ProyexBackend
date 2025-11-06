using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyexBackend.Data;
using ProyexBackend.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add Configuration ---
// Reads from appsettings.json and Environment Variables (from docker-compose)
var Configuration = builder.Configuration;

// --- 2. Add Database Context (EF Core) ---
var connectionString = Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProyexDBContext>(options =>
    options.UseNpgsql(connectionString,
    npgsqlOptions =>
    {
        npgsqlOptions.MapEnum<UserRole>("userRoles");
    }
));

// --- 3. Add Authentication & JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
    };
});

builder.Services.AddAuthorization(); // Adds authorization services

// --- 4. Add Other Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Adds Swagger for API testing

// --- 5. Build the App ---
var app = builder.Build();

// --- 6. Configure HTTP Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // We're not using HTTPS inside Docker for this demo

app.UseAuthentication(); // <-- IMPORTANT: Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers(); // Maps your [ApiController] classes

app.Run();