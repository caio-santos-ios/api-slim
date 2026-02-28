using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class Commission : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("ruleName")]
        public string RuleName { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        
        [BsonElement("escalation")]
        public string Escalation { get; set; } = string.Empty;
        
        [BsonElement("condition")]
        public string Condition { get; set; } = string.Empty;
        
        [BsonElement("recurrence")]
        public string Recurrence { get; set; } = string.Empty;
        
        [BsonElement("conditionValue")]
        public decimal ConditionValue { get; set; }

        [BsonElement("numberOfLives")]
        public int NumberOfLives { get; set; }

        [BsonElement("startPeriod")]
        public DateTime? StartPeriod { get; set; }
        
        [BsonElement("endPeriod")]
        public DateTime? EndPeriod { get; set; }

    }
}