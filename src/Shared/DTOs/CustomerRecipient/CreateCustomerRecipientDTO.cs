using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class CreateCustomerRecipientDTO : Request
        {
                public string DocumentContract { get; set; } = string.Empty; // Mapeado do campo "CPF"
                public string Name { get; set; } = string.Empty; // Mapeado do campo "Nome"
                public string Cpf { get; set; } = string.Empty; // Mapeado do campo "CPF"
                public string Rg { get; set; } = string.Empty; // Mapeado do campo "RG"
                public Address Address { get; set; } = new Address();
                public DateTime? DateOfBirth { get; set; } // Mapeado do campo "Data de Nascimento"
                public string Gender { get; set; } = string.Empty; // Mapeado do campo "Gênero"
                public string Phone { get; set; } = string.Empty; // Mapeado do campo "Telefone"
                public string Whatsapp { get; set; } = string.Empty; // Mapeado do campo "Whatsapp"
                public string Email { get; set; } = string.Empty; // Mapeado do campo "E-mail"
                public string PlanId { get; set; } = string.Empty; 
                public string Notes { get; set; } = string.Empty;
                public string ContractorId { get; set; } = string.Empty;
                public string Bond { get; set; } = string.Empty;
                public decimal SubTotal { get; set; }
                public decimal Total { get; set; }
                public decimal Discount { get; set; }
                public decimal DiscountPercentage { get; set; }
                public DateTime? EffectiveDate { get; set; }
        }
}