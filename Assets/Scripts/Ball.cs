using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float defaultImpactDistance = 0.25f;
    [Networked] private TickTimer life { get; set; }
    private Collider[] _areaHits = new Collider[4];
    private Player _ownerPlayer;

    public override void FixedUpdateNetwork()
    {
        transform.position += 5 * transform.forward * Runner.DeltaTime;
        
        if (CheckOnHit(Runner, transform.forward) || life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    public void Init(Player owner)
    {
        _ownerPlayer = owner;
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    private bool CheckOnHit(NetworkRunner runner, Vector3 velocity)
    {
        bool impact = runner.LagCompensation.Raycast(transform.position, velocity, 0.5f, Object.InputAuthority, out var hitInfo, -1, HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX);
        Debug.Log($"Hit target : {impact}");

        if (impact)
        {
            float distance = Vector3.Distance(transform.position, hitInfo.Point);
            if (distance <= defaultImpactDistance)
            {
                ApplyAreaDamage(hitInfo.Point);
                return true;
            }

            return false;
        }

        return false;
    }

    private void ApplyAreaDamage(Vector3 hitPoint)
    {
        int count = Physics.OverlapSphereNonAlloc(hitPoint, 0.5f, _areaHits);

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
