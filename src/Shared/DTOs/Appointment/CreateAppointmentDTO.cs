namespace api_slim.src.Shared.DTOs
{
    public class CreateAppointmentDTO : Request
    {
        public string AvailabilityUuid {get;set;} = string.Empty;
        public string BeneficiaryUuid {get;set;} = string.Empty;
        public string SpecialtyUuid {get;set;} = string.Empty;
        public bool ApproveAdditionalPayment {get;set;}
        public string BeneficiaryName {get;set;} = string.Empty;
        public string SpecialtyName {get;set;} = string.Empty;
        public string Date {get;set;} = string.Empty;
        public string Time {get;set;} = string.Empty;
        public string ProfessionalName {get;set;} = string.Empty;
        public string Origin {get;set;} = string.Empty;
    }
}