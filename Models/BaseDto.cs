using DevSim.Enums;
using System.Runtime.Serialization;

namespace DevSim.Models
{
    [DataContract]
    public class BaseDto
    {
        [DataMember(Name = "DtoType")]
        public virtual BaseDtoType DtoType { get; init; }
    }
}
