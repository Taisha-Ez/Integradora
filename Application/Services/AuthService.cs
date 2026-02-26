using fenixjobs_api.Application.DTOs;
using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Domain.Documents;
using fenixjobs_api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fenixjobs_api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ISystemLogRepository _logRepository;

        public AuthService(IUserRepository repository, IConfiguration configuration, ISystemLogRepository logRepository)
        {
            _repository = repository;
            _configuration = configuration;
            _logRepository = logRepository;
        }

        public async Task<ServiceResponseDto<Users>> RegisterAsync(RegisterDto dto)
        {
            var response = new ServiceResponseDto<Users>();

            try
            {
                var existingUser = await _repository.GetByUsuarioAsync(dto.Usuario);

                if (existingUser != null)
                {
                    response.Status = false;
                    response.Message = "El nombre de usuario ya está registrado.";
                    return response;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasenia);

                var newUser = new Users
                {
                    nombre = dto.Nombre,
                    apellido_paterno = dto.ApellidoPaterno,
                    apellido_materno = dto.ApellidoMaterno,
                    usuario = dto.Usuario,
                    contrasenia = passwordHash,
                    tipo_usuario = dto.TipoUsuario ?? "cliente"
                };

                await _repository.AddAsync(newUser);

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Register",
                    Email = dto.Usuario,
                    Details = $"Usuario {dto.Usuario} registrado exitosamente como {newUser.tipo_usuario}",
                    CreatedAt = DateTime.UtcNow
                });

                response.Status = true;
                response.Message = "Usuario registrado exitosamente";
                response.Data = newUser;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al procesar el registro: " + ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponseDto<string>> LoginAsync(LoginDto dto)
        {
            var response = new ServiceResponseDto<string>();

            var user = await _repository.GetByUsuarioAsync(dto.Usuario);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Credenciales incorrectas.";
                return response;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Contrasenia, user.contrasenia);

            if (!isPasswordValid)
            {
                response.Status = false;
                response.Message = "Credenciales incorrectas.";
                return response;
            }

            var token = CreateToken(user);

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "Login",
                Email = dto.Usuario,
                Details = "Inicio de sesión exitoso",
                CreatedAt = DateTime.UtcNow
            });

            response.Data = token;
            response.Status = true;
            response.Message = "Login exitoso.";

            return response;
        }

        private string CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, user.nombre),
                new Claim("Usuario", user.usuario),
                new Claim(ClaimTypes.Role, user.tipo_usuario)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}