using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace HologramPrinter
{
    public partial class Form1 : Form, IUI
    {
        delegate void SetTextCallback(string text);

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtProgress.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                string sent = this.txtProgress.Text;
                this.txtProgress.Text = text + "\r\n" + sent;
            }
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        public void UpdatelblProgress(String text)
        {
            //Check if invoke requied if so return - as i will be recalled in correct thread
            if (ControlInvokeRequired(this.txtProgress, () => UpdatelblProgress(text))) return;
        }

        public Form1()
        {
            InitializeComponent();
            Engine.Iui = this;
        }
        private void btnSelectScene_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    //txtSelectedScene.Text = File.ReadAllText(file);
                    txtSelectedScene.Text = file;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void btnSelectMaterial_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog2.FileName;
                try
                {
                    //txtSelectedMaterial.Text = File.ReadAllText(file);
                    txtSelectedMaterial.Text = file;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void btnSelectOutputFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = folderBrowserDialog1.SelectedPath;
                try
                {
                    txtSelectedOutputDirectory.Text = file;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void onFormClosed(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            btnRender.Enabled = false;
            btnCancel.Visible = true;    
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Engine.Initiate();

            for (int i = 1; i <= 100; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    //System.Threading.Thread.Sleep(500);
                    Engine.Render(i);                    
                    worker.ReportProgress(i);
                }
            }
            
        }

        // This event handler updates the progress. 
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Console.WriteLine((e.ProgressPercentage.ToString() + "%"));
            //UpdatelblProgress((e.ProgressPercentage.ToString() + "%"));
            this.txtProgress.Text = (e.ProgressPercentage.ToString() + "%");
            this.progressBar1.Value = e.ProgressPercentage;
        }

        // This event handler deals with the results of the background operation. 
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                //Console.WriteLine("Canceled!");
                //UpdatelblProgress("Canceled!");
                this.txtProgress.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                //Console.WriteLine("Error: " + e.Error.Message);
                //UpdatelblProgress("Error: " + e.Error.Message);
                this.txtProgress.Text = ("Error: " + e.Error.Message);
            }
            else
            {
                //Console.WriteLine("Done!");
                //UpdatelblProgress("Done!");
                this.txtProgress.Text = "Done!";
            }

            this.btnCancel.Visible = false;
            this.btnRender.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        public string getSceneFileName()
        {
            return txtSelectedScene.Text;
        }

        public string getMaterialFileName()
        {
            return txtSelectedMaterial.Text;
        }

        public string getOutputFolderName()
        {
            return txtSelectedOutputDirectory.Text;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            Engine.CompileMaterial();
        }
    }
}
