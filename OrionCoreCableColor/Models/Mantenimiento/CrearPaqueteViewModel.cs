using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearPaqueteViewModel
    {
        public int fiIdPaquete { get; set; }
        [DisplayName("Paquete")]
        [Required]
        public string fcPaquete { get; set; }
        [DisplayName("Valor Mensual")]
        [Required]
        public decimal fnValorMensual { get; set; }
        [DisplayName("Tasa de interes anual")]
        [Required]
        public decimal fnTasaDeInteresAnual { get; set; }
        public List<ListPaqueteDetalleViewModel> Productos { get; set; }
        public bool fbEditar { get; set; }
        public bool fbClonar { get; set; }
        public int fiestadoPaquete { get; set; }
        [DisplayName("Moneda")]
        public int fiIDMoneda { get; set; }
        public bool fbServicio { get; set; }
        public bool fbSoloServicio { get; set; }
    }

    public class Paquetes_Maestro_Ubicacion
    {
        public int fiIDPaquetePorUbicacion { get; set; }
        public int fiIDPaquete { get; set; }
        public int fiCodPais { get; set; }
        public int fiCodDepartamento { get; set; }
        public int fiCodMunicipio { get; set; }

    }

    public class Geo_Municipio
    {
        public int fiCodPais { get; set; }
        public string fcPais { get; set; }
        public int fiCodDepartamento { get; set; }
        public string fcDepartamento { get; set; }
        public int fiCodMunicipio { get; set; }
        public string fcMunicipio { get; set; }
        public bool fbSeleccion { get; set; }
    }
}