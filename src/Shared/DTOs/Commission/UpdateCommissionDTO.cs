namespace api_slim.src.Shared.DTOs
{
        public class UpdateCommissionDTO
        {
                public string Id { get; set; } = string.Empty;
                public string RuleName { get; set; } = string.Empty;

                public string Description { get; set; } = string.Empty;
                
                public string Escalation { get; set; } = string.Empty;
                
                public string Condition { get; set; } = string.Empty;
                
                public string Recurrence { get; set; } = string.Empty;
                
                public decimal ConditionValue { get; set; }

                public int NumberOfLives { get; set; }

                public DateTime StartPeriod { get; set; }
                
                public DateTime EndPeriod { get; set; }                
        }
}