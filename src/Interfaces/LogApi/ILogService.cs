using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface ILogService
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<Log?>> CreateAsync(CreateLogDTO log);
    }
}