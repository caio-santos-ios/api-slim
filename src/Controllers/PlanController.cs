using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/plans")]
    [ApiController]
    public class PlanController(IPlanService planService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await planService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            ResponseApi<List<dynamic>> response = await planService.GetSelectAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await planService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreatePlanDTO plan)
        {
            if (plan == null) return BadRequest("Dados inválidos.");

            ResponseApi<Plan?> response = await planService.CreateAsync(plan);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdatePlanDTO plan)
        {
            if (plan == null) return BadRequest("Dados inválidos.");

            ResponseApi<Plan?> response = await planService.UpdateAsync(plan);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut("save-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdatePlanDTO plan)
        {
            if (plan == null) return BadRequest("Dados inválidos.");

            ResponseApi<Plan?> response = await planService.UpdateImageAsync(plan);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Plan> response = await planService.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}