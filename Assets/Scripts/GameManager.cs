using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _serverObject;
    
    public static GameManager Instance => _instance;
    private static GameManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        GenerateServerObject();
    }

    private void GenerateServerObject()
    {
        if (_serverObject == null) return;

        GameObject serverObject = Instantiate(_serverObject);
        BasicSpawner serverScript = serverObject.GetComponent<BasicSpawner>();
        
        serverScript.SetOnShutDownCallback(OnServerShutdown);
    }

    private void OnServerShutdown()
    {
        GenerateServerObject();
    }
}
