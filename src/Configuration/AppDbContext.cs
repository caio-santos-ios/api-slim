using api_slim.src.Models;
using MongoDB.Driver;

namespace api_slim.src.Configuration
{
    public class AppDbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase Database { get; }

        public AppDbContext()
        {
            try
            {
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }
                
                var mongoClient = new MongoClient(mongoClientSettings);
                Database = mongoClient.GetDatabase(DatabaseName);
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to connect to database. Error: {ex.Message}");
            }
        }

        #region MASTER DATA
        public IMongoCollection<User> Users
        {
            get { return Database.GetCollection<User>("users"); }
        }
        public IMongoCollection<GenericTable> GenericTables
        {
            get { return Database.GetCollection<GenericTable>("generic_tables"); }
        }
        public IMongoCollection<Plan> Plans
        {
            get { return Database.GetCollection<Plan>("plans"); }
        }
        public IMongoCollection<Procedure> Procedures
        {
            get { return Database.GetCollection<Procedure>("procedures"); }
        }
        public IMongoCollection<Billing> Billings
        {
            get { return Database.GetCollection<Billing>("billings"); }
        }
        public IMongoCollection<Seller> Sellers
        {
            get { return Database.GetCollection<Seller>("sellers"); }
        }
        public IMongoCollection<Commission> Commissions
        {
            get { return Database.GetCollection<Commission>("commissions"); }
        }
        public IMongoCollection<AccreditedNetwork> AccreditedNetworks
        {
            get { return Database.GetCollection<AccreditedNetwork>("accredited_networks"); }
        }
        public IMongoCollection<Address> Addresses
        {
            get { return Database.GetCollection<Address>("addresses"); }
        }
        public IMongoCollection<Contact> Contacts
        {
            get { return Database.GetCollection<Contact>("contacts"); }
        }
        public IMongoCollection<SellerRepresentative> SellerRepresentatives
        {
            get { return Database.GetCollection<SellerRepresentative>("seller_representatives"); }
        }
        public IMongoCollection<ServiceModule> ServiceModules
        {
            get { return Database.GetCollection<ServiceModule>("service_modules"); }
        }
        public IMongoCollection<Professional> Professionals
        {
            get { return Database.GetCollection<Professional>("professionals"); }
        }
        public IMongoCollection<Attachment> Attachments
        {
            get { return Database.GetCollection<Attachment>("attachments"); }
        }
        public IMongoCollection<Customer> Customers
        {
            get { return Database.GetCollection<Customer>("customers"); }
        }
        public IMongoCollection<CustomerRecipient> CustomerRecipients
        {
            get { return Database.GetCollection<CustomerRecipient>("customer_recipients"); }
        }
        public IMongoCollection<CustomerContract> CustomerContracts
        {
            get { return Database.GetCollection<CustomerContract>("customer_contracts"); }
        }
        public IMongoCollection<Supplier> Suppliers
        {
            get { return Database.GetCollection<Supplier>("suppliers"); }
        }
        public IMongoCollection<TradingTable> TradingTables
        {
            get { return Database.GetCollection<TradingTable>("trading_tables"); }
        }
        
        public IMongoCollection<TelemedicineHistoric> TelemedicineHistorics
        {
            get { return Database.GetCollection<TelemedicineHistoric>("telemedicine_historics"); }
        }

        public IMongoCollection<Vital> Vitals
        {
            get { return Database.GetCollection<Vital>("vitals"); }
        }
        #endregion

        #region FINANCIAL
        public IMongoCollection<AccountsReceivable> AccountsReceivables
        {
            get { return Database.GetCollection<AccountsReceivable>("accounts_receivables"); }
        }
        public IMongoCollection<AccountsPayable> AccountsPayables
        {
            get { return Database.GetCollection<AccountsPayable>("accounts_payables"); }
        }
        #endregion

        #region SERVICE
        public IMongoCollection<InPerson> InPersons
        {
            get { return Database.GetCollection<InPerson>("in_person"); }
        }
        public IMongoCollection<Historic> Historics
        {
            get { return Database.GetCollection<Historic>("historics"); }
        }
        public IMongoCollection<AppointmentTelemedicine> AppointmentTelemedicines
        {
            get { return Database.GetCollection<AppointmentTelemedicine>("appointment_telemedicines"); }
        }
        #endregion
        
        #region CONFIGURATION
        public IMongoCollection<Log> Logs
        {
            get { return Database.GetCollection<Log>("logs"); }
        }
        public IMongoCollection<MetricApp> MetricApps
        {
            get { return Database.GetCollection<MetricApp>("metric_apps"); }
        }
        #endregion
        #region NOTIFICATIONS
        public IMongoCollection<Notification> Notifications
        {
            get { return Database.GetCollection<Notification>("notifications"); }
        }
        #endregion
        #region B2B PANEL
        public IMongoCollection<B2BMassMovement> B2BMassMovements
        {
            get { return Database.GetCollection<B2BMassMovement>("b2b_mass_movements"); }
        }
        public IMongoCollection<B2BInvoice> B2BInvoices
        {
            get { return Database.GetCollection<B2BInvoice>("b2b_invoices"); }
        }
        public IMongoCollection<B2BAttachment> B2BAttachments
        {
            get { return Database.GetCollection<B2BAttachment>("b2b_attachments"); }
        }
        #endregion

        #region OCCUPATIONAL MANAGEMENT
        public IMongoCollection<OccupationalMicroCheckin> OccupationalMicroCheckins
        {
            get { return Database.GetCollection<OccupationalMicroCheckin>("occupational_micro_checkins"); }
        }
        public IMongoCollection<OccupationalBemVital> OccupationalBemVitals
        {
            get { return Database.GetCollection<OccupationalBemVital>("occupational_bem_vitals"); }
        }
        public IMongoCollection<OccupationalPgr> OccupationalPgrs
        {
            get { return Database.GetCollection<OccupationalPgr>("occupational_pgrs"); }
        }
        #endregion
        #region SETTINGS
        public IMongoCollection<PermissionProfile> PermissionProfiles
        {
            get { return Database.GetCollection<PermissionProfile>("permission_profiles"); }
        }
        #endregion
    }
}