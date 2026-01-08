using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
public interface IServiceModuleRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceModule> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ServiceModule?>> GetByIdAsync(string id);
    Task<ResponseApi<ServiceModule?>> GetByPlanAsync(string planId);
    Task<ResponseApi<List<ServiceModule>>> GetByPlanIdAsync(string planId);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ServiceModule> pagination);
    Task<ResponseApi<ServiceModule?>> GetByNameAsync(string name);
    Task<ResponseApi<long?>> GetNextCodeAsync();
    Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceModule> pagination);
    Task<ResponseApi<ServiceModule?>> CreateAsync(ServiceModule serviceModule);
    Task<ResponseApi<ServiceModule?>> UpdateAsync(ServiceModule serviceModule);
    Task<ResponseApi<ServiceModule>> DeleteAsync(string id);
}
}