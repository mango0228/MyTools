namespace InstallService
{
    partial class frmPermission
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tvPermission = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.tvMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新增节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblItem = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tvMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvPermission
            // 
            this.tvPermission.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvPermission.Location = new System.Drawing.Point(3, 97);
            this.tvPermission.Name = "tvPermission";
            this.tvPermission.Size = new System.Drawing.Size(773, 610);
            this.tvPermission.TabIndex = 0;
            this.tvPermission.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvPermission_NodeMouseClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "加载XML权限配置文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tvMenu
            // 
            this.tvMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新增节点ToolStripMenuItem});
            this.tvMenu.Name = "tvMenu";
            this.tvMenu.Size = new System.Drawing.Size(125, 26);
            // 
            // 新增节点ToolStripMenuItem
            // 
            this.新增节点ToolStripMenuItem.Name = "新增节点ToolStripMenuItem";
            this.新增节点ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.新增节点ToolStripMenuItem.Text = "新增节点";
            this.新增节点ToolStripMenuItem.Click += new System.EventHandler(this.新增节点ToolStripMenuItem_Click);
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(12, 77);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(47, 12);
            this.lblItem.TabIndex = 2;
            this.lblItem.Text = "lblItem";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(183, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "保存新增的Item";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(339, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(426, 50);
            this.textBox1.TabIndex = 4;
            // 
            // frmPermission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 707);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblItem);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tvPermission);
            this.Name = "frmPermission";
            this.Text = "配置权限xml";
            this.Load += new System.EventHandler(this.frmPermission_Load);
            this.tvMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvPermission;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip tvMenu;
        private System.Windows.Forms.ToolStripMenuItem 新增节点ToolStripMenuItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
    }
}