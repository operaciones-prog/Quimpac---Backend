using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Placa
    {
        [Key]
        public int pla_cod { get; set; }
        public string? pla_gru_cue { get; set; }
        public string? pla_pla_veh { get; set; }
        public string? pla_nom { get; set; }
        public string? pla_con_bus { get; set; }
        public string? pla_pai { get; set; }
        public string? pla_idi { get; set; }
        public string? pla_est_reg { get; set; }
        public string? pla_des_err_sap { get; set; }
        public string? pla_est { get; set; }
        public DateTime? pla_fec_cre { get; set; }
        public DateTime? pla_fec_hor_cre_sap { get; set; }
        public string? pla_usu_cre_sap { get; set; }
        public DateTime? pla_fec_mod { get; set; }
        public DateTime? pla_fec_hor_mod_sap { get; set; }
        public string? pla_usu_mod_sap { get; set; }
    }
}
