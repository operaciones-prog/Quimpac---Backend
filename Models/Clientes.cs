using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Clientes
    {
        [Key]
        public int cli_cod_cli { get; set; }
        public string? cli_raz_soc { get; set; }
        public string? cli_cod_sap { get; set; }
        public string? cli_pai { get; set; }
        public string? cli_gru_cue { get; set; }
        public string? cli_est { get; set; }
        public DateTime? cli_fec_cre { get; set; }
        public string? cli_usu_cre_sap { get; set; }
        public DateTime? cli_fec_mod { get; set; }
        public string? cli_usu_mod_sap { get; set; }
    }
}
