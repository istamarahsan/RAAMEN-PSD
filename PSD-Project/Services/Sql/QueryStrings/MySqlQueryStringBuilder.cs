using System;

namespace PSD_Project.Services.Sql.QueryStrings
{
    public class MySqlQueryStringBuilder : QueryStringBuilder
    {
        public override string StringForAddHeader(int headerId, int customerId, int staffId, DateTime date)
        {
            return
                $"insert into Header(id, CustomerId, StaffId, `Date`) values ({headerId}, {customerId}, {staffId}, str_to_date('{date.Day:00}/{date.Month:00}/{date.Year}', '%d/%m/%Y'))";
        }

        public override string StringForAddDetails(int headerId, int ramenId, int quantity)
        {
            return
                $"insert into Detail(Headerid, Ramenid, Quantity) values ({headerId}, {ramenId}, {quantity})";
        }
    }
}