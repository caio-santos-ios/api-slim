using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class MetricApp : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("action")]
        public string Action {get;set;} = string.Empty;
        
        [BsonElement("screen")]
        public string Screen {get;set;} = string.Empty;
        
        [BsonElement("function")]
        public string Function {get;set;} = string.Empty;

        [BsonElement("description")]
        public string Description {get;set;} = string.Empty;     

        [BsonElement("parentId")]
        public string ParentId {get;set;} = string.Empty;     
        
        [BsonElement("parent")]
        public string Parent {get;set;} = string.Empty;          
    }
}