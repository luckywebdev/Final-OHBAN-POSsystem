using System.Drawing;
using System.Windows.Forms;

namespace STV01
{
    internal class CreateFlowLayoutPanel : FlowLayoutPanel
    {
        private Form panel;
        private int v1;
        private int v2;
        private int v3;
        private BorderStyle fixedSingle;
        private Color color;

        public CreateFlowLayoutPanel(Form panel, int v1, int v2, int v3, int height, BorderStyle fixedSingle, Color color)
        {
            this.panel = panel;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            Height = height;
            this.fixedSingle = fixedSingle;
            this.color = color;
        }
    }
}