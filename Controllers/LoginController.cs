using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Services;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PROYEC_QUIMPAC.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly QuimpacContext _quimpacContext;
        private readonly IAuthenticationService _authenticateService;
        private readonly JWTService _jwtService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            IConfiguration configuration,
            QuimpacContext quimpacContext, 
            IAuthenticationService authenticatioService,
            JWTService jwtService,
            ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _quimpacContext = quimpacContext;
            _authenticateService = authenticatioService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLogin user)
        {
            try
            {
                // Validar entrada
                if (user == null || string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
                {
                    _logger.LogWarning("Login attempt with invalid data");
                    return BadRequest(new { message = "Usuario y contraseña son requeridos", error = true });
                }

                // Autenticar usuario
                bool usuarioValido = _authenticateService.Authenticate(user);
                
                if (!usuarioValido)
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", user.username);
                    return Unauthorized(new { message = "Credenciales incorrectas", error = true });
                }

                // Generar token JWT
                var jwtToken = _jwtService.GenerateJSONWebToken(user);

                // Obtener permisos del usuario (usando solo username para la consulta)
                var permiso = from usu in _quimpacContext.Usuarios
                              join rol_per in _quimpacContext.Rol_Permiso
                              on usu.usu_cod_rol equals rol_per.rol_per_cod_rol
                              join per in _quimpacContext.Permiso
                              on rol_per.rol_per_cod_per equals per.per_cod
                              where usu.usu_usu.Equals(user.username)
                              select new { 
                                 per.per_nom,
                                 per.per_uri,
                                 per.per_uri_icon
                              };

                // Obtener datos del usuario (solo username)
                var usuario = from usu in _quimpacContext.Usuarios
                              where usu.usu_usu.Equals(user.username)
                              select new {
                                  usu.usu_cod_usu,
                                  usu.usu_usu,
                                  usu.usu_nom_ape,
                                  usu.usu_cod_rol,
                                  usu.usu_cod_cli,
                                  usu.usu_est
                              };

                _logger.LogInformation("Successful login for user: {Username}", user.username);

                return Ok(new { 
                    message = "Acceso Correcto", 
                    error = false, 
                    data = new { 
                        token = jwtToken, 
                        permiso = permiso.ToList(), 
                        usu = usuario.FirstOrDefault()
                    } 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process for user: {Username}", user?.username ?? "unknown");
                return StatusCode(500, new { 
                    message = "Error interno del servidor", 
                    error = true 
                });
            }
        }
    }
}
