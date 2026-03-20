namespace api_slim.src.Shared.DTOs
{
    public class UpdateUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public List<Module> Modules {get;set;} = [];
        public DateTime? EffectiveDate { get; set; }
        public string PermissionProfile {get;set;} = string.Empty;
    }
}