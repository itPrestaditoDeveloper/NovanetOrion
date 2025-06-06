using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.Soporte;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class MemoryLoadManager
    {
    
        public static List<ListaDeUsuariosViewModel> ListaUsuarios { get; set; }
        public static string URL = HttpContext.Current.Server.MapPath("");
        public static bool Produccion = ConfigurationManager.AppSettings["Produccion"] == "true" ? true : false;
        public static string UrlEquifax = ConfigurationManager.AppSettings["UrlEquifax"];
        public static string Helper = ConfigurationManager.AppSettings["Helper"];
        public static string UrlWeb = ConfigurationManager.AppSettings["UrlAplicacion"];
        public static string UrlContrato = ConfigurationManager.AppSettings["UrlContrato"];
        public static string EmailSystemAdministrator = "angel.bautista@miprestadito.com";
        public static string EmailSystemGustavo = "gustavo.gamez@miprestadito.com";
        public static string VirtualPathServerToEmailTemplates = "~/Documento/Recursos/Plantilla/";
        public static string VirtualPathServerToCustomerAttachment = "~/Documento/Recursos/Solicitud/";
        public static int UsuarioSystemOrionCoreSeguridad = 281;
        public static string ApiKey = "10513B5D-D45D-4EAE-BCB6-81566E457135";
        public static List<sp_ONUsYOLTs_Listar_Result> ListaOLTsONUs;
        public static string orionUrl = "https://orion.novanetgroup.com/";
        public static string urlMensajeria = ConfigurationManager.AppSettings["UrlMensajeria"];
        public static string tokenMensajeria = ConfigurationManager.AppSettings["TokenMensajeria"];
        public static decimal ObtenerTasaCambio()
        {
            using (var connection = new CoreFinancieroEntities2())
            {
                var tasaCambio = new SqlParameter("pnTasaDeCambio", SqlDbType.Decimal)
                {
                    Direction = System.Data.ParameterDirection.Output,
                    Precision = 10,
                    Scale = 4
                };
                connection.Database.ExecuteSqlCommand("sp_CoreTasaDeCambio @pnTasaDeCambio Out", tasaCambio);

             return Convert.ToDecimal(tasaCambio.Value);
            }
        }
        public static void LoadMemory()
        {
            
            using (var context = new OrionSecurityEntities())
            {
                var list = context.Usuarios.ToList().Select(x => new ListaDeUsuariosViewModel
                {
                    fiIdUsuario = x.fiIDUsuario,
                    fcPrimerNombre = x.fcPrimerNombre,
                    fcPrimerApellido = x.fcPrimerApellido,
                    //FechaNacimiento = x.FechaNacimiento,
                    fiEstado = x.fiEstado,
                    fcBuzondeCorreo = x.AspNetUsers.Email,
                    fcTelefonoMovil = x.fcTelefonoMovil,
                    UserName = x.AspNetUsers.UserName,
                    NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : "",
                    fcIdAspNetUser = x.fcIdAspNetUser,
                    // = x.IdUsuarioCoreSeguridad,
                    fcUrlImage = x.fcUrlImage,
                    IdRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Pk_IdRol : 0,
                    //InfoUsuario = new List<InfoUsuarioViewModel>(),
                    fiIDEmpresa = x.fiIDEmpresa,
                     fiIDDistribuidor = x.fiIDDistribuidor
                }).ToList();

                ListaUsuarios = list;
                ListaOLTsONUs = new List<sp_ONUsYOLTs_Listar_Result>();


            }
        }


        public static void ActualizarListaUsuarios()
        {
            using (var context = new OrionSecurityEntities())
            {
                var list = context.Usuarios.ToList().Select(x => new ListaDeUsuariosViewModel
                {
                    fiIdUsuario = x.fiIDUsuario,
                    fcPrimerNombre = x.fcPrimerNombre,
                    fcPrimerApellido = x.fcPrimerApellido,
                    //FechaNacimiento = x.FechaNacimiento,
                    fiEstado = x.fiEstado,
                    fcBuzondeCorreo = x.AspNetUsers.Email,
                    fcTelefonoMovil = x.fcTelefonoMovil,
                    UserName = x.AspNetUsers.UserName,
                    NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : "",
                    fcIdAspNetUser = x.fcIdAspNetUser,
                    // = x.IdUsuarioCoreSeguridad,
                    fcUrlImage = x.fcUrlImage,
                    IdRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Pk_IdRol : 0,
                    //InfoUsuario = new List<InfoUsuarioViewModel>(),
                    fiIDEmpresa = x.fiIDEmpresa
                }).ToList();

                ListaUsuarios = list;
            }
        }
    }
}