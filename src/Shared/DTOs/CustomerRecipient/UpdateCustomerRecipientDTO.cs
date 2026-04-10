using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class UpdateCustomerRecipientDTO : Request
        {
                public string Id { get; set; } = string.Empty;
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
                public string Justification { get; set; } = string.Empty;
                public string Rason { get; set; } = string.Empty;
                public string Device { get; set; } = string.Empty;
                public decimal Height { get; set; }
                public decimal Weight { get; set; }
                public string TargetSleepTime { get; set; } = string.Empty;
                public string LastSupper { get; set; } = string.Empty;
                public string Patrology { get; set; } = string.Empty;
                public List<string> ServiceModuleIds { get; set; } = [];
                public string Branch { get; set; } = string.Empty;
                public string Department { get; set; } = string.Empty;
                public string Registration { get; set; } = string.Empty;
                public string CNO { get; set; } = string.Empty;
                public string Function { get; set; } = string.Empty;
                public string Type { get; set; } = string.Empty;
                public string Cat { get; set; } = string.Empty;
                public string CatNumber { get; set; } = string.Empty;
                public string CatCID { get; set; } = string.Empty;
                public DateTime CatDate { get; set; }
                public string BondId { get; set; } = string.Empty;
                public string HolderCpf { get; set; } = string.Empty;
        }
}