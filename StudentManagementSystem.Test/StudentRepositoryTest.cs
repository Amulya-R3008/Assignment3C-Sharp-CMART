using Xunit;
using Moq;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories;

namespace StudentManagementSystem.Test
{
    public class StudentRepositoryTests
    {
        private readonly Mock<IMongoDbContext> _mockContext;
        private readonly Mock<IMongoCollection<Student>> _mockCollection;
        private readonly StudentRepository _repository;

        public StudentRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Student>>();
            _mockContext = new Mock<IMongoDbContext>();
            _mockContext.Setup(c => c.Students).Returns(_mockCollection.Object);

            _repository = new StudentRepository(_mockContext.Object);
        }

        [Fact]
        public void GetAllStudents_ShouldReturnListOfStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = "1", Name = "Alice" },
                new Student { Id = "2", Name = "Bob" }
            }.AsQueryable();

            var mockCursor = new Mock<IAsyncCursor<Student>>();
            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                      .Returns(true).Returns(false);
            mockCursor.SetupGet(c => c.Current).Returns(students);

            _mockCollection.Setup(c => c.FindSync(
                It.IsAny<FilterDefinition<Student>>(),
                It.IsAny<FindOptions<Student, Student>>(),
                default)).Returns(mockCursor.Object);

            // Act
            var result = _repository.GetAllStudents();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
        }

        [Fact]
        public void GetStudentById_ShouldReturnStudent_WhenExists()
        {
            // Arrange
            var student = new Student { Id = "1", Name = "Alice" };
            var students = new List<Student> { student }.AsQueryable();

            var mockCursor = new Mock<IAsyncCursor<Student>>();
            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                      .Returns(true).Returns(false);
            mockCursor.SetupGet(c => c.Current).Returns(students);

            _mockCollection.Setup(c => c.FindSync(
                It.IsAny<FilterDefinition<Student>>(),
                It.IsAny<FindOptions<Student, Student>>(),
                default)).Returns(mockCursor.Object);

            // Act
            var result = _repository.GetStudentById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public void AddStudent_ShouldCallInsertOne()
        {
            // Arrange
            var student = new Student { Id = "1", Name = "Alice" };

            // Act
            _repository.AddStudent(student);

            // Assert
            _mockCollection.Verify(c => c.InsertOne(student, null, default), Times.Once);
        }

        [Fact]
        public void UpdateStudent_ShouldCallReplaceOne()
        {
            // Arrange
            var student = new Student { Id = "1", Name = "Alice Updated" };

            // Act
            _repository.UpdateStudent("1", student);

            // Assert
            _mockCollection.Verify(c => c.ReplaceOne(It.IsAny<FilterDefinition<Student>>(),
                                                    student,
                                                    It.IsAny<ReplaceOptions>(),
                                                    default),
                                   Times.Once);
        }

        [Fact]
        public void DeleteStudent_ShouldCallDeleteOne()
        {
            // Act
            _repository.DeleteStudent("1");

            // Assert
            _mockCollection.Verify(c => c.DeleteOne(It.IsAny<FilterDefinition<Student>>(), default), Times.Once);
        }
    }
}
