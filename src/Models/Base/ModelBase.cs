using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models.Base
{
    public class ModelBase
    {
        [BsonElement("deleted")]
        public bool Deleted {get;set;} = false;
        
        [BsonElement("active")]
        public bool Active {get;set;} = true;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt {get;set;} = DateTime.UtcNow;
        
        [BsonElement("deletedAt")]
        public DateTime? DeletedAt {get;set;}
        
        [BsonElement("createdBy")]
        public string CreatedBy {get;set;} = string.Empty;

        [BsonElement("updatedBy")]
        public string UpdatedBy {get;set;} = string.Empty;
    }
}