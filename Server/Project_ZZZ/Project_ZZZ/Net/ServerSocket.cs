using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Project_ZZZ.Proto;
using ProtoBuf;
using ProtoBuf.Meta;
using SimpleServer.Proto;
using SqlSugar;

namespace Project_ZZZ.Net
{
    public class ServerSocket : Singleton<ServerSocket>
    {
        private string m_ip = "127.0.0.1";//IP地址
        private int m_Port = 8012;//端口号
        public string PublicKey = "abc123";
        public string SecretKey = "123abc";
        public Socket m_Socket;
        public List<Socket> m_SocketList = new List<Socket>();//客户端列表,用于所有的客户端的Socket
        public Dictionary<Socket,ClientSocket> m_ClientSocketList = new Dictionary<Socket, ClientSocket>();//客户端列表,用于存储所有连接上的客户端
        private long m_pingInterval = 30;//心跳包间隔时间
        public List<ClientSocket> m_tempList;//用来存储心跳包超时的客户端列表
        public void Init()
        {
            IPAddress iPAddress = IPAddress.Parse(m_ip);//将字符串IP地址转换为IPAddress类型
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, m_Port);//创建IP地址和端口号的终结点
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//初始化Socket
            m_Socket.Bind(iPEndPoint);//绑定IP地址和端口号
            m_Socket.Listen(10);//开始监听，最大连接数为10
            Debug.Log("服务器启动成功，监听地址：{0}:{1}",m_ip,m_Port);
            m_tempList = new List<ClientSocket>();//初始化心跳包超时的客户端列表

            while (true)//处理消息
            {
                ResetCheckRead();//每次重置一下客户端列表
                try
                {
                    Socket.Select(m_SocketList, null, null, 100);//选出有消息的客户端
                }catch(Exception e)
                {
                    Debug.LogError("Select异常！" + e);
                }
                for(int i = m_SocketList.Count - 1; i >= 0; i--)//逆序处理消息
                {
                    Socket s = m_SocketList[i];
                    if(s == m_Socket)//如果是服务器的Socket，则表示有新的客户端连接
                    {
                        ReadListen(s);//接收客户端的连接
                    }
                    else//对应处理
                    {
                        ReceiveDate(s);//接收消息
                    }
                }
                long timeNow = GetTimeStamp();//获取当前时间戳
                m_tempList.Clear();
                foreach (ClientSocket clientSocket in m_ClientSocketList.Values)//遍历所有客户端
                {
                    if(timeNow - clientSocket.lastReceiveTime > m_pingInterval * 4)
                    {
                        m_tempList.Add(clientSocket);
                    }
                }
                foreach(ClientSocket clientSocket in m_tempList)//遍历心跳包超时的客户端列表
                {
                    CloseClient(clientSocket);//关闭客户端连接
                }
            }
        }

