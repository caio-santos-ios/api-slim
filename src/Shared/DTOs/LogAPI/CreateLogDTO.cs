namespace api_slim.src.Shared.DTOs
{
    public class CreateLogDTO
    {
        public string Collection {get;set;} = string.Empty;
        public string Action {get;set;} = string.Empty;   
        public string Description {get;set;} = string.Empty;     
        public string ParentId {get;set;} = string.Empty;     
        public string Parent {get;set;} = string.Empty;     
    }
}