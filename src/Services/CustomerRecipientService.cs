using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Templates;
using api_slim.src.Shared.Utils;
using AutoMapper;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using ImageMagick;

namespace api_slim.src.Services
{
    public class CustomerRecipientService
    (
        ICustomerRecipientRepository customerRepository, 
        // ICustomerService customerService, 
        ICustomerRepository customerRepository1, 
        IAddressRepository addressRepository, 
        IPlanRepository planRepository, 
        IServiceModuleRepository serviceModuleRepository, 
        IMapper _mapper, 
        ILogRepository logRepository, 
        CloudinaryHandler cloudinaryHandler, 
        MailHandler mailHandler
    ) : ICustomerRecipientService
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
            string rapidocId = customer.Data.rapidocId;

            if(string.IsNullOrEmpty(customer.Data.rapidocId))
            {
                ResponseApi<dynamic?> res = await GetByCPFAggregateAsync(customer.Data.cpf);
                if(res.Data is not null)
                {
                    rapidocId = res.Data.RapidocId;
                }
            };

            var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries/{customer.Data.rapidocId}/appointments");
            requestHeader.Headers.Add("Authorization", $"Bearer {token}");
            requestHeader.Headers.Add("clientId", clientId);
            
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
            requestHeader.Content = content;
            var response = await client.SendAsync(requestHeader);
            
            string jsonResponse = await response.Content.ReadAsStringAsync();

            dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            DateTime? nextDate = null;
            dynamic telemedicine = new {};
            if(result is not null)
            {
                foreach (dynamic item in result)
                {
                    if(item.status != "SCHEDULED") continue;

                    DateTime date = DateTime.Parse(item.detail.date.ToString(), new CultureInfo("pt-BR"));

                    if(date.Date < DateTime.UtcNow.Date) continue;

                    if(nextDate is null)
                    {
                        nextDate = date.Date;

                        telemedicine = new 
                        { 
                            isToDay = nextDate == DateTime.UtcNow.Date,
                            date = nextDate, 
                            professional = item.professional.name.ToString(),
                            specialty = item.specialty.name.ToString(),
                            beneficiaryUrl = item.beneficiaryUrl.ToString(),
                            from = item.detail.from.ToString(),
                            to = item.detail.to.ToString(),
                        };
                    }
                    else 
                    {
                        if(nextDate > date.Date)
                        {
                            nextDate = date.Date;

                            telemedicine = new 
                            { 
                                isToDay = nextDate == DateTime.UtcNow.Date,
                                date = nextDate, 
                                professional = item.professional.name.ToString(),
                                specialty = item.specialty.name.ToString(),
                                beneficiaryUrl = item.beneficiaryUrl.ToString(),
                                from = item.detail.from.ToString(),
                                to = item.detail.to.ToString(),
                            };
                        }
                    }                    
                };
            };

            customer.Data.telemedicine = telemedicine;

