using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/attachments")]
    [ApiController]
    public class AttachmentController(IAttachmentService attachmentService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await attachmentService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await attachmentService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateAttachmentDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Attachment?> response = await attachmentService.CreateAsync(request);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateAttachmentDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Attachment?> response = await attachmentService.UpdateAsync(request);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Attachment> response = await attachmentService.DeleteAsync(id, userId);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}