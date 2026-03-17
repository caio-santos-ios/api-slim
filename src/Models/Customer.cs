using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class Customer : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty; // Mapeado do campo "Nome"
        
        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty; // Mapeado do campo "Nome"

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty; // Mapeado do campo "CPF"

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

        [BsonElement("segment")]
        public string Segment { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        [BsonElement("responsible")]
        public ContractorResponsible Responsible { get; set; } = new();

        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("minimumValue")]
        public decimal MinimumValue { get; set; }

        [BsonElement("typePlan")]
        public string TypePlan { get; set; } = string.Empty;

        [BsonElement("sellerId")]
        public string SellerId { get; set; } = string.Empty;
        
        [BsonElement("taxRegime")]
        public string TaxRegime { get; set; } = string.Empty;
        
        [BsonElement("technicalManager")]
        public string TechnicalManager { get; set; } = string.Empty;
        
        [BsonElement("ratRate")]
        public decimal RatRate { get; set; }
        
        [BsonElement("sheetValue")]
        public decimal SheetValue { get; set; }

        [BsonElement("cat")]
        public string Cat { get; set; } = string.Empty;
        
        [BsonElement("catNumber")]
        public string CatNumber { get; set; } = string.Empty;

        [BsonElement("catCID")]
        public string CatCID { get; set; } = string.Empty;
        
        [BsonElement("catDate")]
        public DateTime CatDate { get; set; }

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class Contractor
    {
        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty; // Mapeado do campo "Nome"
        
        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty; // Mapeado do campo "Nome"

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty; // Mapeado do campo "CPF"

        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty; // Mapeado do campo "RG"

        [BsonElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; } // Mapeado do campo "Data de Nascimento"

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty; // Mapeado do campo "Gênero"

        [BsonElement("address")]
        public Address Address { get; set; } = new Address();

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty; // Mapeado do campo "Telefone"

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty; // Mapeado do campo "Whatsapp"

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty; // Mapeado do campo "E-mail"

        [BsonElement("segments")]
        public string Segments { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        [BsonElement("responsible")]
        public ContractorResponsible Responsible { get; set; } = new();

        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty; 

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
       
        [BsonElement("minimumValue")]
        public decimal MinimumValue { get; set; }
    }

    public class ContractorResponsible
    {

        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
        
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
    }

    public class Recipient
    {
        [BsonElement("documentContract")]
        public string DocumentContract { get; set; } = string.Empty; // Mapeado do campo "CPF"

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty; // Mapeado do campo "Nome"

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty; // Mapeado do campo "CPF"

        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty; // Mapeado do campo "RG"

        [BsonElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; } // Mapeado do campo "Data de Nascimento"

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty; // Mapeado do campo "Gênero"

        [BsonElement("address")]
        public Address Address { get; set; } = new Address();

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty; // Mapeado do campo "Telefone"

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty; // Mapeado do campo "Whatsapp"

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty; // Mapeado do campo "E-mail"

        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty; // Mapeado do campo "Origem" (5)

        [BsonElement("seller")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SellerId { get; set; } = string.Empty; 

        [BsonElement("planType")]
        public string PlanType { get; set; } = string.Empty; 

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    } 
}