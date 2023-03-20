using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using PSD_Project.EntityFramework;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public class EfTransactionsRepository : ITransactionsRepository
    {

        public async Task<Try<TransactionRecord, Exception>> AddNewTransactionAsync(int customerId, int staffId,
            List<CartItem> cart)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(5)
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var db = new Raamen();
                    var nextId = await db.Headers.Select(h => h.id).DefaultIfEmpty(1).MaxAsync() + 1;
                    var newTransaction = new Header
                    {
                        id = nextId,
                        CustomerId = customerId,
                        Staffid = staffId,
                        Date = DateTime.Now
                    };
                    db.Headers.Add(newTransaction);
                    var details = cart.Select(i => new Detail
                    {
                        Headerid = nextId,
                        Quantity = i.Quantity
                    }).ToList();
                    await db.SaveChangesAsync();
                    db.Details.AddRange(details);
                    await db.SaveChangesAsync();
                    var record = ConvertModel(newTransaction, details);
                    return Try.Of<TransactionRecord, Exception>(record);
                }
                catch (Exception e)
                {
                    return Try.Err<TransactionRecord, Exception>(e);
                }
                finally
                {
                    scope.Complete();
                }
            }
        }

        private TransactionRecord ConvertModel(Header transactionHeader, List<Detail> details)
        {
            return transactionHeader.Date == null
                ? new TransactionRecord(transactionHeader.id,
                    (int)transactionHeader.CustomerId,
                    (int)transactionHeader.Staffid,
                    details.Select(d => new TransactionEntry(d.Ramenid, (int)d.Quantity)).ToList())
                : new TransactionRecord(transactionHeader.id,
                    (int)transactionHeader.CustomerId,
                    (int)transactionHeader.Staffid,
                    details.Select(d => new TransactionEntry(d.Ramenid, (int)d.Quantity)).ToList(),
                    (DateTime)transactionHeader.Date);
        }
    }
}