using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => _instance;
    private static UIManager _instance;

    public TextMeshProUGUI OutputText => _outputText;
    [SerializeField] private TextMeshProUGUI _outputText;

    public TextMeshProUGUI Output2Text => _output2Text;
    [SerializeField] private TextMeshProUGUI _output2Text;
    
    public TextMeshProUGUI Output3Text => _output3Text;
    [SerializeField] private TextMeshProUGUI _output3Text;

    private void Awake()
    {
        _instance = this;
    }
}
