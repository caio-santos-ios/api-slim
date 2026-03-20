using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface INotificationService
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic>> CreateAsync();
        Task<ResponseApi<dynamic>> UpdateAsync(string id);
        Task<ResponseApi<dynamic>> UpdateReadAsync(string id);
        Task<ResponseApi<dynamic>> DeleteAsync(string id);
    }
}