using MongoDB.Driver;
using StudentManagementSystem.Core.Models;

namespace StudentManagementSystem.Infrastructure.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<Student> Students { get; }
    }
}
