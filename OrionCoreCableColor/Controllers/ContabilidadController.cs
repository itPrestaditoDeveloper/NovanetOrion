using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CoreAdministrativoDB;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Contabilidad;
using OrionCoreCableColor.Models.EmailTemplateService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class ContabilidadController : BaseController
    {
        // GET: Contabilidad

        [HttpGet]
        public ActionResult ViewPartidaContable(string partida)
        {
            var partidaModel = VerPartidaContableMaestro(1, GetIdUser(), partida);


            var listaDetalleAgrupada = partidaModel.ListaDetalle.GroupBy(z => z.fcDocumento).ToList();
            foreach (var item in listaDetalleAgrupada)
            {
                if (item.Sum(z => z.fnDebito) != item.Sum(z => z.fnCredito))
                {
                    item.ToList().ForEach(z => { z.RevisionRequerida = true; });
                }
            }


            return PartialView(partidaModel);

        }



        public RespuestaControlMesContableModel Control_MesContable_PermisoCrearTransaccion(DateTime fechaTransaccion, int idEmpresa)
        {
            using (var contexto = new CoreAdministrativoEntities())
            {
                var MesContable = contexto.Control_MesContable.FirstOrDefault(z => z.fiAnio == fechaTransaccion.Year && z.fiMes == fechaTransaccion.Month && z.fiIDEmpresa == idEmpresa && z.fbActivo);

                if (MesContable == null) return new RespuestaControlMesContableModel { Resultado = false, Mensaje = "Mes no registrado en el control, registrar mes para permitir crear la partida" };

                return new RespuestaControlMesContableModel { Resultado = !MesContable.fbMesCerrado, Mensaje = MesContable.fbMesCerrado ? "El mes seleccionado esta cerrado, no es permitido crear la partida." : "Permiso concedido" };
            }


        }

        public ListarPartidaViewModel VerPartidaContableMaestro(int idEmpresa, int idUsuario, string numeroPartida)
        {
            using (var conexion = new CoreAdministrativoEntities())
            {
                var model = conexion.sp_PartidaDiario_VerMaestro(1, idUsuario, idEmpresa, numeroPartida).Select(z => new ListarPartidaViewModel
                {
                    fiIDEmpresa = z.fiIDEmpresa,
                    fcPartida = z.fcPartida,
                    fdFechaPartida = z.fdFechaPartida,
                    fcLote = z.fcLote,
                    fcReferencia = z.fcReferencia,
                    fcTipoPartida = z.fcTipoPartida,
                    fcEstadoPartida = z.fcEstadoPartida,
                    fiMoneda = z.fiMoneda,
                    SimboloMoneda = (z.fiMoneda == 1 ? "L" : "$"),
                    fcOrigen = z.fcOrigen,
                    fcCreador = z.fcCreador,
                    fcSinopsis = z.fcSinopsis,
                    fnValorPartida = z.fnValorPartida ?? 0,
                    fdFechaCreacion = z.fdFechaCreacion,
                    IDGUID = z.IDGUID,
                    fiPartidaMayorizada = z.fiPartidaMayorizada,
                    fcPartidaSeguimiento = z.fcPartidaSeguimiento
                }).FirstOrDefault();
                if (model != null)
                    model.ListaDetalle = ListarPartidasContablesDetalle(idEmpresa, idUsuario, numeroPartida);

                return model;

            }
        }



        public List<CrearDetallePartidaViewModel> ListarPartidasContablesDetalle(int idEmpresa, int idUsuario, string numeroPartida)
        {
            using (var conexion = new CoreAdministrativoEntities())
            {
                var model = conexion.sp_PartidaDiario_ListarDetalle(113, idUsuario, idEmpresa, numeroPartida).Select(z => new CrearDetallePartidaViewModel
                {
                    fiIDFila = z.fiIDFila,
                    fiIDEmpresa = z.fiIDEmpresa,
                    fcPartida = z.fcPartida,
                    fdFechaPartida = z.fdFechaPartida,
                    fcCuentaContable = z.fcCuentaContable,
                    fcAuxiliar = z.fcAuxiliar,
                    fcAuxiliarCodigo = z.fcAuxiliarCodigo,
                    fnDebito = z.fnDebito ?? 0,
                    fnCredito = z.fnCredito ?? 0,
                    fcDebitooCredito = z.fcDebitooCredito,
                    fnTasadeCambio = z.fnTasadeCambio,
                    fcEstado = z.fcEstado,
                    fcTipoOperacion = z.fcTipoPartida,
                    fcFondo = "0108",
                    fcCentrodeCosto = z.fcCentrodeCosto,
                    fcPrograma = z.fcPrograma,
                    fcDocumento = z.fcDocumento,
                    fcOrigen = z.fcOrigen,
                    fdFechaCreacionPartida = z.fdFechaCreacionPartida,
                    fcCuenta = z.fcCuenta,
                    fcReferenciaDetalle = z.fcReferencia,
                    IDGUID = z.IDGUID,
                    fiPartidaMayorizada = z.fiPartidaMayorizada,
                    DescripcionCuenta = z.fcDescripcionCuenta,
                    Exento = z.fbExentoFactura
                }).ToList();

                return model;

            }
        }

        [HttpGet]
        public ActionResult ObtenerCorrelativoTipoPartida(string tipoPartida)
        {
            var correlativo = ObtenerCorrelativoPartida(1, tipoPartida);
            var model = new CorrelativoPartidaViewModel { TipoPartida = tipoPartida, Correlativo = correlativo };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewCrearPartidaContable(int fiIDInventarioMovimientoMaestro, int idMoneda = 2)
        {
            using (var contexto = new CoreFinancieroEntities2())
            {
                ViewBag.ListTipoPartida = GetListaTipoPartida();
                ViewBag.ListOperacionesContablesInventario = GetOperacionesContablesInventario();
                ViewBag.ListaMonedas = GetListMonedas();
                var model = new CrearPartidaViewModel
                {
                    fiMoneda = idMoneda,
                    Moneda = idMoneda == 1 ? "MONEDA NACIONAL" : "MONEDA EXTRANJERA",
                    SimboloMoneda = idMoneda == 1 ? "L" : "$",
                    fcCreador = User.Identity.Name,
                    fdFechaPartida = DateTime.Now,
                    fiIDMovimientoMaestro = fiIDInventarioMovimientoMaestro,
                    fcTipoPartida = "INV",
                    
                };

                return PartialView(model);
            }
        }



        public ActionResult ViewCrearPartidaIngresoInventario(int fiIDInventarioMovimientoMaestro, int idMoneda = 2)
        {
            using (var contexto = new CoreFinancieroEntities2())
            {
                ViewBag.ListTipoPartida = GetListaTipoPartida();
                ViewBag.ListOperacionesContablesInventario = GetOperacionesContablesInventario().Where(x=>x.Value == "5").ToList();
                ViewBag.ListaMonedas = GetListMonedas();
                var model = new CrearPartidaViewModel
                {
                    fiMoneda = idMoneda,
                    Moneda = idMoneda == 1 ? "MONEDA NACIONAL" : "MONEDA EXTRANJERA",
                    SimboloMoneda = idMoneda == 1 ? "L" : "$",
                    fcCreador = User.Identity.Name,
                    fdFechaPartida = DateTime.Now,
                    fiIDMovimientoMaestro = fiIDInventarioMovimientoMaestro,
                    fcTipoPartida = "INV",
                    fiIDOperacionesContablesInventario = 5
                };

                return PartialView("ViewCrearPartidaContable", model);
            }
        }

        [HttpGet]
        public JsonResult LoadDetallePartida(string fcpartida)
        {
            using (var contexto = new CoreAdministrativoEntities())
            {
                if (string.IsNullOrEmpty(fcpartida)) return EnviarListaJson(new List<CrearDetallePartidaViewModel>());
                var list = ListarPartidasContablesDetalle(1, 240, fcpartida);
                return EnviarListaJson(list);
            }
        }


        [HttpGet]
        public void CargarListaParaSelectEnDetallePartida(int idMoneda)
        {
            ViewBag.ListCuentasContables = new List<SelectListItem>(); //_cuentaContableLogic.CargarListaCuentasContablesPorMonedaParaSelect(GetIdEmpresaEnSesion(), idMoneda, GetUserIDCoreSeguridad());


            ViewBag.ListCuentasContablesSelect2 = CargarListaCuentasContablesPorMoneda(1, idMoneda, GetIdUser())
                .Select(x => new { id = x.Value, text = x.Text, disabled = x.Disabled }).ToList();
            ViewBag.ListaTipoCuentaAuxiliar = new List<SelectListItem>() { new SelectListItem() { Value = "FIN", Text = "FIN" }, new SelectListItem() { Value = "CXP", Text = "CXP" }, new SelectListItem() { Value = "CXC", Text = "CXC" }, new SelectListItem() { Value = "SEG", Text = "SEG" } };
            ViewBag.ListaFondos = GetListaFondos();
            ViewBag.ListaPrograma = GetListaProgramas();
            ViewBag.ListaCentroCosto = GetListaCentroCosto();

        }

        [HttpGet]
        public ActionResult ViewCrearDetallePartidaContable(CrearDetallePartidaViewModel model)
        {
            CargarListaParaSelectEnDetallePartida(model.IdMoneda);
            model.EsDolar = model.IdMoneda == 2;
            model.fcCentrodeCosto = "0108";
            var tasa = GetTasaDeCambiosPorMes(model.fdFechaPartida);

            model.fnTasadeCambio = model.EsDolar ? tasa : 1;
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult ViewEditarDetallePartidaContable(CrearDetallePartidaViewModel model)
        {
            CargarListaParaSelectEnDetallePartida(model.IdMoneda);
            model.EsDolar = model.IdMoneda == 2;
           
            return PartialView("ViewCrearDetallePartidaContable", model);
        }


        [HttpGet]
        public JsonResult GetPartidaOperacion(int fiIDOperacionesContablesInventario, int fiIDInventarioMovimientoMaestro, int fiMoneda)
        {
            var mDM = new Diario_Maestro();
            var lDD = new List<Diario_Detalle>();
            var model = new CrearPartidaViewModel();
            using (var contexto = new ORIONDBEntities())
            {
                var operacion = contexto.sp_Catalogo_OperacionesContables_Inventario_Listar().FirstOrDefault(x => x.fiIDOperacionesContablesInventario == fiIDOperacionesContablesInventario);

                if (String.IsNullOrEmpty(operacion.fcProcedimientoVista))
                {
                    return Json(new CrearPartidaViewModel(), JsonRequestBehavior.AllowGet);
                }

                var listaParametros = contexto.sp_VerParametrosDeProcedimiento_Listar(operacion.fcProcedimientoVista).ToList();
                using (var contextoAdministrativo = new CoreAdministrativoEntities())
                {

                    using (var conection = contexto.Database.Connection)
                    {
                        conection.Open();
                        var command = conection.CreateCommand();
                        var sql = $"EXEC {operacion.fcProcedimientoVista} {fiIDInventarioMovimientoMaestro}, {fiMoneda}, {1}, {GetIdUser()}";
                        command.CommandText = sql;

                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new CoreAdministrativoEntities());
                            mDM = db.ObjectContext.Translate<Diario_Maestro>(reader).FirstOrDefault();
                            reader.NextResult();
                            lDD = db.ObjectContext.Translate<Diario_Detalle>(reader).ToList();
                        }
                        model.fiIDEmpresa = mDM.fiIDEmpresa;
                        model.fcTipoPartida = mDM.fcTipoPartida;
                        model.fcPartida = ObtenerCorrelativoPartida(1, mDM.fcTipoPartida);
                        model.fcLote = mDM.fcLote;
                        model.fcReferencia = mDM.fcReferencia;
                        model.fcEstadoPartida = mDM.fcEstadoPartida;
                        model.fiMoneda = fiMoneda;
                        model.Moneda = "";
                        model.SimboloMoneda = fiMoneda == 2 ? "$" : "L";
                        model.fcAutoriza = mDM.fcAutoriza;
                        model.fdFechaAutorizacion = mDM.fdFechaAutorizacion;
                        model.fcOrigen = mDM.fcOrigen;
                        model.fdFechaMigracion = mDM.fdFechaMigracion;
                        model.fcPartidaOrigen = mDM.fcPartidaOrigen;
                        model.fdFechaOrigen = mDM.fdFechaOrigen;
                        model.fcPartidaOrigen = mDM.fcPartidaOrigen;
                        model.fdFechaOrigen = mDM.fdFechaOrigen;
                        model.fcCreador = mDM.fcCreador;
                        model.fcSinopsis = mDM.fcSinopsis;
                        model.fnValorPartida = mDM.fnValorPartida ?? 0;
                        model.ListaDetalle = new List<CrearDetallePartidaViewModel>();
                        model.fiIDMovimientoMaestro = fiIDInventarioMovimientoMaestro;
                        model.fbTrasladoPrestadito = operacion.fbTrasladoPrestadito;

                        var i = 1;
                        foreach (var item in lDD)
                        {
                            var modelItem = new CrearDetallePartidaViewModel();
                            modelItem.fiIDEmpresa = item.fiIDEmpresa;
                            modelItem.fcPartida = item.fcPartida;
                            modelItem.fdFechaPartida = item.fdFechaPartida;
                            modelItem.fcCuentaContable = item.fcCuentaContable;
                            modelItem.TieneAuxiliar = false;
                            modelItem.fcAuxiliar = item.fcAuxiliar;
                            modelItem.fcAuxiliarCodigo = item.fcAuxiliarCodigo;
                            modelItem.fnDebito = item.fnDebito ?? 0;
                            modelItem.fnCredito = item.fnCredito ?? 0;
                            modelItem.fcDebitooCredito = item.fcDebitooCredito;
                            modelItem.fnTasadeCambio = item.fnTasadeCambio;
                            modelItem.fcEstado = item.fcEstado;
                            modelItem.fcTipoOperacion = "";
                            modelItem.fcFondo = item.fcFondo;
                            modelItem.fcCentrodeCosto = item.fcCentrodeCosto;
                            modelItem.fcPrograma = item.fcPrograma;
                            modelItem.fcDocumento = item.fcDocumento;
                            modelItem.fcOrigen = item.fcOrigen;
                            modelItem.fdFechaMigracion = item.fdFechaMigracion;
                            modelItem.fdFechaCreacionPartida = item.fdFechaCreacionPartida;
                            modelItem.fcCuenta = item.fcCuenta;
                            modelItem.fcPartidaAnterior = item.fcPartidaAnterior;
                            modelItem.fiPresu_VistoBueno = item.fiPresu_VistoBueno;
                            modelItem.fiConvertida = item.fiConvertida;
                            modelItem.fiIDFila = i;
                            modelItem.fcCierreSAF = item.fcCierreSAF;
                            modelItem.fcEmpresaSAF = item.fcEmpresaSAF;
                            modelItem.fdUltimaActualizacion = DateTime.Now;
                            modelItem.fcToken = item.fcToken;
                            modelItem.fcAutoGUID = item.fcAutoGUID;
                            modelItem.fcReferenciaDetalle = item.fcReferencia;
                            modelItem.fcSAFMigradoPor = item.fcSAFMigradoPor;
                            modelItem.fcSAFOperador = item.fcSAFOperador;
                            modelItem.fiIDUsuario = item.fiIDUsuario;
                            modelItem.fiPartidaMayorizada = item.fiPartidaMayorizada;
                            modelItem.fdFechadeMayorizacion = item.fdFechadeMayorizacion;
                            modelItem.fiIDUsuarioMayorizacion = item.fiIDUsuarioMayorizacion;
                            modelItem.IDGUID = item.IDGUID;
                            modelItem.EsEditar = false;
                            modelItem.TotalPartida = mDM.fnValorPartida ?? 0;
                            modelItem.TotalDebito = lDD.Sum(x => x.fnDebito ?? 0);
                            modelItem.TotalCredito = lDD.Sum(x => x.fnDebito ?? 0);
                            modelItem.IdMoneda = fiMoneda;
                            modelItem.EsDolar = fiMoneda == 2;
                            modelItem.DescripcionCuenta = item.fcCuentaContable + " - " + GetCuentaContable(item.fcCuentaContable, fiMoneda).fcDescripcionCuenta;
                            modelItem.FilaBloqueada = 0;
                            modelItem.EsCuentaDeBanco = false;
                            modelItem.RevisionRequerida = false;
                            modelItem.Exento = false;
                            modelItem.EsCuentaPorCobrar = false;
                            modelItem.IdRegistroAuxiliar = 0;
                            modelItem.NombreCliente = "NOVANET";
                            modelItem.fcIDLoan = "";
                            modelItem.fcTipoMovimiento = "";
                            modelItem.fiIdTipoMovimiento = 0;
                            modelItem.SelectCuentasContables = new List<CuentasContablesParaSelect>();
                            modelItem.fiCantidad = 0;
                            modelItem.fnPrecioUnitario = 0;
                            i++;
                            model.ListaDetalle.Add(modelItem);
                        }


                        conection.Close();
                    }
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);

        }


        private SqlDbType GetSqlDbType(string dataType)
        {
            if (string.IsNullOrEmpty(dataType))
            {
                throw new ArgumentException("El tipo de dato no puede ser nulo o vacío.", nameof(dataType));
            }

            switch (dataType.ToLower())
            {
                case "int":
                    return SqlDbType.Int;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "varchar":
                    return SqlDbType.VarChar;
                case "datetime":
                    return SqlDbType.DateTime;
                case "bit":
                    return SqlDbType.Bit;
                case "decimal":
                    return SqlDbType.Decimal;
                default:
                    throw new Exception($"Tipo de dato no soportado: {dataType}");
            }
        }




        public async Task<JsonResult> CrearPartidaContable(CrearPartidaViewModel model)
        {
            if (model.ListaDetalle == null || !model.ListaDetalle.Any()) return EnviarResultado(false, "Registros de detalle partida no existentes.");
            if (model.fnValorPartida <= 0) return EnviarResultado(false, "Total de partida debe ser mayor a 0.");
            var partidaLempiras = "";
            var totalDebe = model.ListaDetalle.Sum(z => z.fnDebito);
            var totalHaber = model.ListaDetalle.Sum(z => z.fnCredito);

            if (totalDebe != totalHaber) return EnviarResultado(false, "Error al cuadrar detalles partidas.");

            var _permisoControlMes = Control_MesContable_PermisoCrearTransaccion(model.fdFechaPartida, 1);
            if (!_permisoControlMes.Resultado) return EnviarResultado(false, _permisoControlMes.Mensaje);
            
            using (var conexion = new CoreAdministrativoEntities())
            {
                using(var contexto = new ORIONDBEntities())
                {
                    try
                    {
                        model.fcCreador = User.Identity.Name;
                        model.fiIDUsuarioCreador = 347;
                        model.fiIDEmpresa = 1;
                        model.fcEstadoPartida = "A";
                        model.fnValorPartida = totalDebe;

                        var PartidaCreada = "";

                        var result = conexion.sp_PartidaDiario_CrearMaestro(
                         (short)113,
                         model.fiIDUsuarioCreador,
                         model.fiIDEmpresa,
                         model.fdFechaPartida,
                         model.fcLote ?? "",
                         model.fcReferencia ?? "",
                         model.fcSinopsis,
                         model.fcTipoPartida,
                         model.fcEstadoPartida,
                         (short)model.fiMoneda,
                         model.fcCreador,
                         DateTime.Now,
                         model.fcOrigen ?? "",
                         model.fnValorPartida,
                         model.fcPartidaSeguimiento ?? ""
                         );
                        PartidaCreada = result.FirstOrDefault();

                        var countRow = 1;

                        foreach (var row in model.ListaDetalle)
                        {
                            var resultDetail = conexion.sp_PartidaDiario_CrearDetalle(
                            (short)113,
                            model.fiIDUsuarioCreador,
                            model.fiIDEmpresa,
                            PartidaCreada,
                            model.fdFechaPartida,
                            countRow,
                            row.fcCuentaContable ?? "",
                            row.fcAuxiliar ?? "",
                            row.fcAuxiliarCodigo ?? "",
                            row.fnDebito,
                            row.fnCredito,
                            row.fnDebito > 0 ? "D" : "C",
                            row.fnTasadeCambio,
                            model.fcEstadoPartida,
                            model.fcTipoPartida,
                            row.fcFondo ?? "",
                            row.fcCentrodeCosto ?? "",
                            row.fcPrograma ?? "",
                            row.fcDocumento ?? "",
                            row.fcOrigen ?? "",
                            row.fcReferenciaDetalle ?? "",
                            row.Exento);
                            countRow++;
                        }

                        partidaLempiras = PartidaCreada;
                        if (model.fiMoneda == 2)
                        {
                            
                            var resultado = conexion.sp_PartidaDiario_ConvertirdeDolaresaLempiras(model.fiIDEmpresa, PartidaCreada, 1);
                            partidaLempiras = PartidaCreada + "L";
                        }

                        if (model.fbTrasladoPrestadito)
                        {
                            var resultado = contexto.sp_PartidaTrasladoAPrestadito_Crear(partidaLempiras);
                        }

                        
                        var result2 = contexto.sp_ActualizarPartidaInventario(model.fiIDMovimientoMaestro, PartidaCreada, model.fiIDOperacionesContablesInventario);
                        
                        

                        try {
                            if(model.fiIDOperacionesContablesInventario != 5)
                            {
                                var email = new EmailTemplateService();
                                var resultadoCorreo = await email.SendEmailToSalidaInventarioPartida(new EmailTemplateServiceModel { IdTransaccion = model.fiIDMovimientoMaestro });
                            }
                            
                        }
                        catch(Exception ex)
                        {

                        }

                        
              

                        //conexion.sp_CorePartidas_Nuevo(model.fiIDEmpresa, PartidaCreada);
                        return EnviarResultado(!string.IsNullOrEmpty(PartidaCreada), string.IsNullOrEmpty(PartidaCreada) ? "Error al crear partida" : "Partida creada satisfactoriamente", "");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Exception Error en crear partida");
                    }
                }
                

            }
        }

        [HttpPost]
        public JsonResult GetRowTrasladoPrestadito(CrearDetallePartidaViewModel detalle,int fiMoneda)
        {
            var inventarioNovanetME = GetCuentaContable("6921-02-01-01", 2);
            var trasladoDeFondos = GetCuentaContable("1101-02-01-04", 1);


            var cuentaContableDolar = new Catalogo_Contable();
            var cuentaContableLempira = new Catalogo_Contable();
            var tasaDeCambio = MemoryLoadManager.ObtenerTasaCambio();
            detalle.fcDebitooCredito = detalle.fcDebitooCredito == "C" ? "D" : "C";
            if (fiMoneda == 2)
            {
                cuentaContableDolar = GetCuentaContable(detalle.fcCuentaContable, fiMoneda);
                detalle.fnDebito = detalle.fnDebito * tasaDeCambio;
                detalle.fnCredito = detalle.fnCredito * tasaDeCambio;
                cuentaContableLempira = GetCuentaContable(cuentaContableDolar.fcMonedaEquivalenter, 1);
            }
            else
            {
                cuentaContableLempira = GetCuentaContable(detalle.fcCuentaContable, 1);
            }

            var credito = detalle.fnCredito;
            var debito = detalle.fnDebito;

            detalle.fnDebito = credito;
            detalle.fnCredito = debito;

            if (detalle.fcCuentaContable == "6921-02-01-01" || detalle.fcCuentaContable == "1121-02-01-01")
            {
                detalle.fcCuentaContable = "1101-02-01-04";
                detalle.DescripcionCuenta = "1101-02-01-04 - " + trasladoDeFondos.fcDescripcionCuenta;
                return Json(detalle, JsonRequestBehavior.AllowGet);
            }


            detalle.fcCuentaContable = cuentaContableLempira.fcCuentaContable;
            detalle.DescripcionCuenta = cuentaContableLempira.fcCuentaContable + " - " + cuentaContableLempira.fcDescripcionCuenta;
            
            return Json(detalle, JsonRequestBehavior.AllowGet);
        }

    }
}