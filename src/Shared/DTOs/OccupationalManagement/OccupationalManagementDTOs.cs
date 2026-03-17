namespace api_slim.src.Shared.DTOs
{
    // ─── Micro Checkin ───────────────────────────────────────────────────────────
    public class CreateOccupationalMicroCheckinDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Dimension { get; set; } = string.Empty;
        public decimal EngagementLevel { get; set; }
        public string RiskClassification { get; set; } = string.Empty;
        public decimal RiskLevel { get; set; }
        public decimal SafetyPerception { get; set; }
        public decimal AbsenceRisk { get; set; }
        public decimal EconometerScore { get; set; }
        public DateTime CheckinDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateOccupationalMicroCheckinDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Dimension { get; set; } = string.Empty;
        public decimal EngagementLevel { get; set; }
        public string RiskClassification { get; set; } = string.Empty;
        public decimal RiskLevel { get; set; }
        public decimal SafetyPerception { get; set; }
        public decimal AbsenceRisk { get; set; }
        public decimal EconometerScore { get; set; }
        public DateTime CheckinDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    // ─── Bem Vital ───────────────────────────────────────────────────────────────
    public class CreateOccupationalBemVitalDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ReferenceDate { get; set; }
        public decimal Igs { get; set; }
        public decimal Ign { get; set; }
        public decimal Ies { get; set; }
        public decimal Ipv { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateOccupationalBemVitalDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ReferenceDate { get; set; }
        public decimal Igs { get; set; }
        public decimal Ign { get; set; }
        public decimal Ies { get; set; }
        public decimal Ipv { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    // ─── PGR ─────────────────────────────────────────────────────────────────────
    public class GenerateOccupationalPgrDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public int ReferenceMonth { get; set; }
        public int ReferenceYear { get; set; }
    }
}
