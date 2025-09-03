using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Actualizaciones
    {
        [Key]
        public int act_cod { get; set; }
        public string act_tab { get; set; }
        public DateTime? act_fec_hor { get; set; }
        public string act_pro_est { get; set; }
    }
}   
