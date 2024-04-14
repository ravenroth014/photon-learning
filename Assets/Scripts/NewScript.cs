using Fusion;
using UnityEngine;

public class NewScript : NetworkBehaviour
{
    private void Start()
    {
        string test = string.Empty;
        string test2 = string.Empty;
        
        switch (test2)
        {
            case "word1":
            {
                Debug.Log("word1");
                break;
            }
            case "word2":
            {
                Debug.Log("word2");
                break;
            }
        }
    }
}
