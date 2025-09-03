using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Models
{
    public class Constantes
    {
        [Key]
        public int cod_constante { get; set; }
        public string? ide_constante { get; set; }
        public string? nom_constante { get; set; }
        public string? val_constante { get; set; }
    }
}
