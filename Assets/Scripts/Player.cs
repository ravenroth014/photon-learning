using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private GameObject _bodyGameObject;
    [SerializeField] private PerkSettingObject _perkSetting;
    [SerializeField] private GameObject _processObject;
    [SerializeField] private TestGraph _testGraph;

    [Networked] private TickTimer delay { get; set; }
    [Networked] private TickTimer respawnTime { get; set; }
    [Networked] private NetworkBool isDead { get; set; }
    [Networked] public NetworkBool spawnedProjectile { get; set; }
    [Networked,OnChangedRender(nameof(OnChange))] private int _count { get; set; } = 0;
    
    [Networked] private int PerkIndex { get; set; } = -1;
    [Networked] private int RandomNo { get; set; } = 0;
    [Networked] private PerkData PerkData { get; set; }
    [Networked, Capacity(19)] private NetworkArray<byte> PerkDataList => default;
    [Networked] private NetworkRNG test { get; set; }
    [Networked] private int NodeTask { get; set; } = 0;
    [Networked] private PlayerRef PlayerID { get; set; }
    [Networked, Capacity(16)] private NetworkArray<RandomIntRequest> RandomIntRequestList => default;
    
    private Material _material;
    private TextMeshProUGUI _message;
    private ChangeDetector _changeDetector;
    private NetworkCharacterController _cc;
    private Vector3 _forward;

    private List<int> _dummyList = new();

    private TestNodeObject _testNodeObject;
    private readonly Dictionary<string, RequestRandomLogic> _randomDict = new();
    private readonly List<string> _randomKeyList = new();

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        test = new NetworkRNG(Runner.Tick);
        _testNodeObject = _testGraph.Node.GetNodeObject(Runner, this);
        //CurrentTick = 0;
    }

    public void Init(PlayerRef playerRef)
    {
        PlayerID = playerRef;
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
                    _testNodeObject.SpawnBall(_forward, Object.InputAuthority, this);
                    // Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, OnBeforeSpawnBall);
                    spawnedProjectile = !spawnedProjectile;
                }

                if (data.buttons.IsSet(NetworkInputData.KEY_X))
                {
                    // PerkIndex = Random.Range(0, _perkSetting.PerkSettingList.Count);
                    // RandomNo = Random.Range(0, 1000);

                    int updateRuleSet = PerkData.PerkTree1;
                    updateRuleSet |= 1 << Random.Range(0, 11);

                    PerkData = new PerkData(updateRuleSet);

                    if (_processObject.HasComponent<NewScript>() == false)
                    {
                        _processObject.AddComponent<NewScript>();
                    }
                }

                
                
            }

            // if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            // {
            //     _count++;
            // }
            
            if (data.buttons.IsSet(NetworkInputData.KEY_P))
            {
                //_testGraph.Node.Execute(this);
                _testGraph.Node.ExecuteRandomRequest();
            }
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

        
        
        GenerateRandomNumber();
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Testing");
            //_testGraph.Node.ExecuteRandomRequest();
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
                case nameof(NodeTask):
                {
                    if (_message == null)
                    {
                        _message = UIManager.Instance.OutputText;
                    }

                    _message.text += $"Player : {PlayerID.ToString()}'s task is finished.\n";
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

        float renderTime = Object.IsProxy ? Runner.RemoteRenderTime : Runner.LocalRenderTime;
        float floatTick = renderTime / Runner.DeltaTime;

        UIManager.Instance.Output2Text.text = floatTick.ToString(CultureInfo.InvariantCulture);

        SendRandomValue();
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

    public void FinishExecuteTask()
    {
        NodeTask++;
    }

    public void RegisRandomLogic(string key, RequestRandomLogic logicNode)
    {
        if (_randomDict.TryAdd(key, logicNode) == false)
            _randomDict[key] = logicNode;
        if (_randomKeyList.Contains(key) == false)
            _randomKeyList.Add(key);
    }

    public void RequestRandomNumber(string randomKey, RandomIntRequest request)
    {
        int logicIndex = _randomKeyList.Contains(randomKey) ? _randomKeyList.IndexOf(randomKey) : -1;
        
        if (logicIndex == -1)
            return;
        
        int index = RandomNo % RandomIntRequestList.Length;
        request.LogicIndex = logicIndex;
        RandomIntRequestList.Set(index, request);
        RandomNo++;
    }

    private void GenerateRandomNumber()
    {
        if (Object.IsProxy) return;
        
        for (int index = 0; index < RandomIntRequestList.Length; index++)
        {
            var currentValue = RandomIntRequestList.Get(index);
            if (currentValue.IsRequesting)
            {
                currentValue.ResultValue = Random.Range(currentValue.MinValue, currentValue.MaxValue);
                currentValue.IsRequesting = false;
                currentValue.IsObtainedRandomValue = true;
                RandomIntRequestList.Set(index, currentValue);
            }
        }
    }

    private void SendRandomValue()
    {
        for (int index = 0; index < RandomIntRequestList.Length; index++)
        {
            var currentValue = RandomIntRequestList.Get(index);
            if (currentValue.IsObtainedRandomValue == false) continue;
            if (currentValue.LogicIndex <= -1) continue;

            string key = _randomKeyList[currentValue.LogicIndex];

            if (_randomDict.TryGetValue(key, out var value))
            {
                value.ReceiveRandomValue(currentValue.ResultValue);
                currentValue.IsObtainedRandomValue = false;
                currentValue.IsRequesting = false;
                currentValue.LogicIndex = -1;
                RandomIntRequestList.Set(index, currentValue);
            }
        }
    }
}

[Serializable]
public struct RandomIntRequest : INetworkStruct
{
    //public string RandomKey;
    public bool IsRequesting;
    public bool IsObtainedRandomValue;
    public int MinValue;
    public int MaxValue;
    public int ResultValue;
    public int LogicIndex;
}
