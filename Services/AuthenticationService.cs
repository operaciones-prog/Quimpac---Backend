using Microsoft.IdentityModel.Tokens;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Repositorys;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly QuimpacContext _context;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository, 
            IPasswordHashService passwordHashService,
            QuimpacContext context,
            ILogger<AuthenticationService> logger)
        {
            _authenticationRepository = authenticationRepository;
            _passwordHashService = passwordHashService;
            _context = context;
            _logger = logger;
        }

        public Boolean Authenticate(UserLogin userLogin)
        {
            try
            {
                // Buscar usuario en la base de datos
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.usu_usu == userLogin.username);

                if (usuario == null)
                {
                    _logger.LogWarning("Login attempt for non-existent user: {Username}", userLogin.username);
                    return false;
                }

                // Verificar si la contraseña está hasheada o en texto plano
                bool isPasswordValid;
                
                if (_passwordHashService.IsPasswordHashed(usuario.usu_cla))
                {
                    // Contraseña ya hasheada, verificar con BCrypt
                    isPasswordValid = _passwordHashService.VerifyPassword(userLogin.password, usuario.usu_cla);
                }
                else
                {
                    // Contraseña en texto plano (migración gradual)
                    if (usuario.usu_cla == userLogin.password)
                    {
                        // Contraseña correcta, hashear para próximas veces
                        var hashedPassword = _passwordHashService.HashPassword(userLogin.password);
                        usuario.usu_cla = hashedPassword;
                        _context.SaveChanges();
                        
                        _logger.LogInformation("Password migrated to hash for user: {Username}", userLogin.username);
                        isPasswordValid = true;
                    }
                    else
                    {
                        isPasswordValid = false;
                    }
                }

                if (isPasswordValid)
                {
                    _logger.LogInformation("Successful login for user: {Username}", userLogin.username);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", userLogin.username);
                }

                return isPasswordValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for user: {Username}", userLogin.username);
                return false;
            }
        }
    }
}
