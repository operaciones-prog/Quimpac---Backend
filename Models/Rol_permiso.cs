using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Rol_permiso
    {
        [Key]
        public int rol_per_cod { get; set; }
        public int rol_per_cod_rol { get; set; }
        public int rol_per_cod_per { get; set; }
        public string rol_per_est { get; set; }
        public DateTime? rol_per_fec_cre { get; set; }
        public string? rol_per_usu_cre_sap { get; set; }
        public DateTime? rol_per_fec_mod { get; set; }
        public string? rol_per_usu_mod_sap { get; set; }
    }
}
