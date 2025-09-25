using MongoDB.Driver;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Data;
using System.Collections.Generic;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IMongoDbContext _context;

        public StudentRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAllStudents() =>
            _context.Students.Find(s => true).ToList();

        public Student? GetStudentById(string id)
        {
            var filter = Builders<Student>.Filter.Eq(s => s.Id, id);
            return _context.Students.Find(filter).FirstOrDefault();
        }

        public void AddStudent(Student student) =>
            _context.Students.InsertOne(student);
        //public void AddStudent(Student student)
        //{
        //    student.Id = null; // Let MongoDB auto-generate ObjectId
        //    _context.Students.InsertOne(student);
        //}


        public void UpdateStudent(string id, Student student)
        {
            student.Id = id; // ensure the Id matches
            var filter = Builders<Student>.Filter.Eq(s => s.Id, id);
            _context.Students.ReplaceOne(filter, student);
        }

        public void DeleteStudent(string id)
        {
            var filter = Builders<Student>.Filter.Eq(s => s.Id, id);
            _context.Students.DeleteOne(filter);
        }
    }
}
