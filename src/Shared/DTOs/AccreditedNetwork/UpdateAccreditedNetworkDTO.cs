using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
    public class UpdateAccreditedNetworkDTO : Request
    {
        public string Id { get; set; } = string.Empty;            
        public string CNPJ { get; set; } = string.Empty;        
        public string TradeName { get; set; } = string.Empty;
        public string CorporateName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;        
        public string Notes { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;        
        public string ServiceModuleId { get; set; } = string.Empty;        
        public DateTime? EffectiveDate { get; set; }        
        public decimal ConsumptionLimit { get; set; }        
        public string TradingTable { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
        public string BillingId { get; set; } = string.Empty;
        public CreateAccreditedNetworkResponsibleDTO Responsible { get; set; } = new();
        public string Justification { get; set; } = string.Empty;
    }
}