using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using OrionCoreCableColor.App_Helper;
using Owin;
using System.Net;

[assembly: OwinStartupAttribute(typeof(OrionCoreCableColor.Startup))]
namespace OrionCoreCableColor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MemoryLoadManager.LoadMemory();
            app.MapSignalR();
            app.Map("/signalr", map =>
            {
                var hubConfiguration = new HubConfiguration();
                hubConfiguration.EnableDetailedErrors = true;
                hubConfiguration.EnableJavaScriptProxies = true;
                hubConfiguration.EnableJSONP = true;
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
