namespace api_slim.src.Shared.DTOs
{
    public class CreateForwardingDTO : Request
    {
        public string AvailabilityUuid {get;set;} = string.Empty;
        public string BeneficiaryUuid {get;set;} = string.Empty;
        public string SpecialtyUuid {get;set;} = string.Empty;
        public string BeneficiaryName {get;set;} = string.Empty;
        public string SpecialtyName {get;set;} = string.Empty;
        public string Date {get;set;} = string.Empty;
        public string Time {get;set;} = string.Empty;
        public string ParentId {get;set;} = string.Empty;
        public string BeneficiaryCPF {get;set;} = string.Empty;
        public bool ApproveAdditionalPayment {get;set;}
    }
}