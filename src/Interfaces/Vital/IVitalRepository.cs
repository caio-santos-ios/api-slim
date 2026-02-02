using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
public interface IVitalRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Vital> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Vital?>> GetByIdAsync(string id);
    Task<ResponseApi<Vital?>> GetByBeneficiaryIdAsync(string beneficiaryId);
    Task<ResponseApi<List<Vital>>> GetByBeneficiaryIdWeekAsync(string beneficiaryId);
    Task<ResponseApi<List<Vital>>> GetByBeneficiaryIAllAsync(string beneficiaryId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Vital> pagination);
    Task<ResponseApi<Vital?>> CreateAsync(Vital vital);
    Task<ResponseApi<Vital?>> UpdateAsync(Vital vital);
    Task<ResponseApi<Vital>> DeleteAsync(string id);
}
}