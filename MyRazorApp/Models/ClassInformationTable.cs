namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public string? ClassName { get; set; }
        public int? StudentCount { get; set; }
        public string? Description { get; set; }
        public int? Id { get; set; } 

        public ClassInformationTable(ClassInformationModel model){
                ClassName=model.ClassName;
                StudentCount=model.StudentCount;
                Description=model.Description;
                Id=model.Id;
        }
        public ClassInformationTable(Class model){
                ClassName=model.Name;
                StudentCount=model.PersonCount;
                Description=model.Description;
                Id=model.Id;
        }

    }
}
