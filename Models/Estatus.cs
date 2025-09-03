using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Estatus
    {
        [Key]
        public string est_cod_est { get; set; }
        public string est_des { get; set; }
        public string est_est { get; set; }
        public DateTime est_fec_cre { get; set; }
        public string est_usu_cre_cod_sap { get; set; }
        public DateTime est_fec_mod { get; set; }
        public string est_usu_mod_cod_sap { get; set; }
    }
}
