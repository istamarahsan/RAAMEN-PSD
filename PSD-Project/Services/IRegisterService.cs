using System.Net;
using System.Threading.Tasks;
using PSD_Project.App.Models;

namespace PSD_Project.Services
{
    public interface IRegisterService
    {
        Task<HttpStatusCode> RegisterNewUserAsync(RegistrationFormDetails form);
    }
}