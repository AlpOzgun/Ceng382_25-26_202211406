using System.ComponentModel.DataAnnotations;
namespace MyRazorApp.Models
{
 public class Class
 {
 [Key]
 public int Id { get; set; }
 [Required]
 public string Name { get; set; } = null!;
 [Required]
 public int PersonCount { get; set; }
 public string Description { get; set; } = null!;
 [Required]
 public bool IsActive { get; set; }
 }
}