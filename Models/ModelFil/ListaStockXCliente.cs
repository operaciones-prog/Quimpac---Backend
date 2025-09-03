using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models.ModelFil
{
    public class ListaStockXCliente
    {
        public int cod { get; set; }
        public string cod_cli { get; set; }
        public string des_cli { get; set; }
        public string cod_soc { get; set; }
        public string cod_cen { get; set; }
        public string des_cen { get; set; }
        public string cod_alm { get; set; }
        public string des_alm { get; set; }
        public string num_mat { get; set; }
        public string des_mat { get; set; }
        public string uni_med { get; set; }
        public string num_lot { get; set; }
        public decimal sto_dis { get; set; }
        public decimal ent_sap { get; set; }
        public decimal sto_nna { get; set; }
        public decimal sto_tra { get; set; }
        public decimal sto_blo { get; set; }
        public DateTime? fec_hor_act { get; set; }
        public string est { get; set; }
        public DateTime? fec_cre { get; set; }
        public string? usu_cre_sap { get; set; }
        public DateTime? fec_mod { get; set; }
        public string? usu_mod_sap { get; set; }
        public string? tanque { get; set; }
        public string? num_mat_cli { get; set; }
        public decimal? peso_equi { get; set; }
        public decimal? stock_total { get; set; }
        public decimal? ent_sap_QC_Online { get; set; }
        public decimal? solicitudes_QC_Online { get; set; }
        public decimal? stock_disponible_QC_Online { get; set; }
        
    }
}
