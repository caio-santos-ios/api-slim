using Swashbuckle.AspNetCore.Annotations;

namespace api_slim.src.Shared.DTOs
{
public class CreateAttachmentDTO : Request
{
    public string Type { get; set; } = string.Empty;

    [SwaggerIgnore]
    public IFormFile? File { get; set; }
    public string ParentId { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
}