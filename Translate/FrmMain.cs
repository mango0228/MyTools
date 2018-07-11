using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translate
{
    public partial class FrmMain : Form
    {
        KeyboardHook k_hook;
        public FrmMain()
        {
            InitializeComponent();
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);//钩住键按下 
            //k_hook.KeyPressEvent += K_hook_KeyPressEvent;
            k_hook.Start();//安装键盘钩子
        }


        private bool isShowForm = true;

        public bool IsShowForm
        {
            get
            {
                return isShowForm;
            }
            set
            {
                this.isShowForm = value;
                if (value)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Show();
                }
                else
                {
                    this.Hide();
                }
                
            }
        }

        private void K_hook_KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            //tb1.Text += e.KeyChar;
            int i = (int)e.KeyChar;
            System.Windows.Forms.MessageBox.Show(i.ToString());
        }

        bool shift = false;
        bool alt = false;
        bool q = false;


        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.LShiftKey)
            {
                shift = true;
            }
            if (e.KeyValue == 164)
            {
                alt = true;
            }

            if (e.KeyValue == (int)Keys.Q)
            {
                q = true;
            }
            //if (e.KeyValue == (int)Keys.Q && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt)
            if (shift && alt && q)
            {
                this.IsShowForm = !this.IsShowForm;
                shift = false;
                alt = false;
                q = false;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate("https://translate.google.cn/");
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.IsShowForm = !this.IsShowForm;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.IsShowForm = false;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            k_hook.Stop();
            notifyIcon1.Dispose(); //释放notifyIcon1的所有资源，以保证托盘图标在程序关闭时立即消失
            System.Environment.Exit(0);
        }
    }
}
