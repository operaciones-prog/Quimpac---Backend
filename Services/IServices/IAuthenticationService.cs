using PROYEC_QUIMPAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Services.IServices
{
    public interface IAuthenticationService
    {
        public Boolean Authenticate(UserLogin userLogin);
    }
}
