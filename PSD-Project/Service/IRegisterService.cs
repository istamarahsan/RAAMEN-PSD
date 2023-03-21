using System.Net;
using System.Threading.Tasks;
using PSD_Project.App.Models;

namespace PSD_Project.Service
{
    public interface IRegisterService
    {
        Task<HttpStatusCode> RegisterNewUser(RegistrationFormDetails form);
    }
}