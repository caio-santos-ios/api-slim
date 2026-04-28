using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class ProfessionalService(IProfessionalRepository professionalRepository, IAddressRepository addressRepository, IMapper _mapper) : IProfessionalService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Professional> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> professionals = await professionalRepository.GetAllAsync(pagination);
                int count = await professionalRepository.GetCountDocumentsAsync(pagination);
                return new(professionals.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Professional> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> professionals = await professionalRepository.GetSelectAsync(pagination);
                return new(professionals.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> professional = await professionalRepository.GetByIdAggregateAsync(id);
                if (professional.Data is null) return new(null, 404, "Profissional não encontrado");
                return new(professional.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Professional?>> CreateAsync(CreateProfessionalDTO request)
        {
            try
            {
                Professional professional = _mapper.Map<Professional>(request);
                ResponseApi<Professional?> response = await professionalRepository.CreateAsync(professional);

                request.Address.Parent = "professional";
                request.Address.ParentId = response.Data!.Id;

                Address address = _mapper.Map<Address>(request.Address);
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if (!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Profissional.");

                if (response.Data is null) return new(null, 400, "Falha ao criar Profissional.");
                return new(response.Data, 201, "Profissional criado com sucesso.");
            }
            catch
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Professional?>> UpdateAsync(UpdateProfessionalDTO request)
        {
            try
            {
                ResponseApi<Professional?> professionalResponse = await professionalRepository.GetByIdAsync(request.Id);
                if (professionalResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                Professional professional = _mapper.Map<Professional>(request);
                professional.UpdatedAt = DateTime.UtcNow;

                ResponseApi<Professional?> response = await professionalRepository.UpdateAsync(professional);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if (!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar Profissional.");

                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Professional>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Professional> professional = await professionalRepository.DeleteAsync(id);
                if (!professional.IsSuccess) return new(null, 400, professional.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}