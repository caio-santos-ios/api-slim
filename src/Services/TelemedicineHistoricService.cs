using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class TelemedicineHistoricService(ITelemedicineHistoricRepository repository, IMapper _mapper) : ITelemedicineHistoricService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<TelemedicineHistoric> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> inPersons = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(inPersons.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<TelemedicineHistoric?>> CreateAsync(CreateTelemedicineHistoricDTO request)
        {
            try
            {
                TelemedicineHistoric inPerson = _mapper.Map<TelemedicineHistoric>(request);
                inPerson.Status = request.Status;
                
                ResponseApi<TelemedicineHistoric?> response = await repository.CreateAsync(inPerson);

                if(response.Data is null) return new(null, 400, "Falha ao criar Atendimento Presencial.");

                return new(response.Data, 201, "Atendimento Presencial criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
    }
}