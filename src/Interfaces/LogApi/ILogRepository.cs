using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    public interface ILogRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Log> pagination);
        Task<ResponseApi<Log?>> CreateAsync(Log log);
    }
}