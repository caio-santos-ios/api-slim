namespace api_slim.src.Shared.DTOs
{
    public class CreateUserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public List<Module> Modules {get;set;} = [];
        public DateTime? EffectiveDate { get; set; }
        public string PermissionProfile {get;set;} = string.Empty;
    }

    public class Module 
    {
        public string Code {get;set;} = string.Empty;
        
        public string Description {get;set;} = string.Empty;

        
        public List<Routine> Routines {get;set;} = [];
    }
    
    public class Routine 
    {
        public string Code {get;set;} = string.Empty;
        
        public string Description {get;set;} = string.Empty;

        public PermissionRoutine Permissions {get;set;} = new();
    }

    public class PermissionRoutine 
    {
        
        public bool Read {get;set;} = false;
        
        public bool Create {get;set;} = false;
        
        public bool Update {get;set;} = false;
        
        public bool Delete {get;set;} = false;
    }
}