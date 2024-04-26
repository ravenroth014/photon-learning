using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _serverObject;
    [SerializeField] private TestEnum _testEnum;
    
    public static GameManager Instance => _instance;
    private static GameManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        GenerateServerObject();
        IterateEnum();
    }

    private void IterateEnum()
    {
        int count = Enum.GetValues(typeof(TestEnum)).Length;

        for (int index = 0; index < count; index++)
        {
            UIManager.Instance.Output3Text.text += ((TestEnum)index).ToString();
        }
    }

    private void GenerateServerObject()
    {
        if (!_serverObject) return;

        GameObject serverObject = Instantiate(_serverObject);
        BasicSpawner serverScript = serverObject.GetComponent<BasicSpawner>();
        
        serverScript.SetOnShutDownCallback(OnServerShutdown);
    }

    private void OnServerShutdown()
    {
        GenerateServerObject();
    }
}
