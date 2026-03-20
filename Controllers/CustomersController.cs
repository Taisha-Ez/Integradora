using fenixjobs_api.Application.Interfaces.Customers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fenixjobs_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CustomersController : ControllerBase
    {
        private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Cumplidos",
            "Morosos"
        };

        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [HttpGet("{type}")]
        public async Task<IActionResult> GetAll([FromRoute] string? type = null, [FromQuery] string? queryType = null)
        {
            var effectiveType = type ?? queryType;

            if (!string.IsNullOrWhiteSpace(effectiveType) && !AllowedTypes.Contains(effectiveType))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Tipo invalido. Usa 'Cumplidos' o 'Morosos'."
                });
            }

            var actorUser = User.FindFirstValue("Usuario") ?? User.FindFirstValue(ClaimTypes.Name);
            var response = await _customerService.GetAllAsync(effectiveType, actorUser);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
