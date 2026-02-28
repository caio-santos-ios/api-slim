using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class CreateSellerRepresentativeDTO
        {
                public string Notes { get; set; } = string.Empty;
                public string CNPJ { get; set; } = string.Empty;
                public string TradeName { get; set; } = string.Empty;
                public string CorporateName { get; set; } = string.Empty;
                public string Phone { get; set; } = string.Empty;
                public string Whatsapp { get; set; } = string.Empty;
                public string Email { get; set; } = string.Empty;
                public DateTime EffectiveDate { get; set; }
                public Address Address { get; set; } = new();
                public Contact Contact { get; set; } = new();
                // public Seller Seller { get; set; } = new();
                // public CreateResponsibleDTO Responsible { get; set; } = new();
                public RepresentativeBank Bank { get; set; } = new();        
        }
        public class CreateResponsibleDTO
        {
                public DateTime? DateOfBirth { get; set; }
                public string Gender { get; set; } = string.Empty;
                public string Name { get; set; } = string.Empty;
                public string Cpf { get; set; } = string.Empty;
                public string Rg { get; set; } = string.Empty;
                public string Phone { get; set; } = string.Empty;
                public string Whatsapp { get; set; } = string.Empty;
                public string Notes { get; set; } = string.Empty;
                public string Email { get; set; } = string.Empty;
                public Address Address { get; set; } = new Address();
        }
}