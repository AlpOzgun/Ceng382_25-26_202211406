using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Helpers;
using System.Linq;
namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList = new List<ClassInformationModel>();

        public static List<ClassInformationModel> selectedClasses = new List<ClassInformationModel>();

        public ClassInformationModel? EditedClass { get; set; }
        static bool new_page=true;
        public static int CurrentPage { get; set; } = 1;
        public static List<ClassInformationTable> ClassTable_page = new List<ClassInformationTable>();
        public int? MinStudentCount { get; set; }
        public int? MaxStudentCount { get; set; }
        public void OnGet(int? minStudentCount, int? maxStudentCount)
        {           
            //Prompt : how do i keep MinStudentCount,MaxStudentCount static when an onget happens
             MinStudentCount = minStudentCount ?? MinStudentCount;
             MaxStudentCount = maxStudentCount ?? MaxStudentCount;

            if(new_page){
                for(int i=0;i<100;i++){
                    ClassList.Add(new ClassInformationModel($"{i+1}th class",i+1,$"{i+1} students"));
                }
                new_page=false;
            }
            ClassTable_page.Clear();
            int startIndex = (CurrentPage - 1) * 10;
            int prev=selectedClasses.Count;
            //Prompt : how do i use linq to put an upper and lower bound while getting a list
            selectedClasses = ClassList
                .Where(c => (!MinStudentCount.HasValue || c.StudentCount >= MinStudentCount) && 
                            (!MaxStudentCount.HasValue || c.StudentCount <= MaxStudentCount))
                .ToList();
            if(prev != selectedClasses.Count && prev != selectedClasses.Count-1 && prev != selectedClasses.Count+1){
                CurrentPage=1;
                startIndex=0;
            }
            ClassTable_page.AddRange(selectedClasses.Skip(startIndex).Take(10).Select(c => new ClassInformationTable(c)));
            RedirectToPage();
        }

        //Prompt how do i fill a form with the attributes of an object from am list
        public IActionResult OnGetEdit(int id)
        {
            EditedClass = null;
            for (int i = 0; i < ClassList.Count; i++)
            {
                if (ClassList[i].Id == id)
                {
                    EditedClass = ClassList[i];
                    break;
                }
            }

            if (EditedClass == null)
            {
                return RedirectToPage();
            }

            return Page();
        }

        //how do i make a button that takes values from a form for a class and adds to a list in a cshtml and cshtml.cs file
        public IActionResult OnPostAdd(string className, int studentCount, string description)
        {
            var newClass = new ClassInformationModel(className, studentCount, description);
            ClassList.Add(newClass);
            if(ClassList.Count%10!=0)
            CurrentPage=ClassList.Count/10+1;
            else
            CurrentPage=ClassList.Count/10;
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            int i=0;
            for (i = 0; i < ClassList.Count; i++)
            {
                if (ClassList[i].Id == id)
                {
                    break;
                }
            }

            if (ClassList[i] != null)
            {
                if(ClassList.Count%10==1){
                    if(CurrentPage == ClassList.Count/10+1)
                    CurrentPage-=1;
                }
                    
                ClassList.Remove(ClassList[i]);
            }
            return RedirectToPage();
        }

        //Prompt how do i edit that element that i filled the form with
        public IActionResult OnPostEdit(int id, string className, int studentCount, string description)
        {
            ClassInformationModel? UpdatedClass = null;
            for (int i = 0; i < ClassList.Count; i++)
            {
                if (ClassList[i].Id == id)
                {
                    UpdatedClass = ClassList[i];
                    break;
                }
            }

            if (UpdatedClass != null)
            {
                UpdatedClass.ClassName = className;
                UpdatedClass.StudentCount = studentCount;
                UpdatedClass.Description = description;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostChangePage(int Page, int? minStudentCount, int? maxStudentCount)
        {
            if (Page <= ClassList.Count / 10 + 1)
                CurrentPage = Page; 
            // Prompt : How do i keep the values of MinStudentCount,MaxStudentCount after a post request
            MinStudentCount = minStudentCount ?? MinStudentCount;
            MaxStudentCount = maxStudentCount ?? MaxStudentCount;
            return RedirectToPage(new { minStudentCount = MinStudentCount, maxStudentCount = MaxStudentCount});
        }


        public IActionResult OnPostExportJson(List<string> selectedColumns)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JSON", "exported_classes.json");
            string? JSONPath=Path.GetDirectoryName(filePath);
            if(JSONPath != null){
            Directory.CreateDirectory(JSONPath);
            Utils.ExportJsonToFile(selectedClasses, filePath, selectedColumns);
            }
            return RedirectToPage();
        }


    }

}
