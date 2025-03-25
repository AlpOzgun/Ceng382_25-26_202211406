using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        public static int count = 0;

        [Required]
        public int Id { get; set; }

        [Required]
        public string? ClassName { get; set; }

        [Required]
        public int StudentCount { get; set; }

        [Required]
        public string? Description { get; set; }

        public ClassInformationModel(string className, int studentCount, string description)
        {
            count++;
            Id = count;
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }
    }
}
