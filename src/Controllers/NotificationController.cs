using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        // [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            ResponseApi<dynamic> response = await service.CreateAsync();
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> ReSend(string id)
        {
            ResponseApi<dynamic> response = await service.UpdateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}