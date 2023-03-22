using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    public class TransactionsRepository : ITransactionsRepository, ITransactionsService
    {
        private readonly Raamen readOnlyDbContext = new Raamen();
        
        public Try<Transaction, Exception> CreateTransaction(int customerId, int staffId, DateTime date, List<TransactionEntry> entries)
        {
            var db = new Raamen();
            var transaction = db.Database.BeginTransaction();
            var stringBuilder = Services.GetQueryStringBuilder(Services.GetConfiguredDialect());
            try
            {
                var headerId = db.Headers.Select(h => h.id).DefaultIfEmpty(1).Max() + 1;
                
                var headersAdded = db.Database.ExecuteSqlCommand(stringBuilder.StringForAddHeader(headerId, customerId, staffId, date));
                if (headersAdded != 1) throw new Exception();
                if (entries.Count == 0) return Try.Of<Transaction, Exception>(new Transaction(headerId, customerId, staffId, new List<TransactionEntry>()));
                foreach (var entry in entries)
                {
                    var detailsAdded = db.Database.ExecuteSqlCommand(stringBuilder.StringForAddDetails(headerId, entry.RamenId, entry.Quantity));
                    if (detailsAdded != 1) throw new Exception();
                }
                transaction.Commit();
                return Try.Of<Transaction, Exception>(new Transaction(
                    headerId, 
                    customerId, 
                    staffId,
                    entries.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList(),
                    date));
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Try.Err<Transaction, Exception>(e);
            }
            finally
            {
                transaction.Dispose();
                db.Dispose();
            }
        }

        public Try<Transaction, Exception> CreateTransaction(TransactionDetails transactionDetails)
        {
            return CreateTransaction(transactionDetails.CustomerId, 
                transactionDetails.StaffId, 
                transactionDetails.Date,
                transactionDetails.Entries);
        }

        public Try<List<Transaction>, Exception> GetTransactions()
        {
            try
            {
                var headers = readOnlyDbContext.Headers.ToList();
                var transactions = headers.Select(ConvertModel).ToList();
                return Try.Of<List<Transaction>, Exception>(transactions);
            }
            catch (Exception e)
            {
                return Try.Err<List<Transaction>, Exception>(e);
            }
        }

        public Try<Transaction, Exception> GetTransaction(int transactionId)
        {
            try
            {
                var header = readOnlyDbContext.Headers.Find(transactionId);
                if (header == null) throw new Exception();
                var transaction = ConvertModel(header);
                return Try.Of<Transaction, Exception>(transaction);
            }
            catch (Exception e)
            {
                return Try.Err<Transaction, Exception>(e);
            }
        }

        private Transaction ConvertModel(Header header)
        {
            return new Transaction(
                header.id,
                header.CustomerId ?? 0,
                header.Staffid ?? 0,
                header.Details.Select(d => new TransactionEntry(d.Ramenid, d.Quantity ?? 0)).ToList(),
                header.Date);
        }
    }
}