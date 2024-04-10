using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _rpcPrefab;
    
    private readonly Dictionary<PlayerRef, NetworkObject> _spawnCharacters = new();

    private NetworkRunner _runner;
    private bool _mouseButton0;
    private bool _keyXButton;
    private bool _keyPButton;

    private int _randomSeed;

    private Action _onServerShutdown;

    private void Update()
    {
        _mouseButton0 |= Input.GetMouseButton(0);
        _keyXButton |= Input.GetKeyDown(KeyCode.X);
        _keyPButton |= Input.GetKeyDown(KeyCode.P);
    }
    
    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        _randomSeed = Random.Range(0, 10000);

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode
            , SessionName = "TestRoom"
            , Scene = scene
            , SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void LeaveGame()
    {
        if (_runner != null)
        {
            _runner.Shutdown();
            _runner = null;
        }
    }

    private void OnGUI()
    {
        if (_runner == null || _runner.IsRunning == false)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }

            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }

        if (_runner != null && _runner.IsRunning)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Leave"))
            {
                LeaveGame();
            }
        }
    }

    public void SetOnShutDownCallback(Action callback)
    {
        _onServerShutdown = callback;
    }
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (_runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 1.1f, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player,
                (networkRunner, obj) =>
                {
                    var script = obj.GetComponent<Player>();
                    script.Init(player);
                } );
            
            // Keep track of the player avatars for easy access
            _spawnCharacters.Add(player, networkPlayerObject);

            if (RPC_Manager.Instance != null)
            {
                Debug.Log("WORK!!!");
                RPC_Manager.Instance.RPC_SetRandomSeed(_randomSeed);
            }
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        data.buttons.Set(NetworkInputData.KEY_X, _keyXButton);
        data.buttons.Set(NetworkInputData.KEY_P, _keyPButton);
        
        _mouseButton0 = false;
        _keyXButton = false;
        _keyPButton = false;
        
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        _onServerShutdown?.Invoke();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Test 1");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
