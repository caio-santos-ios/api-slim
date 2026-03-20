using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces;

public interface IPermissionProfileService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PermissionProfile?>> CreateAsync(CreatePermissionProfileDTO request);
    Task<ResponseApi<PermissionProfile?>> UpdateAsync(UpdatePermissionProfileDTO request);
    Task<ResponseApi<PermissionProfile>> DeleteAsync(string id, string userId);
    /// <summary>Copia os modules do perfil para User.Modules (base editável).</summary>
    Task<ResponseApi<User?>> ApplyToUserAsync(ApplyPermissionProfileDTO request);
}

public interface IPermissionProfileRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PermissionProfile> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PermissionProfile?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<PermissionProfile> pagination);
    Task<ResponseApi<PermissionProfile?>> CreateAsync(PermissionProfile entity);
    Task<ResponseApi<PermissionProfile?>> UpdateAsync(PermissionProfile entity);
    Task<ResponseApi<PermissionProfile>> DeleteAsync(string id);
}