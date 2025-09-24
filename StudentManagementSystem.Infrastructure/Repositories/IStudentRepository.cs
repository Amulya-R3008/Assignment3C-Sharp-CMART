using System.Collections.Generic;
using StudentManagementSystem.Core.Models;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetAllStudents();
        Student? GetStudentById(string id);
        void AddStudent(Student student);
        void UpdateStudent(string id, Student student);
        void DeleteStudent(string id);
    }
}
