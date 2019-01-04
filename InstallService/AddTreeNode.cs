using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallService
{

    public delegate void DelegateParam(string NodeName,string StrLink);


    public partial class AddTreeNode : Form
    {


        
        
        public DelegateParam myDelMsg;


        public AddTreeNode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (myDelMsg != null)
            {
                //调用
                myDelMsg(txtNodeName.Text.Trim(),txtlink.Text.Trim());
                this.Close();
            }
        }

        private void AddTreeNode_Load(object sender, EventArgs e)
        {
            this.txtNodeName.Text = "";
            this.txtlink.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
