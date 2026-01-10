using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;



/// <summary>
/// 简易 WebSocket 服务器（只支持：文本帧、Ping/Pong、Close；不支持分片与超大帧）。
/// </summary>
public class WebSocketServer : MonoBehaviour
{
    private TcpListener _tcpListener;
    private Thread _listenerThread;
    private bool _isRunning = false;
    public int port = 8881;

    private readonly List<TcpClient> _clients = new List<TcpClient>();

    private List<string> followUser = new List<string>();
    [SerializeField] private BarrageBase barrageBase; // 通过场景引用或运行时查找，避免 new MonoBehaviour
    private UnityMainThreadDispatcher _dispatcher; // 缓存调度器，避免后台线程访问 Instance 触发查找

    private void HandleMessage(string json)
    {
        Debug.Log($"处理消息: {json}");
 
        BarrageData barrage = JsonUtility.FromJson<BarrageData>(json);
        Debug.Log($"类型: {barrage.Type}");
        if (barrageBase == null)
        {
            // 尝试补救性查找一次
            barrageBase = FindAnyObjectByType<BarrageBase>();
            if (barrageBase == null)
            {
                Debug.LogWarning("BarrageBase 未就绪，丢弃该条消息");
                return;
            }
        }
        switch (barrage.Type)
        {
            case "进场":
                Debug.Log($"{barrage.name} 进入了直播间");
                barrageBase.HandleJoin(json);
                break;
            case "关注":
                Debug.Log($"{barrage.name} 关注了主播");
                if (!followUser.Contains(barrage.name))
                {
                    followUser.Add(barrage.name);
                    barrageBase.HandleAttention(json);
                }
                break;
            case "聊天":
                Debug.Log($"{barrage.name} 说： {barrage.message}");
                barrageBase.HandleBarrage(json);
                break;
            case "点赞":
                Debug.Log($"{barrage.name} 点了 {barrage.count} 个赞");
                barrageBase.handleLike(json);
                break;
            case "礼物":
                Debug.Log($"{barrage.name} 送出了 {barrage.message} X {barrage.num}");
                barrageBase.HandleGift(json);
                break;
        }
    }

    void Start()
    {
        _dispatcher = UnityMainThreadDispatcher.Instance;
        // 若未在 Inspector 赋值，则尝试在场景中查找 BarrageBase
        if (barrageBase == null)
        {
            barrageBase = FindAnyObjectByType<BarrageBase>();
            if (barrageBase == null)
            {
                Debug.LogWarning("WebSocketServer: 未找到场景中的 BarrageBase，请在场景中挂载并在 Inspector 赋值");
            }
        }
        StartServer();
    }

    void StartServer()
    {
        if (_isRunning)
        {
            Debug.LogWarning("WebSocketServer: server already running.");
            return;
        }
        _listenerThread = new Thread(new ThreadStart(ListenForClients));
        _listenerThread.IsBackground = true;
        _listenerThread.Start();
    }

    private void ListenForClients()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _isRunning = true;
            Debug.Log($"WebSocket服务器启动，监听端口: {port}");

            while (_isRunning)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();
                lock (_clients)
                {
                    _clients.Add(client);
                }

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.IsBackground = true;
                clientThread.Start(client);

