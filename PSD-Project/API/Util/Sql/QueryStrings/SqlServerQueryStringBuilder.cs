using System;

namespace PSD_Project.API.Util.Sql.QueryStrings
{
    public class SqlServerQueryStringBuilder : QueryStringBuilder
    {
        public override string StringForAddHeader(int headerId, int customerId, int staffId, DateTime date)
        {
            // maybe this??
            return $"INSERT INTO [Header]([id], [CustomerId], [StaffId], [Date]) VALUES ({headerId}, {customerId}, {staffId}, convert(DATE, '{date.Year:0000}-{date.Month:00}-{date.Day:00}', 23))";
            throw new NotImplementedException();
        }

        public override string StringForAddDetails(int headerId, int ramenId, int quantity)
        {
            // maybe
            return $"insert into [Detail]([Headerid], [Ramenid], [Quantity]) VALUES ({headerId}, {ramenId}, {quantity})";
            throw new NotImplementedException();
        }
    }
}