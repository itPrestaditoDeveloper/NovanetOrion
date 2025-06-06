using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.ConciliacionClientes
{
    public class ListConciliacionClientesViewModel
    {
        public string CodigoCliente { get; set; }
        public string NumeroOrdenCepheus { get; set; }
        public string Nombre { get; set; }
        public string Servicio { get; set; }
        public string Ciudad { get; set; }
        public string MesCreado { get; set; }
        public string MontoTotal { get; set; }
    }
}