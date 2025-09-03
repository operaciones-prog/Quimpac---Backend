using PROYEC_QUIMPAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Repositorys.IRepository
{
    public interface IAuthenticationRepository
    {
        public Boolean Authenticate(UserLogin userLogin);
    }
}
