using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Horario
    {
        [Key]
        public string hor_cod_hor { get; set; }
        public string hor_descrip { get; set; }
        public string hor_est { get; set; }
        public DateTime? hor_fec_cre { get; set; }
        public string? hor_usu_cre_cod_sap { get; set; }
        public DateTime? hor_fec_mod { get; set; }
        public string? hor_usu_mod_cod_sap { get; set; }
    }
}
