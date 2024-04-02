using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    
    public override void FixedUpdateNetwork()
    {
        transform.position += 5 * transform.forward * Runner.DeltaTime;
        
        if (CheckOnHit(Runner, transform.forward) || life.Expired(Runner))
            Runner.Despawn(Object);
    }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    private bool CheckOnHit(NetworkRunner runner, Vector3 velocity)
    {
        var impact = runner.LagCompensation.Raycast(transform.position, velocity, 1, Object.InputAuthority, out var hitinfo, -1, HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX);
        
        Debug.Log($"Hit target : {impact}");
        
        return impact;
    }
}
