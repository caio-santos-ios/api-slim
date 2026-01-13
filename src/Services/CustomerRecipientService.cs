using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;
using System.Text.Json;

namespace api_slim.src.Services
{
    public class CustomerRecipientService(ICustomerRecipientRepository customerRepository, IAddressRepository addressRepository, IPlanRepository planRepository, IServiceModuleRepository serviceModuleRepository, IMapper _mapper, ILogRepository logRepository) : ICustomerRecipientService
{
    HttpClient client = new();
    private readonly string uri = Environment.GetEnvironmentVariable("URI_RAPIDOC") ?? "";
    private readonly string clientId = Environment.GetEnvironmentVariable("CLIENT_ID_RAPIDOC") ?? "";
    private readonly string token = Environment.GetEnvironmentVariable("TOKEN_RAPIDOC") ?? "";

    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<CustomerRecipient> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> customers = await customerRepository.GetAllAsync(pagination);
            int count = await customerRepository.GetCountDocumentsAsync(pagination);
            return new(customers.Data, count, pagination.PageNumber, pagination.PageSize);
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
            if(customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
            return new(customer.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetByRapidocIdAsync(string rapidocId)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customer = await customerRepository.GetByRapidocIdAsync(rapidocId);
            if(customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
            return new(customer.Data);
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
            PaginationUtil<CustomerRecipient> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> customerRecipient = await customerRepository.GetAllAsync(pagination);
            return new(customerRecipient.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<CustomerRecipient?>> CreateAsync(CreateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> existed = await customerRepository.GetByCPFAsync(request.Cpf, request.ContractorId);
            if(existed.Data is not null) return new(null, 400, "CPF já utilizado");

            CustomerRecipient customer = _mapper.Map<CustomerRecipient>(request);
            ResponseApi<long?> code = await customerRepository.GetNextCodeAsync();
            customer.Code = code.Data.ToString()!.PadLeft(6, '0');

            ResponseApi<CustomerRecipient?> response = await customerRepository.CreateAsync(customer);

            if(response.Data is null) return new(null, 400, "Falha ao criar Beneficiário.");

            var requestRapidoc = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

            requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
            requestRapidoc.Headers.Add("clientId", clientId);
            requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string typePlan = "G";
            bool psicologia = false;
            bool especialista = false;

            ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
            if(plan.Data is not null)
            {
                ResponseApi<List<ServiceModule>> serviceModule = await serviceModuleRepository.GetByPlanIdAsync(plan.Data.Id);
                if(serviceModule.Data is not null) 
                {
                    foreach (ServiceModule module in serviceModule.Data!)
                    {
                        if(module.Name.Equals("Bem + Cuidado"))
                        {
                            especialista = true;
                        }

                        if(module.Name.Equals("Bem + Papo"))
                        {
                            psicologia = true;
                        }
                    }
                }
            }

            if(psicologia || especialista) 
            {
                if(psicologia && especialista)
                {
                    typePlan = "GSP";
                }
                else 
                {
                    if(psicologia)
                    {
                        typePlan = "GP";
                    }
                    else 
                    {
                        typePlan = "GS";
                    }
                }
            }
            else
            {
                typePlan = "G";
            };

            var beneficiarios = new[]
            {
                new {
                    name = request.Name,
                    cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
                    birthday = request.DateOfBirth, 
                    email = request.Email,
                    zipCode = new string(request.Address.ZipCode.Where(char.IsDigit).ToArray()),
                    address = $"{request.Address.Street}, {request.Address.Number}",
                    city = request.Address.City,
                    state = "",
                    serviceType = typePlan
                }
            };

            string jsonPayload = JsonSerializer.Serialize(beneficiarios);

            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/vnd.rapidoc.tema-v2+json");

            content.Headers.ContentType!.CharSet = null; 

            requestRapidoc.Content = content;

            var responseRapidoc = await client.SendAsync(requestRapidoc);
            responseRapidoc.EnsureSuccessStatusCode();
            string jsonResponse = await responseRapidoc.Content.ReadAsStringAsync();
            dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            if(result is not null && result.success == "true") 
            {
                response.Data.RapidocId = result!.beneficiaries[0].uuid.ToString(); 
                await customerRepository.UpdateAsync(response.Data);
            }

            Address address = _mapper.Map<Address>(request.Address);
            address.Parent = "customer-recipient";
            address.ParentId = response.Data!.Id;
            ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
            if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");

            await logRepository.CreateAsync(new()
            {   
                Action = "Criação",
                Collection = "customer-recipient",
                Description = $"Criação Beneficiário {request.Name}",
                CreatedBy = request.CreatedBy,
                Parent = "customer",
                ParentId = response.Data.ContractorId                 
            });

            return new(response.Data, 201, "Beneficiário criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<CustomerRecipient?>> UpdateAsync(UpdateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            CustomerRecipient customer = _mapper.Map<CustomerRecipient>(request);
            customer.UpdatedAt = DateTime.UtcNow;
            customer.CreatedAt = customerResponse.Data.CreatedAt;
            customer.Code = customerResponse.Data.Code;
            customer.RapidocId = customerResponse.Data.RapidocId;

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customer);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            System.Console.WriteLine(response.Data!.RapidocId);
            var requestRapidoc = new HttpRequestMessage(HttpMethod.Put, $"{uri}/beneficiaries/{response.Data!.RapidocId}");

            requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
            requestRapidoc.Headers.Add("clientId", clientId);
            requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string typePlan = "G";
            bool psicologia = false;
            bool especialista = false;

            ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
            if(plan.Data is not null)
            {
                ResponseApi<List<ServiceModule>> serviceModule = await serviceModuleRepository.GetByPlanIdAsync(plan.Data.Id);
                if(serviceModule.Data is not null) 
                {
                    foreach (ServiceModule module in serviceModule.Data!)
                    {
                        if(module.Name.Equals("Bem + Cuidado"))
                        {
                            especialista = true;
                        }

                        if(module.Name.Equals("Bem + Papo"))
                        {
                            psicologia = true;
                        }
                    }
                }
            }

            if(psicologia || especialista) 
            {
                if(psicologia && especialista)
                {
                    typePlan = "GSP";
                }
                else 
                {
                    if(psicologia)
                    {
                        typePlan = "GP";
                    }
                    else 
                    {
                        typePlan = "GS";
                    }
                }
            }
            else
            {
                typePlan = "G";
            };

            var beneficiarios = new 
            {
                name = request.Name,
                cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
                birthday = request.DateOfBirth, 
                // email = request.Email,
                zipCode = new string(request.Address.ZipCode.Where(char.IsDigit).ToArray()),
                address = $"{request.Address.Street}, {request.Address.Number}",
                city = request.Address.City,
                state = "",
                paymentType = "S",
                serviceType = typePlan
            };

            string jsonPayload = JsonSerializer.Serialize(beneficiarios);

            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/vnd.rapidoc.tema-v2+json");

            content.Headers.ContentType!.CharSet = null; 

            requestRapidoc.Content = content;

            var responseRapidoc = await client.SendAsync(requestRapidoc);

            await responseRapidoc.Content.ReadAsStringAsync();
            
            if(!string.IsNullOrEmpty(request.Address.Id))
            {            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Address);
                address.Parent = "customer-recipient";
                address.ParentId = response.Data!.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
            };

            await logRepository.CreateAsync(new()
            {   
                Action = "Atualização",
                Collection = "customer-recipient",
                Description = $"Atualizou Beneficiário {customerResponse.Data.Name}",
                CreatedBy = request.UpdatedBy,
                Parent = "customer",
                ParentId = response.Data.ContractorId                 
            });
            
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> UpdateStatusAsync(UpdateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            customerResponse.Data.UpdatedAt = DateTime.UtcNow;
            customerResponse.Data.Justification = customerResponse.Data.Active ? request.Justification : "";
            customerResponse.Data.Rason = customerResponse.Data.Active ? request.Rason : "";
            customerResponse.Data.Active = !customerResponse.Data.Active;

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

            await logRepository.CreateAsync(new()
            {   
                Action = "Atualização",
                Collection = "customer-recipient",
                Description = customerResponse.Data.Active ? $"Ativou Beneficiário {customerResponse.Data.Name}" : $"Inativou Beneficiário {customerResponse.Data.Name}",
                CreatedBy = request.UpdatedBy,
                Parent = "customer",
                ParentId = response.Data.ContractorId                 
            });
            
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<CustomerRecipient>> DeleteAsync(string id, string userId)
    {
        try
        {
            ResponseApi<CustomerRecipient> customer = await customerRepository.DeleteAsync(id);
            if(!customer.IsSuccess) return new(null, 400, customer.Message);

            var requestHeader = new HttpRequestMessage(HttpMethod.Delete, $"{uri}/beneficiaries/{customer.Data!.RapidocId}");
            requestHeader.Headers.Add("Authorization", $"Bearer {token}");
            requestHeader.Headers.Add("clientId", $"{clientId}");
            var response = await client.SendAsync(requestHeader);

            await logRepository.CreateAsync(new()
            {   
                Action = "Exclusão",
                Collection = "customer-recipient",
                Description = $"Exclusão Beneficiário {customer.Data.Name}",
                CreatedBy = userId,
                Parent = "customer",
                ParentId = customer.Data.ContractorId                 
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