using api_slim.src.Interfaces.Metrics;
using api_slim.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_slim.src.Controllers
{
    [Route("api/metrics")]
    [ApiController]
    public class MetricsController(IMetricsService service) : ControllerBase
    {
        /// <summary>
        /// Cards de resumo: ações hoje / semana / mês e usuários únicos no mês.
        /// </summary>
        [Authorize]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            ResponseApi<dynamic> response = await service.GetSummaryAsync();
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>
        /// Usuários mais ativos nos últimos 30 dias.
        /// Query param: limit (padrão 10).
        /// </summary>
        [Authorize]
        [HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers([FromQuery] int limit = 10)
        {
            ResponseApi<List<dynamic>> response = await service.GetTopUsersAsync(limit);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>
        /// Funcionalidades mais acessadas nos últimos 30 dias.
        /// Query param: limit (padrão 10).
        /// </summary>
        [Authorize]
        [HttpGet("top-features")]
        public async Task<IActionResult> GetTopFeatures([FromQuery] int limit = 10)
        {
            ResponseApi<List<dynamic>> response = await service.GetTopFeaturesAsync(limit);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>
        /// Ações por dia nos últimos N dias.
        /// Query param: days (padrão 30).
        /// </summary>
        [Authorize]
        [HttpGet("timeline")]
        public async Task<IActionResult> GetTimeline([FromQuery] int days = 30)
        {
            ResponseApi<List<dynamic>> response = await service.GetTimelineAsync(days);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}
