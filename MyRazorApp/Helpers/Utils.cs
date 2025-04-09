using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using MyRazorApp.Models;

namespace MyRazorApp.Helpers
{
    public static class Utils
    {
        public static string ExportToJson<T>(List<T> list)
        {
            return JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        }
        public static string ExportToJson<T>(List<T> list ,List<string> selectedProperties)
        {
            //Prompt : how do i check if a list of strings matches the parameters of a generic class list with many parameters
            var properties = typeof(T).GetProperties();
            var filteredList = list.Select(item => properties
                .Where(p => selectedProperties.Contains(p.Name))
                .ToDictionary(p => p.Name, p => p.GetValue(item)))
                .ToList();
             return JsonSerializer.Serialize(filteredList, new JsonSerializerOptions { WriteIndented = true });
        }


        public static void ExportJsonToFile<T>(List<T> list, string filePath,List<string> selectedColumns)
        {
            string jsonData="";
            if(selectedColumns==null || selectedColumns.Count==0)
            jsonData = ExportToJson(list);
            else
            jsonData = ExportToJson(list,selectedColumns);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
