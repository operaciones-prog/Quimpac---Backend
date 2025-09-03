using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Entrega
    {
        [Key]
        public int ent_cod { get; set; }
        public string? ent_num_Sol { get; set; }
        public string? ent_org_vent { get; set; }
        public string? ent_canal { get; set; }
        public string? ent_sector { get; set; }
        public string? ent_clase_entr { get; set; }
        public string? ent_puesto_exp { get; set; }
        public string? ent_ruta { get; set; }
        public string? ent_puesto { get; set; }
        public string? ent_clase_tran { get; set; }
        public string? ent_clase_exp { get; set; }
        public string? ent_cond_exp { get; set; }
        public string? ent_ref { get; set; }
        public string? ent_int_ope { get; set; }
        public string? ent_fec_cre_doc { get; set; }
        public string? ent_cli { get; set; }
        public string? ent_des_cli { get; set; }
        public string? ent_cod_cho { get; set; }
        public string? ent_pla_veh { get; set; }
        public string? ent_cod_hor { get; set; }
        public string? ent_fec_hor { get; set; }
        public string? ent_cod_sta { get; set; }
        public string? ent_fec_apr { get; set; }
        public string? ent_usu_apr { get; set; }
        public string? ent_ent_sap { get; set; }
        public string? ent_log_err { get; set; }
        public string? ent_des_err { get; set; }
        public string? ent_usu_cre { get; set; }
        public string? ent_sta_m { get; set; }
        public DateTime? ent_fec_hor_reg_sap { get; set; }
        public string? ent_est { get; set; }
        public DateTime? ent_fec_cre { get; set; }
        public string? ent_usu_cre_cod_sap { get; set; }
        public DateTime? ent_fec_mod { get; set; }
        public string? ent_usu_mod_cod_sap { get; set; }
    }
}
