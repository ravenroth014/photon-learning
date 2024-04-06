using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private GameObject _bodyGameObject;
    [SerializeField] private PerkSettingObject _perkSetting;

    [Networked] private TickTimer delay { get; set; }
    [Networked] private TickTimer respawnTime { get; set; }
    [Networked] private NetworkBool isDead { get; set; }
    [Networked] public NetworkBool spawnedProjectile { get; set; }
    [Networked,OnChangedRender(nameof(OnChange))] private int _count { get; set; } = 0;
    
    [Networked] private int PerkIndex { get; set; } = -1;
    [Networked] private int RandomNo { get; set; } = -1;
    [Networked] private PerkData PerkData { get; set; }

    private Material _material;
    private TextMeshProUGUI _message;
    private ChangeDetector _changeDetector;
    private NetworkCharacterController _cc;
    private Vector3 _forward;

    private List<int> _dummyList = new();

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

            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, OnBeforeSpawnBall);
                    spawnedProjectile = !spawnedProjectile;
                }

                if (data.buttons.IsSet(NetworkInputData.KEY_X))
                {
                    // PerkIndex = Random.Range(0, _perkSetting.PerkSettingList.Count);
                    // RandomNo = Random.Range(0, 1000);

                    int updateRuleSet = PerkData.PerkTree1;
                    updateRuleSet |= 1 << Random.Range(0, 11);

                    PerkData = new PerkData(updateRuleSet);
                }
            }

            // if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            // {
            //     _count++;
            // }
        }

        if (isDead && respawnTime.ExpiredOrNotRunning(Runner))
        {
            //if (HasStateAuthority)
            //    RPC_SetObjectState(true);
            isDead = false;
        }

        // foreach (string changeProperty in _changeDetector.DetectChanges(this, out NetworkBehaviourBuffer previousBuffer, out NetworkBehaviourBuffer currentBuffer))
        // {
        //     OnLogicChangeDetected(changeProperty, previousBuffer, currentBuffer);
        // }
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
        foreach (var change in _changeDetector.DetectChanges(this, out NetworkBehaviourBuffer previousBuffer, out NetworkBehaviourBuffer currentBuffer))
        {
            switch (change)
            {
                case nameof(spawnedProjectile):
                    _material.color = Color.white;
                    break;
                case nameof(isDead):
                    {
                        SetObjectState(!isDead);
                        break;
                    }
                case nameof(PerkData):
                {
                    if (_message == null)
                    {
                        _message = UIManager.Instance.OutputText;
                    }

                    _message.text = Convert.ToString(PerkData.PerkTree1, 2);
                    
                    break;
                }
                // case nameof(PerkIndex):
                // {
                //     if (_message == null)
                //     {
                //         _message = UIManager.Instance.OutputText;
                //     }
                //    
                //     Random.InitState(RPC_Manager.Instance.RandomSeed);
                //     string message = Random.Range(0, 1000).ToString();
                //     _message.text = message;
                //     
                //     break;
                // }
            }
        }

        _material.color = Color.Lerp(_material.color, Color.blue, Time.deltaTime);
    }

    public void OnTakeDamage()
    {
        respawnTime = TickTimer.CreateFromSeconds(Runner, 2f);
        //if (HasStateAuthority)
        //    RPC_SetObjectState(false);
        isDead = true;
    }

    private void SetObjectState(bool state)
    {
        _bodyGameObject.SetActive(state);
    }

    private void OnBeforeSpawnBall(NetworkRunner runner, NetworkObject networkObject)
    {
        // Initialize the Ball before synchronizing it.
        networkObject.GetComponent<Ball>().Init(this);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SetObjectState(bool state)
    {
        if (_bodyGameObject)
            _bodyGameObject.SetActive(state);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (_message == null)
        {
            _message = UIManager.Instance.OutputText;
        }

        string dummyTest = _count <= 0 ? "NONE" : _dummyList[^1].ToString();

        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You said: {message}, dummy latest no. : {dummyTest}\n";
        }
        else
        {
            message = $"Some other player said: {message}, dummy latest no. : {dummyTest}\n";
        }

        _message.text = message;
    }

    private void OnLogicChangeDetected(string changeProperties, NetworkBehaviourBuffer previousBuffer, NetworkBehaviourBuffer currentBuffer)
    {
        Debug.Log($"{Object.InputAuthority.ToString()}");
        
        switch (changeProperties)
        {
            case nameof(_count):
            {
                PropertyReader<int> reader = GetPropertyReader<int>(changeProperties);
                (int previous, int current) = reader.Read(previousBuffer, currentBuffer);
                
                if (previous < current)
                {
                    int randNumber = Random.Range(0, 99);
                    _dummyList.Add(randNumber);
                }
            }
                break;
        }
    }

    private void OnChange()
    {
        int randNumber = Random.Range(0, 99);
        _dummyList.Add(randNumber);
    }
}
