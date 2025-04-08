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
        public static void ExportJsonToFile<T>(List<T> list, string filePath)
        {
            string jsonData = ExportToJson(list);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
