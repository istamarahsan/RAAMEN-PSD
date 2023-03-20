using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using PSD_Project.Features.Ramen;
using Util.Option;
using Util.Try;
using Xunit;

namespace PSD_Project_Test
{
    public class RamenTests
    {
        private static readonly IDictionary<int, Meat> TestMeats = new Dictionary<int, Meat>
        {
            [0] = new Meat(0, "DefaultMeat")
        };

        private class TestRepository : IRamenRepository
        {
            private readonly IDictionary<int, Meat> meats = new Dictionary<int, Meat>();
            private readonly IDictionary<int, Ramen> ramen = new Dictionary<int, Ramen>();

            public TestRepository()
            {
                
            }

            public TestRepository(IDictionary<int, Meat> meats, IDictionary<int, Ramen> ramen)
            {
                this.meats = meats;
                this.ramen = ramen;
            }

            public Task<Try<List<Ramen>, Exception>> GetRamenAsync()
            {
                return Task.FromResult(Try.Of<List<Ramen>, Exception>(ramen.Values.ToList()));
            }

            public Task<Option<Ramen>> GetRamenAsync(int ramenId)
            {
                throw new NotImplementedException();
            }

            public Task<Try<Ramen, Exception>> AddRamenAsync(string name, string borth, string price, int meatId)
            {
                throw new NotImplementedException();
            }

            public Task<Try<Ramen, Exception>> UpdateRamenAsync(int ramenId, string name, string borth, string price, int meatId)
            {
                throw new NotImplementedException();
            }

            public Task<Option<Exception>> DeleteRamenAsync(int ramenId)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async void GetAllRamenReturnsAllRamen()
        {
            var ramen = new Ramen(0, "", "", "20", new Meat(0, ""));
            var controller = new RamenController(new TestRepository(new Dictionary<int, Meat>(), new Dictionary<int, Ramen>{[0] = ramen}));
            var response = await controller.GetAllRamen();
            var returnedRamenList = ((OkNegotiatedContentResult<List<Ramen>>)response).Content;
            Assert.Equal(ramen, returnedRamenList.First());
        }
    }
}