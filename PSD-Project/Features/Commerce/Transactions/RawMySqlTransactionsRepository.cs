using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public class RawMySqlTransactionsRepository : ITransactionsRepository
    {
        public async Task<Try<TransactionRecord, Exception>> AddNewTransactionAsync(int customerId, int staffId, List<CartItem> cart)
        {
            var db = new Raamen();
            var transaction = db.Database.BeginTransaction();
            try
            {
                var headerId = await db.Headers.Select(h => h.id).DefaultIfEmpty(1).MaxAsync() + 1;
                var headersAdded = await db.Database.ExecuteSqlCommandAsync(
                    $"insert into Header(id, CustomerId, StaffId, `Date`) values ({headerId}, {customerId}, {staffId}, current_date)");
                if (headersAdded != 1) throw new Exception();
                if (cart.Count == 0) return Try.Of<TransactionRecord, Exception>(new TransactionRecord(headerId, customerId, staffId, new List<TransactionEntry>()));
                foreach (var item in cart)
                {
                    var detailsAdded = await db.Database.ExecuteSqlCommandAsync(
                        $"insert into Detail(Headerid, Ramenid, Quantity) values ({headerId}, {item.RamenId}, {item.Quantity})");
                    if (detailsAdded != 1) throw new Exception();
                }
                transaction.Commit();
                return Try.Of<TransactionRecord, Exception>(new TransactionRecord(
                    headerId, 
                    customerId, 
                    staffId, 
                    cart.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList()));
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Try.Err<TransactionRecord, Exception>(e);
            }
            finally
            {
                transaction.Dispose();
                db.Dispose();
            }
        }

        private string BuildDetailValues(int headerId, CartItem item)
        {
            return $"({headerId}, {item.RamenId}, {item.Quantity})";
        }
    }
}