using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList = new List<ClassInformationModel>();

        public ClassInformationModel? EditedClass { get; set; }

        public void OnGet()
        {
        }
        
        //how do i make a button that takes values from a form for a class and adds to a list in a cshtml and cshtml.cs file
        public IActionResult OnPostAdd(string className, int studentCount, string description)
        {
            var newClass = new ClassInformationModel(className, studentCount, description);
            ClassList.Add(newClass);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            for (int i = 0; i < ClassList.Count; i++)
            {
                if (ClassList[i].Id == id)
                {
                    ClassList.Remove(ClassList[i]);
                    break;
                }
            }
            return RedirectToPage();
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
    }
}
