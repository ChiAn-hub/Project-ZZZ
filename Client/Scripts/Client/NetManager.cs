using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Project_ZZZ.Net;
using SimpleServer.Proto;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    private Socket m_socket;
    public string PublicKey = "abc123";
    public string SecretKey;
    private ByteArray byteArray;
    private List<MsgBase> m_MsgList;
    private int m_MsgCount;
    private long lastPingTime;
    private long lastPongTime;
    private List<MsgBase> m_UnityMsgList;
    private Thread m_MsgSecret;
    private Queue<ByteArray> m_WriteQueue; // 待发送数据队列
    private int m_QueueCount;
    private Thread m_MsgPing;//心跳包线程
    private long PingInterval = 30;

    public delegate void m_MsgHandler(MsgBase msgBase);
    public Dictionary<ProtocolEnum, m_MsgHandler> m_MsgProtoDict;
    public void Init()//初始化
    {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byteArray = new ByteArray();
        m_MsgList = new List<MsgBase>();
        m_MsgCount = 0;
        m_QueueCount = 0;
        lastPingTime = GetTimeStamp();
        lastPongTime = GetTimeStamp();
        m_UnityMsgList = new List<MsgBase>();
        m_MsgProtoDict = new Dictionary<ProtocolEnum, m_MsgHandler>();
        m_WriteQueue = new Queue<ByteArray>();
    }
    public void Connect(string ip, int port)//连接客户端
    {
        if (m_socket != null && m_socket.Connected)//如果客户端已经连接上了并且不为空
        {
            Debug.Log("已经连接上了");
            return;
        }
        Init();
        m_socket.NoDelay = true;//允许后台运行
        m_socket.BeginConnect(ip, port, ConnectCallback, m_socket);
    }
    public void ConnectCallback(IAsyncResult ar)//当连接上服务端时,调用该回调函数
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            ProtoManager.Instance.CSSecret();
            m_MsgSecret = new Thread(MsgSecretThread);
            m_MsgSecret.IsBackground = true;
            m_MsgSecret.Start();
            m_MsgPing = new Thread(MsgPingThread);
            m_MsgPing.IsBackground = true;
            m_MsgPing.Start();
            socket.BeginReceive(byteArray.bytes, byteArray.writeIdx, byteArray.Remain, 0, ReceiverCallback, socket);
        }
        catch (SocketException e)
        {
            Debug.LogError("连接异常：" + e.ToString());
            return;
        }
    }
    public void ReceiverCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);//接收数据的大小
            if (count <= 0)
            {
                Close();
                return;
            }
            byteArray.writeIdx += count;
            ReceiveData();
            byteArray.CheckAndMoveBytes();
            if (byteArray.Remain < 8)//处理完后剩余数据小于8字节，则扩容然后重新接收数据
            {
                byteArray.Resize(byteArray.Length * 2);
            }
            socket.BeginReceive(byteArray.bytes, byteArray.writeIdx, byteArray.Remain, 0, ReceiverCallback, socket);
        }
        catch (SocketException e)
        {
            Debug.LogError("接收数据异常：" + e.ToString());
            Close();
            return;
        }
    }
    public void ReceiveData()
    {
        if (byteArray.Length <= 4 || byteArray.readIdx < 0)
        {
            return;
        }
        int headCount = BitConverter.ToInt32(byteArray.bytes, byteArray.readIdx);//计算出包头的信息
        if (headCount + 4 > byteArray.Length)
        {
            return;
        }
        byteArray.readIdx += 4;
        ProtocolEnum protocol = ProtocolEnum.None;
        int nameCount;
        try
        {
            protocol = MsgBase.DecodeName(byteArray.bytes, byteArray.readIdx, out nameCount);
        }
        catch (SocketException e)
        {
            Debug.LogError("解析包名出错:" + e.ToString());
            Close();
            throw;
        }
        if (protocol == ProtocolEnum.None)
        {
            Debug.LogError("协议不存在！");
            Close();
            return;
        }
        byteArray.readIdx += nameCount;
        MsgBase msgBase = null;
        int bodyCount = headCount - nameCount;
        try
        {
            msgBase = MsgBase.Decode(protocol, byteArray.bytes, byteArray.readIdx, bodyCount);
        }
        catch (SocketException e)
        {
            Debug.LogError("解析包体出错:" + e.ToString());
            Close();
            return;
        }
        if (msgBase == null)
        {
            Debug.LogError("解析包体出错");
            Close();
            return;
        }
        byteArray.readIdx += bodyCount;
        byteArray.CheckAndMoveBytes();//刷新缓冲区
        lock (m_MsgList)
        {
            m_MsgList.Add(msgBase);
            m_MsgCount++;
        }
        if (byteArray.Length > 8)//证明还有消息
        {
            ReceiveData();//继续处理
        }
    }

    public void MsgSecretThread()
    {
        while (m_socket != null && m_socket.Connected)
        {
            if (m_MsgList.Count <= 0)
            {
                Thread.Sleep(1);
                continue;
            }
            MsgBase msgBase = null;
            lock (m_MsgList)
            {
                if (m_MsgCount > 0)
                {
                    msgBase = m_MsgList[0];
                    m_MsgList.RemoveAt(0);
                }
            }
            if (msgBase != null)
            {
                if (msgBase is MsgPing)//当协议时心跳包时
                {
                    lastPongTime = GetTimeStamp();
                    m_MsgCount--;
                }
                else
                {
                    lock (m_UnityMsgList)
                    {
                        m_UnityMsgList.Add(msgBase);
                    }
                }
            }
            else
            {
                break;
            }
        }
    }
    public void MsgPingThread()
    {
        while (m_socket != null && m_socket.Connected)
        {
            long nowTime = GetTimeStamp();
            if (nowTime - lastPingTime > PingInterval)
            {
                MsgPing msgPing = new MsgPing();
                Send(msgPing);
                lastPingTime = GetTimeStamp();
            }
            if (nowTime - lastPongTime > PingInterval * 4)
            {
                Close();
            }
            Thread.Sleep(100);
        }
    }
    public void MsgUpdate()
    {
        if (m_socket != null && m_socket.Connected)
        {
            if (m_UnityMsgList.Count == 0)
            {
                return;
            }
            MsgBase msgBase = null;
            lock (m_UnityMsgList)
            {
                if (m_UnityMsgList.Count > 0)
                {
                    msgBase = m_UnityMsgList[0];
                    m_UnityMsgList.RemoveAt(0);
                    m_MsgCount--;
                }
            }
            if (msgBase != null)
            {
                InvokeProto(msgBase.ProtocolType, msgBase);
            }
        }
    }
    public void AddProto(ProtocolEnum protocol, m_MsgHandler handler)
    {
        m_MsgProtoDict[protocol] = handler;
    }
    public void InvokeProto(ProtocolEnum protocolEnum, MsgBase msgBase)
    {
        if (m_MsgProtoDict.ContainsKey(protocolEnum))
        {
            m_MsgProtoDict[protocolEnum].Invoke(msgBase);
        }
    }
    public void Send(MsgBase msgBase)
    {
        if (m_socket == null || !m_socket.Connected)
        {
            return;
        }
        byte[] nameBytes = MsgBase.EncodeName(msgBase);
        byte[] bodyBytes = MsgBase.Encode(msgBase);
        byte[] headBytes = BitConverter.GetBytes(nameBytes.Length + bodyBytes.Length);
        byte[] MsgBytes = new byte[nameBytes.Length + bodyBytes.Length + headBytes.Length];
        Array.Copy(headBytes, 0, MsgBytes, 0, headBytes.Length);
        Array.Copy(nameBytes, 0, MsgBytes, headBytes.Length, nameBytes.Length);
        Array.Copy(bodyBytes, 0, MsgBytes, headBytes.Length + nameBytes.Length, bodyBytes.Length);
        try
        {
            ByteArray byteArray = new ByteArray(MsgBytes);
            lock (m_WriteQueue)
            {
                m_WriteQueue.Enqueue(byteArray);
                m_QueueCount = m_WriteQueue.Count;
            }
            if (m_QueueCount == 1)
            {
                m_socket.BeginSend(MsgBytes, 0, MsgBytes.Length, 0, SendCallBack, m_socket);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("发送数据异常！" + e);
            Close();
            return;
        }
    }
    public void SendCallBack(IAsyncResult ar)//发送消息调用的回调
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            ByteArray byteArray;
            lock (m_WriteQueue)
            {
                byteArray = m_WriteQueue.First();
            }
            byteArray.readIdx += count;
            if (byteArray.Length == 0)
            {
                lock (m_WriteQueue)
                {
                    m_WriteQueue.Dequeue();
                    if (m_WriteQueue.Count > 0)
                    {
                        byteArray = m_WriteQueue.First();
                    }
                    else
                    {
                        byteArray = null;
                    }
                }
            }
            if (byteArray != null)
            {
                socket.BeginSend(byteArray.bytes, byteArray.readIdx, byteArray.Length, 0, SendCallBack, socket);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("发送数据异常！" + e);
            Close();
            return;
        }
    }
    public long GetTimeStamp()//计算时间戳
    {
        TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(timeSpan.TotalSeconds);
    }
    public void Close()
    {
        if (m_socket == null)
        {
            return;
        }
        if (m_MsgSecret != null && m_MsgSecret.IsAlive)
        {
            m_MsgSecret.Abort();
            m_MsgSecret = null;
        }
        if (m_MsgPing != null && m_MsgPing.IsAlive)
        {
            m_MsgPing.Abort();
            m_MsgPing = null;
        }
        SecretKey = "";
        m_socket.Close();
        Debug.Log("关闭连接");
    }
}
