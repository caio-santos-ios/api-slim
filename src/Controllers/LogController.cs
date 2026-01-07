using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogController(ILogService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
                
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLogDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<Log?> response = await service.CreateAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}