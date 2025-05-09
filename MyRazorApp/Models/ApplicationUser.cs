using Microsoft.AspNetCore.Identity;
using System;

namespace MyRazorApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}