using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte KEY_X = 10;

    public NetworkButtons buttons;
    public Vector3 direction;
}
