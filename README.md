# Student Management API

This project is a **.NET 8 Web API** that integrates with **MongoDB** to perform CRUD (Create, Read, Update, Delete) operations on student records.  
The purpose of this project is to practice building a REST API end-to-end using ASP.NET Core with MongoDB as the backend.

---

## Project Structure

```StudentManagementSystem
│   StudentManagementSystem.sln
│
├───StudentManagementSystem
│   │   appsettings.Development.json
│   │   appsettings.json
│   │   Program.cs
│   │   StudentManagementSystem.API.csproj
│   │
│   ├───Controllers
│   │       StudentControllers.cs
│   │
│   ├───Logs
│   │       log.txt
│   │
│   └───Properties
│           launchSettings.json
│
├───StudentManagementSystem.Core
│   │   StudentManagementSystem.Core.csproj
│   │
│   ├───Logger
│   │       SimpleFileLogger.cs
│   │       SimpleFileLoggerProvider.cs
│   │
│   ├───Models
│           IStudentStoreDatabaseSettings.cs
│           Student.cs
│           StudentStoreDatabaseSettings.cs
│   
├───StudentManagementSystem.Infrastructure
│   │   StudentManagementSystem.Infrastructure.csproj
│   │
│   ├───Data
│   │       IMongoDbContext.cs
│   │       MongoDbContext.cs
│   │
│   └───Repositories
│           IStudentRepository.cs
│           StudentRepository.cs
│
├───StudentManagementSystem.Services
│   │   StudentManagementSystem.Services.csproj
│   │
│   └───Services
│           IStudentService.cs
│           StudentService.cs
│
├───StudentManagementSystem.Test
│   │   StudentControllerTests.cs
│   │   StudentManagementSystem.Test.csproj
│   │   StudentModelsTests.cs
│   │   StudentRepositoryTest.cs
│   │   StudentServiceTests.cs
│
└───.gitignore
```

