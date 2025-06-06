using Microsoft.AspNet.SignalR;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OrionCoreCableColor
{
    public class NotificacionesHub:Hub
    {
        private string NombreSala = "Notificaciones";
        
        ///CONNECTION
        public override Task OnConnected()
        {

            if (!String.IsNullOrEmpty(Context.User.Identity.Name))
            {
                MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.UserName == Context.User.Identity.Name).connectionId = Context.ConnectionId;
            }
            Clients.Others.ClientConnected(Context.ConnectionId);
           
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            // Notificar a los demás usuarios que el cliente se ha reconectado
            Clients.Others.ClientReconnected(Context.ConnectionId);

            // También puedes ejecutar alguna lógica de reinicio o reconfiguración si es necesario

            return base.OnReconnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            // do the logging her

            //Trace.WriteLine(Context.ConnectionId + " - disconnected");
            Clients.Others.ClientDisconnected(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }


        ///OTROS PROCESOS

        public async Task  EnviarNotificacion(string mensajeNotificaciones)
        {
            try
            {

                // var user_ = Context.ConnectionId;
                await Task.Run(Clients.All.enviarNotificacion(mensajeNotificaciones));
                //this.Clients.All.enviarNotificacion(mensajeNotificaciones);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task InsertarUsuarios(InfoUsuarioViewModel model)
        {
            await Task.Run(Clients.All.insertarUsuarios(model));
        }

        public void AgregarSesion()
        {        
                 Groups.Add(Context.ConnectionId, NombreSala);              
        }
        

        // Enviar señal de oferta o respuesta a otro cliente
        public async Task EnviarSeñal(string usuarioDestino, string señal)
        {
            await Clients.User(usuarioDestino).RecibirSeñal(señal, usuarioDestino);
            //await Clients.User(usuarioDestino).SendAsync("RecibirSeñal", señal);
        }

        // Enviar candidatos ICE
        public async Task EnviarCandidatoIce(string usuarioDestino, string candidato)
        {
            await Clients.User(usuarioDestino).RecibirCandidatoIce(candidato);
            //await Clients.User(usuarioDestino).SendAsync("RecibirCandidatoIce", candidato);
        }

    }
}