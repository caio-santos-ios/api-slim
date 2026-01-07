using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{    
    public class CustomerRecipient : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("code")]
        public string Code { get; set; } = string.Empty; 

        [BsonElement("contractorId")]
        public string ContractorId { get; set; } = string.Empty; 

        [BsonElement("documentContract")]
        public string DocumentContract { get; set; } = string.Empty; // Mapeado do campo "CPF"

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty; // Mapeado do campo "Nome"

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty; // Mapeado do campo "CPF"

        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty; // Mapeado do campo "RG"

        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; } // Mapeado do campo "Data de Nascimento"

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty; // Mapeado do campo "Gênero"

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty; // Mapeado do campo "Telefone"

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty; // Mapeado do campo "Whatsapp"

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty; // Mapeado do campo "E-mail"

        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty; 

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
       
        [BsonElement("bond")]
        public string Bond { get; set; } = string.Empty;
        
        [BsonElement("subTotal")]
        public decimal SubTotal { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("discount")]
        public decimal Discount { get; set; }
        
        [BsonElement("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [BsonElement("rapidocId")]
        public string RapidocId { get; set; } = string.Empty;
        
        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [BsonElement("justification")]
        public string Justification { get; set; } = string.Empty;
    } 
}