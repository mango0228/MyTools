using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {




            try
            {
                try
                {
                    int a = int.Parse("3a");
                }
                catch (Exception ex)
                {

                    
                }

                string s = "";
            }
            catch (Exception)
            {

                throw;
            }
          



            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmPermission());

            //string tt = ReadTxtContent("info.txt");

            //string[] arrtt = tt.Split('\n');

            //List<string> list = new List<string>();

            //for (int i = 0; i < arrtt.Length; i++)
            //{
            //    if (arrtt[i].IndexOf("期数结算完成:") >= 0)
            //    {
            //        list.Add(arrtt[i]);
            //    }
            //}


            //DataTable dt = ExcelToDS("220.xls").Tables[0];

            //foreach (DataRow item in dt.Rows)
            //{

            //    string perid = item["FPeriodID"].ToString();
            //    //decimal win = decimal.Parse(item["FWinningAmountAddFDiffKickback"].ToString());
            //    decimal betAccount = decimal.Parse(item["FStake"].ToString());



            //    foreach (string strline in list)
            //    {
            //        if (strline.IndexOf("期数结算完成:") > 0 && strline.IndexOf(perid) > 0)
            //        {
            //            string s = strline.Replace(@"D:\work\LXService\Common\LXYD.WebGame.Common.MiscHelpers\Logs\LogsManager.cs", "");

            //            //decimal val = decimal.Parse(s.Substring(s.IndexOf("WinAmount:") + 10).Replace(" ", "").Replace("\r", ""));
            //            decimal val = decimal.Parse(s.Substring(s.IndexOf("UserBetAmout:") + 13).Replace(" ", "").Replace("\r", "").Split(',')[0]);

            //            //if (win > val)
            //            //{
            //            //    Console.WriteLine(perid + "    " + win + "    " + val);
            //            //}
            //            if (betAccount != val)
            //            {
            //                Console.WriteLine(perid + "    " + betAccount + "    " + val);
            //            }



            //        }
            //    }

            //}





        }




        /// <summary>
        /// 读取txt文件内容
        /// </summary>
        /// <param name="Path">文件地址</param>
        public static string ReadTxtContent(string Path)
        {
            StreamReader sr = new StreamReader(Path, Encoding.UTF8);

            return sr.ReadToEnd();
        }



        public static DataSet ExcelToDS(string Path)
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [Sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            return ds;
        }


    }
}
