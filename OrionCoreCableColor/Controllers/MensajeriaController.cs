using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.Models.Mensajeria;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class MensajeriaController : BaseController
    {
        // GET: Mensajeria

        #region Telegram
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexTelgramBot()
        {
            ViewBag.urlHub = MemoryLoadManager.Produccion ? $"https://ptdto.com/MensajeriaApi/mensajeriatelegramhub" : "https://localhost:7158/mensajeriatelegramhub";
            ViewBag.fcToken = MemoryLoadManager.tokenMensajeria;
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CargarListaBotsTelegram()
        {
            var bots = await ApiMensajeriaHelper.GetListBotTelegram();
            return EnviarListaJson(bots);
        }




        [HttpGet]
        public async Task<JsonResult> CargarListaUsuariosRegistrados()
        {
            return EnviarListaJson(await ApiMensajeriaHelper.GetListUsuarios());
        }

        [HttpGet]
        public async Task<ActionResult> EnviarMensajeTelegram(ListTelegramViewModel model)
        {
            var listaUsuarios = await ApiMensajeriaHelper.GetListUsuarios();
            ViewBag.ListaUsuarios = listaUsuarios.Select(x => new SelectListItem { Value = x.fiIDChatBot.ToString(), Text = x.fcNombreChat }).ToList();
            return PartialView(model);
        }




        [HttpPost]
        public async Task<JsonResult> EnviarMensajeTelegram(MensajeTelegramViewModel model)
        {
            var result = await ApiMensajeriaHelper.EnviarMensajesTelegram(model);
            return EnviarResultado(result, "Mensaje Telegram");
        }

        #endregion

        #region Sms
        [HttpGet]
        public ActionResult IndexSms()
        {
            return View();
        }

        [HttpGet] 
        public async Task<JsonResult> CargarListaSms()
        {
            var lista = await ApiMensajeriaHelper.GetListSmsService();
            return EnviarListaJson(lista);
        }

        [HttpGet]
        public async Task<ActionResult> EnviarMensajeSms(ListServiciosSmsViewModel model)
        {
            var listaUsuarios = await ApiMensajeriaHelper.GetTelefonosEmpleados();
            ViewBag.ListaTelefonos = listaUsuarios.Select(x=>new SelectListItem { Value = x.fcTelefono, Text = $"{x.fcNombre} ({x.fcTelefono})" });
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> EnviarMensajeSms(MensajeSmsViewModel model)
        {
            var result = await ApiMensajeriaHelper.EnviarMensajesSMS(model);
            return EnviarResultado(result, "Mensaje SMS");
        }
        #endregion

        #region Correo
        public async Task<ActionResult> IndexCorreo()
        {
            var listCorreosEmisor = await ApiMensajeriaHelper.GetListEmails();
            ViewBag.listaCorreosEmisor = listCorreosEmisor.Select(x => new SelectListItem { Value = x.fcCodigo, Text = x.fcEmail }).ToList();
            var listaCorreosDestino = await ApiMensajeriaHelper.GetCorreosEmpleados();
            ViewBag.listCorreos = listaCorreosDestino.Select(x => new SelectListItem { Value = x.fcCorreo, Text = $"{x.fcNombre} [{x.fcCorreo}]" }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> EnviarCorreo(SendEmailViewModel model)
        {
            var result = await ApiMensajeriaHelper.EnviarCorreo(model);
            return EnviarResultado(result, "Correo");
        }
        
        #endregion
    }
}