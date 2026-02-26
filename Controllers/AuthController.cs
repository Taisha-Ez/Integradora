using Microsoft.AspNetCore.Mvc;
using fenixjobs_api.Application.DTOs;
using fenixjobs_api.Application.Interfaces;

namespace fenixjobs_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
