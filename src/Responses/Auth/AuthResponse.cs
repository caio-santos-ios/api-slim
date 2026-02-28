using api_slim.src.Models;

namespace api_slim.src.Responses
{
    public class AuthResponse
    {
        public string Token {get;set;} = string.Empty; 
        public string RefreshToken  {get;set;} = string.Empty; 
        public string Name {get;set;} = string.Empty; 
        public string Role {get;set;} = string.Empty; 
        public bool Admin {get; set;}
        public string Id {get;set;} = string.Empty; 
        public string Photo {get;set;} = string.Empty; 
        public List<Module> Modules {get;set;} = [];
        public DateTime Expires {get;set;}
    }
}