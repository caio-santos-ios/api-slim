using api_slim.src.Models.Base;

namespace api_slim.src.Shared.DTOs
{
    public class CreateMetricAppDTO : ModelBase
    {
        public string Screen {get;set;} = string.Empty;
        public string Action {get;set;} = string.Empty;        
        public string Function {get;set;} = string.Empty;
        public string Description {get;set;} = string.Empty;     
        public string ParentId {get;set;} = string.Empty;     
        public string Parent {get;set;} = string.Empty;      
    }
}