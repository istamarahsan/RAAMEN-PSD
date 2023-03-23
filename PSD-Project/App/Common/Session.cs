using System.Web;
using System.Web.SessionState;
using PSD_Project.API.Features.Authentication;
using Util.Option;

namespace PSD_Project.App.Common
{
    public static class Session
    {
        public static Option<UserSessionDetails> GetUserSession(this HttpSessionState session)
        {
            return session[Globals.SavedSessionName].ToOption().Cast<UserSessionDetails>();
        }

        public static Option<int> GetTokenFromCookie(this HttpRequest request)
        {
            return request.Cookies[Globals.SessionCookieName]
                .ToOption()
                .Map(cookie => cookie.Value)
                .Bind(val => val.TryParseInt().Ok());
        }

        public static Option<string> GetUsernameFromCookie(this HttpRequest request)
        {
            return request.Cookies[Globals.SavedUsernameCookieName]
                .ToOption()
                .Map(cookie => cookie.Value);
        }
        
        public static Option<string> GetPasswordFromCookie(this HttpRequest request)
        {
            return request.Cookies[Globals.SavedPasswordCookieName]
                .ToOption()
                .Map(cookie => cookie.Value);
        }
    }
}