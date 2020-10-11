using System;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void OnComponent<ComponentT>(this GameObject obj, Action<ComponentT> callback) where ComponentT : MonoBehaviour
    {
        ComponentT comp = obj.GetComponent<ComponentT>();
        if (comp != null)
        {
            callback(comp);
        }
    }

    public static bool IfOnComponent<ComponentT>(this GameObject obj, Action<ComponentT> callback) where ComponentT : MonoBehaviour
    {
        ComponentT comp = obj.GetComponent<ComponentT>();
        if (comp != null)
        {
            callback(comp);
            return true;
        }
        else
        {
            return false;
        }
    }
}
