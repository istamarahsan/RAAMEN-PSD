using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract]
    public class Role
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Name;

        public Role(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}