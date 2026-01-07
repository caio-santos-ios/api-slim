namespace api_slim.src.Shared.DTOs
{
public class UpdateContactDTO : Request
{
public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        
        public string Whatsapp { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;
        
        public string ParentId { get; set; } = string.Empty;
        
        public string Parent { get; set; } = string.Empty;
}
}