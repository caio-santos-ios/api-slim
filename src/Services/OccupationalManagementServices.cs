using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;
using Microsoft.Extensions.Primitives;

namespace api_slim.src.Services
{
    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Micro Checkin
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalMicroCheckinService(IOccupationalMicroCheckinRepository repository, IMapper _mapper) : IOccupationalMicroCheckinService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<OccupationalMicroCheckin> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "Micro Checkin não encontrado");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<OccupationalMicroCheckin?>> CreateAsync(CreateOccupationalMicroCheckinDTO request)
        {
            try
            {
                OccupationalMicroCheckin entity = _mapper.Map<OccupationalMicroCheckin>(request);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<OccupationalMicroCheckin?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Micro Checkin.");
                return new(response.Data, 201, "Micro Checkin criado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<OccupationalMicroCheckin?>> UpdateAsync(UpdateOccupationalMicroCheckinDTO request)
        {
            try
            {
                ResponseApi<OccupationalMicroCheckin?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Micro Checkin não encontrado");

                OccupationalMicroCheckin entity = _mapper.Map<OccupationalMicroCheckin>(request);
                entity.Id = request.Id;
                entity.CustomerId = existing.Data.CustomerId;
                entity.RecipientId = existing.Data.RecipientId;
                entity.CreatedAt = existing.Data.CreatedAt;
                entity.CreatedBy = existing.Data.CreatedBy;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<OccupationalMicroCheckin?> response = await repository.UpdateAsync(entity);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Micro Checkin.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalMicroCheckin>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<OccupationalMicroCheckin> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Bem Vital
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalBemVitalService(IOccupationalBemVitalRepository repository, IMapper _mapper) : IOccupationalBemVitalService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<OccupationalBemVital> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "Bem Vital não encontrado");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<OccupationalBemVital?>> CreateAsync(CreateOccupationalBemVitalDTO request)
        {
            try
            {
                OccupationalBemVital entity = _mapper.Map<OccupationalBemVital>(request);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<OccupationalBemVital?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Bem Vital.");
                return new(response.Data, 201, "Bem Vital criado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<OccupationalBemVital?>> UpdateAsync(UpdateOccupationalBemVitalDTO request)
        {
            try
            {
                ResponseApi<OccupationalBemVital?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Bem Vital não encontrado");

                existing.Data.Department = request.Department;
                existing.Data.Role = request.Role;
                existing.Data.ReferenceDate = request.ReferenceDate;
                existing.Data.Igs = request.Igs;
                existing.Data.Ign = request.Ign;
                existing.Data.Ies = request.Ies;
                existing.Data.Ipv = request.Ipv;
                existing.Data.Notes = request.Notes;
                existing.Data.UpdatedAt = DateTime.Now;
                existing.Data.UpdatedBy = request.UpdatedBy;

                ResponseApi<OccupationalBemVital?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Bem Vital.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalBemVital>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<OccupationalBemVital> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational PGR
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalPgrService(IOccupationalPgrRepository repository, IOccupationalMicroCheckinRepository checkinRepository, IOccupationalBemVitalRepository bemVitalRepository) : IOccupationalPgrService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<OccupationalPgr> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "PGR não encontrado");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region GENERATE
        public async Task<ResponseApi<OccupationalPgr?>> GenerateAsync(GenerateOccupationalPgrDTO request)
        {
            try
            {
                // Busca checkins do período para consolidar métricas
                var checkinQuery = new Dictionary<string, string>
                {
                    { "customerId", request.CustomerId },
                    { "deleted", "false" },
                    { "pageSize", "1000" },
                    { "pageNumber", "1" },
                };
                
                PaginationUtil<OccupationalMicroCheckin> checkinPagination = new(checkinQuery);
                ResponseApi<List<dynamic>> checkinsResp = await checkinRepository.GetAllAsync(checkinPagination);

                // Calcula médias (cast dinâmico simples)
                int total = checkinsResp.Data?.Count ?? 0;
                decimal avgEngagement = 0, avgRisk = 0, avgSafety = 0, avgAbsence = 0, avgEconometer = 0;

                if (total > 0 && checkinsResp.Data is not null)
                {
                    foreach (dynamic c in checkinsResp.Data)
                    {
                        avgEngagement  += (decimal)(c.engagementLevel  ?? 0);
                        avgRisk        += (decimal)(c.riskLevel        ?? 0);
                        avgSafety      += (decimal)(c.safetyPerception ?? 0);
                        avgAbsence     += (decimal)(c.absenceRisk      ?? 0);
                        avgEconometer  += (decimal)(c.econometerScore  ?? 0);
                    }
                    avgEngagement  /= total;
                    avgRisk        /= total;
                    avgSafety      /= total;
                    avgAbsence     /= total;
                    avgEconometer  /= total;
                }

                OccupationalPgr entity = new()
                {
                    CustomerId = request.CustomerId,
                    ReferenceMonth = request.ReferenceMonth,
                    ReferenceYear = request.ReferenceYear,
                    GeneratedAt = DateTime.Now,
                    GeneratedBy = request.CreatedBy,
                    Status = "Gerado",
                    TotalBeneficiaries = total,
                    AvgEngagement = Math.Round(avgEngagement, 2),
                    AvgRisk = Math.Round(avgRisk, 2),
                    AvgSafetyPerception = Math.Round(avgSafety, 2),
                    AvgAbsenceRisk = Math.Round(avgAbsence, 2),
                    AvgEconometer = Math.Round(avgEconometer, 2),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy,
                };

                ResponseApi<OccupationalPgr?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao gerar PGR.");
                return new(response.Data, 201, "PGR gerado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalPgr>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<OccupationalPgr> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }
}
