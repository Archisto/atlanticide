using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Interface for objects that can be targeted with the link beam.
    /// </summary>
    public interface ILinkTarget
    {
        GameObject LinkObject { get; set; }
        LinkBeam LinkedLinkBeam { get; set; }
        bool IsLinkTarget { get; set; }
    }
}
