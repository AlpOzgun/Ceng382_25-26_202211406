using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Linq;
using System.Text.Json;
using MyRazorApp.Data;
using Microsoft.EntityFrameworkCore;
namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<User> Users = new List<User>();
        
        private readonly SchoolDbContext _context;
        public static bool wrong { get; set; }

        public IList<Class> ClassList { get; set; }
        public IList<User> UserList { get; set; }

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            
            if(_context.Users != null)
            UserList = await _context.Users.Where(c=>c.IsActive).ToListAsync();
            if(UserList.Count==0){
                _context.Users.Add(new User{
                    CreatedAt = DateTime.Now,
                    Password = "1234",
                    Role = "Admin",
                    Username="admin"
                });
                _context.Users.Add(new User{
                    CreatedAt = DateTime.Now,
                    Password = "123456",
                    Role = "Admin",
                    Username="admin2"
                });
                _context.Users.Add(new User{
                    CreatedAt = DateTime.Now,
                    Password = "123",
                    Role = "Admin",
                    Username="admin3"
                });
                await _context.SaveChangesAsync();
                UserList = await _context.Users.Where(c=>c.IsActive).ToListAsync();
            }
            if(HttpContext.Session.GetString("Token") == Request.Cookies["Token"] && Request.Cookies["Token"] != null){
                HttpContext.Session.SetString("Username",Request.Cookies["Username"]);
                HttpContext.Session.SetString("token",Request.Cookies["token"]);

                return RedirectToPage("/Table");
            }
            wrong=false;
            return Page();
        }


        public async Task<IActionResult> OnPostLogin(string Username,string Password){
            UserList = await _context.Users.ToListAsync();
            User? user = UserList.FirstOrDefault(u => u.Username == Username && u.Password==Password && u.IsActive);
            if(user==null){
                wrong=true;
                return Page();
            }
            

            HttpContext.Session.SetString("Username",Username);
            Response.Cookies.Append("Username",Username);

            string token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("Token",token);
            Response.Cookies.Append("Token",token);

            Response.Cookies.Append("SessionID",HttpContext.Session.Id);

            return RedirectToPage("/Table");
        }
    }

}
