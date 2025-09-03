using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Permiso
    {
        [Key]
        public int per_cod { get; set; }
        public string per_nom { get; set; }
        public string per_uri { get; set; }
        public string? per_uri_icon { get; set; }
        public string per_est { get; set; }
        public DateTime? per_fec_cre { get; set; }
        public string? per_usu_cre_sap { get; set; }
        public DateTime? per_fec_mod { get; set; }
        public string? per_usu_mod_sap { get; set; }
    }
}
