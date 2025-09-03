using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models.ModelFil
{
    public class StockTotalFil
    {
        public string? stf_cod_cli { get; set; }
        public string? stf_des_cli { get; set; }
        public string? stf_cod_alm { get; set; }
        public string? stf_des_alm { get; set; }
        public string? stf_num_mat { get; set; }
        public string? stf_des_mat { get; set; }
        public string? stf_uni_med { get; set; }
        public string? stf_num_lot { get; set; }
        public decimal? stf_sto_dis { get; set; }
        public decimal? stf_ent_sap { get; set; }
        public decimal? stf_sto_nna { get; set; }
        public decimal? stf_sto_tra { get; set; }
        public decimal? stf_sto_blo { get; set; }
        public DateTime? stf_fec_hor_act { get; set; }
        public decimal? stf_stk_total { get; set; }
        public int? stf_sol_qco { get; set; }
        public decimal? stf_stk_dis_qco { get; set; }
    }
}
