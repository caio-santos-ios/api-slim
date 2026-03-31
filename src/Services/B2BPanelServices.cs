using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;
using AutoMapper;

namespace api_slim.src.Services
{
    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Mass Movement
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BMassMovementService(IB2BMassMovementRepository repository, IMapper _mapper) : IB2BMassMovementService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<B2BMassMovement> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "Movimentação não encontrada");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BMassMovement?>> CreateAsync(CreateB2BMassMovementDTO request)
        {
            try
            {
                B2BMassMovement entity = _mapper.Map<B2BMassMovement>(request);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<B2BMassMovement?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Movimentação.");
                return new(response.Data, 201, "Movimentação criada com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BMassMovement?>> UpdateAsync(UpdateB2BMassMovementDTO request)
        {
            try
            {
                ResponseApi<B2BMassMovement?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Movimentação não encontrada");

                B2BMassMovement entity = _mapper.Map<B2BMassMovement>(request);
                entity.CreatedAt = existing.Data.CreatedAt;
                entity.CreatedBy = existing.Data.CreatedBy;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<B2BMassMovement?> response = await repository.UpdateAsync(entity);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Movimentação.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BMassMovement>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<B2BMassMovement> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Invoice
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BInvoiceService(IB2BInvoiceRepository repository, IMapper _mapper) : IB2BInvoiceService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<B2BInvoice> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "Fatura não encontrada");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BInvoice?>> CreateAsync(CreateB2BInvoiceDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CustomerId))
                {
                    return new(null, 400, "CustomerId é obrigatório.");
                }
                bool exists = await repository.ExistsAsync(
                request.ReferenceMonth,
                request.ReferenceYear,
                request.CustomerId
        );

        if (exists)
        {
            return new(null, 200, "Fatura já existente.");
        }
                B2BInvoice entity = _mapper.Map<B2BInvoice>(request);
                // ── Auto-calcula closingDate: último dia do mês de referência ──
                entity.ClosingDate = request.GetClosingDate();
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<B2BInvoice?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Fatura.");
                return new(response.Data, 201, "Fatura criada com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BInvoice?>> UpdateAsync(UpdateB2BInvoiceDTO request)
        {
            try
            {
                ResponseApi<B2BInvoice?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Fatura não encontrada");

                existing.Data.Status = request.Status;
                existing.Data.TotalAmount = request.TotalAmount;
                existing.Data.DueDate = request.DueDate;
                existing.Data.PaidAt = request.PaidAt;
                existing.Data.Items = request.Items;
                existing.Data.UpdatedAt = DateTime.Now;
                existing.Data.UpdatedBy = request.UpdatedBy;
                existing.Data.BeneficiaryCount = request.BeneficiaryCount;

                ResponseApi<B2BInvoice?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Fatura.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BInvoice>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<B2BInvoice> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Attachment
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BAttachmentService(IB2BAttachmentRepository repository, IMapper _mapper) : IB2BAttachmentService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<B2BAttachment> pagination = new(request.QueryParams);
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
                if (data.Data is null) return new(null, 404, "Anexo não encontrado");
                return new(data.Data);
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BAttachment?>> CreateAsync(CreateB2BAttachmentDTO request)
        {
            try
            {
              B2BAttachment entity = _mapper.Map<B2BAttachment>(request);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                ResponseApi<B2BAttachment?> response = await repository.CreateAsync(entity);
                if (response.Data is null) return new(null, 400, "Falha ao criar Anexo.");
                return new(response.Data, 201, "Anexo criado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BAttachment?>> UpdateAsync(UpdateB2BAttachmentDTO request)
        {
            try
            {
                ResponseApi<B2BAttachment?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Anexo não encontrado");

                existing.Data.Name = request.Name;
                existing.Data.FileUrl = request.FileUrl;
                existing.Data.FileName = request.FileName;
                existing.Data.FileType = request.FileType;
                existing.Data.FileSize = request.FileSize;
                existing.Data.Required = request.Required;
                existing.Data.Notes = request.Notes;
                existing.Data.UpdatedAt = DateTime.Now;
                existing.Data.UpdatedBy = request.UpdatedBy;

                ResponseApi<B2BAttachment?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Anexo.");
                return new(response.Data, 200, "Atualizado com sucesso.");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BAttachment>> DeleteAsync(string id, string userId)
        {
            try
            {
                ResponseApi<B2BAttachment> response = await repository.DeleteAsync(id);
                if (!response.IsSuccess) return new(null, 400, response.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); }
        }
        #endregion
    }
}
