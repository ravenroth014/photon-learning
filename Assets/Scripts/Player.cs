using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private GameObject _bodyGameObject;

    [Networked] private TickTimer delay { get; set; }
    [Networked] private TickTimer respawnTime { get; set; }
    [Networked] private NetworkBool isDead { get; set; }
    [Networked] public NetworkBool spawnedProjectile { get; set; }

    private Material _material;
    private TextMeshProUGUI _message;
    private ChangeDetector _changeDetector;
    private NetworkCharacterController _cc;
    private Vector3 _forward;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && isDead == false)
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (HasInputAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward),
                        Object.InputAuthority,
                        (runner, o) =>
                        {
                            // Initialize the Ball before synchronizing it.
                            o.GetComponent<Ball>().Init(this);
                        });
                    spawnedProjectile = !spawnedProjectile;
                }
            }
        }

        if (isDead && respawnTime.ExpiredOrNotRunning(Runner))
        {
            RPC_SetObjectState(true);
            isDead = false;
        }
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Testing");
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(spawnedProjectile):
                    _material.color = Color.white;
                    break;
            }
        }

        _material.color = Color.Lerp(_material.color, Color.blue, Time.deltaTime);
    }

    public void OnTakeDamage()
    {
        respawnTime = TickTimer.CreateFromSeconds(Runner, 2f);
        RPC_SetObjectState(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SetObjectState(bool state)
    {
        if (_bodyGameObject)
        {
            _bodyGameObject.SetActive(state);
            isDead = true;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (_message == null)
        {
            _message = UIManager.Instance.OutputText;
        }

        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player said: {message}\n";
        }

        _message.text += message;
    }
}
