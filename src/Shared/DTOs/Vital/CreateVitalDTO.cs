namespace api_slim.src.Shared.DTOs
{
        public class CreateVitalDTO : Request
        {
                public string BeneficiaryId { get; set; } = string.Empty;

                // SONO
                public decimal SleepHours { get; set; }
                public string SleepTime { get; set; } = string.Empty;
                public int SleepQuality { get; set; }
                public string SleepFragmentation { get; set; } = string.Empty;
                public string SleepCell { get; set; } = string.Empty;

                // NUTRIÇÃO
                public decimal WaterAmount { get; set; }
                public string LastMeal { get; set; } = string.Empty;
                public string GlycemicLoad { get; set; } = string.Empty;
                public string SnackHours { get; set; } = string.Empty;


                // MENTAL
                // public string Mood { get; set; } = string.Empty;
                // public decimal Stress { get; set; }
                // public string Decompression { get; set; } = string.Empty;

                public int Dass1 { get; set; }
                public int Dass2 { get; set; }
                public int Dass3 { get; set; }
                public int Dass4 { get; set; }
                public int Dass5 { get; set; }
                public int Dass6 { get; set; }
                public int Dass7 { get; set; }
                public int Dass8 { get; set; }
                public int Dass9 { get; set; }
        }
}