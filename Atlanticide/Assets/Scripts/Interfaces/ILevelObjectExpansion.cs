using UnityEngine;

/// <summary>
/// Interface for scripts that expand level objects.
/// </summary>
public interface ILevelObjectExpansion
{
    void OnObjectUpdated();
    void OnObjectDestroyed();
    void OnObjectReset();
}
