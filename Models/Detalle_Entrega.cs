using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Detalle_Entrega
    {
        [Key]
        public int det_ent_cod { get; set; }
        public string det_ent_num_sol { get; set; }
        public string det_ent_pos { get; set; }
        public string det_ent_cen { get; set; }
        public string det_ent_alm { get; set; }
        public string det_ent_mat { get; set; }
        public string det_ent_des_mat { get; set; }
        public string det_ent_uni { get; set; }
        public string det_ent_lot { get; set; }
        public decimal det_ent_can { get; set; }
        public string det_ent_sta_c { get; set; }
        public string det_ent_sta_m { get; set; }
        public string det_ent_usu_cre { get; set; }
        public string det_ent_usu_mod { get; set; }
        public string det_ent_est { get; set; }
        public DateTime? det_ent_fec_cre { get; set; }
        public string? det_ent_usu_cre_cod_sap { get; set; }
        public DateTime? det_ent_fec_mod { get; set; }
        public string? det_ent_usu_mod_cod_sap { get; set; }
    }
}
