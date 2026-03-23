using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Interfaces
{
    public interface IAttachmentService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Attachment?>> CreateAsync(CreateAttachmentDTO request);
        Task<ResponseApi<Attachment?>> CreateAllAsync(CreateAttachmentAllDTO request);
        Task<ResponseApi<Attachment?>> UpdateAsync(UpdateAttachmentDTO request);
        Task<ResponseApi<Attachment>> DeleteAsync(string id, string userId);
    }
}