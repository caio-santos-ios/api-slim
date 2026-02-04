using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
        public class UpdateDassCustomerRecipientDTO : Request
        {
                public string Id { get; set; } = string.Empty;
                public decimal Anxiety { get; set; }
                public decimal Depression { get; set; }
                public decimal Stress { get; set; }
                public decimal Total { get; set; }
        }
}