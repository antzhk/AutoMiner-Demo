using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static void Run(IEnumerator coroutine)
    {
        if (_instance == null)
        {
            var go = new GameObject("CoroutineRunner");
            go.hideFlags = HideFlags.HideAndDontSave;
            _instance = go.AddComponent<CoroutineRunner>();
            DontDestroyOnLoad(go);
        }

        _instance.StartCoroutine(coroutine);
    }
}