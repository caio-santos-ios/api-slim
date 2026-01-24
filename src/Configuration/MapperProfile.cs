using AutoMapper;
using api_slim.src.Models;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region MASTER DATA
            CreateMap<CreateGenericTableDTO, GenericTable>().ReverseMap();
            CreateMap<UpdateGenericTableDTO, GenericTable>().ReverseMap();
            
            CreateMap<CreateAccreditedNetworkDTO, AccreditedNetwork>().ReverseMap();
            CreateMap<UpdateAccreditedNetworkDTO, AccreditedNetwork>().ReverseMap();

            CreateMap<CreateAccreditedNetworkResponsibleDTO, AccreditedNetworkResponsible>().ReverseMap();
            CreateMap<CreateAccreditedNetworkResponsibleDTO, AccreditedNetworkResponsible>().ReverseMap();
            
            CreateMap<CreateAddressDTO, Address>().ReverseMap();
            CreateMap<UpdateAddressDTO, Address>().ReverseMap();
            
            CreateMap<CreateContactDTO, Contact>().ReverseMap();
            CreateMap<UpdateContactDTO, Contact>().ReverseMap();

            CreateMap<CreateSellerRepresentativeDTO, SellerRepresentative>().ReverseMap();
            CreateMap<UpdateSellerRepresentativeDTO, SellerRepresentative>().ReverseMap();      

            CreateMap<CreatePlanDTO, Plan>().ReverseMap();
            CreateMap<UpdatePlanDTO, Plan>().ReverseMap();

            CreateMap<CreateProcedureDTO, Procedure>().ReverseMap();
            CreateMap<UpdateProcedureDTO, Procedure>().ReverseMap();
            
            CreateMap<CreateBillingDTO, Billing>().ReverseMap();
            CreateMap<UpdateBillingDTO, Billing>().ReverseMap();
            CreateMap<CreateBillingItemDTO, BillingItem>().ReverseMap();
            CreateMap<UpdateBillingItemDTO, BillingItem>().ReverseMap();

            CreateMap<CreateSellerDTO, Seller>().ReverseMap();
            CreateMap<UpdateSellerDTO, Seller>().ReverseMap();

            CreateMap<CreateCommissionDTO, Commission>().ReverseMap(); 
            CreateMap<UpdateCommissionDTO, Commission>().ReverseMap(); 
            
            CreateMap<CreateServiceModuleDTO, ServiceModule>().ReverseMap();
            CreateMap<UpdateServiceModuleDTO, ServiceModule>().ReverseMap();      
            
            CreateMap<CreateProfessionalDTO, Professional>().ReverseMap();
            CreateMap<UpdateProfessionalDTO, Professional>().ReverseMap();      

            CreateMap<CreateAttachmentDTO, Attachment>().ReverseMap();
            CreateMap<UpdateAttachmentDTO, Attachment>().ReverseMap();      
            
            CreateMap<CreateCustomerDTO, Customer>().ReverseMap();
            CreateMap<UpdateCustomerDTO, Customer>().ReverseMap();      

            CreateMap<CreateContractorResponsibleDTO, ContractorResponsible>().ReverseMap();
            CreateMap<CreateContractorResponsibleDTO, ContractorResponsible>().ReverseMap();      
            
            CreateMap<CreateCustomerRecipientDTO, CustomerRecipient>().ReverseMap();
            CreateMap<UpdateCustomerRecipientDTO, CustomerRecipient>().ReverseMap();      

            CreateMap<CreateCustomerContractDTO, CustomerContract>().ReverseMap();
            CreateMap<UpdateCustomerContractDTO, CustomerContract>().ReverseMap();      
            
            CreateMap<CreateSupplierDTO, Supplier>().ReverseMap();
            CreateMap<UpdateSupplierDTO, Supplier>().ReverseMap();      

            CreateMap<CreateTradingTableDTO, TradingTable>().ReverseMap();
            CreateMap<UpdateTradingTableDTO, TradingTable>().ReverseMap();
            
            CreateMap<CreateTelemedicineHistoricDTO, TelemedicineHistoric>().ReverseMap();
            #endregion

            #region FINANCIAL
            CreateMap<CreateAccountsReceivableDTO, AccountsReceivable>().ReverseMap();
            CreateMap<UpdateAccountsReceivableDTO, AccountsReceivable>().ReverseMap();
            CreateMap<CreateAccountsPayableDTO, AccountsPayable>().ReverseMap();
            CreateMap<UpdateAccountsPayableDTO, AccountsPayable>().ReverseMap();
            #endregion

            #region SERVICE
            CreateMap<CreateInPersonDTO, InPerson>().ReverseMap();
            CreateMap<UpdateInPersonDTO, InPerson>().ReverseMap();
            CreateMap<CreateHistoricDTO, Historic>().ReverseMap();
            CreateMap<UpdateHistoricDTO, Historic>().ReverseMap();
            #endregion
        }
    }
}