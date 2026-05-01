using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class AppointmentTelemedicine : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("appointmentUuid")]
        public string AppointmentUuid { get; set; } = string.Empty;
        
        [BsonElement("beneficiaryUrl")]
        public string BeneficiaryUrl { get; set; } = string.Empty;

        [BsonElement("beneficiaryCPF")]
        public string BeneficiaryCPF { get; set; } = string.Empty;

        [BsonElement("beneficiaryId")]
        public string BeneficiaryId { get; set; } = string.Empty;

        [BsonElement("specialtyUuid")]
        public string SpecialtyUuid { get; set; } = string.Empty;

        [BsonElement("specialtyName")]
        public string SpecialtyName { get; set; } = string.Empty;

        [BsonElement("professionalName")]
        public string ProfessionalName { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("hour")]
        public string Hour { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;
    }
}