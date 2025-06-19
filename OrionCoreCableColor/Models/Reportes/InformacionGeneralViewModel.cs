using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Reportes
{
    public class InformacionGeneralViewModel
    {
        public decimal fnValorARPUGeneral { get; set; }
        public decimal fnValorARPUDelMes { get; set; }
        public int fiClientesAprobados { get; set; }
        public int fiClientesActivos { get; set; }
        public int fiClientesInstalados { get; set; }
        public int fiClientesEnMora { get; set; }
        public int fiClientesCortados { get; set; }
        public int fiClientesCancelados { get; set; }
        public int fiClientesReconectados { get; set; }
        public int fiClientesEnAtraso { get; set; }
        public int fiClientesNuevosMes { get; set; }
        public int fiClientesCambioServicioMes { get; set; }
        public int fiClientesPrecalificadosDelMes { get; set; }
        public decimal fnIngresosdelMes { get; set; }
        public decimal fnIngresosdelGlobales { get; set; }
        public int fiProyeccionVentas { get; set; }
        public int fiProyeccionVentasAcumulada { get; set; }
        public int fiClientesNetos { get; set; }
        public decimal fiChurnRate1Mes { get; set; }
        public int fiCancelacion1Mes { get; set; }
        public List<VentasMesViewModel> DataVentas { get; set; }
        public List<MoraMesViewModel> DataMora { get; set; }
        public List<VentasClientesActivosViewModel> DataClientesActivos { get; set; }
        public List<ArpusViewModel> DataArpusVentas { get; set; }
        public List<DatosIngresosViewModel> DataIngresos { get; set; }

    }
    public class VentasMesViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public int fiConteoVentas { get; set; }
        public decimal fnMontoVentas { get; set; }

    }
    public class MoraMesViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public int fiClientesEnMora { get; set; }
        public int fiTotalClientes { get; set; }
        public decimal fnPorcentajedeMora { get; set; }

    }
    public class VentasClientesActivosViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public int fiConteoVentas { get; set; }
        public decimal fnMontoVentas { get; set; }
        public int fiClientesCancelados { get; set; }
    }
    public class ArpusViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public decimal fnArpuMensual { get; set; }
        public decimal fnCuotaMaxima { get; set; }
        public decimal fnCuotaMinima { get; set; }

    }
    public class DatosIngresosViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public decimal fnValorAbono { get; set; }
        public decimal fnValorAbonoAcumulado { get; set; }

    }
}