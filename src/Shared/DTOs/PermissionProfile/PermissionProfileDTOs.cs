using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
    public class CreatePermissionProfileDTO : Request
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<api_slim.src.Models.Module> Modules { get; set; } = [];
    }

    public class UpdatePermissionProfileDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<api_slim.src.Models.Module> Modules { get; set; } = [];
    }

    /// <summary>Aplica os modules do perfil como base para um usuário.</summary>
    public class ApplyPermissionProfileDTO : Request
    {
        public string UserId { get; set; } = string.Empty;
        public string ProfileId { get; set; } = string.Empty;
    }
}
