using Microsoft.AspNetCore.Mvc;
using fenixjobs_api.Application.DTOs.Auth;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.DTOs.Common;

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
            // Only admins can create admin users.
            if (string.Equals(dto.TipoUsuario, "admin", StringComparison.OrdinalIgnoreCase))
            {
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new ServiceResponseDto<string>
                    {
                        Status = false,
                        Message = "Para registrar un usuario admin debes enviar un JWT valido de admin."
                    });
                }

                if (!User.IsInRole("admin"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ServiceResponseDto<string>
                    {
                        Status = false,
                        Message = "Solo un admin puede registrar otros usuarios admin."
                    });
                }
            }

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