            return new(customer.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetAtendimentoAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> customer = await customerRepository.GetByIdAggregateAsync(id);
            if(customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
            string rapidocId = customer.Data.rapidocId;

            if(string.IsNullOrEmpty(customer.Data.rapidocId))
            {
                ResponseApi<dynamic?> res = await GetByCPFAggregateAsync(customer.Data.cpf);
                if(res.Data is not null)
                {
                    rapidocId = res.Data.RapidocId;
                }
            };

            var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries/{customer.Data.rapidocId}/request-appointment");
            requestHeader.Headers.Add("Authorization", $"Bearer {token}");
            requestHeader.Headers.Add("clientId", clientId);
            
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
            requestHeader.Content = content;
            var response = await client.SendAsync(requestHeader);
            
            string jsonResponse = await response.Content.ReadAsStringAsync();

            dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            string link = "";
            if(result is not null)
            {
                link = result.url;
            }

            dynamic? obj = new {link};

            return new(obj);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetByCPFAggregateAsync(string cpf)
    {
        try
        {
            ResponseApi<dynamic?> customer = await customerRepository.GetByCPFAggregateAsync(cpf);

            if(customer.Data is not null)
            {
                string rapidocId = customer.Data.rapidocId;
                if(string.IsNullOrEmpty(rapidocId))
                {
                    var requestRapidoc = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries/{cpf.Replace(".", "").Replace("-", "")}");

                    requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
                    requestRapidoc.Headers.Add("clientId", clientId);

                    var content = new StringContent(string.Empty);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                    requestRapidoc.Content = content;
                    var response = await client.SendAsync(requestRapidoc);

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                    if(result is not null)
                    {
                        ResponseApi<CustomerRecipient?> res = await customerRepository.GetByIdAsync(customer.Data.id);
                        
                        if(result.success == "true")
                        {
                            if(res.Data is not null)
                            {
                                res.Data.RapidocId = result.beneficiary.uuid.ToString();
                                customer.Data.rapidocId = result.beneficiary.uuid.ToString();
                                await customerRepository.UpdateAsync(res.Data);
                            }
                        }
                        else
                        {
                            if(res.Data is not null)
                            {
                                var requestRapidocPost = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

                                requestRapidocPost.Headers.Add("Authorization", $"Bearer {token}");
                                requestRapidocPost.Headers.Add("clientId", clientId);
                                requestRapidocPost.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                string typePlan = "G";
                                bool psicologia = false;
                                bool especialista = false;

                                ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(res.Data.PlanId);
                                if(plan.Data is not null)
                                {
                                    foreach (string moduleId in plan.Data.ServiceModuleIds)
                                    {
                                        ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                                        if(serviceModule.Data is not null) 
                                        {
                                            if(serviceModule.Data.Name.Equals("Bem + Cuidado"))
                                            {
                                                especialista = true;
                                            }

                                            if(serviceModule.Data.Name.Equals("Bem + Papo"))
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

                                ResponseApi<Address> address = await addressRepository.GetByParentIdAsync(customer.Data.id, "customer-recipient");
                                var beneficiarios = new[]
                                {
                                    new {
                                        name = res.Data.Name,
                                        cpf = new string(res.Data.Cpf.Where(char.IsDigit).ToArray()),
                                        birthday = res.Data.DateOfBirth, 
                                        email = res.Data.Email,
                                        zipCode = new string(address.Data is null ? "" : address.Data.ZipCode.Where(char.IsDigit).ToArray()),
                                        address = address.Data is null ? "" : $"{address.Data.Street}, {address.Data.Number}",
                                        city = address.Data is null ? "" :  address.Data.City,
                                        state = "",
                                        serviceType = typePlan
                                    }
                                };

                                string jsonPayload = JsonSerializer.Serialize(beneficiarios);
                                var contentPost = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/vnd.rapidoc.tema-v2+json");
                                contentPost.Headers.ContentType!.CharSet = null; 
                                requestRapidocPost.Content = contentPost;
                                var responseRapidoc = await client.SendAsync(requestRapidocPost);
                                string jsonResponsePost = await responseRapidoc.Content.ReadAsStringAsync();
                                dynamic? resultPost = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponsePost);
                                if(resultPost is not null)
                                {
                                    if(resultPost.success == "true")
                                    {
                                        customer.Data.rapidocId = resultPost.beneficiaries[0].uuid.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

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
            ResponseApi<List<dynamic>> customerRecipient = await customerRepository.GetSelectAsync(pagination);

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
                foreach (string moduleId in plan.Data.ServiceModuleIds)
                {
                    ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                    if(serviceModule.Data is not null) 
                    {
                        if(serviceModule.Data.Name.Equals("Bem + Cuidado"))
                        {
                            especialista = true;
                        }

                        if(serviceModule.Data.Name.Equals("Bem + Papo"))
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

            string passowrd = request.Cpf.Replace(".", "").Replace("-", "");

            string caminhoDoLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logo", "logo.png");
            string htmlEmail = MailTemplate.GetPwaAccessTemplate(request.Name, request.Cpf, passowrd.Substring(0, 6), "pasbem.com.br/aplicativo", caminhoDoLogo);
            await mailHandler.SendMailAsync(request.Email, "Aplicativo Pasbem", htmlEmail);

            // string template = MailTemplate.GetPwaAccessTemplate(request.Name, request.Cpf, passowrd.Substring(0, 6), "pasbem.com.br/aplicativo");
            // await mailHandler.SendMailAsync(request.Email, "Aplicativo Pasbem", template);

            return new(response.Data, 201, "Beneficiário criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> EmailAsync(CreateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> existed = await customerRepository.GetByCPFAsync(request.Cpf, request.ContractorId);
            if(existed.Data is not null) return new(null, 400, "CPF já utilizado");
            string passowrd = request.Cpf.Replace(".", "").Replace("-", "");

            string caminhoDoLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logo", "logo.png");
            // string template = MailTemplate.GetPwaAccessTemplate(request.Name, request.Cpf, passowrd.Substring(0, 6), "pasbem.com.br/aplicativo");
            string htmlEmail = MailTemplate.GetPwaAccessTemplate("João", "joao@email.com", "123456", "https://pasbem.com.br", caminhoDoLogo);
            await mailHandler.SendMailAsync("caiodev.fullstack@gmail.com", "Aplicativo Pasbem", htmlEmail);

            // CustomerRecipient customer = _mapper.Map<CustomerRecipient>(request);
            // ResponseApi<long?> code = await customerRepository.GetNextCodeAsync();
            // customer.Code = code.Data.ToString()!.PadLeft(6, '0');

            // ResponseApi<CustomerRecipient?> response = await customerRepository.CreateAsync(customer);

            // if(response.Data is null) return new(null, 400, "Falha ao criar Beneficiário.");

            // var requestRapidoc = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

            // requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
            // requestRapidoc.Headers.Add("clientId", clientId);
            // requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // string typePlan = "G";
            // bool psicologia = false;
            // bool especialista = false;

            // ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
            // if(plan.Data is not null)
            // {
            //     foreach (string moduleId in plan.Data.ServiceModuleIds)
            //     {
            //         ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
            //         if(serviceModule.Data is not null) 
            //         {
            //             if(serviceModule.Data.Name.Equals("Bem + Cuidado"))
            //             {
            //                 especialista = true;
            //             }

            //             if(serviceModule.Data.Name.Equals("Bem + Papo"))
            //             {
            //                 psicologia = true;
            //             }
            //         }
            //     }
            // }

            // if(psicologia || especialista) 
            // {
            //     if(psicologia && especialista)
            //     {
            //         typePlan = "GSP";
            //     }
            //     else 
            //     {
            //         if(psicologia)
            //         {
            //             typePlan = "GP";
            //         }
            //         else 
            //         {
            //             typePlan = "GS";
            //         }
            //     }
            // }
            // else
            // {
            //     typePlan = "G";
            // };

            // var beneficiarios = new[]
            // {
            //     new {
            //         name = request.Name,
            //         cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
            //         birthday = request.DateOfBirth, 
            //         email = request.Email,
            //         zipCode = new string(request.Address.ZipCode.Where(char.IsDigit).ToArray()),
            //         address = $"{request.Address.Street}, {request.Address.Number}",
            //         city = request.Address.City,
            //         state = "",
            //         serviceType = typePlan
            //     }
            // };

            // string jsonPayload = JsonSerializer.Serialize(beneficiarios);

            // var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/vnd.rapidoc.tema-v2+json");

            // content.Headers.ContentType!.CharSet = null; 

            // requestRapidoc.Content = content;

            // var responseRapidoc = await client.SendAsync(requestRapidoc);
            // responseRapidoc.EnsureSuccessStatusCode();
            // string jsonResponse = await responseRapidoc.Content.ReadAsStringAsync();
            // dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            // if(result is not null && result.success == "true") 
            // {
            //     response.Data.RapidocId = result!.beneficiaries[0].uuid.ToString(); 
            //     await customerRepository.UpdateAsync(response.Data);
            // }

            // Address address = _mapper.Map<Address>(request.Address);
            // address.Parent = "customer-recipient";
            // address.ParentId = response.Data!.Id;
            // ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
            // if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");

            // await logRepository.CreateAsync(new()
            // {   
            //     Action = "Criação",
            //     Collection = "customer-recipient",
            //     Description = $"Criação Beneficiário {request.Name}",
            //     CreatedBy = request.CreatedBy,
            //     Parent = "customer",
            //     ParentId = response.Data.ContractorId                 
            // });

            return new(null, 201, "Beneficiário criado com sucesso.");
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
                foreach (string moduleId in plan.Data.ServiceModuleIds)
                {
                    ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                    if(serviceModule.Data is not null) 
                    {
                        if(serviceModule.Data.Name.Equals("Bem + Cuidado"))
                        {
                            especialista = true;
                        }

                        if(serviceModule.Data.Name.Equals("Bem + Papo"))
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
            // string jsonResponse = await responseRapidoc.Content.ReadAsStringAsync();
            // dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            // Util.ConsoleLog(result);
            // var response = await client.SendAsync(requestHeader);
            // responseRapidoc.EnsureSuccessStatusCode();
            // Console.WriteLine(await responseRapidoc.Content.ReadAsStringAsync());

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
        catch(Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> UpdateProfileAsync(UpdateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            customerResponse.Data.UpdatedAt = DateTime.UtcNow;
            customerResponse.Data.RapidocId = customerResponse.Data.RapidocId;
            customerResponse.Data.Name = request.Name;
            customerResponse.Data.Email = request.Email;
            customerResponse.Data.Phone = request.Phone;
            customerResponse.Data.Cpf = request.Cpf;
            customerResponse.Data.Weight = request.Weight;
            customerResponse.Data.Height = request.Height;
            customerResponse.Data.TargetSleepTime = request.TargetSleepTime;
            customerResponse.Data.LastSupper = request.LastSupper;
            customerResponse.Data.Patrology = request.Patrology;

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            // await logRepository.CreateAsync(new()
            // {   
            //     Action = "Atualização",
            //     Collection = "customer-recipient",
            //     Description = $"Atualizou Beneficiário {customerResponse.Data.Name}",
            //     CreatedBy = request.UpdatedBy,
            //     Parent = "customer",
            //     ParentId = response.Data.ContractorId                 
            // });
            
            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> UpdateDassAsync(UpdateDassCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            customerResponse.Data.UpdatedAt = DateTime.UtcNow;
            customerResponse.Data.RapidocId = customerResponse.Data.RapidocId;
            customerResponse.Data.Dass = new Dass()
            {
                Anxiety = request.Anxiety,
                Depression = request.Depression,
                Stress = request.Stress,
                Total = request.Total
            };

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> UpdateProfilePhotoAsync(UpdatePhotoCustomerRecipientDTO request)
    {
        try
        {
            if (request.Photo == null || request.Photo.Length == 0) 
            return new(null, 400, "Falha ao salvar foto de perfil");

            var customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if (customerResponse.Data is null) return new(null, 404, "Beneficiário não encontrado");

            byte[] convertedBytes;
            using (var stream = request.Photo.OpenReadStream())
            using (var image = new MagickImage(stream))
            {
                image.AutoOrient(); 
                
                image.Resize(new MagickGeometry(800, 800) { IgnoreAspectRatio = false });
                
                image.Format = MagickFormat.Jpeg;
                image.Quality = 80;
                
                convertedBytes = image.ToByteArray();
            }

            var convertedPhoto = new ByteArrayFormFile(convertedBytes, "profile.jpg");

            string uriPhoto = await cloudinaryHandler.UploadAttachment("customer-recipient", convertedPhoto);

            customerResponse.Data.UpdatedAt = DateTime.UtcNow;
            customerResponse.Data.Photo = uriPhoto;

            var response = await customerRepository.UpdateAsync(customerResponse.Data);
            if (!response.IsSuccess) return new(null, 400, "Falha ao salvar no banco");

            await logRepository.CreateAsync(new()
            { 
                Action = "Atualização",
                Collection = "customer-recipient",
                Description = $"Atualizou Beneficiário {customerResponse.Data.Name}",
                CreatedBy = request.UpdatedBy,
                Parent = "customer",
                ParentId = response.Data?.ContractorId ?? ""                 
            });            

            return new(new() { Photo = response.Data?.Photo ?? "" }, 200, "Atualizado com sucesso");
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
            customerResponse.Data.UpdatedBy = request.UpdatedBy;
            customerResponse.Data.Justification = customerResponse.Data.Active ? request.Justification : "";
            customerResponse.Data.Rason = customerResponse.Data.Active ? request.Rason : "";
            customerResponse.Data.Active = !customerResponse.Data.Active;

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

            string url = $"{uri}/beneficiaries/{customerResponse.Data.RapidocId}";

            var requestHeader = new HttpRequestMessage(customerResponse.Data.Active ? HttpMethod.Put : HttpMethod.Delete, customerResponse.Data.Active ? $"{url}/reactivate" : url);
            requestHeader.Headers.Add("Authorization", $"Bearer {token}");
            requestHeader.Headers.Add("clientId", $"{clientId}");
            var responseRapidoc = await client.SendAsync(requestHeader);

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
    public async Task<ResponseApi<CustomerRecipient?>> UpdateConvertOrContractorAsync(UpdateCustomerRecipientDTO request)
    {
        try
        {
            ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
            if(customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ResponseApi<Customer?> customerRes = await customerRepository1.GetByIdAsync(customerResponse.Data.ContractorId);
            if(customerRes.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ResponseApi<Customer?> newCustomer = await customerRepository1.CreateAsync(new ()
            {
                Type = "B2C",
                CorporateName = customerResponse.Data.Name,
                Document = customerResponse.Data.Cpf,
                Rg = customerResponse.Data.Rg,
                DateOfBirth = customerResponse.Data.DateOfBirth,
                Gender = customerResponse.Data.Gender,
                Phone = customerResponse.Data.Phone,
                Whatsapp = customerResponse.Data.Whatsapp,
                Email = customerResponse.Data.Email,
                Segment = "",
                Origin = "",
                Responsible = new(),
                EffectiveDate = customerResponse.Data.EffectiveDate,
                MinimumValue = 0,
                Notes = "",
                TypePlan = customerRes.Data.TypePlan,
                CreatedBy = request.UpdatedBy
            });

            if(!newCustomer.IsSuccess || newCustomer.Data is null) return new(null, 400, "Falha ao tornar beneficiário contratante");
            
            customerResponse.Data.UpdatedAt = DateTime.UtcNow;
            customerResponse.Data.UpdatedBy = request.UpdatedBy;
            customerResponse.Data.ContractorId = newCustomer.Data.Id;
            customerResponse.Data.Bond = "Titular";

            ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

            await logRepository.CreateAsync(new()
            {   
                Action = "Atualização",
                Collection = "customer-recipient",
                Description = $"O Beneficiário {customerResponse.Data.Name} foi convertido para contratante",
                CreatedBy = request.UpdatedBy,
                Parent = "customer",
                ParentId = response.Data.ContractorId                 
            });
            
            return new(response.Data, 201, "Alterado com sucesso");
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
    #region Fuctions
    public class ByteArrayFormFile(byte[] bytes, string fileName) : IFormFile
    {
        private readonly byte[] _bytes = bytes;
        public string ContentType => "image/jpeg";
        public string ContentDisposition => $"form-data; name=\"photo\"; filename=\"{fileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length => _bytes.Length;
        public string Name => "photo";
        public string FileName => fileName;
        public Stream OpenReadStream() => new MemoryStream(_bytes);
        public void CopyTo(Stream target) => new MemoryStream(_bytes).CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) => new MemoryStream(_bytes).CopyToAsync(target, cancellationToken);
    }
    #endregion
}
}