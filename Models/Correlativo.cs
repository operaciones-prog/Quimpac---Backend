using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Correlativo
    {
        [Key]
        public int cor_cod { get; set; }
        public string cor_nom { get; set; }
        public string cor_ran_1 { get; set; }
        public string cor_ran_2 { get; set; }
        public string cor_ind { get; set; }
    }
}
