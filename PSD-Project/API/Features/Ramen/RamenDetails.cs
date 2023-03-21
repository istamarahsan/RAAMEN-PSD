using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Ramen
{
    [DataContract]
    public class RamenDetails
    {
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Borth;
        [DataMember] 
        public readonly string Price;
        [DataMember] 
        public readonly int MeatId;

        public RamenDetails(string name, string borth, string price, int meatId)
        {
            Name = name;
            Borth = borth;
            Price = price;
            MeatId = meatId;
        }
    }
}