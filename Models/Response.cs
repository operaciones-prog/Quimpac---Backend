using System;
using System.Collections.Generic;

namespace PROYEC_QUIMPAC.Models
{
    public class Response
    {
        public bool error { get; set; }
        public string message { get; set; }
        public string trace { get; set; }
        public List<Object> data { get; set; }
    }
}
