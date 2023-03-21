using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.Features.Commerce.Orders
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public readonly int Id;
        [DataMember] 
        public readonly int CustomerId;
        [DataMember] 
        public readonly DateTime TimeCreated;
        [DataMember] 
        public readonly List<CartItem> Items;

        public Order(int id, int customerId, DateTime timeCreated, List<CartItem> items)
        {
            Id = id;
            CustomerId = customerId;
            TimeCreated = timeCreated;
            Items = items;
        }
    }
}