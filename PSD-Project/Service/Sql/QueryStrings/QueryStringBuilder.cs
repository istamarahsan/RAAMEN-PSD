using System;

namespace PSD_Project.Service.Sql.QueryStrings
{
    public abstract class QueryStringBuilder
    {
        public SqlDialect Dialect { get; protected set; }

        public abstract string StringForAddHeader(int headerId, int customerId, int staffId, DateTime date);
        public abstract string StringForAddDetails(int headerId, int ramenId, int quantity);
    }
}