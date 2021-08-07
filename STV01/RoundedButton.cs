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
    class RoundedButton: Button
    {
        public int diffValue { get; set; }
        public int radiusValue { get; set; }
        Constant constants = new Constant();

        public Color borderColor { get; set; }
        public string text { get; set; }
        public Color ForeColors { get; set; }
        public int borderSize { get; set; }
        public Color ColorTop { get; set; }
        public Color ColorBottom { get; set; }
        GraphicsPath GetRoundPath(RectangleF Rect, int radius = 10)
        {
            radiusValue = radius;
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
            GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
            GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
            GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
            GraphPath.AddArc(Rect.X + Rect.Width - radius,
                             Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
            GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
            GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
            GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
            GraphPath.CloseFigure();
            return GraphPath;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                RectangleF Rect = new RectangleF(diffValue, diffValue, this.Width, this.Height);
                using (LinearGradientBrush lgb = new LinearGradientBrush(Rect, this.ColorTop, this.ColorBottom, 90F))
                {
                    Graphics g = e.Graphics;
                    g.FillRectangle(lgb, Rect);
                }

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(255, 114, 118, 126)))
                {
                    using (StringFormat sf = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
                    {
                        e.Graphics.DrawString(text, Font, sb, Rect, sf);
                    }
                }

                Rect = new RectangleF(0, 0, this.Width, this.Height);
                using (SolidBrush sb = new SolidBrush(ForeColors))
                {
                    using (StringFormat sf = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
                    {
                        e.Graphics.DrawString(text, Font, sb, Rect, sf);
                    }
                }
                if (radiusValue != 0)
                {
                    using (GraphicsPath GraphPath = GetRoundPath(Rect, radiusValue))
                    {
                        this.Region = new Region(GraphPath);
                    }
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("roundedButton====>", ex.ToString());
                return;
            }
        }
    }
}
