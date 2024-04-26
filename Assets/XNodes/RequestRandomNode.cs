using UnityEngine;
using XNode;

public class RequestRandomNode : Node
{
    [Input] public RequestRandomNode Input;

    public string RandomKey;
    public int MinValue;
    public int MaxValue;

    private RequestRandomLogic _logic;
    
    public void Init(Player player)
    {
        _logic = new RequestRandomLogic(player, RandomKey, MinValue, MaxValue);
    }

    public void RequestRandomValue()
    {
        _logic?.GenerateRequest();
    }
    
    // Use this for initialization
    protected override void Init() {
        base.Init();
        VerifyConnections();
    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port) {
        return null; // Replace this
    }
}

public class RequestRandomLogic
{
    private Player Player { get; }
    private string RandomKey { get; }
    private int MinValue { get; }
    private int MaxValue { get; }

    private int _resultValue;
    
    public RequestRandomLogic(Player player, string randomKey, int minValue, int maxValue)
    {
        Player = player;
        RandomKey = randomKey;
        MinValue = minValue;
        MaxValue = maxValue;
        
        Player.RegisRandomLogic(RandomKey, this);
    }

    public void GenerateRequest()
    {
        RandomIntRequest request = new()
        {
            IsRequesting = true,
            IsObtainedRandomValue = false,
            MinValue = MinValue,
            MaxValue = MaxValue,
            ResultValue = MinValue - 1
        };

        Player.RequestRandomNumber(RandomKey, request);
    }

    public void ReceiveRandomValue(int randomValue)
    {
        _resultValue = randomValue;
        
        Debug.Log($"Random result of {Player.Object.Runner.LocalPlayer}'s Random Key {RandomKey}: {_resultValue}");
    }
}