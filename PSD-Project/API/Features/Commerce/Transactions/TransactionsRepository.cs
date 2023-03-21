using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly Raamen readOnlyDbContext = new Raamen();
        
        public async Task<Try<TransactionRecord, Exception>> CreateTransaction(int customerId, int staffId, DateTime date, List<TransactionEntry> entries)
        {
            var db = new Raamen();
            var transaction = db.Database.BeginTransaction();
            try
            {
                var headerId = await db.Headers.Select(h => h.id).DefaultIfEmpty(1).MaxAsync() + 1;
                var headersAdded = await db.Database.ExecuteSqlCommandAsync(
                    $"insert into Header(id, CustomerId, StaffId, `Date`) values ({headerId}, {customerId}, {staffId}, str_to_date('{date.Day:00}/{date.Month:00}/{date.Year}', '%d/%m/%Y'))");
                if (headersAdded != 1) throw new Exception();
                if (entries.Count == 0) return Try.Of<TransactionRecord, Exception>(new TransactionRecord(headerId, customerId, staffId, new List<TransactionEntry>()));
                foreach (var entry in entries)
                {
                    var detailsAdded = await db.Database.ExecuteSqlCommandAsync(
                        $"insert into Detail(Headerid, Ramenid, Quantity) values ({headerId}, {entry.RamenId}, {entry.Quantity})");
                    if (detailsAdded != 1) throw new Exception();
                }
                transaction.Commit();
                return Try.Of<TransactionRecord, Exception>(new TransactionRecord(
                    headerId, 
                    customerId, 
                    staffId,
                    entries.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList(),
                    date));
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

        public async Task<Try<List<TransactionRecord>, Exception>> GetTransactions()
        {
            try
            {
                var headers = await readOnlyDbContext.Headers.ToListAsync();
                var transactions = headers.Select(ConvertModel).ToList();
                return Try.Of<List<TransactionRecord>, Exception>(transactions);
            }
            catch (Exception e)
            {
                return Try.Err<List<TransactionRecord>, Exception>(e);
            }
        }

        public async Task<Try<TransactionRecord, Exception>> GetTransaction(int transactionId)
        {
            try
            {
                var header = await readOnlyDbContext.Headers.FindAsync(transactionId);
                if (header == null) throw new Exception();
                var transaction = ConvertModel(header);
                return Try.Of<TransactionRecord, Exception>(transaction);
            }
            catch (Exception e)
            {
                return Try.Err<TransactionRecord, Exception>(e);
            }
        }

        private TransactionRecord ConvertModel(Header header)
        {
            return new TransactionRecord(
                header.id,
                header.CustomerId ?? 0,
                header.Staffid ?? 0,
                header.Details.Select(d => new TransactionEntry(d.Ramenid, d.Quantity ?? 0)).ToList(),
                header.Date);
        }
    }
}