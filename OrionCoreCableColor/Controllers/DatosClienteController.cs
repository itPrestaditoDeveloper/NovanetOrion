using OrionCoreCableColor.Models.DatosCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrionCoreCableColor.DbConnection.OrionDB;
using System.Configuration;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.Models.Reportes;
using System.IO;
using System.Text.RegularExpressions;
using OrionCoreCableColor.Models.Solicitudes;
using System.Data.Entity.Infrastructure;
using OrionCoreCableColor.Models.Base;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using OrionCoreCableColor.App_Services.EmailService;
using System.Net.Http;
using System.Reflection;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace OrionCoreCableColor.Controllers
{
    [AllowAnonymous]
    public class DatosClienteController : Controller
    {

        private readonly DbServiceConnection _connection;
        public NotificacionesHub notificacion = new NotificacionesHub();
        public DatosClienteController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString; // DataCrypt.Desencriptar(ConfigurationManager.ConnectionStrings["ConexionEncriptada"].ConnectionString);
            _connection = new DbServiceConnection(ConnectionString);
            notificacion = new NotificacionesHub();

        }




        #region MIGRADOS DE BASECONTROLLER

        public string GetPropiedades(Object o)
        {
            Type t = o.GetType();
            PropertyInfo[] pis = t.GetProperties();
            string[] valores = pis.Select(p => p.Name + " : " + p.GetValue(o)).ToArray();
            string delimitados = string.Join(",", valores);
            return delimitados;
        }

        public T GetConfiguracion<T>(string llave)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return (T)Convert.ChangeType(contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == llave).ValorLLave, typeof(T));
            }
        }
        public List<int> ListaDias()
        {
            var lista = new List<int>();
            for (int i = 1; i <= 31; i++)
            {
                lista.Add(i);
            }
            return lista;
        }

        public List<int> ListaMeses()
        {
            var lista = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                lista.Add(i);
            }
            return lista;
        }
        public List<int> ListaAnio()
        {
            int anioActual = DateTime.Now.Year;
            int aniolimite = anioActual - 40;
            var lista = new List<int>();
            for (int i = anioActual; i >= aniolimite; i--)
            {
                lista.Add(i);
            }
            return lista;
        }


        public JsonResult EnviarListaJson(object e)
        {
            var jsonResult = Json(e, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }

        public int GetIdUser()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Usuarios_Maestros_ObtenerIdUsuario(User?.Identity?.Name ?? "").FirstOrDefault() ?? 20;
            }

        }


        public void EnviarNotificacion(string mensajeNotificacion)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.enviarNotificacion(mensajeNotificacion);
            hubContext.Clients.All.recibirNotificacionLog($"Usuario: sistembot, Operacion: {mensajeNotificacion}");
            hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
            {
                fiIDUsuario = GetIdUser(),
                fdFechaTransaccion = DateTime.Now,
                fcUsuario = "sistembot",
                fcOperacion = mensajeNotificacion,
                fcTipoTransaccion = "CLIENTE",
                fcClase = "primary"
            });
        }

        public JsonResult EnviarResultado(bool resultado, string Titulo)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = resultado ? "Action Successful" : "Error",
                Estado = resultado

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje, int Id)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
                Id = Id
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje, string Correlativo)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
                Correlativo = Correlativo
            }, JsonRequestBehavior.AllowGet);

        }


        public void RegistrarError(Exception ex)
        {
            using (var log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry((ex.InnerException?.Message ?? ex.Message) + ": " + ex.StackTrace, EventLogEntryType.Error, 101, 1);

                var correo = new SendEmailService();
                correo.SendEmailExceptionWithOutAsync(ex, "Error de sistema");
            }

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
            {
                fiIDUsuario = GetIdUser(),
                fdFechaTransaccion = DateTime.Now,
                fcUsuario = User?.Identity?.Name ?? "sistembot",
                fcOperacion = (ex.InnerException?.Message ?? ex.Message) + ": " + ex.StackTrace,
                fcClase = "danger",
                fcTipoTransaccion = "ERROR"
            });
        }


        public void eliminar(int idequifax)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.eliminarrow(idequifax);
        }

        public void agregar(int idequifax)
        {
            //var model = contexto.sp_Solicitudes_Bandeja_ObtenerPorIDSolicitud(fiIDSolicitud).FirstOrDefault();
            var model = _connection.OrionContext.sp_ReporteClientes_ObtenerPorID(idequifax).FirstOrDefault();


            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.agregarrow(model);

        }



        public JsonResult EnviarException(Exception e, string Titulo)
        {

            if (MemoryLoadManager.Produccion)
            {
                RegistrarError(e);
            }


            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace,
                Estado = false,

            }, JsonRequestBehavior.AllowGet);

        }


        public string[] PreguntasATomar()
        {
            Random rand = new Random();
            List<int> preguntasAgregada = new List<int>();
            int numeroPregunta = rand.Next(1, 7);
            preguntasAgregada.Add(numeroPregunta);
            int numeroPregunta2 = rand.Next(1, 7);
            while (preguntasAgregada.Contains(numeroPregunta2))
            {
                numeroPregunta2 = rand.Next(1, 7);
            }
            preguntasAgregada.Add(numeroPregunta2);

            int numeroPregunta3 = rand.Next(1, 7);
            preguntasAgregada.Add(numeroPregunta3);
            while (preguntasAgregada.Contains(numeroPregunta3))
            {
                numeroPregunta3 = rand.Next(1, 7);
            }


            //while (numeroPregunta == numeroPregunta2)
            //{
            //    numeroPregunta2 = rand.Next(1, 6);
            //}
            //while (numeroPregunta2 == numeroPregunta3)
            //{
            //    numeroPregunta3 = rand.Next(1, 6);
            //}
            //while (numeroPregunta == numeroPregunta3)
            //{
            //    numeroPregunta3= rand.Next(1, 6);
            //}
            //while (numeroPregunta2 == numeroPregunta)
            //{
            //    numeroPregunta = rand.Next(1, 6);
            //}
            var ListadoSeleccionadoPreguntas = new List<string>() { "Vive", "Trabaja", "Parentesco", "Estado Civil", "Ciudad", "Tiene Hijos" }.ToArray();
            string pregunta1Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta - 1];
            string pregunta2Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta2 - 1];
            string pregunta3Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta3 - 1];

            return new List<string>() { pregunta1Seleccionada, pregunta2Seleccionada, pregunta3Seleccionada }.ToArray();



        }

        public List<string> PreguntasAleatorias(List<string> lista, string opcionCorrecta, int tipoLista)
        {
            Random rand = new Random();
            //preguntas


            //si es 1 hacer randon de respuestas
            if (tipoLista == 1)
            {
                lista = lista.Where(x => x != opcionCorrecta).Select(x => x).ToList();
                var listaArreglo = lista.ToArray();
                int cantidaddeElementos = lista.Count;
                int numeroAlzar = rand.Next(1, cantidaddeElementos);
                int numeroAlzar2 = rand.Next(1, cantidaddeElementos);
                string opcion1AlAzar = listaArreglo[numeroAlzar];
                string opcion2AlAzar = listaArreglo[numeroAlzar2];
                while (numeroAlzar2 == numeroAlzar)
                {
                    numeroAlzar2 = rand.Next(1, cantidaddeElementos);
                    opcion2AlAzar = listaArreglo[numeroAlzar2];
                }

                var ListaRetornar = new List<string> { opcion1AlAzar, opcion2AlAzar, opcionCorrecta };

                ListaRetornar = ListaRetornar.OrderBy(x => rand.Next()).ToList();
                return ListaRetornar;
            }
            // sino solo retornar lista
            else
            {
                return lista;
            }
        }

        public static HttpPostedFileBase ConvertirBase64AImagen(string base64String, string nombreArchivo)
        {
            // Convertir la cadena Base64 en un array de bytes
            //byte[] bytes = Convert.FromBase64String(CleanBase64String(base64String));

            HttpPostedFileBase archivo = new ByteArrayHttpPostedFile(base64String, nombreArchivo);
            //GuardarImagen(bytes, archivo);
            return archivo;

        }

        public bool UploadFileServer148(string carpeta, HttpPostedFileBase file)
        {

            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    try
                    {
                        content.Add(new StreamContent(file.InputStream), "documento", file.FileName);
                        var requestUri = $@"{MemoryLoadManager.Helper}?type=guardar&carpeta={carpeta}";
                        var result = client.PostAsync(requestUri, content).Result;

                        return true;
                    }
                    catch (Exception ex)
                    {

                        return false;
                    }
                }

            }


        }

        #endregion

        //// GET: DatosCliente
        public ActionResult Index()
        {
            return View();
        }
        public List<estadoCivilViewModel> ListaEstadosCiviles()
        {

            var sel = _connection.OrionContext.sp_Catalogo_EstadoCivil().Select(a => new estadoCivilViewModel {
                id = a.fiIDEstadoCivil ?? 0,
                descripcion = a.fcDescripcionEstadoCivil
            }).ToList();

            return sel;
        }
        public List<sp_geo_ListaDepartamentos_Result> ListaDepartamentos()
        {
            var lista = _connection.OrionContext.sp_geo_ListaDepartamentos().ToList();

            return lista;
        }
        public List<sp_Catalogo_RangoSalarial_Select_Result> ListaRangosSalariales()
        {
            var lista = _connection.OrionContext.sp_Catalogo_RangoSalarial_Select().ToList();

            return lista;
        }
        public List<sp_Catalogo_AntiguedadLaboral_Select_Result> ListaAntiguedadLaboral()
        {
            var lista = _connection.OrionContext.sp_Catalogo_AntiguedadLaboral_Select().ToList();

            return lista;
        }
        public List<sp_TipoVivienda_Lista_Result> ListaViviendas()
        {
            //var casado = new tipoViviendaViewModel { id = 1, descripcion = "Casado" };
            //var soltero = new tipoViviendaViewModel { id = 2, descripcion = "Soltero" };
            return _connection.OrionContext.sp_TipoVivienda_Lista().ToList();
        }
        public List<tieneHijosViewModel> ListaEstadosTieneHijos()
        {
            var casado = new tieneHijosViewModel { id = 1, descripcion = "Si" };
            var soltero = new tieneHijosViewModel { id = 2, descripcion = "No" };
            return new List<tieneHijosViewModel> { casado, soltero };
        }
        public ActionResult ViewFormDatosCliente()
        {
            datosclienteViewModel model = new datosclienteViewModel();
            return View(model);
        }

        public ActionResult PartialViewFormularioCliente(int idEquifax, int tipoSolicitud, int estadoForm, string identidad)
        {
            try
            {
                var listaParentescos = _connection.OrionContext.sp_Catalogo_Parentescos_List().ToList();

                if (tipoSolicitud == 1)
                {
                    listaParentescos = listaParentescos.Where(a => a.fiIDParentesco != 6).ToList();
                }
                var listaCiudades = _connection.OrionContext.sp_ListadoCiudades().ToList();

                var listaDiasPagos = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "DiasPagos").ValorLLave.Split(',');


                ViewBag.ListaParentesco = listaParentescos.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDParentesco), Text = z.fcDescripcionParentesco }).ToList();
                ViewBag.ListaDias = ListaDias().Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z.ToString() }).ToList();
                ViewBag.ListaMeses = ListaMeses().Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z.ToString() }).ToList();
                ViewBag.ListaAnios = ListaAnio().Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z.ToString() }).ToList();
                ViewBag.ListaEstadosCiviles = ListaEstadosCiviles().Select(z => new SelectListItem { Value = Convert.ToString(z.id), Text = z.descripcion.ToString() }).ToList();

                ViewBag.ListaEstadosTieneHijos = ListaEstadosTieneHijos().Select(z => new SelectListItem { Value = Convert.ToString(z.id), Text = z.descripcion.ToString() }).ToList();
                ViewBag.ListaCiudades = listaCiudades.Select(z => new SelectListItem { Value = Convert.ToString(z.fcCiudad), Text = z.fcCiudad.ToString() }).ToList();
                ViewBag.ListaViviendas = ListaViviendas().Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDTipoVivienda), Text = z.fcDescripcionTipoVivienda.ToString() }).ToList();
                ViewBag.ListaDepartamentos = ListaDepartamentos().Select(z => new SelectListItem { Value = Convert.ToString(z.fiCodDepartamento), Text = z.fcDepartamento.ToString() }).ToList();
                ViewBag.ListaDiasPagos = listaDiasPagos.Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z.ToString() }).ToList();
                ViewBag.ListaRangosSalariales = ListaRangosSalariales().Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDCatalogo_RangosSalariales), Text = z.fcDescripcion_RangosSalariales.ToString() }).ToList();
                ViewBag.ListaAntiguedadLaboral = ListaAntiguedadLaboral().Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDAntiguedadLaboral), Text = z.fcDescripcionAntiguedad.ToString() }).ToList();
                datosclienteViewModel modelSend = new datosclienteViewModel() { fiIDEquifax = idEquifax, fiTipoSolicitud = tipoSolicitud, fcIdentidad = identidad };
                return PartialView(modelSend);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public ActionResult ViewFormReferenciasCliente()
        {

            referenciaclienteViewModel model = new referenciaclienteViewModel()
            {

                fcPregunta1 = "Vive en el siguiente barrio o colonia:",
                fcPregunta2 = "Trabaja:",
                fcPregunta3 = "Mi parentesco es:",
            };
            return View(model);
        }
        public ActionResult ViewFormMapa()
        {

            return View();
        }
        public ActionResult ViewFormQR()
        {

            return View();
        }
        public ActionResult ViewFormAdendun()
        {

            return View();
        }
        public ActionResult ViewFormClienteAgendar()
        {
            return View();
        }
        public ActionResult ViewFormCargarIdentidad()
        {
            return View();
        }




        public JsonResult ValidarDocumentos(int idSolicitud)
        {
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                ViewBag.IdSolicitud = idSolicitud;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {idSolicitud}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();

                    DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();
                }
                var lista = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "IdSelfi_y_Cedula").ValorLLave.Split(',').Select(a => Convert.ToInt32(a)).ToList();

                connection.Close();
                return EnviarListaJson(DatosDocumentoListado.Any(a => !lista.Any(b => b == a.fiIDTipoArchivo))); // siempre traera true cuando encuentre por lo menos a uno, asi que este comentario es para solucionarlo mas tarde 
            }
        }

        [HttpPost]
        public ActionResult guardarUbicacion(int idEquifax, string latitud, string longitud)
        {
            try
            {
                var guardar = _connection.OrionContext.sp_Equifax_Cliente_ActualizarUbicacion(latitud + "," + longitud, idEquifax);
                var model = _connection.OrionContext.sp_InfoClientePorIdEquifax(idEquifax).FirstOrDefault();
                EnviarNotificacion($"Se guardo la ubicacion del cliente {model.fcNombre} con equifax {model.fiIDEquifax}");
                return EnviarResultado(true, "Ubicación Enviada Correctamente");

            }
            catch (Exception e)
            {

                return EnviarResultado(false, "Favor llene todo el formulario");
            }
        }

        [HttpPost]
        public ActionResult CrearDatosClienteParaPaqueteCompleto(datosclienteViewModel model)
        {
            try
            {

                var ObtenerNombreTelefono = _connection.OrionContext.sp_cliente_NombreTelefono(model.fiIDEquifax).FirstOrDefault();
                var cliente = _connection.OrionContext.sp_InfoClientePorIdEquifax(model.fiIDEquifax).FirstOrDefault();
                int contadorReferenciasFamiliares = 0;
                var listaReferenciasTipoFamiliar = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "ReferenciasTipoFamiliar").ValorLLave.Split(',').Select(a => Convert.ToInt32(a)).ToList();

                var fechaminimacontratar = GetConfiguracion<DateTime>("FechaNacimientoMinimaParaContratar");

                if (model.fdFechaNacimiento < fechaminimacontratar)
                {
                    return EnviarResultado(false, "Ingrese una Fecha de nacimiento Valido");
                }
                if (model.fcTelefonoReferencia1 == ObtenerNombreTelefono.fcTelefono || model.fcTelefonoReferencia2 == ObtenerNombreTelefono.fcTelefono)
                {
                    //return EnviarResultado(false, "No puedes usar tu numero personal para las referencias");
                    return Json(new { Estado = false, Mensaje = "No puedes usar tu numero personal para las referencias", Titulo = "Error" });
                }
                if (model.fcNombreReferencia1 == ObtenerNombreTelefono.fcNombre || model.fcNombreReferencia2 == ObtenerNombreTelefono.fcNombre)
                {
                    return Json(new { Estado = false, Mensaje = "No puedes usar tu nombre para las referencias", Titulo = "Error" });
                }
                //if ($"{model.fcPrimerNombre} {model.fcSegundoNombre} {model.fcPrimerApellido} {model.fcSegundoApellido}" != ObtenerNombreTelefono.fcNombre)
                if (model.fcNombreCompleto.ToUpper().Trim() != ObtenerNombreTelefono.fcNombre.ToUpper().Trim())
                {

                    //return Json(new { Estado = false, Mensaje = "Estas escribiendo mal tu nombre, favor corrigelo", Titulo = "Error" });
                }
                if (model.fcTelefonoReferencia1 == model.fcTelefonoReferencia2)
                {
                    return Json(new { Estado = false, Mensaje = "No es valido, el mismo telefono para varias referencias", Titulo = "Error" });
                }
                if (listaReferenciasTipoFamiliar.Contains(model.fcParentescoReferencia1))
                {
                    contadorReferenciasFamiliares++;
                }
                if (listaReferenciasTipoFamiliar.Contains(model.fcParentescoReferencia2))
                {
                    contadorReferenciasFamiliares++;
                }

                if (!model.fcCorreo.Contains("@"))
                {
                    return EnviarResultado(false, "Ingresa un correo valido");
                }
                if (model.fcCiudad == "" || model.fcParentescoReferencia1 == 0 || model.fcParentescoReferencia2 == 0 || model.fiEstadoCivil == 0 || model.tieneHijosHelper == 0)
                {
                    return EnviarResultado(false, "Favor llene todo el formulario");
                }

                //model.fdFechaNacimiento = new DateTime(model.AnioNacimento, model.MesNacimiento, model.diaNacimiento);
                model.fcTelefonoReferencia1 = model.fcTelefonoReferencia1.Replace("-", "");
                model.fcTelefonoReferencia2 = model.fcTelefonoReferencia2.Replace("-", "");
                //model.fcTelefonoReferencia3 = model.fcTelefonoReferencia3.Replace("-", "");
                if (model.fcTelefonoReferencia1.Length < 8 || model.fcTelefonoReferencia2.Length < 8)
                {
                    return Json(new { Estado = false, Mensaje = "Numero de telefono invalido, revise los datos", Titulo = "Error" });
                }

                //model.fcPrimerNombre = model.fcPrimerNombre.Replace(" ", string.Empty);
                //model.fcSegundoNombre = model.fcSegundoNombre.Replace(" ", string.Empty);
                //model.fcPrimerApellido = model.fcPrimerApellido.Replace(" ", string.Empty);
                //model.fcSegundoApellido = model.fcSegundoApellido.Replace(" ", string.Empty);

                ///////

                //if (listaReferenciasTipoFamiliar.Contains(model.fcParentescoReferencia3))
                //{
                //    contadorReferenciasFamiliares++;
                //}

                if (contadorReferenciasFamiliares < 2)
                {
                    return Json(new { Estado = false, Mensaje = "Las  2 referencias deben ser familiares", Titulo = "Error" });
                }

                var telefonosecundario = (model.fcTelefonoSecundario is null) ? "0" : model.fcTelefonoSecundario;
                // var InsertarTablaIntermedia = CrearSolicitudCliente_Equifax(model.fiIDEquifax);
                var InsertarinDBCliente = _connection.OrionContext.sp_Equifax_Cliente_Actualizar(model.fiIDEquifax, model.fcIdentidad, $"{model.fcNombreCompleto}", model.fdFechaNacimiento, model.fcLugarTrabajo, "", model.fiEstadoCivil, model.fbTieneHijos, model.fcCiudad, model.fcDireccionDetallada, model.fcCorreo, model.fcProfesion, model.fcTipoVivienda, model.fiDepartamento, model.fiMunicipio, 1, model.fiColonia, model.fcDiaPago, model.fiRangoSalarial, model.fiAntiguedadLaboral, telefonosecundario) > 0;
                var InsertarinDBReferencias = Convert.ToInt32(_connection.OrionContext.sp_Cliente_Referencia_Insertar(model.fiIDEquifax, model.fcNombreReferencia1, model.fcTelefonoReferencia1, model.fcParentescoReferencia1, 1, model.fcApellidoReferencia1).FirstOrDefault());
                var InsertarinDBReferencias2 = Convert.ToInt32(_connection.OrionContext.sp_Cliente_Referencia_Insertar(model.fiIDEquifax, model.fcNombreReferencia2, model.fcTelefonoReferencia2, model.fcParentescoReferencia2, 1, model.fcApellidoReferencia2).FirstOrDefault());
                // var InsertarinDBReferencias3 = Convert.ToInt32(_connection.OrionContext.sp_Cliente_Referencia_Insertar(model.fiIDEquifax, model.fcNombreReferencia3, model.fcTelefonoReferencia3, model.fcParentescoReferencia3, 1, model.fcApellidoReferencia3).FirstOrDefault());


                if (InsertarinDBCliente && InsertarinDBReferencias > 0 && InsertarinDBReferencias2 > 0)
                {
                    MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias, model.fcNombreReferencia1, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia1, "");
                    MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias2, model.fcNombreReferencia2, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia2, "");
                    //MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias3, model.fcNombreReferencia3, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia3, "");
                    EnviarNotificacion($"{model.fcNombreCompleto} ha llenado el formulario");
                    return Json(new { Estado = true, Mensaje = "Datos enviados, puede cerrar la pagina!", Titulo = "Gracias" });
                }

                return EnviarResultado(false, "Consulte con el administrador");

            }
            catch (Exception e)
            {
                RegistrarError(e);
                return EnviarResultado(false, "Favor llene todo el formulario");
            }
        }
        [HttpPost]
        public ActionResult CrearDatosClienteParaSoloInternet(datosclienteViewModel model)
        {
            try
            {

                var listaReferenciasTipoFamiliar = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "ReferenciasTipoFamiliar").ValorLLave.Split(',').Select(a => Convert.ToInt32(a)).ToList();


                var fechaminimacontratar = GetConfiguracion<DateTime>("FechaNacimientoMinimaParaContratar");

                if (model.fdFechaNacimiento < fechaminimacontratar)
                {
                    return EnviarResultado(false, "Ingrese una Fecha de nacimiento Valido");
                }
                if (!model.fcCorreo.Contains("@"))
                {
                    return EnviarResultado(false, "Ingresa un correo valido");
                }
                if (model.fcCiudad == "" || model.fcParentescoReferencia1 == 0 || model.tieneHijosHelper == 0)
                {
                    return EnviarResultado(false, "Favor llene todo el formulario");
                }

                //model.fdFechaNacimiento = new DateTime(model.AnioNacimento, model.MesNacimiento, model.diaNacimiento);
                model.fcTelefonoReferencia1 = model.fcTelefonoReferencia1.Replace("-", "");

                if (model.fcTelefonoReferencia1.Length < 8)
                {
                    return Json(new { Estado = false, Mensaje = "Numero de telefono invalido, revise los datos", Titulo = "Error" });
                }

                // model.fcPrimerNombre = model.fcPrimerNombre.Replace(" ", string.Empty);
                //model.fcSegundoNombre = model.fcSegundoNombre.Replace(" ", string.Empty);
                //model.fcPrimerApellido = model.fcPrimerApellido.Replace(" ", string.Empty);
                //model.fcSegundoApellido = model.fcSegundoApellido.Replace(" ", string.Empty);
                var ObtenerNombreTelefono = _connection.OrionContext.sp_cliente_NombreTelefono(model.fiIDEquifax).FirstOrDefault();

                if (model.fcTelefonoReferencia1 == ObtenerNombreTelefono.fcTelefono)
                {
                    //return EnviarResultado(false, "No puedes usar tu numero personal para las referencias");
                    return Json(new { Estado = false, Mensaje = "No puedes usar tu numero personal para las referencias", Titulo = "Error" });
                }
                if (model.fcNombreReferencia1 == ObtenerNombreTelefono.fcNombre)
                {
                    return Json(new { Estado = false, Mensaje = "No puedes usar tu nombre para las referencias", Titulo = "Error" });
                }
                if (model.fcNombreCompleto.ToUpper().Trim() != ObtenerNombreTelefono.fcNombre.ToUpper().Trim())
                {
                    return Json(new { Estado = false, Mensaje = $"{model.fcNombreCompleto} Esta Mal Escrito, favor corrigelo", Titulo = "Error" });
                }

                var telefonosecundario = (model.fcTelefonoSecundario is null) ? "0" : model.fcTelefonoSecundario;
                //var InsertarTablaIntermedia = CrearSolicitudCliente_Equifax(model.fiIDEquifax);
                var InsertarinDBCliente = _connection.OrionContext.sp_Equifax_Cliente_Actualizar(model.fiIDEquifax, model.fcIdentidad, $"{model.fcNombreCompleto}", model.fdFechaNacimiento, model.fcLugarTrabajo, "", model.fiEstadoCivil, model.fbTieneHijos, model.fcCiudad, model.fcDireccionDetallada, model.fcCorreo, model.fcProfesion, model.fcTipoVivienda, model.fiDepartamento, model.fiMunicipio, 1, model.fiColonia, model.fcDiaPago, model.fiRangoSalarial, model.fiAntiguedadLaboral, telefonosecundario) > 0;

                var InsertarinDBReferencias = Convert.ToInt32(_connection.OrionContext.sp_Cliente_Referencia_Insertar(model.fiIDEquifax, model.fcNombreReferencia1, model.fcTelefonoReferencia1, model.fcParentescoReferencia1, 1, model.fcApellidoReferencia1).FirstOrDefault());



                if (InsertarinDBCliente && InsertarinDBReferencias > 0)
                {
                    MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias, model.fcNombreReferencia1, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia1, "");
                    // MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias2, model.fcNombreReferencia2, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia2, "");
                    //MensajeDeTexto.EnviarNumeroReferencias(InsertarinDBReferencias3, model.fcNombreReferencia3, model.fcPrimerNombre + " " + model.fcSegundoNombre + " " + model.fcPrimerApellido + " " + model.fcSegundoApellido, model.fiIDEquifax, model.fcTelefonoReferencia3, "");
                    eliminar(model.fiIDEquifax);
                    agregar(model.fiIDEquifax);
                    EnviarNotificacion($"{ model.fcNombreCompleto }  ha llenado el formulario");
                    return Json(new { Estado = true, Mensaje = "Datos enviados, puede cerrar la pagina!", Titulo = "Gracias" });
                }

                return EnviarResultado(false, "Consulte con el administrador");

            }
            catch (Exception e)
            {
                RegistrarError(e);
                return EnviarResultado(false, "Favor llene todo el formulario");
            }
        }

        private int CrearSolicitudCliente_Equifax(int fiIDEquifax)
        {

            try
            {
                var crear = Convert.ToInt32(_connection.OrionContext.sp_OrionSolicitud_EquifaxCliente_Crear(fiIDEquifax));
                return crear;
            }
            catch (Exception e)
            {
                return 0;

            }

        }

        public JsonResult ObtenerIdentidad(int fiIDEquifax)
        {
            try
            {
                //_connection.OrionContext.Database.Connection.Close();
                //var fcIdentidad = _connection.OrionContext.sp_Cliente_ObtenerIdentidad(fiIDEquifax);

                //_connection.OrionContext.Database.Connection.Close();
                //var existe = _connection.OrionContext.sp_Cliente_Referencia_ValidacionFormCliente(fiIDEquifax);
                //int algo = existe.Count();
                //if (existe.Count() > 0)
                //{

                //    return EnviarListaJson(new { fcIdentidad = "NO" });
                //}
                return EnviarListaJson(_connection.OrionContext.sp_Cliente_ObtenerIdentidad(fiIDEquifax).FirstOrDefault());
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public JsonResult ObtenerUbicacion(int fiIDEquifax)
        {
            try
            {
                //_connection.OrionContext.Database.Connection.Close();
                //var fcIdentidad = _connection.OrionContext.sp_Cliente_ObtenerIdentidad(fiIDEquifax);

                //_connection.OrionContext.Database.Connection.Close();
                //var existe = _connection.OrionContext.sp_Cliente_Referencia_ValidacionFormCliente(fiIDEquifax);
                //int algo = existe.Count();
                //if (existe.Count() > 0)
                //{

                //    return EnviarListaJson(new { fcIdentidad = "NO" });
                //}
                var idsolicitud = _connection.OrionContext.sp_Solicitud_Maestro_Validar_Ubicacion(fiIDEquifax).FirstOrDefault() ?? 0; // aun que diga IdEquifax se esta validando el id de la solicitud
                //var id = _connection.OrionContext.sp_EquifaxCliente_ValidarUbicacion(fiIDEquifax).FirstOrDefault() ?? 0;
                return EnviarListaJson(idsolicitud);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public JsonResult ObtenerNombre(int fiIDReferencia)
        {
            try
            {
                _connection.OrionContext.Database.Connection.Close();
                var llenoForm = _connection.OrionContext.sp_Cliente_Referencia_ValidacionFormulario(fiIDReferencia);
                if (Convert.ToInt32(llenoForm.FirstOrDefault().Value) != 0)
                {
                    return EnviarListaJson(new
                    {
                        resultadollenoForm = true
                    });
                }
                _connection.OrionContext.Database.Connection.Close();
                var Idcliente = _connection.OrionContext.sp_Cliente_ObtenerIdCliente(fiIDReferencia).FirstOrDefault();
                _connection.OrionContext.Database.Connection.Close();
                var nombre = _connection.OrionContext.sp_Cliente_ObtenerNombre(Idcliente.fiIDEquifax);

                //_connection.OrionContext.Database.Connection.Close();
                //var listaColoniasFalsas = _connection.OrionContext.sp_ColoniasFalsas_List().ToList();
                //_connection.OrionContext.Database.Connection.Close();
                //var listaTrabajosFalsos = _connection.OrionContext.sp_TrabajosFalsos_List().ToList();
                //_connection.OrionContext.Database.Connection.Close();
                //var respuestas = _connection.OrionContext.sp_cliente_obtenerDatos(fiIDReferencia).FirstOrDefault();
                return EnviarListaJson(new
                {
                    resultadollenoForm = false,
                    fcNombre = nombre,
                    fcReferencia = Idcliente.fcNombreCompletoReferencia,
                    idEquifax = Idcliente.fiIDEquifax
                });
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }


        }
        public JsonResult ObtenerRespuestas(int fiIDReferencia)
        {
            try
            {
                //var listaColoniasFalsas = _connection.OrionContext.sp_ColoniasFalsas_List().ToList();            
                //var listaTrabajosFalsos = _connection.OrionContext.sp_TrabajosFalsos_List().ToList();
                var respuestas = _connection.OrionContext.sp_cliente_obtenerDatos(fiIDReferencia).FirstOrDefault();
                return EnviarListaJson(new
                {

                    //listaColonias = listaColoniasFalsas,
                    //listaTrabajos = listaTrabajosFalsos,
                    respuestaCorrectaVive = respuestas.fcLugarVive,
                    respuestaCorrectaTrabajo = respuestas.fcLugarTrabajo,
                    respuestaCorrectaParentesco = respuestas.fiIDParentescoReferencia,
                    respuestaCorrectaEstadoCivil = respuestas.fiEstadoCivil,
                    respuestaCorrectaTieneHijos = respuestas.fbTieneHijos,
                    respuestaCorrectaCiudad = respuestas.fcCiudad

                });
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }


        }

        public JsonResult ObtenerNombreCliente(int fiIDEquifax)
        {
            try
            {
                int solicitud = fiIDEquifax;
                var DatosClientesDB = _connection.OrionContext.sp_ObtenerDatosCliente_ByIdSolicitud(solicitud).FirstOrDefault();
                return Json(DatosClientesDB);
            }
            catch (Exception e)
            {
                return EnviarException(e, e.Message);

            }
        }

        public JsonResult ListaMunicipios(int CodDepartamento)
        {
            try
            {
                return EnviarListaJson(_connection.OrionContext.sp_Geo_ListaMunicipio_ByIdDepartamento(CodDepartamento).ToList());
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }


        }

        public JsonResult ListaMunicipiosByDepartamento(int CodDepartamento)
        {
            try
            {
                return EnviarListaJson(_connection.OrionContext.sp_Geo_ListaMunicipio_ByIdDepartamento(CodDepartamento).ToList());
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }
        }
        public JsonResult ListaPobladosByMunicipio(int CodDepartamento, int CodMunicipio)
        {
            try
            {
                return EnviarListaJson(_connection.OrionContext.sp_Geo_ListaPoblado_ByIdMunicipio(CodDepartamento, CodMunicipio).ToList());
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }
        }
        public JsonResult ListaColoniasByPoblado(int CodDepartamento, int CodMunicipio)
        {
            try
            {
                return EnviarListaJson(_connection.OrionContext.sp_Geo_ListaBarrioColonia(CodDepartamento, CodMunicipio, 1).ToList());
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }
        }
        public ActionResult ViewPartialRespuestas(int fiIDEquifax, int fiIDReferencia, string lugar, string trabajo, int parentesco, int estadocivil, string ciudad, bool tieneHijos)
        {
            try
            {

                string respuesta1Correcta = "";
                string respuesta2Correcta = "";
                string respuesta3Correcta = "";
                string pregunta1 = "";
                string pregunta2 = "";
                string pregunta3 = "";
                //alterar preguntas donde vive
                var listaPreguntas1 = new List<string>();
                var listaPreguntas2 = new List<string>();
                var listaPreguntas3 = new List<string>();
                var listadopreguntasSeleccionadas = PreguntasATomar();
                //Obtener Parentesco por id 
                string parentescoCorrecto = _connection.OrionContext.sp_Catalogo_Parentescos_List().Where(x => x.fiIDParentesco == parentesco).Select(x => x.fcDescripcionParentesco).FirstOrDefault();
                string estadoCivilCorrecto = ListaEstadosCiviles().Where(x => x.id == estadocivil).Select(x => x.descripcion).FirstOrDefault();
                //seleccionar preguntas1
                if (listadopreguntasSeleccionadas[0] == "Vive")
                {
                    listaPreguntas1 = PreguntasAleatorias(_connection.OrionContext.sp_ColoniasFalsas_List().ToList(), lugar, 1);
                    pregunta1 = Preguntas.preguntaVive;
                    respuesta1Correcta = lugar;
                }
                if (listadopreguntasSeleccionadas[0] == "Trabaja")
                {
                    listaPreguntas1 = PreguntasAleatorias(_connection.OrionContext.sp_TrabajosFalsos_List().ToList(), trabajo, 1);
                    pregunta1 = Preguntas.preguntatrabajo;
                    respuesta1Correcta = trabajo;
                }
                if (listadopreguntasSeleccionadas[0] == "Parentesco")
                {
                    listaPreguntas1 = PreguntasAleatorias(_connection.OrionContext.sp_Catalogo_Parentescos_List().Select(x => x.fcDescripcionParentesco).ToList(), parentescoCorrecto.ToString(), 0);
                    pregunta1 = Preguntas.preguntaParentesco;
                    respuesta1Correcta = parentescoCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[0] == "Estado Civil")
                {
                    listaPreguntas1 = PreguntasAleatorias(ListaEstadosCiviles().Select(x => x.descripcion).ToList(), estadoCivilCorrecto.ToString(), 0);
                    pregunta1 = Preguntas.preguntaEstadoCivil;
                    respuesta1Correcta = estadoCivilCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[0] == "Ciudad")
                {
                    listaPreguntas1 = PreguntasAleatorias(_connection.OrionContext.sp_ListadoCiudades().Select(x => x.fcCiudad).ToList(), ciudad, 1);
                    pregunta1 = Preguntas.PreguntaCiudad;
                    respuesta1Correcta = ciudad;
                }
                if (listadopreguntasSeleccionadas[0] == "Tiene Hijos")
                {
                    listaPreguntas1 = PreguntasAleatorias(ListaEstadosTieneHijos().Select(x => x.descripcion).ToList(), tieneHijos == true ? "Si" : "No", 0);
                    pregunta1 = Preguntas.preguntaTieneHijos;
                    respuesta1Correcta = tieneHijos == true ? "Si" : "No";
                }
                ///fin preguntas1
                //seleccionar preguntas2
                if (listadopreguntasSeleccionadas[1] == "Vive")
                {
                    listaPreguntas2 = PreguntasAleatorias(_connection.OrionContext.sp_ColoniasFalsas_List().ToList(), lugar, 1);
                    pregunta2 = Preguntas.preguntaVive;
                    respuesta2Correcta = lugar;
                }
                if (listadopreguntasSeleccionadas[1] == "Trabaja")
                {
                    listaPreguntas2 = PreguntasAleatorias(_connection.OrionContext.sp_TrabajosFalsos_List().ToList(), trabajo, 1);
                    pregunta2 = Preguntas.preguntatrabajo;
                    respuesta2Correcta = trabajo;
                }

                if (listadopreguntasSeleccionadas[1] == "Parentesco")
                {
                    listaPreguntas2 = PreguntasAleatorias(_connection.OrionContext.sp_Catalogo_Parentescos_List().Select(x => x.fcDescripcionParentesco).ToList(), parentescoCorrecto.ToString(), 0);
                    pregunta2 = Preguntas.preguntaParentesco;
                    respuesta2Correcta = parentescoCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[1] == "Estado Civil")
                {
                    listaPreguntas2 = PreguntasAleatorias(ListaEstadosCiviles().Select(x => x.descripcion).ToList(), estadoCivilCorrecto.ToString(), 0);
                    pregunta2 = Preguntas.preguntaEstadoCivil;
                    respuesta2Correcta = estadoCivilCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[1] == "Ciudad")
                {
                    listaPreguntas2 = PreguntasAleatorias(_connection.OrionContext.sp_ListadoCiudades().Select(x => x.fcCiudad).ToList(), ciudad, 1);
                    pregunta2 = Preguntas.PreguntaCiudad;
                    respuesta2Correcta = ciudad;
                }
                if (listadopreguntasSeleccionadas[1] == "Tiene Hijos")
                {
                    listaPreguntas2 = PreguntasAleatorias(ListaEstadosTieneHijos().Select(x => x.descripcion).ToList(), tieneHijos == true ? "Si" : "No", 0);
                    pregunta2 = Preguntas.preguntaTieneHijos;
                    respuesta2Correcta = tieneHijos == true ? "Si" : "No";
                }
                ///fin preguntas2
                ///

                //seleccionar preguntas3
                if (listadopreguntasSeleccionadas[2] == "Vive")
                {
                    listaPreguntas3 = PreguntasAleatorias(_connection.OrionContext.sp_ColoniasFalsas_List().ToList(), lugar, 1);
                    pregunta3 = Preguntas.preguntaVive;
                    respuesta3Correcta = lugar;
                }
                if (listadopreguntasSeleccionadas[2] == "Trabaja")
                {
                    listaPreguntas3 = PreguntasAleatorias(_connection.OrionContext.sp_TrabajosFalsos_List().ToList(), trabajo, 1);
                    pregunta3 = Preguntas.preguntatrabajo;
                    respuesta3Correcta = trabajo;
                }

                if (listadopreguntasSeleccionadas[2] == "Parentesco")
                {
                    listaPreguntas3 = PreguntasAleatorias(_connection.OrionContext.sp_Catalogo_Parentescos_List().Select(x => x.fcDescripcionParentesco).ToList(), parentescoCorrecto.ToString(), 0);
                    pregunta3 = Preguntas.preguntaParentesco;
                    respuesta3Correcta = parentescoCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[2] == "Estado Civil")
                {
                    listaPreguntas3 = PreguntasAleatorias(ListaEstadosCiviles().Select(x => x.descripcion).ToList(), estadoCivilCorrecto.ToString(), 0);
                    pregunta3 = Preguntas.preguntaEstadoCivil;
                    respuesta3Correcta = estadoCivilCorrecto.ToString();
                }
                if (listadopreguntasSeleccionadas[2] == "Ciudad")
                {
                    listaPreguntas3 = PreguntasAleatorias(_connection.OrionContext.sp_ListadoCiudades().Select(x => x.fcCiudad).ToList(), ciudad, 1);
                    pregunta3 = Preguntas.PreguntaCiudad;
                    respuesta3Correcta = ciudad;
                }
                if (listadopreguntasSeleccionadas[2] == "Tiene Hijos")
                {
                    listaPreguntas3 = PreguntasAleatorias(ListaEstadosTieneHijos().Select(x => x.descripcion).ToList(), tieneHijos == true ? "Si" : "No", 0);
                    pregunta3 = Preguntas.preguntaTieneHijos;
                    respuesta3Correcta = tieneHijos == true ? "Si" : "No";
                }
                ///fin preguntas3

                var listaColoniasFalsas = listaPreguntas1;

                ViewBag.ListaColonias = listaColoniasFalsas.Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z }).ToList();
                //
                var listaTrabajosFalsos = listaPreguntas2;
                //listaTrabajosFalsos = PreguntasAleatorias(listaTrabajosFalsos, trabajo);
                ViewBag.ListaTrabajos = listaTrabajosFalsos.Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z }).ToList();
                var listaParentescos = listaPreguntas3;
                ViewBag.ListaParentescos = listaParentescos.Select(z => new SelectListItem { Value = Convert.ToString(z), Text = z }).ToList();
                var nombre = _connection.OrionContext.sp_Cliente_ObtenerNombre(fiIDEquifax);

                referenciaclienteViewModel model = new referenciaclienteViewModel()
                {
                    fiIDCliente = fiIDEquifax,
                    fcIDReferencia = fiIDReferencia,

                    fcPregunta1 = pregunta1,
                    fcPregunta2 = pregunta2,
                    fcPregunta3 = pregunta3,
                    fcRespuesta1Correcta = respuesta1Correcta,
                    fcRespuesta2Correcta = respuesta2Correcta,
                    fcRespuesta3Correcta = respuesta3Correcta,
                    nombrecliente = nombre.FirstOrDefault()
                };
                return PartialView(model);
            }
            catch (Exception e)
            {
                EnviarException(e, "ERROR TECNICO");
                throw;
            }

        }

        [HttpPost]
        public ActionResult GuardarRespuestasReferencias(referenciaclienteViewModel model)
        {
            try
            {
                if (model.AceptoSerReferencia)
                {
                    int cantidadRespuestasCorrectas = 0;
                    if (model.fcRespuesta1.ToString() == "" || model.fcRespuesta2.ToString() == "" || model.fcRespuesta3.ToString() == "")
                    {
                        return EnviarResultado(false, "Completa todas las preguntas,gracias");
                    }
                    ///fin validaciones

                    if (model.fcRespuesta1 == model.fcRespuesta1Correcta)
                    {
                        cantidadRespuestasCorrectas++;
                    }
                    if (model.fcRespuesta2 == model.fcRespuesta2Correcta)
                    {
                        cantidadRespuestasCorrectas++;
                    }
                    if (model.fcRespuesta3 == model.fcRespuesta3Correcta)
                    {
                        cantidadRespuestasCorrectas++;
                    }
                    var guardarDatos = _connection.OrionContext.sp_Cliente_Referencia_Respuestas_Insert(model.fcIDReferencia, 1, cantidadRespuestasCorrectas, 3 - cantidadRespuestasCorrectas, model.fcRespuesta1, model.fcRespuesta2, model.fcRespuesta3) > 0;
                    if (guardarDatos)
                    {
                        var referencia = _connection.OrionContext.sp_ClienteReferencia_ObtenerPorIdReferencia(model.fcIDReferencia).FirstOrDefault();
                        if (referencia != null)
                        {
                            EnviarNotificacion($"La referencia {referencia.fcNombreCompletoReferencia}, {referencia.fcDescripcionParentesco} del cliente {referencia.fcNombre} ha acertado {referencia.fiRespuestasNoAcertadas} preguntas");
                        }

                        return EnviarResultado(true, "Gracias por sus respuestas, puede cerrar la pagina!");
                    }
                }
                else
                {
                    var guardarDatos = _connection.OrionContext.sp_Cliente_Referencia_Respuestas_Insert(model.fcIDReferencia, 2, 0, 0, "", "", "") > 0;
                    if (guardarDatos)
                    {
                        return EnviarResultado(true, "Gracias por su respuesta, puede cerrar la pagina!");
                    }
                }
                return EnviarResultado(false, "Error en el proceso");

            }
            catch (Exception e)
            {
                return EnviarResultado(false, "Favor completa todo el formulario");

            }

        }

        public JsonResult ObtenerInfoQR(int id)
        {
            try
            {
                //var solicitud = _connection.OrionContext.sp_ObtenerSolicitud_Idequifax(id).FirstOrDefault();
                var solicitud = _connection.OrionContext.sp_ObtenerSolicitud_Estado(id).FirstOrDefault();
                return EnviarListaJson(solicitud);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]

        public JsonResult GuardarFotos(List<Base64ynombreViewModel> fotosbase, int fiIDSolicitud)
        {

            try
            {
                //  var SolicitudCliente = _connection.OrionContext.sp_ObtenerSolicitud_Idequifax(fiIDEquifaxCliente).FirstOrDefault();
                foreach (var item in fotosbase)
                {
                    //item.IdSolicitud = fiIDSolicitud;
                    using (var context = new ORIONDBEntities())
                    {
                        var arch = ConvertirBase64AImagen(item.base64string, item.nombrearchivo);

                        var documentoURL = @"\Solicitudes\Solicitud_" + fiIDSolicitud;
                        var urlPdf = MemoryLoadManager.URL + documentoURL;
                        var ruta = documentoURL + @"\";
                        ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                        var pdfFile = urlPdf + @"\" + item.nombrearchivo;
                        var exists = System.IO.Directory.Exists(urlPdf);
                        //item.Archivo.SaveAs(MemoryLoadManager.URL + @"\" + item.Archivo.FileName);
                        context.sp_Crear_DocumentosOrion(GetIdUser(), fiIDSolicitud, Path.GetFileNameWithoutExtension(item.nombrearchivo), Path.GetExtension(item.nombrearchivo), urlPdf, pdfFile, item.numimagen, "_", 0);
                        UploadFileServer148(@"Solicitudes\Solicitud_" + fiIDSolicitud, arch);

                        //EnviarNotificacion((model.NombreCliente ?? "") + " ha firmado su contrato de convenio y servicios.");
                    }
                }


                return EnviarResultado(true, "", "Subido");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }

        }

        public ActionResult VistadeLlenadoFormulario()
        {
            return View();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        public ActionResult QRDescuento()
        {
            return View();
        }
        
        public ActionResult GetQRDescuento(string UID)
        {
            string qrData = null;

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Orion_VentaSolicitudCliente_Promocion_By_Token '{UID}'";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var data = new
                        {
                            fiVentaClientePromocion = reader["fiVentaClientePromocion"].ToString(),
                            fiIDSolicitud = reader["fiIDSolicitud"].ToString(),
                            fcCodigoPromocion = reader["fcCodigoPromocion"].ToString(),
                            fiEstadoPromocion = reader["fiEstadoPromocion"].ToString(),
                            fiUsuarioCreador = reader["fiUsuarioCreador"].ToString(),
                            fdFechaCreacion = reader["fdFechaCreacion"].ToString(),
                            fiUsuarioModificacion = reader["fiUsuarioModificacion"].ToString(),
                            fdFechaModificacion = reader["fdFechaModificacion"].ToString(),
                            fcToken = reader["fcToken"].ToString(),
                            fdFechaUso = reader["fdFechaUso"].ToString(),
                        };
                        //var promocion = Convert.ToBase64String(Encoding.UTF8.GetBytes(data.fcCodigoPromocion));
                        //var solicitud = Convert.ToBase64String(Encoding.UTF8.GetBytes(data.fiIDSolicitud.ToString()));
                        
                        //qrData = $"{promocion} | {solicitud}";
                        //qrData = $"{{\"fcCodigoPromocion\":\"{data.fcCodigoPromocion}\", \"fiIDSolicitud\":{data.fiIDSolicitud}}}";
                        qrData = $"{data.fcCodigoPromocion} | {data.fiIDSolicitud}";
                        //qrData = $"{{\"fcCodigoPromocion\":\"{promocion}\", \"fiIDSolicitud\":{solicitud}}}";

                    }
                }
            }   

            if (string.IsNullOrEmpty(qrData))
            {
                return HttpNotFound("No se ha encontrado ningun dato.");
            }

            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            return File(ms.ToArray(), "image/png");
                        }
                    }
                }
            }
        }

    }
}