using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models.ModelFil
{
    public class StockClienteFil
    {
        public string org_ven { get; set; }
        public int cantidad { get; set; }
        public string cod_cli { get; set; }
        public string des_alm { get; set; }
        public string des_mat { get; set; }
        public string num_lot { get; set; }
        public string ser_pre { get; set; }
        public string cli_buq { get; set; }
        public string fec_mov_des { get; set; }
        public string fec_mov_has { get; set; }
        public string fec_lle_des { get; set; }
        public string fec_lle_has { get; set; }
        public string fec_sal_des { get; set; }
        public string fec_sal_has {get;set;}
        public string stk_cli_cod_cho { get; set; }
        public string stk_cli_guia { get; set; }
        public string cod_soc { get; set; }
    }
}
