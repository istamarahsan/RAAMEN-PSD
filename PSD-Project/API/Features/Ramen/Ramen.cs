using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Ramen
{
    [DataContract]
    public class Ramen
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Borth;
        [DataMember]
        public readonly string Price;
        [DataMember]
        public readonly Meat Meat;

        public Ramen(int id, string name, string borth, string price, Meat meat)
        {
            Id = id;
            Name = name;
            Borth = borth;
            Price = price;
            Meat = meat;
        }
    }
}