using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/smclick")]
    [ApiController]
    public class SmClickController(ISmClickService service) : ControllerBase
    {
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}