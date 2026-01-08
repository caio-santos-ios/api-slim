using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class PlanService(IPlanRepository planRepository, IServiceModuleRepository serviceModuleRepository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IPlanService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Plan> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> plans = await planRepository.GetAllAsync(pagination);
                int count = await planRepository.GetCountDocumentsAsync(pagination);
                return new(plans.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> plan = await planRepository.GetByIdAggregateAsync(id);
                if(plan.Data is null) return new(null, 404, "Plano não encontrado");
                return new(plan.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Plan?>> CreateAsync(CreatePlanDTO request)
        {
            try
            {
                Plan plan = _mapper.Map<Plan>(request);

                if(!string.IsNullOrEmpty(request.ListServiceModuleIds))
                {
                    plan.ServiceModuleIds = request.ListServiceModuleIds.Split(",").Select(x => x).ToList();
                }

                ResponseApi<long?> code = await planRepository.GetNextCodeAsync();
                plan.Code = code.Data.ToString()!.PadLeft(6, '0');

                ResponseApi<Plan?> response = await planRepository.CreateAsync(plan);

                if(request.Image is not null && response.Data is not null)
                {
                    response.Data.Image = await cloudinaryHandler.UploadAttachment("plan", request.Image);
                    await planRepository.UpdateAsync(response.Data);
                }

                if(response.Data is null) return new(null, 400, "Falha ao criar plano.");
                return new(response.Data, 201, "Plano criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Plan?>> UpdateAsync(UpdatePlanDTO request)
        {
            try
            {
                ResponseApi<Plan?> planResponse = await planRepository.GetByIdAsync(request.Id);
                if(planResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Plan plan = _mapper.Map<Plan>(request);
                plan.UpdatedAt = DateTime.UtcNow;
                plan.CreatedAt = planResponse.Data.CreatedAt;
                plan.Image = planResponse.Data.Image;
                plan.Code = planResponse.Data.Code;
                
                if(!string.IsNullOrEmpty(request.ListServiceModuleIds))
                {
                    plan.ServiceModuleIds = request.ListServiceModuleIds.Split(",").Select(x => x).ToList();
                }

                ResponseApi<Plan?> response = await planRepository.UpdateAsync(plan);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                if(request.Image is not null && response.Data is not null)
                {
                    response.Data.Image = await cloudinaryHandler.UploadAttachment("plan", request.Image);
                    await planRepository.UpdateAsync(response.Data);
                }

                // ResponseApi<List<ServiceModule>> lastServiceModule = await serviceModuleRepository.GetByPlanIdAsync(plan.Id);
                // foreach (ServiceModule serviceModule in lastServiceModule.Data!)
                // {
                //     serviceModule.PlanId = "";
                //     serviceModule.UpdatedAt = DateTime.UtcNow;

                //     await serviceModuleRepository.UpdateAsync(serviceModule);
                // };

                // if(!string.IsNullOrEmpty(request.ServiceModuleIds))
                // {
                //     List<string> listId = request.ServiceModuleIds.Split(",").Select(x => x).ToList();
                //     foreach (string serviceModuleId in listId)
                //     {
                //         ResponseApi<ServiceModule?> serviceModule = await serviceModuleRepository.GetByIdAsync(serviceModuleId);
                //         if(serviceModule.Data is not null)
                //         {
                //             serviceModule.Data.PlanId = plan.Id;
                //             serviceModule.Data.UpdatedAt = DateTime.UtcNow;
                            
                //             await serviceModuleRepository.UpdateAsync(serviceModule.Data);
                //         }
                //     };
                // };

                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<Plan?>> UpdateImageAsync(UpdatePlanDTO request)
        {
            try
            {
                Plan plan = new();
                if(!string.IsNullOrEmpty(request.Id))
                {
                    ResponseApi<Plan?> planResponse = await planRepository.GetByIdAsync(request.Id);
                    if(planResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                    
                    planResponse.Data.UpdatedAt = DateTime.UtcNow;
                    planResponse.Data.CreatedAt = planResponse.Data.CreatedAt;
                    planResponse.Data.Image = planResponse.Data.Image;

                    ResponseApi<Plan?> response = await planRepository.UpdateAsync(planResponse.Data);
                    if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                    
                    response.Data!.Image = await cloudinaryHandler.UploadAttachment("plan", request.Image!);
                    await planRepository.UpdateAsync(response.Data);
                };

                if(request.Image is not null)
                {
                    plan.Image = await cloudinaryHandler.UploadAttachment("plan", request.Image);
                };

                return new(plan, 200, "Imagem salva com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Plan>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Plan> plan = await planRepository.DeleteAsync(id);
                if(!plan.IsSuccess) return new(null, 400, "Falha ao excluír Plano");

                ResponseApi<List<ServiceModule>> lastServiceModule = await serviceModuleRepository.GetByPlanIdAsync(plan.Data!.Id);
                foreach (ServiceModule serviceModule in lastServiceModule.Data!)
                {
                    serviceModule.PlanId = "";
                    serviceModule.UpdatedAt = DateTime.UtcNow;

                    await serviceModuleRepository.UpdateAsync(serviceModule);
                };

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