using Mgo.SocketCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.Socket.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建客户端对象，默认连接本机127.0.0.1,14524
            SocketClient client = new SocketClient(14521);
            //开始运行客户端
            client.StartClient();

            //string c = client.SendToSyncGetResponseContent(item);




            List<string> sendStr = new List<string>();

            for (int i = 0; i < 50; i++)
            {
                Random r = new Random();
                int a = r.Next(1, 50);
                sendStr.Add(GetRandomString(a, true, true, true, false, ""));
                System.Threading.Thread.Sleep(20);
            }


            // client.Send(item, Guid.NewGuid().ToString("N"));
            // client.SyncGetResponseContent();


            foreach (var item in sendStr)
            {
                //string key = Guid.NewGuid().ToString("N");
                Stopwatch sp = new Stopwatch();
                sp.Start();
                string c = client.SendToSyncGetResponseContent(item);
                sp.Stop();
                Console.WriteLine(c + "【timer:" + sp.ElapsedMilliseconds + "】");

                // string c = client.SyncGetResponseContent(key);
                // Console.WriteLine(c);
                //System.Threading.Thread.Sleep(50);
            }

            Console.WriteLine("-========================================================>>>接下来并发版本.");


            Parallel.ForEach(sendStr, (item) =>
            {

                //client.Send(item, Guid.NewGuid().ToString("N"));
                //client.SyncGetResponseContent();

                Stopwatch sp = new Stopwatch();
                sp.Start();
                string c = client.SendToSyncGetResponseContent(item);
                sp.Stop();
                Console.WriteLine(c + "【timer:" + sp.ElapsedMilliseconds + "】");


            });





            Console.ReadLine();
            client.Close();

            


            //while (true)
            //{
            //    Console.WriteLine("输入:quit关闭客户端，输入其它消息发送到服务器");
            //    string str = Console.ReadLine();
            //    if (str == "quit")
            //    {
            //        client.Close();
            //        break;
            //    }
            //    else
            //    {
            //        client.Send(str);
            //    }
            //}
        }

        #region 5.0 生成随机字符串 + static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
        #endregion
    }
}
