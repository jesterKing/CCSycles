/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

namespace csycles_tester
{
	partial class CSyclesTester
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
			this.renderResult = new System.Windows.Forms.PictureBox();
			this.renderStatus = new System.Windows.Forms.StatusStrip();
			this.btnRender = new System.Windows.Forms.Button();
			this.toolRenderStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnRenderNoCopy = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.renderResult)).BeginInit();
			this.renderStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// renderResult
			// 
			this.renderResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.renderResult.Location = new System.Drawing.Point(0, 29);
			this.renderResult.Name = "renderResult";
			this.renderResult.Size = new System.Drawing.Size(307, 286);
			this.renderResult.TabIndex = 0;
			this.renderResult.TabStop = false;
			// 
			// renderStatus
			// 
			this.renderStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolRenderStatus});
			this.renderStatus.Location = new System.Drawing.Point(0, 318);
			this.renderStatus.Name = "renderStatus";
			this.renderStatus.Size = new System.Drawing.Size(310, 22);
			this.renderStatus.TabIndex = 1;
			this.renderStatus.Text = "render status";
			// 
			// btnRender
			// 
			this.btnRender.Location = new System.Drawing.Point(53, 0);
			this.btnRender.Name = "btnRender";
			this.btnRender.Size = new System.Drawing.Size(75, 23);
			this.btnRender.TabIndex = 2;
			this.btnRender.Text = "Render";
			this.btnRender.UseVisualStyleBackColor = true;
			this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
			// 
			// toolRenderStatus
			// 
			this.toolRenderStatus.Name = "toolRenderStatus";
			this.toolRenderStatus.Size = new System.Drawing.Size(109, 17);
			this.toolRenderStatus.Text = "toolStripStatusLabel1";
			// 
			// btnRenderNoCopy
			// 
			this.btnRenderNoCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRenderNoCopy.Location = new System.Drawing.Point(144, 0);
			this.btnRenderNoCopy.Name = "btnRenderNoCopy";
			this.btnRenderNoCopy.Size = new System.Drawing.Size(96, 23);
			this.btnRenderNoCopy.TabIndex = 3;
			this.btnRenderNoCopy.Text = "Render NoCopy";
			this.btnRenderNoCopy.UseVisualStyleBackColor = true;
			this.btnRenderNoCopy.Click += new System.EventHandler(this.btnRenderWithout_Click);
			// 
			// CSyclesTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(310, 340);
			this.Controls.Add(this.btnRenderNoCopy);
			this.Controls.Add(this.btnRender);
			this.Controls.Add(this.renderStatus);
			this.Controls.Add(this.renderResult);
			this.Name = "CSyclesTester";
			this.Text = "CSyclesTester";
			((System.ComponentModel.ISupportInitialize)(this.renderResult)).EndInit();
			this.renderStatus.ResumeLayout(false);
			this.renderStatus.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.StatusStrip renderStatus;
		private System.Windows.Forms.Button btnRender;
		public System.Windows.Forms.ToolStripStatusLabel toolRenderStatus;
		public System.Windows.Forms.PictureBox renderResult;
		private System.Windows.Forms.Button btnRenderNoCopy;
	}
}