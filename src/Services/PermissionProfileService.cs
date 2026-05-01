using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class PermissionProfileService(
        IPermissionProfileRepository repository,
        IUserRepository userRepository,
        IMapper _mapper) : IPermissionProfileService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<PermissionProfile> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> data = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(data.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> data = await repository.GetByIdAggregateAsync(id);
                if (data.Data is null) return new(null, 404, "Perfil não encontrado");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<PermissionProfile?>> CreateAsync(CreatePermissionProfileDTO request)
        {
            try
            {
                PermissionProfile entity = _mapper.Map<PermissionProfile>(request);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<PermissionProfile?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Perfil.");
                return new(response.Data, 201, "Perfil criado com sucesso.");
            }
            catch 
            { 
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); 
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<PermissionProfile?>> UpdateAsync(UpdatePermissionProfileDTO request)
        {
            try
            {
                ResponseApi<PermissionProfile?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Perfil não encontrado");

                existing.Data.Name        = request.Name;
                existing.Data.Description = request.Description;
                existing.Data.Modules     = request.Modules;
                existing.Data.UpdatedAt   = DateTime.Now;
                existing.Data.UpdatedBy   = request.UpdatedBy;

                ResponseApi<PermissionProfile?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Perfil.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<PermissionProfile>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<PermissionProfile> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region APPLY TO USER
        /// <summary>
        /// Copia os Modules do perfil para User.Modules.
        /// O perfil é base — as permissões individuais do usuário
        /// podem ser ajustadas depois via PUT /users/modules.
        /// </summary>
        public async Task<ResponseApi<User?>> ApplyToUserAsync(ApplyPermissionProfileDTO request)
        {
            try
            {
                ResponseApi<PermissionProfile?> profile = await repository.GetByIdAsync(request.ProfileId);
                if (profile.Data is null) return new(null, 404, "Perfil não encontrado");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.UserId);
                if (user.Data is null) return new(null, 404, "Usuário não encontrado");

                // Deep-copy para não vincular a referência — perfil e usuário ficam independentes
                user.Data.Modules   = profile.Data.Modules
                    .Select(m => new api_slim.src.Models.Module
                    {
                        Code        = m.Code,
                        Description = m.Description,
                        Routines    = m.Routines.Select(r => new api_slim.src.Models.Routine
                        {
                            Code        = r.Code,
                            Description = r.Description,
                            Permissions = new Models.PermissionRoutine
                            {
                                Read   = r.Permissions.Read,
                                Create = r.Permissions.Create,
                                Update = r.Permissions.Update,
                                Delete = r.Permissions.Delete,
                            }
                        }).ToList()
                    }).ToList();

                user.Data.UpdatedAt = DateTime.Now;
                user.Data.UpdatedBy = request.UpdatedBy;

                ResponseApi<User?> updated = await userRepository.UpdateAsync(user.Data);
                if (!updated.IsSuccess) return new(null, 400, "Falha ao aplicar perfil.");
                return new(updated.Data, 200, $"Perfil \"{profile.Data.Name}\" aplicado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }
}
