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

using System;
using System.Threading;
using System.Windows.Forms;

namespace csycles_tester
{
	public partial class CSyclesTester : Form
	{
		public CSyclesRender Renderer { get; set; }
		public Thread RenderThread { get; set; }

		public CSyclesTester()
		{
			InitializeComponent();
			Renderer = new CSyclesRender
			{
				Cst = this
			};
		}

		~CSyclesTester()
		{
			if (RenderThread != null) RenderThread.Join();
		}

		public void DisableButtons()
		{
			btnRender.Enabled = false;
			btnRenderNoCopy.Enabled = false;
		}

		public delegate void EnableButtonsDelegate();
		public void DoEnableButtons()
		{
			btnRender.Enabled = true;
			btnRenderNoCopy.Enabled = true;
		}

		public void EnableButtons()
		{
			BeginInvoke(new EnableButtonsDelegate(DoEnableButtons));
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			DisableButtons();
			RenderThread = new Thread(CSyclesRender.Renderer);
			Renderer.SkipCopy = false;
			RenderThread.Start(Renderer);
		}

		private void btnRenderWithout_Click(object sender, EventArgs e)
		{
			DisableButtons();
			RenderThread = new Thread(CSyclesRender.Renderer);
			Renderer.SkipCopy = true;
			RenderThread.Start(Renderer);
		}
	}
}
