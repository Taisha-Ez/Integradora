using System.Security.Claims;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Application.Interfaces.Vales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValesController : ControllerBase
    {
        private static readonly Dictionary<string, string?> StatusMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Todos"] = null,
            ["Pendiente"] = "Pendiente",
            ["Pendientes"] = "Pendiente",
            ["Aceptado"] = "Aceptado",
            ["Aceptados"] = "Aceptado",
            ["Rechazado"] = "Rechazado",
            ["Rechazados"] = "Rechazado"
        };

        private readonly IValeService _valeService;

        public ValesController(IValeService valeService)
        {
            _valeService = valeService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [HttpGet("{status}")]
        public async Task<IActionResult> GetAll([FromRoute] string? status = null, [FromQuery(Name = "status")] string? queryStatus = null)
        {
            var effectiveStatus = status ?? queryStatus;

            if (!string.IsNullOrWhiteSpace(effectiveStatus) && !StatusMap.ContainsKey(effectiveStatus))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Status invalido. Usa 'Todos', 'Pendientes', 'Aceptados' o 'Rechazados'."
                });
            }

            effectiveStatus = string.IsNullOrWhiteSpace(effectiveStatus)
                ? null
                : StatusMap[effectiveStatus];

            var actorUser = User.FindFirstValue("Usuario") ?? User.FindFirstValue(ClaimTypes.Name);
            var response = await _valeService.GetAllAsync(effectiveStatus, actorUser);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "cliente")]
        [HttpPost("Solicitar")]
        public async Task<IActionResult> Create([FromBody] CreateValeDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var actorUser = User.FindFirstValue("Usuario") ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    Status = false,
                    Message = "Token invalido: no se encontro el identificador del usuario."
                });
            }

            var response = await _valeService.CreateForClientAsync(userId, dto, actorUser);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
