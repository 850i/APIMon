using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using APIMonShared;
using APIMonLib;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace VisualProgramLauncher {
    public partial class MainForm : Form {

        CPNMonitorLauncher monitor_launcher = new CPNMonitorLauncher();

        public MainForm() {
            InitializeComponent();
        }

        private void chooseLaunchDirectoryToolStripMenuItem_Click(object sender, EventArgs e) {
            folderBrowserDialog1.ShowDialog();
            loadDirectory();
        }

        /// <summary>
        /// 
        /// </summary>
        private void loadDirectory() {
            try {
                string[] executables_list = ProgramStartDescription.findExecutablesRecursive(folderBrowserDialog1.SelectedPath);
                programs_to_launch_listView.Items.Clear();
                foreach (string executable in executables_list) {
                    string file_name = System.IO.Path.GetFileName(executable);
                    string image_dir = System.IO.Path.GetDirectoryName(executable);
                    ProgramStartDescription program_start_description = new ProgramStartDescription();
                    program_start_description.image_dir = image_dir;
                    program_start_description.image_filename = file_name;
                    program_start_description.max_running_time = 20;

                    ListViewItem list_view_item = new ListViewItem(program_start_description.image_filename);
                    programs_to_launch_listView.Items.Add(list_view_item);

                    list_view_item.Tag = program_start_description;
                    list_view_item.SubItems.Add(program_start_description.image_dir).Name="dir";
                    list_view_item.SubItems.Add(program_start_description.image_path).Name="path";
                    list_view_item.SubItems.Add("status unknown ...").Name="status";
                }
                toolStripStatusLabel2.Text = folderBrowserDialog1.SelectedPath;
                programs_to_launch_listView.Refresh();
            } catch (DirectoryNotFoundException) {
                folderBrowserDialog1.SelectedPath = "Failed to get list of executables from " + folderBrowserDialog1.SelectedPath;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void listView1_ItemActivate(object sender, EventArgs e) {
            if (programs_to_launch_listView.SelectedItems.Count>1) throw new Exception("Wrong ListView configuration. Can not select more than one item.");
        }

        private void MainForm_Load(object sender, EventArgs e) {
            //loadDirectory();
            monitor_launcher.addStateHasChangedSubscriber(remoteServerHasChanged);
            remoteServerHasChanged();
        }

        private void programs_to_launch_listView_DoubleClick(object sender, EventArgs e) {
            if (programs_to_launch_listView.SelectedItems.Count > 1) throw new Exception("Wrong ListView configuration. Can not select more than one item.");
            if (monitor_launcher.server_present) {
                foreach (ListViewItem lvi in programs_to_launch_listView.SelectedItems) {
                    ProgramStartDescription psd = (ProgramStartDescription)lvi.Tag;
                    lvi.BackColor = System.Drawing.Color.LightBlue;
                    monitor_launcher.launchProgram(psd);
                }
            }
        }

        private void remoteServerHasChanged(){
            if (monitor_launcher.server_present) {
                remoteServerStarted();
            } else {
                remoteServerStopped();
            }
        }

        private void remoteServerStarted() {
            launch_CPN_monitor_button.Enabled = false;
            stop_CNP_monitor.Enabled = true;
        }

        private void remoteServerStopped() {
            launch_CPN_monitor_button.Enabled = true;
            stop_CNP_monitor.Enabled = false;
        }

        private void launch_CPN_monitor_button_Click(object sender, EventArgs e) {
            monitor_launcher.launchCPNMonitor();
        }

        private void remote_server_watchdog_Tick(object sender, EventArgs e) {
            monitor_launcher.ping();
        }

        private void stop_CNP_monitor_Click(object sender, EventArgs e) {
            monitor_launcher.exit();
        }

        private void status_watch_timer_Tick(object sender, EventArgs e) {
            if (monitor_launcher.isLaunchResultsAvailable()) {
                ProgramResponseDescription[] results = monitor_launcher.getLaunchResults();
                foreach (ListViewItem lvi in programs_to_launch_listView.Items) {
                    ProgramStartDescription psd = (ProgramStartDescription)lvi.Tag;
                    foreach (ProgramResponseDescription response in results) {
                        if (psd.id == response.id) {
                            lvi.BackColor = System.Drawing.Color.LightCoral;
                            lvi.SubItems["status"].Text = response.desciption;
                        }
                    }
                }
                programs_to_launch_listView.Refresh();
            }
        }

    }
}
