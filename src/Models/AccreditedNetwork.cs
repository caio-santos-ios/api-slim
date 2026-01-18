using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class AccreditedNetwork : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;
        
        [BsonElement("cnpj")]
        public string CNPJ { get; set; } = string.Empty;
        
        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;
        
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
       
        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
       
        [BsonElement("serviceModuleId")]
        public string ServiceModuleId { get; set; } = string.Empty;
        
        [BsonElement("billingId")]
        public string BillingId { get; set; } = string.Empty;
        
        [BsonElement("effectiveDate")]
        public DateTime EffectiveDate { get; set; }
        
        [BsonElement("consumptionLimit")]
        public decimal ConsumptionLimit { get; set; }
        
        [BsonElement("tradingTable")]
        public string TradingTable { get; set; } = string.Empty;
        
        [BsonElement("responsible")]
        public AccreditedNetworkResponsible Responsible { get; set; } = new();
        
        [BsonElement("justification")]
        public string Justification { get; set; } = string.Empty;
    }
    
    public class AccreditedNetworkResponsible
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
}