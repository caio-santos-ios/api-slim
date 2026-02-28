using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class UpdateSellerRepresentativeDTO
        {
                public string Id { get; set; } = string.Empty;
                
                public string Notes { get; set; } = string.Empty;
                
                public string CNPJ { get; set; } = string.Empty;
                
                public string TradeName { get; set; } = string.Empty;

                public string CorporateName { get; set; } = string.Empty;

                public string Phone { get; set; } = string.Empty;

                public string Whatsapp { get; set; } = string.Empty;
                public string Email { get; set; } = string.Empty;
                public string Gender { get; set; } = string.Empty;
                public DateTime EffectiveDate { get; set; }
                public Address Address { get; set; } = new();
                public Contact Contact { get; set; } = new();
                public Seller Seller { get; set; } = new();
                public CreateResponsibleDTO Responsible { get; set; } = new();
                public RepresentativeBank Bank { get; set; } = new();
        }
}