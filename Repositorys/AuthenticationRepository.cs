using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Repositorys
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly QuimpacContext _quimpacContext;

        public AuthenticationRepository(QuimpacContext quimpacContext)
        {
            _quimpacContext = quimpacContext;
        }
        public Boolean Authenticate(UserLogin userLogin)
        {
            Boolean validado = false;
            var usuario = _quimpacContext.Usuarios;
            
            //string result = string.Empty;

            //byte[] encryted = System.Text.Encoding.Unicode.GetBytes(userLogin.password);
            //result = Convert.ToBase64String(encryted);
            
            if (usuario.Any(x => x.usu_usu.Equals(userLogin.username) && x.usu_cla.Equals(userLogin.password)))
            {
                validado = true;
            }
            return validado;
        }
    }
}
