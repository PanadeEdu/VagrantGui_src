/**
 *   A Simple Control Gui for Executing Vagrant Commands on Windows.
 *   Copyright (C) 2015  Florian Tatzel
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 **/

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VagrantGui
{
    public partial class Gui : Form
    {
        Process proc_var;
        public Gui()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String path = textBox3.Text;
            if (path.Equals(""))
            {
                path = Directory.GetCurrentDirectory();
            }
            String command = "vagrant up";
            textBox2.AppendText("\n ############## Starting Vagrant Box ############## \n");
            proc_start(command, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String command = "vagrant halt";
            textBox2.AppendText("\n ############## Stopping Vagrant Box ############## \n");
            proc_start(command, true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String command = "vagrant reload";
            textBox2.AppendText("\n ############## Restarting Vagrant Box ############ \n");
            proc_start(command, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String command = "vagrant suspend";
            textBox2.AppendText("\n ############## Suspending Vagrant Box ############ \n");
            proc_start(command, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var confirmation = MessageBox.Show("Are you sure, you want to Destroy your Vagrant Box? Unsaved Data may be lost!", "Confirm Vagrant Destroy", MessageBoxButtons.YesNo);
            if (confirmation == DialogResult.Yes)
            {
                String command = "vagrant destroy -f";
                textBox2.AppendText("\n ############## Destroying Vagrant Box ############ \n");
                proc_start(command, true);
            }
            else
            {
                textBox2.AppendText("\n ############## Destroying Vagrant Box ############ \n");
                textBox2.AppendText("<<<< ABORT >>>> \n");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String command = textBox1.Text;
            textBox2.AppendText("\n ############## Sending Command ################### \n");
            proc_start(command, true);
        }

        void proc_start(String command, Boolean shell)
        {
            String path = getPath();
            textBox2.AppendText("Executing: " + command + " \n" + "Path: '" + path + "'\n");

            // Create Process with Config
            this.proc_var = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C cd " + path + " & " + command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = shell
                }
            };

            // Set our event handler to asynchronously read the Process output.
            this.proc_var.OutputDataReceived += new DataReceivedEventHandler(proc_OnOutputDataReceived);
            this.proc_var.ErrorDataReceived += new DataReceivedEventHandler(proc_OnOutputDataReceived);
            this.proc_var.EnableRaisingEvents = true;
            this.proc_var.Exited += new EventHandler(proc_OnExitCodeRecieved);

            // Start Process
            this.proc_var.Start();

            disableControl();

            // Start the asynchronous read of the sort output stream.
            this.proc_var.BeginOutputReadLine();
            this.proc_var.BeginErrorReadLine();

            // Wait for Exit with Timeout (otherwise wouldnt print Async)
            this.proc_var.WaitForExit(30);
        }

        String getPath()
        {
            String path = textBox3.Text;
            if (path.Equals(""))
            {
                path = Directory.GetCurrentDirectory();
            }
            return path;
        }

        void proc_OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
            this.BeginInvoke(new MethodInvoker(() =>
            {
                textBox2.AppendText(e.Data ?? string.Empty);
                textBox2.AppendText("\n");
            }));
        }

        void proc_OnExitCodeRecieved(object sender, System.EventArgs e)
        {
            Trace.WriteLine(e.ToString());
            this.BeginInvoke(new MethodInvoker(() =>
            {
                textBox2.AppendText("Exit Code: " + this.proc_var.ExitCode.ToString() + "\n" ?? string.Empty);
                textBox2.AppendText("<<<< DONE >>>> \n");
                enableControl();
            }));

        }

        void disableControl()
        {
            label2.Text = "...running";

            button1.Enabled = 
            button2.Enabled = 
            button3.Enabled = 
            button4.Enabled = 
            button5.Enabled = 
            button6.Enabled = 
            button7.Enabled = false;
            
            button5.BackColor = Color.FromArgb(50, Color.Black);
            button7.BackColor = Color.FromArgb(50, Color.Aqua);
        }

        void enableControl()
        {
            label2.Text = "finished";

            button1.Enabled =
            button2.Enabled =
            button3.Enabled =
            button4.Enabled =
            button5.Enabled =
            button6.Enabled =
            button7.Enabled = true;

            button5.BackColor = Color.FromArgb(255, Color.Black);
            button7.BackColor = Color.FromArgb(255, Color.Aqua);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button6.PerformClick();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Gui_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VagrantGui.AboutBox2 box = new VagrantGui.AboutBox2();
            box.ShowDialog();      
        }
    }
}
 