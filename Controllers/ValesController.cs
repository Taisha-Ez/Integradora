using System.Security.Claims;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Application.Interfaces.Vales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "cliente")]
    public class ValesController : ControllerBase
    {
        private readonly IValeService _valeService;

        public ValesController(IValeService valeService)
        {
            _valeService = valeService;
        }

        [HttpPost("Solicitar")]
        public async Task<IActionResult> Create([FromBody] CreateValeDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    Status = false,
                    Message = "Token invalido: no se encontro el identificador del usuario."
                });
            }

            var response = await _valeService.CreateForClientAsync(userId, dto);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
