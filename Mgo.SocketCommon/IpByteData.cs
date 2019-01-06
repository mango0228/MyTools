using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mgo.SocketCommon
{
    public class IpByteData
    {
        /// <summary>
        /// 获取包的长度.
        /// </summary>
        //public int LengContent
        //{
        //    get
        //    {

        //        //包头，4字节存放 包的长度
        //        byte[] lenByte = new byte[4];
        //        if (Bytes.Count >= 4)
        //        {
        //            Array.Copy(Bytes.ToArray(), 0, lenByte, 0, 4);
        //            return BitConverter.ToInt32(lenByte, 0);
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}

        /// <summary>
        /// 存放多个完整的包字节
        /// </summary>
        public List<List<byte>> BytesComplete = new List<List<byte>>();


        /// <summary>
        /// 剩余的字节[应当按IP缓冲起来]
        /// </summary>
        public List<byte> RemainingBytes = new List<byte>();



        /// <summary>
        /// 分包处理，把完整包存放到Bytes，剩余字节存放到RemainingBytes
        /// </summary>
        /// <param name="bytes"></param>
        public void SetBytes(byte[] bytes)
        {

            //保证不空
            if (bytes != null && bytes.Length > 0)
            {
                //保证至少有包头
                if (bytes.Length >= 4)
                {

                    //包头，4字节存放 包的长度
                    byte[] lenByte = new byte[4];
                    Array.Copy(bytes, 0, lenByte, 0, 4);
                    int leng = BitConverter.ToInt32(lenByte, 0); //包的长度

                    //刚刚好。 可以解包
                    if (bytes.Length == leng + 4) 
                    {
                        byte[] content = new byte[leng];
                        Array.Copy(bytes, 4, content, 0, leng);
                        BytesComplete.Add(content.ToList());
                    }
                    //内容长度大于包头长度，需要分包处理。 有粘包情况
                    else if (bytes.Length > leng + 4)
                    {

                        byte[] content = new byte[leng];
                        Array.Copy(bytes, 4, content, 0, leng);
                        BytesComplete.Add(content.ToList());

                        int start = leng + 4;
                        int a = bytes.Length - leng - 4;
                        byte[] remainingBytes = new byte[a];
                        Array.Copy(bytes, start, remainingBytes, 0, a);
                        //把剩余字节继续处理
                        SetBytes(remainingBytes);
                    }
                    //长度不够包内容，需要先缓冲包内容。
                    else
                    {
                        //处理包不够包头的长度【含有包头在里面的】
                        RemainingBytes.AddRange(bytes); 
                    }
                }
                else
                {
                    //包头不够先缓冲.
                    RemainingBytes.AddRange(bytes);
                }
            }


        }


        /// <summary>
        /// 获取指定的长度数组。 多余的部分返回出去。 没有多余则返回null
        /// </summary>
        private void GetBytesByLeng(byte[] bytes,int start,int leng)
        {
            byte[] content = new byte[leng];
            Array.Copy(bytes, start, content, 0, leng);
        }

        /// <summary>
        /// 只有带请求key的才能使用此方法转换[UTF-8编码]
        /// </summary>
        /// <param name="bytes">响应字节（完整包不含包头）</param>
        /// <param name="key">转换后的key字符串</param>
        /// <param name="Content">转换后的内容字符串</param>
        public static void GetContentAndRequetKey(byte[] bytes, out string key, out string content)
        {
            byte[] bykey = new byte[32];
            byte[] bycontent = new byte[bytes.Length - 32];

            Array.Copy(bytes, 0, bykey, 0, bykey.Length);
            key = Encoding.UTF8.GetString(bykey);

            Array.Copy(bytes, bykey.Length, bycontent, 0, bycontent.Length);
            content = Encoding.UTF8.GetString(bycontent);
        }










    }
}
