using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebPush;

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

        [BsonElement("holderId")]
        public string HolderId { get; set; } = string.Empty; 

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
        
        [BsonElement("rason")]
        public string Rason { get; set; } = string.Empty;
        
        [BsonElement("imported")]
        public bool Imported { get; set; } = false;
        
        [BsonElement("firstAccess")]
        public bool FirstAccess { get; set; } = true;
        
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("photo")]
        public string Photo { get; set; } = string.Empty;

        [BsonElement("codeAccess")]
        public string CodeAccess {get;set;} = string.Empty;

        [BsonElement("validatedAccess")]
        public bool ValidatedAccess {get;set;} = false;

        [BsonElement("codeAccessExpiration")]
        public DateTime? CodeAccessExpiration { get; set; }
        
        [BsonElement("height")]
        public decimal Height { get; set; }

        [BsonElement("weight")]
        public decimal Weight { get; set; }
        
        [BsonElement("targetSleepTime")]
        public string TargetSleepTime { get; set; } = string.Empty;
        
        [BsonElement("lastSupper")]
        public string LastSupper { get; set; } = string.Empty;

        [BsonElement("patrology")]
        public string Patrology { get; set; } = string.Empty;
        
        [BsonElement("dass")]
        public Dass Dass { get; set; } = new();

        [BsonElement("serviceModuleIds")]
        public List<string> ServiceModuleIds { get; set; } = [];

        [BsonElement("branch")]
        public string Branch { get; set; } = string.Empty;
        
        [BsonElement("department")]
        public string Department { get; set; } = string.Empty;
        
        [BsonElement("registration")]
        public string Registration { get; set; } = string.Empty;
        
        [BsonElement("cno")]
        public string CNO { get; set; } = string.Empty;
        
        [BsonElement("function")]
        public string Function { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("subNotification")]
        public PushSubscriptionRequest SubNotification { get; set; } = new();

        [BsonElement("IGSNotification")]
        public DateTime IGSNotification { get; set; }
        
        [BsonElement("IGNNotification")]
        public DateTime IGNNotification { get; set; }

        [BsonElement("IESNotification")]
        public DateTime IESNotification { get; set; }

        [BsonElement("bondId")]
        public string BondId { get; set; } = string.Empty;
        
        [BsonElement("holderCpf")]
        public string HolderCpf { get; set; } = string.Empty;
    }

    public class Dass 
    {
        [BsonElement("anxiety")]
        public decimal Anxiety { get; set; }

        [BsonElement("depression")]
        public decimal Depression { get; set; }
        
        [BsonElement("stress")]
        public decimal Stress { get; set; }
        [BsonElement("total")]
        public decimal Total { get; set; }
    }
}