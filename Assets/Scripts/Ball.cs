using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    private Collider[] _areaHits = new Collider[4];
    private Player _ownerPlayer;

    public override void FixedUpdateNetwork()
    {
        transform.position += 5 * transform.forward * Runner.DeltaTime;
        
        if (CheckOnHit(Runner, transform.forward) || life.Expired(Runner))
            Runner.Despawn(Object);
    }

    public void Init(Player owner)
    {
        _ownerPlayer = owner;
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    private bool CheckOnHit(NetworkRunner runner, Vector3 velocity)
    {
        var impact = runner.LagCompensation.Raycast(transform.position, velocity, 1, Object.InputAuthority, out var hitInfo, -1, HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX);
        Debug.Log($"Hit target : {impact}");

        if (impact)
        {
            ApplyAreaDamage(hitInfo.Point);
        }

        return impact;
    }

    private void ApplyAreaDamage(Vector3 hitPoint)
    {
        int count = Physics.OverlapSphereNonAlloc(hitPoint, 1, _areaHits);

        if (count > 0)
        {
            for (int index = 0; index < count; index++)
            {
                GameObject other = _areaHits[index].gameObject;
                if (other)
                {
                    Player target = other.GetComponent<Player>();
                    if (target != null && target != _ownerPlayer)
                    {
                        target.OnTakeDamage();
                    }
                }
            }
        }
    }
}
