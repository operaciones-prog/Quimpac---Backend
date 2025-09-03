using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Stock
    {
        [Key]
        public int stk_cod { get; set; }
        public string stk_cod_cli { get; set; }
        public string stk_des_cli { get; set; }
        public string stk_cod_cen { get; set; }
        public string stk_des_cen { get; set; }
        public string stk_cod_alm { get; set; }
        public string stk_des_alm { get; set; }
        public string stk_num_mat { get; set; }
        public string stk_des_mat { get; set; }
        public string stk_uni_med { get; set; }
        public string stk_num_lot { get; set; }
        public decimal stk_sto_dis { get; set; }
        public decimal stk_ent_sap { get; set; }
        public decimal stk_sto_nna { get; set; }
        public decimal stk_sto_tra { get; set; }
        public decimal stk_sto_blo { get; set; }
        public DateTime? stk_fec_hor_act { get; set; }
        public string stk_est { get; set; }
        public DateTime? stk_fec_cre { get; set; }
        public string? stk_usu_cre_sap { get; set; }
        public DateTime? stk_fec_mod { get; set; }
        public string? stk_usu_mod_sap { get; set; }
        public string? stk_num_mat_cli { get; set; }
        public decimal? stk_peso_equi { get; set; }
        public string? stk_cod_soc { get; set; }
    }
}
