﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class NumerosALetras
    {

        public static string ConvertirCantidadALetras(string cantidad)
        {
            string resultado, centavos = string.Empty;
            Int64 entero;
            int decimales;
            double cantidadNumerica;

            try
            {
                cantidadNumerica = Convert.ToDouble(cantidad);
                entero = Convert.ToInt64(Math.Truncate(cantidadNumerica));
                decimales = Convert.ToInt32(Math.Round((cantidadNumerica - entero) * 100, 2));
                centavos = " CON " + PadNum(decimales) + "/100 CTVS";
                resultado = ToText(Convert.ToDouble(entero)) + centavos;
            }
            catch
            {
                return "";
            }

            return resultado;
        }


        public static string ConvertirDolaresAletras(string cantidad)
        {
            string resultado, centavos = string.Empty;
            Int64 entero;
            int decimales;
            double cantidadNumerica;

            try
            {
                cantidadNumerica = Convert.ToDouble(cantidad);
                entero = Convert.ToInt64(Math.Truncate(cantidadNumerica));
                decimales = Convert.ToInt32(Math.Round((cantidadNumerica - entero) * 100, 2));
                centavos = " DOLARES CON " + PadNum(decimales) + "/100 CTVS";
                resultado = ToText(Convert.ToDouble(entero)) + centavos;
            }
            catch
            {
                return "";
            }

            return resultado;
        }


        public static string ConvertirLempirasAletras(string cantidad)
        {
            string resultado, centavos = string.Empty;
            Int64 entero;
            int decimales;
            double cantidadNumerica;

            try
            {
                cantidadNumerica = Convert.ToDouble(cantidad);
                entero = Convert.ToInt64(Math.Truncate(cantidadNumerica));
                decimales = Convert.ToInt32(Math.Round((cantidadNumerica - entero) * 100, 2));
                centavos = " LEMPIRAS CON " + PadNum(decimales) + "/100 CTVS";
                resultado = ToText(Convert.ToDouble(entero)) + centavos;
            }
            catch
            {
                return "";
            }

            return resultado;
        }

        private static string PadNum(int entero)
        {
            return entero < 9 ? "0" + entero : entero.ToString();
        }

        public static string ToText(double value)
        {
            var Num2Text = string.Empty;
            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + ToText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + ToText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = ToText(Math.Truncate(value / 10) * 10) + " Y " + ToText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + ToText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = ToText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = ToText(Math.Truncate(value / 100) * 100) + " " + ToText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + ToText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + ToText(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + ToText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000) * 1000000);
            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {
                Num2Text = ToText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            return Num2Text;

        }
    }
}