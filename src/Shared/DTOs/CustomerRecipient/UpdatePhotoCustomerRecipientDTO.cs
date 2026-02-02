
namespace api_slim.src.Shared.DTOs
{
        public class UpdatePhotoCustomerRecipientDTO : Request
        {
                public string Id { get; set; } = string.Empty;
                public IFormFile? Photo { get; set; }
        }
}