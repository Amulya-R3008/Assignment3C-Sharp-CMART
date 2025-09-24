using Xunit;
using Moq;
using System.Collections.Generic;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Repositories;
using StudentManagementSystem.Services.Services;
using System;

namespace StudentManagementSystem.Test
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _mockRepo;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _service = new StudentService(_mockRepo.Object);
        }

        #region Positive Scenarios

        [Fact]
        public void GetAllStudents_ShouldReturnListOfStudents()
        {
            var students = new List<Student>
            {
                new Student { Id = "1", Name = "Alice", Age = 20, Gender = "Female" },
                new Student { Id = "2", Name = "Bob", Age = 22, Gender = "Male" }
            };

            _mockRepo.Setup(r => r.GetAllStudents()).Returns(students);

            var result = _service.Get();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
        }

        [Fact]
        public void GetStudentById_ShouldReturnStudent_WhenExists()
        {
            var student = new Student { Id = "1", Name = "Alice", Age = 20, Gender = "Female" };
            _mockRepo.Setup(r => r.GetStudentById("1")).Returns(student);

            var result = _service.Get("1");

            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public void CreateStudent_ShouldCallRepositoryAddStudentOnce()
        {
            var student = new Student { Id = "3", Name = "Charlie", Age = 25, Gender = "Male" };

            var result = _service.Create(student);

            _mockRepo.Verify(r => r.AddStudent(student), Times.Once);
            Assert.Equal(student, result);
        }

        [Fact]
        public void UpdateStudent_ShouldCallRepositoryUpdateStudentOnce()
        {
            var student = new Student { Id = "1", Name = "Updated", Age = 21, Gender = "Male" };

            _mockRepo.Setup(r => r.GetStudentById("1")).Returns(student);
            _service.Update("1", student);

            _mockRepo.Verify(r => r.UpdateStudent("1", student), Times.Once);
        }

        [Fact]
        public void DeleteStudent_ShouldCallRepositoryDeleteStudentOnce()
        {
            var student = new Student { Id = "1", Name = "Alice", Age = 20, Gender = "Female" };
            _mockRepo.Setup(r => r.GetStudentById("1")).Returns(student);

            _service.Remove("1");

            _mockRepo.Verify(r => r.DeleteStudent("1"), Times.Once);
        }

        #endregion

        #region Negative / Validation Scenarios

        [Fact]
        public void GetStudentById_ShouldThrowException_WhenIdIsNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => _service.Get(""));
        }

        [Fact]
        public void GetStudentById_ShouldThrowException_WhenStudentNotFound()
        {
            _mockRepo.Setup(r => r.GetStudentById("99")).Returns((Student?)null);
            Assert.Throws<KeyNotFoundException>(() => _service.Get("99"));
        }

        [Fact]
        public void CreateStudent_ShouldThrowException_WhenStudentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Create(null!));
        }

        [Fact]
        public void CreateStudent_ShouldThrowException_WhenNameIsEmpty()
        {
            var student = new Student { Name = "", Age = 20, Gender = "Male" };
            Assert.Throws<ArgumentException>(() => _service.Create(student));
        }

        [Fact]
        public void CreateStudent_ShouldThrowException_WhenAgeIsInvalid()
        {
            var student = new Student { Name = "Test", Age = 200, Gender = "Male" };
            Assert.Throws<ArgumentException>(() => _service.Create(student));
        }

        [Fact]
        public void CreateStudent_ShouldThrowException_WhenGenderIsInvalid()
        {
            var student = new Student { Name = "Test", Age = 20, Gender = "Unknown" };
            Assert.Throws<ArgumentException>(() => _service.Create(student));
        }

        [Fact]
        public void CreateStudent_ShouldThrowException_WhenCoursesExceedLimit()
        {
            var student = new Student
            {
                Name = "Test",
                Age = 20,
                Gender = "Male",
                Courses = new string[] { "C1", "C2", "C3", "C4", "C5", "C6" }
            };
            Assert.Throws<ArgumentException>(() => _service.Create(student));
        }

        [Fact]
        public void UpdateStudent_ShouldThrowException_WhenIdIsInvalid()
        {
            var student = new Student { Name = "Test", Age = 20, Gender = "Male" };
            Assert.Throws<ArgumentException>(() => _service.Update("", student));
        }

        [Fact]
        public void UpdateStudent_ShouldThrowException_WhenStudentNotFound()
        {
            var student = new Student { Name = "Test", Age = 20, Gender = "Male" };
            _mockRepo.Setup(r => r.GetStudentById("99")).Returns((Student?)null);
            Assert.Throws<KeyNotFoundException>(() => _service.Update("99", student));
        }

        [Fact]
        public void RemoveStudent_ShouldThrowException_WhenIdIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => _service.Remove(""));
        }

        [Fact]
        public void RemoveStudent_ShouldThrowException_WhenStudentNotFound()
        {
            _mockRepo.Setup(r => r.GetStudentById("99")).Returns((Student?)null);
            Assert.Throws<KeyNotFoundException>(() => _service.Remove("99"));
        }

        #endregion
    }
}
