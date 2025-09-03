using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Ordenes
    {
        [Key]
        public int ord_cod { get; set; }
        public string? ord_cod_pro { get; set; }
        public string? ord_des_pro { get; set; }
        public string? ord_cod_cen { get; set; }
        public string? ord_des_cen { get; set; }
        public string? ord_com { get; set; }
        public string? ord_num_mat { get; set; }
        public string? ord_des_mat { get; set; }
        public string? ord_val_2 { get; set; }
        public string? ord_fec_fin_pla { get; set; }
        public string? ord_fec_reg { get; set; }
        public string? ord_hor_reg { get; set; }
        public decimal? ord_can_pro { get; set; }
        public string? ord_uni_med { get; set; }
        public string? ord_des { get; set; }
        public string? ord_ori { get; set; }
        public string? ord_est_doc { get; set; }
        public DateTime? ord_fec_hor_act_sap { get; set; }
        public string? ord_est { get; set; }
        public DateTime? ord_fec_cre { get; set; }
        public string? ord_usu_cre_sap { get; set; }
        public DateTime? ord_fec_mod { get; set; }
        public string? ord_usu_mod_sap { get; set; }
        public string? ord_guia { get; set; }
        public string? ord_cod_cho { get; set; }
        public string? ord_chofer { get; set; }
        public string? ord_cod_soc { get; set; }
        public string? ord_cod_ser_pre { get; set; }
    }
}
