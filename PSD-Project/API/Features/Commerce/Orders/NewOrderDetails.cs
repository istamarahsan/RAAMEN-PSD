using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [DataContract]
    public class NewOrderDetails
    {
        [DataMember]
        public readonly int CustomerId;
        [DataMember]
        public readonly List<CartItem> Cart;

        public NewOrderDetails(int customerId, List<CartItem> cart)
        {
            CustomerId = customerId;
            Cart = cart;
        }
    }
}