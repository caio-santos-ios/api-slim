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
using ClosedXML.Excel;

namespace api_slim.src.Services
{
    public class CustomerRecipientService
    (
        ICustomerRecipientRepository customerRepository,
        ICustomerRepository customerRepository1,
        IAddressRepository addressRepository,
        IPlanRepository planRepository,
        IServiceModuleRepository serviceModuleRepository,
        IMapper _mapper,
        ILogRepository logRepository,
        CloudinaryHandler cloudinaryHandler,
        MailHandler mailHandler,
        IAppointmentNotificationService appointmentNotificationService,
        ITelemedicineHistoricRepository telemedicineHistoricRepository,
        IVitalRepository vitalRepository,
        IB2BInvoiceRepository b2BInvoiceRepository,
        INotificationRepository notificationRepository
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
        public async Task<ResponseApi<List<dynamic>>> GetRankingAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<CustomerRecipient> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customers = await customerRepository.GetAllAsync(pagination);

                List<dynamic> rankings = [];
                if (customers.Data is not null)
                {
                    foreach (var item in customers.Data)
                    {
                        var dataDict = (IDictionary<string, object>)item;

                        if (!dataDict.ContainsKey("rapidocId")) continue;
                        if (string.IsNullOrEmpty(item.rapidocId)) continue;

                        ResponseApi<TelemedicineHistoric?> res = await telemedicineHistoricRepository.GetByRecipientIdAsync(item.rapidocId);

                        if (res.Data is null)
                        {
                            ResponseApi<Vital?> vital = await vitalRepository.GetByBeneficiaryIdAsync(item.id);

                            if (vital.Data is null) continue;
                        }

                        rankings.Add(item);
                    }
                }
                return new(rankings);
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
                if (customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
                string rapidocId = "";

                var dataDict = (IDictionary<string, object>)customer.Data;
                if (dataDict.ContainsKey("rapidocId"))
                {
                    rapidocId = dataDict["rapidocId"]?.ToString()!;
                }

                if (string.IsNullOrEmpty(rapidocId))
                {
                    ResponseApi<dynamic?> res = await GetByCPFAggregateAsync(customer.Data.cpf);
                    if (res.Data is not null)
                    {
                        var dataDict2 = (IDictionary<string, object>)customer.Data;

                        if (dataDict2.ContainsKey("rapidocId"))
                        {
                            rapidocId = dataDict2["rapidocId"]?.ToString()!;
                        }
                    }
                }
                ;

                if (!string.IsNullOrEmpty(rapidocId))
                {
                    var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries/{rapidocId}/appointments");
                    requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                    requestHeader.Headers.Add("clientId", clientId);

                    var content = new StringContent(string.Empty);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                    requestHeader.Content = content;
                    var response = await client.SendAsync(requestHeader);

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                    DateTime? nextDate = null;
                    dynamic telemedicine = new { };
                    if (result is not null)
                    {
                        foreach (dynamic item in result)
                        {
                            var element = item is Newtonsoft.Json.Linq.JProperty jProp ? jProp.Value : (Newtonsoft.Json.Linq.JToken)item;

                            if (element.Type != Newtonsoft.Json.Linq.JTokenType.Object) continue;

                            var status = element["status"]?.ToString();
                            if (status != "SCHEDULED") continue;
                            // if(item.status != "SCHEDULED") continue;

                            DateTime date = DateTime.Parse(item.detail.date.ToString(), new CultureInfo("pt-BR"));

                            if (date.Date < DateTime.UtcNow.Date) continue;

                            if (nextDate is null)
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
                                if (nextDate > date.Date)
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
                        }
                        ;
                    }
                    ;

                    customer.Data.telemedicine = telemedicine;
                }

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
                if (customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
                string rapidocId = customer.Data.rapidocId;

                if (string.IsNullOrEmpty(customer.Data.rapidocId))
                {
                    ResponseApi<dynamic?> res = await GetByCPFAggregateAsync(customer.Data.cpf);
                    if (res.Data is not null)
                    {
                        rapidocId = res.Data.RapidocId;
                    }
                }
                ;

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
                if (result is not null)
                {
                    link = result.url;
                }

                dynamic? obj = new { link };

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
                var dataDict = (IDictionary<string, object>)customer.Data!;

                if (customer.Data is not null)
                {
                    string rapidocId = "";
                    if (dataDict.ContainsKey("rapidocId"))
                    {
                        rapidocId = dataDict["rapidocId"]?.ToString()!;
                    }
                    else if (dataDict.ContainsKey("RapidocId"))
                    {
                        rapidocId = dataDict["RapidocId"]?.ToString()!;
                    }

                    if (string.IsNullOrEmpty(rapidocId))
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

                        if (result is not null)
                        {
                            ResponseApi<CustomerRecipient?> res = await customerRepository.GetByIdAsync(customer.Data.id);

                            if (result.success == "true")
                            {
                                if (res.Data is not null)
                                {
                                    res.Data.RapidocId = result.beneficiary.uuid.ToString();
                                    customer.Data.rapidocId = result.beneficiary.uuid.ToString();
                                    await customerRepository.UpdateAsync(res.Data);
                                }
                            }
                            else
                            {
                                if (res.Data is not null)
                                {
                                    var requestRapidocPost = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

                                    requestRapidocPost.Headers.Add("Authorization", $"Bearer {token}");
                                    requestRapidocPost.Headers.Add("clientId", clientId);
                                    requestRapidocPost.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                    string typePlan = "G";
                                    bool psicologia = false;
                                    bool especialista = false;

                                    ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(res.Data.PlanId);
                                    if (plan.Data is not null)
                                    {
                                        foreach (string moduleId in plan.Data.ServiceModuleIds)
                                        {
                                            ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                                            if (serviceModule.Data is not null)
                                            {
                                                if (serviceModule.Data.Name.Equals("Bem + Cuidado"))
                                                {
                                                    especialista = true;
                                                }

                                                if (serviceModule.Data.Name.Equals("Bem + Papo"))
                                                {
                                                    psicologia = true;
                                                }
                                            }
                                        }
                                    }

                                    if (psicologia || especialista)
                                    {
                                        if (psicologia && especialista)
                                        {
                                            typePlan = "GSP";
                                        }
                                        else
                                        {
                                            if (psicologia)
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
                                    }
                                    ;

                                    ResponseApi<Address> address = await addressRepository.GetByParentIdAsync(customer.Data.id, "customer-recipient");
                                    var beneficiarios = new[]
                                    {
                                    new {
                                        name = res.Data.Name,
                                        cpf = new string(res.Data.Cpf.Where(char.IsDigit).ToArray()),
                                        birthday = res.Data.DateOfBirth is null ? DateTime.UtcNow : res.Data.DateOfBirth,
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
                                    if (resultPost is not null)
                                    {
                                        if (resultPost.success == "true")
                                        {
                                            customer.Data.rapidocId = resultPost.beneficiaries[0].uuid.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (customer.Data is null) return new(null, 404, "Beneficiário não encontrado");

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
                if (customer.Data is null) return new(null, 404, "Beneficiário não encontrado");
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
        public async Task<ResponseApi<List<dynamic>>> GetManagerPanelAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<CustomerRecipient> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customerRecipient = await customerRepository.GetManagerContractorIdAggregationAsync(pagination);

                DateTime today = DateTime.UtcNow;
                DateTime currentMonthStart = new(today.Year, today.Month, 1);

                request.QueryParams.TryGetValue("contractorId", out string? contractorId);
                if (!string.IsNullOrEmpty(contractorId))
                {
                    ResponseApi<Customer?> customer = await customerRepository1.GetByIdAsync(contractorId);
                    if (customer.Data is not null)
                    {
                        DateTime? effectiveDate = customer.Data.EffectiveDate;
                        if (effectiveDate is not null)
                        {
                            DateTime dataCorrente = new(effectiveDate.Value.Year, effectiveDate.Value.Month, 1);

                            while (dataCorrente <= currentMonthStart)
                            {
                                bool isCurrentMonth = dataCorrente.Year == today.Year && dataCorrente.Month == today.Month;

                                DateTime lastDayOfMonth = new DateTime(dataCorrente.Year, dataCorrente.Month, 1).AddMonths(1).AddDays(-1);

                                string status = isCurrentMonth ? "Em Aberto" : "Fechada";

                                ResponseApi<B2BInvoice?> findInvoice = await b2BInvoiceRepository.GetByMonthAsync(dataCorrente.Month, dataCorrente.Year, contractorId);

                                if (findInvoice?.Data is null)
                                {
                                    B2BInvoice newInvoice = new()
                                    {
                                        CustomerId = contractorId!,
                                        ReferenceMonth = dataCorrente.Month,
                                        ReferenceYear = dataCorrente.Year,
                                        CycleStart = dataCorrente,
                                        CycleEnd = isCurrentMonth ? null : lastDayOfMonth,
                                        DueDate = isCurrentMonth ? null : lastDayOfMonth.AddDays(3),
                                        ClosingDate = isCurrentMonth ? null : lastDayOfMonth,
                                        Status = status,
                                        TotalAmount = 0,
                                        BeneficiaryCount = 0,
                                        Items = [],
                                        CreatedAt = dataCorrente,
                                    };

                                    await b2BInvoiceRepository.CreateAsync(newInvoice);
                                }
                                else if (findInvoice.Data.Status != status)
                                {
                                    // Dia 1: fecha o mês anterior que estava "Em Aberto"
                                    findInvoice.Data.Status = "Fechada";
                                    findInvoice.Data.CycleEnd = lastDayOfMonth;
                                    findInvoice.Data.DueDate = lastDayOfMonth.AddDays(3);
                                    findInvoice.Data.ClosingDate = lastDayOfMonth;

                                    await b2BInvoiceRepository.UpdateAsync(findInvoice.Data);
                                }

                                dataCorrente = dataCorrente.AddMonths(1);
                            }
                        }
                    }
                }

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
                if (existed.Data is not null) return new(null, 400, "CPF já utilizado");

                CustomerRecipient customer = _mapper.Map<CustomerRecipient>(request);
                ResponseApi<long?> code = await customerRepository.GetNextCodeAsync();
                customer.Code = code.Data.ToString()!.PadLeft(6, '0');

                ResponseApi<Customer?> customerContractor = await customerRepository1.GetByIdAsync(customer.ContractorId);
                customer.Type = customerContractor.Data is not null ? customerContractor.Data.Type : "";

                ResponseApi<CustomerRecipient?> response = await customerRepository.CreateAsync(customer);

                if (response.Data is null) return new(null, 400, "Falha ao criar Beneficiário.");

                var requestRapidoc = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

                requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
                requestRapidoc.Headers.Add("clientId", clientId);
                requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string typePlan = "G";
                bool psicologia = false;
                bool especialista = false;

                ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
                if (plan.Data is not null)
                {
                    foreach (string moduleId in plan.Data.ServiceModuleIds)
                    {
                        ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                        if (serviceModule.Data is not null)
                        {
                            if (serviceModule.Data.Name.Equals("Bem + Cuidado"))
                            {
                                especialista = true;
                            }

                            if (serviceModule.Data.Name.Equals("Bem + Papo"))
                            {
                                psicologia = true;
                            }
                        }
                    }
                }

                if (psicologia || especialista)
                {
                    if (psicologia && especialista)
                    {
                        typePlan = "GSP";
                    }
                    else
                    {
                        if (psicologia)
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
                }
                ;

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
                if (result is not null && result.success == "true")
                {
                    response.Data.RapidocId = result!.beneficiaries[0].uuid.ToString();
                    await customerRepository.UpdateAsync(response.Data);
                }

                Address address = _mapper.Map<Address>(request.Address);
                address.Parent = "customer-recipient";
                address.ParentId = response.Data!.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if (!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");

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

                if (!string.IsNullOrEmpty(request.Whatsapp))
                {
                    // List<NotificationJob> jobs = new()
                    // {
                    //     new() {
                    //         Parent = "CustomerRecipient",
                    //         ParentId = response.Data.Id!,
                    //         Phone = request.Phone,
                    //         BeneficiaryName = request.Name,
                    //         BeneficiaryCPF = request.Cpf,
                    //         BeneficiaryId = response.Data.Id,
                    //         Message = WhatsAppTemplate.Welcome(request.Name),
                    //         SendDate = DateTime.UtcNow.AddSeconds(15),
                    //         Type = "Welcome",
                    //         Origin = "WhatsApp",
                    //         Title = "Boas Vindas"
                    //     },
                    //     new() {
                    //         Parent = "CustomerRecipient",
                    //         ParentId = response.Data.Id!,
                    //         Phone = request.Phone,
                    //         BeneficiaryName = request.Name,
                    //         BeneficiaryCPF = request.Cpf,
                    //         BeneficiaryId = response.Data.Id,
                    //         Message = WhatsAppTemplate.AppDownloadInstructions(),
                    //         SendDate = DateTime.UtcNow.AddSeconds(30),
                    //         Type = "InstalationApp",
                    //         Origin = "WhatsApp",
                    //         Title = "Instruções pra instalar APP"
                    //     },
                    // };
                    // await appointmentNotificationService.CreateNotificationsAsync(jobs, Util.CleanPhone(request.Whatsapp));
                }

                return new(response.Data, 201, "Beneficiário criado com sucesso.");
            }
            catch
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<CustomerRecipient?>> CreatePanelManagerAsync(CreateCustomerRecipientDTO request)
        {
            try
            {
                ResponseApi<CustomerRecipient?> response = await CreateAsync(request);
                DateTime today = DateTime.UtcNow;

                var invoice = await b2BInvoiceRepository.GetByMonthAsync(today.Month - 1, today.Year, request.ContractorId);

                if (invoice.Data is not null)
                {
                    if (!string.IsNullOrEmpty(request.PlanId))
                    {
                        var plan = await planRepository.GetByIdAsync(request.PlanId);
                        if (plan.Data is not null)
                        {
                            invoice.Data.TotalAmount += plan.Data.Price;
                            invoice.Data.BeneficiaryCount += 1;
                        }
                    }

                    foreach (string serviceModuleId in request.ServiceModuleIds)
                    {
                        var modules = await serviceModuleRepository.GetByIdAsync(serviceModuleId);

                        if (modules.Data is not null)
                        {
                            invoice.Data.TotalAmount += modules.Data.Price;
                        }
                    }

                    await b2BInvoiceRepository.UpdateAsync(invoice.Data);
                }

                return new(new(), 400, "Beneficiário criado com sucesso.");
            }
            catch
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<CustomerRecipient?>> CreateRapidocAsync(CreateCustomerRecipientDTO request)
        {
            try
            {
                var requestRapidoc = new HttpRequestMessage(HttpMethod.Post, $"{uri}/beneficiaries");

                requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
                requestRapidoc.Headers.Add("clientId", clientId);
                requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string typePlan = "G";
                bool psicologia = false;
                bool especialista = false;

                ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
                if (plan.Data is not null)
                {
                    foreach (string moduleId in plan.Data.ServiceModuleIds)
                    {
                        ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                        if (serviceModule.Data is not null)
                        {
                            if (serviceModule.Data.Name.Equals("Bem + Cuidado"))
                            {
                                especialista = true;
                            }

                            if (serviceModule.Data.Name.Equals("Bem + Papo"))
                            {
                                psicologia = true;
                            }
                        }
                    }
                }

                if (psicologia || especialista)
                {
                    if (psicologia && especialista)
                    {
                        typePlan = "GSP";
                    }
                    else
                    {
                        if (psicologia)
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
                }
                ;

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

                return new(new()
                {
                    RapidocId = result is not null ? result.beneficiaries[0].uuid : ""
                }, 201, "Beneficiário criado com sucesso.");
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
                if (existed.Data is not null) return new(null, 400, "CPF já utilizado");
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
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                ResponseApi<Customer?> customerContractor = await customerRepository1.GetByIdAsync(customerResponse.Data.ContractorId);

                CustomerRecipient customer = _mapper.Map<CustomerRecipient>(request);
                customer.UpdatedAt = DateTime.UtcNow;
                customer.CreatedAt = customerResponse.Data.CreatedAt;
                customer.Code = customerResponse.Data.Code;
                customer.RapidocId = customerResponse.Data.RapidocId;
                customer.Type = customerContractor.Data is not null ? customerContractor.Data.Type : "";

                ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customer);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                var requestRapidoc = new HttpRequestMessage(HttpMethod.Put, $"{uri}/beneficiaries/{response.Data!.RapidocId}");

                requestRapidoc.Headers.Add("Authorization", $"Bearer {token}");
                requestRapidoc.Headers.Add("clientId", clientId);
                requestRapidoc.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string typePlan = "G";
                bool psicologia = false;
                bool especialista = false;

                ResponseApi<Plan?> plan = await planRepository.GetByIdAsync(request.PlanId);
                if (plan.Data is not null)
                {
                    foreach (string moduleId in plan.Data.ServiceModuleIds)
                    {
                        ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(moduleId);
                        if (serviceModule.Data is not null)
                        {
                            if (serviceModule.Data.Name.Equals("Bem + Cuidado"))
                            {
                                especialista = true;
                            }

                            if (serviceModule.Data.Name.Equals("Bem + Papo"))
                            {
                                psicologia = true;
                            }
                        }
                    }
                }

                if (psicologia || especialista)
                {
                    if (psicologia && especialista)
                    {
                        typePlan = "GSP";
                    }
                    else
                    {
                        if (psicologia)
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
                }
                ;

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

                ResponseApi<Address?> existingAddress = await addressRepository.GetByParentIdAsync(response.Data!.Id, "customer-recipient");

                if (existingAddress.Data is not null)
                {
                    existingAddress.Data.Street = request.Address.Street;
                    existingAddress.Data.Number = request.Address.Number;
                    existingAddress.Data.Complement = request.Address.Complement;
                    existingAddress.Data.Neighborhood = request.Address.Neighborhood;
                    existingAddress.Data.City = request.Address.City;
                    existingAddress.Data.State = request.Address.State;
                    existingAddress.Data.ZipCode = request.Address.ZipCode;

                    ResponseApi<Address?> addressUpdateResponse = await addressRepository.UpdateAsync(existingAddress.Data);
                    if (!addressUpdateResponse.IsSuccess) return new(null, 400, "Falha ao atualizar endereço.");
                }
                else
                {
                    Address address = _mapper.Map<Address>(request.Address);
                    address.Parent = "customer-recipient";
                    address.ParentId = response.Data!.Id;

                    ResponseApi<Address?> addressCreateResponse = await addressRepository.CreateAsync(address);
                    if (!addressCreateResponse.IsSuccess) return new(null, 400, "Falha ao criar endereço.");
                }
                ;

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
        public async Task<ResponseApi<CustomerRecipient?>> UpdateProfileAsync(UpdateCustomerRecipientDTO request)
        {
            try
            {
                ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

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
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

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
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

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
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

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
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                customerResponse.Data.UpdatedAt = DateTime.UtcNow;
                customerResponse.Data.UpdatedBy = request.UpdatedBy;
                customerResponse.Data.Justification = customerResponse.Data.Active ? request.Justification : "";
                customerResponse.Data.Rason = customerResponse.Data.Active ? request.Rason : "";
                customerResponse.Data.Active = !customerResponse.Data.Active;

                ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
                if (!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

                string url = $"{uri}/beneficiaries/{customerResponse.Data.RapidocId}";

                var requestHeader = new HttpRequestMessage(customerResponse.Data.Active ? HttpMethod.Put : HttpMethod.Delete, customerResponse.Data.Active ? $"{url}/reactivate" : url);
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", $"{clientId}");
                var responseRapidoc = await client.SendAsync(requestHeader);

                await logRepository.CreateAsync(new()
                {
                    Action = "Atualização",
                    Collection = request.Rason,
                    Description = customerResponse.Data.Active ? $"Ativou Beneficiário {customerResponse.Data.Name}" : $"Inativou Beneficiário {customerResponse.Data.Name} - Justificativa: {request.Justification}",
                    CreatedBy = request.UpdatedBy,
                    Parent = "customer",
                    ParentId = customerResponse.Data.Id,
                    Key = "update-status-recipient"
                });

                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<CustomerRecipient?>> UpdateWhatsAppAsync(UpdateCustomerRecipientDTO request)
        {
            try
            {
                ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.Id);
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                customerResponse.Data.UpdatedAt = DateTime.UtcNow;
                customerResponse.Data.UpdatedBy = request.UpdatedBy;
                customerResponse.Data.Whatsapp = request.Whatsapp;

                ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
                if (!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

                ResponseApi<List<NotificationJob>> notifications = await notificationRepository.GetByParentIdAsync(request.ParentId, "InPerson");
                if (notifications.Data is not null)
                {
                    foreach (NotificationJob notification in notifications.Data)
                    {
                        notification.Phone = request.Whatsapp;

                        await notificationRepository.UpdateAsync(notification);
                    }
                }

                return new(response.Data, 200, "Atualizado com sucesso");
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
                if (customerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                ResponseApi<Customer?> customerRes = await customerRepository1.GetByIdAsync(customerResponse.Data.ContractorId);
                if (customerRes.Data is null) return new(null, 404, "Falha ao atualizar");

                ResponseApi<Customer?> newCustomer = await customerRepository1.CreateAsync(new()
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

                if (!newCustomer.IsSuccess || newCustomer.Data is null) return new(null, 400, "Falha ao tornar beneficiário contratante");

                customerResponse.Data.UpdatedAt = DateTime.UtcNow;
                customerResponse.Data.UpdatedBy = request.UpdatedBy;
                customerResponse.Data.ContractorId = newCustomer.Data.Id;
                customerResponse.Data.Bond = "Titular";

                ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
                if (!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

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
        public async Task<ResponseApi<CustomerRecipient?>> UpdateSubNotificationAsync(PushSubscriptionRequest request)
        {
            try
            {
                ResponseApi<CustomerRecipient?> customerResponse = await customerRepository.GetByIdAsync(request.UserId);
                if (customerResponse.Data is not null)
                {
                    customerResponse.Data.UpdatedAt = DateTime.UtcNow;
                    customerResponse.Data.UpdatedBy = request.UserId;
                    customerResponse.Data.SubNotification = new()
                    {
                        Endpoint = request.Endpoint,
                        ExpirationTime = request.ExpirationTime,
                        UserId = request.UserId,
                        Keys = new()
                        {
                            P256dh = request.Keys.P256dh,
                            Auth = request.Keys.Auth
                        }
                    };

                    ResponseApi<CustomerRecipient?> response = await customerRepository.UpdateAsync(customerResponse.Data);
                }
                ;

                return new(null, 200, "Alterado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<CustomerRecipient?>> UpdateManagerPanelAsync(ImportCustomerRecipientDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0) return new(null, 400, "Arquivo para importação é obrigatório.");

                ResponseApi<Customer?> customerResponse = await customerRepository1.GetByIdAsync(request.ContractorId);
                if (customerResponse.Data is not null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await request.File.CopyToAsync(stream);
                        using (var workbook = new XLWorkbook(stream))
                        {
                            var worksheet = workbook.Worksheet("Beneficiários");
                            var rows = worksheet.RangeUsed()!.RowsUsed().Skip(1);
                            foreach (var row in rows)
                            {
                                string name = row.Cell(1).GetValue<string>();
                                string cpf = row.Cell(2).GetValue<string>();
                                string active = row.Cell(3).GetValue<string>();
                                string plan = row.Cell(4).GetValue<string>();
                                string dateOfBirth = row.Cell(5).GetValue<string>();
                                string email = row.Cell(6).GetValue<string>();
                                string phone = row.Cell(7).GetValue<string>();
                                string whatsapp = row.Cell(8).GetValue<string>();
                                string department = row.Cell(9).GetValue<string>();
                                string function = row.Cell(10).GetValue<string>();
                                string bond = row.Cell(11).GetValue<string>();
                                string holderCpf = row.Cell(12).GetValue<string>();

                                string zipcode = row.Cell(13).GetValue<string>();
                                string number = row.Cell(14).GetValue<string>();
                                string street = row.Cell(15).GetValue<string>();
                                string complement = row.Cell(16).GetValue<string>();
                                string neighborhood = row.Cell(17).GetValue<string>();
                                string city = row.Cell(18).GetValue<string>();
                                string state = row.Cell(19).GetValue<string>();

                                ResponseApi<CustomerRecipient?> recipient = await customerRepository.GetByCPFAsync(cpf, request.ContractorId);
                                if (recipient.Data is not null)
                                {
                                    ResponseApi<Plan?> findPlan = await planRepository.GetByNameAsync(plan);

                                    if (findPlan.Data is not null)
                                    {
                                        recipient.Data.PlanId = findPlan.Data.Id;
                                    }

                                    recipient.Data.Name = name;
                                    recipient.Data.Active = active.ToLower() == "ativo";
                                    recipient.Data.DateOfBirth = DateTime.TryParse(dateOfBirth, out var dob) ? dob : recipient.Data.DateOfBirth;
                                    recipient.Data.Email = email;
                                    recipient.Data.Phone = phone;
                                    recipient.Data.Whatsapp = whatsapp;

                                    recipient.Data.Department = department;
                                    recipient.Data.Function = function;
                                    recipient.Data.Bond = bond;
                                    recipient.Data.HolderCpf = holderCpf;

                                    await customerRepository.UpdateAsync(recipient.Data);

                                    ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(recipient.Data.Id, "customer-recipient");
                                    Address address = new();

                                    if (findAddress.Data is null)
                                    {
                                        address = new()
                                        {
                                            City = city,
                                            Complement = complement,
                                            Neighborhood = neighborhood,
                                            Number = complement,
                                            Parent = "customer-recipient",
                                            ParentId = recipient.Data.Id,
                                            State = state,
                                            Street = street,
                                            ZipCode = zipcode
                                        };

                                        await addressRepository.CreateAsync(address);
                                    }
                                    else
                                    {
                                        address = findAddress.Data;
                                        address.City = city;
                                        address.Complement = complement;
                                        address.Neighborhood = neighborhood;
                                        address.Number = complement;
                                        address.State = state;
                                        address.Street = street;
                                        address.ZipCode = zipcode;

                                        await addressRepository.UpdateAsync(address);
                                    }
                                }
                                else
                                {
                                    DateTime? DateOfBirth = DateTime.TryParse(dateOfBirth, out var dob) ? dob : null;
                                    ResponseApi<Plan?> findPlan = await planRepository.GetByNameAsync(plan);

                                    ResponseApi<CustomerRecipient?> created = await CreateRapidocAsync(new()
                                    {
                                        Name = name,
                                        Cpf = cpf,
                                        DateOfBirth = DateOfBirth,
                                        Email = email,
                                        Address = new()
                                        {
                                            ZipCode = zipcode,
                                            Street = street,
                                            Number = number,
                                            City = city,
                                            State = state
                                        },
                                        PlanId = findPlan.Data is null ? "" : findPlan.Data.Id,
                                    });

                                    CreateCustomerRecipientDTO newCustomer = new()
                                    {
                                        Name = name,
                                        DateOfBirth = DateOfBirth,
                                        Email = email,
                                        Phone = phone,
                                        Whatsapp = whatsapp,
                                        Department = department,
                                        Function = function,
                                        Bond = bond,
                                        RapidocId = created.Data is not null ? created.Data.RapidocId : "",
                                        ContractorId = request.ContractorId,
                                        Cpf = cpf,
                                        PlanId = findPlan.Data is null ? "" : findPlan.Data.Id,
                                        HolderCpf = holderCpf
                                    };

                                    ResponseApi<CustomerRecipient?> customerRes = await CreateAsync(newCustomer);
                                    if (customerRes.Data is not null)
                                    {
                                        Address address = new()
                                        {
                                            City = city,
                                            Complement = complement,
                                            Neighborhood = neighborhood,
                                            Number = complement,
                                            Parent = "customer-recipient",
                                            ParentId = customerRes.Data.Id,
                                            State = state,
                                            Street = street,
                                            ZipCode = zipcode
                                        };

                                        await addressRepository.CreateAsync(address);
                                    }
                                    ;
                                }
                            }
                        }
                    }
                }

                return new(null, 200, "Importação feita com sucesso");
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
                if (!customer.IsSuccess) return new(null, 400, customer.Message);

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