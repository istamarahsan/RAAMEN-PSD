using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public class DictTransactionsRepository : ITransactionsRepository
    {
        private Dictionary<int, TransactionRecord> records = new Dictionary<int, TransactionRecord>();

        public Task<Try<TransactionRecord, Exception>> AddNewTransactionAsync(int customerId, int staffId, List<CartItem> cart)
        {
            var record = new TransactionRecord(
                records.Keys.DefaultIfEmpty(1).Max() + 1,
                customerId,
                staffId,
                cart.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList());
            records[record.Id] = record;
            return Task.FromResult(Try.Of<TransactionRecord, Exception>(record));
        }
    }
}