using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [DataContract]
    public class CartItem
    {
        [DataMember]
        public readonly int RamenId;
        [DataMember]
        public readonly int Quantity;

        public CartItem(int ramenId, int quantity)
        {
            RamenId = ramenId;
            Quantity = quantity;
        }
    }
}