        public void ReadListen(Socket s)//用于接收客户端的连接
        {
            try
            {
                Socket socket = s.Accept();//接收客户端的连接
                ClientSocket clientSocket = new ClientSocket();
                clientSocket.Socket = socket;//将客户端的Socket赋值给ClientSocket类
                clientSocket.lastReceiveTime = GetTimeStamp();//记录最后接收时间
                if(socket != null)
                {
                    m_ClientSocketList.Add(socket, clientSocket);//添加到字典中
                }
                Debug.Log("一个客户链接成功{0}，当前{1}个客户端在线！", clientSocket.Socket.LocalEndPoint.ToString(), m_ClientSocketList.Count);
            }
            catch (SocketException e)
            {
                Debug.LogError("接收链接异常！" + e);
            }
        }
        public void ReceiveDate(Socket s)
        {
            ClientSocket clientSocket = m_ClientSocketList[s];//得到对应的消息
            ByteArray byteArray = clientSocket.bytes;
            int count = 0;
            if (byteArray.Remain <= 0)
            {
                OnReceiverDate(clientSocket);//处理消息
                byteArray.CheckAndMoveBytes();//刷新缓冲区
                if(byteArray.Remain <= 0)//证明内存不够,进行扩容
                {
                    int size = byteArray.Length > ByteArray.DefaultSize ? byteArray.Length : ByteArray.DefaultSize;
                    byteArray.Resize(size);
                }
            }
            try
            {
                count = clientSocket.Socket.Receive(byteArray.bytes,byteArray.writeIdx,byteArray.Remain,0);
            }catch(SocketException e)
            {
                Debug.LogError("接收数据异常！" + e);
                CloseClient(clientSocket);
                return;
            }
            if (count == 0)
            {
                CloseClient(clientSocket);
                return;
            }

            byteArray.writeIdx += count;
            OnReceiverDate(clientSocket);
            byteArray.CheckAndMoveBytes();
        }
        public void OnReceiverDate(ClientSocket clientSocket)//处理对应客户端的消息
        {
            ByteArray byteArray = clientSocket.bytes;
            if(byteArray.Length <=4 || byteArray.readIdx < 0)
            {
                return;
            }
            int headCount = BitConverter.ToInt32(byteArray.bytes, byteArray.readIdx);//计算出包头的信息
            if(headCount+4 > byteArray.Length)//证明消息不完整
            {
                return;
            }
            byteArray.readIdx += 4;
            int nameCount = 0;
            ProtocolEnum protocolEnum = ProtocolEnum.None;
            try
            {
                protocolEnum = MsgBase.DecodeName(byteArray.bytes, byteArray.readIdx, out nameCount);
            }
            catch(SocketException e)
            {
                Debug.LogError("解析协议名异常！" + e);
                CloseClient(clientSocket);
            }
            if(protocolEnum == ProtocolEnum.None)
            {
                Debug.LogError("解析协议名错误！");
                CloseClient(clientSocket);
                return;
            }
            byteArray.readIdx += nameCount;
            MsgBase msgBase = null;
            int bodyCount = headCount - nameCount;//计算出包体的长度
            try
            {
                msgBase = MsgBase.Decode(protocolEnum, byteArray.bytes, byteArray.readIdx, bodyCount);
            }catch(SocketException e)
            {
                Debug.LogError("解析包体异常！" + e);
                CloseClient(clientSocket);
                return;
            }
            if(msgBase == null)
            {
                Debug.LogError("{0}协议解析错误！" + protocolEnum.ToString());
                CloseClient(clientSocket);
                return;
            }
            byteArray.readIdx += bodyCount;
            byteArray.CheckAndMoveBytes();
            MethodInfo methodInfo = typeof(SysMsg).GetMethod(protocolEnum.ToString());
            object[] objects = new object[] { clientSocket, msgBase };
            if(methodInfo != null)
            {
                methodInfo.Invoke(null, objects);
            }
            else
            {
                Debug.LogError("{0}协议所对应的处理方法不存在", protocolEnum.ToString());
            }
            if(byteArray.Length > 4)
            {
                OnReceiverDate(clientSocket);
            }
        }
        public void SendDate(ClientSocket clientSocket, MsgBase msgBase)
        {
            byte[] nameBytes = MsgBase.EncodeName(msgBase);
            byte[] bodyBytes = MsgBase.Encode(msgBase);
            byte[] headBytes = BitConverter.GetBytes(nameBytes.Length + bodyBytes.Length);
            byte[] MsgBytes = new byte[nameBytes.Length + bodyBytes.Length + headBytes.Length];
            Array.Copy(headBytes, 0, MsgBytes, 0, headBytes.Length);
            Array.Copy(nameBytes, 0, MsgBytes, headBytes.Length, nameBytes.Length);
            Array.Copy(bodyBytes, 0, MsgBytes, headBytes.Length + nameBytes.Length, bodyBytes.Length);
            try
            {
                clientSocket.Socket.BeginSend(MsgBytes, 0, MsgBytes.Length, 0, null, null);//发送数据
            }
            catch (SocketException e)
            {
                Debug.LogError("发送数据异常！" + e);
                CloseClient(clientSocket);
            }
        }
        public void ResetCheckRead()//用于重置读取
        {
            m_SocketList.Clear();//清空客户端列表
            m_SocketList.Add(m_Socket);//将服务器的Socket添加到客户端列表中
            foreach(Socket s in m_ClientSocketList.Keys)
            {
                m_SocketList.Add(s);//将字典中存储的连接上的客户端的Socket添加到客户端列表中
            }
        }

        public long GetTimeStamp()//计算时间戳
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(timeSpan.TotalSeconds);
        }

        public void CloseClient(ClientSocket clientSocket)
        {
            Debug.Log("一个客户端断开连接{0}，当前{1}个客户端在线！", clientSocket.Socket.LocalEndPoint.ToString(), m_ClientSocketList.Count - 1);
            clientSocket.Socket.Close();
            m_ClientSocketList.Remove(clientSocket.Socket);
        }
    }
}
