using System.Security.Claims;
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
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("v2")]
        public async Task<IActionResult> GetAllV2()
        {
            ResponseApi<List<dynamic>> response = await service.GetAllV2Async(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("user/{beneficiaryUuid}")]
        public async Task<IActionResult> GetByUser(string beneficiaryUuid)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAsync(beneficiaryUuid);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialtiesAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetSpecialtiesAllAsync();
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("specialty-availability/{specialtyUuid}/{beneficiaryUuid}")]
        public async Task<IActionResult> GetSpecialtyAvailabilityAllAsync(string specialtyUuid, string beneficiaryUuid)
        {
            ResponseApi<List<dynamic>> response = await service.GetSpecialtyAvailabilityAllAsync(specialtyUuid, beneficiaryUuid);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            ResponseApi<dynamic?> response = await service.CreateAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelAsync([FromBody] CancelForwardingDTO request)
        {
            request.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<dynamic?> response = await service.CancelAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}