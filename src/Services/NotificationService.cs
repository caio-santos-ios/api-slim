using api_slim.src.Handlers;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Services
{
    public class NotificationService(INotificationRepository repository, ICustomerRecipientRepository customerRecipientRepository, SmClickHandler smClickHandler) : INotificationService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<NotificationJob> pagination = new(request.QueryParams);

                ResponseApi<List<dynamic>> notifications = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(notifications.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetListAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<NotificationJob> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> notifications = await repository.GetListAsync(pagination);
                return new(notifications.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<dynamic>> CreateAsync()
        {
            try
            {
                PaginationUtil<CustomerRecipient> pagination = new(new Dictionary<string, string>(){{"deleted", "false"}});
                ResponseApi<List<dynamic>> list = await customerRecipientRepository.GetAllAsync(pagination);
                
                if(list.Data is not null)
                {
                    foreach (var item in list.Data)
                    {
                        // if(item.cpf != "086.306.285-70") continue;

                        ResponseApi<NotificationJob> notificationWelcome = await repository.GetByTypeAsync(item.cpf, "Welcome");

                        DateTime today = DateTime.Now.Date.AddHours(9);

                        if(notificationWelcome.Data is null)
                        {
                            await repository.CreateAsync(new ()
                            {
                                BeneficiaryCPF = item.cpf,
                                BeneficiaryId = item.id,
                                BeneficiaryName = item.name,
                                Message = WhatsAppTemplate.Welcome(item.name),
                                Origin = "WhatsApp",
                                Parent = "CustomerRecipient",
                                ParentId = item.id,
                                Phone = item.whatsapp,
                                Title = "Boas Vindas",
                                SendDate = today.AddDays(1),
                                Sent = false,
                                Type = "Welcome"
                            });
                        }

                        ResponseApi<NotificationJob> notificationInstalation = await repository.GetByTypeAsync(item.cpf, "InstalationApp");
                        if(notificationInstalation.Data is null)
                        {
                            await repository.CreateAsync(new ()
                            {
                                BeneficiaryCPF = item.cpf,
                                BeneficiaryId = item.id,
                                BeneficiaryName = item.name,
                                Message = WhatsAppTemplate.AppDownloadInstructions(),
                                Origin = "WhatsApp",
                                Parent = "CustomerRecipient",
                                ParentId = item.id,
                                Phone = item.whatsapp,
                                Title = "Instruções pra instalar APP",
                                SendDate = today.AddDays(1).AddSeconds(30),
                                Sent = false,
                                Type = "InstalationApp"
                            });
                        }
                    }
                }

                // await repository.CreateAsync(request);
                return new(null, 201, "Notificação criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<dynamic>> UpdateAsync(string id)
        {
            try
            {
                ResponseApi<NotificationJob?> notification = await repository.GetByIdAsync(id);
                if(notification.Data is not null)
                {
                    await smClickHandler.SendTextMessageAsync(notification.Data.Phone, notification.Data.Message);

                    notification.Data.Sent = true;
                    notification.Data.SendDate = DateTime.Now;

                    await repository.UpdateAsync(notification.Data);
                }

                return new(null, 200, "Notificação reenviada");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic>> UpdateReadAsync(string id)
        {
            try
            {
                ResponseApi<NotificationJob?> notification = await repository.GetByIdAsync(id);

                if(notification.Data is not null)
                {
                    notification.Data.Read = true;
                    await repository.UpdateAsync(notification.Data);
                }

                return new(null, 200, "Notificação reenviada");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<dynamic>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<NotificationJob?> notification = await repository.GetByIdAsync(id);

                if(notification.Data is not null)
                {
                    notification.Data.Deleted = true;

                    await repository.UpdateAsync(notification.Data);
                }

                return new(null, 200, "Notificação excluída");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}