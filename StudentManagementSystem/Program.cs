using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories;
using StudentManagementSystem.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- MongoDB Configuration ----------------------
builder.Services.Configure<StudentStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(StudentStoreDatabaseSettings)));

builder.Services.AddSingleton<IStudentStoreDatabaseSettings>(
    sp => sp.GetRequiredService<IOptions<StudentStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration["StudentStoreDatabaseSettings:ConnectionString"]));

builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

// ---------------------- JWT Authentication ----------------------
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetBytes(jwtKey).Length < 32)
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long.");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

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
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

// ---------------------- Controllers ----------------------
builder.Services.AddControllers();

// ---------------------- Swagger Configuration ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentManagementAPI", Version = "v1" });

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// ---------------------- Middleware ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentManagementAPI v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
