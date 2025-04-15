using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Linq;
using System.Text.Json;
namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<User> Users = new List<User>();
        public static bool wrong { get; set; }
        public IActionResult  OnGet()
        {
            if(HttpContext.Session.GetString("Token") == Request.Cookies["Token"] && Request.Cookies["Token"] != null){
                HttpContext.Session.SetString("Username",Request.Cookies["Username"]);
                HttpContext.Session.SetString("token",Request.Cookies["token"]);

                return RedirectToPage("/Table");
            }
            wrong=false;
            if(Users.Count==0){
                
                var filePath = Path.Combine("wwwroot", "JSON", "UsersJSON.json");
                string jsonString = System.IO.File.ReadAllText(filePath);
                Users = JsonSerializer.Deserialize<List<User>>(jsonString);
            }
            return Page();
        }

        public IActionResult OnPostLogin(string Username,string Password){
            User? user = Users.FirstOrDefault(u => u.Username == Username && u.Password==Password && u.IsActive);

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
