using UnityEngine;

public static class Extension
{
    public static bool HasComponent<T>(this GameObject currentObject)
    {
        return currentObject.GetComponent<T>() != null;
    }
}
