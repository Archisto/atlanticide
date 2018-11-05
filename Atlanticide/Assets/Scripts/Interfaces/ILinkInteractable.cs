using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Interface for objects that can be interacted with the link beam.
    /// </summary>
    public interface ILinkInteractable
    {
        bool TryInteract(LinkBeam linkBeam);
        bool TryInteractInstant(LinkBeam linkBeam);
        bool GivePulse(LinkBeam linkBeam, float speedModifier);
    }
}
