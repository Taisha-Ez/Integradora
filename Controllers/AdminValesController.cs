using System.Security.Claims;
using fenixjobs_api.Application.Interfaces.Vales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/admin/vales")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminValesController : ControllerBase
    {
        private static readonly Dictionary<string, string?> StatusMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Todos"] = null,
            ["Pendiente"] = "Pendiente",
            ["Pendientes"] = "Pendiente",
            ["Aceptado"] = "Aceptado",
            ["Aceptados"] = "Aceptado",
            ["Rechazado"] = "Rechazado",
            ["Rechazados"] = "Rechazado",
            ["Pagado"] = "Pagado",
            ["Pagados"] = "Pagado"
        };

        private readonly IValeService _valeService;

        public AdminValesController(IValeService valeService)
        {
            _valeService = valeService;
        }

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
                    Message = "Status invalido. Usa 'Todos', 'Pendientes', 'Aceptados', 'Rechazados' o 'Pagados'."
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
    }
}