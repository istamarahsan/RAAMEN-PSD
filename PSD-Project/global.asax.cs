using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PSD_Project
{
    public class RaamenApp : HttpApplication
    {
        public static readonly HttpClient HttpClient = new HttpClient();
        protected void Application_Start(object sender, EventArgs e) 
        {
            var config = GlobalConfiguration.Configuration;
            config.RegisterRoutes();
            config.EnsureInitialized();
        }
    }
}