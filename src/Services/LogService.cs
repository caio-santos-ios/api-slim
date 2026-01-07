using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class LogService(ILogRepository logRepository, IMapper _mapper) : ILogService
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Log> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> accountsPayables = await logRepository.GetAllAsync(pagination);
                return new(accountsPayables.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        #endregion
        #region CREATE
        public async Task<ResponseApi<Log?>> CreateAsync(CreateLogDTO request)
        {
            try
            {
                Log log = _mapper.Map<Log>(request);
                log.CreatedAt = DateTime.UtcNow;
                
                ResponseApi<Log?> response = await logRepository.CreateAsync(log);

                return new(null, 201, "Log criado com sucesso.");
            }
            catch
            {                
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
    }
}