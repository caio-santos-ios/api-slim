using System.Net.Http.Headers;
using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace api_slim.src.Services
{
    public class AppointmentService(IHistoricService historicService) : IAppointmentService
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
                System.Console.WriteLine(status);
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/appointments{query}");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", clientId);
                
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.rapidoc.tema-v2+json");
                requestHeader.Content = content;
                var response = await client.SendAsync(requestHeader);
                // response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(jsonResponse);

                List<dynamic> list = [];
                foreach (dynamic item in result!)
                {          
                    BsonDocument bson = BsonDocument.Parse(item.ToString());
   
                    list.Add(new {
                        id = item.uuid.ToString(),                
                        recipientDescription = item.beneficiary.name.ToString(),
                        cpf = item.beneficiary.cpf.ToString(),
                        date = item.detail.date.ToString(),
                        startTime = item.detail.from.ToString(),
                        endTime = item.detail.to.ToString(),
                        specialty = item.specialty.name.ToString(),
                        professional = item.professional.name.ToString(),
                        status = item.status.ToString(),
                        beneficiaryUrl = bson.Contains("beneficiaryUrl") ? bson["beneficiaryUrl"].ToString() : "" 
                    });                            
                }
                return new(list);
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
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
                response.EnsureSuccessStatusCode();
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
                DateTime endDate = date.AddMonths(2);
                
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

                response.EnsureSuccessStatusCode();
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

                responseRapidoc.EnsureSuccessStatusCode();
                string jsonResponse = await responseRapidoc.Content.ReadAsStringAsync();
                dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                await historicService.CreateAsync(new ()
                {
                    Collection = "appointment",
                    Description = "Agendamento",
                });

                return new(null, 201, "Agendamento feito com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region  UPDATE
        public async Task<ResponseApi<dynamic?>> CancelAsync(string id)
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Delete, $"{uri}/appointments/{id}");
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

                await historicService.CreateAsync(new ()
                {
                    Collection = "appointment",
                    Description = "Agendamento Cancelado",
                });

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