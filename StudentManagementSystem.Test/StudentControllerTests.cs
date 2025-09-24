using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Services.Services;
using StudentManagementSystem.API.Controllers;

namespace StudentManagementSystem.Test
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _mockService;
        private readonly Mock<ILogger<StudentsController>> _mockLogger;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _mockService = new Mock<IStudentService>();
            _mockLogger = new Mock<ILogger<StudentsController>>();
            _controller = new StudentsController(_mockLogger.Object, _mockService.Object);
        }

        [Fact]
        public void GetAll_ReturnsListOfStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = "1", Name = "Alice", Age = 20 },
                new Student { Id = "2", Name = "Bob", Age = 22 }
            };
            _mockService.Setup(s => s.Get()).Returns(students);

            // Act
            var result = _controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Student>>>(result);
            var returnValue = Assert.IsType<List<Student>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public void GetById_ReturnsStudent_WhenExists()
        {
            var student = new Student { Id = "1", Name = "Alice", Age = 20 };
            _mockService.Setup(s => s.Get("1")).Returns(student);

            var result = _controller.Get("1");

            var actionResult = Assert.IsType<ActionResult<Student>>(result);
            var returnValue = Assert.IsType<Student>(actionResult.Value);
            Assert.Equal("Alice", returnValue.Name);
        }

        [Fact]
        public void GetById_ReturnsBadRequest_WhenIdIsNullOrEmpty()
        {
            var result = _controller.Get("");

            var actionResult = Assert.IsType<ActionResult<Student>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void Post_ReturnsCreatedStudent_WhenValid()
        {
            var student = new Student { Id = "1", Name = "Alice", Age = 20, Gender = "Female" };

            var result = _controller.Post(student);

            var actionResult = Assert.IsType<ActionResult<Student>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Student>(createdAtActionResult.Value);
            Assert.Equal("Alice", returnValue.Name);
            _mockService.Verify(s => s.Create(student), Times.Once);
        }

        [Fact]
        public void Post_ReturnsBadRequest_WhenStudentIsNull()
        {
            var result = _controller.Post(null);

            var actionResult = Assert.IsType<ActionResult<Student>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void Put_ReturnsNoContent_WhenUpdateIsValid()
        {
            var student = new Student { Id = "1", Name = "Alice Updated", Age = 21, Gender = "Female" };
            _mockService.Setup(s => s.Get("1")).Returns(student);

            var result = _controller.Put("1", student);

            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.Update("1", student), Times.Once);
        }

        [Fact]
        public void Put_ReturnsBadRequest_WhenIdIsEmpty()
        {
            var student = new Student { Id = "1", Name = "Alice", Age = 20 };
            var result = _controller.Put("", student);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_ReturnsOk_WhenStudentExists()
        {
            var student = new Student { Id = "1", Name = "Alice" };
            _mockService.Setup(s => s.Get("1")).Returns(student);

            var result = _controller.Delete("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockService.Verify(s => s.Remove("1"), Times.Once);
        }

        [Fact]
        public void Delete_ReturnsBadRequest_WhenIdIsEmpty()
        {
            var result = _controller.Delete("");
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
