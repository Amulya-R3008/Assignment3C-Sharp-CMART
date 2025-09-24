using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StudentManagementSystem.Core.Models;
using Xunit;

namespace StudentManagementSystem.ModelTests
{
    public class StudentModelTests
    {
        // Helper method to validate a model
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Student_WithValidData_ShouldPassValidation()
        {
            var student = new Student
            {
                Name = "Alice",
                Age = 25,
                Gender = "Female",
                IsGraduated = true,
                Courses = new[] { "Math", "Science" }
            };

            var results = ValidateModel(student);

            Assert.Empty(results); // No validation errors
        }

        [Fact]
        public void Student_WithoutName_ShouldFailValidation()
        {
            var student = new Student
            {
                Name = "", // Invalid
                Age = 25,
                Gender = "Female"
            };

            var results = ValidateModel(student);

            Assert.Contains(results, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Student_WithInvalidAge_ShouldFailValidation()
        {
            var student = new Student
            {
                Name = "Bob",
                Age = 150, // Invalid
                Gender = "Male"
            };

            var results = ValidateModel(student);

            Assert.Contains(results, v => v.MemberNames.Contains("Age"));
        }

        [Fact]
        public void Student_WithInvalidGender_ShouldFailValidation()
        {
            var student = new Student
            {
                Name = "Charlie",
                Age = 30,
                Gender = "Unknown" // Invalid
            };

            var results = ValidateModel(student);

            Assert.Contains(results, v => v.MemberNames.Contains("Gender"));
        }

        [Fact]
        public void Student_NameTooShort_ShouldFailValidation()
        {
            var student = new Student
            {
                Name = "A", // Too short
                Age = 22,
                Gender = "Male"
            };

            var results = ValidateModel(student);

            Assert.Contains(results, v => v.MemberNames.Contains("Name"));
        }
    }
}
