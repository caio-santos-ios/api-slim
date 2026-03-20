using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
        
        [Authorize]
        [HttpGet("app")]
        public async Task<IActionResult> GetAppAll()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var queryDict = Request.Query.ToDictionary(x => x.Key, x => x.Value);

            queryDict["beneficiaryId"] = new StringValues(userId);

            QueryCollection modifiedQuery = new(queryDict);

            ResponseApi<List<dynamic>> response = await service.GetAllAsync(new(modifiedQuery));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
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

        [Authorize]
        [HttpPut("read/{id}")]
        public async Task<IActionResult> Read(string id)
        {
            ResponseApi<dynamic> response = await service.UpdateReadAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<dynamic> response = await service.DeleteAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}