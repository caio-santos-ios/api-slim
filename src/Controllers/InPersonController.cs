using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/in-persons")]
    [ApiController]
    public class InPersonController(IInPersonService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            ResponseApi<List<dynamic>> response = await service.GetTotalAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInPersonDTO accreditedNetwork)
        {
            if (accreditedNetwork == null) return BadRequest("Dados inválidos.");

            ResponseApi<InPerson?> response = await service.CreateAsync(accreditedNetwork);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateInPersonDTO accreditedNetwork)
        {
            if (accreditedNetwork == null) return BadRequest("Dados inválidos.");

            ResponseApi<InPerson?> response = await service.UpdateAsync(accreditedNetwork);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut("alter-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateInPersonStatusDTO accreditedNetwork)
        {
            if (accreditedNetwork == null) return BadRequest("Dados inválidos.");

            ResponseApi<InPerson?> response = await service.UpdateStatusAsync(accreditedNetwork);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<InPerson> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}