using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PROYEC_QUIMPAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly QuimpacContext _quimpacContext;
        private IAdminService _adminService;
        public AdminController(QuimpacContext quimpacContext , IAdminService adminService)
        {
            _quimpacContext = quimpacContext;
            _adminService = adminService;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------TABLA USUARIOS---------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------

        //SELECIONAR TODOS LOS USUARIOS
        [HttpGet("usu")]
        public ActionResult usuarios()
        {
            try
            {
                return Ok(_adminService.listarUsuario());
            }
            catch
            {
                return Ok("No se listo los usuarios");
            }
            
        }

        //CREAR NUEVO USUARIO
        [HttpPost("newusu")]
        public ActionResult newusu([FromBody] Usuarios nu)
        {
            IActionResult rpta = Unauthorized();
            string mensaje = "";
            string tipoMensaje = "S";
            string msj = "";
            try
            {
                mensaje = _adminService.newUsuario(nu);
                if (mensaje.Equals("administrador"))
                {
                    msj = "se guardo correctamente un administrador del cliente";
                    rpta = Ok(new { messaje = "Se creo correctamente un Administrador del cliente", error = false, data = new { msj = tipoMensaje , usuario = nu} });
                }
               
                else
                if (mensaje.Equals("usuario"))
                {
                    msj = "se guardo correctamente un usuario";
                    rpta = Ok(new { messaje = "Se creo correctamente el usuario", error = false, data = new { msj = tipoMensaje, usuario = nu } });
                }else if (mensaje.Equals("admin"))
                {
                    msj = "se guardo correctamente un Administrador";
                    rpta = Ok(new { messaje = "Se creo correctamente el Administrador", error = false, data = new { msj = tipoMensaje, usuario = nu } });
                }
                else
                {
                    rpta = Ok(new { messaje = mensaje, error = true});
                    msj = "No se pudo crear";
                }
                return Ok(rpta);
                
            }
            catch
            {
                return Ok("No se registro ningun usuario");
            }

        }

        //BUSCAR USUARIO POR COD_SAP
        [HttpGet("buscarUsu/{cod_sap}/{cod_soc}")]
        public ActionResult buscarUsu(string  cod_sap, string cod_soc)
        {
            var usuario = _quimpacContext.Usuarios;
            try
            {         
                    return Ok(_adminService.buscarUsuario(cod_sap,cod_soc));

            }
            catch
            {
                return Ok("No se valido el código sap");
            }
            
        }


        //ACTUALIZAR USUARIO SELECCIONADO
        [HttpPost("actualizarUsu")]
        public ActionResult actualizarUsu([FromBody] Usuarios un)
        {
            IActionResult rpta = Unauthorized();
            string mensaje = _adminService.updateUsuario(un);
            try
            {
                if (mensaje.Equals("actualizado"))
                {
                    rpta = Ok(new { messaje = "Se actualizo correctamente", error = false ,data = new { msj = mensaje} }) ;
                }
                else if(mensaje.Equals("usuario"))
                {
                    rpta = Ok(new { messaje = "El usuarios a sido actualizado", error = false });

                }
                return Ok(rpta);

            }
            catch
            {
                return Ok("No se pudo actualizar usuario");
            }
            
        }

        //SELECCIONAR USUARIOS ACTIVOS - INACTIVOS
        [HttpGet("selectUsuActivos/{estado}")]
        public ActionResult selectUsuActivos (int estado)
        {
            var activos = from usuActivos in _quimpacContext.Usuarios
                          where usuActivos.usu_est.Equals(estado) 
                          select usuActivos;
            if (estado == 1)
            {
                return Ok(activos);
            }
            else
            {
                return Ok(activos);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------TABLA ROL----------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------

        //SELECIONAR ROLES
        [HttpGet("selecRol")]
        public ActionResult selectRol()
        {
            try
            {
                return Ok(_adminService.listarRoles());
            }
            catch
            {
                return Ok("No se pudo seleccionar usuarios");
            }
            
        }

        //CREAR ROL
        [HttpPost("crearRol")]
        public ActionResult crearRol()
        {
            IActionResult result = Unauthorized();

            StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            string content = reader.ReadToEndAsync().Result.ToString();
            JObject jsonObject = JObject.Parse(content);

            string rpta = _adminService.newRol(jsonObject);
            try
            {
                if (rpta.Equals("correctamente"))
                {
                    result = Ok(new { messaje = "Se creo el rol correctamente",error=false,data=new {msj= rpta } });
                    return Ok(result);
                }
                else
                {
                    result = Ok(new { messaje = "error", error = true });
                    return Ok(result);
                }
                
            }
            catch
            {
                return Ok("No se pudo crear rol");
            }
            
        }

        //BUSCAR ROL
        [HttpGet("buscarRol/{cod_rol}")]
        public ActionResult buscarRol(int cod_rol)
        {
            try
            {
                return Ok(_adminService.buscarRol(cod_rol));
            }
            catch
            {
                return Ok("No se encontro ningun rol");
            }
            
        }

        //lista rol por cod sap
        [HttpGet("buscarRolXsap/{cod_sap}")]
        public ActionResult buscarRolXsap(string cod_sap)
        {
            try
            {
                return Ok(_adminService.buscarRolXsap(cod_sap));
            }
            catch
            {
                return Ok("No se encontro registro con este código");
            }
            
        }

        //ACTUALIZAR ROL
        [HttpPost("actualizarRol")]
        public ActionResult actualizarRol([FromBody] Rol rl)
        {
            try
            {
                return Ok(_adminService.actualizarRol(rl));
            }
            catch
            {
                return Ok("No se pudo actualizar el registro");
            }
            
        }

        //SELECCIONAR ACTIVO_INACTIVO
        [HttpGet("selectRolActivo/{rol_estado}")]
        public ActionResult selectRolActivo(int rol_estado)
        {
            var estado = from rol in _quimpacContext.Rol
                         where rol.rol_est.Equals(rol_estado)
                         select rol;

            if (rol_estado==1)
            {
                return Ok(estado);
            }
            else
            {
                return Ok(estado);
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------TABLA CLIENTES-------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------

        //SELECCIONAR CLIENTES
        [HttpGet("selectClientes/{cli_org_ven}")]
        public ActionResult selectClientes(string cli_org_ven)
        {
            try
            {
                return Ok(_adminService.listarClientes(cli_org_ven));
            }
            catch
            {
                return Ok("No se pudo seleccionar registros");
            }
            
        }

        //CREAR CLIENTE
        [HttpPost("crearCliente")]
        public ActionResult crearCliente([FromBody] Clientes cli)
        {
            IActionResult result = Unauthorized();
            string rpta = _adminService.crearCliente(cli);
            try
            {
                if (rpta.Equals("creo"))
                {
                    result = Ok(new { messaje = "Se creo correctamente", error = false, data = new { msj = rpta } });
                }
                else
                {
                    result = Ok(new { messaje = rpta, error = true });
                }
                return Ok(result);
            }
            catch
            {
                return Ok("no se pudo crear el cliente");
            }
            
        }

        //BUSCAR CLIENTE 
        [HttpGet("buscarCliente/{cod_sap}")]
        public ActionResult buscarCliente(string cod_sap)
        {
            try
            {
                return Ok(_adminService.buscarCliente(cod_sap));
            }
            catch
            {
                return Ok("No se pudo encontrar el registro");
            }
            
        }

        //ACTUALIZAR CLIENTE
        [HttpPost("actualizarCli")]
        public ActionResult actualizarCli([FromBody] Clientes cli)
        {
            IActionResult result = Unauthorized();
            string rpta = _adminService.actualizarCli(cli);
            try
            {
                if (rpta.Equals("actualizo"))
                {
                    result = Ok(new { messaje = "Se actualizo correctamente", error = false, data = new { msj = rpta } });

                }
                else
                {
                    result = Ok(new { messaje = "No se pudo actualizar", error = true });
                }
                
                return Ok(result);
            }
            catch
            {
                return Ok("No se puedo actualizar el registro");
            }
            
        }

        //BUSCAR CLIEBTE POR ESTADO
        [HttpGet("clienteEstado/{cli_estado}")]
        public ActionResult clienteEstado(int cli_estado)
        {
            var estado = from cli in _quimpacContext.Clientes
                         where cli.cli_est.Equals(cli_estado)
                         select cli;

            if (cli_estado == 0)
            {
                return Ok(estado);
            }
            else
            {
                return Ok(estado);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------TABLA PERMISO---------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------

        //SELECIONAR PERMISO

        [HttpGet("selectPermiso")]
        public ActionResult selectPermiso()
        {
            try
            {
                return Ok(_adminService.listarPermiso());
            }
            catch
            {
                return Ok("No se puedo encontrar registros");
            }
            
        }

        // CREAR PERMISO

        [HttpPost("crearPermiso")]
        public ActionResult crearPermiso([FromBody] Permiso per)
        {
            string rpta = _adminService.crearPermiso(per);
            string respuesta = "";
            try
            {
                if (rpta.Equals("se creo correctamente"))
                {
                    respuesta = "Se creo bien";
                }
                else
                {
                    respuesta = "El permiso ya existe";
                }
                return Ok(respuesta);
            }
            catch
            {
                return Ok("No se pudo crear registro");
            }
                      
        }

        //BUSCAR PERMISO
        [HttpGet("buscarPermiso/{cod_per}")]
        public ActionResult buscarPermiso(int cod_per)
        {
            try
            {
                return Ok(_adminService.buscarPermiso(cod_per));
            }
            catch
            {
                return Ok("No se puedo encontrar registro");
            }
            
        }

        //ACTUALIZAR PERMISO
        [HttpPost("actualizarPermiso")]
        public ActionResult actualizarPermiso([FromBody] Permiso per)
        {
            string rpta = _adminService.actualizarPermiso(per);
            string respuesta = "";
            try
            {
                if (rpta.Equals("se actualizo correctamente"))
                {
                    respuesta = "Se actualixo correctamente";
                }
                else
                {
                    respuesta = "no se puedo actualizar";
                }
                return Ok(respuesta);
            }
            catch
            {
                return Ok("no se puedo actualizar registro");
            }
            
        }

        //SELECCIONAR PERMISOS ACTIVOS-INACTIVOS
        [HttpGet("permisoActivos/{estado}")]
        public ActionResult permisoActivos(int estado)
        {
            var estadoPer = from per in _quimpacContext.Permiso
                            where per.per_est.Equals(estado)
                            select per;

            if (estado == 1)
            {
                return Ok(estadoPer);
            }
            else
            {
                return Ok(estadoPer);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------TABLA ROL PERMISO-------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------

        //CREAR ROL_PERMISO
        [HttpPost("newRolPermiso")]
        public ActionResult newRolPermiso(List<Rol_permiso> rol_Permiso)
        {
            string rpta = "";
            try
            {
                var agregar = rol_Permiso.Select(x => {
                    _quimpacContext.Rol_Permiso.Add(x);
                    _quimpacContext.SaveChanges();
                    return x;
                });
                return Ok(agregar);
            }
            catch
            {
                return Ok("No se pudo crear registro");
            }
            
        }

        //ACTUALIZAR ROL_PERMISO
        [HttpPost("updateRolPermiso")]
        public ActionResult updateRolPermiso(List<Rol_permiso> rol_Permiso)
        {
            try
            {
                var actualizar = rol_Permiso.Select(x => {
                    _quimpacContext.Rol_Permiso.Update(x);
                    _quimpacContext.SaveChanges();
                    return x;
                });
                return Ok(actualizar);
            }
            catch
            {
                return Ok("No se pudo actualizar registro");
            }
            
        }

        //LISTAR ROL_PERMISO
        [HttpGet("listarRolPermiso")]
        public ActionResult listarRolPermiso()
        {
            try
            {
                var listar = from lista in _quimpacContext.Rol_Permiso
                             select lista;
                return Ok(listar);
            }
            catch
            {
                return Ok("No se puedo listar rol");
            }
            
        }

        //BUSCAR ROL_PERMISO
        [HttpGet("buscarRolPermiso")]
        public ActionResult buscarRolPermiso(int id)
        {
            try
            {
                var rol_permiso = _quimpacContext.Rol_Permiso.Where(x => x.rol_per_cod.Equals(id)).FirstOrDefault();

                return Ok(rol_permiso);
            }
            catch
            {
                return Ok("No se puedo encontrar registro");
            }
            
        }

        [HttpGet("listasPerXestado/{estado}")]
        public ActionResult listasPerXestado(string estado)
        {
            try
            {
                var listaPermisos = _adminService.ListarXestado(estado);
                return Ok(listaPermisos);
            }
            catch
            {
                return Ok("No se puedo lista los permsisos");
            }
        }

        //BUSCAR ROL_PERMISOS ACTIVOS
        [HttpGet("rolPermisoActivos")]
        public ActionResult rolPermisoActivos(int estado)
        {
            var activos = from rol_per in _quimpacContext.Rol_Permiso
                          where rol_per.rol_per_est.Equals(estado)
                          select rol_per;
            return Ok(activos);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------TABLA CHOFER-------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost("newChofer")]
        public ActionResult newChofer(Chofer cho)
        {
            string rpta = _adminService.newChofer(cho);
            IActionResult response = Unauthorized();
            try
            {
                if (rpta.Equals("creado"))
                {
                    response = Ok(new { messaje ="Se creó correctamente",error = false,data = new { msj=rpta  , chofer = cho} });
                    return Ok(response);
                }
                else
                {
                    response = Ok(new { messaje = rpta, error = true  });
                    return Ok(response);
                }
            }
            catch
            {
                return Ok("No se pudo crear registro");
            }
            
        }

        [HttpPost("updateChofer")]
        public ActionResult updateChofer(Chofer cho)
        {
            IActionResult result = Unauthorized();
            string rpta = _adminService.updateChofer(cho);
            try
            {
                if (rpta.Equals("actualizado"))
                {
                    result = Ok(new { messaje = "Se actualizo correctamente", error = false, data = new { chofer = cho} });
                    return Ok(result);
                }
                else
                {
                    result = Ok(new { messaje =rpta,error=true});
                    return Ok(result);
                }
                
            }
            catch
            {
                result = Ok(new { messaje = "No  se puedo actualizar el registro", error = true });
                return Ok(result);
            }
            
        }

        [HttpGet("listarChofer")]
        public ActionResult listarChofer()
        {
            try
            {
                return Ok(_adminService.listarChofer());
            }
            catch( Exception e)
            {
                return Ok(e);
            }
            
        }

        [HttpGet("buscarChofer/{cod_sap}")]
        public ActionResult buscarChofer(string cod_sap)
        {
            try
            {
                return Ok(_adminService.buscarChofer(cod_sap));
            }
            catch
            {
                return Ok("No se puedo encontrar registro los registros");
            }
            
        }
        [HttpDelete("eliminarChofer/{cod_cho}")]
        public ActionResult eliminarChofer(int cod_cho)
        {
            IActionResult result = Unauthorized();
            var validarCHo = _quimpacContext.Chofer;
            Chofer chofer = _quimpacContext.Chofer.Where(w => w.cho_cod.Equals(cod_cho)).FirstOrDefault();
            if (chofer.cho_est_reg.Equals("E") || chofer.cho_est_reg.Equals("W"))
            {
                 _quimpacContext.Chofer.Remove(chofer);
                _quimpacContext.SaveChanges();
                result = Ok(new { messaje = "Se Elimino correctamente", error = false });
                
            }
            else
            {
                result = Ok(new { messaje = "Chofer no se puede eliminar", error = true  });
            }
            return Ok(result);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------TABLA Vehiculo-----------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost("newVehiculo")]
        public ActionResult newVehiculo(Placa vehiculo)
        {
            IActionResult result = Unauthorized();
            string rpta = _adminService.newVehiculo(vehiculo);
            try
            {
                if (rpta.Equals("correcto"))
                {
                    result = Ok(new {messaje = "Se creo el vehiculo correctamente", error=false, data=new { msj = rpta , placa=vehiculo} });
                    return Ok(result);
                }
                else
                {
                    result = Ok(new { messaje =rpta, error = true });
                    return Ok(result);
                }
                
            }
            catch
            {
                return Ok("No se puedo crear el registro");
            }
            
        }

        [HttpPost("updateVehiculo")]
        public ActionResult updateVehiculo(Placa vehiculo)
        {
            IActionResult result = Unauthorized();
            string rpta = _adminService.updateVehiculo(vehiculo);
            try
            {
                if (rpta.Equals("actualizo"))
                {
                    result = Ok(new { messaje = "Se actualizó el vehiculo correctamente", error = false, data = new { msj = rpta , placa=vehiculo} });
                    return Ok(result);
                }
                else
                {
                    result = Ok(new { messaje = "No se actualizó el vehiculo", error = true });
                    return Ok(result);
                }

            }
            catch
            {
                return Ok("No se puedo crear el registro");
            }

        }

        [HttpGet("listarVehiculo")]
        public ActionResult listarVehiculo()
        {

            try
            {
                return Ok(_adminService.listarVehiculo());
            }
            catch
            {
                return Ok("No se puedo listar los registro");
            }
            
        }
        

        [HttpGet("buscarVehiculo/{placa}")]
        public ActionResult buscarVehiculo(string placa)
        {
            try
            {
                return Ok(_adminService.buscarVehiculo(placa));
            }
            catch
            {
                return Ok("No se puedo encontrar el registro");
            }
            
        }

       [HttpDelete("eliminarPlaca/{cod_placa}")]
       public ActionResult eliminarPlaca(int cod_placa)
        {
            IActionResult result = Unauthorized();
            var validarCHo = _quimpacContext.Placa;
            Placa placa = _quimpacContext.Placa.Where(w => w.pla_cod.Equals(cod_placa)).FirstOrDefault();
            if (placa.pla_est_reg.Equals("E") || placa.pla_est_reg.Equals("W"))
            {
                _quimpacContext.Placa.Remove(placa);
                _quimpacContext.SaveChanges();
                result = Ok(new { messaje = "Se Elimino correctamente", error = false });

            }
            else
            {
                result = Ok(new { messaje = "Placa no se puede eliminar", error = true });
            }
            return Ok(result);
        }
    }
}
