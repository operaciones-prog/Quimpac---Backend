using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Usuarios 
    {

        [Key]
        public int usu_cod_usu { get; set; }
        public string? usu_cod_cli { get; set; }
        public int usu_cod_rol { get; set; }
        public string? usu_cod_soc { get; set; }
        public string? usu_nom_ape { get; set; }
        public string? usu_usu { get; set; }
        public string? usu_cla { get; set; }
        public string? usu_cor { get; set; }
        public string? usu_cod_sap { get; set; }
        public string? usu_est { get; set; }
        public DateTime? usu_fec_cre { get; set; }
        public string? usu_usu_cre_sap { get; set; }
        public DateTime? usu_fec_mod { get; set; }
        public string? usu_usu_mod_sap { get; set; }
        //public Rol rol { get; set; }
    }
}
