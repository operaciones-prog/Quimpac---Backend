using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Sociedad
    {
        [Key]
        public int soc_cod { get; set; }
        public string? soc_cod_soc { get; set; }
        public string? soc_raz_soc { get; set; }
        public string? soc_pai { get; set; }
        public string? soc_gru_cue { get; set; }
        public string? soc_est { get; set; }
        public DateTime? soc_fec_cre { get; set; }
        public string? soc_usu_cre_cod_sap { get; set; }
        public DateTime? soc_fec_mod { get; set; }
        public string? soc_usu_mod_cod_sap { get; set; }
    }
}
