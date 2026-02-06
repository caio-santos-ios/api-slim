using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class Vital : ModelBase 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("beneficiaryId")]
        public string BeneficiaryId { get; set; } = string.Empty;

        // SONO
        
        [BsonElement("sleepHours")]
        public int SleepHours { get; set; }
        
        [BsonElement("sleepTime")]
        public string SleepTime { get; set; } = string.Empty;
        
        [BsonElement("sleepQuality")]
        public int SleepQuality { get; set; }
        
        [BsonElement("sleepFragmentation")]
        public string SleepFragmentation { get; set; } = string.Empty;
        
        [BsonElement("sleepCell")]
        public string SleepCell { get; set; } = string.Empty;

        // NUTRIÇÃO
        [BsonElement("waterAmount")]
        public decimal WaterAmount { get; set; }

        [BsonElement("lastMeal")]
        public string LastMeal { get; set; } = string.Empty;
        
        [BsonElement("glycemicLoad")]
        public string GlycemicLoad { get; set; } = string.Empty;
        
        [BsonElement("snackHours")]
        public string SnackHours { get; set; } = string.Empty;

        // MENTAL
        // [BsonElement("mood")]
        // public string Mood { get; set; } = string.Empty;

        // [BsonElement("stress")]
        // public decimal Stress { get; set; }

        // [BsonElement("decompression")]
        // public string Decompression { get; set; } = string.Empty;
        [BsonElement("dass1")] 
        public int Dass1 { get; set; } // Perspectiva
        
        [BsonElement("dass2")] 
        public int Dass2 { get; set; } // Positividade
        
        [BsonElement("dass3")] 
        public int Dass3 { get; set; } // Valor próprio
        
        [BsonElement("dass4")] 
        public int Dass4 { get; set; } // Boca seca
        
        [BsonElement("dass5")] 
        public int Dass5 { get; set; } // Respiração
        
        [BsonElement("dass6")] 
        public int Dass6 { get; set; } // Tremores
        
        [BsonElement("dass7")] 
        public int Dass7 { get; set; } // Relaxar
        
        [BsonElement("dass8")] 
        public int Dass8 { get; set; } // Irritabilidade
        
        [BsonElement("dass9")] 
        public int Dass9 { get; set; } // Nervosismo
        
        // METRICAS
        [BsonElement("metrics")]
        public VitalMetric Metric { get; set; } = new();
        
        [BsonElement("weekMetrics")]
        public List<VitalMetric> WeekMetric { get; set; } = new();
    }

    public class VitalMetric 
    {
        public double IGS {get;set;}
        public double IGN {get;set;}
        public double IES {get;set;}
        public double IPV {get;set;}
        public string Day {get; set;} = string.Empty;
    }
}