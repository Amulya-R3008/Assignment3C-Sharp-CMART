using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Services.Services;

namespace StudentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
        {
            _logger = logger;
            this.studentService = studentService;
        }

        // GET: api/students
        [HttpGet]
        public ActionResult<List<Student>> Get()
        {
            _logger.LogInformation("Fetching all students");
            var students = studentService.Get();
            _logger.LogInformation("Successfully fetched {Count} students", students.Count);
            return students;
        }

        // GET api/students/{id}
        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GET request failed: Student Id is required");
                return BadRequest("Student Id is required.");
            }

            _logger.LogInformation("Fetching student with Id={Id}", id);
            var student = studentService.Get(id);

            if (student == null)
            {
                _logger.LogWarning("Student with Id={Id} not found", id);
                return NotFound($"Student with Id={id} not found");
            }

            _logger.LogInformation("Student with Id={Id} found", id);
            return student;
        }

        // POST api/students
        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            if (student == null)
            {
                _logger.LogWarning("POST request failed: Student object is null");
                return BadRequest("Student cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(student.Name))
            {
                _logger.LogWarning("POST request failed: Student Name is required");
                return BadRequest("Student name is required.");
            }

            if (student.Age < 0 || student.Age > 120)
            {
                _logger.LogWarning("POST request failed: Invalid age {Age}", student.Age);
                return BadRequest("Student age must be between 0 and 120.");
            }

            if (!string.IsNullOrWhiteSpace(student.Gender) &&
                student.Gender.ToLower() != "male" &&
                student.Gender.ToLower() != "female" &&
                student.Gender.ToLower() != "other")
            {
                _logger.LogWarning("POST request failed: Invalid gender {Gender}", student.Gender);
                return BadRequest("Gender must be Male, Female, or Other.");
            }

            studentService.Create(student);
            _logger.LogInformation("Successfully created student with Id={Id}", student.Id);
            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }

        // PUT api/students/{id}
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("PUT request failed: Student Id is required");
                return BadRequest("Student Id is required.");
            }

            if (student == null)
            {
                _logger.LogWarning("PUT request failed: Student object is null");
                return BadRequest("Student cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(student.Name))
            {
                _logger.LogWarning("PUT request failed: Student Name is required");
                return BadRequest("Student name is required.");
            }

            if (student.Age < 0 || student.Age > 120)
            {
                _logger.LogWarning("PUT request failed: Invalid age {Age}", student.Age);
                return BadRequest("Student age must be between 0 and 120.");
            }

            var existingStudent = studentService.Get(id);
            if (existingStudent == null)
            {
                _logger.LogWarning("Cannot update. Student with Id={Id} not found", id);
                return NotFound($"Student with Id={id} not found");
            }

            studentService.Update(id, student);
            _logger.LogInformation("Successfully updated student with Id={Id}", id);
            return NoContent();
        }

        // DELETE api/students/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("DELETE request failed: Student Id is required");
                return BadRequest("Student Id is required.");
            }

            var student = studentService.Get(id);
            if (student == null)
            {
                _logger.LogWarning("Cannot delete. Student with Id={Id} not found", id);
                return NotFound($"Student with Id={id} not found");
            }

            studentService.Remove(student.Id);
            _logger.LogInformation("Successfully deleted student with Id={Id}", id);
            return Ok($"Student with Id={id} deleted");
        }
    }
}
