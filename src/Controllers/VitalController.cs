using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/vitals")]
    [ApiController]
    public class VitalController(IVitalService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("beneficiary")]
        public async Task<IActionResult> GetByBeneficiaryIdAsync()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            ResponseApi<Vital?> response = await service.GetByBeneficiaryIdAsync(userId);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVitalDTO vital)
        {
            if (vital == null) return BadRequest("Dados inválidos.");
            vital.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            vital.BeneficiaryId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Vital?> response = await service.CreateAsync(vital);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateVitalDTO vital)
        {
            if (vital == null) return BadRequest("Dados inválidos.");
            vital.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Vital?> response = await service.UpdateAsync(vital);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Vital> response = await service.DeleteAsync(id, userId);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}