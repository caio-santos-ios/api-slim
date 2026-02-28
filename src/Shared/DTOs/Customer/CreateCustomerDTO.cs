using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class CreateCustomerDTO : Request
        {
                public string Type { get; set; } = string.Empty;
                public string CorporateName { get; set; } = string.Empty;         
                public string TradeName { get; set; } = string.Empty; 
                public string Document { get; set; } = string.Empty; 
                public string Rg { get; set; } = string.Empty; 
                public DateTime? DateOfBirth { get; set; }
                public string Gender { get; set; } = string.Empty; 
                public Address Address { get; set; } = new Address();
                public string Phone { get; set; } = string.Empty; 
                public string Whatsapp { get; set; } = string.Empty; 
                public string Email { get; set; } = string.Empty;
                public string Segment { get; set; } = string.Empty;
                public string Origin { get; set; } = string.Empty;
                public CreateContractorResponsibleDTO Responsible { get; set; } = new();
                public DateTime? EffectiveDate { get; set; }
                public string PlanId { get; set; } = string.Empty; 
                public string Notes { get; set; } = string.Empty;       
                public decimal MinimumValue { get; set; }
                public string TypePlan { get; set; } = string.Empty;
                public string SellerId { get; set; } = string.Empty;
                public string TaxRegime { get; set; } = string.Empty;
                public string TechnicalManager { get; set; } = string.Empty;
                public decimal RatRate { get; set; }
                public decimal SheetValue { get; set; }
        }
}