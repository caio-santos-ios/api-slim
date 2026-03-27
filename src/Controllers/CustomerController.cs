using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        PaginationApi<List<dynamic>> response = await customerService.GetAllAsync(new(Request.Query));
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpGet("select")]
    public async Task<IActionResult> GetSelect()
    {
        ResponseApi<List<dynamic>> response = await customerService.GetSelectAsync(new(Request.Query));
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        ResponseApi<dynamic?> response = await customerService.GetByIdAggregateAsync(id);
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        customer.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        ResponseApi<Customer?> response = await customerService.CreateAsync(customer);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerDTO customer)
        {
            if (customer == null) return BadRequest("Dados inválidos.");
            customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Customer?> response = await customerService.UpdateAsync(customer);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut("painel-access")]
        public async Task<IActionResult> UpdatePainelAccess([FromBody] UpdateCustomerDTO customer)
        {
            if (customer == null) return BadRequest("Dados inválidos.");
            customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Customer?> response = await customerService.UpdatePainelAccessAsync(customer);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Customer> response = await customerService.DeleteAsync(id, userId);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}