using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

namespace api_slim.src.Controllers
{
[Route("api/customer-recipients")]
[ApiController]
public class CustomerRecipientController(ICustomerRecipientService service, ICustomerRecipientRepository repository) : ControllerBase
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
    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> GetByCPFAsync(string cpf)
    {
        ResponseApi<dynamic?> response = await service.GetByCPFAggregateAsync(cpf);
        return StatusCode(response.StatusCode, new { response.Result });
    }
    
    [Authorize]
    [HttpGet("rapidoc/{id}")]
    public async Task<IActionResult> GetByRapidocIdAsync(string id)
    {
        ResponseApi<dynamic?> response = await service.GetByRapidocIdAsync(id);
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
    [HttpGet("logged")]
    public async Task<IActionResult> GetLoggedAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(userId!);
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
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerRecipientDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient?> response = await service.UpdateProfileAsync(customer);

        return StatusCode(response.StatusCode, new { response.Result });
    }
    
    [Authorize]
    [HttpPut("profile-photo")]
    public async Task<IActionResult> UpdateProfileApp([FromForm] UpdatePhotoCustomerRecipientDTO customer)
    {
        if (customer == null) return BadRequest("Dados inválidos.");
        customer.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        customer.Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        ResponseApi<CustomerRecipient?> response = await service.UpdateProfilePhotoAsync(customer);

        return StatusCode(response.StatusCode, new { response.Result });
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
    [HttpPut("import")]
    public async Task<IActionResult> Import([FromForm] ImportCustomerRecipientDTO request)
    {
        if (request.File == null || request.File.Length == 0) return BadRequest("Arquivo não selecionado.");

        using (var stream = new MemoryStream())
        {
            await request.File.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet("NEXUS");
                var rows = worksheet.RangeUsed()!.RowsUsed().Skip(1); 
                string holderId = "";
                foreach (var row in rows)
                {
                    string cpf = row.Cell(5).GetValue<string>();
                    var exited = await repository.GetByCPFImportAsync(cpf, request.ContractorId);

                    if(exited.Data is null) 
                    {                        
                        string createdAt = row.Cell(2).GetValue<string>(); 
                        DateTime dateCreatedAt = createdAt.Length == 19 ? DateTime.Parse(createdAt) : DateTime.UtcNow;
                        
                        string deletedAt = row.Cell(2).GetValue<string>(); 
                        DateTime? dateDeletedAt = !string.IsNullOrEmpty(deletedAt) && deletedAt.Length == 19 ? DateTime.Parse(deletedAt) : null;
                        
                        string strDateOfBirth = row.Cell(7).GetValue<string>(); 
                        DateTime? dateOfBirth = !string.IsNullOrEmpty(strDateOfBirth) && strDateOfBirth.Length == 19 ? DateTime.Parse(strDateOfBirth) : null;

                        if(row.Cell(14).GetValue<string>() == "Titular")
                        {
                            var code = await repository.GetNextCodeAsync();

                            CustomerRecipient customer = new ()
                            {
                                Code = code.Data.ToString()!.PadLeft(6, '0'),
                                Active = row.Cell(1).GetValue<string>() == "ATIVO",
                                CreatedAt = dateCreatedAt,
                                DeletedAt = dateDeletedAt,
                                Deleted = row.Cell(1).GetValue<string>() == "INATIVO",
                                Name = row.Cell(4).GetValue<string>(),
                                Cpf = row.Cell(5).GetValue<string>(),
                                Rg = row.Cell(6).GetValue<string>(),
                                DateOfBirth = dateOfBirth,
                                Phone = row.Cell(8).GetValue<string>(),
                                Email = row.Cell(9).GetValue<string>(),
                                Bond = row.Cell(14).GetValue<string>(),
                                EffectiveDate = dateCreatedAt,
                                ContractorId = request.ContractorId
                            };

                            await repository.CreateAsync(customer);
                            holderId = customer.Id;
                        };

                        if(row.Cell(14).GetValue<string>() == "Dependente")
                        {
                            string strDateOfBirthRece = row.Cell(18).GetValue<string>(); 
                            DateTime? dateOfBirthRece = !string.IsNullOrEmpty(strDateOfBirthRece) ? DateTime.Parse(strDateOfBirthRece) : null;
                            var newCode = await repository.GetNextCodeAsync();

                            CustomerRecipient customerRecipient = new ()
                            {
                                Code = newCode.Data.ToString()!.PadLeft(6, '0'),
                                Active = row.Cell(1).GetValue<string>() == "ATIVO",
                                CreatedAt = dateCreatedAt,
                                DeletedAt = dateDeletedAt,
                                Deleted = row.Cell(1).GetValue<string>() == "INATIVO",
                                Name = row.Cell(15).GetValue<string>(),
                                Cpf = row.Cell(16).GetValue<string>(),
                                Rg = row.Cell(17).GetValue<string>(),
                                DateOfBirth = dateOfBirthRece,
                                Phone = row.Cell(19).GetValue<string>(),
                                Email = row.Cell(9).GetValue<string>(),
                                Bond = row.Cell(14).GetValue<string>(),
                                EffectiveDate = dateCreatedAt,
                                HolderId = holderId,
                                ContractorId = request.ContractorId
                            };

                            await repository.CreateAsync(customerRecipient);
                        }
                    }
                }
            }
        }

        return StatusCode(200, new { Message = "Importação feita com sucesso!" });
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