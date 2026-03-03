using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class InPersonService(IInPersonRepository repository, IMapper _mapper, IAppointmentNotificationService appointmentNotificationService, ICustomerRecipientRepository customerRecipientRepository) : IInPersonService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<InPerson> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> inPersons = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(inPersons.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> inPerson = await repository.GetByIdAggregateAsync(id);
            if(inPerson.Data is null) return new(null, 404, "Atendimento Presencial não encontrado");
            return new(inPerson.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<InPerson?>> CreateAsync(CreateInPersonDTO request)
    {
        try
        {
            InPerson inPerson = _mapper.Map<InPerson>(request);
            inPerson.Status = "Solicitada";
            ResponseApi<InPerson?> response = await repository.CreateAsync(inPerson);

            if(response.Data is null) return new(null, 400, "Falha ao criar Atendimento Presencial.");

            ResponseApi<CustomerRecipient?> recipientResponse = await customerRecipientRepository.GetByIdAsync(request.RecipientId);
            if(recipientResponse.Data is not null)
            {
                if(string.IsNullOrEmpty(recipientResponse.Data.Whatsapp))
                {
                    return new(null, 400, "O Beneficiário não tem WhatsApp cadastrado. A Consulta foi criada, mas não foi possível enviar a notificação.");
                }

                TimeSpan time = TimeSpan.Parse(request.Hour);
                DateTime? dateTime = request?.Date?.Add(time);
                
                List<NotificationJob> jobs = new()
                {
                    new() {
                        Parent = "InPerson",
                        ParentId = response.Data.Id!,
                        Phone = recipientResponse.Data.Phone,
                        BeneficiaryName = recipientResponse.Data.Name,
                        BeneficiaryCPF = recipientResponse.Data.Cpf,
                        Message = WhatsAppTemplate.InPersonConfirmation(recipientResponse.Data.Name, recipientResponse.Data.Name, request!.ProcedureDescription, request.ProfessionalDescription, request.Date?.ToString("dd/MM/yyyy")!, request.Hour, request.AccreditedDescription, request.AddressDescription),
                        SendDate = DateTime.UtcNow.AddSeconds(30),
                        Type = "Notification"
                    },
                    new() {
                        Parent = "InPerson",
                        ParentId = response.Data.Id!,
                        Phone = recipientResponse.Data.Phone,
                        BeneficiaryName = recipientResponse.Data.Name,
                        BeneficiaryCPF = recipientResponse.Data.Cpf,
                        Message = WhatsAppTemplate.InPersonDayReminder(recipientResponse.Data.Name, recipientResponse.Data.Name, request.ProcedureDescription, request.ProfessionalDescription, request.Date?.ToString("dd/MM/yyyy")!, request.Hour, request.AddressDescription),
                        SendDate = dateTime?.AddDays(-1).AddHours(-1) ?? DateTime.UtcNow.AddSeconds(30),
                        Type = "Notification"
                    },
                    // Notificação de avaliação vou implementar depois.
                };
                await appointmentNotificationService.CreateNotificationsAsync(jobs, Util.CleanPhone(recipientResponse.Data.Whatsapp));
            }

            return new(response.Data, 201, "Atendimento Presencial criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<InPerson?>> UpdateAsync(UpdateInPersonDTO request)
    {
        try
        {
            ResponseApi<InPerson?> inPersonResponse = await repository.GetByIdAsync(request.Id);
            if(inPersonResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            InPerson inPerson = _mapper.Map<InPerson>(request);
            inPerson.UpdatedAt = DateTime.UtcNow;
            inPerson.CreatedAt = inPersonResponse.Data.CreatedAt;

            ResponseApi<InPerson?> response = await repository.UpdateAsync(inPerson);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<InPerson?>> UpdateStatusAsync(UpdateInPersonStatusDTO request)
    {
        try
        {
            ResponseApi<InPerson?> inPersonResponse = await repository.GetByIdAsync(request.Id);
            if(inPersonResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            inPersonResponse.Data.UpdatedAt = DateTime.UtcNow;
            inPersonResponse.Data.Status = request.Status;

            ResponseApi<InPerson?> response = await repository.UpdateAsync(inPersonResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<InPerson>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<InPerson> inPerson = await repository.DeleteAsync(id);
            if(!inPerson.IsSuccess) return new(null, 400, inPerson.Message);
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