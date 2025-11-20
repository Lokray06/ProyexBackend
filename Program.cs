using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models; // <--- Add this namespace
using Npgsql;                // <--- Add this namespace

var builder = WebApplication.CreateBuilder(args);

// Read config
var Configuration = builder.Configuration;

// --- REPLACE THE OLD DB CONTEXT SETUP WITH THIS BLOCK ---

// 1. Create the data source builder
var connectionString = Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// 2. Map your C# Enums to the Postgres Enum names (case-sensitive from init.sql)
dataSourceBuilder.MapEnum<UserRole>("userRoles");
dataSourceBuilder.MapEnum<GroupType>("groupType");
dataSourceBuilder.MapEnum<ProyexBackend.Models.TaskStatus>("taskStatus"); // Full name to avoid conflict
dataSourceBuilder.MapEnum<ProjectStatus>("projectStatus");
dataSourceBuilder.MapEnum<Priority>("priority");
dataSourceBuilder.MapEnum<Visibility>("visibility");

// 3. Build the data source
var dataSource = dataSourceBuilder.Build();

// 4. Add DbContext using the data source
builder.Services.AddDbContext<ProyexDBContext>(options =>
    options.UseNpgsql(dataSource)
);

// --------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
