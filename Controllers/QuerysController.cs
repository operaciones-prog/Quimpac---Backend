using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using PROYEC_QUIMPAC.Models.ModelRequest;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PROYEC_QUIMPAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuerysController : ControllerBase
    {
        private IAdminService _adminService;
        private IQueryService _queryService;
        private readonly QuimpacContext _quimpacContext;
        public QuerysController(IAdminService adminService,IQueryService queryService,QuimpacContext quimpacContext)
        {
            _quimpacContext = quimpacContext;
            _adminService = adminService;
            _queryService = queryService;
        }

        [HttpGet("usuarioXCliente/{id}")]
        public List<Usuarios> usuarioXCliente(int id)
        {
            return _queryService.usuariosXcliente(id);
        }

        [HttpGet("listaStkCliente")]
        public List<Resumen_stock_Cliente> listaStkCliente()
        {
            return _adminService.listaStockCliente();
        }

        [HttpGet("listaStock")]
        public List<Stock> listaStock()
        {
            return _queryService.listaStock();
        }

        [HttpGet("listaOrdenes")]
        public List<Ordenes> listaOrdenes()
        {
            return _queryService.listaOrdenes();
        }
       
        [HttpPost("listaStockXCliente")]
        public ActionResult listaStockXCliente([FromBody] StockFil stk)
        {
            try
            {
                return Ok(_queryService.listaStockXCliente(stk));
            }
            catch
            {
                return Ok("Ingresar datos");
            }
            

        }

        [HttpPost("visualizarMov2")]
        public List<Resumen_stock_Cliente> visualizarMov2([FromBody] StockClienteFil stk)
        {
            return _queryService.visualizarMov2(stk);
        }

        [HttpPost("visualizarOrd")]
        public List<Ordenes> visualizarOrd([FromBody] OrdenesFil ord)
        {
            return _queryService.visualizarOrdenes(ord);
        }


        [HttpGet("permisoXrol/{rol}")]
        public List<Rol_permiso> permisoXrol(int rol)
        {
            return _queryService.permisosXRol(rol);
        }

        [HttpPost("newEntrega")]
        public ActionResult newEntrega()
        {
            IActionResult rpta = Unauthorized();
            string entrega = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                entrega = _queryService.newEntrega(jsonObject);

                if (entrega.Equals("creo"))
                {
                    rpta = Ok(new { messaje = "Se creo correctamente", error = false, data = new { entre = entrega } });
                }
                else
                {
                    rpta = Ok(new { messaje = entrega, error = true });
                }
                return Ok(rpta);
            }
            catch(Exception e)
            {
                return Ok(e);
            }
            
        }

        [HttpGet("solicEntrega/{org_ven}/{cod_cho}/{pla_veh}/{fecha}/{cod_hor}")]
        public ActionResult solicEntrega(string org_ven,string cod_cho,string pla_veh,string fecha,string cod_hor)
        {
            IActionResult result = Unauthorized();
            List<Entrega> entregas = _queryService.SolicEntrega(org_ven,cod_cho,pla_veh,fecha,cod_hor);
            try
            {
                if (entregas.Any(x => x.ent_fec_hor.Equals(fecha) && x.ent_cod_hor.Equals(cod_hor)))
                {
                    result = Ok(new { messaje ="Los horarios ya estan disponibles",error=true });
                }
                else
                {
                    result = Ok(new { messaje = "Horario disponible", error = false });
                }
                return Ok(result);
            }
            catch
            {
                return Ok("verificar la entrega");
            }
        }

        [HttpPost("listaXestado")]
        public ActionResult listaXestado(ListaXestadoFil2 lxs)
        {
            IActionResult result = Unauthorized();
            List<ListaXestadoFil> entrega = _queryService.listaXestado(lxs);
            try
            {
                result = Ok(new { messaje = "Lista de entregas por estatus" , error = false , data = new { lista = entrega} });
                return Ok(result);
            }
            catch
            {
                return Ok("Error al ejecutar");
            }
        }

        [HttpPost("editarEntrega")]
        public ActionResult editarEntrega()
        {
            IActionResult result = Unauthorized();
            string entrega = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                entrega = _queryService.editarEntrega(jsonObject);

                if (entrega.Equals("actualizo"))
                {
                    result = Ok(new { messaje = "Se actualizo correctamente", error = false, data = new { entre = entrega } });
                }
                else
                {
                    result = Ok(new { messaje = "Error al actualizar", error = true });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("buscarEntrega/{num_solicitud}")]
        public ActionResult buscarEntrega(string num_solicitud)
        {
            IActionResult result = Unauthorized();
            var num_sol = _quimpacContext.Entrega;
            Entrega entrega = _queryService.buscarEntrega(num_solicitud);
            try
            {
                if (num_sol.Any(y => y.ent_num_Sol.Equals(num_solicitud)))
                {
                    var listaDetalles = _quimpacContext.Detalle_Entrega.Where(x => x.det_ent_num_sol.Equals(num_solicitud)).ToList();
                    result = Ok(new { messaje = "Entrega encontrada", error = false, Data = new { miEntrega = entrega , listaDetalles = listaDetalles} });

                }
                else
                {
                    result = Ok(new { messaje ="No existe entrega", error = true });
                }

                return Ok(result);
            }
            catch
            {
                return Ok("Revisar el sistema");
            }
        }

        [HttpGet("buscarHora/{cod_hor}")]
        public ActionResult buscarHora(string cod_hor)
        {
            IActionResult result = Unauthorized();
            string rpta = _queryService.buscarHora(cod_hor);
            try
            {
                if (rpta.Equals("encontro"))
                {
                    var hora = _quimpacContext.Horario.Where(x => x.hor_cod_hor.Equals(cod_hor)).FirstOrDefault();
                    result = Ok(new { messaje ="Se encontro el horario", error = false , data = new { hor = hora} });
                }
                else
                {
                    result = Ok(new { messaje = "No se encontro  horario", error = true });
                }
                return Ok(result);
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("listaHorario")]
        public ActionResult listaHorario()
        {
            List<Horario> lista = _queryService.listaHora();
            return Ok(lista);
        }

        [HttpPost("actualizarListaEntrega")]
        public ActionResult actualizarListaEntrega()
        {
            IActionResult result = Unauthorized();
            string rpta = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);
                rpta = _queryService.actualizarEstadoEntrega(jsonObject);

                if (rpta.Equals("aprobo"))
                {
                    result = Ok(new { messaje = "Se aprobó correctamente", error = false });
                }
                else if (rpta.Equals("anulo"))
                {
                    result = Ok(new { messaje = "Se rechazó correctamente", error = false });
                }
                else if (rpta.Equals("reprocesado"))
                {
                    result = Ok(new { messaje = "Se envió a estado anterior (Por aprobar)", error = false });
                }
                else
                {
                    result = Ok(new { messaje = rpta, error = true });
                }
                
            }
            catch (Exception e)
            {
                result = Ok(new { messaje = "Se originó un error en el proceso", error = true });
            }
            return Ok(result);
        }

        [HttpPost("listaStockXCliente2")]
        public ActionResult listaStockXCliente2([FromBody] StockFil stk)
        {
            IActionResult result = Unauthorized();
            var fechaAct = _quimpacContext.Stock.Select(x => x.stk_fec_hor_act);
            
            try
            {
                List<ListaStockXCliente> listaStock = _queryService.listaStockXCliente2(stk);
                result = Ok(new { messeje = "Lista de entregas", error = false ,data = new { lista = listaStock} });
            }
            catch(Exception e)
            {
                result = Ok(new { messeje = "Error al listar", error = true});
            }

            return Ok(result);
        }

        [HttpPost("selectCho")]
        public ActionResult selectCho([FromBody]ChoferXPaisEstado c)
        {
            var choferes = (from cho in _quimpacContext.Chofer
                            where cho.cho_pai.Equals(c.pais) && cho.cho_est.Equals(c.estado)
                            select cho
                            ).ToList();
            return Ok(choferes);
        }

        [HttpPost("selectPla")]
        public ActionResult selectPla([FromBody] PlacaXPaisEstado p)
        {
            var placas = (from pla in _quimpacContext.Placa
                            where pla.pla_pai.Equals(p.pais) && pla.pla_est.Equals(p.estado)
                          select pla
                            ).ToList();
            return Ok(placas);
        }

        [HttpGet("validarStock/{cod_sap}/{cod_almacen}/{cod_producto}/{lote}")]
        public ActionResult validarStock(string cod_sap,string cod_almacen ,string cod_producto, string lote)
        {
            IActionResult result = Unauthorized();
            var validar = _quimpacContext.Stock;
            string rpta = _queryService.validarStock(cod_sap,cod_almacen,cod_producto,lote);
            
            try
            {
                if (rpta.Equals("validado"))
                {
                    result = Ok(new { message = "Registro correcto", error = false });
                }
                else
                {
                    result = Ok(new { message = rpta, error = true });
                }
                
            }
            catch
            {
                result = Ok(new { message = "Se originó un error en el proceso", error = true });
                
            }
            return Ok(result);

        }

        [HttpPost("listarEntregaXClienteEstado")]
        public ActionResult listarEntregaXClienteEstado([FromBody] EntregaFil e)
        {
            List<ListaXestadoFil> listaXstatus = (from ent in _quimpacContext.Entrega
                                                  join hor in _quimpacContext.Horario
                                                  on ent.ent_cod_hor equals hor.hor_cod_hor
                                                  where ent.ent_cli.Contains(e.cod_cli) && 
                                                  ent.ent_cod_sta.Contains(e.sta_ent) &&
                                                  ent.ent_usu_cre_cod_sap.Contains(e.usu_cre)
                                                  orderby ent.ent_cod descending
                                                  select new ListaXestadoFil
                                                  {
                                                      ent_cod = ent.ent_cod,
                                                      ent_num_Sol = ent.ent_num_Sol,
                                                      ent_org_ven = ent.ent_org_vent,
                                                      ent_can = ent.ent_canal,
                                                      ent_sec = ent.ent_sector,
                                                      ent_puesto_exp = ent.ent_puesto_exp,
                                                      ent_fec_cre_doc = ent.ent_fec_cre_doc,
                                                      ent_cli = ent.ent_cli,
                                                      ent_des_cli = ent.ent_des_cli,
                                                      ent_cod_cho = ent.ent_cod_cho,
                                                      ent_pla_veh = ent.ent_pla_veh,
                                                      ent_cod_hor = hor.hor_descrip,
                                                      ent_fec_hor = ent.ent_fec_hor,
                                                      ent_cod_sta = ent.ent_cod_sta,
                                                      ent_fec_apr = ent.ent_fec_apr,
                                                      ent_usu_apr = ent.ent_usu_apr,
                                                      ent_ent_sap = ent.ent_ent_sap,
                                                      ent_log_err = ent.ent_log_err,
                                                      ent_des_err = ent.ent_des_err,
                                                      ent_usu_cre = ent.ent_usu_cre,
                                                      ent_sta_m = ent.ent_sta_m,
                                                      ent_fec_hor_reg_sap = ent.ent_fec_hor_reg_sap,
                                                      ent_est = ent.ent_est,
                                                      ent_fec_cre = ent.ent_fec_cre,
                                                      ent_usu_cre_cod_sap = ent.ent_usu_cre_cod_sap,
                                                      ent_fec_mod = ent.ent_fec_mod,
                                                      ent_usu_mod_cod_sap = ent.ent_usu_mod_cod_sap
                                                  }).ToList();
            return Ok(listaXstatus);
        }

        [HttpGet("listarActualizaciones")]
        public ActionResult listarActualizaciones()
        {

            var lista = _quimpacContext.Actualizaciones.ToList();
            return Ok(lista);
        }

        [HttpGet("listaEntregas/{org_ven}")]
        public ActionResult listaEntregas(string org_ven)
        {
            List<ListaXestadoFil> listaXstatus = (from ent in _quimpacContext.Entrega
                                                  join hor in _quimpacContext.Horario
                                                  on ent.ent_cod_hor equals hor.hor_cod_hor
                                                  where ent.ent_org_vent.Contains(org_ven)
                                                  orderby ent.ent_cod descending
                                                  select new ListaXestadoFil
                                                  {
                                                      ent_cod = ent.ent_cod,
                                                      ent_num_Sol = ent.ent_num_Sol,
                                                      ent_org_ven = ent.ent_org_vent,
                                                      ent_can = ent.ent_canal,
                                                      ent_sec = ent.ent_sector,
                                                      ent_puesto_exp = ent.ent_puesto_exp,
                                                      ent_fec_cre_doc = ent.ent_fec_cre_doc,
                                                      ent_cli = ent.ent_cli,
                                                      ent_des_cli = ent.ent_des_cli,
                                                      ent_cod_cho = ent.ent_cod_cho,
                                                      ent_pla_veh = ent.ent_pla_veh,
                                                      ent_cod_hor = hor.hor_descrip,
                                                      ent_fec_hor = ent.ent_fec_hor,
                                                      ent_cod_sta = ent.ent_cod_sta,
                                                      ent_fec_apr = ent.ent_fec_apr,
                                                      ent_usu_apr = ent.ent_usu_apr,
                                                      ent_ent_sap = ent.ent_ent_sap,
                                                      ent_log_err = ent.ent_log_err,
                                                      ent_des_err = ent.ent_des_err,
                                                      ent_usu_cre = ent.ent_usu_cre,
                                                      ent_sta_m = ent.ent_sta_m,
                                                      ent_fec_hor_reg_sap = ent.ent_fec_hor_reg_sap,
                                                      ent_est = ent.ent_est,
                                                      ent_fec_cre = ent.ent_fec_cre,
                                                      ent_usu_cre_cod_sap = ent.ent_usu_cre_cod_sap,
                                                      ent_fec_mod = ent.ent_fec_mod,
                                                      ent_usu_mod_cod_sap = ent.ent_usu_mod_cod_sap
                                                  }).ToList();
            return Ok(listaXstatus);
        }

        [HttpGet("selectChoTodos/{pais}")]
        public ActionResult selectChoActivos(string pais)
        {
            var choferes = (from cho in _quimpacContext.Chofer
                            where cho.cho_pai.Equals(pais)
                            select cho
                            ).ToList();
            return Ok(choferes);
        }

        [HttpGet("selectPlaTodos/{pais}")]
        public ActionResult selectPlaActivos(string pais)
        {
            var placas = (from pla in _quimpacContext.Placa
                          where pla.pla_pai.Equals(pais)
                          select pla
                            ).ToList();
            return Ok(placas);
        }

        [HttpGet("buscarSociedadXcod/{cod_soc}")]
        public ActionResult buscarSociedadXid(string cod_soc)
        {
            var sociedad = (from s in _quimpacContext.Sociedad
                           where s.soc_cod_soc.Contains(cod_soc)
                           select s).FirstOrDefault();
            return Ok(sociedad);
        }

        [HttpGet("listarSociedad")]
        public ActionResult listarSociedad(string cod_soc)
        {
            var sociedad = (from s in _quimpacContext.Sociedad
                            select s).ToList();
            return Ok(sociedad);
        }

        [HttpGet("enviarEmailAprobacion/{num_sol}/{ent_sap}/{num_tra}")]
        public ActionResult enviarEmailAprobacion(string num_sol, string ent_sap, string num_tra)
        {
            string rpta = "Enviado";
            try
            {
                Entrega entrega = (from ent in _quimpacContext.Entrega
                                   where ent.ent_num_Sol.Equals(num_sol)
                                   select ent).FirstOrDefault();

                Chofer chofer = (from cho in _quimpacContext.Chofer
                                 where cho.cho_cod_cho.Equals(entrega.ent_cod_cho)
                                 select cho).FirstOrDefault();

                List<Detalle_Entrega> detalle_entrega = (from det_ent in _quimpacContext.Detalle_Entrega
                                                            where det_ent.det_ent_num_sol.Equals(num_sol)
                                                            select det_ent).ToList();

                List<Usuarios> lstAdm = (from usua in _quimpacContext.Usuarios
                                         join role in _quimpacContext.Rol
                                         on usua.usu_cod_rol equals role.rol_cod
                                         join rope in _quimpacContext.Rol_Permiso
                                         on usua.usu_cod_rol equals rope.rol_per_cod_rol
                                         where ((usua.usu_cod_cli.Equals(entrega.ent_cli) && rope.rol_per_cod_per.Equals(8) && usua.usu_est.Equals("1")) ||
                                         (usua.usu_cod_soc.Equals(entrega.ent_org_vent) && rope.rol_per_cod_per.Equals(9) && usua.usu_est.Equals("1")))
                                         select usua
                                         ).ToList();

                Usuarios usu = _quimpacContext.Usuarios.Where(x => x.usu_usu.Equals(entrega.ent_usu_cre)).FirstOrDefault();

                String asunto = "Solicitud de entrega aprobada";
                String mensaje = "<p>Estimado Cliente,</p>" +
                    "<p>La orden solicitada por el usuario " + usu.usu_nom_ape +
                    " fue aprobada. Su numero de entrega es " + ent_sap +
                    ", con numero de transporte " + num_tra + " y su fecha agendada es " +
                    entrega.ent_fec_hor;
                if (entrega.ent_cod_hor != "" && entrega.ent_cod_hor != null)
                {
                    mensaje = mensaje + " a las " + entrega.ent_cod_hor.Substring(0,2) + ":" + entrega.ent_cod_hor.Substring(2, 2) + " hrs";
                }
                mensaje = mensaje + ".</p><p>Nombre de Chofer:" + chofer.cho_nom.Replace("|"," ") + "<br>" +
                    "Patente o Placa: " + entrega.ent_pla_veh + "</p>" +
                    "<table><thead>" +
                    "<th>Descripción de Material</th>" +
                    "<th>Cantidad</th>" +
                    "<th>Unid. de medida</th></thead><tbody>";

                for (int i=0; i< detalle_entrega.Count(); i++)
                {
                    mensaje = mensaje + "<tr><td>" + detalle_entrega[i].det_ent_des_mat + "</td>" +
                        "<td>" + detalle_entrega[i].det_ent_can + "</td>" +
                        "<td>" + detalle_entrega[i].det_ent_uni + "</td></tr>";
                }

                mensaje = mensaje + "</tbody></table><p>Saludos Cordiales.</p>";

                _queryService.SendEmailAsync(lstAdm, asunto, mensaje, entrega.ent_org_vent);
            }
            catch(Exception e)
            {
                e.Message.ToString();
                rpta = "";
            } 

            return Ok(rpta);
        }

        [HttpPost("enviarEmailLogErrores")]
        public ActionResult enviarEmailLogErrores([FromBody] MensajeRequest mensajeRequest)
        {
            string rpta = "Enviado";
            
            try
            {
                String asunto = "Log de Error QCTerminales";
                String msj = "<p>Estimado Usuario</p><p>Se ha producido un error en ejecucion en la plataforma QCTerminales con el siguente detalle: </p>" + mensajeRequest.mensaje;

                List<string> lst = new List<string>();
                List<Constantes> lstcts = new List<Constantes>();
                lstcts = _queryService.obtenerConstante("LEEC");

                foreach (Constantes cts in lstcts)
                {
                    lst.Add(cts.val_constante);
                }

                _queryService.SendEmailQC(lst, asunto, msj, "EC30");

                lst = new List<string>();
                lstcts = new List<Constantes>();
                lstcts = _queryService.obtenerConstante("LECL");

                foreach (Constantes cts in lstcts)
                {
                    lst.Add(cts.val_constante);
                }

                _queryService.SendEmailQC(lst, asunto, msj, "CL20");

            }
            catch (Exception e)
            {
                e.Message.ToString();
                rpta = "";
            }

            return Ok(rpta);
        }

        [HttpPost("eliminarDetalleEntrega")]
        public ActionResult eliminarDetalleEntrega()
        {
            IActionResult result = Unauthorized();
            string entrega = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                entrega = _queryService.eliminarDetalleEntrega(jsonObject);

                if (entrega.Equals("eliminado"))
                {
                    result = Ok(new { messaje = "Se elimino correctamente", error = false });
                }
                else
                {
                    result = Ok(new { messaje = "Error al actualizar", error = true });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("postCombo")]
        public async Task<ActionResult<List<Combo>>> postCombo(OpcionCentro r)
        {
            var s = new Response();
            try
            {

                var w = await _queryService.postCombo(r);
                if (w == null)
                    return NoContent();
                else
                {
                    s.error = false;
                    s.message = "Operación Exitosa";
                    s.trace = "";
                    s.data = w.Cast<object>().ToList();
                    return Ok(s);
                }
            }
            catch (Exception e)
            {
                s.error = true;
                s.message = e.Message;
                s.trace = e.StackTrace;
                s.data = new List<object>();
                return StatusCode(200, e);
            }
        }

        [HttpPost("cargaOrdenes")]
        public ActionResult cargaOrdenes()
        {
            IActionResult result = Unauthorized();
            string rpta = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                rpta = _queryService.cargaOrdenes(jsonObject);

                if (rpta.Equals("1") || rpta.Equals("2"))
                {
                    rpta = "{\"message\":\""+rpta+"\",\"type\":\"S\"}";
                }
                else
                {
                    rpta = "{\"message\":\"Error al cargar\",\"type\":\"E\"}";
                }
            }
            catch (Exception e)
            {
                rpta = "{\"message\":\""+e.ToString()+"\",\"type\":\"E\"}";
            }

            return Ok(rpta);
        }

        [HttpPost("cargaMovimientos")]
        public ActionResult cargaMovimientos()
        {
            string rpta = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                rpta = _queryService.cargaMovimientos(jsonObject);

                if (rpta.Equals("1") || rpta.Equals("2"))
                {
                    rpta = "{\"message\":\""+rpta+"\",\"type\":\"S\"}";
                }
                else
                {
                    rpta = "{\"message\":\"Error al cargar\",\"type\":\"E\"}";
                }
                
            }
            catch (Exception e)
            {
                rpta = "{\"message\":\""+e.ToString()+"\",\"type\":\"E\"}";
            }

            return Ok(rpta);
        }

        [HttpPost("cargaStock")]
        public ActionResult cargaStock()
        {
            string rpta = "";
            try
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                string content = reader.ReadToEndAsync().Result.ToString();
                JObject jsonObject = JObject.Parse(content);

                rpta = _queryService.cargaStock(jsonObject);

                if (rpta.Equals("1") || rpta.Equals("2"))
                {
                    rpta = "{\"message\":\"" + rpta + "\",\"type\":\"S\"}";
                }
                else
                {
                    rpta = "{\"message\":\"Error al cargar\",\"type\":\"E\"}";
                }

            }
            catch (Exception e)
            {
                rpta = "{\"message\":\"" + e.ToString() + "\",\"type\":\"E\"}";
            }

            return Ok(rpta);
        }
    }
}
