using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            string DotnetPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            string installUtil = DotnetPath + "InstallUtil.exe";
            ProcessStartInfo info = new ProcessStartInfo();
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.FileName = installUtil;
            info.Arguments = "\"" + AppDomain.CurrentDomain.BaseDirectory + "WindowsServiceTest.exe\"";
            Process pro = Process.Start(info);
            Console.WriteLine("正在安装监控服务...");
            pro.WaitForExit();

            //info.FileName = "net.exe";
            //info.Arguments = "start WindowsServiceTest";
            //pro = Process.Start(info);
            //Console.WriteLine("正在启动监控服务...");
            //pro.WaitForExit();



        }

        private static void RunCmd(string str)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServiceController[] services = ServiceController.GetServices();

            DataTable dt = CreateTable();
            foreach (var item in services)
            {
                if (item.ServiceName.IndexOf("Off") >= 0 || item.ServiceName.IndexOf("Common") >= 0 || item.ServiceName.IndexOf("Balan") >= 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["ServiceName"] = item.ServiceName;
                    dr["Status"] = item.Status.ToString();
                    dt.Rows.Add(dr);
                }
            }
            ServiceInstaller sm = new ServiceInstaller();
            this.dataGridView1.DataSource = dt;
        }


        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ServiceName");
            dt.Columns.Add("Status");




            return dt;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            BathRunStopServic("net start", ServiceControllerStatus.Stopped);
        }
        private void BathRunStopServic(string cmd, ServiceControllerStatus status)
        {
            DataTable dt = this.dataGridView1.DataSource as DataTable;
            ServiceController[] services = ServiceController.GetServices();
            foreach (DataRow item in dt.Rows)
            {

                for (int i = 0; i < services.Length; i++)
                {
                    if (item["ServiceName"].ToString() == services[i].ServiceName && services[i].Status == status)
                    {
                        RunCmd(cmd + " " + services[i].ServiceName);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BathRunStopServic("net stop", ServiceControllerStatus.Running);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BathRunStopServic("sc delete", ServiceControllerStatus.Stopped);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] strFiles = Directory.GetFiles(textBox1.Text.Trim(), "*", SearchOption.AllDirectories);

            string[] strExeServiceName = { "CommonTimerService.exe", "BalanceService.exe", "Official.Bet.Service.exe", "OfficialAccountService.exe",
                "OfficialAutoPayment.exe", "OfficialCacheService.exe", "OfficialOrderWinService.exe", "OfficialPeriodService.exe", "OfficialReportService.exe", "OfficialSettlementService.exe","PayService.exe"};

            List<string> installArray = new List<string>();
            foreach (string item in strFiles)
            {
                foreach (var strName in strExeServiceName)
                {
                    //源码环境到bin去找
                    if (ckIsCode.Checked && item.IndexOf("bin") > 0 && item.IndexOf(strName) > 0 && Path.GetExtension(item) == ".exe")
                    {
                        installArray.Add(item);
                    }
                    else if (item.IndexOf(strName) > 0 && Path.GetExtension(item) == ".exe") //部署环境直接找.
                    {
                        installArray.Add(item);
                    }

                }
            }

            foreach (var item in installArray)
            {
                InstallServic(item);
            }

            MessageBox.Show("安装成功.");

        }


        private void InstallServic(string path)
        {
            string ServiceName = Path.GetFileNameWithoutExtension(path);
            if (ServiceName.IndexOf("Official") < 0)
            {
                ServiceName = "Official" + ServiceName;
            }
            ServiceName = ServiceName.Replace(".","");
            ServiceName += "_"+txtVersion.Text.Trim();
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //path = @"D:\WorkCode\LXService\OfficialReportService\bin\Debug\OfficialReportService.exe";
            Process.Start("sc", "create " + ServiceName + " binpath= \"" + path + "\" displayName= "+ServiceName+" start= auto");

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var selrow = this.dataGridView1.SelectedRows;
            if (selrow != null && selrow.Count > 0 && MessageBox.Show("确定停止服务吗？","提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string ServiceName = selrow[0].Cells[0].Value.ToString();
                RunCmd("net stop " + ServiceName);
            }
            else
            {
                MessageBox.Show("没有选中.");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var selrow = this.dataGridView1.SelectedRows;
            if (selrow != null && selrow.Count > 0 && MessageBox.Show("确定删除服务吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string ServiceName = selrow[0].Cells[0].Value.ToString();
                RunCmd("sc delete " + ServiceName);
            }
            else
            {
                MessageBox.Show("没有选中.");
            }
        }
    }


    public class ServiceDisplay
    {
        public string ServiceName;

        public string Status;




    }



}
