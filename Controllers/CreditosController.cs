using System.Security.Claims;
using fenixjobs_api.Application.DTOs.Creditos;
using fenixjobs_api.Application.Interfaces.Creditos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CreditosController : ControllerBase
    {
        private readonly ICreditRequestService _creditRequestService;

        public CreditosController(ICreditRequestService creditRequestService)
        {
            _creditRequestService = creditRequestService;
        }

        [HttpPost("solicitar")]
        public async Task<IActionResult> Create([FromBody] CreateCreditRequestDto dto)
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

            var response = await _creditRequestService.CreateForUserAsync(userId, dto, actorUser);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}