using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.Features.Commerce.Orders
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public readonly int CustomerId;
        [DataMember]
        public readonly List<CartItem> Cart;

        public Order(int customerId, List<CartItem> cart)
        {
            CustomerId = customerId;
            Cart = cart;
        }
    }
}