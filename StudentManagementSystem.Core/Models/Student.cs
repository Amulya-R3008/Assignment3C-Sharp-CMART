using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentManagementSystem.Core.Models
{
    [BsonIgnoreExtraElements]
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [BsonElement("graduated")]
        public bool Graduated { get; set; }

        [BsonElement("courses")]
        public string[]? Courses { get; set; }

        [BsonElement("gender")]
        [Required]
        [RegularExpression("Male|Female|Other")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("age")]
        [Required]
        [Range(1, 120)]
        public int Age { get; set; }
    }
}
