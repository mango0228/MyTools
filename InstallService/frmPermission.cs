using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace InstallService
{
    public partial class frmPermission : Form
    {
        public frmPermission()
        {
            InitializeComponent();
        }



        public List<string> NewListItem = new List<string>();



        public string GetXmlNodeStringByAttribute(string xml,string attribute)
        {
            XmlDocument x = new XmlDocument();
            x.LoadXml(xml);
            XmlNode mm = x.GetElementsByTagName("Item").Item(0);
            return mm.Attributes[attribute].Value;
        }

        public XmlNode GetXmlNodeByString(string xml)
        {
            XmlDocument x = new XmlDocument();
            x.LoadXml(xml);
            XmlNode mm = x.GetElementsByTagName("Item").Item(0);
            return mm;
        }



        private void button1_Click(object sender, EventArgs e)
        {

            //XmlDocument x = new XmlDocument();
            //x.LoadXml("<Item ID=\"4008\" ParentId=\"1\" Order=\"1\" Controller=\"AccountGameMg\" Action=\"Index\" Link=\" / AccountGameMg / Index\">游戏管理</Item>");

            //XmlNode mm = x.GetElementsByTagName("Item").Item(0);

            //string str = mm.Attributes["ID"].Value;



            tvPermission.Nodes.Clear();

            XmlDocument xml = new XmlDocument();
            xml.Load("Company.xml");

            XmlNodeList xnl = xml.GetElementsByTagName("Item");
            foreach (XmlNode item in xnl)
            {
                //Console.WriteLine(xnl.Item(i).InnerXml);
                XmlNode valNode = item.Attributes["ParentId"];
                if (valNode.Value == "0")
                {
                    TreeNode nodeChild = InitTreeNode(item,0);
                    tvPermission.Nodes.Add(nodeChild);
                    LoadChildNode(xnl, nodeChild, item.Attributes["ID"].Value,0);
                }


            }
        }

        private TreeNode InitTreeNode(XmlNode item,int Hierarchy)
        {
            return InitTreeNode(item.InnerText + "_" + item.Attributes["ID"].Value + "_" + Hierarchy, item.OuterXml);
        }
        private TreeNode InitTreeNode(string name,string OuterXml)
        {
            TreeNode nodeChild = new TreeNode(name);
            nodeChild.ContextMenuStrip = tvMenu;
            nodeChild.Tag = OuterXml;
            return nodeChild;
        }



        private void LoadChildNode(XmlNodeList xnl, TreeNode node, string ItemId, int Hierarchy)
        {

            int curHie = Hierarchy + 1;
            foreach (XmlNode item in xnl)
            {
                XmlNode valNode = item.Attributes["ParentId"];

                if (valNode.Value == ItemId)
                {
                    TreeNode nodeChild = InitTreeNode(item, curHie);
                    node.Nodes.Add(nodeChild);
                    LoadChildNode(xnl, nodeChild, item.Attributes["ID"].Value, curHie);
                }

            }
        }

        private void 新增节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTreeNode frm = new AddTreeNode();
            this.AddNodeName = "";
            frm.myDelMsg = new DelegateParam(SetNodeName);
            frm.ShowDialog();

            string addName = this.AddNodeName;
            if (addName != "")
            {
                TreeNode node = (sender as ToolStripMenuItem).GetCurrentParent().Tag as TreeNode;
                if (node != null)
                {
                    //计算ItemID
                    TreeNode tnLast = node.LastNode;
                    long l = 0;
                    int order = 1;
                    string xml = "";
                    int hierarchy = 0; //层级
                    if (tnLast != null)
                    {
                        xml = tnLast.Tag.ToString();
                        l = long.Parse(GetXmlNodeStringByAttribute(xml, "ID")) + 1;
                        order = int.Parse(GetXmlNodeStringByAttribute(xml, "Order")) + 1;
                    }
                    else
                    {
                        xml = node.Tag.ToString();
                        l = long.Parse(GetXmlNodeStringByAttribute(xml, "ID") + "01");
                    }

                    xml = node.Tag.ToString();
                    long ParentId = long.Parse(GetXmlNodeStringByAttribute(xml, "ID"));
                    string tagString = "<Item ID=\"" + l.ToString() + "\" ParentId=\"" + ParentId + "\" Order=\"" + order + "\">" + addName + "</Item>";
                    Clipboard.SetDataObject(l.ToString()); //复制新增的ItemId 到粘贴板
                    NewListItem.Add(tagString);

                    TreeNode nodeChild = InitTreeNode(addName + "_" + l.ToString(), tagString);
                    node.Nodes.Add(nodeChild);

                }
            }



           
        }

        public string AddNodeName { get; set; }
        private void SetNodeName(string name)
        {
            this.AddNodeName = name;
        }

        private void tvPermission_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvMenu.Tag = e.Node;

           lblItem.Text = e.Node.Tag.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {



           TreeNodeCollection listAll =  tvPermission.Nodes;

            string strXml = "<?xml version=\"1.0\" encoding=\"utf - 8\" ?>\r\n< Permission > ";

            foreach (TreeNode item in listAll)
            {
                string str = item.Tag.ToString();
                if (str.IndexOf("Hierarchy=") < 0)
                {
                   str =  str.Replace("ParentId=", "Hierarchy=\"" + item.Text.Split('_')[2] + "\" ParentId=") + "\r\n";
                }
                strXml += str;
                GetChildNodeTag(item,ref strXml);
            }
            strXml += "</Permission>";


            FileStream fs = new FileStream("Company222.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs,Encoding.UTF8);
            fs.SetLength(0);//首先把文件清空了。
            sw.Write(strXml);//写你的字符串。
            sw.Close();

        }


        private void GetChildNodeTag(TreeNode node, ref string strXml)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode item in node.Nodes)
                {
                    string str = item.Tag.ToString();
                    if (str.IndexOf("Hierarchy=") < 0)
                    {
                        str = str.Replace("ParentId=", "Hierarchy=\"" + item.Text.Split('_')[2] + "\" ParentId=") + "\r\n";
                    }
                    strXml += str;
                    GetChildNodeTag(item, ref strXml);
                }
            }
        }

        private void frmPermission_Load(object sender, EventArgs e)
        {


            List<NodeParam> list = new List<NodeParam>();
            list.Add(new NodeParam() {
                TestText = "abc"
            });
            list.Add(new NodeParam()
            {
                TestText = "ab"
            });
            list.Add(new NodeParam()
            {
                TestText = "ddd"
            });

            List<string> bb = new List<string>();
            bb.Add(list.Where(m => m.TestText == "aaaa").FirstOrDefault()?.TestText);

            bool b = bb.Contains("111");



        }
    }

    public class NodeParam
    {


        /// <summary>
        /// 存放当前的xml节点
        /// </summary>
        public XmlNode XmlCurrentNode { get; set; }


        /// <summary>
        /// 存放上级Node
        /// </summary>
        public TreeNode TreeParentNode { get; set; }


        public string TestText { get; set; }


    }


}
