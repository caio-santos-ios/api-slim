namespace api_slim.src.Shared.DTOs
{
    public class ForgotPasswordDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
    }
}