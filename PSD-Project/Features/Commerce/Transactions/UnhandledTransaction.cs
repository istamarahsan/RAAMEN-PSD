using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PSD_Project.Features.Commerce.Orders;

namespace PSD_Project.Features.Commerce.Transactions
{
    [DataContract]
    public class UnhandledTransaction
    {
        [DataMember]
        public readonly int Id;
        [DataMember] 
        public readonly int CustomerId;
        [DataMember] 
        public readonly DateTime TimeCreated;
        [DataMember] 
        public readonly List<CartItem> Items;

        public UnhandledTransaction(int id, int customerId, DateTime timeCreated, List<CartItem> items)
        {
            Id = id;
            CustomerId = customerId;
            TimeCreated = timeCreated;
            Items = items;
        }
    }
}