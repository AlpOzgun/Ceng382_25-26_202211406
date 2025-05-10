using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Linq;
using MyRazorApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MyApp.Namespace
{
    public class TableModel : PageModel
    {
        public static List<Class> Db_selectedClasses = new List<Class>();
        public Class? Db_EditedClass { get; set; }
        static bool new_page=true;
        public static int CurrentPage { get; set; } = 1;
        public static List<ClassInformationTable> Db_ClassTable_page = new List<ClassInformationTable>();
        public int? MinStudentCount { get; set; }
        public int? MaxStudentCount { get; set; }
         private readonly SchoolDbContext _context;



        public TableModel(SchoolDbContext context)
        {
            _context = context;
        }
        public IList<Class> DbClassList { get; set; }
        public async Task OnGetAsync(int? minStudentCount, int? maxStudentCount)
        {           
            
            //Prompt : how do i keep MinStudentCount,MaxStudentCount static when an onget happens
             MinStudentCount = minStudentCount ?? MinStudentCount;
             MaxStudentCount = maxStudentCount ?? MaxStudentCount;
            int startIndex = (CurrentPage - 1) * 10;
            //Prompt : how do i use linq to put an upper and lower bound while getting a list

            
            if(!await _context.Classes.AnyAsync(c => c.IsActive)){
                for(int i=0;i<100;i++){
                    _context.Classes.Add(new Class{
                        Name=$"{i+1}th class",
                        PersonCount = i+1,
                        Description = $"{i+1} students",
                        IsActive=true
                    });
                }
                new_page=false;
                await _context.SaveChangesAsync();
                
                         }   
            DbClassList = await _context.Classes.ToListAsync();
            int db_prev=Db_selectedClasses.Count;
            Db_selectedClasses =DbClassList.Where(c => (!MinStudentCount.HasValue || c.PersonCount >= MinStudentCount) && 
                            (!MaxStudentCount.HasValue || c.PersonCount <= MaxStudentCount) && c.IsActive).ToList();
            if(db_prev != Db_selectedClasses.Count && db_prev != Db_selectedClasses.Count-1 && db_prev != Db_selectedClasses.Count+1){
                CurrentPage=1;
                startIndex=0;
            } 
            Db_ClassTable_page.Clear();
            Db_ClassTable_page.AddRange(Db_selectedClasses.Skip(startIndex).Take(10).Select(c => new ClassInformationTable(c)));

            RedirectToPage();
        }

        //Prompt how do i fill a form with the attributes of an object from am list
        public async Task<IActionResult> OnGetEdit(int id)
        {

            Db_EditedClass=null;
            
            Db_EditedClass=await _context.Classes.Where(d=>d.IsActive).FirstOrDefaultAsync(c => c.Id==id);

            return Page();
        }

        //how do i make a button that takes values from a form for a class and adds to a list in a cshtml and cshtml.cs file
        public async Task<IActionResult> OnPostAdd(string className, int studentCount, string description)
        {
                var newClass = new Class
                {
                    Name = className,
                    PersonCount = studentCount,
                    Description = description,
                    IsActive = true
                };
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                DbClassList = await _context.Classes.Where(c=>c.IsActive).ToListAsync();

                if(DbClassList.Count%10!=0)
                    CurrentPage=DbClassList.Count/10+1;
                else
                    CurrentPage=DbClassList.Count/10;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            Class? removed;

            if(_context.Classes != null){
                removed = await _context.Classes.FirstOrDefaultAsync(k => k.Id==id);
                if(removed != null){
                    removed.IsActive=false;
                    await _context.SaveChangesAsync();
                    DbClassList = await _context.Classes.Where(c=>c.IsActive).ToListAsync();
                    if(CurrentPage*10 == DbClassList.Count+10)
                    CurrentPage-=1;
                }
            }
            return RedirectToPage();
        }

        //Prompt how do i edit that element that i filled the form with
        public async Task<IActionResult> OnPostEdit(int id, string className, int studentCount, string description)
        {

            Class? Updated;
            if(_context.Classes != null){
                Updated = await _context.Classes.FirstOrDefaultAsync(c=>c.Id==id);
                if(Updated != null){
                    Updated.Name=className;
                    Updated.Description=description;
                    Updated.PersonCount=studentCount;
                    await _context.SaveChangesAsync();
                }
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePage(int Page, int? minStudentCount, int? maxStudentCount)
        {
            DbClassList = await _context.Classes.Where(c=>c.IsActive).ToListAsync();
            if (Page <= DbClassList.Count / 10 + 1)
                CurrentPage = Page; 
            // Prompt : How do i keep the values of MinStudentCount,MaxStudentCount after a post request
            MinStudentCount = minStudentCount ?? MinStudentCount;
            MaxStudentCount = maxStudentCount ?? MaxStudentCount;
            return RedirectToPage(new { minStudentCount = MinStudentCount, maxStudentCount = MaxStudentCount});
        }


        public async Task<IActionResult> OnPostExportJson(List<string> selectedColumns,int? minStudentCount, int? maxStudentCount)
        {
            MinStudentCount=minStudentCount;
            MaxStudentCount=maxStudentCount;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JSON", "exported_classes.json");
            string? JSONPath=Path.GetDirectoryName(filePath);
            DbClassList=await _context.Classes.ToListAsync();
            Db_selectedClasses =DbClassList.Where(c => (!MinStudentCount.HasValue || c.PersonCount >= MinStudentCount) && 
                            (!MaxStudentCount.HasValue || c.PersonCount <= MaxStudentCount) && c.IsActive).ToList();
            if(JSONPath != null){
            Directory.CreateDirectory(JSONPath);
            Utils.ExportJsonToFile(Db_selectedClasses, filePath, selectedColumns);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostLogOut(){
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Token");
            Response.Cookies.Delete("SessionID");
             return RedirectToPage("/Index");
        }
    }
}
