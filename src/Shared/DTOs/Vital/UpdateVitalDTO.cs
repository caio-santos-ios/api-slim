namespace api_slim.src.Shared.DTOs
{
        public class UpdateVitalDTO : Request
        {
                public string Id { get; set; } = string.Empty;
                public string BeneficiaryId { get; set; } = string.Empty;
                public string Mood { get; set; } = string.Empty;
                public decimal SleepHours { get; set; }
                public string SleepTime { get; set; } = string.Empty;
                public decimal WaterAmount { get; set; }
                public bool ChekinIGN { get; set; }
                public bool ChekinIES { get; set; }
                public bool ChekinISO { get; set; }
                public int ChekinISOPoint { get; set; }
                public int ChekinIGSPoint { get; set; }
                public int ChekinIGNPoint { get; set; }
                public int ChekinIESPoint { get; set; }
                public int Level { get; set; }
                public string ChekinISOQuestion { get; set; } = string.Empty;
        }
}