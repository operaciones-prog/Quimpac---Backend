using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Resumen_stock_Cliente
    {
        [Key]
        public int stk_cli_cod { get; set; }
        public string stk_cli_cod_cen { get; set; }
        public string stk_cli_des_cen { get; set; }
        public string stk_cli_cod_cli { get; set; }
        public string stk_cli_des_cli { get; set; }
        public string stk_cli_cod_alm { get; set; }
        public string stk_cli_des_alm { get; set; }
        public string stk_cli_num_mat { get; set; }
        public string stk_cli_des_mat { get; set; }
        public string stk_cli_uni_med { get; set; }
        public string stk_cli_num_lot { get; set; }
        public string stk_cli_fec_mov { get; set; }
        public string stk_cli_ser_pre { get; set; }
        public decimal stk_cli_can { get; set; }
        public string stk_cli_buq { get; set; }
        public string stk_cli_fec_lle { get; set; }
        public string stk_cli_hor_lle { get; set; }
        public string stk_cli_fec_sal { get; set; }
        public string stk_cli_hor_sal { get; set; }
        public string stk_cli_ord { get; set; }
        public DateTime? stk_cli_fec_hor_act_sap { get; set; }
        public string stk_cli_est { get; set; }
        public DateTime? stk_cli_fec_cre { get; set; }
        public string? stk_cli_usu_cre_sap { get; set; }
        public DateTime? stk_cli_fec_mod { get; set; }
        public string? stk_cli_usu_mod_sap { get; set; }
        public string? stk_cli_guia { get; set; }
        public string? stk_cli_cod_cho { get; set; }
        public string? stk_cli_chofer { get; set; }
        public string? stk_cli_cod_soc { get; set; }
        public string? stk_cli_cod_ser_pre { get; set; }
    }
}
