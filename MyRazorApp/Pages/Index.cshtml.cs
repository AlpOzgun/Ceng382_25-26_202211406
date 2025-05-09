using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SchoolDbContext _context;
        
        public static bool wrong { get; set; }
        public IList<ApplicationUser> UserList { get; set; }

        public IndexModel(SchoolDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_context.Users != null)
                UserList = await _context.Users.Where(c => c.IsActive).ToListAsync();

            var roleManager = HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (UserList.Count == 0)
            {
                var admin1 = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",  
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                var result1 = await _userManager.CreateAsync(admin1, "Admin@1234");
                if (result1.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin1, "Admin");
                }

                var admin2 = new ApplicationUser
                {
                    UserName = "admin2",
                    Email = "admin2@example.com",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                var result2 = await _userManager.CreateAsync(admin2, "Admin@12345");
                if (result2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin2, "Admin");
                }

                var admin3 = new ApplicationUser
                {
                    UserName = "admin3",
                    Email = "admin3@example.com",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                var result3 = await _userManager.CreateAsync(admin3, "Admin@123");
                if (result3.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin3, "Admin");
                }

                await _context.SaveChangesAsync();
                UserList = await _context.Users.Where(c => c.IsActive).ToListAsync();
            }

            if (HttpContext.Session.GetString("Token") == Request.Cookies["Token"] && Request.Cookies["Token"] != null)
            {
                HttpContext.Session.SetString("Username", Request.Cookies["Username"]);
                HttpContext.Session.SetString("token", Request.Cookies["token"]);

                return RedirectToPage("/Table");
            }

            wrong = false;
            return Page();
        }

        public async Task<IActionResult> OnPostLogin(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password) || !user.IsActive)
            {
                wrong = true;
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                string token = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Token", token);
                Response.Cookies.Append("Username", username);
                Response.Cookies.Append("Token", token);
                Response.Cookies.Append("SessionID", HttpContext.Session.Id);
                string? roles = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                Response.Cookies.Append("UserRole",roles);
                return RedirectToPage("/Table");
            }

            wrong = true;
            return Page();
        }
    }
}
