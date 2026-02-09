namespace api_slim.src.Responses
{
    public class AuthAppResponse
    {
        public string Token {get;set;} = string.Empty; 
        public string RefreshToken  {get;set;} = string.Empty; 
        public string Name {get;set;} = string.Empty; 
        public string Photo {get;set;} = string.Empty; 
        public string RapidocId {get;set;} = string.Empty; 
        public bool FirstAccess {get;set;}
    }
}