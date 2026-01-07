using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Newtonsoft.Json;

namespace api_slim.src.Services
{
    public class TelemedicineService(ICustomerRecipientRepository customerRepository) : ITelemedicineService
    {
        private readonly HttpClient client = new();
        private readonly string uri = Environment.GetEnvironmentVariable("URI_RAPIDOC") ?? "";
        private readonly string clientId = Environment.GetEnvironmentVariable("CLIENT_ID_RAPIDOC") ?? "";
        private readonly string token = Environment.GetEnvironmentVariable("TOKEN_RAPIDOC") ?? "";

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync()
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Get, $"{uri}/beneficiaries");
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
                if(result is not null) 
                {
                    if(result.success == "true") 
                    {
                        foreach (dynamic item in result.beneficiaries)
                        {
                            string cpf = Regex.Replace(item.cpf.ToString(), @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");
                            // ResponseApi<CustomerRecipient?> recipient = await customerRepository.GetByCPFAsync(cpf);
                            
                            list.Add(new {
                                id = item.uuid.ToString(),
                                dateOfBirth = item.birthday.ToString(),
                                cpf = item.cpf.ToString(),
                                name = item.name.ToString(),
                                status = item.isActive.ToString(),
                                // system = recipient.Data is not null
                            });                            
                        }
                    };
                }

                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region  UPDATE
        public async Task<ResponseApi<dynamic?>> UpdateStatusAsync(UpdateStatusTelemedicineDTO request)
        {
            try
            {
                var requestHeader = new HttpRequestMessage(HttpMethod.Delete, $"{uri}/beneficiaries/{request.Id}");
                requestHeader.Headers.Add("Authorization", $"Bearer {token}");
                requestHeader.Headers.Add("clientId", $"{clientId}");
                var response = await client.SendAsync(requestHeader);

                return new(null, 204, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}