using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
[Route("api/customer-recipients")]
[ApiController]
public class CustomerRecipientController(ICustomerRecipientService service) : ControllerBase
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
    public async Task<IActionResult> GetSelectAsync()
    {
        ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRecipientDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        customer.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient?> response = await service.CreateAsync(customer);

        return StatusCode(response.StatusCode, new { response.Message });
    }
    
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerRecipientDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient?> response = await service.UpdateAsync(customer);

        return StatusCode(response.StatusCode, new { response.Message });
    }
    
    [Authorize]
    [HttpPut("alter-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateCustomerRecipientDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        
        customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient?> response = await service.UpdateStatusAsync(customer);

        return StatusCode(response.StatusCode, new { response.Message });
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient> response = await service.DeleteAsync(id, userId);

        return StatusCode(response.StatusCode, new { response.Message });
    }
}
}