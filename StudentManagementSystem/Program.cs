using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure StudentStoreDatabaseSettings from appsettings.json
builder.Services.Configure<StudentStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(StudentStoreDatabaseSettings)));

// Register the interface as singleton
builder.Services.AddSingleton<IStudentStoreDatabaseSettings>(
    sp => sp.GetRequiredService<IOptions<StudentStoreDatabaseSettings>>().Value);

// MongoClient singleton
builder.Services.AddSingleton<IMongoClient>(
    s => new MongoClient(builder.Configuration.GetValue<string>("StudentStoreDatabaseSettings:ConnectionString")));

// Register Infrastructure DbContext
builder.Services.AddSingleton<MongoDbContext>();

// Register Repository
// Register Repository (interface → implementation)
builder.Services.AddScoped<IStudentRepository, StudentRepository>();


// Register Service
builder.Services.AddScoped<IStudentService, StudentService>();

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();