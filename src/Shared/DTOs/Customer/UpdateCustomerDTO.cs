using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
public class UpdateCustomerDTO : Request
{
        public string Id { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;

        public string CorporateName { get; set; } = string.Empty; // Mapeado do campo "Nome"
        
        public string TradeName { get; set; } = string.Empty; // Mapeado do campo "Nome"

        public string Document { get; set; } = string.Empty; // Mapeado do campo "CPF"

        public string Rg { get; set; } = string.Empty; // Mapeado do campo "RG"

        public DateTime DateOfBirth { get; set; } // Mapeado do campo "Data de Nascimento"

        public string Gender { get; set; } = string.Empty; // Mapeado do campo "Gênero"

        public Address Address { get; set; } = new Address();

        public string Phone { get; set; } = string.Empty; // Mapeado do campo "Telefone"

        public string Whatsapp { get; set; } = string.Empty; // Mapeado do campo "Whatsapp"

        public string Email { get; set; } = string.Empty; // Mapeado do campo "E-mail"

        public string Segment { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        public string Origin { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

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

        public class CreateContractorResponsibleDTO
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