using PSD_Project.App.Services.RamenService;
using PSD_Project.App.Services.Auth;
using PSD_Project.App.Services.Login;
using PSD_Project.App.Services.Register;
using PSD_Project.App.Services.Users;

namespace PSD_Project.App
{
    public static class AppServices
    {
        public class AppSingletons
        {
            public readonly ILoginService LoginService = new AdapterLoginService();
            public readonly IAuthService AuthService = new AdapterAuthService();
            public readonly IRegisterService RegisterService = new AdapterRegisterService();
            public readonly IUserService UserService = new AdapterUserService();
            public readonly IRamenService RamenService = new AdapterRamenService();
        }

        public static readonly AppSingletons Singletons = new AppSingletons();
    }
}