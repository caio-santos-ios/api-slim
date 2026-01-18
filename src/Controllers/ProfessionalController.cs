using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/professionals")]
[ApiController]
public class ProfessionalController(IProfessionalService service) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
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
    [HttpGet("select")]
    public async Task<IActionResult> GetSelect()
    {
        ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfessionalDTO professional)
    {
        if (professional == null) return BadRequest("Dados inválidos.");

        ResponseApi<Professional?> response = await service.CreateAsync(professional);

        return StatusCode(response.StatusCode, new { response.Message });
    }
    
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfessionalDTO professional)
    {
        if (professional == null) return BadRequest("Dados inválidos.");

        ResponseApi<Professional?> response = await service.UpdateAsync(professional);

        return StatusCode(response.StatusCode, new { response.Message });
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        ResponseApi<Professional> response = await service.DeleteAsync(id);

        return StatusCode(response.StatusCode, new { response.Message });
    }
}
}