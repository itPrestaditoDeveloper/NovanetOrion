using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class CorrelativeHelper
    {
        public static string CrearCorrelativoPartida(string prefijo, int valorCorrelativo)
        {
            return prefijo.Trim() + "-" + string.Format("{0,6:000000}", valorCorrelativo);
        }

        public static string CrearCorrelativoDocumento(string prefijo, int valorCorrelativo)
        {
            return prefijo.Trim() + "-" + string.Format("{0,8:00000000}", valorCorrelativo);
        }
    }
}