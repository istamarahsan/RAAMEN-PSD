using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Option;
using Util.Try;

namespace PSD_Project.Features.Ramen
{
    public class RamenRepository : IRamenRepository
    {
        private readonly Raamen db = new Raamen();
        
        public async Task<Try<List<Ramen>, Exception>> GetAllRamenAsync()
        {
            try
            {
                var ramen = await db.Ramen.ToListAsync();
                return Try.Of<List<Ramen>, Exception>(ramen.Select(ConvertModel).ToList());
            }
            catch (Exception e)
            {
                return Try.Err<List<Ramen>, Exception>(e);
            }
        }

        public async Task<Try<Ramen, Exception>> AddRamenAsync(string name, string borth, string price, int meatId)
        {
            var nextId = await db.Ramen.Select(ramen => ramen.id).MaxAsync() + 1;
            var meat = await db.Meats.FindAsync(meatId);

            if (meat == null) return Try.Err<Ramen, Exception>(new ArgumentException());

            var newRamen = new Raman
            {
                id = nextId,
                Name = name,
                Price = price,
                Borth = borth,
                Meatid = meat.id
            };

            try
            {
                db.Ramen.Add(newRamen);
                await db.SaveChangesAsync();
                return Try.Of<Ramen, Exception>(ConvertModel(newRamen));
            }
            catch (Exception e)
            {
                return Try.Err<Ramen, Exception>(e);
            }
        }

        public async Task<Try<Ramen, Exception>> UpdateRamenAsync(int ramenId, string name, string borth, string price, int meatId)
        {
            var ramen = await db.Ramen.FindAsync(ramenId);
            var meat = await db.Meats.FindAsync(meatId);
            
            if (ramen == null || meat == null)
                return Try.Err<Ramen, Exception>(new ArgumentException());
            
            ramen.Name = name;
            ramen.Borth = borth;
            ramen.Price = price;
            ramen.Meat = meat;
            try
            {
                await db.SaveChangesAsync();
                return Try.Of<Ramen, Exception>(ConvertModel(ramen));
            }
            catch (Exception e)
            {
                return Try.Err<Ramen, Exception>(e);
            }
        }

        public async Task<Option<Exception>> DeleteRamenAsync(int ramenId)
        {
            var ramen = await db.Ramen.FindAsync(ramenId);
            if (ramen == null)
                return Option.Some<Exception>(new ArgumentException());
            try
            {
                db.Ramen.Remove(ramen);
                await db.SaveChangesAsync();
                return Option.None<Exception>();
            }
            catch (Exception e)
            {
                return Option.Some(e);
            }
        }
        
        private Ramen ConvertModel(Raman ramen)
        {
            return new Ramen(ramen.id, ramen.Name, ramen.Borth, ramen.Price, new Meat(ramen.Meat.id, ramen.Meat.name));
        }
    }
}