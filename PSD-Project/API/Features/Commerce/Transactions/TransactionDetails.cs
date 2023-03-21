using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [DataContract]
    public class TransactionDetails
    {
        [DataMember] public readonly int CustomerId;
        [DataMember] public readonly int StaffId;
        [DataMember] public readonly DateTime Date;
        [DataMember] public readonly List<TransactionEntry> Entries;

        public TransactionDetails(int customerId, int staffId, DateTime date, List<TransactionEntry> entries)
        {
            CustomerId = customerId;
            StaffId = staffId;
            Date = date;
            Entries = entries;
        }
    }
}