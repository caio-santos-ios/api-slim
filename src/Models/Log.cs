using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class Log : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("collection")]
        public string Collection {get;set;} = string.Empty;
       
        [BsonElement("action")]
        public string Action {get;set;} = string.Empty;   

        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;     

        [BsonElement("parentId")]
        public string ParentId {get;set;} = string.Empty;     
        
        [BsonElement("parent")]
        public string Parent {get;set;} = string.Empty;      
    }
}