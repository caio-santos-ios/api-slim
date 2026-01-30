using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class TelemedicineHistoric : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("recipientId")]
        public string RecipientId {get;set;} = string.Empty;
        
        [BsonElement("recipientName")]
        public string RecipientName {get;set;} = string.Empty;
        
        [BsonElement("specialistId")]
        public string SpecialistId {get;set;} = string.Empty;

        [BsonElement("specialistName")]
        public string SpecialistName {get;set;} = string.Empty;
        
        [BsonElement("professionalId")]
        public string ProfessionalId {get;set;} = string.Empty;

        [BsonElement("professionalName")]
        public string ProfessionalName {get;set;} = string.Empty;

        [BsonElement("status")]
        public string Status {get;set;} = string.Empty;   

        [BsonElement("date")]
        public string Date {get;set;} = string.Empty;     

        [BsonElement("time")]
        public string Time {get;set;} = string.Empty;     

        [BsonElement("type")]
        public string Type {get;set;} = string.Empty;     
        
        [BsonElement("parentId")]
        public string ParentId {get;set;} = string.Empty;

        [BsonElement("parentUuid")]
        public string ParentUuid {get;set;} = string.Empty;
    }
}