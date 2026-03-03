using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Shared.DTOs
{
    public class PushSubscriptionRequest
    {
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        
        [BsonElement("endpoint")]
        public string Endpoint { get; set; } = string.Empty;

        [BsonElement("expirationTime")]
        public long? ExpirationTime { get; set; }
        
        [BsonElement("keys")]
        public PushKeys Keys { get; set; } = new();
    }

    public class PushKeys
    {
        [BsonElement("p256dh")]
        public string P256dh { get; set; } = string.Empty;
        [BsonElement("auth")]
        public string Auth { get; set; } = string.Empty;
    }
}