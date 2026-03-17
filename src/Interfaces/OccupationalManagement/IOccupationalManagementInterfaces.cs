using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    // ─── Micro Checkin ───────────────────────────────────────────────────────────
    public interface IOccupationalMicroCheckinService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalMicroCheckin?>> CreateAsync(CreateOccupationalMicroCheckinDTO request);
        Task<ResponseApi<OccupationalMicroCheckin?>> UpdateAsync(UpdateOccupationalMicroCheckinDTO request);
        Task<ResponseApi<OccupationalMicroCheckin>> DeleteAsync(string id, string userId);
    }

    public interface IOccupationalMicroCheckinRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalMicroCheckin> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalMicroCheckin?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalMicroCheckin> pagination);
        Task<ResponseApi<OccupationalMicroCheckin?>> CreateAsync(OccupationalMicroCheckin entity);
        Task<ResponseApi<OccupationalMicroCheckin?>> UpdateAsync(OccupationalMicroCheckin entity);
        Task<ResponseApi<OccupationalMicroCheckin>> DeleteAsync(string id);
    }

    // ─── Bem Vital ───────────────────────────────────────────────────────────────
    public interface IOccupationalBemVitalService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalBemVital?>> CreateAsync(CreateOccupationalBemVitalDTO request);
        Task<ResponseApi<OccupationalBemVital?>> UpdateAsync(UpdateOccupationalBemVitalDTO request);
        Task<ResponseApi<OccupationalBemVital>> DeleteAsync(string id, string userId);
    }

    public interface IOccupationalBemVitalRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalBemVital> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalBemVital?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalBemVital> pagination);
        Task<ResponseApi<OccupationalBemVital?>> CreateAsync(OccupationalBemVital entity);
        Task<ResponseApi<OccupationalBemVital?>> UpdateAsync(OccupationalBemVital entity);
        Task<ResponseApi<OccupationalBemVital>> DeleteAsync(string id);
    }

    // ─── PGR ─────────────────────────────────────────────────────────────────────
    public interface IOccupationalPgrService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalPgr?>> GenerateAsync(GenerateOccupationalPgrDTO request);
        Task<ResponseApi<OccupationalPgr>> DeleteAsync(string id, string userId);
    }

    public interface IOccupationalPgrRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalPgr> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<OccupationalPgr?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalPgr> pagination);
        Task<ResponseApi<OccupationalPgr?>> CreateAsync(OccupationalPgr entity);
        Task<ResponseApi<OccupationalPgr?>> UpdateAsync(OccupationalPgr entity);
        Task<ResponseApi<OccupationalPgr>> DeleteAsync(string id);
    }
}
