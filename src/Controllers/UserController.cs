using System.Security.Claims;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            PaginationApi<List<dynamic>> response = await userService.GetAllAsync(new(Request.Query), userId);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await userService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await userService.CreateAsync(user);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await userService.UpdateAsync(user);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPut("modules")]
        public async Task<IActionResult> UpdateModules([FromBody] UpdateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await userService.UpdateModuleAsync(user);

            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [HttpPut("code-access")]
        public async Task<IActionResult> ResendCodeAccess([FromBody] UpdateUserDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await userService.ResendCodeAccessAsync(user);

            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }
        
        [HttpPut("confirm-access")]
        public async Task<IActionResult> ValidatedAccessAsync([FromBody] User user)
        {
            if (user == null) return BadRequest("Dados inválidos.");

            ResponseApi<User?> response = await userService.ValidatedAccessAsync(user.CodeAccess);
            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }

        [Authorize]
        [HttpPut("profile-photo")]
        public async Task<IActionResult> SavePhotoProfileAsync([FromForm] SaveUserPhotoDTO user)
        {
            ResponseApi<User?> response = await userService.SavePhotoProfileAsync(user);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("remove-profile-photo/{id}")]
        public async Task<IActionResult> RemovePhotoProfileAsync(string id)
        {
            ResponseApi<User?> response = await userService.RemovePhotoProfileAsync(id);
            return response.IsSuccess ? Ok(new{response.Message}) : BadRequest(new{response.Message});
        }
        
        [Authorize]
        [HttpGet("logged")]
        public async Task<IActionResult> GetLoggedAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var email = User.FindFirst(ClaimTypes.Email)?.Value;
            // var role = User.FindFirst(ClaimTypes.Role)?.Value;

            ResponseApi<dynamic?> response = await userService.GetByIdAggregateAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<User> response = await userService.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}