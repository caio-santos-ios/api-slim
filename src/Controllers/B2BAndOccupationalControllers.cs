using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Mass Movement
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/b2b-mass-movements")]
    [ApiController]
    public class B2BMassMovementController(IB2BMassMovementService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateB2BMassMovementDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateB2BMassMovementDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Invoice
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/b2b-invoices")]
    [ApiController]
    public class B2BInvoiceController(IB2BInvoiceService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateB2BInvoiceDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            dto.CustomerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateB2BInvoiceDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Attachment
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/b2b-attachments")]
    [ApiController]
    public class B2BAttachmentController(IB2BAttachmentService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateB2BAttachmentDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            dto.CustomerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateB2BAttachmentDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Micro Checkin
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/occupational-micro-checkins")]
    [ApiController]
    public class OccupationalMicroCheckinController(IOccupationalMicroCheckinService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOccupationalMicroCheckinDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOccupationalMicroCheckinDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Bem Vital
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/occupational-bem-vitals")]
    [ApiController]
    public class OccupationalBemVitalController(IOccupationalBemVitalService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOccupationalBemVitalDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.CreateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOccupationalBemVitalDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.UpdateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational PGR
    // ═══════════════════════════════════════════════════════════════════════════
    [Route("api/occupational-pgr")]
    [ApiController]
    public class OccupationalPgrController(IOccupationalPgrService service) : ControllerBase
    {
        [Authorize][HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateOccupationalPgrDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");
            dto.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.GenerateAsync(dto);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize][HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var response = await service.DeleteAsync(id, userId);
            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}
