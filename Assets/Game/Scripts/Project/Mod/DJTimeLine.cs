using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJTimeLine 
{
    // 卡点数据字典，key为视频编号，value为卡点时间列表（单位：秒）
    public static Dictionary<int, List<float>> beatMapData = new Dictionary<int, List<float>>();

    // 初始化所有视频的卡点数据
    public static void InitializeBeatMapData()
    {
        // 视频1的卡点数据（具体时间点）
        beatMapData[1] = new List<float>
        {
            10f, 10.21f, 11.04f, 11.17f, 11.29f, 12.12f, 12.25f, 13.21f, 14.06f, 14.18f,
            15.00f, 15.13f, 15.26f, 16.09f, 17.05f, 17.18f, 18.13f, 18.27f, 19.10f, 19.23f,
            20.18f, 21.14f, 21.28f, 22.20f, 23.07f, 30.01f, 30.15f, 30.27f, 31.10f, 31.23f,
            32.05f, 32.18f, 33.00f, 33.13f, 33.26f, 34.10f, 34.23f, 35.07f, 35.20f, 36.04f,
            36.17f, 36.29f, 37.12f, 37.24f, 38.06f, 38.19f, 39.02f, 39.18f, 40.04f, 40.20f,
            41.06f, 41.22f, 42.08f, 42.24f
        };

        // 视频2的卡点数据
        beatMapData[2] = new List<float>();
        // 2秒-5秒9次（间隔0.33秒）
        for (int i = 0; i <= 12; i++) beatMapData[2].Add(2f + i * 0.33f);
        // 5秒-8秒8次（间隔0.375秒）
        for (int i = 0; i <= 8; i++) beatMapData[2].Add(5f + i * 0.375f);
        // 11秒-15秒9次（间隔0.444秒）
        for (int i = 0; i <= 9; i++) beatMapData[2].Add(11f + i * 0.444f);
        // 15秒-18秒8次（间隔0.375秒）
        for (int i = 0; i <= 8; i++) beatMapData[2].Add(15f + i * 0.375f);
        // 具体时间点
        beatMapData[2].Add(18.16f);
        beatMapData[2].Add(19.13f);
        // 21秒-23秒5次（间隔0.4秒）
        for (int i = 0; i <= 5; i++) beatMapData[2].Add(21f + i * 0.4f);
        // 24秒-26秒7次（间隔0.2857秒）
        for (int i = 0; i <= 7; i++) beatMapData[2].Add(24f + i * 0.2857f);
        // 27秒-30秒5次（间隔0.6秒）
        for (int i = 0; i <= 5; i++) beatMapData[2].Add(27f + i * 0.6f);
        // 31秒-35秒14次（间隔0.2857秒）
        for (int i = 0; i <= 14; i++) beatMapData[2].Add(31f + i * 0.2857f);
        // 35秒-36秒3次（间隔0.33秒）
        for (int i = 0; i <= 3; i++) beatMapData[2].Add(35f + i * 0.33f);

        // 视频3的卡点数据
        beatMapData[3] = new List<float>();
        beatMapData[3].Add(0.28f);
        // 3秒-9秒11次（间隔0.545秒）
        for (int i = 0; i <= 11; i++) beatMapData[3].Add(3f + i * 0.545f);
        beatMapData[3].Add(10.04f);
        beatMapData[3].Add(12.15f);
        // 15秒-22秒14次（间隔0.5秒）
        for (int i = 0; i <= 14; i++) beatMapData[3].Add(15f + i * 0.5f);
        // 23秒-28秒14次（间隔0.357秒）
        for (int i = 0; i <= 14; i++) beatMapData[3].Add(23f + i * 0.357f);
        // 29秒-34秒14次（间隔0.357秒）
        for (int i = 0; i <= 14; i++) beatMapData[3].Add(29f + i * 0.357f);
        // 34秒-38秒8次（间隔0.5秒）
        for (int i = 0; i <= 8; i++) beatMapData[3].Add(34f + i * 0.5f);
        beatMapData[3].Add(41.05f);

        // 视频4的卡点数据（具体时间点）
        beatMapData[4] = new List<float>
        {
            5.05f, 5.23f, 6.11f, 6.28f, 7.16f, 8.03f, 8.22f, 9.09f, 9.27f, 10.15f,
            17.27f, 18.15f, 19.03f, 19.22f, 20.11f, 20.28f, 21.17f, 22.05f, 22.23f, 23.10f,
            23.28f, 24.16f, 25.04f, 27.14f, 28.02f, 28.21f, 29.09f, 29.27f, 30.15f, 31.03f,
            31.26f, 31.26f, 32.18f, 33.05f, 33.23f, 34.11f, 34.28f, 35.15f, 36.03f, 36.21f,
            37.08f, 37.26f, 38.15f, 39.02f, 46.21f, 47.08f, 47.27f, 48.14f, 49.03f, 49.21f,
            50.08f, 50.26f, 51.14f, 52.03f, 52.20f, 53.08f, 53.25f, 54.13f, 55.02f, 55.19f,
            56.07f, 56.25f, 57.13f, 58.00f, 58.18f, 59.07f, 59.25f, 60.13f, 61.00f, 61.17f,
            62.05f, 62.22f
        };

        // 视频5的卡点数据
        beatMapData[5] = new List<float>();
        // 12秒-15秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[5].Add(12f + i * 0.375f);
        // 15秒-18秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[5].Add(15f + i * 0.375f);
        // 19秒-22秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[5].Add(19f + i * 0.375f);
        // 22秒-25秒5次（间隔0.5秒）
        for (int i = 0; i <= 5; i++) beatMapData[5].Add(22f + i * 0.5f);
        // 29秒-32秒5次（间隔0.5秒）
        for (int i = 0; i <= 5; i++) beatMapData[5].Add(29f + i * 0.5f);
        // 32秒-35秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[5].Add(32f + i * 0.375f);
        // 36秒-39秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[5].Add(36f + i * 0.375f);
        // 39秒-42秒5次（间隔0.5秒）
        for (int i = 0; i <= 5; i++) beatMapData[5].Add(39f + i * 0.5f);

        // 视频6的卡点数据
        beatMapData[6] = new List<float>();
        // 5-9秒7次（间隔0.5秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(5f + i * 0.5f);
        // 9-13秒9次（间隔0.4秒）
        for (int i = 0; i <= 9; i++) beatMapData[6].Add(9f + i * 0.4f);
        // 13-17秒7次（间隔0.5秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(13f + i * 0.5f);
        // 17-20秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(17f + i * 0.375f);
        // 21-24秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(21f + i * 0.375f);
        // 24-27秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(24f + i * 0.375f);
        // 28-31秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(28f + i * 0.375f);
        // 32-35秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(32f + i * 0.375f);
        // 36-40秒7次（间隔0.5秒）
        for (int i = 0; i <= 7; i++) beatMapData[6].Add(36f + i * 0.5f);

        // 视频7的卡点数据
        beatMapData[7] = new List<float>();
        // 1-4秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[7].Add(1f + i * 0.375f);
        // 4-8秒7次（间隔0.5秒）
        for (int i = 0; i <= 7; i++) beatMapData[7].Add(4f + i * 0.5f);
        // 8-11秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[7].Add(8f + i * 0.375f);
        // 12-14秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[7].Add(12f + i * 0.333f);
        // 15-18秒8次（间隔0.333秒）
        for (int i = 0; i <= 8; i++) beatMapData[7].Add(15f + i * 0.333f);
        // 19-22秒8次（间隔0.333秒）
        for (int i = 0; i <= 8; i++) beatMapData[7].Add(19f + i * 0.333f);
        // 22-26秒8次（间隔0.444秒）
        for (int i = 0; i <= 8; i++) beatMapData[7].Add(22f + i * 0.444f);
        // 26-28秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[7].Add(26f + i * 0.333f);

        // 视频8的卡点数据
        beatMapData[8] = new List<float>();
        // 7-11秒7次（间隔0.5秒）
        for (int i = 0; i <= 7; i++) beatMapData[8].Add(7f + i * 0.5f);
        // 12-13秒4次（间隔0.2秒）
        for (int i = 0; i <= 4; i++) beatMapData[8].Add(12f + i * 0.2f);
        // 14-15秒4次（间隔0.2秒）
        for (int i = 0; i <= 4; i++) beatMapData[8].Add(14f + i * 0.2f);
        // 24-26秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[8].Add(24f + i * 0.4f);
        // 26-28秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[8].Add(26f + i * 0.4f);
        // 28-32秒10次（间隔0.363秒）
        for (int i = 0; i <= 10; i++) beatMapData[8].Add(28f + i * 0.363f);

        // 视频9的卡点数据
        beatMapData[9] = new List<float>();
        // 1-4秒9次（间隔0.3秒）
        for (int i = 0; i <= 9; i++) beatMapData[9].Add(1f + i * 0.3f);
        // 8-9秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[9].Add(8f + i * 0.25f);
        // 8-14秒6次（间隔0.857秒）
        for (int i = 0; i <= 6; i++) beatMapData[9].Add(8f + i * 0.857f);
        // 14-16秒3次（间隔0.5秒）
        for (int i = 0; i <= 3; i++) beatMapData[9].Add(14f + i * 0.5f);
        // 20-30秒29次（间隔0.333秒）
        for (int i = 0; i <= 29; i++) beatMapData[9].Add(20f + i * 0.333f);
        // 30-32秒3次（间隔0.5秒）
        for (int i = 0; i <= 3; i++) beatMapData[9].Add(30f + i * 0.5f);
        // 33-44秒28次（间隔0.379秒）
        for (int i = 0; i <= 28; i++) beatMapData[9].Add(33f + i * 0.379f);
        // 45-57秒31次（间隔0.375秒）
        for (int i = 0; i <= 31; i++) beatMapData[9].Add(45f + i * 0.375f);

        // 视频10的卡点数据
        beatMapData[10] = new List<float>();
        // 3-8秒14次（间隔0.333秒）
        for (int i = 0; i <= 14; i++) beatMapData[10].Add(3f + i * 0.333f);
        // 10-12秒7次（间隔0.25秒）
        for (int i = 0; i <= 7; i++) beatMapData[10].Add(10f + i * 0.25f);
        // 13-16秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[10].Add(13f + i * 0.375f);
        // 16-17秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(16f + i * 0.25f);
        // 18-19秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(18f + i * 0.25f);
        // 19-25秒15次（间隔0.357秒）
        for (int i = 0; i <= 15; i++) beatMapData[10].Add(19f + i * 0.357f);
        // 25-28秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[10].Add(25f + i * 0.375f);
        // 31-32秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(31f + i * 0.25f);
        // 32-33秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(32f + i * 0.25f);
        // 34-37秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[10].Add(34f + i * 0.375f);
        // 37-38秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(37f + i * 0.25f);
        // 38-39秒3次（间隔0.25秒）
        for (int i = 0; i <= 3; i++) beatMapData[10].Add(38f + i * 0.25f);
        // 40-43秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[10].Add(40f + i * 0.375f);

        // 视频11的卡点数据
        beatMapData[11] = new List<float>();
        // 0-2秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(0f + i * 0.4f);
        // 2-4秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(2f + i * 0.4f);
        // 5-7秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(5f + i * 0.4f);
        // 7-10秒4次（间隔0.6秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(7f + i * 0.6f);
        // 10-12秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(10f + i * 0.4f);
        // 13-15秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(13f + i * 0.4f);
        // 15-17秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(15f + i * 0.4f);
        // 18-20秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[11].Add(18f + i * 0.4f);
        // 20-22秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(20f + i * 0.333f);
        // 23-25秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(23f + i * 0.333f);
        // 25-27秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(25f + i * 0.333f);
        // 28-30秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(28f + i * 0.333f);
        // 30-33秒5次（间隔0.5秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(30f + i * 0.5f);
        // 33-35秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(33f + i * 0.333f);
        // 35-37秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[11].Add(35f + i * 0.333f);
        // 38-41秒7次（间隔0.375秒）
        for (int i = 0; i <= 7; i++) beatMapData[11].Add(38f + i * 0.375f);

        // 视频12的卡点数据
        beatMapData[12] = new List<float>();
        // 0-2秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[12].Add(0f + i * 0.4f);
        // 2-4秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[12].Add(2f + i * 0.4f);
        // 5-7秒4次（间隔0.4秒）
        for (int i = 0; i <= 4; i++) beatMapData[12].Add(5f + i * 0.4f);
        // 7-9秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(7f + i * 0.333f);
        // 10-12秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(10f + i * 0.333f);
        // 12-15秒6次（间隔0.4秒）
        for (int i = 0; i <= 6; i++) beatMapData[12].Add(12f + i * 0.4f);
        // 15-17秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(15f + i * 0.333f);
        // 17-19秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(17f + i * 0.333f);
        // 20-22秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(20f + i * 0.333f);
        // 22-25秒5次（间隔0.5秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(22f + i * 0.5f);
        // 25-27秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(25f + i * 0.333f);
        // 27-29秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(27f + i * 0.333f);
        // 30-32秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(30f + i * 0.333f);
        // 32-34秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(32f + i * 0.333f);
        // 35-37秒5次（间隔0.333秒）
        for (int i = 0; i <= 5; i++) beatMapData[12].Add(35f + i * 0.333f);
        // 38-40秒7次（间隔0.25秒）
        for (int i = 0; i <= 7; i++) beatMapData[12].Add(38f + i * 0.25f);
    }
}
