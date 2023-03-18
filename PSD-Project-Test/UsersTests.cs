using System.Linq;
using PSD_Project;
using Xunit;

namespace PSD_Project_Test
{
    public class UsersTests
    {
        private Raamen _db = new Raamen();

        [Fact]
        public void DatabaseConnection()
        {
            _db.Users.FirstOrDefault();
        }
    }
}