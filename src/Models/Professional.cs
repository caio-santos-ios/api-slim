using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class Professional : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        
        [BsonElement("specialty")]
        public string Specialty { get; set; } = string.Empty;

        [BsonElement("registration")]
        public string Registration { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;
        
        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }
    }
}