namespace OrionCoreCableColor.Models.Usuario
{
    public class BandejaComisionesVendedor
    {
        public int fiIDUsuario { get; set; }
        public string NombreOficial { get; set; }
        public int fiCantidadVentas { get; set; }
        public decimal fnCantidadVentaDolares { get; set; }
        public decimal fnVentasDolares { get; set; }
        public decimal fnVentasLempiras { get; set; }
        public decimal fnTotalVenta { get; set; }
        public decimal fnTotalVentaSinISV { get; set; }
        public string Porcentaje { get; set; }
        public decimal fntotalComision { get; set; }
        public decimal fnBono { get; set; }
        public string DepartamentoVenta { get; set; }
        public decimal fnTotalGlobal { get; set; }
    }

    public class CalculoComisionesNovanetModel
    {
        public int fiIDUsuario { get; set; }
        public string NombreOficial { get; set; }
        public int fiCantidadVentas { get; set; }
        public decimal fnCantidadVentaDolares { get; set; }
        public decimal fnVentasDolares { get; set; }
        public decimal fnVentasLempiras { get; set; }
        public decimal fnTotalVenta { get; set; }
        public decimal fnArpuAlcanzado { get; set; }
        public decimal fnComisionAcumuladaLempiras { get; set; }
        public decimal fnComisionAcumulada { get; set; }
        public decimal fnMetaArpu { get; set; }
        public decimal fntotalComision { get; set; }
        public decimal fnCumpArpu { get; set; }
        public decimal fnBono { get; set; }
        public decimal fnTotalComisionArpu { get; set; }
        public decimal fnTotalGlobal { get; set; }
        public decimal fnMetaComision { get; set; }
        public decimal fnArpuMeta { get; set; }
        public decimal ComisioncumuladaDolaresPasadaL { get; set; }
        public decimal fnTotalGlobalComision { get; set; }
    }


}