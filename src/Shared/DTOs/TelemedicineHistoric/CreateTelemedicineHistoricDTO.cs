namespace api_slim.src.Shared.DTOs
{
        public class CreateTelemedicineHistoricDTO : Request
        {
                public string RecipientId {get;set;} = string.Empty;
                public string RecipientName {get;set;} = string.Empty;
                public string SpecialistId {get;set;} = string.Empty;
                public string SpecialistName {get;set;} = string.Empty;
                public string Status {get;set;} = string.Empty;   
                public string Date {get;set;} = string.Empty;     
                public string Time {get;set;} = string.Empty; 
                public string Type {get;set;} = string.Empty; 
                public string ParentId {get;set;} = string.Empty; 
                public string ParentUuid {get;set;} = string.Empty; 
                public string BeneficiaryCPF {get;set;} = string.Empty; 
        }
}