using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class RPC_Manager : NetworkBehaviour
{
    public static RPC_Manager Instance => _instance;
    private static RPC_Manager _instance;

    public int RandomSeed { get; private set; }
    
    private void Awake()
    {
        _instance = this;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SetRandomSeed(int randomSeed)
    {
        Debug.Log(randomSeed);
        Random.InitState(randomSeed);
        RandomSeed = randomSeed;
    }
}
