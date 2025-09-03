using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Rol
    {
        [Key]
        public int rol_cod { get; set; }
        public string rol_nom { get; set; }
        public string rol_cod_usu_sap { get; set; }
        public string rol_est { get; set; }
        public DateTime? rol_fec_cre { get; set; }
        public string? rol_usu_cre_sap { get; set; }
        public DateTime? rol_fec_mod { get; set; }
        public string? rol_usu_mod_sap { get; set; }
    }
}
