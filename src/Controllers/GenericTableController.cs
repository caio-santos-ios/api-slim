using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/generic-tables")]
    [ApiController]
    public class GenericTableController(IGenericTableService genericTableService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await genericTableService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await genericTableService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpGet("table/{table}")]
        public async Task<IActionResult> GetByTableAsync(string table)
        {
            ResponseApi<List<dynamic>> response = await genericTableService.GetByTableAggregateAsync(table);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("table/all/{table}")]
        public async Task<IActionResult> GetByTableAllAsync(string table)
        {
            System.Console.WriteLine(table);
            ResponseApi<List<dynamic>> response = await genericTableService.GetByTableAllAggregateAsync(table);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGenericTableDTO genericTable)
        {
            if (genericTable == null) return BadRequest("Dados inválidos.");

            ResponseApi<GenericTable?> response = await genericTableService.CreateAsync(genericTable);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateGenericTableDTO genericTable)
        {
            if (genericTable == null) return BadRequest("Dados inválidos.");

            ResponseApi<GenericTable?> response = await genericTableService.UpdateAsync(genericTable);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<GenericTable> response = await genericTableService.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("table/{table}")]
        public async Task<IActionResult> DeleteTable(string table)
        {
            ResponseApi<GenericTable> response = await genericTableService.DeleteByTableAsync(table);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}