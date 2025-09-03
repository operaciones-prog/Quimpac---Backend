using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Chofer
    {
        [Key]
        public int cho_cod { get; set; }
        public string? cho_gru_cue { get; set; }
        public string? cho_cod_cho { get; set; }
        public string? cho_nom { get; set; }
        public string? cho_con_bus { get; set; }
        public string? cho_pai { get; set; }
        public string? cho_idi { get; set; }
        public string? cho_num_ide_fis { get; set; }
        public string? cho_num_tip_nif { get; set; }
        public string? cho_num_per_fis { get; set; }
        public string? cho_cia_imp { get; set; }
        public string? cho_est_reg { get; set; }
        public string? cho_des_err_sap { get; set; }
        public string? cho_est { get; set; }
        public DateTime? cho_fec_cre { get; set; }
        public DateTime? cho_fec_hor_cre_sap { get; set; }
        public string? cho_usu_cre_sap { get; set; }
        public DateTime? cho_fec_mod { get; set; }
        public DateTime? cho_fec_hor_mod_sap { get; set; }
        public string? cho_usu_mod_sap { get; set; }
    }
}
