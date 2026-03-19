using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using api_slim.src.Shared.Utils;

namespace api_slim.src.Interfaces
{
    // ─── Mass Movement ───────────────────────────────────────────────────────────
    public interface IB2BMassMovementService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BMassMovement?>> CreateAsync(CreateB2BMassMovementDTO request);
        Task<ResponseApi<B2BMassMovement?>> UpdateAsync(UpdateB2BMassMovementDTO request);
        Task<ResponseApi<B2BMassMovement>> DeleteAsync(string id, string userId);
    }

    public interface IB2BMassMovementRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BMassMovement> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BMassMovement?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<B2BMassMovement> pagination);
        Task<ResponseApi<B2BMassMovement?>> CreateAsync(B2BMassMovement entity);
        Task<ResponseApi<B2BMassMovement?>> UpdateAsync(B2BMassMovement entity);
        Task<ResponseApi<B2BMassMovement>> DeleteAsync(string id);
    }

    // ─── Invoice ─────────────────────────────────────────────────────────────────
    public interface IB2BInvoiceService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BInvoice?>> CreateAsync(CreateB2BInvoiceDTO request);
        Task<ResponseApi<B2BInvoice?>> UpdateAsync(UpdateB2BInvoiceDTO request);
        Task<ResponseApi<B2BInvoice>> DeleteAsync(string id, string userId);
    }

    public interface IB2BInvoiceRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BInvoice> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BInvoice?>> GetByIdAsync(string id);
        Task<ResponseApi<B2BInvoice?>> GetByMonthAsync(int month);
        Task<int> GetCountDocumentsAsync(PaginationUtil<B2BInvoice> pagination);
        Task<ResponseApi<B2BInvoice?>> CreateAsync(B2BInvoice entity);
        Task<ResponseApi<B2BInvoice?>> UpdateAsync(B2BInvoice entity);
        Task<ResponseApi<B2BInvoice>> DeleteAsync(string id);
    }

    // ─── Attachment ──────────────────────────────────────────────────────────────
    public interface IB2BAttachmentService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BAttachment?>> CreateAsync(CreateB2BAttachmentDTO request);
        Task<ResponseApi<B2BAttachment?>> UpdateAsync(UpdateB2BAttachmentDTO request);
        Task<ResponseApi<B2BAttachment>> DeleteAsync(string id, string userId);
    }

    public interface IB2BAttachmentRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BAttachment> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<B2BAttachment?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<B2BAttachment> pagination);
        Task<ResponseApi<B2BAttachment?>> CreateAsync(B2BAttachment entity);
        Task<ResponseApi<B2BAttachment?>> UpdateAsync(B2BAttachment entity);
        Task<ResponseApi<B2BAttachment>> DeleteAsync(string id);
    }
}
