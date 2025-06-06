namespace OrionCoreCableColor.Models.Reportes
{
    public class ListSolicitudesPorAsesorViewModel
    {

        public int IDOficial { get; set; }
        public string NombreOficial { get; set; }
        public string DepartamentoVenta { get; set; }
        public double TotalIngresadas { get; set; }
        public int TotalInstaladas { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalEnProceso { get; set; }
        public double InstaladasVsPendientes { get; set; }
        public double EnProcesoVsPendientes { get; set; }
        public int TotalIngresadasSoloServicio { get; set; }
        public int TotalIngresadasConArticulo { get; set; }
        public double TotalIngresadasSoloServicioVsTotal { get; set; }
        public double TotalIngresadasConArticuloVsTotal { get; set; }
        public double ArpuPromedioTotalIngresadas { get; set; }


    }
}