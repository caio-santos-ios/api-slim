namespace api_slim.src.Shared.DTOs
{
    public class CreateAppointmentTelemedicineDTO : Request
    {        
        public string BeneficiaryCPF { get; set; } = string.Empty;
        public string BeneficiaryId { get; set; } = string.Empty;
        public string SpecialtyUuid { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string ProfessionalUuid { get; set; } = string.Empty;
        public string ProfessionalName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Hour { get; set; } = string.Empty;
    }
}