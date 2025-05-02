using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class User
    {
        [Key]
        public int Id{get; set;}
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; } 
        [Required]
        public string? Role { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
    }
}