                Debug.Log("新客户端连接，准备握手");
            }
        }
        catch (Exception e)
        {
           Debug.LogError($"服务器监听异常: {e.Message}");
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        try
        {
            if (!PerformHandshake(stream, client))
            {
                Debug.LogWarning("握手失败，关闭连接");
                return;
            }
            Debug.Log("握手成功，开始接收数据（文本帧）");

            // 进入帧读取循环（简化版：只处理文本）
            while (client.Connected)
            {
                if (stream.DataAvailable)
                {
                    string message = ReadTextFrame(stream);
                    if (message == null)
                    {
                        // 客户端可能已发送关闭帧
                        break;
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        var disp = _dispatcher; // 本地引用防止并发更改
                        if (disp != null)
                        {
                            disp.Enqueue(() =>
                            {
                                HandleMessage(message);
                            });
                        }
                        else
                        {
                            Debug.LogWarning("主线程调度器未初始化，丢弃收到的消息。");
                        }
                    }
                    SendText(stream, "服务器收到: " + message);
                }
                Thread.Sleep(5);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"客户端处理异常: {e.Message}");
        }
        finally
        {
            lock (_clients)
            {
                _clients.Remove(client);
            }
            try { client.Close(); } catch { }
            Debug.Log("客户端断开连接");
        }
    }


    private bool PerformHandshake(NetworkStream stream, TcpClient client)
    {
        // 读取完整 HTTP 请求头，直到出现空行（\r\n\r\n）
        string request = ReadHttpHeaders(stream, 5000);
        if (string.IsNullOrEmpty(request)) return false;

        string key = null;
        string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase))
            {
                int idx = line.IndexOf(":");
                if (idx >= 0)
                {
                    key = line.Substring(idx + 1).Trim();
                    break;
                }
            }
        }
        if (string.IsNullOrEmpty(key)) return false;

        string acceptKey = ComputeWebSocketAcceptKey(key);
        string response = "HTTP/1.1 101 Switching Protocols\r\n" +
                          "Upgrade: websocket\r\n" +
                          "Connection: Upgrade\r\n" +
                          "Sec-WebSocket-Accept: " + acceptKey + "\r\n\r\n";
        byte[] respBytes = Encoding.ASCII.GetBytes(response);
        stream.Write(respBytes, 0, respBytes.Length);
        return true;
    }

    private string ReadHttpHeaders(NetworkStream stream, int timeoutMs)
    {
        DateTime start = DateTime.UtcNow;
        StringBuilder sb = new StringBuilder();
        byte[] buffer = new byte[1024];
        while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
        {
            if (stream.DataAvailable)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;
                sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
                if (sb.ToString().Contains("\r\n\r\n"))
                {
                    break;
                }
            }
            else
            {
                Thread.Sleep(5);
            }
        }
        string text = sb.ToString();
        int endIdx = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        if (endIdx >= 0)
        {
            return text.Substring(0, endIdx + 4);
        }
        return null;
    }

    private string ComputeWebSocketAcceptKey(string key)
    {
        string magic = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(magic));
            return Convert.ToBase64String(hash);
        }
    }

    private string ReadTextFrame(NetworkStream stream)
    {
        // 基本帧结构：FIN+Opcode, Mask+PayloadLen, [扩展长度], MaskKey(4), Payload
        int b1 = stream.ReadByte();
        if (b1 == -1) return null;
        int b2 = stream.ReadByte();
        if (b2 == -1) return null;

        bool fin = (b1 & 0x80) != 0;
        int opcode = b1 & 0x0F;
        bool masked = (b2 & 0x80) != 0;
        ulong payloadLen = (ulong)(b2 & 0x7F);

        if (opcode == 0x8) // Close
        {
            // 读取并回显关闭帧
            byte[] closePayload = ReadArbitraryPayload(stream, masked, payloadLen);
            SendControlFrame(stream, 0x8, closePayload);
            return null;
        }
        if (opcode == 0x9) // Ping -> 读取payload并立即回复 Pong
        {
            byte[] pingPayload = ReadArbitraryPayload(stream, masked, payloadLen);
            SendControlFrame(stream, 0xA, pingPayload); // Pong
            return string.Empty; // 继续循环
        }
        if (opcode == 0xA)
        {
            // Pong 忽略
            byte[] _ = ReadArbitraryPayload(stream, masked, payloadLen);
            return string.Empty;
        }
        if (opcode != 0x1)
        {
            return string.Empty;
        }

        if (!masked)
        {
            // 客户端帧必须有掩码
            return null;
        }

        byte[] maskingKey = new byte[4];

        if (payloadLen == 126)
        {
            byte[] ext = new byte[2];
            stream.Read(ext, 0, 2);
            if (BitConverter.IsLittleEndian) Array.Reverse(ext);
            payloadLen = BitConverter.ToUInt16(ext, 0);
        }
        else if (payloadLen == 127)
        {
            byte[] ext = new byte[8];
            stream.Read(ext, 0, 8);
            if (BitConverter.IsLittleEndian) Array.Reverse(ext);
            payloadLen = BitConverter.ToUInt64(ext, 0);
        }

        int read = stream.Read(maskingKey, 0, 4);
        if (read != 4) return null;

        if (payloadLen > int.MaxValue)
        {
            Debug.LogWarning("Payload 太大，放弃");
            return null;
        }
        byte[] payload = new byte[payloadLen];
        int total = 0;
        while (total < (int)payloadLen)
        {
            int r = stream.Read(payload, total, (int)payloadLen - total);
            if (r <= 0) return null;
            total += r;
        }
        for (int i = 0; i < payload.Length; i++)
        {
            payload[i] = (byte)(payload[i] ^ maskingKey[i % 4]);
        }
        if (!fin)
        {
            // 简化：不支持分片
            Debug.LogWarning("收到分片帧（FIN=0），当前实现不支持");
        }
        return Encoding.UTF8.GetString(payload);
    }

    private byte[] ReadArbitraryPayload(NetworkStream stream, bool masked, ulong payloadLen)
    {
        if (payloadLen == 126)
        {
            byte[] ext = new byte[2];
            int r0 = stream.Read(ext, 0, 2);
            if (r0 != 2) return Array.Empty<byte>();
            if (BitConverter.IsLittleEndian) Array.Reverse(ext);
            payloadLen = BitConverter.ToUInt16(ext, 0);
        }
        else if (payloadLen == 127)
        {
            byte[] ext = new byte[8];
            int r0 = stream.Read(ext, 0, 8);
            if (r0 != 8) return Array.Empty<byte>();
            if (BitConverter.IsLittleEndian) Array.Reverse(ext);
            payloadLen = BitConverter.ToUInt64(ext, 0);
        }
        byte[] maskingKey = null;
        if (masked)
        {
            maskingKey = new byte[4];
            int r1 = stream.Read(maskingKey, 0, 4);
            if (r1 != 4) return Array.Empty<byte>();
        }
        if (payloadLen > int.MaxValue) return Array.Empty<byte>();
        byte[] data = new byte[payloadLen];
        int total = 0;
        while (total < (int)payloadLen)
        {
            int r = stream.Read(data, total, (int)payloadLen - total);
            if (r <= 0) break;
            total += r;
        }
        if (masked && maskingKey != null)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ maskingKey[i % 4]);
            }
        }
        return data;
    }

    private void SendControlFrame(NetworkStream stream, int opcode, byte[] payload)
    {
        if (payload == null) payload = Array.Empty<byte>();
        int len = payload.Length;
        if (len > 125) len = 0; // 简化：不发送超长控制帧
        byte[] frame = new byte[2 + payload.Length];
        frame[0] = (byte)(0x80 | (opcode & 0x0F));
        frame[1] = (byte)payload.Length;
        Array.Copy(payload, 0, frame, 2, payload.Length);
        stream.Write(frame, 0, frame.Length);
    }

    private void SendClose(NetworkStream stream, ushort statusCode = 1000, string reason = null)
    {
        byte[] reasonBytes = string.IsNullOrEmpty(reason) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(reason);
        byte[] payload = new byte[2 + reasonBytes.Length];
        byte[] codeBytes = BitConverter.GetBytes(statusCode);
        if (BitConverter.IsLittleEndian) Array.Reverse(codeBytes);
        payload[0] = codeBytes[0];
        payload[1] = codeBytes[1];
        Array.Copy(reasonBytes, 0, payload, 2, reasonBytes.Length);
        SendControlFrame(stream, 0x8, payload);
    }

    private void SendText(NetworkStream stream, string message)
    {
        if (string.IsNullOrEmpty(message)) message = string.Empty;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        // 仅支持长度 < 126 简化
        if (payload.Length < 126)
        {
            byte[] frame = new byte[2 + payload.Length];
            frame[0] = 0x81; // FIN + 文本
            frame[1] = (byte)payload.Length; // 服务器发给客户端不需要mask
            Array.Copy(payload, 0, frame, 2, payload.Length);
            stream.Write(frame, 0, frame.Length);
        }
        else if (payload.Length <= ushort.MaxValue)
        {
            byte[] frame = new byte[4 + payload.Length];
            frame[0] = 0x81;
            frame[1] = 126;
            ushort len = (ushort)payload.Length;
            byte[] ext = BitConverter.GetBytes(len);
            if (BitConverter.IsLittleEndian) Array.Reverse(ext);
            frame[2] = ext[0];
            frame[3] = ext[1];
            Array.Copy(payload, 0, frame, 4, payload.Length);
            stream.Write(frame, 0, frame.Length);
        }
        else
        {
            // 超长（>65535）暂不支持，简单拆分发送
            Debug.LogWarning("消息过长，当前实现未完整支持，尝试截断");
            SendText(stream, Encoding.UTF8.GetString(payload, 0, 65535));
        }
    }

    void OnDestroy()
    {
        StopServer();
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    private void StopServer()
    {
        if (!_isRunning) return;
        Debug.Log("正在停止 WebSocket 服务器...");
        _isRunning = false;
        try
        {
            _tcpListener?.Stop();
        }
        catch { }
        lock (_clients)
        {
            foreach (var client in _clients)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        SendClose(stream, 1001, "Server shutting down");
                    }
                    client.Close();
                }
                catch { }
            }
            _clients.Clear();
        }
        try { _listenerThread?.Join(100); } catch { }
        Debug.Log("WebSocket 服务器已关闭");
    }
}