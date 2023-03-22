using PSD_Project.App.Services.Auth;
using PSD_Project.App.Services.Login;

namespace PSD_Project.App
{
    public static class AppServices
    {
        public class AppSingletons
        {
            public readonly ILoginService LoginService = new AdapterLoginService();
            public readonly IAuthService AuthService = new AdapterAuthService();
        }

        public static readonly AppSingletons Singletons = new AppSingletons();
    }
}