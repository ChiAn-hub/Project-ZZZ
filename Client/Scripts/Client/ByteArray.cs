using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_ZZZ.Net
{
    public class ByteArray
    {
        public const int DefaultSize = 1024;
        public byte[] bytes;//消息缓冲区
        private int initSize;//初始值
        private int capacity;//容量
        public int readIdx;//读取索引
        public int writeIdx;//写入索引
        public int Length { get { return writeIdx - readIdx; } }//数据长度
        public int Remain { get { return capacity - writeIdx; } }//剩余容量

        public ByteArray()//初始化
        {
            initSize = DefaultSize;
            bytes = new byte[DefaultSize];
            capacity = DefaultSize;
            readIdx = 0;
            writeIdx = 0;
        }
        public ByteArray(byte[] bytes)//初始化
        {
            initSize = bytes.Length;
            this.bytes = bytes;
            capacity = bytes.Length;
            readIdx = 0;
            writeIdx = bytes.Length;
        }

        public void CheckAndMoveBytes()
        {
            if (readIdx <= 0)
            {
                return;
            }

            int length = Length;

            if (length > 0)
            {
                Array.Copy(bytes, readIdx, bytes, 0, length);
            }

            readIdx = 0;
            writeIdx = length;
        }
        public void Resize(int size)
        {
            if (size <= capacity)
            {
                return;
            }

            int length = Length;
            int newCapacity = capacity;

            while (newCapacity < size)
            {
                newCapacity *= 2;
            }

            byte[] newBytes = new byte[newCapacity];

            if (length > 0)
            {
                Array.Copy(bytes, readIdx, newBytes, 0, length);
            }

            bytes = newBytes;
            capacity = newCapacity;
            readIdx = 0;
            writeIdx = length;
        }
    }
}
