using System.Text;
using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Repository;
using api_slim.src.Services;
using api_slim.src.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace api_slim.src.Configuration
{
    public static class Build
    {
        public static void AddBuilderConfiguration(this WebApplicationBuilder builder)
        {
            AppDbContext.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? ""; 
            AppDbContext.DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? ""; 
            bool IsSSL;
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_SSL")))
            {
                IsSSL = Convert.ToBoolean(Environment.GetEnvironmentVariable("IS_SSL"));
            }
            else
            {
                IsSSL = false;
            }

            AppDbContext.IsSSL = IsSSL;
        }

        public static void AddBuilderAuthentication(this WebApplicationBuilder builder)
        {
            string? SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
            string? Issuer = Environment.GetEnvironmentVariable("ISSUER") ?? "";
            string? Audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? "";
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(SecretKey)
                    )
                };
            });
        }

        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AppDbContext>();
        }

        public static void AddBuilderServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAuthService, AuthService>();                  
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();    
            
            // FINANCIAL
            builder.Services.AddTransient<IAccountsReceivableService, AccountsReceivableService>();
            builder.Services.AddTransient<IAccountsReceivableRepository, AccountsReceivableRepository>();    
            builder.Services.AddTransient<IAccountsPayableService, AccountsPayableService>();
            builder.Services.AddTransient<IAccountsPayableRepository, AccountsPayableRepository>();    
            
            // MASTER DATA
            builder.Services.AddTransient<IGenericTableService, GenericTableService>();
            builder.Services.AddTransient<IGenericTableRepository, GenericTableRepository>();                       
            builder.Services.AddTransient<IAccreditedNetworkService, AccreditedNetworkService>();
            builder.Services.AddTransient<IAccreditedNetworkRepository, AccreditedNetworkRepository>();                        
            builder.Services.AddTransient<IAddressService, AddressService>();
            builder.Services.AddTransient<IAddressRepository, AddressRepository>();                        
            builder.Services.AddTransient<IContactService, ContactService>();
            builder.Services.AddTransient<IContactRepository, ContactRepository>();                        
            builder.Services.AddTransient<ISellerRepresentativeService, SellerRepresentativeService>();
            builder.Services.AddTransient<ISellerRepresentativeRepository, SellerRepresentativeRepository>();      
            builder.Services.AddTransient<IPlanService, PlanService>();
            builder.Services.AddTransient<IPlanRepository, PlanRepository>();
            builder.Services.AddTransient<IProcedureService, ProcedureService>();
            builder.Services.AddTransient<IProcedureRepository, ProcedureRepository>();
            builder.Services.AddTransient<IBillingService, BillingService>();
            builder.Services.AddTransient<IBillingRepository, BillingRepository>();
            builder.Services.AddTransient<ISellerService, SellerService>();
            builder.Services.AddTransient<ISellerRepository, SellerRepository>();
            builder.Services.AddTransient<ICommissionService, CommissionService>();
            builder.Services.AddTransient<ICommissionRepository, CommissionRepository>();
            builder.Services.AddTransient<IServiceModuleService, ServiceModuleService>();
            builder.Services.AddTransient<IServiceModuleRepository, ServiceModuleRepository>();                  
            builder.Services.AddTransient<IProfessionalService, ProfessionalService>();
            builder.Services.AddTransient<IProfessionalRepository, ProfessionalRepository>();                  
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();
            builder.Services.AddTransient<IAttachmentRepository, AttachmentRepository>();                  
            builder.Services.AddTransient<ICustomerService, CustomerService>();
            builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();                  
            builder.Services.AddTransient<ICustomerRecipientService, CustomerRecipientService>();
            builder.Services.AddTransient<ICustomerRecipientRepository, CustomerRecipientRepository>();                  
            builder.Services.AddTransient<ICustomerContractService, CustomerContractService>();
            builder.Services.AddTransient<ICustomerContractRepository, CustomerContractRepository>();                  
            builder.Services.AddTransient<ISupplierService, SupplierService>();
            builder.Services.AddTransient<ISupplierRepository, SupplierRepository>();
            builder.Services.AddTransient<ITradingTableService, TradingTableService>();
            builder.Services.AddTransient<ITradingTableRepository, TradingTableRepository>();
            builder.Services.AddTransient<IVitalService, VitalService>();
            builder.Services.AddTransient<IVitalRepository, VitalRepository>();

            // SERVICE
            builder.Services.AddTransient<IInPersonService, InPersonService>();
            builder.Services.AddTransient<IInPersonRepository, InPersonRepository>();
            builder.Services.AddTransient<IHistoricService, HistoricService>();
            builder.Services.AddTransient<IHistoricRepository, HistoricRepository>();
            builder.Services.AddTransient<ITelemedicineService, TelemedicineService>();
            builder.Services.AddTransient<IForwardingService, ForwardingService>();
            builder.Services.AddTransient<IAppointmentService, AppointmentService>();
            builder.Services.AddTransient<ITelemedicineHistoricService, TelemedicineHistoricService>();
            builder.Services.AddTransient<ITelemedicineHistoricRepository, TelemedicineHistoricRepository>();

            // SMCLICK
            builder.Services.AddTransient<IAppointmentNotificationService, AppointmentNotificationService>();
            builder.Services.AddTransient<ISmClickService, SmClickService>();
            builder.Services.AddTransient<SmClickHandler>();

            // DASHBOARD
            builder.Services.AddTransient<IDashboardService, DashboardService>();
            builder.Services.AddTransient<IDashboardRepository, DashboardRepository>();

            // CONFIGURATION
            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddTransient<ILogRepository, LogRepository>();

            // HANDLERS
            builder.Services.AddTransient<SmsHandler>();
            builder.Services.AddTransient<MailHandler>();
            builder.Services.AddTransient<CloudinaryHandler>();
            builder.Services.AddSingleton<WebPushHandler>();

            // WORKERS
            builder.Services.AddHostedService<NotificationWorker>();
            builder.Services.AddHostedService<BirthdayNotificationWorker>();
            builder.Services.AddHostedService<WebPushWorker>();
            
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}