using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [DataContract]
    public class NewTransactionDetails
    {
        [DataMember] public readonly int CustomerId;
        [DataMember] public readonly int StaffId;
        [DataMember] public readonly DateTime Date;
        [DataMember] public readonly List<TransactionEntry> Details;

        public NewTransactionDetails(int customerId, int staffId, DateTime date, List<TransactionEntry> details)
        {
            CustomerId = customerId;
            StaffId = staffId;
            Date = date;
            Details = details;
        }
    }
}