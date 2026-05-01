using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    public class CustomerContractService(ICustomerContractRepository customerContractRepository, IAccountsReceivableService accountsReceivableService, IMapper _mapper, ILogRepository logRepository) : ICustomerContractService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<CustomerContract> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customerContracts = await customerContractRepository.GetAllAsync(pagination);
                int count = await customerContractRepository.GetCountDocumentsAsync(pagination);
                return new(customerContracts.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> customerContract = await customerContractRepository.GetByIdAggregateAsync(id);
                if(customerContract.Data is null) return new(null, 404, "Contrato não encontrado");
                return new(customerContract.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<CustomerContract?>> CreateAsync(CreateCustomerContractDTO request)
        {
            try
            {
                CustomerContract customerContract = _mapper.Map<CustomerContract>(request);

                ResponseApi<long> code = await customerContractRepository.GetNextCodeAsync();
                if(!code.IsSuccess) return new(null, 400, "Falha ao criar Contrato.");
                
                customerContract.Code = (code.Data + 1).ToString().PadLeft(6, '0');
                customerContract.CreatedAt = DateTime.UtcNow;
                customerContract.UpdatedAt = DateTime.UtcNow;   
                customerContract.SaleDate = DateTime.UtcNow;

                ResponseApi<CustomerContract?> response = await customerContractRepository.CreateAsync(customerContract);

                if(response.Data is null) return new(null, 400, "Falha ao criar Contrato.");

                if(request.Type == "Avulsos")
                {
                    if(request.PaymentCondition == "À Vista") 
                    {
                        CreateAccountsReceivableDTO accountsReceivable = new ()
                        {
                            ContractId = customerContract.Id,
                            Value = customerContract.Total,
                            LowDate = null,
                            LowValue = 0,
                            CustomerId = customerContract.ContractorId
                        };

                        await accountsReceivableService.CreateAsync(accountsReceivable);
                    }
                    else 
                    {
                        decimal valueParc = request.Total / request.PaymentInstallmentQuantity;
                        DateTime? dueDate = request.DueDate;

                        for (int i = 0; i < request.PaymentInstallmentQuantity; i++)
                        {
                            DateTime? currentBilling = NextBillingDate(request.BillingPeriod, dueDate, request.Billing);

                            CreateAccountsReceivableDTO accountsReceivable = new ()
                            {
                                ContractId = customerContract.Id,
                                Value = valueParc,
                                LowDate = null,
                                LowValue = 0,
                                CustomerId = customerContract.ContractorId,
                                DueDate = dueDate,
                                BillingDate = currentBilling
                            };

                            await accountsReceivableService.CreateAsync(accountsReceivable);

                            if(dueDate is not null) 
                            {
                                dueDate = NextDueDate(request.BillingPeriod, dueDate);
                            }
                        }
                    };
                }
                else
                {
                    DateTime? dueDate = request.DueDate;
                    while(request.EndRecurrence > request.SaleDate) 
                    {
                        DateTime? currentBilling = NextBillingDate(request.BillingPeriod, dueDate, request.Billing);

                        CreateAccountsReceivableDTO accountsReceivable = new ()
                        {
                            ContractId = customerContract.Id,
                            Value = customerContract.Total,
                            LowDate = null,
                            LowValue = 0,
                            CustomerId = customerContract.ContractorId,
                            DueDate = dueDate,
                            BillingDate = currentBilling
                        };

                        await accountsReceivableService.CreateAsync(accountsReceivable);

                        request.SaleDate = request.SaleDate.Value.AddMonths(1);
                        if(dueDate is not null) 
                        {
                            dueDate = NextDueDate(request.BillingPeriod, dueDate);
                        };
                    }
                };

                await logRepository.CreateAsync(new()
                {   
                    Action = "Criação",
                    Collection = "customer-contract",
                    Description = $"Criação Contrato Nº {response.Data.Code}",
                    CreatedBy = request.CreatedBy,
                    Parent = "customer",
                    ParentId = response.Data.ContractorId                 
                });

                return new(response.Data, 201, "Contrato criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion    
        #region UPDATE
        public async Task<ResponseApi<CustomerContract?>> UpdateAsync(UpdateCustomerContractDTO request)
        {
            try
            {
                ResponseApi<CustomerContract?> customerContractResponse = await customerContractRepository.GetByIdAsync(request.Id);
                if(customerContractResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                CustomerContract customerContract = _mapper.Map<CustomerContract>(request);
                customerContract.UpdatedAt = DateTime.UtcNow;
                customerContract.CreatedAt = customerContractResponse.Data.CreatedAt;
                customerContract.Code = customerContractResponse.Data.Code;

                ResponseApi<CustomerContract?> response = await customerContractRepository.UpdateAsync(customerContract);
                if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

                if(request.Type == "Avulsos") 
                {
                    ResponseApi<List<AccountsReceivable>> list = await accountsReceivableService.GetByContractId(customerContract.Id);
                    if(!list.IsSuccess || list.Data is null) return new(null, 400, "Falha ao atualizar");

                    foreach (AccountsReceivable account in list.Data)
                    {
                        UpdateAccountsReceivableDTO accountsReceivable = new ()
                        {
                            ContractId = customerContract.Id,
                            Value = customerContract.Total,
                            LowDate = account.LowDate,
                            LowValue = account.LowValue,
                            CustomerId = customerContract.ContractorId,
                            Id = account.Id,
                            Code = account.Code
                        };

                        await accountsReceivableService.UpdateAsync(accountsReceivable);
                    }
                    
                };

                await logRepository.CreateAsync(new()
                {   
                    Action = "Atualização",
                    Collection = "customer-contract",
                    Description = $"Atualização Contrato Nº {response.Data.Code}",
                    CreatedBy = request.CreatedBy,
                    Parent = "customer",
                    ParentId = response.Data.ContractorId                 
                });
                
                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<CustomerContract>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<CustomerContract> customerContract = await customerContractRepository.DeleteAsync(id);
                if(!customerContract.IsSuccess || customerContract.Data is null) return new(null, 400, customerContract.Message);

                await logRepository.CreateAsync(new()
                {   
                    Action = "Exclusão",
                    Collection = "customer-contract",
                    Description = $"Exclusão Contrato Nº {customerContract.Data.Code}",
                    CreatedBy = userId,
                    Parent = "customer",
                    ParentId = customerContract.Data.ContractorId                 
                });

                return new(null, 204, "Excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion 
        #region FUNCTION
        public static DateTime? NextDueDate(string billingPeriod, DateTime? currentDueDate)
        {
            if (currentDueDate == null) return null;

            return billingPeriod switch
            {
                "Mensal" => currentDueDate.Value.AddMonths(1),
                "Semanal" => currentDueDate.Value.AddDays(7),
                "Anual" => currentDueDate.Value.AddYears(1),
                _ => currentDueDate
            };
        }
        public static DateTime? NextBillingDate(string billingPeriod, DateTime? currentDueDate, string billingDay)
        {
            if (currentDueDate == null || string.IsNullOrEmpty(billingDay)) return currentDueDate;

            switch (billingPeriod)
            {
                case "Mensal":
                    int day = Convert.ToInt32(billingDay);
                    int lastDayOfMonth = DateTime.DaysInMonth(currentDueDate.Value.Year, currentDueDate.Value.Month);
                    int safeDay = Math.Min(day, lastDayOfMonth);
                    
                    return new DateTime(currentDueDate.Value.Year, currentDueDate.Value.Month, safeDay);

                case "Anual":
                    var spl = billingDay.Split("-");
                    int lastDayOfAnualMonth = DateTime.DaysInMonth(Convert.ToInt32(spl[0]), Convert.ToInt32(spl[1]));
                    return new DateTime(currentDueDate.Value.Year, currentDueDate.Value.Month, Math.Min(Convert.ToInt32(spl[2]), lastDayOfAnualMonth));

                case "Semanal":
                    return currentDueDate;

                default:
                    return currentDueDate;
            }
        }
        #endregion
    }
}