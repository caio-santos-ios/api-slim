using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    // ─── Micro Checkin ISO ────────────────────────────────────────────────────────
    public class OccupationalMicroCheckin : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("recipientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecipientId { get; set; } = string.Empty;

        [BsonElement("recipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [BsonElement("department")]
        public string Department { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("dimension")]
        public string Dimension { get; set; } = string.Empty;

        // Scores do simulador de diagnóstico ocupacional
        [BsonElement("engagementLevel")]
        public decimal EngagementLevel { get; set; }

        [BsonElement("riskClassification")]
        public string RiskClassification { get; set; } = string.Empty; // Baixo | Médio | Alto | Crítico

        [BsonElement("riskLevel")]
        public decimal RiskLevel { get; set; }

        [BsonElement("safetyPerception")]
        public decimal SafetyPerception { get; set; }

        [BsonElement("absenceRisk")]
        public decimal AbsenceRisk { get; set; }

        [BsonElement("econometerScore")]
        public decimal EconometerScore { get; set; }

        [BsonElement("checkinDate")]
        public DateTime CheckinDate { get; set; } = DateTime.Now;

        [BsonElement("period")]
        public string Period { get; set; } = string.Empty; // Manha | Tarde | Noite

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }

    // ─── Bem Vital Ocupacional ─────────────────────────────────────────────────
    public class OccupationalBemVital : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("recipientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecipientId { get; set; } = string.Empty;

        [BsonElement("recipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [BsonElement("department")]
        public string Department { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("referenceDate")]
        public DateTime ReferenceDate { get; set; } = DateTime.Now;

        // IGS — Índice Geral de Saúde
        [BsonElement("igs")]
        public decimal Igs { get; set; }

        // IGN — Índice Geral de Nutrição
        [BsonElement("ign")]
        public decimal Ign { get; set; }

        // IES — Índice de Engajamento Social
        [BsonElement("ies")]
        public decimal Ies { get; set; }

        // IPV — Índice de Performance de Vida
        [BsonElement("ipv")]
        public decimal Ipv { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }

    // ─── PGR — Programa de Gerenciamento de Riscos ────────────────────────────
    public class OccupationalPgr : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [BsonElement("referenceMonth")]
        public int ReferenceMonth { get; set; }

        [BsonElement("referenceYear")]
        public int ReferenceYear { get; set; }

        [BsonElement("generatedAt")]
        public DateTime? GeneratedAt { get; set; }

        [BsonElement("generatedBy")]
        public string GeneratedBy { get; set; } = string.Empty;

        [BsonElement("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        // Snapshot dos indicadores no momento da geração
        [BsonElement("totalBeneficiaries")]
        public int TotalBeneficiaries { get; set; }

        [BsonElement("avgEngagement")]
        public decimal AvgEngagement { get; set; }

        [BsonElement("avgRisk")]
        public decimal AvgRisk { get; set; }

        [BsonElement("avgSafetyPerception")]
        public decimal AvgSafetyPerception { get; set; }

        [BsonElement("avgAbsenceRisk")]
        public decimal AvgAbsenceRisk { get; set; }

        [BsonElement("avgEconometer")]
        public decimal AvgEconometer { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pendente"; // Pendente | Gerado | Enviado
    }
}
