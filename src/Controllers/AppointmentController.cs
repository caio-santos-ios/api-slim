using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialtiesAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetSpecialtiesAllAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpGet("specialty-availability/{specialtyUuid}/{beneficiaryUuid}")]
        public async Task<IActionResult> GetSpecialtyAvailabilityAllAsync(string specialtyUuid, string beneficiaryUuid)
        {
            ResponseApi<List<dynamic>> response = await service.GetSpecialtyAvailabilityAllAsync(specialtyUuid, beneficiaryUuid);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO genericTable)
        {
            if (genericTable == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.CreateAsync(genericTable);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.CancelAsync(id);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}