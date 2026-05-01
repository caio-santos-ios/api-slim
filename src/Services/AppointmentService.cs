using System.Globalization;
using System.Net.Http.Headers;
using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace api_slim.src.Services
{
    public class AppointmentService(
        ITelemedicineHistoricService telemedicineHistoricService, 
        ITelemedicineHistoricRepository telemedicineHistoricRepository, 
        ICustomerRecipientRepository customerRecipientRepository, 
        IAppointmentNotificationService appointmentNotificationService,
        IAppointmentTelemedicineRepository appointmentTelemedicineRepository,
        INotificationRepository notificationRepository
    ) : IAppointmentService
    {
        private readonly HttpClient client = new();
        private readonly string uri = Environment.GetEnvironmentVariable("URI_RAPIDOC") ?? "";
        private readonly string clientId = Environment.GetEnvironmentVariable("CLIENT_ID_RAPIDOC") ?? "";
        private readonly string token = Environment.GetEnvironmentVariable("TOKEN_RAPIDOC") ?? "";

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                string query = "";
                
                request.QueryParams.TryGetValue("status", out string? status);
                request.QueryParams.TryGetValue("beneficiaryUuid", out string? beneficiaryUuid);

                if(!string.IsNullOrEmpty(status)) query += $"?status={status}";

                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/appointments{query}");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);
                List<dynamic> list = [];
                foreach (dynamic item in result!)
                {
                    BsonDocument bson = BsonDocument.Parse(item.ToString());
                    if(!string.IsNullOrEmpty(beneficiaryUuid))
                    {   
                        if(beneficiaryUuid != item.beneficiary.uuid.ToString()) continue;
                    };

                    list.Add(new {
                        id = item.uuid.ToString(),                
                        recipientDescription = item.beneficiary.name.ToString(),
                        recipientName = item.beneficiary.name.ToString(),
                        beneficiaryUuid = item.beneficiary.uuid.ToString(),
                        cpf = item.beneficiary.cpf.ToString(),
                        date = item.detail.date.ToString(),
                        data = DateTime.Parse(item.detail.date.ToString(), new CultureInfo("pt-BR")),
                        startTime = item.detail.from.ToString(),
                        endTime = item.detail.to.ToString(),
                        time = $"{item.detail.from.ToString()} até {item.detail.to.ToString()}",
                        specialty = item.specialty.name.ToString(),
                        specialistName = item.specialty.name.ToString(),
                        specialtyUuid = item.specialty.uuid.ToString(),
                        specialistId = item.specialty.uuid.ToString(),
                        professional = item.professional.name.ToString(),
                        status = item.status.ToString(),
                        beneficiaryUrl = bson.Contains("beneficiaryUrl") ? bson["beneficiaryUrl"].ToString() : "" 
                    });                            
                }
                list = list.OrderByDescending(x => x.data).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<PaginationApi<List<dynamic>>> GetAllV2Async(GetAllDTO request)
        {
            try
            {
                PaginationUtil<AppointmentTelemedicine> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customers = await appointmentTelemedicineRepository.GetAllAsync(pagination);
                int count = await appointmentTelemedicineRepository.GetCountDocumentsAsync(pagination);
                return new(customers.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetByIdAsync(string id)
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries/{id}/appointments");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);
                
                List<dynamic> list = [];
                foreach (dynamic item in result!)
                {
                    BsonDocument bson = BsonDocument.Parse(item.ToString());

                    list.Add(new {
                        id = item.uuid.ToString(),                
                        recipientDescription = item.beneficiary.name.ToString(),
                        beneficiaryUuid = item.beneficiary.uuid.ToString(),
                        cpf = item.beneficiary.cpf.ToString(),
                        date = item.detail.date.ToString(),
                        data = DateTime.Parse(item.detail.date.ToString(), new CultureInfo("pt-BR")),
                        startTime = item.detail.from.ToString(),
                        endTime = item.detail.to.ToString(),
                        specialty = item.specialty.name.ToString(),
                        specialtyUuid = item.specialty.uuid.ToString(),
                        professional = item.professional.name.ToString(),
                        status = item.status.ToString(),
                        beneficiaryUrl = bson.Contains("beneficiaryUrl") ? bson["beneficiaryUrl"].ToString() : "" 
                    });                            
                }
                list = list.OrderByDescending(x => x.data).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSpecialtiesAllAsync()
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/specialties");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);

                List<dynamic> list = [];
                foreach (dynamic item in result!)
                {                    
                    list.Add(new {
                        id = item.uuid.ToString(),                
                        name = item.name.ToString()
                    });                            
                }
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSpecialtyAvailabilityAllAsync(string specialtyUuid, string beneficiaryUuid)
        {
            try
            {
                DateTime date = DateTime.UtcNow;
                DateTime endDate = date.AddMonths(12);
                
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/specialty-availability?specialtyUuid={specialtyUuid}&dateInitial={date.ToString("dd/MM/yyyy")}&dateFinal={endDate.ToString("dd/MM/yyyy")}&beneficiaryUuid={beneficiaryUuid}");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    dynamic? resultError = JsonConvert.DeserializeObject(error);

                    string msg = resultError!.message.ToString();
                    return new(null, 400, msg);
                };

                // response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);

                List<dynamic> list = [];
                foreach (dynamic item in result!)
                {                  
                    list.Add(new {
                        id = item.uuid.ToString(),   
                        name = $"{item.date.ToString()} - {item.from.ToString()} Até {item.to.ToString()}",              
                        date = item.date.ToString(),
                        startTime = item.from.ToString(),
                        endTime = item.to.ToString(),
                    });                            
                }

                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetBeneficiaryMedicalReferralsAsync()
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiary-medical-referrals");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);
                List<dynamic> list = [];

                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region  CREATE
        public async Task<ResponseApi<dynamic?>> CreateAsync(CreateAppointmentDTO request)
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Post, $"{uri}/appointments");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", $"{clientId}");
                requestHeader.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                dynamic item = new
                {
                    approveAdditionalPayment = true,
                    availabilityUuid = request.AvailabilityUuid,
                    beneficiaryUuid = request.BeneficiaryUuid,
                    specialtyUuid = request.SpecialtyUuid
                };

                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(item);

                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/vnd.rapidoc.tema-v2+json");

                content.Headers.ContentType!.CharSet = null; 

                requestHeader.Content = content;

                var responseRapidoc = await client.SendAsync(requestHeader);
                if (!responseRapidoc.IsSuccessStatusCode)
                {
                    var error = await responseRapidoc.Content.ReadAsStringAsync();
                    dynamic? resultError = JsonConvert.DeserializeObject(error);

                    string msg = resultError!.message.ToString();
                    return new(null, 400, msg);
                };

                string jsonResponse = await responseRapidoc.Content.ReadAsStringAsync();
                dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                await telemedicineHistoricService.CreateAsync(new ()
                {
                    Status = "Agendado",
                    Date = request.Date,
                    Time = request.Time,
                    RecipientId = request.BeneficiaryUuid,
                    SpecialistId = request.SpecialtyUuid,
                    RecipientName = request.BeneficiaryName,
                    SpecialistName = request.SpecialtyName,
                    CreatedBy = request.CreatedBy,
                    Type = "Agendamento"
                });
                ResponseApi<CustomerRecipient?> recipientResponse = await customerRecipientRepository.GetByRapidocIdAsync(request.BeneficiaryUuid);
                if(request.Origin == "app")
                {
                    recipientResponse = await customerRecipientRepository.GetByIdAsync(request.CreatedBy);
                }

                if(recipientResponse.Data is not null && result is not null)
                {
                    string professionalName = result.professional.name;
                    DateTime date = DateTime.ParseExact(request.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var timeString = request.Time.ToLower().Split("até")[0].Trim();
                    TimeSpan time = TimeSpan.Parse(timeString);
                    DateTime dateTime = date.Add(time);

                    await appointmentTelemedicineRepository.CreateAsync(new ()
                    {
                        Active = true,
                        AppointmentUuid = result!.uuid.ToString(),
                        BeneficiaryCPF = recipientResponse.Data.Cpf,
                        BeneficiaryId = recipientResponse.Data.Id,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request.CreatedBy,
                        Date = dateTime,
                        Hour = request.Time,
                        SpecialtyUuid = result.specialty.uuid.ToString(),
                        SpecialtyName = result.specialty.name,
                        ProfessionalName = professionalName,
                        BeneficiaryUrl = result.beneficiaryUrl.ToString(),
                        Status = "Agendado"
                    });

                    List<Notification> jobs = new()
                    {
                        new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentConfirmation(recipientResponse.Data.Name, request.SpecialtyName, professionalName, request.Date, request.Time, result.beneficiaryUrl.ToString(), request.Module),
                            SendPreviusDate = DateTime.UtcNow.AddSeconds(30),
                            Type = "AppPush",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Confirmação do Agendamento"
                        },
                        new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentDayReminder(recipientResponse.Data.Name, request.SpecialtyName, request.Date, request.Time, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddDays(-1),
                            Type = "AppPush",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 1 dia antes do Agendamento"
                        },
                        new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentOneHourReminder(recipientResponse.Data.Name, professionalName, request.Time, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddHours(-1),
                            Type = "AppPush",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 1 hora antes do Agendamento"
                        },
                        new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentFiveMinutesReminder(recipientResponse.Data.Name, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddMinutes(-5),
                            Type = "AppPush",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 5 minutos antes do Agendamento"
                        },
                    };

                    if(!string.IsNullOrEmpty(recipientResponse.Data.Whatsapp))
                    {

                        jobs.Add(new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentConfirmation(recipientResponse.Data.Name, request.SpecialtyName, professionalName, request.Date, request.Time, result.beneficiaryUrl.ToString(), request.Module),
                            SendPreviusDate = DateTime.UtcNow.AddSeconds(30),
                            Type = "WhatsApp",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Confirmação do Agendamento"
                        });

                        jobs.Add(new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentDayReminder(recipientResponse.Data.Name, request.SpecialtyName, request.Date, request.Time, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddDays(-1),
                            Type = "WhatsApp",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 1 dia antes do Agendamento"
                        });

                        jobs.Add(new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentOneHourReminder(recipientResponse.Data.Name, professionalName, request.Time, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddHours(-1),
                            Type = "WhatsApp",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 1 hora antes do Agendamento"
                        });
                        
                        jobs.Add(new() {
                            Parent = "Appointment",
                            ParentId = result!.uuid.ToString(),
                            Phone = recipientResponse.Data.Whatsapp,
                            BeneficiaryName = recipientResponse.Data.Name,
                            BeneficiaryCPF = recipientResponse.Data.Cpf,
                            Message = WhatsAppTemplate.AppointmentFiveMinutesReminder(recipientResponse.Data.Name, result.beneficiaryUrl.ToString()),
                            SendPreviusDate = dateTime.AddMinutes(-5),
                            Type = "WhatsApp",
                            BeneficiaryId = recipientResponse.Data.Id,
                            Title = "Lembrete 5 minutos antes do Agendamento"
                        });
                    }
                    
                    await appointmentNotificationService.CreateNotificationsAsync(jobs, Util.CleanPhone(recipientResponse.Data.Whatsapp));
                };
                
                return new(null, 201, "Agendamento feito com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region  UPDATE
        public async Task<ResponseApi<dynamic?>> CancelAsync(CancelForwardingDTO request)
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Delete, $"{uri}/appointments/{request.Id}");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", $"{clientId}");
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    dynamic? resultError = JsonConvert.DeserializeObject(error);

                    string msg = resultError!.message.ToString();
                    return new(null, 400, msg);
                };

                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                string type = "Agendamento";
                ResponseApi<TelemedicineHistoric?> historic = await telemedicineHistoricRepository.GetByParentUuidAsync(request.Id);
                if(historic.Data is not null)
                {
                    historic.Data.Active = false;
                    type = historic.Data.Type;
                    
                    await telemedicineHistoricRepository.UpdateAsync(historic.Data);
                }

                await telemedicineHistoricService.CreateAsync(new ()
                {
                    Status = "Cancelado",
                    Date = request.Date,
                    Time = request.Time,
                    RecipientId = request.BeneficiaryUuid,
                    SpecialistId = request.SpecialtyUuid,
                    RecipientName = request.BeneficiaryName,
                    SpecialistName = request.SpecialtyName,
                    CreatedBy = request.CreatedBy,
                    Type = type
                });

                await appointmentNotificationService.CancelNotificationsAsync(request.Id, "Appointment");

                ResponseApi<AppointmentTelemedicine?> appoint = await appointmentTelemedicineRepository.GetByAppointmentUuidAsync(request.Id);
                if(appoint.Data is not null)
                {
                    appoint.Data.Status = "Cancelado";
                    await appointmentTelemedicineRepository.UpdateAsync(appoint.Data);
                }

                return new(null, 204, "Cancelado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}