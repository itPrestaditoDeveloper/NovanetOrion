using Newtonsoft.Json;
using OrionCoreCableColor.Models.Saris;
using OrionCoreCableColor.Models.Soporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class SarisController : BaseController
    {
        // GET: Saris
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CrearTicketSarisModal(TicketsViewModel model)
        {
            var flCategorias = await ObtenerCategoriasAsync();
            var flAreas = await ObtenerAreasAsync();
            ViewBag.ListaCategorias = flCategorias.Select(a => new SelectListItem { Value = a.fiIdTicket_Categoria.ToString(), Text = a.fcDescripcionCategoria }).ToList();
            ViewBag.ListaAreas = flAreas.Select(a => new SelectListItem { Value = a.fiIDArea.ToString(), Text = a.fcDescripcion }).ToList();
            return PartialView(model);
        }

        public async Task<JsonResult> CrearTicket(TicketsViewModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            
            var usuario = GetUser();
            model.fiIdUsuarioCreacion = usuario.fiIdUsuario;

            using (var client = new HttpClient())
            {
                var url = "https://saris.novanetgroup.com/ApiSaris/CrearTicketApi";
                //var url = "https://localhost:7291/ApiSaris/CrearTicketApi";
                var modelJson = JsonConvert.SerializeObject(model);
                var content = new StringContent(modelJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                return EnviarListaJson(response);
            }
        }




        #region Listados 
        public async Task<List<CategoriaIncidenciaViewModel>> ObtenerCategoriasAsync()
        {
            using (var client = new HttpClient())
            {
                var url = "https://saris.novanetgroup.com/CategoriasIncidentes/DatosBandejaCategoria";
                var response = await client.GetAsync(url);

                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<CategoriaIncidenciaViewModel>>(json);
                    return categorias;
                }
                else
                {
                    Console.WriteLine("Error al llamar al servicio: " + response.StatusCode);
                    return new List<CategoriaIncidenciaViewModel>();
                }
            }
        }

        public async Task<List<AreasViewModel>> ObtenerAreasAsync()
        {
            using (var client = new HttpClient())
            {
                var url = "https://saris.novanetgroup.com/Areas/Areas";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<AreasViewModel>>(json);
                    return categorias;
                }
                else
                {
                    Console.WriteLine("Error al llamar al servicio: " + response.StatusCode);
                    return new List<AreasViewModel>();
                }
            }
        }


        public async Task<JsonResult> SelectTipoIncidenciaSaris(int fiIdCategoria)
            {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://saris.novanetgroup.com/LlenarCampos/SelectTipoIncidencia?fiIdCategoria={fiIdCategoria}";

                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var tipos = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
                        return EnviarListaJson(tipos);
                    }

                    return EnviarListaJson(new List<SelectListItem>());
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        #endregion

    }
}