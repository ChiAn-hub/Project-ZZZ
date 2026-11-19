using ProtoBuf;
using Project_ZZZ.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Proto
{
    //消息基类
    public class MsgBase
    {
        public virtual ProtocolEnum ProtocolType { get; set; }
        //编码协议名
        public static byte[] EncodeName(MsgBase msgBase)
        {
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.ProtocolType.ToString());
            Int16 len = (Int16)nameBytes.Length;
            byte[] bytes = new byte[2 + len];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }
        //解码协议名
        public static ProtocolEnum DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            if (offset + 2 > bytes.Length)
            {
                return ProtocolEnum.None;
            }
            Int16 len = (Int16)(bytes[offset + 1] << 8 | bytes[offset]);
            if (offset + 2 + len > bytes.Length)
            {
                return ProtocolEnum.None;
            }
            count = 2 + len;
            try
            {
                string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
                return (ProtocolEnum)System.Enum.Parse(typeof(ProtocolEnum), name);
            }
            catch (Exception e)
            {
                Debug.LogError("不存在协议：" + e.ToString());
                return ProtocolEnum.None;
            }
        }
        //协议内容序列化和加密
        public static byte[] Encode(MsgBase msgBase)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Serializer.Serialize(memory, msgBase);
                byte[] bytes = memory.ToArray();
                string secret = ServerSocket.Instance.SecretKey;
                if (msgBase is MsgSecret)//当前的消息是请求密钥的时候
                {
                    secret = ServerSocket.Instance.PublicKey;
                }
                bytes = AES.AESEncrypt(bytes, secret);
                return bytes;
            }
        }
        //协议内容解密和反序列化
        public static MsgBase Decode(ProtocolEnum protocol, byte[] bytes, int offset, int count)
        {
            if (count <= 0)
            {
                Debug.LogError("协议解密错误，数据长度为0");
                return null;
            }
            try
            {
                byte[] newBytes = new byte[count];
                Array.Copy(bytes, offset, newBytes, 0, count);
                string secret = ServerSocket.Instance.SecretKey;//默认是密钥解密
                if (protocol == ProtocolEnum.MsgSecret)//请求密钥，用公钥解密
                {
                    secret = ServerSocket.Instance.PublicKey;
                }
                newBytes = AES.AESDecrypt(newBytes, secret);//解密
                //反序列化
                using (MemoryStream memory = new MemoryStream(newBytes, 0, newBytes.Length))
                {
                    Type type = System.Type.GetType(protocol.ToString());//根据协议名获取到对应的类类型
                    return (MsgBase)Serializer.NonGeneric.Deserialize(type, memory);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("协议内容解密异常！" + e.ToString());
                return null;
            }
        }
    }
}
