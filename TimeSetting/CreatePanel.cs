using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSetting
{
    class CreatePanel
    {
        public Panel CreateMainPanel(Form formpanel, int PanelLeft, int PanelTop, int PanelWidth, int PanelHeight, BorderStyle borderstyle, Color colors)
        {
            Panel Panel = new Panel();
            Panel.Size = new Size(PanelWidth, PanelHeight);
            Panel.Location = new Point(PanelLeft, PanelTop);
            Panel.BorderStyle = borderstyle;
            Panel.BackColor = colors;
            formpanel.Controls.Add(Panel);
            return Panel;
        }
        public Panel CreateSubPanel(Panel parentPanel, int PanelLeft, int PanelTop, int PanelWidth, int PanelHeight, BorderStyle borderstyle, Color color, Color colorTop = default(Color), Color colorBottom = default(Color))
        {
            Panel Panel = new Panel();
            Panel.Size = new Size(PanelWidth, PanelHeight);
            Panel.Location = new Point(PanelLeft, PanelTop);
            Panel.BorderStyle = borderstyle;
            Panel.BackColor = color;

            if (parentPanel.InvokeRequired)
            {
                parentPanel.Invoke((MethodInvoker)delegate
                {
                    parentPanel.Controls.Add(Panel);
                });
            }
            else
            {
                parentPanel.Controls.Add(Panel);
            }

            return Panel;
        }
        public FlowLayoutPanel CreateFlowLayoutPanel(Panel panel, int PanelLeft, int PanelTop, int PanelWidth, int PanelHeight, Color color, Padding paddings, bool borderEnable = false)
        {
            FlowLayoutPanel FlowPanel = new FlowLayoutPanel();
            FlowPanel.Size = new Size(PanelWidth, PanelHeight);
            FlowPanel.Location = new Point(PanelLeft, PanelTop);
            FlowPanel.BackColor = color;
            if (borderEnable)
            {
                FlowPanel.BorderStyle = BorderStyle.FixedSingle;
            }
            FlowPanel.Padding = paddings;

            if (panel.InvokeRequired)
            {
                panel.Invoke((MethodInvoker)delegate
                {
                    panel.Controls.Add(FlowPanel);
                });
            }
            else
            {
                panel.Controls.Add(FlowPanel);
            }

            return FlowPanel;
        }

    }
}
