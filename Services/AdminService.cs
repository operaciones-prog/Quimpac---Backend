using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Services
{
    public class AdminService : IAdminService
    {
        private IAdminRepository _adminRepository;
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public string actualizarCli(Clientes cli)
        {
            return _adminRepository.actualizarCli(cli);
        }

        public string actualizarPermiso(Permiso per)
        {
            return _adminRepository.actualizarPermiso(per);
        }

        public string actualizarRol(Rol rl)
        {
            return _adminRepository.actualizarRol(rl);
        }

        public Chofer buscarChofer(string cod_sap)
        {
            return _adminRepository.buscarChofer(cod_sap);
        }

        public Clientes buscarCliente(string cod_sap)
        {
            return _adminRepository.buscarCliente(cod_sap);
        }

        public Permiso buscarPermiso(int cod_per)
        {
            return _adminRepository.buscarPermiso(cod_per);
        }

        public Rol buscarRol(int cod_rol)
        {
            return _adminRepository.buscarRol(cod_rol);
        }

        public List<Rol> buscarRolXsap(string cod_sap)
        {
            return _adminRepository.buscarRolXsap(cod_sap);
        }

        public List<UsuarioXrol> buscarUsuario(string cod_sap , string cod_soc)
        {
            return _adminRepository.buscarUsuario(cod_sap,cod_soc);
        }

        public Placa buscarVehiculo(string placa)
        {
            return _adminRepository.buscarVehiculo(placa);
        }

        public string crearCliente(Clientes cli)
        {
            return _adminRepository.crearCliente(cli);
        }

        public string crearPermiso(Permiso per)
        {
            return _adminRepository.crearPermiso(per);
        }

        public List<Chofer> listarChofer()
        {
            return _adminRepository.listarChofer();
        }

        public List<Clientes> listarClientes(string cli_org_ven)
        {
            return _adminRepository.listarClientes(cli_org_ven);
        }

        public List<Permiso> listarPermiso()
        {
            return _adminRepository.listarPermiso();
        }

        public List<Rol> listarRoles()
        {
            return _adminRepository.listarRoles();
        }

        public List<Usuarios> listarUsuario()
        {
            return _adminRepository.listarUsuario();
        }

        public List<Placa> listarVehiculo()
        {
            return _adminRepository.listarVehiculo();
        }

        public List<Permiso> ListarXestado(string estado)
        {
            return _adminRepository.ListarXestado(estado);
        }

        public List<Resumen_stock_Cliente> listaStockCliente()
        {
            return _adminRepository.listaStockCliente();
        }

        public string newChofer(Chofer cho)
        {
            return _adminRepository.newChofer(cho);
        }

        public string newRol(JObject json)
        {
            return _adminRepository.newRol(json);
        }

        public string newUsuario(Usuarios nu)
        {
            return _adminRepository.newUsuario(nu);
        }

        public string newVehiculo(Placa veh)
        {
            return _adminRepository.newVehiculo(veh);
        }

        public void SendEmailAsync(string email, string subject, string message)
        {
            _adminRepository.SendEmailAsync(email, subject, message);
        }

        public string updateChofer(Chofer cho)
        {
            return _adminRepository.updateChofer(cho);
        }

        public string updateUsuario(Usuarios un)
        {
            return _adminRepository.updateUsuario(un);
        }

        public string updateVehiculo(Placa veh)
        {
            return _adminRepository.updateVehiculo(veh);
        }
    }
}
