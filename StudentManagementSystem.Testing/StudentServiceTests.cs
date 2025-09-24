using Xunit;
using Moq;
using System.Collections.Generic;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Repositories;
using StudentManagementSystem.Services.Services;

namespace StudentManagementSystem.Testing
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
            var student = new Student { Id = "1", Name = "Alice", Age = 20 };
            _mockRepo.Setup(r => r.GetStudentById("1")).Returns(student);

            var result = _service.Get("1");

            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public void GetStudentById_ShouldReturnNull_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetStudentById("99")).Returns((Student?)null);

            var result = _service.Get("99");

            Assert.Null(result);
        }

        [Fact]
        public void CreateStudent_ShouldCallRepositoryAddStudentOnce()
        {
            var student = new Student { Id = "3", Name = "Charlie", Age = 25 };

            var result = _service.Create(student);

            _mockRepo.Verify(r => r.AddStudent(student), Times.Once);
            Assert.Equal(student, result);
        }

        [Fact]
        public void UpdateStudent_ShouldCallRepositoryUpdateStudentOnce()
        {
            var student = new Student { Id = "1", Name = "Updated", Age = 21 };

            _service.Update("1", student);

            _mockRepo.Verify(r => r.UpdateStudent("1", student), Times.Once);
        }

        [Fact]
        public void DeleteStudent_ShouldCallRepositoryDeleteStudentOnce()
        {
            _service.Remove("1");

            _mockRepo.Verify(r => r.DeleteStudent("1"), Times.Once);
        }
    }
}
