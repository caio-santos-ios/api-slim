using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/customer-contracts")]
    [ApiController]
    public class CustomerContractController(ICustomerContractService customerContractService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await customerContractService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await customerContractService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerContractDTO customer)
        {
            if (customer == null) return BadRequest("Dados inválidos.");
            customer.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<CustomerContract?> response = await customerContractService.CreateAsync(customer);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerContractDTO customer)
        {
            if (customer == null) return BadRequest("Dados inválidos.");

            customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<CustomerContract?> response = await customerContractService.UpdateAsync(customer);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<CustomerContract> response = await customerContractService.DeleteAsync(id, userId);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}