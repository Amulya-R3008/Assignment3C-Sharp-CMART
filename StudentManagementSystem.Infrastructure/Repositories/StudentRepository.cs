using System.Collections.Generic;
using MongoDB.Driver;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Data;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly MongoDbContext _context;

        public StudentRepository(MongoDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAllStudents() => _context.Students.Find(s => true).ToList();
        public Student? GetStudentById(string id) => _context.Students.Find(s => s.Id == id).FirstOrDefault();
        public void AddStudent(Student student) => _context.Students.InsertOne(student);
        public void UpdateStudent(string id, Student student) => _context.Students.ReplaceOne(s => s.Id == id, student);
        public void DeleteStudent(string id) => _context.Students.DeleteOne(s => s.Id == id);
    }
}
