using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        [Authorize]
        [HttpGet("first-card")]
        public async Task<IActionResult> GetCard()
        {
            ResponseApi<dynamic?> response = await service.GetFirstCardAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        [Authorize]
        [HttpGet("recent-patients")]
        public async Task<IActionResult> GetRecentPatient()
        {
            ResponseApi<List<dynamic>> response = await service.GetRecentPatientAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }

        /// <summary>
        /// Retorna dados completos para os novos cards e gráficos do dashboard:
        /// últimas 5 e próximas 5 consultas, KPIs financeiros, série mensal receita x despesa,
        /// distribuição por status e totais.
        /// </summary>
        [Authorize]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var response = await service.GetSummaryAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}
