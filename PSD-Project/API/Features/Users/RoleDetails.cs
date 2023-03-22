using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract(Name = "Role")]
    public class RoleDetails
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Name;

        public RoleDetails(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}