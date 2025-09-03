using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Context;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Repositorys
{
    public class AdminRepository : IAdminRepository
    {
        private readonly QuimpacContext _quimpacContext;
        public AdminRepository(QuimpacContext quimpacContext)
        {
            _quimpacContext = quimpacContext;
        }

        public string actualizarCli(Clientes cli)
        {
            string rpta = "";
            
            Clientes cliente = _quimpacContext.Clientes.Where(x => x.cli_cod_cli.Equals(cli.cli_cod_cli)).FirstOrDefault();
            DateTime fechaModificar = DateTime.Now;

            cliente.cli_cod_cli = cli.cli_cod_cli;
            cliente.cli_raz_soc = cli.cli_raz_soc;
            cliente.cli_cod_sap = cli.cli_cod_sap;
            cliente.cli_pai = cli.cli_pai;
            cliente.cli_gru_cue = cli.cli_gru_cue;
            cliente.cli_est = cli.cli_est;
            cliente.cli_fec_cre = cli.cli_fec_cre;
            cliente.cli_usu_cre_sap = cli.cli_usu_cre_sap;
            cliente.cli_fec_mod = fechaModificar;
            cliente.cli_usu_mod_sap = cli.cli_usu_mod_sap;
           
            _quimpacContext.Clientes.Update(cliente);
            _quimpacContext.SaveChanges();
            rpta = "actualizo";
            return rpta;
        }

        public string actualizarPermiso(Permiso per)
        {
            string rpta = "";
            DateTime fechaModificar = DateTime.Now;
            Permiso permiso = new Permiso
            {
                per_cod = per.per_cod,
                per_nom = per.per_nom,
                per_uri = per.per_uri,
                per_uri_icon = per.per_uri_icon,
                per_est = per.per_est,
                per_fec_cre = per.per_fec_cre,
                per_usu_cre_sap = per.per_usu_cre_sap,
                per_fec_mod = fechaModificar,
                per_usu_mod_sap = per.per_usu_mod_sap
            };
            _quimpacContext.Permiso.Update(permiso);
            _quimpacContext.SaveChanges();
            rpta = "Se actualizo correctamente";
            return rpta;
        }

        public string actualizarRol(Rol rl)
        {
            string rpta = "";
            Rol rol = _quimpacContext.Rol.Where(x => x.rol_cod.Equals(rl.rol_cod)).FirstOrDefault();
            DateTime fechaModificar = DateTime.Now;

            rol.rol_cod = rl.rol_cod;
            rol.rol_nom = rl.rol_nom;
            rol.rol_cod_usu_sap = rl.rol_cod_usu_sap;
            rol.rol_est = rl.rol_est;
            rol.rol_fec_cre = rl.rol_fec_cre;
            rol.rol_usu_cre_sap = rl.rol_usu_cre_sap;
            rol.rol_fec_mod = fechaModificar;
            rol.rol_usu_mod_sap = rl.rol_usu_mod_sap;
            
            _quimpacContext.Rol.Update(rol);
            _quimpacContext.SaveChanges();
            rpta = "Se actualizo correctamente";
            return rpta;
        }

        public Chofer buscarChofer(string cod_sap)
        {
            var chofer = _quimpacContext.Chofer.Where(x => x.cho_cod_cho.Equals(cod_sap)).FirstOrDefault();
            return chofer;
        }

        public Clientes buscarCliente(string cod_sap)
        {
            var cliente = _quimpacContext.Clientes.Where(x => x.cli_cod_sap.Equals(cod_sap)).FirstOrDefault();
            return cliente;
        }

        public Permiso buscarPermiso(int cod_per)
        {
            var permiso = _quimpacContext.Permiso.Where(x => x.per_cod.Equals(cod_per)).FirstOrDefault();

            return permiso;
        }

        public Rol buscarRol(int cod_rol)
        {
            var rol = _quimpacContext.Rol.Where(x => x.rol_cod.Equals(cod_rol)).FirstOrDefault();
            return rol;
        }

        public List<Rol> buscarRolXsap(string cod_sap)
        {
            var rol = _quimpacContext.Rol.Where(x => x.rol_cod_usu_sap.Equals(cod_sap)).ToList();
            return rol;
        }

        // en pureba
        public List<UsuarioXrol> buscarUsuario(string cod_sap ,string cod_soc)
        {
            List<UsuarioXrol> usuario = new List<UsuarioXrol>();
            if (cod_soc.Equals("ALL"))
            {
                usuario = (from usu in _quimpacContext.Usuarios
                               join r in _quimpacContext.Rol
                               on usu.usu_cod_rol equals r.rol_cod
                               where usu.usu_cod_sap.Equals(cod_sap) 
                               orderby usu.usu_cod_usu descending
                               select new UsuarioXrol
                               {
                                   usu_cod_usu = usu.usu_cod_usu,
                                   usu_cod_cli = usu.usu_cod_cli,
                                   usu_cod_rol = usu.usu_cod_rol,
                                   usu_cod_soc = usu.usu_cod_soc,
                                   usu_nom_ape = usu.usu_nom_ape,
                                   usu_usu = usu.usu_usu,
                                   usu_cla = usu.usu_cla,
                                   usu_cor = usu.usu_cor,
                                   usu_cod_sap = usu.usu_cod_sap,
                                   usu_est = usu.usu_est,
                                   usu_fec_cre = usu.usu_fec_cre,
                                   usu_usu_cre_sap = usu.usu_usu_cre_sap,
                                   usu_fec_mod = usu.usu_fec_mod,
                                   usu_usu_mod_sap = usu.usu_usu_mod_sap,
                                   rol_nom = r.rol_nom
                               }).ToList();
            }
            else
            {
                usuario = (from usu in _quimpacContext.Usuarios
                           join r in _quimpacContext.Rol
                           on usu.usu_cod_rol equals r.rol_cod
                           where usu.usu_cod_sap.Equals(cod_sap) && usu.usu_cod_soc.Equals(cod_soc)
                           orderby usu.usu_cod_usu descending
                           select new UsuarioXrol
                           {
                               usu_cod_usu = usu.usu_cod_usu,
                               usu_cod_cli = usu.usu_cod_cli,
                               usu_cod_rol = usu.usu_cod_rol,
                               usu_cod_soc = usu.usu_cod_soc,
                               usu_nom_ape = usu.usu_nom_ape,
                               usu_usu = usu.usu_usu,
                               usu_cla = usu.usu_cla,
                               usu_cor = usu.usu_cor,
                               usu_cod_sap = usu.usu_cod_sap,
                               usu_est = usu.usu_est,
                               usu_fec_cre = usu.usu_fec_cre,
                               usu_usu_cre_sap = usu.usu_usu_cre_sap,
                               usu_fec_mod = usu.usu_fec_mod,
                               usu_usu_mod_sap = usu.usu_usu_mod_sap,
                               rol_nom = r.rol_nom
                           }).ToList();
            }

            return usuario;
        }

        public Placa buscarVehiculo(string placa)
        {
            var vehiculo = _quimpacContext.Placa.Where(x => x.pla_pla_veh.Equals(placa)).FirstOrDefault();
            return vehiculo;
        }

        public string crearCliente(Clientes cli)
        {
            string rpta = "";
            var verificarCli = _quimpacContext.Clientes;
            DateTime fechaCrear = DateTime.Now;
            Clientes cliente = new Clientes
            {
                cli_raz_soc = cli.cli_raz_soc,
                cli_cod_sap = cli.cli_cod_sap,
                cli_pai = cli.cli_pai,
                cli_gru_cue = cli.cli_gru_cue,
                cli_est = cli.cli_est,
                cli_fec_cre = fechaCrear,
                cli_usu_cre_sap = cli.cli_usu_cre_sap,
                cli_fec_mod = null,
                cli_usu_mod_sap = cli.cli_usu_mod_sap
            };

            //var c = "";
            int coda_sap = Convert.ToInt32(cliente.cli_cod_sap);
            var c = coda_sap.ToString("D10"); 
            string copsap_text = "";
            copsap_text = Convert.ToString(c);
            cliente.cli_cod_sap = copsap_text;

            //verifica si el cliente existe
            if (verificarCli.Any(x =>  x.cli_cod_sap.Equals(cliente.cli_cod_sap)))
            {
                rpta ="El código SAP ya existe";
            }
            else
            {
                
                _quimpacContext.Clientes.Add(cliente);
                _quimpacContext.SaveChanges();
                rpta = "creo";
            }
            return rpta;
        }

        public string crearPermiso(Permiso per)
        {
            string rpta = "";
            var verificarPer = _quimpacContext.Permiso;
            DateTime fechaCrear = DateTime.Now;
            Permiso permiso = new Permiso
            {
                per_nom = per.per_nom,
                per_uri = per.per_uri,
                per_uri_icon = per.per_uri_icon,
                per_est = per.per_est,
                per_fec_cre = fechaCrear,
                per_usu_cre_sap = per.per_usu_cre_sap,
                per_fec_mod = per.per_fec_mod,
                per_usu_mod_sap = per.per_usu_mod_sap
            };

            //verifica si existe el permiso
            if (verificarPer.Any(x => x.per_cod.Equals(per.per_cod) || x.per_nom.Equals(per.per_nom)))
            {
                rpta ="";
            }
            else
            {
                _quimpacContext.Permiso.Add(permiso);
                _quimpacContext.SaveChanges();
                rpta = "Se creo correctamente";
                
            }
            return rpta;
        }

        public List<Chofer> listarChofer()
        {
            var lista = _quimpacContext.Chofer.OrderByDescending(x => x.cho_cod).ToList();
            return lista;
        }

        public List<Clientes> listarClientes(string cli_org_ven)
        {
            if (cli_org_ven.Equals("0"))
            {
                cli_org_ven = "";
            }
            var clientes = _quimpacContext.Clientes.Where(x => x.cli_gru_cue.Contains(cli_org_ven)).OrderByDescending(x => x.cli_cod_cli).ToList();
            return clientes;
        }

        public List<Permiso> listarPermiso()
        {
            var permisos = _quimpacContext.Permiso.ToList();
            return permisos;
        }

        public List<Rol> listarRoles()
        {
            var roles = _quimpacContext.Rol.ToList();
            return roles;
        }

        public List<Usuarios> listarUsuario()
        {
            var result = "";
            var usuarios = _quimpacContext.Usuarios.ToList();
            
            return usuarios;
        }

        public List<Placa> listarVehiculo()
        {
            var listaVehiculo = _quimpacContext.Placa.OrderByDescending(x => x.pla_cod).ToList();
            return listaVehiculo;
        }

        public List<Permiso> ListarXestado(string estado)
        {

            var listaEstados = _quimpacContext.Permiso;
            if(estado == "2")
            {
                return listaEstados.Where(s =>s.per_est.Equals(estado)).Select(r =>r).ToList();
            }
            else
            {
                return listaEstados.ToList();
            }
            //return listaEstados;
        }

        public List<Resumen_stock_Cliente> listaStockCliente()
        {
            DateTime horaFecha = DateTime.Today;
            var fecha = horaFecha.Date;
            var hora = horaFecha.TimeOfDay;

            List<Resumen_stock_Cliente> listaStockCliente = _quimpacContext.Resumen_Stock_Cliente.Select(a => a).ToList();

            return listaStockCliente;
        }

        public string newChofer(Chofer cho)
        {
            string rpta = "";
            var verificarCho = _quimpacContext.Chofer;
            DateTime fechaCrear = DateTime.Now;
            Chofer chofer = new Chofer
            {
                cho_gru_cue = cho.cho_gru_cue,
                cho_cod_cho = cho.cho_cod_cho,
                cho_nom = cho.cho_nom,
                cho_con_bus = cho.cho_con_bus,
                cho_pai = cho.cho_pai,
                cho_idi = cho.cho_idi,
                cho_num_ide_fis = cho.cho_num_ide_fis,
                cho_num_tip_nif = cho.cho_num_tip_nif,
                cho_num_per_fis = cho.cho_num_per_fis,
                cho_cia_imp = cho.cho_cia_imp,
                cho_est_reg = "",
                cho_des_err_sap = "",
                cho_est = "0",
                cho_fec_cre = fechaCrear,
                cho_fec_hor_cre_sap = null,
                cho_usu_cre_sap = cho.cho_usu_cre_sap,
                cho_fec_mod = null,
                cho_fec_hor_mod_sap = null,
                cho_usu_mod_sap = null
            };

            if (verificarCho.Any(x => x.cho_cod_cho.Equals(cho.cho_cod_cho)))
            {
                rpta = "Existe código de chofer";
            }
            else if (verificarCho.Any(x => x.cho_num_ide_fis.Equals(cho.cho_num_ide_fis)))
            {
                rpta = "Existe número de Identificación fiscal";
            }
            else
            {
                _quimpacContext.Chofer.Add(chofer);
                _quimpacContext.SaveChanges();
                rpta ="creado";
            }
            return rpta;
        }

        public string newRol(JObject json)
        {
            string rpta = "";
            var verificarRol = _quimpacContext.Rol;
            DateTime fechaCrear = DateTime.Now;
            JObject jsonRol = (JObject)json["rol"];
            Rol rol = new Rol
            {
                rol_nom = jsonRol["rol_nom"].ToString(),
                rol_cod_usu_sap = jsonRol["rol_cod_usu_sap"].ToString(),
                rol_est = jsonRol["rol_est"].ToString(),
                rol_fec_cre = Convert.ToDateTime(jsonRol["rol_fec_cre"].ToString()),
                rol_usu_cre_sap = jsonRol["rol_usu_cre_sap"].ToString(),
                rol_fec_mod = Convert.ToDateTime(jsonRol["rol_fec_mod"].ToString()),
                rol_usu_mod_sap = jsonRol["rol_usu_mod_sap"].ToString()
            };

            //verifica si existe el cod de rol y el nombre de rol , si no existe se registrará un rol nuevo
            if (verificarRol.Any(x => x.rol_nom.Equals(jsonRol["rol_nom"].ToString()) && x.rol_cod.Equals(jsonRol["rol_cod"])))
            {
                rpta ="existe";
                
            }
            else
            {
                _quimpacContext.Rol.Add(rol);
                _quimpacContext.SaveChanges();

                JArray jarrayRolPermiso = (JArray)json["rol_permiso"];
                for(int i = 0; i < jarrayRolPermiso.Count; i++)
                {
                    Rol_permiso rol_permiso = new Rol_permiso
                    {
                        rol_per_cod_rol = rol.rol_cod,
                        rol_per_cod_per = Convert.ToInt32(jarrayRolPermiso[i]["rol_per_cod_per"].ToString()),
                        rol_per_est = jarrayRolPermiso[i]["rol_per_est"].ToString(),
                        rol_per_fec_cre = Convert.ToDateTime(jarrayRolPermiso[i]["rol_per_fec_cre"].ToString()),
                        rol_per_usu_cre_sap = jarrayRolPermiso[i]["rol_per_usu_cre_sap"].ToString(),
                        rol_per_fec_mod = Convert.ToDateTime(jarrayRolPermiso[i]["rol_per_fec_mod"].ToString()),
                        rol_per_usu_mod_sap = jarrayRolPermiso[i]["rol_per_usu_mod_sap"].ToString(),
                    };
                    _quimpacContext.Rol_Permiso.Add(rol_permiso);
                    _quimpacContext.SaveChanges();
                }
               
                rpta ="correctamente";
            }
            return rpta;
        }


        public string newUsuario(Usuarios nu)
        {
            string result = string.Empty;
            string rpta = "";
            //var validaCli = _quimpacContext.Clientes;
            var usuariosExist = _quimpacContext.Usuarios;
            DateTime fechaHora = DateTime.Now;
            var usuario = new Usuarios
            {
                usu_cod_cli = nu.usu_cod_cli,
                usu_cod_rol = nu.usu_cod_rol,
                usu_cod_soc = nu.usu_cod_soc,
                usu_nom_ape = nu.usu_nom_ape,
                usu_usu = nu.usu_usu,
                usu_cla = nu.usu_cla,
                usu_cor = nu.usu_cor,
                usu_cod_sap = nu.usu_cod_sap,
                usu_est = nu.usu_est,
                usu_fec_cre = fechaHora,
                usu_usu_cre_sap = nu.usu_usu_cre_sap,
                usu_fec_mod = null,
                usu_usu_mod_sap = nu.usu_usu_mod_sap
            };
            
            
                if (usuariosExist.Any(x =>(x.usu_usu.Equals(nu.usu_usu) && x.usu_cod_cli.Equals(nu.usu_cod_cli)) || (x.usu_usu.Equals(nu.usu_usu) && x.usu_cod_cli !=nu.usu_cod_cli)))
                {
                    rpta = "Usuario ya registrado , probar con otro";
                }
                else
                {
                    if (nu.usu_cod_rol.Equals(0))
                    {
                        var rol = new Rol
                        {
                            rol_nom = "Administrador de cliente",
                            rol_cod_usu_sap = nu.usu_cod_sap,
                            rol_est = "1",
                            rol_fec_cre = fechaHora,
                            rol_usu_cre_sap = "0"
                        };

                    //byte[] encryted = System.Text.Encoding.Unicode.GetBytes(nu.usu_cla);
                    //result = Convert.ToBase64String(encryted);

                        _quimpacContext.Rol.Add(rol);
                        _quimpacContext.SaveChanges();
                        usuario.usu_cod_rol = rol.rol_cod;
                        //usuario.usu_cla = result;
                        _quimpacContext.Usuarios.Add(usuario);
                        _quimpacContext.SaveChanges();

                        List<Permiso> permisos = _quimpacContext.Permiso.Where(x => x.per_est.Equals("2")).ToList();

                        for (int i = 0; i < permisos.LongCount(); i++)
                        {
                            var rol_permiso = new Rol_permiso
                            {
                                rol_per_cod_rol = rol.rol_cod,
                                rol_per_cod_per = permisos[i].per_cod,
                                rol_per_est = "1",
                                rol_per_fec_cre = fechaHora,
                                rol_per_usu_cre_sap = "0"
                            };
                            _quimpacContext.Rol_Permiso.Add(rol_permiso);
                            _quimpacContext.SaveChanges();
                        rpta = "administrador";
                        }
                    }
                    else 
                    {
                    //byte[] encryted = System.Text.Encoding.Unicode.GetBytes(nu.usu_cla);
                    //result = Convert.ToBase64String(encryted);
                    //usuario.usu_cla = result;
                    _quimpacContext.Usuarios.Add(usuario);
                        _quimpacContext.SaveChanges();
                        rpta = "usuario";
                    }
               
                }
            

            return rpta;
        }

        public string newVehiculo(Placa veh)
        {
            string rpta = "";
            DateTime fechaCrear = DateTime.Now;
            Placa vehiculo = new Placa
            {
                pla_gru_cue = veh.pla_gru_cue,
                pla_pla_veh = veh.pla_pla_veh,
                pla_nom = veh.pla_nom,
                pla_con_bus = veh.pla_con_bus,
                pla_pai = veh.pla_pai,
                pla_idi = veh.pla_idi,
                pla_est_reg = "",
                pla_des_err_sap="",
                pla_est = "0",
                pla_fec_cre = fechaCrear,
                pla_fec_hor_cre_sap=null,
                pla_usu_cre_sap = veh.pla_usu_cre_sap,
                pla_fec_mod = null,
                pla_fec_hor_mod_sap=null,
                pla_usu_mod_sap = null
            };
            var varificarVeh = _quimpacContext.Placa;
            if (varificarVeh.Any(x => x.pla_pla_veh.Equals(veh.pla_pla_veh)))
            {
                rpta = "Placa ya existe";
            }
            else 
            {
                _quimpacContext.Placa.Add(vehiculo);
                _quimpacContext.SaveChanges();
                rpta = "correcto";

            }
            return rpta;

        }

        public string updateChofer(Chofer cho)
        {
            var validarEst = _quimpacContext.Chofer;
            Chofer chofer = _quimpacContext.Chofer.Where(w => w.cho_cod.Equals(cho.cho_cod)).FirstOrDefault();
            string rpta = "";
            DateTime fechaUpdate = DateTime.Now;

            chofer.cho_gru_cue = cho.cho_gru_cue;
            chofer.cho_cod_cho = cho.cho_cod_cho;
            chofer.cho_nom = cho.cho_nom;
            chofer.cho_con_bus = cho.cho_con_bus;
            chofer.cho_pai = cho.cho_pai;
            chofer.cho_idi = cho.cho_idi;
            chofer.cho_num_ide_fis = cho.cho_num_ide_fis;
            chofer.cho_num_tip_nif = cho.cho_num_tip_nif;
            chofer.cho_num_per_fis = cho.cho_num_per_fis;
            chofer.cho_des_err_sap = cho.cho_des_err_sap;
            chofer.cho_cia_imp = cho.cho_cia_imp;
            chofer.cho_fec_mod = fechaUpdate;
            chofer.cho_usu_mod_sap = cho.cho_usu_mod_sap;


            if (validarEst.Any(x => x.cho_num_per_fis.Equals(cho.cho_num_per_fis) && x.cho_num_ide_fis.Equals(cho.cho_num_ide_fis) && x.cho_cod != cho.cho_cod))
            {
                rpta = "Existe número de nif";
            }
            else if ((chofer.cho_est_reg.Equals("S") && chofer.cho_est.Equals("1")) || (chofer.cho_est_reg.Equals("E") && chofer.cho_est.Equals("0")) || (chofer.cho_est_reg.Equals("W") && chofer.cho_est.Equals("0")))
            {
                if (chofer.cho_est_reg.Equals("E"))
                {
                    chofer.cho_est_reg = "";
                }
                else
                {
                    chofer.cho_est_reg = "M";
                }
                chofer.cho_est = "0";
                _quimpacContext.Chofer.Update(chofer);
                _quimpacContext.SaveChanges();
                rpta = "actualizado";
            }
            else
            {
                rpta = "No se puede actualizar por el momento";
            }
                

            return rpta;

        }

        public string updateUsuario(Usuarios un)
        {
            string result = string.Empty;
            string rpta = "";
            Usuarios usuario = _quimpacContext.Usuarios.Where(x => x.usu_cod_usu.Equals(un.usu_cod_usu)).FirstOrDefault();
            DateTime fechaActualizar = DateTime.Now;

            //byte[] encryted = System.Text.Encoding.Unicode.GetBytes(un.usu_cla);
            //result = Convert.ToBase64String(encryted);
            //usuario.usu_cla = result;

            usuario.usu_cod_usu = un.usu_cod_usu;
            usuario.usu_cod_cli = un.usu_cod_cli;
            usuario.usu_cod_rol = un.usu_cod_rol;
            usuario.usu_cod_soc = un.usu_cod_soc;
            usuario.usu_nom_ape = un.usu_nom_ape;
            usuario.usu_usu = un.usu_usu;
            usuario.usu_cla = un.usu_cla;
            usuario.usu_cor = un.usu_cor;
            usuario.usu_cod_sap = un.usu_cod_sap;
            usuario.usu_est = un.usu_est;
            usuario.usu_fec_cre = un.usu_fec_cre;
            usuario.usu_usu_cre_sap = un.usu_usu_cre_sap;
            usuario.usu_fec_mod = fechaActualizar;
            usuario.usu_usu_mod_sap = un.usu_usu_mod_sap;
          
            var verficar = _quimpacContext.Usuarios;

            /*if(verficar.Any(x => (x.usu_usu.Equals(un.usu_usu) && x.usu_cod_cli != un.usu_cod_cli)))
            {

                rpta = "El Usuario ya existe , intenta con otro";
            }else */
            if (usuario.usu_usu.Equals(un.usu_usu) && usuario.usu_cla.Equals(un.usu_cla))
            {
                _quimpacContext.Usuarios.Update(usuario);
                _quimpacContext.SaveChanges();
                rpta = "usuario";
            }
            else
            {
                _quimpacContext.Usuarios.Update(usuario);
                _quimpacContext.SaveChanges();
                rpta = "actualizado";
            }
            return rpta;
        }

        public string updateVehiculo(Placa veh)
        {
            
            Placa placa = _quimpacContext.Placa.Where(x => x.pla_cod.Equals(veh.pla_cod)).FirstOrDefault();
            string rpta = "";
            DateTime fechaUpdate = DateTime.Now;


            placa.pla_gru_cue = veh.pla_gru_cue;
            placa.pla_pla_veh = veh.pla_pla_veh;
            placa.pla_nom = veh.pla_nom;
            placa.pla_con_bus = veh.pla_con_bus;
            placa.pla_pai = veh.pla_pai;
            placa.pla_idi = veh.pla_idi;
            placa.pla_fec_mod = fechaUpdate;
            placa.pla_usu_mod_sap = veh.pla_usu_mod_sap;

            if((placa.pla_est_reg.Equals("S") && placa.pla_est.Equals("1")) || (placa.pla_est_reg.Equals("E") && placa.pla_est.Equals("0") ) || (placa.pla_est_reg.Equals("W") && placa.pla_est.Equals("0")))
            {
                if (placa.pla_est_reg.Equals("E"))
                {
                    placa.pla_est_reg = "";
                }
                else
                {
                    placa.pla_est_reg = "M";
                }
                placa.pla_est = "0";
                _quimpacContext.Placa.Update(placa);
                _quimpacContext.SaveChanges();
                rpta = "actualizo";
            }
            else
            {
                rpta = "No actualizo la Placa";
            }

            return rpta;

        }

        public void SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential("qcterminalesclec@gmail.com", "algoritmo21");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("qcterminalesclec@gmail.com", "QCTerminales"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = false,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            //return Task.CompletedTask;
        }
    }
}
