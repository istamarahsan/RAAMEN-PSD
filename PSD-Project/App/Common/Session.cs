using System.Web.SessionState;
using PSD_Project.Features.LogIn;
using Util.Option;

namespace PSD_Project.App.Common
{
    public static class Session
    {
        public static Option<UserSessionDetails> GetUserSession(this HttpSessionState session)
        {
            return session[Globals.SavedSessionName].ToOption().Cast<UserSessionDetails>();
        }
    }
}