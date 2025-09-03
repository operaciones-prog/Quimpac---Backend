using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using PROYEC_QUIMPAC.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Repositorys
{
    public class QueryRepository : IQueryRepository
    {
        private IConfiguration _config;
        private readonly QuimpacContext _quimpacContex;
        private readonly string _connectionString;

        public QueryRepository(IConfiguration config, QuimpacContext quimpacContext)
        {
            _config = config;
            _quimpacContex = quimpacContext;
            _connectionString = config.GetConnectionString(Constants.KEY_ENV);
        }

        public string actualizarEstadoEntrega(JObject json)
        {
            
            string rpta = string.Empty;
            DateTime fechaActualiza = DateTime.Now;
            JArray jsonListaEntrega = (JArray)json["listaEntrega"];
            for(int i = 0; i < jsonListaEntrega.Count; i++)
            {
                try
                {
                    Entrega entrega = _quimpacContex.Entrega.Where(x => x.ent_cod == Convert.ToInt32(jsonListaEntrega[i]["ent_cod"])).FirstOrDefault();
                    
                    entrega.ent_cod_sta = jsonListaEntrega[i]["ent_cod_sta"].ToString();
                    if (entrega.ent_cod_sta.Equals("A"))
                    {
                        entrega.ent_fec_apr = fechaActualiza.ToString("dd/MM/yyyy");
                        entrega.ent_usu_apr = jsonListaEntrega[i]["ent_usu_mod_cod_sap"].ToString();
                    }
                    entrega.ent_fec_mod = fechaActualiza;
                    entrega.ent_usu_mod_cod_sap = jsonListaEntrega[i]["ent_usu_mod_cod_sap"].ToString();

                    _quimpacContex.Entrega.Update(entrega);
                    _quimpacContex.SaveChanges();
                    if (entrega.ent_cod_sta.Equals("A"))
                    {
                        rpta = "aprobo";
                    }
                    else if (entrega.ent_cod_sta.Equals("X"))
                    {
                        rpta = "anulo";
                    }
                    else
                    {
                        rpta = "reprocesado";
                    }
                    try
                    {
                        Usuarios usu = new Usuarios();
                        Sociedad soc = new Sociedad();
                        List <Usuarios> lstAdm = (from usua in _quimpacContex.Usuarios
                                                 join role in _quimpacContex.Rol
                                                 on usua.usu_cod_rol equals role.rol_cod
                                                 join rope in _quimpacContex.Rol_Permiso
                                                 on usua.usu_cod_rol equals rope.rol_per_cod_rol
                                                 where (usua.usu_cod_cli.Equals(entrega.ent_cli) && usua.usu_usu.Equals(entrega.ent_usu_cre))
                                                 select usua
                                                 ).ToList();

                        string asunto = "";
                        string mensaje = "";
                        
                        if (rpta.Equals("anulo"))
                        {
                            usu = _quimpacContex.Usuarios.Where(x => x.usu_usu.Equals(entrega.ent_usu_mod_cod_sap)).FirstOrDefault();
                            soc = _quimpacContex.Sociedad.Where(x => x.soc_cod.Equals(usu.usu_cod_soc)).FirstOrDefault();
                            asunto = "Solicitud de entrega rechazada";
                            mensaje = "Se ha rechazado la solicitud de entrega " + entrega.ent_num_Sol + " por el usuario " + usu.usu_nom_ape + ". Pongase en contacto vía correo " + usu.usu_cor + ".";

                            SendEmailAsync(lstAdm, asunto, mensaje, entrega.ent_org_vent);
                        }
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                    }
                }
                catch (Exception e)
                {
                    rpta = "Se origino un error en el proceso";
                }
                
            }

            return rpta;

        }

        public Entrega buscarEntrega(string num_solicitud)
        {
            var entrega = _quimpacContex.Entrega.Where(x => x.ent_num_Sol.Equals(num_solicitud)).FirstOrDefault();
            return entrega;
        }

        public string buscarHora(string cod_hor)
        {
            string rpta = "";
            var hora = _quimpacContex.Horario;
            if (hora.Any(x => x.hor_cod_hor.Equals(cod_hor)))
            {
                rpta = "encontro";
            }
            else
            {
                rpta = "no";
            }
            return rpta;
        }

        public string editarEntrega(JObject json)
        {
            string rpta = "";
            string cod_sta = "";
            DateTime fechaActualizar = DateTime.Now;

            try
            {
                JObject jsonEntregaAct = (JObject)json["entrega"];

                string cod_constante = jsonEntregaAct["ent_org_ven"].ToString() + jsonEntregaAct["ent_puesto_exp"].ToString();
                List<Constantes> lst_constante = obtenerConstante(cod_constante);

                Entrega entrega = _quimpacContex.Entrega.Where(x => x.ent_cod == Convert.ToInt32(jsonEntregaAct["ent_cod"])).FirstOrDefault();
                entrega.ent_org_vent = Convert.ToString(jsonEntregaAct["ent_org_ven"]);
                entrega.ent_puesto_exp = Convert.ToString(jsonEntregaAct["ent_puesto_exp"]);

                entrega.ent_clase_entr = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.CLASE_ENTR)
                                  select s.val_constante).FirstOrDefault();
                entrega.ent_canal = (from s in lst_constante
                                 where s.nom_constante.Contains(Constants.CANAL)
                                 select s.val_constante).FirstOrDefault();
                entrega.ent_sector = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.SECTOR)
                                  select s.val_constante).FirstOrDefault();
                entrega.ent_ruta = (from s in lst_constante
                                where s.nom_constante.Contains(Constants.RUTA)
                                select s.val_constante).FirstOrDefault();
                entrega.ent_puesto = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.PUESTO)
                                  select s.val_constante).FirstOrDefault();
                entrega.ent_clase_tran = (from s in lst_constante
                                      where s.nom_constante.Contains(Constants.CLASE_TRAN)
                                      select s.val_constante).FirstOrDefault();
                entrega.ent_clase_exp = (from s in lst_constante
                                     where s.nom_constante.Contains(Constants.CLASE_EXP)
                                     select s.val_constante).FirstOrDefault();
                entrega.ent_cond_exp = (from s in lst_constante
                                    where s.nom_constante.Contains(Constants.COND_EXP)
                                    select s.val_constante).FirstOrDefault();

                entrega.ent_ref = Convert.ToString(jsonEntregaAct["ent_ref"]);
                entrega.ent_int_ope = Convert.ToString(jsonEntregaAct["ent_int_ope"]);
                entrega.ent_cli = Convert.ToString(jsonEntregaAct["ent_cli"]);
                entrega.ent_des_cli = Convert.ToString(jsonEntregaAct["ent_des_cli"]);
                entrega.ent_cod_cho = Convert.ToString(jsonEntregaAct["ent_cod_cho"]);
                entrega.ent_pla_veh = Convert.ToString(jsonEntregaAct["ent_pla_veh"]);
                entrega.ent_cod_hor = Convert.ToString(jsonEntregaAct["ent_cod_hor"]);
                entrega.ent_fec_hor = Convert.ToString(jsonEntregaAct["ent_fec_hor"]);
                entrega.ent_fec_mod = fechaActualizar;
                entrega.ent_usu_mod_cod_sap = Convert.ToString(jsonEntregaAct["ent_usu_mod_cod_sap"]);

                cod_sta = entrega.ent_cod_sta;

                entrega.ent_cod_sta = "S";
                entrega.ent_sta_m = "X";
                _quimpacContex.Entrega.Update(entrega);
                _quimpacContex.SaveChanges();

                JArray jArrayDEntregaAct = (JArray)json["detallaEntrega"];

                for (int i = 0; i < jArrayDEntregaAct.Count; i++)
                {
                    if (Convert.ToInt32(jArrayDEntregaAct[i]["det_ent_cod"]) == 0)
                    {
                        Detalle_Entrega dt = new Detalle_Entrega
                        {
                            det_ent_num_sol = Convert.ToString(jArrayDEntregaAct[i]["det_ent_num_sol"]),
                            det_ent_pos = Convert.ToString(jArrayDEntregaAct[i]["det_ent_pos"]),
                            det_ent_cen = Convert.ToString(jArrayDEntregaAct[i]["det_ent_cen"]),
                            det_ent_alm = Convert.ToString(jArrayDEntregaAct[i]["det_ent_alm"]),
                            det_ent_mat = Convert.ToString(jArrayDEntregaAct[i]["det_ent_mat"]),
                            det_ent_des_mat = Convert.ToString(jArrayDEntregaAct[i]["det_ent_des_mat"]),
                            det_ent_uni = Convert.ToString(jArrayDEntregaAct[i]["det_ent_uni"]),
                            det_ent_lot = Convert.ToString(jArrayDEntregaAct[i]["det_ent_lot"]),
                            det_ent_can = Convert.ToDecimal(jArrayDEntregaAct[i]["det_ent_can"]),
                            det_ent_sta_m = "X",
                            det_ent_est = "1",
                            det_ent_usu_mod = Convert.ToString(jArrayDEntregaAct[i]["det_ent_usu_mod_cod_sap"]),
                            det_ent_fec_mod = fechaActualizar,
                            det_ent_usu_mod_cod_sap = Convert.ToString(jArrayDEntregaAct[i]["det_ent_usu_mod_cod_sap"])
                        };
                        _quimpacContex.Detalle_Entrega.Add(dt);
                    }
                    else
                    {
                        Detalle_Entrega det_ent = _quimpacContex.Detalle_Entrega.Where(x => x.det_ent_cod == Convert.ToInt32(jArrayDEntregaAct[i]["det_ent_cod"])).FirstOrDefault();
                        det_ent.det_ent_pos = Convert.ToString(jArrayDEntregaAct[i]["det_ent_pos"]);
                        det_ent.det_ent_cen = Convert.ToString(jArrayDEntregaAct[i]["det_ent_cen"]);
                        det_ent.det_ent_alm = Convert.ToString(jArrayDEntregaAct[i]["det_ent_alm"]);
                        det_ent.det_ent_mat = Convert.ToString(jArrayDEntregaAct[i]["det_ent_mat"]);
                        det_ent.det_ent_des_mat = Convert.ToString(jArrayDEntregaAct[i]["det_ent_des_mat"]);
                        det_ent.det_ent_uni = Convert.ToString(jArrayDEntregaAct[i]["det_ent_uni"]);
                        det_ent.det_ent_lot = Convert.ToString(jArrayDEntregaAct[i]["det_ent_lot"]);
                        det_ent.det_ent_can = Convert.ToDecimal(jArrayDEntregaAct[i]["det_ent_can"]);
                        det_ent.det_ent_sta_m = "X";
                        det_ent.det_ent_usu_mod = Convert.ToString(jArrayDEntregaAct[i]["det_ent_usu_mod_cod_sap"]);
                        det_ent.det_ent_fec_mod = fechaActualizar;
                        det_ent.det_ent_usu_mod_cod_sap = Convert.ToString(jArrayDEntregaAct[i]["det_ent_usu_mod_cod_sap"]);
                        _quimpacContex.Detalle_Entrega.Update(det_ent);
                    }
                    _quimpacContex.SaveChanges();
                }

                rpta = "actualizo";

                if (cod_sta.Equals("X"))
                {
                    try
                    {
                        Usuarios usu = _quimpacContex.Usuarios.Where(x => x.usu_usu.Equals(entrega.ent_usu_mod_cod_sap)).FirstOrDefault();
                        Clientes cli = _quimpacContex.Clientes.Where(x => x.cli_cod_sap.Equals(entrega.ent_cli)).FirstOrDefault();
                        List<Usuarios> lstAdm = (from usua in _quimpacContex.Usuarios
                                                 join role in _quimpacContex.Rol
                                                 on usua.usu_cod_rol equals role.rol_cod
                                                 join rope in _quimpacContex.Rol_Permiso
                                                 on usua.usu_cod_rol equals rope.rol_per_cod_rol
                                                 where (usua.usu_cod_soc.Equals(entrega.ent_org_vent) && rope.rol_per_cod_per.Equals(9) && usua.usu_est.Equals("1"))
                                                 select usua).ToList();

                        string asunto = "Solicitud de entrega reprocesada";
                        string mensaje = "Se ha modificado la solicitud de entrega " + entrega.ent_num_Sol + " por el usuario " + usu.usu_nom_ape + " del cliente " + cli.cli_raz_soc + ".";

                        SendEmailAsync(lstAdm, asunto, mensaje, entrega.ent_org_vent);
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                    }
                }
            }
            catch(Exception e)
            {
                rpta = e.Message.ToString();
            }
            
            return rpta;
        }

        public List<Horario> listaHora()
        {
            var horarios = _quimpacContex.Horario.ToList();
            return horarios;
        }

        public List<Ordenes> listaOrdenes()
        {
            var listaOrdenes = _quimpacContex.Ordenes.ToList();
            return listaOrdenes;
        }

        public List<Stock> listaStock()
        {
            var listaStock = _quimpacContex.Stock.ToList();
            return listaStock;
        }

        public List<StockTotalFil> listaStockXCliente(StockFil stk)
        {
            int suma = 0;
            var solicQC = _quimpacContex.Entrega;
            
            var solic_QC_Onl = _quimpacContex.Entrega.Where(x => x.ent_cod_sta.Equals("S") && x.ent_cod.Equals("A") && x.ent_cli.Equals(stk.cod_cli)).Count();
            var listaNueva = (from stke in _quimpacContex.Stock
                              where stke.stk_des_mat.Contains(stk.des_mat) && stke.stk_cod_alm.Contains(stk.tan) 
                                    && stke.stk_num_lot.Contains(stk.lot) && stke.stk_cod_cli.Contains(stk.cod_cli)
                              select new StockTotalFil
                              {
                                  stf_cod_cli = stke.stk_cod_cli,
                                  stf_des_cli = stke.stk_des_cli,
                                  stf_cod_alm = stke.stk_cod_alm,
                                  stf_des_alm = stke.stk_des_alm,
                                  stf_num_mat = stke.stk_num_mat,
                                  stf_des_mat = stke.stk_des_mat,
                                  stf_uni_med = stke.stk_uni_med,
                                  stf_num_lot = stke.stk_num_lot,
                                  stf_sto_dis = stke.stk_sto_dis,
                                  stf_ent_sap = stke.stk_ent_sap,
                                  stf_sto_nna = stke.stk_sto_nna,
                                  stf_sto_tra = stke.stk_sto_tra,
                                  stf_sto_blo = stke.stk_sto_blo,
                                  stf_fec_hor_act = stke.stk_fec_hor_act,
                                  stf_stk_total = stke.stk_sto_nna + stke.stk_sto_tra + stke.stk_sto_blo + stke.stk_sto_dis,
                                  stf_sol_qco = solic_QC_Onl,
                                  stf_stk_dis_qco = stke.stk_sto_dis - stke.stk_ent_sap - suma
                              }).ToList();

            return listaNueva;

        }

        public List<ListaStockXCliente> listaStockXCliente2(StockFil stk)
        {
            List<EntregaCli> listDT = new List<EntregaCli>();
            List<EntregaCli> listDTSap = new List<EntregaCli>();
            List<ListaStockXCliente> stock = new List<ListaStockXCliente>();

            listDT = (from a in _quimpacContex.Entrega
                                    join dt in _quimpacContex.Detalle_Entrega
                                    on a.ent_num_Sol equals dt.det_ent_num_sol
                                    where (a.ent_cod_sta.Equals("S") || a.ent_cod_sta.Equals("A")) && a.ent_cli.Equals(stk.cod_cli)
                                    select new EntregaCli
                                    {
                                        ent_cli = a.ent_cli,
                                        det_ent_mat = dt.det_ent_mat,
                                        det_ent_lot = dt.det_ent_lot,
                                        det_ent_can = dt.det_ent_can,
                                        det_ent_alm = dt.det_ent_alm,
                                    }).ToList();

            Actualizaciones actualizaciones = _quimpacContex.Actualizaciones.Where(x => x.act_tab.Equals("stock")).FirstOrDefault();

            if (actualizaciones.act_pro_est.Equals("1"))
            {
                listDTSap = (from s in _quimpacContex.Stock
                             join a in _quimpacContex.Entrega
                             on s.stk_cod_cli equals a.ent_cli
                             join dt in _quimpacContex.Detalle_Entrega
                             on a.ent_num_Sol equals dt.det_ent_num_sol
                             where a.ent_cod_sta.Equals("E") && s.stk_fec_hor_act <= a.ent_fec_hor_reg_sap && a.ent_cli.Equals(stk.cod_cli)
                             select new EntregaCli
                             {
                                 ent_cli = a.ent_cli,
                                 det_ent_mat = dt.det_ent_mat,
                                 det_ent_lot = dt.det_ent_lot,
                                 det_ent_can = dt.det_ent_can,
                                 det_ent_alm = dt.det_ent_alm
                             }).ToList();

                if (stk.cantidad > 0)
                {
                    stock = (from x in _quimpacContex.Stock
                             where x.stk_des_alm.Contains(stk.tan) && x.stk_des_mat.Contains(stk.des_mat)
                             && x.stk_num_lot.Contains(stk.lot) && x.stk_cod_cli.Contains(stk.cod_cli) 
                             && x.stk_cod_cen.Contains(stk.cod_cen) && x.stk_cod_soc.Contains(stk.cod_soc)
                             select new ListaStockXCliente
                             {
                                 cod = x.stk_cod,
                                 cod_cli = x.stk_cod_cli,
                                 des_cli = x.stk_des_cli,
                                 cod_soc = x.stk_cod_soc,
                                 cod_cen = x.stk_cod_cen,
                                 des_cen = x.stk_des_cen,
                                 cod_alm = x.stk_cod_alm,
                                 fec_hor_act = x.stk_fec_hor_act,
                                 est = x.stk_est,
                                 fec_cre = x.stk_fec_cre,
                                 usu_cre_sap = x.stk_usu_cre_sap,
                                 fec_mod = x.stk_fec_mod,
                                 usu_mod_sap = x.stk_usu_mod_sap,
                                 tanque = x.stk_des_alm,
                                 num_mat = x.stk_num_mat,
                                 des_mat = x.stk_des_mat,
                                 uni_med = x.stk_uni_med,
                                 num_lot = x.stk_num_lot,
                                 stock_total = x.stk_sto_nna + x.stk_sto_tra + x.stk_sto_blo + x.stk_sto_dis,
                                 sto_nna = x.stk_sto_nna,
                                 sto_tra = x.stk_sto_tra,
                                 sto_blo = x.stk_sto_blo,
                                 sto_dis = x.stk_sto_dis,
                                 num_mat_cli = x.stk_num_mat_cli,
                                 peso_equi = x.stk_peso_equi,
                                 ent_sap_QC_Online = calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 ent_sap = x.stk_ent_sap + calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 solicitudes_QC_Online = calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 stock_disponible_QC_Online = x.stk_sto_dis - x.stk_ent_sap - calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot)
                             }).Take(stk.cantidad).ToList();
                }
                else
                {
                    stock = (from x in _quimpacContex.Stock
                             where x.stk_des_alm.Contains(stk.tan) && x.stk_des_mat.Contains(stk.des_mat)
                             && x.stk_num_lot.Contains(stk.lot) && x.stk_cod_cli.Contains(stk.cod_cli) 
                             && x.stk_cod_cen.Contains(stk.cod_cen) && x.stk_cod_soc.Contains(stk.cod_soc)
                             select new ListaStockXCliente
                             {
                                 cod = x.stk_cod,
                                 cod_cli = x.stk_cod_cli,
                                 des_cli = x.stk_des_cli,
                                 cod_cen = x.stk_cod_cen,
                                 des_cen = x.stk_des_cen,
                                 cod_alm = x.stk_cod_alm,
                                 fec_hor_act = x.stk_fec_hor_act,
                                 est = x.stk_est,
                                 fec_cre = x.stk_fec_cre,
                                 usu_cre_sap = x.stk_usu_cre_sap,
                                 fec_mod = x.stk_fec_mod,
                                 usu_mod_sap = x.stk_usu_mod_sap,
                                 tanque = x.stk_des_alm,
                                 num_mat = x.stk_num_mat,
                                 des_mat = x.stk_des_mat,
                                 uni_med = x.stk_uni_med,
                                 num_lot = x.stk_num_lot,
                                 stock_total = x.stk_sto_nna + x.stk_sto_tra + x.stk_sto_blo + x.stk_sto_dis,
                                 sto_nna = x.stk_sto_nna,
                                 sto_tra = x.stk_sto_tra,
                                 sto_blo = x.stk_sto_blo,
                                 sto_dis = x.stk_sto_dis,
                                 num_mat_cli = x.stk_num_mat_cli,
                                 peso_equi = x.stk_peso_equi,
                                 ent_sap_QC_Online = calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 ent_sap = x.stk_ent_sap + calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 solicitudes_QC_Online = calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 stock_disponible_QC_Online = x.stk_sto_dis - x.stk_ent_sap - calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot)
                             }).ToList();
                }
            }
            else
            {
                listDTSap = (from s in _quimpacContex.Stock_aux
                             join a in _quimpacContex.Entrega
                             on s.stk_cod_cli equals a.ent_cli
                             join dt in _quimpacContex.Detalle_Entrega
                             on a.ent_num_Sol equals dt.det_ent_num_sol
                             where a.ent_cod_sta.Equals("E") && s.stk_fec_hor_act <= a.ent_fec_hor_reg_sap && a.ent_cli.Equals(stk.cod_cli)
                             select new EntregaCli
                             {
                                 ent_cli = a.ent_cli,
                                 det_ent_mat = dt.det_ent_mat,
                                 det_ent_lot = dt.det_ent_lot,
                                 det_ent_can = dt.det_ent_can,
                                 det_ent_alm = dt.det_ent_alm
                             }).ToList();

                if (stk.cantidad > 0)
                {
                    stock = (from x in _quimpacContex.Stock_aux
                             where x.stk_des_alm.Contains(stk.tan) && x.stk_des_mat.Contains(stk.des_mat)
                             && x.stk_num_lot.Contains(stk.lot) && x.stk_cod_cli.Contains(stk.cod_cli)
                             && x.stk_cod_cen.Contains(stk.cod_cen) && x.stk_cod_soc.Contains(stk.cod_soc)
                             select new ListaStockXCliente
                             {
                                 cod = x.stk_cod,
                                 cod_cli = x.stk_cod_cli,
                                 des_cli = x.stk_des_cli,
                                 cod_cen = x.stk_cod_cen,
                                 des_cen = x.stk_des_cen,
                                 cod_alm = x.stk_cod_alm,
                                 fec_hor_act = x.stk_fec_hor_act,
                                 est = x.stk_est,
                                 fec_cre = x.stk_fec_cre,
                                 usu_cre_sap = x.stk_usu_cre_sap,
                                 fec_mod = x.stk_fec_mod,
                                 usu_mod_sap = x.stk_usu_mod_sap,
                                 tanque = x.stk_des_alm,
                                 num_mat = x.stk_num_mat,
                                 des_mat = x.stk_des_mat,
                                 uni_med = x.stk_uni_med,
                                 num_lot = x.stk_num_lot,
                                 stock_total = x.stk_sto_nna + x.stk_sto_tra + x.stk_sto_blo + x.stk_sto_dis,
                                 sto_nna = x.stk_sto_nna,
                                 sto_tra = x.stk_sto_tra,
                                 sto_blo = x.stk_sto_blo,
                                 sto_dis = x.stk_sto_dis,
                                 num_mat_cli = x.stk_num_mat_cli,
                                 peso_equi = x.stk_peso_equi,
                                 ent_sap_QC_Online = calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 ent_sap = x.stk_ent_sap + calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 solicitudes_QC_Online = calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 stock_disponible_QC_Online = x.stk_sto_dis - x.stk_ent_sap - calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot)
                             }).Take(stk.cantidad).ToList();
                }
                else
                {
                    stock = (from x in _quimpacContex.Stock_aux
                             where x.stk_des_alm.Contains(stk.tan) && x.stk_des_mat.Contains(stk.des_mat)
                             && x.stk_num_lot.Contains(stk.lot) && x.stk_cod_cli.Contains(stk.cod_cli)
                             && x.stk_cod_cen.Contains(stk.cod_cen) && x.stk_cod_soc.Contains(stk.cod_soc)
                             select new ListaStockXCliente
                             {
                                 cod = x.stk_cod,
                                 cod_cli = x.stk_cod_cli,
                                 des_cli = x.stk_des_cli,
                                 cod_cen = x.stk_cod_cen,
                                 des_cen = x.stk_des_cen,
                                 cod_alm = x.stk_cod_alm,
                                 fec_hor_act = x.stk_fec_hor_act,
                                 est = x.stk_est,
                                 fec_cre = x.stk_fec_cre,
                                 usu_cre_sap = x.stk_usu_cre_sap,
                                 fec_mod = x.stk_fec_mod,
                                 usu_mod_sap = x.stk_usu_mod_sap,
                                 tanque = x.stk_des_alm,
                                 num_mat = x.stk_num_mat,
                                 des_mat = x.stk_des_mat,
                                 uni_med = x.stk_uni_med,
                                 num_lot = x.stk_num_lot,
                                 stock_total = x.stk_sto_nna + x.stk_sto_tra + x.stk_sto_blo + x.stk_sto_dis,
                                 sto_nna = x.stk_sto_nna,
                                 sto_tra = x.stk_sto_tra,
                                 sto_blo = x.stk_sto_blo,
                                 sto_dis = x.stk_sto_dis,
                                 num_mat_cli = x.stk_num_mat_cli,
                                 peso_equi = x.stk_peso_equi,
                                 ent_sap_QC_Online = calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 ent_sap = x.stk_ent_sap + calcular_solicitudes_SAP_QC_Online(listDTSap, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 solicitudes_QC_Online = calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot),
                                 stock_disponible_QC_Online = x.stk_sto_dis - x.stk_ent_sap - calcular_solicitudes_QCOnline(listDT, x.stk_cod_cli, x.stk_num_mat, x.stk_cod_alm, x.stk_num_lot)
                             }).ToList();
                }
            }

            for (int i = 0; i < stock.Count(); i++)
            {
                if (stock[i].stock_disponible_QC_Online < 0)
                {
                    stock[i].stock_disponible_QC_Online = 0;
                }
            }

            return stock;
        }

        //Calcular stock solicitudes online por stock-cliente-lote
        public static decimal calcular_solicitudes_QCOnline(List<EntregaCli> listDT, string cod_cli, string num_mat,string num_alm, string num_lot)
        {
            decimal calculo = 0;

            for(int i=0; i<listDT.Count(); i++)
            {
                if(listDT[i].ent_cli.Equals(cod_cli) && listDT[i].det_ent_mat.Equals(num_mat) && listDT[i].det_ent_alm.Equals(num_alm) && listDT[i].det_ent_lot.Equals(num_lot)) 
                {
                    calculo = calculo + listDT[i].det_ent_can;
                }
            }

            return calculo;
        }

        //Calcular stock solicitudes SAP online por stock-cliente-lote
        public static decimal calcular_solicitudes_SAP_QC_Online(List<EntregaCli> listDTSap, string cod_cli, string num_mat,string num_alm, string num_lot)
        {
            decimal calculo = 0;

            for (int i = 0; i < listDTSap.Count(); i++)
            {
                if(listDTSap[i].ent_cli.Equals(cod_cli) && listDTSap[i].det_ent_mat.Equals(num_mat) && listDTSap[i].det_ent_alm.Equals(num_alm) && listDTSap[i].det_ent_lot.Equals(num_lot))
                {
                    calculo = calculo + listDTSap[i].det_ent_can;
                }
            }

            return calculo;
        }

        public List<ListaXestadoFil> listaXestado(ListaXestadoFil2 lxs)
        {
            //var listaXstatus = _quimpacContex.Entrega.Where(x => x.ent_cod_sta.Equals(status)).ToList();
            List<ListaXestadoFil> listaXstatus = (from ent in _quimpacContex.Entrega
                                join hor in _quimpacContex.Horario
                                on ent.ent_cod_hor equals hor.hor_cod_hor
                                where ent.ent_cod_sta.Contains(lxs.status) && ent.ent_org_vent.Contains(lxs.org_vem)
                                orderby ent.ent_cod descending
                                select new ListaXestadoFil
                                {
                                    ent_cod = ent.ent_cod,
                                    ent_num_Sol = ent.ent_num_Sol,
                                    ent_org_ven = ent.ent_org_vent,
                                    ent_can = ent.ent_canal,
                                    ent_sec = ent.ent_sector,
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
            return listaXstatus;
        }

        public string newEntrega(JObject json)
        {
            DateTime fechaHora = new DateTime();
            fechaHora = DateTime.Now;
            string rpta = "";
            int sumaCor = 0;
            var validarEnt = _quimpacContex.Entrega;
            var validarDetEnt = _quimpacContex.Detalle_Entrega;

            try
            {
                var correlativo = (from indice in _quimpacContex.Correlativo
                                   select indice.cor_ind).FirstOrDefault();

                JObject jsonEntrega = (JObject)json["entrega"];

                string cod_constante = jsonEntrega["ent_org_ven"].ToString() + jsonEntrega["ent_puesto_exp"].ToString();
                List<Constantes> lst_constante = obtenerConstante(cod_constante);
                
                Entrega entrega = new Entrega
                {
                    ent_num_Sol = correlativo,
                    ent_org_vent = jsonEntrega["ent_org_ven"].ToString(),
                    ent_puesto_exp = jsonEntrega["ent_puesto_exp"].ToString(),

                    ent_clase_entr = (from s in lst_constante 
                                      where s.nom_constante.Contains(Constants.CLASE_ENTR)
                                      select s.val_constante).FirstOrDefault(),
                    ent_canal = (from s in lst_constante
                                 where s.nom_constante.Contains(Constants.CANAL)
                                 select s.val_constante).FirstOrDefault(),
                    ent_sector = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.SECTOR)
                                  select s.val_constante).FirstOrDefault(),
                    ent_ruta = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.RUTA)
                                  select s.val_constante).FirstOrDefault(),
                    ent_puesto = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.PUESTO)
                                  select s.val_constante).FirstOrDefault(),
                    ent_clase_tran = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.CLASE_TRAN)
                                  select s.val_constante).FirstOrDefault(),
                    ent_clase_exp = (from s in lst_constante
                                  where s.nom_constante.Contains(Constants.CLASE_EXP)
                                  select s.val_constante).FirstOrDefault(),
                    ent_cond_exp = (from s in lst_constante
                                     where s.nom_constante.Contains(Constants.COND_EXP)
                                     select s.val_constante).FirstOrDefault(),

                    ent_ref = jsonEntrega["ent_ref"].ToString(),
                    ent_int_ope = jsonEntrega["ent_int_ope"].ToString(),
                    ent_fec_cre_doc = fechaHora.ToString("dd/MM/yyyy"),
                    ent_cli = jsonEntrega["ent_cli"].ToString(),
                    ent_des_cli = jsonEntrega["ent_des_cli"].ToString(),
                    ent_cod_cho = jsonEntrega["ent_cod_cho"].ToString(),
                    ent_pla_veh = jsonEntrega["ent_pla_veh"].ToString(),
                    ent_cod_hor = jsonEntrega["ent_cod_hor"].ToString(),
                    ent_fec_hor = jsonEntrega["ent_fec_hor"].ToString(),
                    ent_cod_sta = "S",
                    ent_ent_sap = "",
                    ent_log_err = "",
                    ent_des_err = "",
                    ent_usu_cre = jsonEntrega["ent_usu_cre_cod_sap"].ToString(),
                    ent_sta_m = "",
                    ent_est = "1",
                    ent_fec_cre = fechaHora,
                    ent_usu_cre_cod_sap = jsonEntrega["ent_usu_cre_cod_sap"].ToString()
                };

                if (validarEnt.Any(x => x.ent_num_Sol.Equals(entrega.ent_num_Sol)))
                {
                    rpta = "El número de solicitud ya existe";
                }
                else
                {
                    _quimpacContex.Entrega.Add(entrega);
                    _quimpacContex.SaveChanges();

                    sumaCor = Convert.ToInt32(correlativo);
                    sumaCor += 1;

                    Correlativo cor = new Correlativo
                    {
                        cor_cod = 1,
                        cor_nom = "ENTR",
                        cor_ran_1 = "1000000000",
                        cor_ran_2 = "1099999999",
                        cor_ind = sumaCor.ToString()
                    };
                    _quimpacContex.Correlativo.Update(cor);
                    _quimpacContex.SaveChanges();

                    JArray jArrayDEntrega = (JArray)json["detallaEntrega"];
                    for (int i = 0; i < jArrayDEntrega.Count; i++)
                    {
                        Detalle_Entrega dat_ent = new Detalle_Entrega
                        {
                            det_ent_num_sol = entrega.ent_num_Sol,
                            det_ent_pos = jArrayDEntrega[i]["det_ent_pos"].ToString(),
                            det_ent_cen = jArrayDEntrega[i]["det_ent_cen"].ToString(),
                            det_ent_alm = jArrayDEntrega[i]["det_ent_alm"].ToString(),
                            det_ent_mat = jArrayDEntrega[i]["det_ent_mat"].ToString(),
                            det_ent_des_mat = jArrayDEntrega[i]["det_ent_des_mat"].ToString(),
                            det_ent_uni = jArrayDEntrega[i]["det_ent_uni"].ToString(),
                            det_ent_lot = jArrayDEntrega[i]["det_ent_lot"].ToString(),
                            det_ent_can = Convert.ToDecimal(jArrayDEntrega[i]["det_ent_can"].ToString()),
                            det_ent_sta_c = "X",
                            det_ent_usu_cre = jArrayDEntrega[i]["det_ent_usu_cre_cod_sap"].ToString(),
                            det_ent_est = "",
                            det_ent_fec_cre = fechaHora,
                            det_ent_usu_cre_cod_sap = jArrayDEntrega[i]["det_ent_usu_cre_cod_sap"].ToString()
                        };

                        if (validarDetEnt.Any(x => x.det_ent_num_sol.Equals(dat_ent.det_ent_num_sol) && x.det_ent_pos.Equals(dat_ent.det_ent_pos)))
                        {
                            rpta = "Material debe estar en otra posición";
                        }
                        else
                        {
                            _quimpacContex.Detalle_Entrega.Add(dat_ent);
                            _quimpacContex.SaveChanges();
                            rpta = "creo";
                        }
                    }

                    try
                    {
                        Usuarios usu = _quimpacContex.Usuarios.Where(x => x.usu_usu.Equals(entrega.ent_usu_cre)).FirstOrDefault();
                        Clientes cli = _quimpacContex.Clientes.Where(x => x.cli_cod_sap.Equals(entrega.ent_cli)).FirstOrDefault();
                        List<Usuarios> lstAdm = (from usua in _quimpacContex.Usuarios
                                                    join role in _quimpacContex.Rol
                                                    on usua.usu_cod_rol equals role.rol_cod
                                                    join rope in _quimpacContex.Rol_Permiso
                                                    on usua.usu_cod_rol equals rope.rol_per_cod_rol
                                                    where (usua.usu_cod_soc.Equals(entrega.ent_org_vent) && rope.rol_per_cod_per.Equals(9) && usua.usu_est.Equals("1"))
                                                    select usua
                                                ).ToList();
                            
                        string asunto = "Nueva solicitud de entrega";
                        string mensaje = "Se ha generado la solicitud de entrega " + entrega.ent_num_Sol + " por el usuario " + usu.usu_nom_ape + " del cliente " + cli.cli_raz_soc + ".";

                        SendEmailAsync(lstAdm, asunto, mensaje, entrega.ent_org_vent);
                    }
                    catch (Exception e)
                    {
                        e.Message.ToString();
                    }
                }
            }
            catch (SqlException e)
            {
                e.Message.ToString();
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            
            return rpta;
        }

        public List<Rol_permiso> permisosXRol(int rol)
        {
            var listaPermisos = 
                _quimpacContex.Rol_Permiso.Where(x => x.rol_per_cod_rol.Equals(rol)).ToList();
            return listaPermisos;

        }

        public List<Entrega> SolicEntrega(string org_ven, string cod_cho, string pla_veh, string fecha, string cod_hor)
        {
            List<Entrega> listaEntrega = (from entrega in _quimpacContex.Entrega
                                         where entrega.ent_org_vent.Equals(org_ven) && entrega.ent_cod_cho.Equals(cod_cho)
                                               && entrega.ent_pla_veh.Equals(pla_veh) && entrega.ent_fec_hor.Equals(fecha)
                                               && entrega.ent_cod_hor.Equals(cod_hor)
                                         select entrega).ToList();

            return listaEntrega;
        }

        public List<Usuarios> usuariosXcliente(int id)
        {
            var listaUsuarios = _quimpacContex.Usuarios.Where(w => w.usu_cod_cli.Equals(id)).Select(x => x).ToList();
            return listaUsuarios;
        }

        public string validarStock(string cod_sap, string cod_almacen, string cod_producto , string lote)
        {
            string rpta = "";
            var lstStock = _quimpacContex.Stock.Where(x => x.stk_cod_cli.Contains(cod_sap) && x.stk_cod_alm.Equals(cod_almacen) && x.stk_num_mat.Contains(cod_producto)).ToList();
            if (lstStock.Count > 0)
            {
                var stock = _quimpacContex.Stock.Where(x => x.stk_cod_cli.Contains(cod_sap) && x.stk_cod_alm.Equals(cod_almacen) && x.stk_num_mat.Contains(cod_producto) && x.stk_num_lot.Equals(lote)).ToList();
                if (stock.Count > 0)
                {
                    rpta = "validado";
                }
                else
                {
                    rpta = "No existe lote para el almacen";
                }
            }
            else
            {
                rpta = "No existe almacen para el producto";
            }
            return rpta;
        }

        public List<Resumen_stock_Cliente> visualizarMov2(StockClienteFil stk)
        {
            var visualizarMov = new List<Resumen_stock_Cliente>();

            Actualizaciones actualizaciones = _quimpacContex.Actualizaciones.Where(x => x.act_tab.Equals("movimientos")).FirstOrDefault();

            if (actualizaciones.act_pro_est.Equals("1"))
            {
                if (stk.cantidad > 0)
                {
                    visualizarMov = _quimpacContex.Resumen_Stock_Cliente
                    .Where(x => x.stk_cli_cod_cli.Contains(stk.cod_cli)
                    && x.stk_cli_des_alm.Contains(stk.des_alm)
                    && x.stk_cli_des_mat.Contains(stk.des_mat)
                    && x.stk_cli_num_lot.Contains(stk.num_lot)
                    && x.stk_cli_cod_ser_pre.Contains(stk.ser_pre)
                    && x.stk_cli_buq.Contains(stk.cli_buq)
                    && x.stk_cli_cod_soc.Contains(stk.cod_soc)
                    && x.stk_cli_cod_cen.Contains(stk.org_ven)
                    && x.stk_cli_cod_cho.Contains(stk.stk_cli_cod_cho)
                    && x.stk_cli_guia.Contains(stk.stk_cli_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.stk_cli_fec_mov) >= Convert.ToDateTime(stk.fec_mov_des) && Convert.ToDateTime(x.stk_cli_fec_mov) <= Convert.ToDateTime(stk.fec_mov_has)
                    && Convert.ToDateTime(x.stk_cli_fec_lle) >= Convert.ToDateTime(stk.fec_lle_des) && Convert.ToDateTime(x.stk_cli_fec_lle) <= Convert.ToDateTime(stk.fec_lle_has)
                    && Convert.ToDateTime(x.stk_cli_fec_sal) >= Convert.ToDateTime(stk.fec_sal_des) && Convert.ToDateTime(x.stk_cli_fec_sal) <= Convert.ToDateTime(stk.fec_sal_has)).OrderByDescending(x => x.stk_cli_fec_hor_act_sap).Take(stk.cantidad).ToList();

                }
                else
                {
                    visualizarMov = _quimpacContex.Resumen_Stock_Cliente
                    .Where(x => x.stk_cli_cod_cli.Contains(stk.cod_cli)
                    && x.stk_cli_des_alm.Contains(stk.des_alm)
                    && x.stk_cli_des_mat.Contains(stk.des_mat)
                    && x.stk_cli_num_lot.Contains(stk.num_lot)
                    && x.stk_cli_cod_ser_pre.Contains(stk.ser_pre)
                    && x.stk_cli_buq.Contains(stk.cli_buq)
                    && x.stk_cli_cod_soc.Contains(stk.cod_soc)
                    && x.stk_cli_cod_cen.Contains(stk.org_ven)
                    && x.stk_cli_cod_cho.Contains(stk.stk_cli_cod_cho)
                    && x.stk_cli_guia.Contains(stk.stk_cli_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.stk_cli_fec_mov) >= Convert.ToDateTime(stk.fec_mov_des) && Convert.ToDateTime(x.stk_cli_fec_mov) <= Convert.ToDateTime(stk.fec_mov_has)
                    && Convert.ToDateTime(x.stk_cli_fec_lle) >= Convert.ToDateTime(stk.fec_lle_des) && Convert.ToDateTime(x.stk_cli_fec_lle) <= Convert.ToDateTime(stk.fec_lle_has)
                    && Convert.ToDateTime(x.stk_cli_fec_sal) >= Convert.ToDateTime(stk.fec_sal_des) && Convert.ToDateTime(x.stk_cli_fec_sal) <= Convert.ToDateTime(stk.fec_sal_has)).OrderByDescending(x => x.stk_cli_fec_hor_act_sap).ToList();
                }
            }
            else
            {
                if (stk.cantidad > 0)
                {
                    var visualizarMovAux = _quimpacContex.Resumen_Stock_Cliente_aux
                    .Where(x => x.stk_cli_cod_cli.Contains(stk.cod_cli)
                    && x.stk_cli_des_alm.Contains(stk.des_alm)
                    && x.stk_cli_des_mat.Contains(stk.des_mat)
                    && x.stk_cli_num_lot.Contains(stk.num_lot)
                    && x.stk_cli_cod_ser_pre.Contains(stk.ser_pre)
                    && x.stk_cli_buq.Contains(stk.cli_buq)
                    && x.stk_cli_cod_soc.Contains(stk.cod_soc)
                    && x.stk_cli_cod_cen.Contains(stk.org_ven)
                    && x.stk_cli_cod_cho.Contains(stk.stk_cli_cod_cho)
                    && x.stk_cli_guia.Contains(stk.stk_cli_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.stk_cli_fec_mov) >= Convert.ToDateTime(stk.fec_mov_des) && Convert.ToDateTime(x.stk_cli_fec_mov) <= Convert.ToDateTime(stk.fec_mov_has)
                    && Convert.ToDateTime(x.stk_cli_fec_lle) >= Convert.ToDateTime(stk.fec_lle_des) && Convert.ToDateTime(x.stk_cli_fec_lle) <= Convert.ToDateTime(stk.fec_lle_has)
                    && Convert.ToDateTime(x.stk_cli_fec_sal) >= Convert.ToDateTime(stk.fec_sal_des) && Convert.ToDateTime(x.stk_cli_fec_sal) <= Convert.ToDateTime(stk.fec_sal_has)).OrderByDescending(x => x.stk_cli_fec_hor_act_sap).Take(stk.cantidad).ToList();

                    var visualizarMovJson = JsonConvert.SerializeObject(visualizarMovAux);
                    visualizarMov = JsonConvert.DeserializeObject<List<Resumen_stock_Cliente>>(visualizarMovJson);
                }
                else
                {
                    var visualizarMovAux = _quimpacContex.Resumen_Stock_Cliente_aux
                    .Where(x => x.stk_cli_cod_cli.Contains(stk.cod_cli)
                    && x.stk_cli_des_alm.Contains(stk.des_alm)
                    && x.stk_cli_des_mat.Contains(stk.des_mat)
                    && x.stk_cli_num_lot.Contains(stk.num_lot)
                    && x.stk_cli_cod_ser_pre.Contains(stk.ser_pre)
                    && x.stk_cli_buq.Contains(stk.cli_buq)
                    && x.stk_cli_cod_soc.Contains(stk.cod_soc)
                    && x.stk_cli_cod_cen.Contains(stk.org_ven)
                    && x.stk_cli_cod_cho.Contains(stk.stk_cli_cod_cho)
                    && x.stk_cli_guia.Contains(stk.stk_cli_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.stk_cli_fec_mov) >= Convert.ToDateTime(stk.fec_mov_des) && Convert.ToDateTime(x.stk_cli_fec_mov) <= Convert.ToDateTime(stk.fec_mov_has)
                    && Convert.ToDateTime(x.stk_cli_fec_lle) >= Convert.ToDateTime(stk.fec_lle_des) && Convert.ToDateTime(x.stk_cli_fec_lle) <= Convert.ToDateTime(stk.fec_lle_has)
                    && Convert.ToDateTime(x.stk_cli_fec_sal) >= Convert.ToDateTime(stk.fec_sal_des) && Convert.ToDateTime(x.stk_cli_fec_sal) <= Convert.ToDateTime(stk.fec_sal_has)).OrderByDescending(x => x.stk_cli_fec_hor_act_sap).ToList();

                    var visualizarMovJson = JsonConvert.SerializeObject(visualizarMovAux);
                    visualizarMov = JsonConvert.DeserializeObject<List<Resumen_stock_Cliente>>(visualizarMovJson);
                }
            }
            return visualizarMov;
        }

        public List<Ordenes> visualizarOrdenes(OrdenesFil ord)
        {
            var visualizarOrd = new List<Ordenes>();

            Actualizaciones actualizaciones = _quimpacContex.Actualizaciones.Where(x => x.act_tab.Equals("ordenes")).FirstOrDefault();

            if (actualizaciones.act_pro_est.Equals("1"))
            {
                if (ord.cantidad > 0)
                {
                    visualizarOrd = _quimpacContex.Ordenes
                    .Where(x => x.ord_cod_pro.Contains(ord.cod_cli)
                    && x.ord_des_mat.Contains(ord.des_mat)
                    && x.ord_cod_ser_pre.Contains(ord.val_2)
                    && x.ord_ori.Contains(ord.ori)
                    && x.ord_des.Contains(ord.des)
                    && x.ord_est_doc.Contains(ord.est)
                    && x.ord_cod_soc.Contains(ord.cod_soc)
                    && x.ord_cod_cen.Contains(ord.org_ven)
                    && x.ord_cod_cho.Contains(ord.ord_cod_cho)
                    && x.ord_guia.Contains(ord.ord_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.ord_fec_fin_pla) >= Convert.ToDateTime(ord.fec_pla_ini_des) && Convert.ToDateTime(x.ord_fec_fin_pla) <= Convert.ToDateTime(ord.fec_pla_ini_has)
                    && Convert.ToDateTime(x.ord_fec_reg) >= Convert.ToDateTime(ord.fec_rea_ini_des) && Convert.ToDateTime(x.ord_fec_reg) <= Convert.ToDateTime(ord.fec_rea_ini_has)).OrderByDescending(x => x.ord_fec_hor_act_sap).Take(ord.cantidad).ToList();
                }
                else
                {
                    visualizarOrd = _quimpacContex.Ordenes
                    .Where(x => x.ord_cod_pro.Contains(ord.cod_cli)
                    && x.ord_des_mat.Contains(ord.des_mat)
                    && x.ord_cod_ser_pre.Contains(ord.val_2)
                    && x.ord_ori.Contains(ord.ori)
                    && x.ord_des.Contains(ord.des)
                    && x.ord_est_doc.Contains(ord.est)
                    && x.ord_cod_soc.Contains(ord.cod_soc)
                    && x.ord_cod_cen.Contains(ord.org_ven)
                    && x.ord_cod_cho.Contains(ord.ord_cod_cho)
                    && x.ord_guia.Contains(ord.ord_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.ord_fec_fin_pla) >= Convert.ToDateTime(ord.fec_pla_ini_des) && Convert.ToDateTime(x.ord_fec_fin_pla) <= Convert.ToDateTime(ord.fec_pla_ini_has)
                    && Convert.ToDateTime(x.ord_fec_reg) >= Convert.ToDateTime(ord.fec_rea_ini_des) && Convert.ToDateTime(x.ord_fec_reg) <= Convert.ToDateTime(ord.fec_rea_ini_has)).OrderByDescending(x => x.ord_fec_hor_act_sap).ToList();

                }
            }
            else
            {
                if (ord.cantidad > 0)
                {
                    var visualizarOrdAux = _quimpacContex.Ordenes_aux
                    .Where(x => x.ord_cod_pro.Contains(ord.cod_cli)
                    && x.ord_des_mat.Contains(ord.des_mat)
                    && x.ord_cod_ser_pre.Contains(ord.val_2)
                    && x.ord_ori.Contains(ord.ori)
                    && x.ord_des.Contains(ord.des)
                    && x.ord_est_doc.Contains(ord.est)
                    && x.ord_cod_soc.Contains(ord.cod_soc)
                    && x.ord_cod_cen.Contains(ord.org_ven)
                    && x.ord_cod_cho.Contains(ord.ord_cod_cho)
                    && x.ord_guia.Contains(ord.ord_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.ord_fec_fin_pla) >= Convert.ToDateTime(ord.fec_pla_ini_des) && Convert.ToDateTime(x.ord_fec_fin_pla) <= Convert.ToDateTime(ord.fec_pla_ini_has)
                    && Convert.ToDateTime(x.ord_fec_reg) >= Convert.ToDateTime(ord.fec_rea_ini_des) && Convert.ToDateTime(x.ord_fec_reg) <= Convert.ToDateTime(ord.fec_rea_ini_has)).OrderByDescending(x => x.ord_fec_hor_act_sap).Take(ord.cantidad).ToList();

                    var visualizarOrdJson = JsonConvert.SerializeObject(visualizarOrdAux);
                    visualizarOrd = JsonConvert.DeserializeObject<List<Ordenes>>(visualizarOrdJson);
                }
                else
                {
                    var visualizarOrdAux = _quimpacContex.Ordenes_aux
                    .Where(x => x.ord_cod_pro.Contains(ord.cod_cli)
                    && x.ord_des_mat.Contains(ord.des_mat)
                    && x.ord_cod_ser_pre.Contains(ord.val_2)
                    && x.ord_ori.Contains(ord.ori)
                    && x.ord_des.Contains(ord.des)
                    && x.ord_est_doc.Contains(ord.est)
                    && x.ord_cod_soc.Contains(ord.cod_soc)
                    && x.ord_cod_cen.Contains(ord.org_ven)
                    && x.ord_cod_cho.Contains(ord.ord_cod_cho)
                    && x.ord_guia.Contains(ord.ord_guia)).ToList()
                    .Where(x => Convert.ToDateTime(x.ord_fec_fin_pla) >= Convert.ToDateTime(ord.fec_pla_ini_des) && Convert.ToDateTime(x.ord_fec_fin_pla) <= Convert.ToDateTime(ord.fec_pla_ini_has)
                    && Convert.ToDateTime(x.ord_fec_reg) >= Convert.ToDateTime(ord.fec_rea_ini_des) && Convert.ToDateTime(x.ord_fec_reg) <= Convert.ToDateTime(ord.fec_rea_ini_has)).OrderByDescending(x => x.ord_fec_hor_act_sap).ToList();

                    var visualizarOrdJson = JsonConvert.SerializeObject(visualizarOrdAux);
                    visualizarOrd = JsonConvert.DeserializeObject<List<Ordenes>>(visualizarOrdJson);
                }
            }

            return visualizarOrd;
        }

        public void SendEmailAsync(List<Usuarios> lstAdm, string subject, string message, string ind_soc)
        {
            try
            {
                string remit = "";
                string clave = "";
                string seudo = "";

                if (ind_soc.Equals("EC30"))
                {
                    remit = "despachos@qcterminales.com.ec";
                    clave = "Desp@chos.2021";
                    seudo = "QCTerminalesEC";
                }
                else
                {
                    remit = "noreply@qcterminales.cl";
                    clave = "*qcTer2015*";
                    seudo = "QCTerminalesCL";
                }
                // Credentials
                var credentials = new NetworkCredential(remit, clave);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(remit, seudo),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                for (int i = 0; i < lstAdm.Count; i++)
                {
                    mail.To.Add(new MailAddress(lstAdm[i].usu_cor.ToString()));
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.office365.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        public string eliminarDetalleEntrega(JObject json)
        {
            string rpta = "";

            try
            {
                JArray jArrayDEntregaAct = (JArray)json["detallaEntrega"];

                for (int i = 0; i < jArrayDEntregaAct.Count; i++)
                {
                    try
                    {
                        Detalle_Entrega det_ent = _quimpacContex.Detalle_Entrega.Where(x => x.det_ent_cod == Convert.ToInt32(jArrayDEntregaAct[i]["det_ent_cod"])).FirstOrDefault();
                        _quimpacContex.Detalle_Entrega.Remove(det_ent);
                        _quimpacContex.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }

                rpta = "eliminado";

            }
            catch (Exception e)
            {
                rpta = e.Message.ToString();
            }

            return rpta;
        }

        public string cargaStockSAPSQL(JObject json)
        {
            string rpta = "";

            try
            {
                using (SqlConnection bdSql = new SqlConnection(_config.GetConnectionString("quimpac")))
                {
                    bdSql.Open();

                    string strsql = "select ";

                    DataTable usr = new DataTable();

                    SqlCommand bdComando = new SqlCommand(strsql, bdSql);
                    using (SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand)bdComando))
                    {
                        adapter.Fill(usr);
                        rpta = JsonConvert.SerializeObject(usr, Formatting.Indented);
                        rpta = rpta.Replace('[', ' ').Replace(']', ' ');
                    }
                }

            }
            catch (Exception e)
            {
                rpta = e.Message.ToString();
            }

            return rpta;
        }

        public void SendEmailQC(List<String> lstDestinatarios, string subject, string message, string ind_soc)
        {
            try
            {
                string remit = "";
                string clave = "";
                string seudo = "";

                if (ind_soc.Equals("EC30"))
                {
                    remit = "despachos@qcterminales.com.ec";
                    clave = "Desp@chos.2021";
                    seudo = "QCTerminalesEC";
                }
                else
                {
                    remit = "noreply@qcterminales.cl";
                    clave = "*qcTer2015*";
                    seudo = "QCTerminalesCL";
                }
                // Credentials
                var credentials = new NetworkCredential(remit, clave);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(remit, seudo),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                for (int i = 0; i < lstDestinatarios.Count; i++)
                {
                    mail.To.Add(new MailAddress(lstDestinatarios[i].ToString()));
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.office365.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        public List<Constantes> obtenerConstante(string codigo)
        {

            List<Constantes> rpta = new List<Constantes>();

            try
            {
                rpta = _quimpacContex.Constantes.Where(x => x.ide_constante == codigo).ToList();
            }
            catch
            {
                rpta = new List<Constantes>();
            }

            return rpta;

        }

        public async Task<List<Combo>> postCombo(OpcionCentro r)
        {
            var w = new List<Combo>();
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                using (var c = new SqlCommand("qct_opcion_sp_get_combo", cnx))
                {
                    c.CommandType = CommandType.StoredProcedure;
                    c.Parameters.Add(new SqlParameter("@str_opcion", r.Opcion));
                    c.Parameters.Add(new SqlParameter("@str_centro", r.Centro));
                    await cnx.OpenAsync();

                    using (var reader = (SqlDataReader)await c.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            w.Add(new Combo()
                            {
                                Cod = HelperDAL.String(reader["cod"]),
                                Dsc = HelperDAL.String(reader["dsc"]),
                            });
                        }
                    }
                }
            }
            return w;
        }

        public string cargaOrdenes(JObject jsonObject)
        {
            string rpta = "";

            string pagina = "";
            string paginas = "";
            string factor = "";
            string intervalo = "";
            string hora = "";
            string estado = "";
            string tabla = "";
            JArray jsonArray = new JArray();

            try
            {
                pagina = jsonObject["pagina"].ToString();
                paginas = jsonObject["paginas"].ToString();
                factor = jsonObject["factor"].ToString();
                intervalo = jsonObject["intervalo"].ToString();
                hora = jsonObject["hora"].ToString();
                estado = jsonObject["estado"].ToString();
                jsonArray = (JArray)jsonObject["contenido"];

                using (SqlConnection bdSql = new SqlConnection(_connectionString))
                {
                    bdSql.Open();
                    SqlCommand bdComando = new SqlCommand();

                    if (pagina.Equals("1"))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", ""));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "C"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "ordenes"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                        estado = bdComando.Parameters[3].Value.ToString();

                        if (estado.Equals("1"))
                        {
                            estado = "2";
                        }
                        else if (estado.Equals("2"))
                        {
                            estado = "1";
                        }

                        bdComando = new SqlCommand("sp_EliminarOrdenesXDias", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.Parameters.Add(new SqlParameter("@rango", Convert.ToInt32(intervalo)));
                        bdComando.ExecuteNonQuery();
                    }

                    if (estado.Equals("1"))
                    {
                        tabla = "ordenes";
                    }
                    else if (estado.Equals("2"))
                    {
                        tabla = "ordenes_aux";
                    }

                    using (SqlTransaction transaction = bdSql.BeginTransaction())
                    {
                        using (var command = new SqlCommand())
                        {
                            command.Connection = bdSql;
                            command.Transaction = transaction;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into " + tabla + " (ord_cod_pro,ord_des_pro,ord_cod_cen," +
                                "ord_des_cen,ord_com,ord_num_mat,ord_des_mat,ord_val_2,ord_fec_fin_pla,ord_fec_reg," +
                                "ord_hor_reg,ord_can_pro,ord_uni_med,ord_des,ord_ori,ord_est_doc,ord_fec_hor_act_sap," +
                                "ord_est,ord_fec_cre,ord_guia,ord_cod_cho,ord_chofer,ord_cod_soc,ord_cod_ser_pre)" +
                                " values (@ord_cod_pro,@ord_des_pro,@ord_cod_cen,@ord_des_cen,@ord_com," +
                                "@ord_num_mat,@ord_des_mat,@ord_val_2,@ord_fec_fin_pla,@ord_fec_reg,@ord_hor_reg," +
                                "@ord_can_pro,@ord_uni_med,@ord_des,@ord_ori,@ord_est_doc,@ord_fec_hor_act_sap," +
                                "@ord_est,@ord_fec_cre,@ord_guia,@ord_cod_cho,@ord_chofer,@ord_cod_soc,@ord_cod_ser_pre)";

                            DateTime fhActual = DateTime.Now;

                            command.Parameters.Add("@ord_cod_pro", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_des_pro", SqlDbType.NVarChar, 70);
                            command.Parameters.Add("@ord_cod_cen", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_des_cen", SqlDbType.NVarChar, 30);
                            command.Parameters.Add("@ord_com", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_num_mat", SqlDbType.NVarChar, 18);
                            command.Parameters.Add("@ord_des_mat", SqlDbType.NVarChar, 40);
                            command.Parameters.Add("@ord_val_2", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@ord_fec_fin_pla", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_fec_reg", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_hor_reg", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_can_pro", SqlDbType.Decimal);
                            command.Parameters.Add("@ord_uni_med", SqlDbType.NVarChar, 3);
                            command.Parameters.Add("@ord_des", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@ord_ori", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@ord_est_doc", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@ord_fec_hor_act_sap", SqlDbType.DateTime);
                            command.Parameters.Add("@ord_est", SqlDbType.NVarChar, 1);
                            command.Parameters.Add("@ord_fec_cre", SqlDbType.DateTime);
                            command.Parameters.Add("@ord_guia", SqlDbType.NVarChar, 25);
                            command.Parameters.Add("@ord_cod_cho", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@ord_chofer", SqlDbType.NVarChar, 35);
                            command.Parameters.Add("@ord_cod_soc", SqlDbType.NVarChar, 4);
                            command.Parameters.Add("@ord_cod_ser_pre", SqlDbType.NVarChar, 6);

                            try
                            {
                                for (int i = 0; i < jsonArray.Count; i++)
                                {
                                    command.Parameters["@ord_cod_pro"].Value = jsonArray[i]["ord_cod_pro"].ToString();
                                    command.Parameters["@ord_des_pro"].Value = jsonArray[i]["ord_des_pro"].ToString();
                                    command.Parameters["@ord_cod_cen"].Value = jsonArray[i]["ord_cod_cen"].ToString();
                                    command.Parameters["@ord_des_cen"].Value = jsonArray[i]["ord_des_cen"].ToString();
                                    command.Parameters["@ord_com"].Value = jsonArray[i]["ord_com"].ToString();
                                    command.Parameters["@ord_num_mat"].Value = jsonArray[i]["ord_num_mat"].ToString();
                                    command.Parameters["@ord_des_mat"].Value = jsonArray[i]["ord_des_mat"].ToString();
                                    command.Parameters["@ord_val_2"].Value = jsonArray[i]["ord_val_2"].ToString();
                                    command.Parameters["@ord_fec_fin_pla"].Value = jsonArray[i]["ord_fec_fin_pla"].ToString();
                                    command.Parameters["@ord_fec_reg"].Value = jsonArray[i]["ord_fec_reg"].ToString();
                                    command.Parameters["@ord_hor_reg"].Value = jsonArray[i]["ord_hor_reg"].ToString();
                                    command.Parameters["@ord_can_pro"].Value = decimal.Parse(jsonArray[i]["ord_can_pro"].ToString());
                                    command.Parameters["@ord_uni_med"].Value = jsonArray[i]["ord_uni_med"].ToString();
                                    command.Parameters["@ord_des"].Value = jsonArray[i]["ord_des"].ToString();
                                    command.Parameters["@ord_ori"].Value = jsonArray[i]["ord_ori"].ToString();
                                    command.Parameters["@ord_est_doc"].Value = jsonArray[i]["ord_est_doc"].ToString();
                                    command.Parameters["@ord_fec_hor_act_sap"].Value = DateTime.Parse(jsonArray[i]["ord_fec_hor_act_sap"].ToString());
                                    command.Parameters["@ord_est"].Value = "";
                                    command.Parameters["@ord_fec_cre"].Value = fhActual;
                                    command.Parameters["@ord_guia"].Value = jsonArray[i]["ord_guia"].ToString();
                                    command.Parameters["@ord_cod_cho"].Value = jsonArray[i]["ord_cod_cho"].ToString();
                                    command.Parameters["@ord_chofer"].Value = jsonArray[i]["ord_chofer"].ToString();
                                    command.Parameters["@ord_cod_soc"].Value = jsonArray[i]["ord_cod_soc"].ToString();
                                    command.Parameters["@ord_cod_ser_pre"].Value = jsonArray[i]["ord_cod_ser_pre"].ToString();
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                rpta = ex.Message.ToString();
                                transaction.Rollback();
                                bdSql.Close();
                                throw new InvalidOperationException(ex.Message);
                            }
                        }
                    }

                    if (pagina.Equals(paginas) && rpta.Equals(""))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "A"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "ordenes"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                    }

                    if (rpta.Equals(""))
                    {
                        rpta = estado;
                    }

                    bdSql.Close();
                }
            }
            catch (Exception ex)
            {
                rpta = ex.Message.ToString();
                throw new InvalidOperationException(ex.Message);
            }

            return rpta;
        }

        public string cargaMovimientos(JObject jsonObject)
        {
            string rpta = "";

            string pagina = "";
            string paginas = "";
            string factor = "";
            string intervalo = "";
            string hora = "";
            string estado = "";
            string tabla = "";
            JArray jsonArray = new JArray();

            try
            {
                pagina = jsonObject["pagina"].ToString();
                paginas = jsonObject["paginas"].ToString();
                factor = jsonObject["factor"].ToString();
                intervalo = jsonObject["intervalo"].ToString();
                hora = jsonObject["hora"].ToString();
                estado = jsonObject["estado"].ToString();
                jsonArray = (JArray)jsonObject["contenido"];

                using (SqlConnection bdSql = new SqlConnection(_connectionString))
                {
                    bdSql.Open();
                    SqlCommand bdComando = new SqlCommand();

                    if (pagina.Equals("1"))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", ""));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "C"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "movimientos"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                        estado = bdComando.Parameters[3].Value.ToString();

                        if (estado.Equals("1"))
                        {
                            estado = "2";
                        }
                        else if (estado.Equals("2"))
                        {
                            estado = "1";
                        }

                        bdComando = new SqlCommand("sp_EliminarMovimientosXDias", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.Parameters.Add(new SqlParameter("@rango", Convert.ToInt32(intervalo)));
                        bdComando.ExecuteNonQuery();

                    }

                    if (estado.Equals("1"))
                    {
                        tabla = "resumen_stock_cliente";
                    }
                    else if (estado.Equals("2"))
                    {
                        tabla = "resumen_stock_cliente_aux";
                    }

                    using (SqlTransaction transaction = bdSql.BeginTransaction())
                    {
                        using (var command = new SqlCommand())
                        {
                            command.Connection = bdSql;
                            command.Transaction = transaction;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into " + tabla + " (stk_cli_cod_cen,stk_cli_des_cen," +
                                "stk_cli_cod_cli,stk_cli_des_cli,stk_cli_cod_alm,stk_cli_des_alm,stk_cli_num_mat," +
                                "stk_cli_des_mat,stk_cli_uni_med,stk_cli_num_lot,stk_cli_fec_mov,stk_cli_ser_pre," +
                                "stk_cli_can,stk_cli_buq,stk_cli_fec_lle,stk_cli_hor_lle,stk_cli_fec_sal," +
                                "stk_cli_hor_sal,stk_cli_ord,stk_cli_fec_hor_act_sap,stk_cli_est,stk_cli_fec_cre," +
                                "stk_cli_guia,stk_cli_cod_cho,stk_cli_chofer,stk_cli_cod_soc,stk_cli_cod_ser_pre)" +
                                " values (@stk_cli_cod_cen,@stk_cli_des_cen,@stk_cli_cod_cli,@stk_cli_des_cli," +
                                "@stk_cli_cod_alm,@stk_cli_des_alm,@stk_cli_num_mat,@stk_cli_des_mat," +
                                "@stk_cli_uni_med,@stk_cli_num_lot,@stk_cli_fec_mov,@stk_cli_ser_pre," +
                                "@stk_cli_can,@stk_cli_buq,@stk_cli_fec_lle,@stk_cli_hor_lle,@stk_cli_fec_sal," +
                                "@stk_cli_hor_sal,@stk_cli_ord,@stk_cli_fec_hor_act_sap,@stk_cli_est,@stk_cli_fec_cre," +
                                "@stk_cli_guia,@stk_cli_cod_cho,@stk_cli_chofer,@stk_cli_cod_soc," +
                                "@stk_cli_cod_ser_pre)";

                            DateTime fhActual = DateTime.Now;

                            command.Parameters.Add("@stk_cli_cod_cen", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@stk_cli_des_cen", SqlDbType.NVarChar, 70);
                            command.Parameters.Add("@stk_cli_cod_cli", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_des_cli", SqlDbType.NVarChar, 70);
                            command.Parameters.Add("@stk_cli_cod_alm", SqlDbType.NVarChar, 4);
                            command.Parameters.Add("@stk_cli_des_alm", SqlDbType.NVarChar, 50);
                            command.Parameters.Add("@stk_cli_num_mat", SqlDbType.NVarChar, 18);
                            command.Parameters.Add("@stk_cli_des_mat", SqlDbType.NVarChar, 40);
                            command.Parameters.Add("@stk_cli_uni_med", SqlDbType.NVarChar, 3);
                            command.Parameters.Add("@stk_cli_num_lot", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_fec_mov", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_ser_pre", SqlDbType.NVarChar, 50);
                            command.Parameters.Add("@stk_cli_can", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_cli_buq", SqlDbType.NVarChar, 30);
                            command.Parameters.Add("@stk_cli_fec_lle", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_hor_lle", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_fec_sal", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_hor_sal", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_ord", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_cli_fec_hor_act_sap", SqlDbType.DateTime);
                            command.Parameters.Add("@stk_cli_est", SqlDbType.Char, 1);
                            command.Parameters.Add("@stk_cli_fec_cre", SqlDbType.DateTime);
                            command.Parameters.Add("@stk_cli_guia", SqlDbType.VarChar, 25);
                            command.Parameters.Add("@stk_cli_cod_cho", SqlDbType.VarChar, 10);
                            command.Parameters.Add("@stk_cli_chofer", SqlDbType.VarChar, 35);
                            command.Parameters.Add("@stk_cli_cod_soc", SqlDbType.VarChar, 4);
                            command.Parameters.Add("@stk_cli_cod_ser_pre", SqlDbType.VarChar, 6);

                            try
                            {
                                for (int i = 0; i < jsonArray.Count; i++)
                                {
                                    command.Parameters["@stk_cli_cod_cen"].Value = jsonArray[i]["stk_cli_cod_cen"].ToString();
                                    command.Parameters["@stk_cli_des_cen"].Value = jsonArray[i]["stk_cli_des_cen"].ToString();
                                    command.Parameters["@stk_cli_cod_cli"].Value = jsonArray[i]["stk_cli_cod_cli"].ToString();
                                    command.Parameters["@stk_cli_des_cli"].Value = jsonArray[i]["stk_cli_des_cli"].ToString();
                                    command.Parameters["@stk_cli_cod_alm"].Value = jsonArray[i]["stk_cli_cod_alm"].ToString();
                                    command.Parameters["@stk_cli_des_alm"].Value = jsonArray[i]["stk_cli_des_alm"].ToString();
                                    command.Parameters["@stk_cli_num_mat"].Value = jsonArray[i]["stk_cli_num_mat"].ToString();
                                    command.Parameters["@stk_cli_des_mat"].Value = jsonArray[i]["stk_cli_des_mat"].ToString();
                                    command.Parameters["@stk_cli_uni_med"].Value = jsonArray[i]["stk_cli_uni_med"].ToString();
                                    command.Parameters["@stk_cli_num_lot"].Value = jsonArray[i]["stk_cli_num_lot"].ToString();
                                    command.Parameters["@stk_cli_fec_mov"].Value = jsonArray[i]["stk_cli_fec_mov"].ToString();
                                    command.Parameters["@stk_cli_ser_pre"].Value = jsonArray[i]["stk_cli_ser_pre"].ToString();
                                    command.Parameters["@stk_cli_can"].Value = decimal.Parse(jsonArray[i]["stk_cli_can"].ToString());
                                    command.Parameters["@stk_cli_buq"].Value = jsonArray[i]["stk_cli_buq"].ToString();
                                    command.Parameters["@stk_cli_fec_lle"].Value = jsonArray[i]["stk_cli_fec_lle"].ToString();
                                    command.Parameters["@stk_cli_hor_lle"].Value = jsonArray[i]["stk_cli_hor_lle"].ToString();
                                    command.Parameters["@stk_cli_fec_sal"].Value = jsonArray[i]["stk_cli_fec_sal"].ToString();
                                    command.Parameters["@stk_cli_hor_sal"].Value = jsonArray[i]["stk_cli_hor_sal"].ToString();
                                    command.Parameters["@stk_cli_ord"].Value = jsonArray[i]["stk_cli_ord"].ToString();
                                    command.Parameters["@stk_cli_fec_hor_act_sap"].Value = DateTime.Parse(jsonArray[i]["stk_cli_fec_hor_act_sap"].ToString());
                                    command.Parameters["@stk_cli_est"].Value = "";
                                    command.Parameters["@stk_cli_fec_cre"].Value = fhActual;
                                    command.Parameters["@stk_cli_guia"].Value = jsonArray[i]["stk_cli_guia"].ToString();
                                    command.Parameters["@stk_cli_cod_cho"].Value = jsonArray[i]["stk_cli_cod_cho"].ToString();
                                    command.Parameters["@stk_cli_chofer"].Value = jsonArray[i]["stk_cli_chofer"].ToString();
                                    command.Parameters["@stk_cli_cod_soc"].Value = jsonArray[i]["stk_cli_cod_soc"].ToString();
                                    command.Parameters["@stk_cli_cod_ser_pre"].Value = jsonArray[i]["stk_cli_cod_ser_pre"].ToString();
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                rpta = ex.Message.ToString();
                                transaction.Rollback();
                                bdSql.Close();
                                throw new InvalidOperationException(ex.Message);
                            }
                        }
                    }

                    if (pagina.Equals(paginas) && rpta.Equals(""))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "A"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "movimientos"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                    }

                    if (rpta.Equals(""))
                    {
                        rpta = estado;    
                    }

                    bdSql.Close();
                }
            }
            catch (Exception ex)
            {
                rpta = ex.Message.ToString();
                throw new InvalidOperationException(ex.Message);
            }

            return rpta;
        }
        public string cargaStock(JObject jsonObject)
        {
            string rpta = "";

            string pagina = "";
            string paginas = "";
            string factor = "";
            string intervalo = "";
            string hora = "";
            string estado = "";
            string tabla = "";
            JArray jsonArray = new JArray();

            try
            {
                pagina = jsonObject["pagina"].ToString();
                paginas = jsonObject["paginas"].ToString();
                factor = jsonObject["factor"].ToString();
                intervalo = jsonObject["intervalo"].ToString();
                hora = jsonObject["hora"].ToString();
                estado = jsonObject["estado"].ToString();
                jsonArray = (JArray)jsonObject["contenido"];

                using (SqlConnection bdSql = new SqlConnection(_connectionString))
                {
                    bdSql.Open();
                    SqlCommand bdComando = new SqlCommand();

                    if (pagina.Equals("1"))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", ""));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "C"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "stock"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                        estado = bdComando.Parameters[3].Value.ToString();

                        if (estado.Equals("1"))
                        {
                            estado = "2";
                        }
                        else if (estado.Equals("2"))
                        {
                            estado = "1";
                        }

                        bdComando = new SqlCommand("sp_EliminarStock", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.ExecuteNonQuery();
                    }

                    if (estado.Equals("1"))
                    {
                        tabla = "stock";
                    }
                    else if (estado.Equals("2"))
                    {
                        tabla = "stock_aux";
                    }

                    using (SqlTransaction transaction = bdSql.BeginTransaction())
                    {
                        using (var command = new SqlCommand())
                        {
                            command.Connection = bdSql;
                            command.Transaction = transaction;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into " + tabla + " (stk_cod_cli,stk_des_cli,stk_cod_cen," +
                                "stk_des_cen,stk_cod_alm,stk_des_alm,stk_num_mat,stk_des_mat,stk_uni_med,stk_num_lot," +
                                "stk_sto_dis,stk_ent_sap,stk_sto_nna,stk_sto_tra,stk_sto_blo,stk_fec_hor_act,stk_est," +
                                "stk_num_mat_cli,stk_peso_equi,stk_cod_soc)" +
                                " values (@stk_cod_cli,@stk_des_cli,@stk_cod_cen,@stk_des_cen,@stk_cod_alm," +
                                "@stk_des_alm,@stk_num_mat,@stk_des_mat,@stk_uni_med,@stk_num_lot,@stk_sto_dis," +
                                "@stk_ent_sap,@stk_sto_nna,@stk_sto_tra,@stk_sto_blo,@stk_fec_hor_act,@stk_est," +
                                "@stk_num_mat_cli,@stk_peso_equi,@stk_cod_soc)";

                            command.Parameters.Add("@stk_cod_cli", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_des_cli", SqlDbType.NVarChar, 70);
                            command.Parameters.Add("@stk_cod_cen", SqlDbType.NVarChar, 4);
                            command.Parameters.Add("@stk_des_cen", SqlDbType.NVarChar, 30);
                            command.Parameters.Add("@stk_cod_alm", SqlDbType.NVarChar, 4);
                            command.Parameters.Add("@stk_des_alm", SqlDbType.NVarChar, 16);
                            command.Parameters.Add("@stk_num_mat", SqlDbType.NVarChar, 18);
                            command.Parameters.Add("@stk_des_mat", SqlDbType.NVarChar, 40);
                            command.Parameters.Add("@stk_uni_med", SqlDbType.NVarChar, 3);
                            command.Parameters.Add("@stk_num_lot", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@stk_sto_dis", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_ent_sap", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_sto_nna", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_sto_tra", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_sto_blo", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_fec_hor_act", SqlDbType.DateTime);
                            command.Parameters.Add("@stk_est", SqlDbType.NVarChar, 1);
                            command.Parameters.Add("@stk_num_mat_cli", SqlDbType.NVarChar, 18);
                            command.Parameters.Add("@stk_peso_equi", SqlDbType.Decimal);
                            command.Parameters.Add("@stk_cod_soc", SqlDbType.NVarChar, 4);

                            try
                            {
                                for (int i = 0; i < jsonArray.Count; i++)
                                {
                                    command.Parameters["@stk_cod_cli"].Value = jsonArray[i]["stk_cod_cli"].ToString();
                                    command.Parameters["@stk_des_cli"].Value = jsonArray[i]["stk_des_cli"].ToString();
                                    command.Parameters["@stk_cod_cen"].Value = jsonArray[i]["stk_cod_cen"].ToString();
                                    command.Parameters["@stk_des_cen"].Value = jsonArray[i]["stk_des_cen"].ToString();
                                    command.Parameters["@stk_cod_alm"].Value = jsonArray[i]["stk_cod_alm"].ToString();
                                    command.Parameters["@stk_des_alm"].Value = jsonArray[i]["stk_des_alm"].ToString();
                                    command.Parameters["@stk_num_mat"].Value = jsonArray[i]["stk_num_mat"].ToString();
                                    command.Parameters["@stk_des_mat"].Value = jsonArray[i]["stk_des_mat"].ToString();
                                    command.Parameters["@stk_uni_med"].Value = jsonArray[i]["stk_uni_med"].ToString();
                                    command.Parameters["@stk_num_lot"].Value = jsonArray[i]["stk_num_lot"].ToString();
                                    command.Parameters["@stk_sto_dis"].Value = decimal.Parse(jsonArray[i]["stk_sto_dis"].ToString());
                                    command.Parameters["@stk_ent_sap"].Value = decimal.Parse(jsonArray[i]["stk_ent_sap"].ToString());
                                    command.Parameters["@stk_sto_nna"].Value = decimal.Parse(jsonArray[i]["stk_sto_nna"].ToString());
                                    command.Parameters["@stk_sto_tra"].Value = decimal.Parse(jsonArray[i]["stk_sto_tra"].ToString());
                                    command.Parameters["@stk_sto_blo"].Value = decimal.Parse(jsonArray[i]["stk_sto_blo"].ToString());
                                    command.Parameters["@stk_fec_hor_act"].Value = DateTime.Parse(jsonArray[i]["stk_fec_hor_act"].ToString());
                                    command.Parameters["@stk_est"].Value = "";
                                    command.Parameters["@stk_num_mat_cli"].Value = jsonArray[i]["stk_num_mat_cli"].ToString();
                                    command.Parameters["@stk_peso_equi"].Value = jsonArray[i]["stk_peso_equi"].ToString();
                                    command.Parameters["@stk_cod_soc"].Value = jsonArray[i]["stk_cod_soc"].ToString();
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                rpta = ex.Message.ToString();
                                transaction.Rollback();
                                bdSql.Close();
                                throw new InvalidOperationException(ex.Message);
                            }
                        }
                    }

                    if (pagina.Equals(paginas) && rpta.Equals(""))
                    {
                        bdComando = new SqlCommand("sp_ControlActualizaciones", bdSql);
                        bdComando.CommandType = CommandType.StoredProcedure;
                        bdComando.Parameters.Add(new SqlParameter("@estado", estado));
                        bdComando.Parameters.Add(new SqlParameter("@operador", "A"));
                        bdComando.Parameters.Add(new SqlParameter("@codigo", "stock"));
                        bdComando.Parameters.Add(new SqlParameter("@message", SqlDbType.Char, 1));
                        bdComando.Parameters[3].Direction = ParameterDirection.Output;
                        bdComando.ExecuteNonQuery();
                    }

                    if (rpta.Equals(""))
                    {
                        rpta = estado;
                    }

                    bdSql.Close();
                }
            }
            catch (Exception ex)
            {
                rpta = ex.Message.ToString();
                throw new InvalidOperationException(ex.Message);
            }

            return rpta;
        }
    }
}
