using Swashbuckle.AspNetCore.Annotations;

namespace api_slim.src.Shared.DTOs
{
        public class ImportCustomerRecipientDTO : Request
        {
                public string ContractorId { get; set; } = string.Empty;
                
                [SwaggerIgnore]
                public IFormFile? File { get; set; }
        }
}