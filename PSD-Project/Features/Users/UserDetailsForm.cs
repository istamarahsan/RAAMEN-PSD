namespace PSD_Project.Features.Users
{
    public partial class UsersController
    {
        public class UserDetailsForm
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }

            public UserDetailsForm(string username, string email, string gender, string password)
            {
                Username = username;
                Email = email;
                Gender = gender;
                Password = password;
            }
        }
    }
}