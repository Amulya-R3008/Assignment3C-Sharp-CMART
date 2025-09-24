using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace StudentManagementSystem.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        // Get all students
        public List<Student> Get() => _repository.GetAllStudents();

        // Get single student by Id
        public Student Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Student Id is required.");

            var student = _repository.GetStudentById(id);
            if (student == null)
                throw new KeyNotFoundException($"Student with Id={id} not found.");

            return student;
        }

        // Create a new student
        public Student Create(Student student)
        {
            ValidateStudent(student);

            _repository.AddStudent(student);
            return student;
        }

        // Update existing student
        public void Update(string id, Student student)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Student Id is required.");

            var existing = _repository.GetStudentById(id);
            if (existing == null)
                throw new KeyNotFoundException($"Student with Id={id} not found.");

            ValidateStudent(student);

            _repository.UpdateStudent(id, student);
        }

        // Delete student by Id
        public void Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Student Id is required.");

            var existing = _repository.GetStudentById(id);
            if (existing == null)
                throw new KeyNotFoundException($"Student with Id={id} not found.");

            _repository.DeleteStudent(id);
        }

        // Private helper for validating business rules
        private void ValidateStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");

            if (string.IsNullOrWhiteSpace(student.Name))
                throw new ArgumentException("Student name is required.");

            if (student.Age < 0 || student.Age > 120)
                throw new ArgumentException("Age must be between 0 and 120.");

            if (!string.IsNullOrWhiteSpace(student.Gender) &&
                student.Gender.ToLower() != "male" &&
                student.Gender.ToLower() != "female" &&
                student.Gender.ToLower() != "other")
                throw new ArgumentException("Gender must be Male, Female, or Other.");

            if (student.Courses != null && student.Courses.Length > 5)
                throw new ArgumentException("A student cannot enroll in more than 5 courses.");
        }
    }
}
