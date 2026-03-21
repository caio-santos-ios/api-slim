using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/permission-profiles")]
    [ApiController]
    public class PermissionProfileController(IPermissionProfileService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionProfileDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePermissionProfileDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPut("apply")]
        public async Task<IActionResult> ApplyToUser([FromBody] ApplyPermissionProfileDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.ApplyToUserAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}
