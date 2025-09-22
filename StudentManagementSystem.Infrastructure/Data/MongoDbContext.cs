using MongoDB.Driver;
using StudentManagementSystem.Core.Models;

namespace StudentManagementSystem.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IStudentStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(settings.DatabaseName);
        }

        // Collections
        public IMongoCollection<Student> Students => _database.GetCollection<Student>("studentcourses");
    }
}
