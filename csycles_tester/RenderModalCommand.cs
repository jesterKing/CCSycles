using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ef = Eto.Forms;
using ccl;

namespace csycles_tester
{
	public class RenderModalCommand : ef.Command
	{

		CSyclesForm m_parent;
		public RenderModalCommand(CSyclesForm parent, string filename)
		{
			MenuText = filename;
			Tag = filename;
			m_parent = parent;
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			(m_parent.DataContext as RendererModel).RenderScene(MenuText);
			m_parent.Image.Image = (m_parent.DataContext as RendererModel).Result;
			m_parent.Invalidate();
		}
	}
}
