using System;

namespace PSD_Project.API.Util.Sql.QueryStrings
{
    public class SqlServerQueryStringBuilder : QueryStringBuilder
    {
        public override string StringForAddHeader(int headerId, int customerId, int staffId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public override string StringForAddDetails(int headerId, int ramenId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}