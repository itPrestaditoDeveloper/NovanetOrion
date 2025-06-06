using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.App_Services.ReportesService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Prestamo;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class PrestamoController : BaseController
    {
        // GET: Prestamo
        public DbServiceConnection _connection;
        public ActionResult Bandeja_Credito()
        {
            return View();
        }
        public ActionResult Bandeja_CreditoCancelado()
        {
            return View();
        }
        public ActionResult RegistrarPago()
        {
            return View(); //RedirectToAction(":https://orion.novanetgroup.com/Operaciones/OperacionesNovaNetAbonosaCuotas.aspx?uebAtRAuEVAemd5EkR0K/1DFw0E2LsJdNjGknjYR77JVU8QTRz0wWsaCelOsQM1N");
        }
        public ActionResult MenuDetallePrestamo(string idSolicitud, string idPrestamo, string idIDentidad, string idClienteSAF)
        {

            return View(new InformacionClientePrincipalViewModel { idSolicitud = idSolicitud, idPrestamo = idPrestamo, fcIdentidad = idIDentidad, fcClienteSAF = idClienteSAF });
        }


        #region PrestamosCliente
        public ActionResult DetallesCliente(string fcIdentidad, string fcClienteSAF)
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = conetion.sp_Prestamos_Lista_Novanet(1, 1, GetIdUser()).Where(x => x.fcIdentidad == fcIdentidad).ToList();

                ViewBag.ListadoPrestamosCliente = lista;

                return View(new InformacionClientePrincipalViewModel { fcIdentidad = fcIdentidad, fcClienteSAF = fcClienteSAF });
            }
        }

        public ActionResult _DetalleCredito(string idCustomer, string idLoan)
        {
            try
            {
                var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
                using (var conetion = new ORIONDBEntities())
                {
                    var modelClientesCredito = conetion.sp_Prestamo_InformacionTotal_Novanet(1, GetIdUser(), idCustomer).FirstOrDefault();
                    if (modelClientesCredito == null) return PartialView(new InformacionCreditoClienteViewModel { fcIDCliente = idCustomer });
                    var clienteDetalle = new InformacionCreditoClienteViewModel
                    {
                        fcPrestamo = modelClientesCredito.fcIDPrestamo,
                        fcIDCliente = modelClientesCredito.fcClienteSAF,
                        fiDiasAtraso = modelClientesCredito.fiDiasAtraso,
                        fiCuotasAtraso = modelClientesCredito.fiCuotasAtrasadas,
                        fiCuotasPagadas = modelClientesCredito.fiCuotasPagadas,
                        fiTotalCuotas = modelClientesCredito.fiTotalCuotas,
                        fnValorCuota = modelClientesCredito?.fnValorCuota ?? 0,
                        fnCapitalOtorgado = modelClientesCredito.fnCapitalFinanciado,
                        fnSaldoActual = modelClientesCredito.fnSaldoActualCapital,
                        fnCapitalVencido = modelClientesCredito.fnCapitalVencido,
                        fnInteresesVencidos = modelClientesCredito.fnInteresesVencidos,
                        fnRecargos = modelClientesCredito.fnRecargos ?? 0,
                        fnSaldoPonerAlDia = modelClientesCredito.fnSaldoPonerAlDia,
                        fnTotalCuenta = modelClientesCredito.fnTotalCuenta,
                        fdFechaInicioPrestamo = modelClientesCredito.fdFechaInicioPrestamo,
                        fdFechaPrimerPagoPrestamo = modelClientesCredito.fdFechaPrimerPagoPrestamo,
                        fdFechaVencePrestamo = modelClientesCredito.fdFechaVencePrestamo,
                        fdUltimoAbono = modelClientesCredito.fdUltimoAbono,
                        fnValorUltimoAbono = modelClientesCredito.fnValorUltimoAbono,
                        fcProducto = modelClientesCredito.fcProducto,
                        fcTipoDePlazo = modelClientesCredito.fcTipoDePlazo,
                        SolicitudMaestroNovanet = modelClientesCredito.SolicitudMaestroNovanet



                    };


                    var listImagenesInstalacion = new List<ListImagenesInstalacionViewModel>();
                    var ListaImagenesInstalacion = conetion.sp_Documentos_Instalacion_List(1, modelClientesCredito.SolicitudMaestroNovanet).ToList();
                    foreach (var foto in ListaImagenesInstalacion)
                    {
                        listImagenesInstalacion.Add(new ListImagenesInstalacionViewModel
                        {
                            fcDescription = foto.fcDescripcion,
                            fcUrlFile = foto.fcUrlDocumento,
                            fcFileName = foto.fcNombreDocumento
                        });
                    }

                    clienteDetalle.ListaImagenesInstalacion = listImagenesInstalacion;

                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {clienteDetalle.SolicitudMaestroNovanet}";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            //listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                            reader.NextResult();
                            //listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();
                            reader.NextResult();
                            listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                        }

                        connection.Close();
                    }
                    clienteDetalle.ListaGarantia = listaEquifaxGarantia;
                    return PartialView(clienteDetalle);

                }
            }
            catch (Exception e)
            {
                return PartialView(new InformacionCreditoClienteViewModel());
            }


        }

        public ActionResult _InformacionPersonalCliente(string id, string idPrestamo, string idIDentidad, string fcClienteSAF)
        {
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            using (var connectionNextResult = (new ORIONDBEntities()).Database.Connection)
            {

                connectionNextResult.Open();
                var command = connectionNextResult.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {id}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                }


                using (var conetion = new ORIONDBEntities())
                {
                    var Identidad = idIDentidad;
                    // var InformacionPrestamo = conetion.sp_SRCClienteDetalle_Novanet(idIDentidad, null).FirstOrDefault();
                    var InformacionPrestamo = conetion.sp_SRCClienteDetalle_Novanet(idIDentidad, idPrestamo).FirstOrDefault();

                    var model = new InformacionClientePrincipalViewModel
                    {
                        fcIdentidad = InformacionPrestamo.fcIdentidad,
                        fcNombreSAF = InformacionPrestamo.fcNombreSAF,
                        fcTelefonos = InformacionPrestamo.fcTelefonos,
                        fcMunicipio = InformacionPrestamo.fcMunicipio,
                        fcDireccionDetallada = InformacionPrestamo.fcDireccionDetallada,
                        fcBarrio = InformacionPrestamo.fcBarrio,
                        fcDescripcionAntiguedad = InformacionPrestamo.fcDescripcionAntiguedad,
                        fcDescripcion_RangosSalariales = InformacionPrestamo.fcDescripcion_RangosSalariales,
                        fcLugarTrabajo = InformacionPrestamo.fcLugarTrabajo,
                        fcIDPrestamo = InformacionPrestamo.fcIDPrestamo,
                        fcClienteSAF = fcClienteSAF,
                        idSolicitud = id,
                        fcCorreo = listaDetalleCliente.fcCorreo,


                    };
                    connectionNextResult.Close();
                    return PartialView(model);
                }

            }

        }


        public ActionResult _MostrarDetalleOrdenTrabajo(int IDSolicitud, string Identidad)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                int IDUser = GetIdUser();
                connection.Open();

                var commandEquifax = connection.CreateCommand();
                commandEquifax.CommandText = $"SELECT TOP 1 fiIDEquifax FROM Equifax_Cliente WHERE fcIdentidad = '{Identidad}'";

                int fiIDEquifax = 0;

                using (var reader = commandEquifax.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fiIDEquifax = reader.GetInt32(0);
                    }
                }

                var list = new List<DetalleInstalacionesCliente>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_SolicitudesDeTrabajo_PorIDCliente {fiIDEquifax}, {IDUser}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    list = db.ObjectContext.Translate<DetalleInstalacionesCliente>(reader).ToList();
                }

                connection.Close();

                var filteredList = list.Where(x => x.fiIDSolicitud == IDSolicitud).ToList();
                return PartialView(filteredList);
            }
        }


        public ActionResult _Informacionpago(string idCustomer, string idLoan, string idClienteSAF)
        {
            try
            {
                using (var conetionCore2 = new CoreFinancieroEntities2())
                {
                    using (var conetion = new ORIONDBEntities())
                    {
                        conetionCore2.Database.CommandTimeout = 0;
                        var modelClientesCredito = conetion.sp_Prestamo_InformacionTotal_Novanet(1, GetIdUser(), idLoan).FirstOrDefault();
                        if (modelClientesCredito == null) return PartialView(new InformacionCreditoClienteViewModel { fcIDCliente = idCustomer });

                        var EstadoDeResultado = conetionCore2.Database.SqlQuery<CustomerStatementDetailViewModel>($"Corefinanciero.dbo.sp_SRCEstadoDeCuenta_PorPrestamo '{idLoan}' ").ToList();
                        var totalAmountReceived = (EstadoDeResultado.Any(z => z.fnTotal < 0) ? (EstadoDeResultado.Where(z => z.fnTotal < 0).Sum(z => z.fnTotal)) : 0m) * (-1);
                        var totalPrincipal = (EstadoDeResultado.Any(z => z.fnCapital < 0) ? (EstadoDeResultado.Where(z => z.fnCapital < 0).Sum(z => z.fnCapital)) : 0m) * (-1);
                        var totalInterest = (EstadoDeResultado.Any(z => z.fnInteresOrdinario < 0) ? (EstadoDeResultado.Where(z => z.fnInteresOrdinario < 0).Sum(z => z.fnInteresOrdinario)) : 0m) * (-1);
                        var totalLateFees = (EstadoDeResultado.Any(z => z.fnInteresMoratorio < 0) ? (EstadoDeResultado.Where(z => z.fnInteresMoratorio < 0).Sum(z => z.fnInteresMoratorio)) : 0m) * (-1);
                        var totalOthers = (EstadoDeResultado.Any(z => z.fnOtrosCargos < 0) ? (EstadoDeResultado.Where(z => z.fnOtrosCargos < 0).Sum(z => z.fnOtrosCargos)) : 0m) * (-1);
                        var totalInsurance = (EstadoDeResultado.Any(z => z.fnSegurosDeuda < 0) ? (EstadoDeResultado.Where(z => z.fnSegurosDeuda < 0).Sum(z => z.fnSegurosDeuda)) : 0m) * (-1);

                        var APR = ExcelFunctionsHelper.APR(modelClientesCredito.fiTotalCuotas, modelClientesCredito.fnValorCuotaPrestamo, modelClientesCredito.fnCapitalFinanciado);

                        // var comandLoanFinancialInfo = $"{StoreProcedure_Command.SP_LoanFinancialInfo_Query}, '{idLoan}'";
                        // var listCuentaLoan = _connection.LoadListDataWithDbContext<ListLoanCuentasContablesViewModel>(comandLoanFinancialInfo);
                        //var PaidToDealer = listCuentaLoan.FirstOrDefault(z => z.CuentaContable.Trim() == LoanCuentasContablesEnum.CuentaEnlaceEntreMódulosSAF5)?.Credito ?? 0;

                        var clienteDetalle = new InformacionCreditoClienteViewModel
                        {
                            fcNombre = modelClientesCredito.fcNombre,
                            fcPrestamo = modelClientesCredito.fcIDPrestamo,
                            fcIDCliente = modelClientesCredito.fcClienteSAF,
                            fiDiasAtraso = modelClientesCredito.fiDiasAtraso,
                            fiCuotasAtraso = modelClientesCredito.fiCuotasAtrasadas,
                            fiCuotasPagadas = modelClientesCredito.fiCuotasPagadas,
                            fiTotalCuotas = modelClientesCredito.fiTotalCuotas,
                            fnValorCuota = modelClientesCredito?.fnValorCuota ?? 0,
                            fnCapitalOtorgado = modelClientesCredito.fnCapitalFinanciado,
                            fnSaldoActual = modelClientesCredito.fnSaldoActualCapital,
                            fnCapitalVencido = modelClientesCredito.fnCapitalVencido,
                            fnInteresesVencidos = modelClientesCredito.fnInteresesVencidos,
                            fnRecargos = modelClientesCredito.fnRecargos ?? 0,
                            fnSaldoPonerAlDia = modelClientesCredito.fnSaldoPonerAlDia,
                            fnTotalCuenta = modelClientesCredito.fnTotalCuenta,
                            fdFechaInicioPrestamo = modelClientesCredito.fdFechaInicioPrestamo,
                            fdFechaPrimerPagoPrestamo = modelClientesCredito.fdFechaPrimerPagoPrestamo,
                            fdFechaVencePrestamo = modelClientesCredito.fdFechaVencePrestamo,
                            fdUltimoAbono = modelClientesCredito.fdUltimoAbono,
                            fnValorUltimoAbono = modelClientesCredito.fnValorUltimoAbono,
                            fcClienteSAF = idClienteSAF,

                            TotalAmountReceived = totalAmountReceived,
                            Amount_Principal = totalPrincipal,
                            Amount_Interest = totalInterest,
                            Amount_LateFees = totalLateFees,

                            Frecuency = modelClientesCredito.fcTipoDePlazo,
                            APR = APR,
                            PaymentsQuantity = modelClientesCredito.fiTotalCuotas,
                            //    DownPayment = modelClientesCredito.fnValorPrima ?? 0,
                            //    ValorMercadoGarantia = modelClientesCredito.fnValorGarantia ?? 0,
                            //    fnValorAPrestar = modelClientesCredito.fnValorAPrestar ?? 0,
                            //    fnValorTotalContrato = modelClientesCredito.fnValorTotalContrato ?? 0,
                            //    fnValorTotalFinanciamiento = modelClientesCredito.fnValorTotalFinanciamiento ?? 0,
                            //    fnValorTotalSeguro = modelClientesCredito.fnValorTotalSeguro ?? 0,
                            //    fnCostoGPS = modelClientesCredito.fnCostoGPS ?? 0,
                            //    fnGastosDeCierre = modelClientesCredito.fnGastosDeCierre ?? 0
                        };
                        return PartialView(clienteDetalle);
                    }
                }


            }
            catch (Exception e)
            {
                return PartialView(new InformacionCreditoClienteViewModel());
            }
        }

        public ActionResult _EstadoDeCuenta(string idCustomer, string idLoan, string idClienteSAF)
        {
            using (var conetion = new CoreFinancieroEntities2())
            {
                conetion.Database.CommandTimeout = 0;
                var EstadoDeResultado = conetion.Database.SqlQuery<CustomerStatementDetailViewModel>($"Corefinanciero.dbo.sp_SRCEstadoDeCuenta '{idClienteSAF}', '{idLoan}' ").ToList();
                var saldo = 0m;
                var count = 1;
                EstadoDeResultado.ForEach(z =>
                {
                    z.SaldoDelPeriodo = z.fnTotal + saldo;
                    saldo += z.fnTotal;
                    z.ItemNumber = count;
                    count++;
                });
                return PartialView(EstadoDeResultado);

            }
        }

        public ActionResult _PlandePago(string idCustomer, string idLoan, string idClienteSAF)
        {


            using (var conetion = new CoreFinancieroEntities2())
            {
                conetion.Database.CommandTimeout = 0;

                var EstadoDeResultado = conetion.Database.SqlQuery<sp_Prestamo_PlandePago_ConsultarAvancePorPrestamo_ViewModel>($"Corefinanciero.dbo.sp_SRCClientesPlandePago '{idClienteSAF}', '{idLoan}' ").ToList();
                var count = 1;
                foreach (var item in EstadoDeResultado)
                {
                    item.ItemNumber = count;
                    count++;

                }

                return PartialView(EstadoDeResultado);

            }


        }
        #endregion





        #region Detalle Prestamo
        [HttpGet]
        public JsonResult ListaPrestamo()
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Prestamos_Lista_Novanet(1, 1, GetIdUser()).ToList());
                return lista;
            }

        }
        [HttpGet]
        public JsonResult ListaPrestamoCancelado()
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Prestamos_Lista_Novanet_Cancelado(1, 1, GetIdUser()).ToList());
                return lista;
            }

        }
        [HttpGet]
        public ActionResult ModalCrearSolicitudCorteNovanet(string Nombre, int IDSolicitud, int fiIDEstadoInstalacion, int fiIDEquifax)
        {

            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, fcCodigoVendedor = "", fiIDEstadoInstalacion = fiIDEstadoInstalacion, IdCliente = fiIDEquifax });
            }

        }

        public ActionResult FichaPrestamoDetalle(string idCustomer, string idLoan)
        {
            return PartialView();
        }
        public ActionResult AbrirInformacionPrestamoCliente(string id, string idPrestamo, string idIDentidad, string fcClienteSAF)
        {
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            using (var connectionNextResult = (new ORIONDBEntities()).Database.Connection)
            {

                connectionNextResult.Open();
                var command = connectionNextResult.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {id}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                }


                using (var conetion = new ORIONDBEntities())
                {
                    var Identidad = idIDentidad;
                    // var InformacionPrestamo = conetion.sp_SRCClienteDetalle_Novanet(idIDentidad, null).FirstOrDefault();
                    var InformacionPrestamo = conetion.sp_SRCClienteDetalle_Novanet(idIDentidad, idPrestamo).FirstOrDefault();

                    var model = new InformacionClientePrincipalViewModel
                    {
                        fcIdentidad = InformacionPrestamo.fcIdentidad,
                        fcNombreSAF = InformacionPrestamo.fcNombreSAF,
                        fcTelefonos = InformacionPrestamo.fcTelefonos,
                        fcMunicipio = InformacionPrestamo.fcMunicipio,
                        fcDireccionDetallada = InformacionPrestamo.fcDireccionDetallada,
                        fcBarrio = InformacionPrestamo.fcBarrio,
                        fcDescripcionAntiguedad = InformacionPrestamo.fcDescripcionAntiguedad,
                        fcDescripcion_RangosSalariales = InformacionPrestamo.fcDescripcion_RangosSalariales,
                        fcLugarTrabajo = InformacionPrestamo.fcLugarTrabajo,
                        fcIDPrestamo = InformacionPrestamo.fcIDPrestamo,
                        fcClienteSAF = fcClienteSAF,
                        idSolicitud = id,
                        fcCorreo = listaDetalleCliente.fcCorreo,


                    };
                    connectionNextResult.Close();
                    return PartialView(model);
                }

            }

        }

        public ActionResult PosteoPago()
        {
            return View();

        }

        public ActionResult MostrarDetalleCreditoCliente(string idCustomer, string idLoan)
        {
            try
            {
                var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
                using (var conetion = new ORIONDBEntities())
                {
                    var modelClientesCredito = conetion.sp_Prestamo_InformacionTotal_Novanet(1, GetIdUser(), idCustomer).FirstOrDefault();
                    if (modelClientesCredito == null) return PartialView(new InformacionCreditoClienteViewModel { fcIDCliente = idCustomer });
                    var clienteDetalle = new InformacionCreditoClienteViewModel
                    {
                        fcPrestamo = modelClientesCredito.fcIDPrestamo,
                        fcIDCliente = modelClientesCredito.fcClienteSAF,
                        fiDiasAtraso = modelClientesCredito.fiDiasAtraso,
                        fiCuotasAtraso = modelClientesCredito.fiCuotasAtrasadas,
                        fiCuotasPagadas = modelClientesCredito.fiCuotasPagadas,
                        fiTotalCuotas = modelClientesCredito.fiTotalCuotas,
                        fnValorCuota = modelClientesCredito?.fnValorCuota ?? 0,
                        fnCapitalOtorgado = modelClientesCredito.fnCapitalFinanciado,
                        fnSaldoActual = modelClientesCredito.fnSaldoActualCapital,
                        fnCapitalVencido = modelClientesCredito.fnCapitalVencido,
                        fnInteresesVencidos = modelClientesCredito.fnInteresesVencidos,
                        fnRecargos = modelClientesCredito.fnRecargos ?? 0,
                        fnSaldoPonerAlDia = modelClientesCredito.fnSaldoPonerAlDia,
                        fnTotalCuenta = modelClientesCredito.fnTotalCuenta,
                        fdFechaInicioPrestamo = modelClientesCredito.fdFechaInicioPrestamo,
                        fdFechaPrimerPagoPrestamo = modelClientesCredito.fdFechaPrimerPagoPrestamo,
                        fdFechaVencePrestamo = modelClientesCredito.fdFechaVencePrestamo,
                        fdUltimoAbono = modelClientesCredito.fdUltimoAbono,
                        fnValorUltimoAbono = modelClientesCredito.fnValorUltimoAbono,
                        fcProducto = modelClientesCredito.fcProducto,
                        fcTipoDePlazo = modelClientesCredito.fcTipoDePlazo,
                        SolicitudMaestroNovanet = modelClientesCredito.SolicitudMaestroNovanet



                    };


                    var listImagenesInstalacion = new List<ListImagenesInstalacionViewModel>();
                    var ListaImagenesInstalacion = conetion.sp_Documentos_Instalacion_List(1, modelClientesCredito.SolicitudMaestroNovanet).ToList();
                    foreach (var foto in ListaImagenesInstalacion)
                    {
                        listImagenesInstalacion.Add(new ListImagenesInstalacionViewModel
                        {
                            fcDescription = foto.fcDescripcion,
                            fcUrlFile = foto.fcUrlDocumento,
                            fcFileName = foto.fcNombreDocumento
                        });
                    }

                    clienteDetalle.ListaImagenesInstalacion = listImagenesInstalacion;

                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {clienteDetalle.SolicitudMaestroNovanet}";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            //listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                            reader.NextResult();
                            //listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();
                            reader.NextResult();
                            listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                        }

                        connection.Close();
                    }
                    clienteDetalle.ListaGarantia = listaEquifaxGarantia;
                    return PartialView(clienteDetalle);

                }
            }
            catch (Exception e)
            {
                return PartialView(new InformacionCreditoClienteViewModel());
            }


        }

        public ActionResult MostrarDetalleOrdenTrabajo(int IDSolicitud, string Identidad)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                int IDUser = GetIdUser();
                connection.Open();

                var commandEquifax = connection.CreateCommand();
                commandEquifax.CommandText = $"SELECT TOP 1 fiIDEquifax FROM Equifax_Cliente WHERE fcIdentidad = '{Identidad}'";

                int fiIDEquifax = 0;

                using (var reader = commandEquifax.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fiIDEquifax = reader.GetInt32(0);
                    }
                }

                var list = new List<DetalleInstalacionesCliente>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_SolicitudesDeTrabajo_PorIDCliente {fiIDEquifax}, {IDUser}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    list = db.ObjectContext.Translate<DetalleInstalacionesCliente>(reader).ToList();
                }

                connection.Close();

                var filteredList = list.ToList();
                return PartialView(filteredList);
            }
        }


        public ActionResult AbrirInformacionDePago(string idCustomer, string idLoan, string idClienteSAF)
        {
            try
            {
                using (var conetionCore2 = new CoreFinancieroEntities2())
                {
                    using (var conetion = new ORIONDBEntities())
                    {
                        conetionCore2.Database.CommandTimeout = 0;
                        var modelClientesCredito = conetion.sp_Prestamo_InformacionTotal_Novanet(1, GetIdUser(), idLoan).FirstOrDefault();
                        if (modelClientesCredito == null) return PartialView(new InformacionCreditoClienteViewModel { fcIDCliente = idCustomer });

                        var EstadoDeResultado = conetionCore2.Database.SqlQuery<CustomerStatementDetailViewModel>($"Corefinanciero.dbo.sp_SRCEstadoDeCuenta_PorPrestamo '{idLoan}' ").ToList();
                        var totalAmountReceived = (EstadoDeResultado.Any(z => z.fnTotal < 0) ? (EstadoDeResultado.Where(z => z.fnTotal < 0).Sum(z => z.fnTotal)) : 0m) * (-1);
                        var totalPrincipal = (EstadoDeResultado.Any(z => z.fnCapital < 0) ? (EstadoDeResultado.Where(z => z.fnCapital < 0).Sum(z => z.fnCapital)) : 0m) * (-1);
                        var totalInterest = (EstadoDeResultado.Any(z => z.fnInteresOrdinario < 0) ? (EstadoDeResultado.Where(z => z.fnInteresOrdinario < 0).Sum(z => z.fnInteresOrdinario)) : 0m) * (-1);
                        var totalLateFees = (EstadoDeResultado.Any(z => z.fnInteresMoratorio < 0) ? (EstadoDeResultado.Where(z => z.fnInteresMoratorio < 0).Sum(z => z.fnInteresMoratorio)) : 0m) * (-1);
                        var totalOthers = (EstadoDeResultado.Any(z => z.fnOtrosCargos < 0) ? (EstadoDeResultado.Where(z => z.fnOtrosCargos < 0).Sum(z => z.fnOtrosCargos)) : 0m) * (-1);
                        var totalInsurance = (EstadoDeResultado.Any(z => z.fnSegurosDeuda < 0) ? (EstadoDeResultado.Where(z => z.fnSegurosDeuda < 0).Sum(z => z.fnSegurosDeuda)) : 0m) * (-1);

                        var APR = ExcelFunctionsHelper.APR(modelClientesCredito.fiTotalCuotas, modelClientesCredito.fnValorCuotaPrestamo, modelClientesCredito.fnCapitalFinanciado);

                        // var comandLoanFinancialInfo = $"{StoreProcedure_Command.SP_LoanFinancialInfo_Query}, '{idLoan}'";
                        // var listCuentaLoan = _connection.LoadListDataWithDbContext<ListLoanCuentasContablesViewModel>(comandLoanFinancialInfo);
                        //var PaidToDealer = listCuentaLoan.FirstOrDefault(z => z.CuentaContable.Trim() == LoanCuentasContablesEnum.CuentaEnlaceEntreMódulosSAF5)?.Credito ?? 0;

                        var clienteDetalle = new InformacionCreditoClienteViewModel
                        {
                            fcNombre = modelClientesCredito.fcNombre,
                            fcPrestamo = modelClientesCredito.fcIDPrestamo,
                            fcIDCliente = modelClientesCredito.fcClienteSAF,
                            fiDiasAtraso = modelClientesCredito.fiDiasAtraso,
                            fiCuotasAtraso = modelClientesCredito.fiCuotasAtrasadas,
                            fiCuotasPagadas = modelClientesCredito.fiCuotasPagadas,
                            fiTotalCuotas = modelClientesCredito.fiTotalCuotas,
                            fnValorCuota = modelClientesCredito?.fnValorCuota ?? 0,
                            fnCapitalOtorgado = modelClientesCredito.fnCapitalFinanciado,
                            fnSaldoActual = modelClientesCredito.fnSaldoActualCapital,
                            fnCapitalVencido = modelClientesCredito.fnCapitalVencido,
                            fnInteresesVencidos = modelClientesCredito.fnInteresesVencidos,
                            fnRecargos = modelClientesCredito.fnRecargos ?? 0,
                            fnSaldoPonerAlDia = modelClientesCredito.fnSaldoPonerAlDia,
                            fnTotalCuenta = modelClientesCredito.fnTotalCuenta,
                            fdFechaInicioPrestamo = modelClientesCredito.fdFechaInicioPrestamo,
                            fdFechaPrimerPagoPrestamo = modelClientesCredito.fdFechaPrimerPagoPrestamo,
                            fdFechaVencePrestamo = modelClientesCredito.fdFechaVencePrestamo,
                            fdUltimoAbono = modelClientesCredito.fdUltimoAbono,
                            fnValorUltimoAbono = modelClientesCredito.fnValorUltimoAbono,
                            fcClienteSAF = idClienteSAF,

                            TotalAmountReceived = totalAmountReceived,
                            Amount_Principal = totalPrincipal,
                            Amount_Interest = totalInterest,
                            Amount_LateFees = totalLateFees,

                            Frecuency = modelClientesCredito.fcTipoDePlazo,
                            APR = APR,
                            PaymentsQuantity = modelClientesCredito.fiTotalCuotas,
                            //    DownPayment = modelClientesCredito.fnValorPrima ?? 0,
                            //    ValorMercadoGarantia = modelClientesCredito.fnValorGarantia ?? 0,
                            //    fnValorAPrestar = modelClientesCredito.fnValorAPrestar ?? 0,
                            //    fnValorTotalContrato = modelClientesCredito.fnValorTotalContrato ?? 0,
                            //    fnValorTotalFinanciamiento = modelClientesCredito.fnValorTotalFinanciamiento ?? 0,
                            //    fnValorTotalSeguro = modelClientesCredito.fnValorTotalSeguro ?? 0,
                            //    fnCostoGPS = modelClientesCredito.fnCostoGPS ?? 0,
                            //    fnGastosDeCierre = modelClientesCredito.fnGastosDeCierre ?? 0
                        };
                        return PartialView(clienteDetalle);
                    }
                }


            }
            catch (Exception e)
            {
                return PartialView(new InformacionCreditoClienteViewModel());
            }
        }

        public ActionResult ShowBitacoraCustomer(string idSolicitud)
        {
            var IDSolicitud = Convert.ToInt32(idSolicitud);
            var DatosBitacorasListado = new List<sp_OrionListaBitacoras_Clientes_ViewModel>();
            var DatosBitacorasListadoMantoSolicitud = new List<sp_OrionListaBitacoras_Clientes_ViewModel>();
            try
            {
                using (var conetion = new ORIONDBEntities())
                {

                    var listBitacoraDB = conetion.sp_OrionListaBitacoras_Clientes(IDSolicitud).ToList();
                    var listBitacoraMantoSolicitudes = conetion.sp_BitacoraMantenimientoHistorialSolicitudLista(IDSolicitud).ToList();

                    foreach (var bita in listBitacoraDB)
                    {
                        var ImagerUrl = "";

                        DatosBitacorasListado.Add(new sp_OrionListaBitacoras_Clientes_ViewModel
                        {

                            fcNombreCorto = bita.fcPrimerNombre,
                            fcBitacora = bita.fcBitacora,
                            fcGestion = bita.fcGestion,
                            fdGestion = bita.fdGestion

                        });
                    }

                    foreach (var bita in listBitacoraMantoSolicitudes)
                    {
                        var ImagerUrl = "";

                        DatosBitacorasListadoMantoSolicitud.Add(new sp_OrionListaBitacoras_Clientes_ViewModel
                        {

                            fcNombreCorto = bita.fcNombre,
                            fcBitacora = bita.fcObservaciones,
                            fcGestion = "Manto Solicitud",
                            fdFechaVolveraLlamar = bita.fdFechaMantenimiento

                        });
                    }
                    var model = new SolicitudesViewModel
                    {
                        fiIDOrionSolicitud = IDSolicitud,
                        BitacorasClienteSoporte = DatosBitacorasListado,
                        BitacorasClienteMantoSolicitudes = DatosBitacorasListadoMantoSolicitud
                    };
                    return PartialView(model);
                }
            }
            catch (Exception)
            {
                return PartialView(new SolicitudesViewModel());
            }

        }
        public ActionResult AbrirDocumentos(string idSolicitud)
        {
            var IDSolicitud = Convert.ToInt32(idSolicitud);
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            var listaReferencias = new List<SolicitudesReferenciaViewModel>();
            var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        reader.NextResult();
                        reader.NextResult();
                        reader.NextResult();
                        DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();

                    }

                    connection.Close();
                }
                var model = new SolicitudesViewModel
                {
                    fiIDOrionSolicitud = IDSolicitud,
                    DatosDocumentoListado = DatosDocumentoListado
                };
                return PartialView(model);
            }
            catch (Exception)
            {
                return PartialView(new SolicitudesViewModel());
            }

        }

        public ActionResult AbrirDocumentosPersonales(string idSolicitud)
        {
            var IDSolicitud = Convert.ToInt32(idSolicitud);
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            var listaReferencias = new List<SolicitudesReferenciaViewModel>();
            var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        reader.NextResult();
                        reader.NextResult();
                        reader.NextResult();
                        DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();

                    }

                    connection.Close();
                }
                var model = new SolicitudesViewModel
                {
                    fiIDOrionSolicitud = IDSolicitud,
                    DatosDocumentoListado = DatosDocumentoListado
                };
                return PartialView(model);
            }
            catch (Exception)
            {
                return PartialView(new SolicitudesViewModel());
            }

        }

        public JsonResult LoadListStatementAccount(string idCustomer, string idLoan, string idClienteSAF)
        {

            using (var conetion = new CoreFinancieroEntities2())
            {
                conetion.Database.CommandTimeout = 0;
                var EstadoDeResultado = conetion.Database.SqlQuery<CustomerStatementDetailViewModel>($"Corefinanciero.dbo.sp_SRCEstadoDeCuenta '{idClienteSAF}', '{idLoan}' ").ToList();
                var saldo = 0m;
                var count = 1;
                EstadoDeResultado.ForEach(z =>
                {
                    z.SaldoDelPeriodo = z.fnTotal + saldo;
                    saldo += z.fnTotal;
                    z.ItemNumber = count;
                    count++;
                });
                return EnviarListaJson(EstadoDeResultado);

            }


        }
        public JsonResult ListaPlanDePago(string idCustomer, string idLoan, string idClienteSAF)
        {


            using (var conetion = new CoreFinancieroEntities2())
            {
                conetion.Database.CommandTimeout = 0;

                var EstadoDeResultado = conetion.Database.SqlQuery<sp_Prestamo_PlandePago_ConsultarAvancePorPrestamo_ViewModel>($"Corefinanciero.dbo.sp_SRCClientesPlandePago '{idClienteSAF}', '{idLoan}' ").ToList();
                var count = 1;
                foreach (var item in EstadoDeResultado)
                {
                    item.ItemNumber = count;
                    count++;

                }
                ;
                return EnviarListaJson(EstadoDeResultado);

            }


        }

        [HttpPost]
        public JsonResult CrearSolicitudCorteAdministrativo(SolicitudesViewModel model, string Comentario)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = Convert.ToInt32(context.sp_OrionSolicitud_SolicitudCorte_Insertar(model.IDSolicitud, model.IdCliente, model.fiIDEstadoInstalacion, Comentario, GetIdUser()).FirstOrDefault());
                        return EnviarResultado(true, "Datos", "Crear Solicitud");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        #endregion
        #region Soporte
        [HttpGet]
        public ActionResult ModalCrearBitacoras(string Nombre, int IDSolicitud)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListaGestion {1} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListadoGestion = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDGestion), Text = z.fcGestion }).ToList();


                }
                connection.Close();
            }
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, fiIDGestionAnterior = 1 });
            }

        }
        [HttpGet]
        public ActionResult ModalVerBitacorasCliente(string Nombre, int IDSolicitud)
        {
            try
            {
                var DatosBitacorasListado = new List<sp_OrionListaBitacoras_Clientes_ViewModel>();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionListaBitacoras_Clientes {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        DatosBitacorasListado = db.ObjectContext.Translate<sp_OrionListaBitacoras_Clientes_ViewModel>(reader).ToList();

                    }

                    connection.Close();
                }

                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, BitacorasClienteSoporte = DatosBitacorasListado });
            }
            catch (Exception e)
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud });
            }


        }
        [HttpPost]
        public JsonResult GuardarBitacoras(SolicitudesViewModel model, int IdGestion, string ObservacionesBitacoras)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_Orion_ClienteBitacora_Registrar(GetIdUser(), IdGestion, ObservacionesBitacoras, model.IDSolicitud) > 0;

                        return EnviarResultado(true, "Datos", "Datos Registrados");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        #endregion

        #region Promocion
        [HttpGet]
        public ActionResult ModalAgregarCodigoPromocion(string Nombre, int IDSolicitud)
        {

            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, fcCodigoVendedor = "" });
            }

        }

        [HttpPost]
        public JsonResult GuardarCodigoPromocion(SolicitudesViewModel model, string CodigoPromocion, string ObservacionesBitacoras)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var list = context.sp_VentaSolicitudCliente_Promocion_Codigo(model.IDSolicitud, GetIdUser()).First();
                        //if (list.Any(x => x.fcCodigoPromocion == CodigoPromocion))
                        if (list.fcCodigoPromocion != CodigoPromocion)
                        {
                            return EnviarResultado(false, "", "El Codigo Es invalido");
                        }
                        if (list.fiEstadoPromocion == 2)
                        {
                            return EnviarResultado(false, "", "El Codigo ya fue Utilizado");
                        }
                        var result = context.sp_VentaSolicitudCliente_Promocion_Actualizar(model.IDSolicitud, CodigoPromocion, 2, GetIdUser()) > 0;
                        return EnviarResultado(true, "Datos", "Datos Actualizar");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        #endregion

        [HttpGet]
        public JsonResult ConsultarClientePrestamo(string Identidad)
        {

            using (var conetion = new CoreFinancieroEntities2())
            {
                var model = conetion.sp_OperacionesCaja_ConsultarCliente_NovaNet(1, 117, GetIdUser(), Identidad).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }

        }



        [HttpGet]
        public JsonResult ConsultarClienteUltimosMovimientios(string idPRestamo)
        {

            using (var conetion = new CoreFinancieroEntities2())
            {
                return Json(conetion.sp_OperacionesCaja_UltimosMovimientos(idPRestamo).ToList(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult ConsultarClientesPagoHoy(DateTime Fecha)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_Prestamos_ClientesaPagarPorFecha '{Fecha.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var listaEquifaxGarantia = db.ObjectContext.Translate<sp_Prestamos_ClientesaPagarPorFecha_Result>(reader).ToList();
                    return EnviarListaJson(listaEquifaxGarantia);
                }


            }


        }

        [HttpGet]
        public JsonResult ConsultarClientesPagoAtrasado(DateTime Fecha)
        {

            using (var conetion = new ORIONDBEntities())
            {
                return Json(conetion.sp_Prestamos_ClientesEnAtraso(Fecha).ToList(), JsonRequestBehavior.AllowGet);
            }

            //using (var connection = (new ORIONDBEntities()).Database.Connection)
            //{

            //    connection.Open();
            //    var command = connection.CreateCommand();
            //    command.CommandText = $@"EXEC sp_Prestamos_ClientesEnAtraso '{Fecha.ToString("yyyy-MM-dd")}'";
            //    using (var reader = command.ExecuteReader())
            //    {
            //        var db = ((IObjectContextAdapter)new ORIONDBEntities());
            //        var listaEquifaxGarantia = db.ObjectContext.Translate<sp_Prestamos_ClientesEnAtraso_Result>(reader).ToList();
            //        return EnviarListaJson(listaEquifaxGarantia);
            //    }


            //}

        }

        [HttpGet]
        public JsonResult ConsultarPrestamoPosteo(string idPRestamo, string Identidad)
        {
            using (var connection = (new CoreFinancieroEntities2()).Database.Connection)
            {

                connection.Open();
                var list = new List<sp_OperacionesCaja_ConsultarCliente_NovaNet_Result>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OperacionesCaja_ConsultarCliente_NovaNet_PorPrestamo {1} ,{117} ,{GetIdUser()},'{Identidad}','{idPRestamo}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new CoreFinancieroEntities2());
                    list = db.ObjectContext.Translate<sp_OperacionesCaja_ConsultarCliente_NovaNet_Result>(reader).ToList();
                }



                connection.Close();

                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult ListadoCuentas()
        {

            using (var conetion = new CoreFinancieroEntities2())
            {
                var model = conetion.sp_OperacionesBanco_CuentasdeBanco(1, 117, 1, 1).Select(x => new { id = x.fcCuentaContable, text = x.fcDescripcionCuenta }).ToList();
                return EnviarListaJson(model);
            }

        }


        [HttpGet]
        public JsonResult listadoFechaAbono()
        {
            using (var connection = (new CoreFinancieroEntities2()).Database.Connection)
            {
                connection.Open();
                var list = new List<FechaAbonoViewModel>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OperacionesCaja_FechasRetroactivas {1} ,{1} ,{GetIdUser()}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new CoreFinancieroEntities2());
                    list = db.ObjectContext.Translate<FechaAbonoViewModel>(reader).ToList();
                }

                var model = list.Select(x => new { id = x.fcFechaCaracter, text = x.fcFechaCaracter });

                connection.Close();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostearPago(string Identidad, decimal abono, string idPRestamo, string Comentario, string Cuenta, string referencia, string FehcaAbonos)
        {
            try
            {
                int parametro = 0;
                var fcCorreo = new List<CorreoViewModels>();
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_CorreoCliente_Identidad '{Identidad}'";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        fcCorreo = db.ObjectContext.Translate<CorreoViewModels>(reader).ToList();
                    }

                    connection.Close();
                }


                using (var conetion = new CoreFinancieroEntities2())
                {
                    var Transaccion = new SqlParameter("piIDTransaccion", SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    if (FehcaAbonos == "Hoy")
                    {
                        var sql = $"sp_OperacionesCaja_AplicarAbono_NovaNet 117,{GetIdUser()}, '{Request.UserHostAddress}', '{Identidad}', {abono}, '{idPRestamo}', '{Comentario}', '{Cuenta}', '{referencia}', @piIDTransaccion Out";
                        conetion.Database.ExecuteSqlCommand(sql, Transaccion);
                    }
                    else
                    {
                        var sql = $"sp_OperacionesCaja_AplicarAbono_NovaNet 117,{GetIdUser()}, '{Request.UserHostAddress}', '{Identidad}', {abono}, '{idPRestamo}', '{Comentario}', '{Cuenta}', '{referencia}', @piIDTransaccion Out,'{FehcaAbonos}'";
                        conetion.Database.ExecuteSqlCommand(sql, Transaccion);
                    }




                    //var model = conetion.sp_OperacionesCaja_AplicarAbono_NovaNet(117, GetIdUser(), Request.UserHostName, Identidad, abono, idPRestamo, Comentario, Cuenta, referencia, Transaccion).ToList();
                    EnviarMensajeDePago(Convert.ToInt32(Transaccion.Value));
                    EnviarMensajeDeFactura(Convert.ToInt32(Transaccion.Value));
                    parametro = Convert.ToInt32(Transaccion.Value);



                    var _emailTemplateService = new EmailTemplateService();
                    await _emailTemplateService.SendEmailToSolicitudFactura(new EmailTemplateServiceModel
                    {
                        CustomerEmail = fcCorreo[0].fcCorreo,
                        //CustomerEmail = "kevinsanme@gmail.com",
                        IdTransaccion = Convert.ToInt32(Transaccion.Value)
                        //IdTransaccion = 7598
                    });
                }

                return EnviarListaJson(new List<int>());
            }
            catch (Exception ex)
            {
                var _emailTemplateService = new EmailTemplateService();
                await _emailTemplateService.SendEmailToSolicitudFactura(new EmailTemplateServiceModel
                {

                    CustomerEmail = "kevinsanme@gmail.com",
                    IdTransaccion = 7598
                });
                EnviarException(ex, "Error");
                return EnviarListaJson(new List<int>());
            }




        }

        public void EnviarMensajeDeFactura(int Transaccion)
        {
            using (var context = new CoreFinancieroEntities2())
            {
                try
                {
                    var result = context.sp_OperacionesCaja_ConsultarAbono(Transaccion, null).FirstOrDefault();
                    string telefonoCliente = result.fcTelefonoSMS;
                    string lcURLMensajedeTexto = "";
                    //if (telefonoCliente.Substring(0) == "9") // metodo de mensaje normales, si ya no se ocupa borrarlo actualizacion el 29/04/2024
                    //{
                    //    lcURLMensajedeTexto = @"https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + telefonoCliente.Trim() + "&MESSAGE=Gracias por su pago. Puede consultar su abono en https://ptdto.com/dt/dt.aspx?" + Transaccion.ToString();
                    //}
                    //else
                    //{
                    //    lcURLMensajedeTexto = @"https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&SOURCEADDR=NOVANET&DESTADDR=504" + telefonoCliente.Trim() + "&MESSAGE=Gracias por su pago. Puede consultar su abono en https://ptdto.com/dt/dt.aspx?" + Transaccion.ToString();
                    //}

                    lcURLMensajedeTexto = "Puede consultar su *FACTURA* en https://ptdto.com/dt/fac.aspx?" + Transaccion.ToString();

                    MensajeriaApi.MensajesDigitales(telefonoCliente, lcURLMensajedeTexto, "");

                    //WebRequest request = WebRequest.Create(lcURLMensajedeTexto);
                    //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //Stream dataStream = response.GetResponseStream();
                    //StreamReader reader = new StreamReader(dataStream);
                    //string responseFromServer = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    EnviarException(ex, "Error");
                }
            }

        }

        public void EnviarMensajeDePago(int Transaccion)
        {
            using (var context = new CoreFinancieroEntities2())
            {
                try
                {
                    var result = context.sp_OperacionesCaja_ConsultarAbono(Transaccion, null).FirstOrDefault();
                    string telefonoCliente = result.fcTelefonoSMS;
                    string lcURLMensajedeTexto = "";
                    //if (telefonoCliente.Substring(0) == "9") // metodo de mensaje normales, si ya no se ocupa borrarlo actualizacion el 29/04/2024
                    //{
                    //    lcURLMensajedeTexto = @"https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + telefonoCliente.Trim() + "&MESSAGE=Gracias por su pago. Puede consultar su abono en https://ptdto.com/dt/dt.aspx?" + Transaccion.ToString();
                    //}
                    //else
                    //{
                    //    lcURLMensajedeTexto = @"https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&SOURCEADDR=NOVANET&DESTADDR=504" + telefonoCliente.Trim() + "&MESSAGE=Gracias por su pago. Puede consultar su abono en https://ptdto.com/dt/dt.aspx?" + Transaccion.ToString();
                    //}

                    lcURLMensajedeTexto = "Gracias por su pago. Puede consultar su *ABONO* en https://ptdto.com/dt/dt.aspx?" + Transaccion.ToString();

                    MensajeriaApi.MensajesDigitales(telefonoCliente, lcURLMensajedeTexto, "");

                    //WebRequest request = WebRequest.Create(lcURLMensajedeTexto);
                    //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //Stream dataStream = response.GetResponseStream();
                    //StreamReader reader = new StreamReader(dataStream);
                    //string responseFromServer = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    EnviarException(ex, "Error");
                }
            }

        }

        //[HttpGet]
        //public ActionResult ModalGuardarFichoLink(string Identidad, string Nombre, int Cuota, DateTime Fecha)
        //{
        //    return PartialView(new SolicitudesViewModel { Nombre = Nombre, Identidad = Identidad, Cuota = Cuota, fdfechaMaximaPago = Fecha });
        //}


        [HttpPost]
        public ActionResult ModalGuardarFichoLink(string clientes)
        {
            var listaClientes = JsonConvert.DeserializeObject<List<SolicitudesViewModel>>(clientes);
            return PartialView("ModalGuardarFichoLink", listaClientes);
        }


        // [HttpGet]
        //public ActionResult ModalGuardarFichoLinkAtrasados(string Identidad, string Nombre, int Cuota)
        //{
        //    return PartialView(new SolicitudesViewModel { Nombre = Nombre, Identidad = Identidad, Cuota = Cuota });
        //}


        [HttpPost]
        public ActionResult ModalGuardarFichoLinkAtrasados(string clientes)
        {
            var listaClientes = JsonConvert.DeserializeObject<List<SolicitudesViewModel>>(clientes);
            return PartialView("ModalGuardarFichoLinkAtrasados", listaClientes);
        }


        [HttpPost]
        public JsonResult GuardarFicoLink(List<SolicitudesViewModel> model, string FicoLink)
        {
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    foreach (var item in model)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = $@"EXEC sp_Clientes_LinkdePago_Agregar '{item.Identidad}', {item.Cuota}, '{item.fdfechaMaximaPago.ToString("yyyy-MM-dd")}', '{FicoLink}', {GetIdUser()}";
                        command.ExecuteReader();
                    }
                    return EnviarResultado(true, "FicoLink", "Se guardaron exitosamente los FicoLinks para todos los clientes seleccionados.");
                }
            }
            catch (Exception ex)
            {

                return EnviarResultado(false, "FicoLink", ex.Message);
            }


        }

        //[HttpPost]
        //public JsonResult GuardarFicoLinkAtrasado(SolicitudesViewModel model, string FicoLink, DateTime FechaMaxima)
        //{

        //    using (var connection = (new ORIONDBEntities()).Database.Connection)
        //    {

        //        connection.Open();
        //        var command = connection.CreateCommand();
        //        command.CommandText = $@"EXEC sp_Clientes_Atrasados_LinkdePago_Agregar '{model.Identidad}',{model.Cuota},'{FechaMaxima:yyyy-MM-dd}','{FicoLink}',{GetIdUser()}";
        //        using (var reader = command.ExecuteReader())
        //        {
        //            return EnviarResultado(true, "FicoLink", "Se guardo Exitosamente");
        //        }

        //        return EnviarResultado(false, "FicoLink", "Error en el Guardado");

        //    }

        //}

        [HttpPost]
        public JsonResult GuardarFicoLinkAtrasado(List<SolicitudesViewModel> model, string FicoLink)
        {
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    foreach (var item in model)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = $@"EXEC sp_Clientes_Atrasados_LinkdePago_Agregar '{item.Identidad}',{item.Cuota},'{item.FechaMaxima.ToString("yyyy-MM-dd")}','{FicoLink}',{GetIdUser()}";
                        command.ExecuteReader();
                    }
                    return EnviarResultado(true, "FicoLink", "Se guardaron exitosamente los FicoLinks para todos los clientes seleccionados.");
                }
            }
            catch (Exception ex)
            {

                return EnviarResultado(false, "FicoLink", ex.Message);
            }

        }

        [HttpPost]
        public JsonResult EliminarFicoLink(string Identidad, int Cuota, DateTime fdfechaMaximaPago)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_Eliminar_Clientes_LinksdePago '{Identidad}',{Cuota},'{fdfechaMaximaPago.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    return EnviarResultado(true, "FicoLink", "Se Elimino Exitosamente");
                }

                return EnviarResultado(false, "FicoLink", "Error en el Guardado");

            }

        }

        [HttpPost]
        public JsonResult EliminarFicoLinkAtrasado(string Identidad, int Cuota, DateTime fdfechaMaximaPago)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_Eliminar_Clientes_Atrasados_LinksdePago '{Identidad}',{Cuota},'{fdfechaMaximaPago.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    return EnviarResultado(true, "FicoLink", "Se Elimino Exitosamente");
                }

                return EnviarResultado(false, "FicoLink", "Error en el Guardado");

            }

        }

        public ActionResult ClientesPagoHoy()
        {
            return View();

        }

        public ActionResult ClientesPrimeraCuota()
        {
            return View();

        }


        [HttpGet]
        public JsonResult ConsultarClientesPrimeraCuotaPagoHoy(DateTime Fecha)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_Prestamos_ClientesaPrimeraCuotaPagarPorFecha '{Fecha.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var listaEquifaxGarantia = db.ObjectContext.Translate<sp_Prestamos_ClientesaPagarPorFecha_Result>(reader).ToList();
                    return EnviarListaJson(listaEquifaxGarantia);
                }


            }


        }

        public ActionResult ClientesPagoPendientes()
        {
            return View();
        }

        [HttpGet]
        public JsonResult EnvioFicoLink(DateTime Fecha)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_MensajeMasiva_Funciones {GetIdUser()},1,'{Fecha.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    return EnviarResultado(true, "FicoLink", "Se mando los Mensajes");
                }
            }
        }

        [HttpGet]
        public JsonResult EnvioFicoLinkAtrazado(DateTime Fecha)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $@"EXEC sp_MensajeMasiva_Funciones {GetIdUser()},2,'{Fecha.ToString("yyyy-MM-dd")}'";
                using (var reader = command.ExecuteReader())
                {
                    return EnviarResultado(true, "FicoLink", "Se mando los Mensajes");
                }
            }
        }

        [HttpGet]
        public ActionResult ViewListaProductosInstalados(int fiIDEquifax)
        {
            return PartialView(fiIDEquifax);
        }

        [HttpGet]
        public JsonResult CargarListaInstalacionDetalle(int fiIDEquifax)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ubicacion = contexto.sp_Catalogo_Ubicaciones_ObtenerPorIdEquifax(fiIDEquifax).FirstOrDefault();
                var list = contexto.sp_Productos_Inventario_ExistenciaInventario(ubicacion.fiIDUbicacion).ToList();
                return EnviarListaJson(list);
            }
        }


        #region FACTURACION
        [HttpGet]
        public ActionResult IndexFacturacion()
        {
            using (var contexto = new ORIONDBEntities())
            {

                return View();
            }
        }

        [HttpGet]
        public ActionResult CargarListaFacturacion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Facturacion_Maestro_Listar().ToList();
                return EnviarListaJson(lista);
            }
        }







        [HttpPost]
        public ActionResult CatalogoDeProductos(List<ListFacturacionDetalleViewModel> lista)
        {
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.ListarBodegasConStock = contexto.sp_BodegasConStock_Listar().Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = x.fcUbicacion }).ToList();
                return PartialView(lista);
            }

        }

        [HttpGet]
        public JsonResult BuscarClienteEquifax(string fcIdentidad)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var model = contexto.sp_SRC_Clientes_BuscarPorIdentidad(fcIdentidad).FirstOrDefault() ?? new sp_SRC_Clientes_BuscarPorIdentidad_Result();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ListInventarioStock(int fiIDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var existencia = contexto.sp_Productos_Maestro_ExistenciaPorUbicacion(fiIDUbicacion).ToList();
                var lista = new List<ListFacturacionDetalleViewModel>();
                foreach (var item in existencia)
                {
                    lista.Add(new ListFacturacionDetalleViewModel
                    {
                        fiIDProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        Foto = string.IsNullOrEmpty(item.fcImagenProducto) ? "" : GetContentDocument("img", @"ImgProductos", System.IO.Path.GetFileName(item.fcImagenProducto)),
                        fnExistencia = item.fnCantidad,
                        fnValorVentaDeContado = item.fnValorVentaDeContado ?? 0
                    });
                }
                return Json(lista, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult VerEditarValorVenta(int fiIDProducto, int fiMoneda = 2)
        {
            using (var contexto = new ORIONDBEntities())
            {
                contexto.Database.CommandTimeout = int.MaxValue;
                var modelDb = contexto.sp_Producto_Maestro_GetById(fiIDProducto).FirstOrDefault();

                var model = new ListFacturacionDetalleViewModel
                {
                    fiIDProducto = fiIDProducto,
                    fcProducto = modelDb.fcProducto,
                    fnValorVentaDeContado = modelDb.fnValorVentaDeContado ?? 0
                };
                return PartialView(model);
            }
        }

        [HttpPost]
        public JsonResult EditarValorDeContado(ListFacturacionDetalleViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var result = contexto.sp_Producto_Maestro_EditarValorVentaDeContado(model.fiIDProducto, model.fnValorVentaDeContado, GetIdUser(), 2) > 0;
                if (result)
                {
                    return EnviarResultado(true, "Valor de venta", "Valor de contado actualizado");
                }
                else
                {
                    return EnviarResultado(false, "Error", "Error de red");
                }
            }
        }


        [HttpGet]
        public ActionResult CrearFacturacion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var dataFacturacion = contexto.sp_Facturacion_Datos().FirstOrDefault();
                //ViewBag.listClientes = GetClientesSRC();
                ViewBag.fnValorImpuesto = GetConfiguracion<decimal>("Impuesto");
                ViewBag.ListarMonedas = GetListMonedas();
                var modelo = new CrearFacturacionViewModel();
                modelo.fdFechaRangoInicialFacturacion = dataFacturacion.fdIniciodeCorrelativo;
                modelo.fdFechaRangoFinalFacturacion = dataFacturacion.fdFindeCorrelativo;
                modelo.fcCAI = dataFacturacion.fcCAI;
                modelo.fcRangoInicialFacturacion = dataFacturacion.fcInicioCorrelativo;
                modelo.fcRangoFinalFacturacion = dataFacturacion.fcFinCorrelativo;
                modelo.fcReferenciaDocumento = dataFacturacion.fcFactura;
                modelo.fdFechaTransaccion = DateTime.Now;
                modelo.fcCorrelativoDocumento = dataFacturacion.fcNoDeclaracion;
                modelo.fcRTN = dataFacturacion.fcRTN;
                modelo.fiMoneda = 2;
                return View(modelo);
            }
        }

        [HttpPost]
        public JsonResult CrearFacturacion(CrearFacturacionViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var piIDFacturacionMaestro = new ObjectParameter("piIDFacturacionMaestro", typeof(int));
                    contexto.sp_Facturacion_Maestro_Insertar(
                        model.fiIDEquifax,
                        model.fdFechaTransaccion,
                        model.fcCorrelativoDocumento,
                        model.fcReferenciaDocumento,
                        model.fcCAI,
                        model.fnSubTotal,
                        model.fnImporteExento,
                        model.fnImporteExonerado,
                        model.fnImporteGravado,
                        model.fcConcepto,
                        model.fdFechaTransaccion.AddDays(30),
                        model.fcConcepto,
                        GetIdUser(),
                        model.fcNombreCliente,
                        model.fcCorreoCliente,
                        model.fcTelefonoCliente,
                        model.fcIdentidad,
                        model.fnImpuestos,
                        model.fnTotalFactura,
                        piIDFacturacionMaestro,
                        model.fiMoneda);



                    foreach (var item in model.DetalleFacturacion)
                    {
                        contexto.sp_Facturacion_Detalle_Insertar(
                            Convert.ToInt32(piIDFacturacionMaestro.Value),
                            item.fiIDProducto,
                            item.fnCantidad,
                            item.fnValorVentaDeContado,
                            GetIdUser(),
                            item.fbAplicaImpuesto);
                    }
                    return EnviarResultado(true, "", "Facturacion registrada exitosamente");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public ActionResult EditarFacturacion(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(fiIDFacturacionMaestro).FirstOrDefault();
                var modelDbDetalle = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(fiIDFacturacionMaestro).ToList();
                ViewBag.ListarMonedas = GetListMonedas();
                ViewBag.fnValorImpuesto = modelDbDetalle.FirstOrDefault().fnPorcentajeImpuesto1 == 0 || modelDbDetalle.FirstOrDefault().fnPorcentajeImpuesto1 == null ?
                    GetConfiguracion<decimal>("Impuesto") :
                    (modelDbDetalle.FirstOrDefault().fnPorcentajeImpuesto1 / 100);
                var modelo = new CrearFacturacionViewModel();

                modelo.fiIDEquifax = modelDb.fiIDEquifax ?? 0;
                modelo.fiIDFacturacionMaestro = modelDb.fiIDFacturacionMaestro;
                modelo.fdFechaRangoInicialFacturacion = modelDb.fdFechaRangoInicialFacturacion ?? DateTime.Now;
                modelo.fdFechaRangoFinalFacturacion = modelDb.fdFechaRangoFinalFacturacion ?? DateTime.Now;
                modelo.fcCAI = modelDb.fcCAI;
                modelo.fcRangoInicialFacturacion = modelDb.fcRangoInicialFacturacion;
                modelo.fcRangoFinalFacturacion = modelDb.fcRangoFinalFacturacion;
                modelo.fcReferenciaDocumento = modelDb.fcReferenciaDocumento;
                modelo.fdFechaTransaccion = modelDb.fdFechaTransaccion ?? DateTime.Now;
                modelo.fcCorrelativoDocumento = modelDb.fcCorrelativoDocumento;
                modelo.fcRTN = modelDb.fcRTN;
                modelo.fcIdentidad = modelDb.fcIdentidad;
                modelo.fcNombreCliente = modelDb.fcNombreCliente;
                modelo.fcTelefonoCliente = modelDb.fcTelefonoCliente;
                modelo.fcToken = modelDb.fcToken;
                modelo.fbEditar = true;
                modelo.fcConcepto = modelDb.fcConcepto;
                modelo.fnSubTotal = modelDb.fnSubTotal ?? 0;
                modelo.fnDescuento = 0;
                modelo.fnImporteExento = modelDb.fnImporteExento ?? 0;
                modelo.fnImporteExonerado = modelDb.fnImporteExonerado ?? 0;
                modelo.fnImporteGravado = modelDb.fnImporteGravado ?? 0;
                modelo.fnImpuestos = modelDb.fnImpuestos ?? 0;
                modelo.fnTotalFactura = modelDb.fnTotalFactura ?? 0;
                modelo.fcCorreoCliente = modelDb.fcCorreoCliente;
                modelo.DetalleFacturacion = new List<ListFacturacionDetalleViewModel>();
                modelo.fiMoneda = modelDb?.fiMoneda ?? 1;
                foreach (var item in modelDbDetalle)
                {
                    modelo.DetalleFacturacion.Add(new ListFacturacionDetalleViewModel
                    {
                        fbAplicaImpuesto = item.fbAplicaImpuesto ?? false,
                        fbSeleccionado = true,
                        fcProducto = item.fcProducto,
                        fcToken = item.fcToken,
                        fiIDFacturacionDetalle = item.fiIDFacturacionDetalle,
                        fiIDFacturacionMaestro = item.fiIDFacturacionMaestro ?? 0,
                        fiIDProducto = item.fiIDProducto ?? 0,
                        fnCantidad = item.fnCantidad ?? 0,
                        fnExistencia = 0,
                        fnPorcentajeImpuesto1 = item.fnPorcentajeImpuesto1 ?? 0,
                        fnPorcentajeImpuesto2 = item.fnPorcentajeImpuesto2 ?? 0,
                        fnSubTotal = (item.fnValorVentaDeContado ?? 0) * (item.fnCantidad ?? 0),
                        fnTotal = item.fnTotal ?? 0,
                        fnValorImpuesto = item.fnValorImpuesto ?? 0,
                        fnValorProductoME = item.fnValorProductoME ?? 0,
                        fnValorVentaDeContado = item.fnValorVentaDeContado ?? 0,
                        Foto = ""
                    });
                }

                return View("CrearFacturacion", modelo);
            }
        }

        [HttpPost]
        public JsonResult EditarFacturacion(CrearFacturacionViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(model.fiIDFacturacionMaestro).FirstOrDefault();
                    var modelDbDetalle = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(model.fiIDFacturacionMaestro).ToList();

                    var result = contexto.sp_Facturacion_Maestro_Editar(
                        model.fiIDFacturacionMaestro,
                        model.fiIDEquifax,
                        model.fdFechaTransaccion,
                        model.fcCorrelativoDocumento,
                        model.fcReferenciaDocumento,
                        model.fcCAI,
                        model.fnSubTotal,
                        model.fnImporteExento,
                        model.fnImporteExonerado,
                        model.fnImporteGravado,
                        model.fcConcepto,
                        model.fdFechaTransaccion.AddDays(30),
                        model.fcConcepto,
                        GetIdUser(),
                        model.fcNombreCliente,
                        model.fcCorreoCliente,
                        model.fcTelefonoCliente,
                        model.fcIdentidad,
                        model.fnImpuestos,
                        model.fnTotalFactura,
                        model.fcToken) > 0;

                    if (result)
                    {
                        foreach (var item in modelDbDetalle)
                        {
                            if (model.DetalleFacturacion?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.DetalleFacturacion.FirstOrDefault(x => x.fcToken == item.fcToken);
                                model.DetalleFacturacion.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditar = true;

                                contexto.sp_Facturacion_Detalle_Editar(
                                    itemModel.fiIDFacturacionDetalle,
                                    itemModel.fiIDFacturacionMaestro,
                                    itemModel.fiIDProducto,
                                    itemModel.fnCantidad,
                                    itemModel.fnValorVentaDeContado,
                                    GetIdUser(),
                                    itemModel.fbAplicaImpuesto);
                            }
                            else
                            {
                                contexto.sp_Facturacion_Detalle_Eliminar(item.fcToken);
                            }
                        }

                        if (model.DetalleFacturacion != null)
                        {
                            foreach (var item in model.DetalleFacturacion.Where(x => x.fbEditar == false).ToList())
                            {
                                contexto.sp_Facturacion_Detalle_Insertar(
                                    model.fiIDFacturacionMaestro,
                                    item.fiIDProducto,
                                    item.fnCantidad,
                                    item.fnValorVentaDeContado,
                                    GetIdUser(),
                                    item.fbAplicaImpuesto);
                            }
                        }
                    }

                    return EnviarResultado(true, "", "Facturacion editada exitosamente");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        public JsonResult CambiarStatusFactura(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var result = contexto.sp_Facturacion_Maestro_EditarEstado(fiIDFacturacionMaestro, GetIdUser()) > 0;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetPDFFactura(int fiIDFacturacionMaestro)
        {
            var DocumentoInventarioService = new ReportesTemplateService();
            var archivo = DocumentoInventarioService.GenerarFacturaDeVenta(fiIDFacturacionMaestro);
            TempData["ReportePDF"] = archivo;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ViewListaProductosPorId(int fiIDInventarioMovimientoMaestro, int fiIDProducto)
        {
            var model = new ListFacturacionDetalleViewModel { fiIDProducto = fiIDProducto, fiIDInventarioMovimientoMaestro = fiIDInventarioMovimientoMaestro };
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ConfirmarEntrega(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(fiIDFacturacionMaestro)?.FirstOrDefault() ?? null;
                var modelDbDetalle = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(fiIDFacturacionMaestro).ToList();

                var modelo = new CrearFacturacionViewModel();

                modelo.fiIDEquifax = modelDb.fiIDEquifax ?? 0;
                modelo.fiIDFacturacionMaestro = modelDb.fiIDFacturacionMaestro;
                modelo.fdFechaRangoInicialFacturacion = modelDb.fdFechaRangoInicialFacturacion ?? DateTime.Now;
                modelo.fdFechaRangoFinalFacturacion = modelDb.fdFechaRangoFinalFacturacion ?? DateTime.Now;
                modelo.fcCAI = modelDb.fcCAI;
                modelo.fcRangoInicialFacturacion = modelDb.fcRangoInicialFacturacion;
                modelo.fcRangoFinalFacturacion = modelDb.fcRangoFinalFacturacion;
                modelo.fcReferenciaDocumento = modelDb.fcReferenciaDocumento;
                modelo.fdFechaTransaccion = modelDb.fdFechaTransaccion ?? DateTime.Now;
                modelo.fcCorrelativoDocumento = modelDb.fcCorrelativoDocumento;
                modelo.fcRTN = modelDb.fcRTN;
                modelo.fcIdentidad = modelDb.fcIdentidad;
                modelo.fcNombreCliente = modelDb.fcNombreCliente;
                modelo.fcTelefonoCliente = modelDb.fcTelefonoCliente;
                modelo.fcToken = modelDb.fcToken;
                modelo.fbEditar = true;
                modelo.fcConcepto = modelDb.fcConcepto;
                modelo.fnSubTotal = modelDb.fnSubTotal ?? 0;
                modelo.fnDescuento = 0;
                modelo.fnImporteExento = modelDb.fnImporteExento ?? 0;
                modelo.fnImporteExonerado = modelDb.fnImporteExonerado ?? 0;
                modelo.fnImporteGravado = modelDb.fnImporteGravado ?? 0;
                modelo.fnImpuestos = modelDb.fnImpuestos ?? 0;
                modelo.fnTotalFactura = modelDb.fnTotalFactura ?? 0;
                modelo.fcCorreoCliente = modelDb.fcCorreoCliente;
                modelo.fnTasadeCambio = modelDb.fnTasadeCambio ?? 0;
                modelo.fcValorEnLetras = NumerosALetras.ConvertirDolaresAletras(modelDb.fnTotalFactura?.ToString("n2") ?? "0");
                modelo.fnValorLempiras = Math.Round(modelDb.fnTotalFactura ?? 0, 2) * (modelDb.fnTasadeCambio ?? 0);
                modelo.DetalleFacturacion = new List<ListFacturacionDetalleViewModel>();
                foreach (var item in modelDbDetalle)
                {
                    modelo.DetalleFacturacion.Add(new ListFacturacionDetalleViewModel
                    {
                        fbAplicaImpuesto = item.fbAplicaImpuesto ?? false,
                        fbSeleccionado = true,
                        fcProducto = item.fcProducto,
                        fcToken = item.fcToken,
                        fiIDFacturacionDetalle = item.fiIDFacturacionDetalle,
                        fiIDFacturacionMaestro = item.fiIDFacturacionMaestro ?? 0,
                        fiIDProducto = item.fiIDProducto ?? 0,
                        fnCantidad = item.fnCantidad ?? 0,
                        fnExistencia = 0,
                        fnPorcentajeImpuesto1 = (item.fnPorcentajeImpuesto1 ?? 0) / 100,
                        fnPorcentajeImpuesto2 = item.fnPorcentajeImpuesto2 ?? 0,
                        fnSubTotal = (item.fnValorVentaDeContado ?? 0) * (item.fnCantidad ?? 0),
                        fnTotal = item.fnTotal ?? 0,
                        fnValorImpuesto = item.fnValorImpuesto ?? 0,
                        fnValorProductoME = item.fnValorProductoME ?? 0,
                        fnValorVentaDeContado = item.fnValorVentaDeContado ?? 0,
                        Foto = ""
                    });
                }

                return PartialView(modelo);
            }
        }


        public async Task<JsonResult> ContabilizarFactura(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Facturacion_ConfirmarVentaDeContado(fiIDFacturacionMaestro, GetIdUser()) > 0;
                    if (result)
                    {
                        var email = new EmailTemplateService();
                        var correo = new EmailTemplateServiceModel();
                        correo.IdTransaccion = fiIDFacturacionMaestro;
                        correo.HtmlBody = "";
                        await email.SendEmailVenta(correo);
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
        }
        [HttpGet]
        public ActionResult ContabilizarPago(int fiIDFacturacionMaestro)
        {
            using (var coreFinanciero = new CoreFinancieroEntities2())
            {
                using (var contexto = new ORIONDBEntities())
                {
                    ViewBag.LitaCuentasBancos = coreFinanciero.sp_OperacionesBanco_CuentasdeBanco(1, 1, 1, 1).ToList().Select(x => new SelectListItem { Value = x.fcCuentaContable, Text = x.fcDescripcionCuenta }).ToList();
                    return PartialView(new FacturacionDocumentoViewModel { fiIDFacturacionMaestro = fiIDFacturacionMaestro, fdFechaDePago = DateTime.Now });
                }
            }
        }


        public ActionResult ObtenerUsuariosParaDocumento()
        {
            return PartialView();
        }


        public JsonResult EnviarModalParaDocumento(InfoUsuarioViewModel usuario, string usuarioPeticion)
        {
            try
            {
                var modeloR = new ObjSignalRModalViewModel
                {
                    InfoUsuario = usuario,
                    Url = Url.Action("CapturaDeDocumento", "Prestamo"),
                    Obj = new { usuarioPeticion }
                };
                var algo = EnviarFormularioSignalR<object>(modeloR);
                return EnviarResultado(true, "", algo);
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }

        [HttpGet]
        public ActionResult CapturaDeDocumento(string usuarioPeticion)
        {
            var model = new DocumentoExternoViewModel { UsuarioPeticion = usuarioPeticion };
            return PartialView(model);
        }

        [HttpPost]
        public void RetornarDocumento(string usuarioPeticion, HttpPostedFileBase documento, string type)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();

            byte[] thePictureAsBytes = new byte[documento.ContentLength];
            using (BinaryReader theReader = new BinaryReader(documento.InputStream))
            {
                thePictureAsBytes = theReader.ReadBytes(documento.ContentLength);
            }
            string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
            using (var log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry(documento.FileName, EventLogEntryType.Error, 101, 1);

            }
            hubContext.Clients.All.retornarDocumento(new { UsuarioPeticion = usuarioPeticion, Documento = thePictureDataAsString, Nombre = documento.FileName, Type = type });
        }

        [HttpPost]
        public async Task<JsonResult> ContabilizarPago(FacturacionDocumentoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    if (model.Documento == null)
                    {
                        return EnviarResultado(false, "", "Agregue un documento");
                    }

                    var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(model.fiIDFacturacionMaestro).FirstOrDefault();

                    var lempiras = Decimal.Round(Decimal.Round((modelDb.fnTotalFactura ?? 0), 2) * (modelDb.fnTasadeCambio ?? 0), 0);

                    //if (model.fnValorPago >= (lempiras - 1) && model.fnValorPago <= (lempiras + 1)) 
                    //{
                    //    return EnviarResultado(false, "", "VALOR INGRESADO ES DIFERENTE AL DE LA FACTURA");
                    //}


                    //if (model.fnValorPago != Math.Round(modelDb.fnTotalFactura ?? 0, 2))
                    //{
                    //    return EnviarResultado(false, "", "VALOR INGRESADO ES DIFERENTE AL DE LA FACTURA");
                    //}


                    model.fcNombre = model.Documento.FileName;
                    var extension = "img";
                    if (Path.GetExtension(model.Documento.FileName) == ".pdf")
                    {
                        extension = "pdf";
                    }
                    var result = contexto.sp_Facturacion_Documentos_Insertar(model.fiIDFacturacionMaestro, model.fcNombre, model.fcExtension, GetContentDocument(extension, "Facturacion", model.fcNombre), GetIdUser()) > 0;
                    if (result)
                    {
                        await UploadFileServer148Async("Facturacion", model.Documento);
                        contexto.Database.CommandTimeout = int.MaxValue;

                        contexto.sp_Facturacion_Maestro_Contabilizar(model.fiIDFacturacionMaestro, GetIdUser());
                        contexto.sp_Facturacion_Maestro_ContabilizarPago(model.fiIDFacturacionMaestro, model.fcCuentaContableBanco, GetIdUser(), model.fcReferencia, model.fdFechaDePago, model.fcConceptoPago, model.fnValorPago);
                        return EnviarResultado(true, "", "Documento subido exitosamente");
                    }
                    return EnviarResultado(false, "", "ERROR DE RED");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public ActionResult ModalCalculadoraFacturacion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.TazaDeCambio = MemoryLoadManager.ObtenerTasaCambio();
                return PartialView();
            }
        }
        #endregion


    }
}