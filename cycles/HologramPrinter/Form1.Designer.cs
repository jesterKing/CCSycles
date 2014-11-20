
using System.Windows.Forms;

namespace HologramPrinter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRender = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelectOutputFolder = new System.Windows.Forms.Button();
            this.txtSelectedOutputDirectory = new System.Windows.Forms.TextBox();
            this.btnSelectMaterial = new System.Windows.Forms.Button();
            this.txtSelectedMaterial = new System.Windows.Forms.TextBox();
            this.btnSelectScene = new System.Windows.Forms.Button();
            this.txtSelectedScene = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtProgress = new System.Windows.Forms.TextBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 9);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectScene, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtSelectedScene, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectMaterial, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtSelectedMaterial, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnRender, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectOutputFolder, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnCompile, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtSelectedOutputDirectory, 2, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(504, 370);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnRender
            // 
            this.btnRender.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnRender.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnRender.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRender.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnRender.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnRender.ForeColor = System.Drawing.Color.Black;
            this.btnRender.Location = new System.Drawing.Point(53, 303);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(140, 44);
            this.btnRender.TabIndex = 7;
            this.btnRender.Text = "Render";
            this.btnRender.UseVisualStyleBackColor = false;
            this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(203, 303);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 44);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelectOutputFolder
            // 
            this.btnSelectOutputFolder.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSelectOutputFolder.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSelectOutputFolder.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSelectOutputFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnSelectOutputFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnSelectOutputFolder.ForeColor = System.Drawing.Color.Black;
            this.btnSelectOutputFolder.Location = new System.Drawing.Point(53, 233);
            this.btnSelectOutputFolder.Name = "btnSelectOutputFolder";
            this.btnSelectOutputFolder.Size = new System.Drawing.Size(140, 44);
            this.btnSelectOutputFolder.TabIndex = 5;
            this.btnSelectOutputFolder.Text = "Select Output Folder";
            this.btnSelectOutputFolder.UseVisualStyleBackColor = false;
            this.btnSelectOutputFolder.Click += new System.EventHandler(this.btnSelectOutputFolder_Click);
            // 
            // txtSelectedOutputDirectory
            // 
            this.txtSelectedOutputDirectory.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtSelectedOutputDirectory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSelectedOutputDirectory.Location = new System.Drawing.Point(203, 233);
            this.txtSelectedOutputDirectory.Multiline = true;
            this.txtSelectedOutputDirectory.Name = "txtSelectedOutputDirectory";
            this.txtSelectedOutputDirectory.Size = new System.Drawing.Size(244, 44);
            this.txtSelectedOutputDirectory.TabIndex = 6;
            // 
            // btnSelectMaterial
            // 
            this.btnSelectMaterial.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSelectMaterial.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSelectMaterial.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSelectMaterial.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnSelectMaterial.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnSelectMaterial.ForeColor = System.Drawing.Color.Black;
            this.btnSelectMaterial.Location = new System.Drawing.Point(53, 93);
            this.btnSelectMaterial.Name = "btnSelectMaterial";
            this.btnSelectMaterial.Size = new System.Drawing.Size(140, 44);
            this.btnSelectMaterial.TabIndex = 3;
            this.btnSelectMaterial.Text = "Select Your Material";
            this.btnSelectMaterial.UseVisualStyleBackColor = false;
            this.btnSelectMaterial.Click += new System.EventHandler(this.btnSelectMaterial_Click);
            // 
            // txtSelectedMaterial
            // 
            this.txtSelectedMaterial.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtSelectedMaterial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSelectedMaterial.Location = new System.Drawing.Point(203, 93);
            this.txtSelectedMaterial.Multiline = true;
            this.txtSelectedMaterial.Name = "txtSelectedMaterial";
            this.txtSelectedMaterial.Size = new System.Drawing.Size(244, 44);
            this.txtSelectedMaterial.TabIndex = 4;
            // 
            // btnSelectScene
            // 
            this.btnSelectScene.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSelectScene.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSelectScene.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSelectScene.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnSelectScene.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnSelectScene.ForeColor = System.Drawing.Color.Black;
            this.btnSelectScene.Location = new System.Drawing.Point(53, 23);
            this.btnSelectScene.Name = "btnSelectScene";
            this.btnSelectScene.Size = new System.Drawing.Size(140, 44);
            this.btnSelectScene.TabIndex = 1;
            this.btnSelectScene.Text = "Select Your Scene";
            this.btnSelectScene.UseVisualStyleBackColor = false;
            this.btnSelectScene.Click += new System.EventHandler(this.btnSelectScene_Click);
            // 
            // txtSelectedScene
            // 
            this.txtSelectedScene.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtSelectedScene.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSelectedScene.Location = new System.Drawing.Point(203, 23);
            this.txtSelectedScene.Multiline = true;
            this.txtSelectedScene.Name = "txtSelectedScene";
            this.txtSelectedScene.Size = new System.Drawing.Size(244, 44);
            this.txtSelectedScene.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 553);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(504, 54);
            this.panel1.TabIndex = 20;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(53, 27);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(394, 10);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 21;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.Controls.Add(this.txtProgress, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 370);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(504, 183);
            this.tableLayoutPanel2.TabIndex = 23;
            // 
            // txtProgress
            // 
            this.txtProgress.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtProgress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProgress.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtProgress.Location = new System.Drawing.Point(50, 0);
            this.txtProgress.Margin = new System.Windows.Forms.Padding(0);
            this.txtProgress.Multiline = true;
            this.txtProgress.Name = "txtProgress";
            this.txtProgress.ReadOnly = true;
            this.txtProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtProgress.Size = new System.Drawing.Size(400, 183);
            this.txtProgress.TabIndex = 25;
            // 
            // btnCompile
            // 
            this.btnCompile.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCompile.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCompile.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCompile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnCompile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnCompile.ForeColor = System.Drawing.Color.Black;
            this.btnCompile.Location = new System.Drawing.Point(53, 163);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(140, 44);
            this.btnCompile.TabIndex = 9;
            this.btnCompile.Text = "Compile Material";
            this.btnCompile.UseVisualStyleBackColor = false;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(504, 607);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hologram Printer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onFormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSelectScene;
        private System.Windows.Forms.Button btnSelectMaterial;
        private System.Windows.Forms.Button btnSelectOutputFolder;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox txtSelectedScene;
        private System.Windows.Forms.TextBox txtSelectedMaterial;
        private System.Windows.Forms.TextBox txtSelectedOutputDirectory;
        private System.Windows.Forms.Panel panel1;
        private Button btnRender;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ProgressBar progressBar1;
        private Button btnCancel;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox txtProgress;
        private Button btnCompile;

    }
}

