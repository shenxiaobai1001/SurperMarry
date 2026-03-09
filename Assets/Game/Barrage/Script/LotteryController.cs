using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LotteryController : MonoBehaviour
{
    public List<LotteryItem> lotteryItems = new List<LotteryItem>();
    public Image avatarImage;
    public GameObject lotteryTitle;
    public GameObject successFx;

    public AudioClip winAudio;

    // 每个奖项的权重（与 lotteryItems 一一对应，值来自 LotteryItemSetting.count）
    [HideInInspector]
    public List<int> weights = new List<int>();

    // 抽中结果回调（返回 itemName/callName）
    public System.Action<string> OnResult;

    private AudioSource audioSource;

    private bool isRot = true;
    private bool isRandom = false;

    private int GetWeightedRandomIndex()
    {
        if (lotteryItems == null || lotteryItems.Count == 0) return 0;
        if (weights == null || weights.Count != lotteryItems.Count)
        {
            // 未配置权重时退化为均匀随机
            return Random.Range(0, lotteryItems.Count);
        }

        int total = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            total += Mathf.Max(0, weights[i]);
        }
        if (total <= 0) return Random.Range(0, lotteryItems.Count);

        int r = Random.Range(0, total);
        int acc = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            acc += Mathf.Max(0, weights[i]);
            if (r < acc) return i;
        }
        return weights.Count - 1;
    }

    private void Awake()
    {
        Initialized();
    }


    void Update()
    {

    }

    private void Initialized()
    {
        transform.root.GetComponent<Canvas>().worldCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        lotteryItems.Clear();

        foreach (Transform child in transform)
        {
            LotteryItem item = child.GetComponent<LotteryItem>();

            if (item != null)
            {
                lotteryItems.Add(item);
            }
            else
            {
                Debug.LogWarning($"子物体 {child.name} 没有 LotteryItem 组件");
            }
        }
    }

    private IEnumerator LoadAvatarToImage(string avatarPath)
    {
        if (avatarImage == null) yield break;
        if (string.IsNullOrEmpty(avatarPath)) yield break;

        string url = avatarPath;
        // 兼容本地路径：UnityWebRequestTexture 需要 file:///
        if (!avatarPath.StartsWith("http://") && !avatarPath.StartsWith("https://") && !avatarPath.StartsWith("file://"))
        {
            url = "file:///" + avatarPath.Replace("\\", "/");
        }

        using (var req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"加载头像失败: {avatarPath} ({req.error})");
                yield break;
            }

            var tex = DownloadHandlerTexture.GetContent(req);
            if (tex == null) yield break;

            var rect = new Rect(0, 0, tex.width, tex.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var sprite = Sprite.Create(tex, rect, pivot);
            avatarImage.sprite = sprite;
            avatarImage.enabled = true;
        }
    }

    public IEnumerator LotteryStart(string avatarPath)
    {

        if (!string.IsNullOrEmpty(avatarPath))
        {
            yield return StartCoroutine(LoadAvatarToImage(avatarPath));
        }

        audioSource.Play();
        int index = Random.Range(0, lotteryItems.Count);
        while (isRot)
        {
            index++;
            if (index == lotteryItems.Count) index = 0;
            lotteryItems[index].SelectedOn();
            yield return new WaitForSeconds(0.05f);


            if (audioSource.time >= 4.0f)
            {
                isRot = false;
                lotteryItems[index].StartCoroutine(lotteryItems[index].FlickerOn());
                yield return new WaitForSeconds(2.5f);
                lotteryItems[index].StopFlicker();
                isRandom = true;
            }
        }

        while(isRandom)
        {
            // 随机滚动阶段仍然随便跳，营造动画效果
            int tmp = Random.Range(0, lotteryItems.Count);
            lotteryItems[tmp].SelectedOn();
            yield return new WaitForSeconds(0.05f);

            if (audioSource.time >= 8.5f)
            {
                isRandom = false;
                // 最终中奖：按权重抽取
                int winIndex = GetWeightedRandomIndex();
                lotteryItems[winIndex].SelectedOn();
                lotteryItems[winIndex].StartCoroutine(lotteryItems[winIndex].FlickerOn());
                yield return new WaitForSeconds(0.8f);
                // 生成特效
                GameObject fx = Instantiate(successFx, transform.root);
                Destroy(fx, 4f);

                lotteryItems[winIndex].StopFlicker();
                audioSource.clip = winAudio;
                audioSource.Play();
                Debug.Log($"抽中功能: {lotteryItems[winIndex].itemName}");
                CreateTitle(lotteryItems[winIndex].itemName);
                OnResult?.Invoke(lotteryItems[winIndex].itemName);
                yield return new WaitForSeconds(4f);
                Destroy(gameObject.transform.root.gameObject);
            }
        }




    }

    public void CreateTitle(string text)
    {
        GameObject obj = Instantiate(lotteryTitle, GameObject.Find("Canvas").transform);
        obj.GetComponent<Text>().text = text;
        Destroy(obj, 4f);
    }
}
