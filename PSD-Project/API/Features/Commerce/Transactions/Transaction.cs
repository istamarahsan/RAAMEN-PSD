using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [DataContract]
    public class Transaction
    {
        [DataMember] public readonly int Id;
        [DataMember] public readonly int CustomerId;
        [DataMember] public readonly int StaffId;
        [DataMember] public readonly DateTime? Date;
        [DataMember] public readonly List<TransactionEntry> Details;

        public Transaction(int id, int customerId, int staffId, List<TransactionEntry> details, DateTime? date)
        {
            Id = id;
            CustomerId = customerId;
            StaffId = staffId;
            Date = date;
            Details = details;
        }
        
        public Transaction(int id, int customerId, int staffId, List<TransactionEntry> details)
        {
            Id = id;
            CustomerId = customerId;
            StaffId = staffId;
            Date = null;
            Details = details;
        }
    }
}