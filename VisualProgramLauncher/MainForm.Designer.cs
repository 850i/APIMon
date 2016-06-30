namespace VisualProgramLauncher {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.launch_CPN_monitor_button = new System.Windows.Forms.Button();
            this.stop_CNP_monitor = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseLaunchDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.remote_server_watchdog = new System.Windows.Forms.Timer(this.components);
            this.status_watch_timer = new System.Windows.Forms.Timer(this.components);
            this.programs_to_launch_listView = new VisualProgramLauncher.ListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.column_Status = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1277, 460);
            this.panel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.programs_to_launch_listView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1277, 460);
            this.splitContainer1.SplitterDistance = 908;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.launch_CPN_monitor_button);
            this.flowLayoutPanel1.Controls.Add(this.stop_CNP_monitor);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(363, 460);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // launch_CPN_monitor_button
            // 
            this.launch_CPN_monitor_button.AutoSize = true;
            this.launch_CPN_monitor_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.launch_CPN_monitor_button.Dock = System.Windows.Forms.DockStyle.Top;
            this.launch_CPN_monitor_button.Location = new System.Drawing.Point(3, 3);
            this.launch_CPN_monitor_button.Name = "launch_CPN_monitor_button";
            this.launch_CPN_monitor_button.Size = new System.Drawing.Size(165, 30);
            this.launch_CPN_monitor_button.TabIndex = 0;
            this.launch_CPN_monitor_button.Text = "Launch CPN monitor";
            this.launch_CPN_monitor_button.UseVisualStyleBackColor = true;
            this.launch_CPN_monitor_button.Click += new System.EventHandler(this.launch_CPN_monitor_button_Click);
            // 
            // stop_CNP_monitor
            // 
            this.stop_CNP_monitor.AutoSize = true;
            this.stop_CNP_monitor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stop_CNP_monitor.Dock = System.Windows.Forms.DockStyle.Top;
            this.stop_CNP_monitor.Enabled = false;
            this.stop_CNP_monitor.Location = new System.Drawing.Point(3, 39);
            this.stop_CNP_monitor.Name = "stop_CNP_monitor";
            this.stop_CNP_monitor.Size = new System.Drawing.Size(165, 30);
            this.stop_CNP_monitor.TabIndex = 1;
            this.stop_CNP_monitor.Text = "Stop CPN monitor";
            this.stop_CNP_monitor.UseVisualStyleBackColor = true;
            this.stop_CNP_monitor.Click += new System.EventHandler(this.stop_CNP_monitor_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1277, 31);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseLaunchDirectoryToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // chooseLaunchDirectoryToolStripMenuItem
            // 
            this.chooseLaunchDirectoryToolStripMenuItem.Name = "chooseLaunchDirectoryToolStripMenuItem";
            this.chooseLaunchDirectoryToolStripMenuItem.Size = new System.Drawing.Size(248, 26);
            this.chooseLaunchDirectoryToolStripMenuItem.Text = "Choose launch directory";
            this.chooseLaunchDirectoryToolStripMenuItem.Click += new System.EventHandler(this.chooseLaunchDirectoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(245, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(248, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.toolStripSeparator2,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(54, 25);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(122, 26);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Please, choose the folder containing programs to launch";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel2});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 491);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1277, 26);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip2";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(137, 21);
            this.toolStripStatusLabel3.Text = "Selected directory:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(55, 21);
            this.toolStripStatusLabel2.Text = "(none)";
            // 
            // remote_server_watchdog
            // 
            this.remote_server_watchdog.Enabled = true;
            this.remote_server_watchdog.Interval = 2000;
            this.remote_server_watchdog.Tick += new System.EventHandler(this.remote_server_watchdog_Tick);
            // 
            // status_watch_timer
            // 
            this.status_watch_timer.Enabled = true;
            this.status_watch_timer.Interval = 1527;
            this.status_watch_timer.Tick += new System.EventHandler(this.status_watch_timer_Tick);
            // 
            // programs_to_launch_listView
            // 
            this.programs_to_launch_listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.programs_to_launch_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.column_Status});
            this.programs_to_launch_listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.programs_to_launch_listView.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.programs_to_launch_listView.FullRowSelect = true;
            this.programs_to_launch_listView.GridLines = true;
            this.programs_to_launch_listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.programs_to_launch_listView.Location = new System.Drawing.Point(0, 0);
            this.programs_to_launch_listView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.programs_to_launch_listView.MultiSelect = false;
            this.programs_to_launch_listView.Name = "programs_to_launch_listView";
            this.programs_to_launch_listView.Size = new System.Drawing.Size(908, 460);
            this.programs_to_launch_listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.programs_to_launch_listView.TabIndex = 0;
            this.programs_to_launch_listView.UseCompatibleStateImageBehavior = false;
            this.programs_to_launch_listView.View = System.Windows.Forms.View.Details;
            this.programs_to_launch_listView.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            this.programs_to_launch_listView.DoubleClick += new System.EventHandler(this.programs_to_launch_listView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 266;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Directory";
            this.columnHeader2.Width = 0;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Path";
            this.columnHeader3.Width = 369;
            // 
            // column_Status
            // 
            this.column_Status.Text = "Status";
            this.column_Status.Width = 262;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1277, 517);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "APIMon Launcher";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseLaunchDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private ListViewEx programs_to_launch_listView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button launch_CPN_monitor_button;
        private System.Windows.Forms.Button stop_CNP_monitor;
        private System.Windows.Forms.Timer remote_server_watchdog;
        private System.Windows.Forms.Timer status_watch_timer;
        private System.Windows.Forms.ColumnHeader column_Status;
    }
}

