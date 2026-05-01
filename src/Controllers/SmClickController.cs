using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPush;
using System.Text.Json;
using api_slim.src.Shared.DTOs;

namespace api_slim.src.Controllers
{
    [Route("api/smclick")]
    [ApiController]
    public class SmClickController(ISmClickService service) : ControllerBase
    {
        [Authorize]
        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationSmClickDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.SendNotificationAsync(request);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}