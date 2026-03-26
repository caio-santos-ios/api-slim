using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Templates;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class CustomerService(ICustomerRepository customerRepository, IAddressRepository addressRepository, ICustomerRecipientService customerRecipientService, IMapper _mapper, ILogRepository logRepository, MailHandler mailHandler) : ICustomerService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Customer> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> customers = await customerRepository.GetAllAsync(pagination);
            int count = await customerRepository.GetCountDocumentsAsync(pagination);
            return new(customers.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Customer> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> customers = await customerRepository.GetSelectAsync(pagination);
            return new(customers.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> customer = await customerRepository.GetByIdAggregateAsync(id);
            if(customer.Data is null) return new(null, 404, "Cliente não encontrado");
            return new(customer.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request)
    {
        try
        {
            Customer customer = _mapper.Map<Customer>(request);
            
            ResponseApi<Customer?> response = await customerRepository.CreateAsync(customer);

            if(response.Data is null) return new(null, 400, "Falha ao criar Cliente.");

            Address address = _mapper.Map<Address>(request.Address);
            address.Parent = "customer-contract";
            address.ParentId = response.Data!.Id;
            ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
            if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            if(request.Type == "B2C")
            {
                switch(request.TypePlan)
                {
                    case "Individual":
                    case "Familiar":
                        Address recipientAddress = request.Address;
                        recipientAddress.Id = "";
                        ResponseApi<CustomerRecipient?> res = await customerRecipientService.CreateAsync(new ()
                        {
                            Address = recipientAddress,
                            DocumentContract = request.Document,
                            Name = request.CorporateName,
                            Cpf = request.Document,
                            Rg = request.Rg,
                            DateOfBirth = request.DateOfBirth,
                            Gender = request.Gender,
                            Phone = request.Phone,
                            Whatsapp = request.Whatsapp,
                            Email = request.Email,
                            Notes = request.Notes,
                            PlanId = "",
                            ContractorId = response.Data.Id
                        });
                        break;

                    case "Concessão":
                    case "Concessão - Familia":
                        break;
                }
            }

            await logRepository.CreateAsync(new()
            {   
                Action = "Criação",
                Collection = "customer",
                Description = $"Criação Cliente {response.Data.CorporateName}",
                CreatedBy = request.CreatedBy,
                Parent = "customer",
                ParentId = response.Data.Id                 
            });

            return new(response.Data, 201, "Cliente criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request)
    {
        try
        {
            ResponseApi<Customer?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Customer customer = _mapper.Map<Customer>(request);
            customer.UpdatedAt = DateTime.UtcNow;
            customer.CreatedAt = customerResponse.Data.CreatedAt;
            customer.Password = customerResponse.Data.Password;

            ResponseApi<Customer?> response = await customerRepository.UpdateAsync(customer);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

            if(!string.IsNullOrEmpty(request.Address.Id))
            {
                ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(request.Address.ParentId, request.Address.Parent);
                if(!findAddress.IsSuccess || findAddress.Data is null) return new(null, 400, "Falha ao atualizar.");
                
                request.Address.Id = findAddress.Data.Id;
            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Address);
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            };
            
            if(!string.IsNullOrEmpty(request.Responsible.Address.Id))
            {
                ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(request.Responsible.Address.ParentId, request.Responsible.Address.Parent);
                if(!findAddress.IsSuccess || findAddress.Data is null) return new(null, 400, "Falha ao atualizar.");
                
                request.Responsible.Address.Id = findAddress.Data.Id;
            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Responsible.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Responsible.Address);
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            };

            await logRepository.CreateAsync(new()
            {   
                Action = "Atualização",
                Collection = "customer",
                Description = $"Atualização Cliente {response.Data.CorporateName}",
                CreatedBy = request.CreatedBy,
                Parent = "customer",
                ParentId = response.Data.Id                 
            });
            
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Customer?>> UpdatePainelAccessAsync(UpdateCustomerDTO request)
    {
        try
        {
            ResponseApi<Customer?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            {
                dynamic access = Util.GenerateCodeAccess();
                customerResponse.Data.Password = BCrypt.Net.BCrypt.HashPassword(access.CodeAccess);
                
                var response = await customerRepository.UpdateAsync(customerResponse.Data);

                if(!response.IsSuccess) return new(null, 400, "Falha ao enviar acesso");
                
                string caminhoDoLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logo", "logo.png");
                string htmlEmail = MailTemplate.FirstAccessPainel(customerResponse.Data.CorporateName, customerResponse.Data.Email, access.CodeAccess, "pasbem.com.br/erp");
                await mailHandler.SendMailAsync(customerResponse.Data.Email, "Acesso ao Painel Gestor", htmlEmail);
            }
            
            return new(null, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Customer>> DeleteAsync(string id, string userId)
    {
        try
        {
            ResponseApi<Customer> customer = await customerRepository.DeleteAsync(id);
            if(!customer.IsSuccess || customer.Data is null) return new(null, 400, customer.Message);

            await logRepository.CreateAsync(new()
            {   
                Action = "Exclusão",
                Collection = "customer",
                Description = $"Exclusão Cliente {customer.Data.CorporateName}",
                CreatedBy = userId,
                Parent = "customer",
                ParentId = customer.Data.Id                 
            });

            return new(null, 204, "Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}