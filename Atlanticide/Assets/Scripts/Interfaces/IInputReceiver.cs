using UnityEngine;

/// <summary>
/// An interface for objects that can receive player input.
/// </summary>
public interface IInputReceiver
{
    void MoveInput(Vector3 direction);
    void LookInput(Vector3 direction);
    void ActionInput(bool active);
}
