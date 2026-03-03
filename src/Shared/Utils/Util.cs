using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace api_slim.src.Shared.Utils
{
    public static class Util
    {
        public static void ConsoleLog(dynamic obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        public static dynamic GenerateCodeAccess()
        {
            return new {
                CodeAccess = new Random().Next(100000, 999999).ToString(),
                CodeAccessExpiration = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public static string CleanPhone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone)) return string.Empty;
            
            return Regex.Replace(telefone, @"[^\d]", "");
        }
    }
}