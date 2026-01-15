using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Responses;
using api_slim.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO user)
        {
            if (user == null) return BadRequest("Dados inválidos");

            ResponseApi<AuthResponse> response = await authService.LoginAsync(user);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [HttpPost]
        [Route("app/login")]
        public async Task<IActionResult> LoginAppAsync([FromBody] LoginAppDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos");

            ResponseApi<AuthAppResponse> response = await authService.LoginAppAsync(request);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            ResponseApi<AuthResponse> response = await authService.RefreshTokenAsync(Request.Headers.Authorization[0]!.Split(" ")[1]);
            return response.IsSuccess ? Ok(new {response.Data}) : BadRequest(new{response.Data, response.Message});
        }
        
        [Authorize]
        [HttpPut]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos");

            ResponseApi<User> response = await authService.ResetPasswordAsync(request);
            return response.IsSuccess ? Ok(new {response.Message}) : BadRequest(new{response.Message});
        }
        
        [HttpPut]
        [Route("request-forgot-password")]
        public async Task<IActionResult> RequestForgotPasswordAsync([FromBody] ForgotPasswordDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos");

            ResponseApi<User> response = await authService.RequestForgotPasswordAsync(request);
            return StatusCode(response.StatusCode, new { response.Message, response.Data });
        }

        [HttpPut]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ResetPasswordDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos");

            ResponseApi<User> response = await authService.ForgotPasswordAsync(request);
            return response.IsSuccess ? Ok(new {response.Message}) : BadRequest(new{response.Message});
        }
    }
}