using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [DataContract]
    public class TransactionEntry
    {
        [DataMember] public readonly int RamenId;
        [DataMember] public readonly int Quantity;

        public TransactionEntry(int ramenId, int quantity)
        {
            RamenId = ramenId;
            Quantity = quantity;
        }
    }
}