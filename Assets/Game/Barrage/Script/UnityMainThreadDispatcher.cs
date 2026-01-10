using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly ConcurrentQueue<System.Action> _actions = new ConcurrentQueue<System.Action>();

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UnityMainThreadDispatcher>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MainThreadDispatcher");
                    _instance = go.AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // 确保在主线程、加载场景前创建实例，避免后台线程首次访问时触发 FindObjectOfType 警告/异常
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        var _ = Instance;
    }

    void Update()
    {
        while (_actions.TryDequeue(out System.Action action))
        {
            action?.Invoke();
        }
    }

    public void Enqueue(System.Action action)
    {
        _actions.Enqueue(action);
    }
}