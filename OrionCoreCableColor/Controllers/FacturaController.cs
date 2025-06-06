using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.Models.Factura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class FacturaController : BaseController
    {
        // GET: Facturas
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Factura()
        {
            return View();
        }

        public ActionResult RptFactura(int fiIDTransaccion)
        {
            using(var contexto = new CoreFinancieroEntities2())
            {
                var result = contexto.sp_Facturacion_ConsultarFactura(fiIDTransaccion, null).FirstOrDefault();
                var model = new RptFacturaCuentaPorCobrarViewModel
                {
                    Empresa = "Prestadito S.A. DE C.V.",
                    Direccion = result.fcDireccionAgencia ?? "", //cliente.fcMunicipio ?? "",
                    Ciudad = "San Pedro Sula",
                    Telefono = "(+504) 2504-6682",
                    Correo = "contacto@novanetgroup.com",
                    Domicilio = "",//empresa?.fcCiudadDomiciliada ?? "", //cliente.fcDireccion1.
                    RTNEmpresa = "05019016811399",
                    Factura = result.fcFactura,
                    CAI = result.fcCAI,
                    Cliente = result.fcNombreCliente,
                    RTNCliente = result.fcRTN,
                    Logo = @"/Content/img/NOVANETLOGO.png",
                    FechaLimiteEmision = result.fdFechaRangoFinalFacturacion,
                    //FechaLimiteEmision = empresaModel.fcFechaLimiteEmisionFactura ?? DateTime.Now,
                    RangoAutorizacion = result.fcRangoInicialFacturacion + " al " + result.fcRangoFinalFacturacion,
                    //RangoAutorizacion = empresaModel.fcPrefijoFactura + empresaModel.fnRangoInicialFactura + " al " + empresaModel.fcPrefijoFactura + empresaModel.fnRangoFinalFactura,

                    Fecha = result.fdFechaFactura,
                    NoDeclaracion = "",
                    fcConcepto = "",
                    SubTotal = result.fnSubtotal,
                    Descuento = 0,
                    Exento = result.fnImporteExento,
                    Exonerado = result.fnImporteExonerado,
                    Gravado = result.fnImporteGravado,
                    ISV = result.fnImpuestos,
                    Total = result.fnTotal,
                    TotalLetras = result.fcValorenLetras,
                    fcProductoDescripcion = result.fcProductoDescripcion,
                    fcSimboloMoneda = result.fcSimboloMoneda,
                    fnTasaDeCambio = result.fnTasadeCambio,
                    fnTotalLempiras = result.fnValorLempiras ?? 0
                };



                return PartialView(model);
            }
            
        }

    }
}