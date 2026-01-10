// SceneLauncher.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    [SerializeField] private string startSceneName = "MainMenu"; // 设置你想要启动的场景名

    private static bool hasLoadedStartScene = false;

    private void Awake()
    {
        // 确保只执行一次
        if (!hasLoadedStartScene && SceneManager.GetActiveScene().name != startSceneName)
        {
            hasLoadedStartScene = true;

            // 卸载当前场景，加载启动场景
            SceneManager.LoadScene(startSceneName);

            // 或者使用异步加载
            // SceneManager.LoadSceneAsync(startSceneName);
        }
    }
}