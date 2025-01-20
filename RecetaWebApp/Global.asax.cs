using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RecetaWebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Iniciar el consumidor al arrancar la aplicación
            Task.Run(() => StartRabbitMqConsumer());
        }

        // Método asincrónico para iniciar el consumidor
        private void StartRabbitMqConsumer()
        {
            var consumer = new RabbitMqConsumer();
            consumer.StartConsuming();  // Iniciar la escucha de mensajes
        }
    }
}

