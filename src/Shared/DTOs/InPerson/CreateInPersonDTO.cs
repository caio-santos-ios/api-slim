namespace api_slim.src.Shared.DTOs
{
    public class CreateInPersonDTO
    {
        public string RecipientId {get;set;} = string.Empty; 
        public string AccreditedNetworkId {get;set;} = string.Empty; 
        public string ServiceModuleId {get;set;} = string.Empty;
        public string ProfessionalId {get;set;} = string.Empty; 
        public List<string> ProcedureIds {get;set;} = [];
        public DateTime? Date { get; set; }
        public string Hour { get; set; } = string.Empty;
        public string ResponsiblePayment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value {get;set;}
    }
}