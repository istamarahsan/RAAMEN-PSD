using System.Runtime.Serialization;

namespace PSD_Project.Features.Ramen
{
    [DataContract]
    public class Meat
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Name;

        public Meat(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}