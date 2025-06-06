using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class referenciaclienteViewModel
    {
        public int fiIDCliente  { get; set; }
        public string nombrecliente { get; set; }
        public int fcIDReferencia { get; set; }
        public string fcPregunta1 { get; set; }
        public string fcPregunta2 { get; set; }
        public string fcPregunta3 { get; set; }
        public string fcRespuesta1 { get; set; }
        public string fcRespuesta2 { get; set; }
        public string fcRespuesta3 { get; set; }
        public string fcRespuesta1Correcta { get; set; }
        public string fcRespuesta2Correcta { get; set; }
        public string fcRespuesta3Correcta { get; set; }
        public bool AceptoSerReferencia { get; set; }
    }
    public class List_coloniasFalsas
    {
      
        public string descripcion { get; set; }
    }
    public static class Preguntas
    {
        public static string preguntaVive = "Vive en la colonia/barrio:";
        public static string preguntatrabajo = "Trabaja en:";
        public static string preguntaParentesco = "Es mi:";
        public static string preguntaTieneHijos = "Tiene hijos?:";
        public static string preguntaEstadoCivil = "Su estado civil es:";
        public static string PreguntaCiudad = "Vive en la ciudad de:";

    }
}