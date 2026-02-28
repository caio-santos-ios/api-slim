using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class SellerRepresentative : ModelBase
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
        
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

        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [BsonElement("responsible")]
        public RepresentativeResponsible Responsible { get; set; } = new();

        [BsonElement("bank")]
        public RepresentativeBank Bank { get; set; } = new();
    }

    public class RepresentativeResponsible
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

    public class RepresentativeBank 
    {
        [BsonElement("bank")]
        public string Bank { get; set; } = string.Empty;

        [BsonElement("agency")]
        public string Agency { get; set; } = string.Empty;
        
        [BsonElement("account")]
        public string Account { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("pixKey")]
        public string PixKey { get; set; } = string.Empty;

        [BsonElement("pixType")]
        public string PixType { get; set; } = string.Empty;
    }
}