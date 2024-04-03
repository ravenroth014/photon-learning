using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => _instance;
    private static UIManager _instance;

    public TextMeshProUGUI OutputText => _outputText;
    [SerializeField] private TextMeshProUGUI _outputText;

    private void Awake()
    {
        _instance = this;
    }
}
