using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class GradientPanel: Panel
    {
        public Color ColorTop { get; set; }
        public Color ColorBottom { get; set; }
        Constant constants = new Constant();

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                using (LinearGradientBrush lgb = new LinearGradientBrush(this.ClientRectangle, this.ColorTop, this.ColorBottom, 90F))
                {
                    Graphics g = e.Graphics;
                    g.FillRectangle(lgb, this.ClientRectangle);
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("gradientPanel====>", ex.ToString());
                return;
            }
        }

    }
}
