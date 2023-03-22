using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.EntityFramework;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    public class RamenRepository : IRamenRepository, IRamenService
    {
        private readonly Raamen db = new Raamen();
        
        public Try<List<Ramen>, Exception> GetRamen()
        {
            try
            {
                var ramen = db.Ramen
                    .ToList()
                    .Select(ConvertModel)
                    .ToList();
                return Try.Of<List<Ramen>, Exception>(ramen);
            }
            catch (Exception e)
            {
                return Try.Err<List<Ramen>, Exception>(e);
            }
        }

        public Try<Ramen, Exception> CreateRamen(RamenDetails ramenDetails)
        {
            return CreateRamen(ramenDetails.Name, ramenDetails.Borth, ramenDetails.Price, ramenDetails.MeatId);
        }

        public Try<Ramen, Exception> UpdateRamen(int id, RamenDetails ramenDetails)
        {
            return UpdateRamen(id, ramenDetails.Name, ramenDetails.Borth, ramenDetails.Price, ramenDetails.MeatId);
        }

        public Try<Ramen, Exception> GetRamen(int ramenId)
        {
            var ramen = db.Ramen.Find(ramenId);
            return ramen.ToOption()
                .OrErr(() => new Exception("Ramen not found"))
                .Map(ConvertModel);
        }

        public Try<Ramen, Exception> CreateRamen(string name, string borth, string price, int meatId)
        {
            var nextId = db.Ramen.Select(ramen => ramen.id).Max() + 1;
            var meat = db.Meats.Find(meatId);

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
                db.SaveChanges();
                return Try.Of<Ramen, Exception>(ConvertModel(newRamen));
            }
            catch (Exception e)
            {
                return Try.Err<Ramen, Exception>(e);
            }
        }

        public Try<Ramen, Exception> UpdateRamen(int ramenId, string name, string borth, string price, int meatId)
        {
            var ramen = db.Ramen.Find(ramenId);
            var meat = db.Meats.Find(meatId);
            
            if (ramen == null || meat == null)
                return Try.Err<Ramen, Exception>(new ArgumentException());
            
            ramen.Name = name;
            ramen.Borth = borth;
            ramen.Price = price;
            ramen.Meat = meat;
            try
            {
                db.SaveChanges();
                return Try.Of<Ramen, Exception>(ConvertModel(ramen));
            }
            catch (Exception e)
            {
                return Try.Err<Ramen, Exception>(e);
            }
        }

        public Option<Exception> DeleteRamen(int ramenId)
        {
            var ramen = db.Ramen.Find(ramenId);
            if (ramen == null)
                return Option.Some<Exception>(new ArgumentException());
            try
            {
                db.Ramen.Remove(ramen);
                db.SaveChanges();
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