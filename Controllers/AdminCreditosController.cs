using System.Security.Claims;
using fenixjobs_api.Application.Interfaces.Creditos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/admin/creditos")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminCreditosController : ControllerBase
    {
        private readonly ICreditRequestService _creditRequestService;

        public AdminCreditosController(ICreditRequestService creditRequestService)
        {
            _creditRequestService = creditRequestService;
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> GetClientsWithCredit()
        {
            var actorUser = User.FindFirstValue("Usuario") ?? User.FindFirstValue(ClaimTypes.Name);
            var response = await _creditRequestService.GetClientsWithCreditAsync(actorUser);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}