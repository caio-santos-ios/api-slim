using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models;

public class NotificationJob
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("parentId")]
    public string ParentId { get; set; } = string.Empty;
    
    [BsonElement("parent")]
    public string Parent { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [BsonElement("beneficiaryName")]
    public string BeneficiaryName { get; set; } = string.Empty;
    
    [BsonElement("beneficiaryCPF")]
    public string BeneficiaryCPF { get; set; } = string.Empty;
    
    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("sendDate")]
    public DateTime SendDate { get; set; }
    
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("sent")]
    public bool Sent { get; set; } = false;
}