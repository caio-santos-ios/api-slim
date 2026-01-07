using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactController(IContactService contactService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await contactService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await contactService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDTO contact)
        {
            if (contact == null) return BadRequest("Dados inválidos.");
            contact.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Contact?> response = await contactService.CreateAsync(contact);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateContactDTO contact)
        {
            if (contact == null) return BadRequest("Dados inválidos.");
            contact.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Contact?> response = await contactService.UpdateAsync(contact);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            ResponseApi<Contact> response = await contactService.DeleteAsync(id, userId);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}