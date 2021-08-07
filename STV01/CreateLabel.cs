using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class CreateLabel
    {
        private Label labelGlobal = null;
        int thickness = 4;//it's up to you
        Color labelBorderColors = Color.FromArgb(255, 0, 176, 80);
        public Label CreateLabelsInPanel (Panel labelPanel, string labelName, string labelText, int labelLeft, int labelTop, int labelWidth, int labelHeight, Color labelBackColor, Color labelForeColor, int labelFontSize, bool borderFlag = false, ContentAlignment labelTextAlignment = ContentAlignment.MiddleCenter)
        {
            Label labelItem = new Label();

            labelItem.Location = new Point(labelLeft, labelTop);
            labelItem.Size = new Size(labelWidth, labelHeight);
            labelItem.Text = labelText;
            labelItem.Name = labelName;
            labelItem.Cursor = Cursors.Hand;
            labelItem.AutoEllipsis = true;
            labelItem.AllowDrop = true;
            labelItem.BackColor = labelBackColor;
            labelItem.ForeColor = labelForeColor;
            labelItem.Margin = new Padding(5, 5, 5, 5);
            labelItem.Font = new Font("Serif", labelFontSize, FontStyle.Bold);
            if (borderFlag)
            {
                // labelItem.BorderStyle = BorderStyle.FixedSingle;
                labelGlobal = labelItem;
                labelItem.Paint += new PaintEventHandler(this.labelBorder_Paint);
            }
            labelItem.TextAlign = labelTextAlignment;
            if (labelPanel.InvokeRequired)
            {
                labelPanel.Invoke(new MethodInvoker(delegate
                {
                    labelPanel.Controls.Add(labelItem);
                }));
            }
            else
            {
                labelPanel.Controls.Add(labelItem);
            }
            return labelItem;
        }
        public Label CreateLabels(FlowLayoutPanel labelPanel, string labelName, string labelText, int labelLeft, int labelTop, int labelWidth, int labelHeight, Color labelBackColor, Color labelForeColor, int labelFontSize, bool borderFlag = false, ContentAlignment labelTextAlignment = ContentAlignment.MiddleCenter, Padding labelMargin = default(Padding), int labelThickness = 4, Color labelBorderColor = default(Color), string labelBorderStyle = "rectangle")
        {
            Label labelItem = new Label();
            thickness = labelThickness;
            labelItem.Location = new Point(labelLeft, labelTop);
            labelItem.Size = new Size(labelWidth, labelHeight);
            labelItem.Text = labelText;
            labelItem.Name = labelName;
            labelItem.Cursor = Cursors.Hand;
            labelItem.AutoEllipsis = true;
            labelItem.AllowDrop = true;
            labelItem.BackColor = labelBackColor;
            labelItem.ForeColor = labelForeColor;
            labelItem.Margin = labelMargin;
            labelItem.Font = new Font("Seri", labelFontSize, FontStyle.Bold);
            labelItem.TextAlign = labelTextAlignment;
            if (borderFlag)
            {
                // labelItem.BorderStyle = BorderStyle.FixedSingle;
                
                labelBorderColors = labelBorderColor;
                labelGlobal = labelItem;
                if(labelBorderStyle == "rectangle")
                {
                    labelItem.Paint += new PaintEventHandler(labelBorder_Paint);
                }
                else if(labelBorderStyle == "top_bottom_line")
                {
                    labelItem.Paint += new PaintEventHandler(this.labelBorder_Top_Bottom_Paint);
                }
            }
            if (labelPanel.InvokeRequired)
            {
                labelPanel.Invoke(new MethodInvoker(delegate
                {
                    labelPanel.Controls.Add(labelItem);
                }));
            }
            else
            {
                labelPanel.Controls.Add(labelItem);
            }
            return labelItem;
        }


        public void labelBorder_Paint(object sender, PaintEventArgs e)
        {
            //  if (labelGlobal.BorderStyle == BorderStyle.FixedSingle)
            //  {
            Label lbTemp = (Label)sender;
            int halfThickness = thickness / 2;
            using (Pen p = new Pen(labelBorderColors, thickness))
            {
                e.Graphics.DrawRectangle(p, new Rectangle(halfThickness,
                                                          halfThickness,
                                                          lbTemp.Width - thickness,
                                                          lbTemp.Height - thickness));
            }
            //   }
        }
        public void labelBorder_Top_Bottom_Paint(object sender, PaintEventArgs e)
        {
            //  if (labelGlobal.BorderStyle == BorderStyle.FixedSingle)
            //  {
            Label lbTemp = (Label)sender;
            int halfThickness = thickness / 2;
            using (Pen p = new Pen(labelBorderColors, thickness))
            {
                e.Graphics.DrawLine(p, halfThickness, halfThickness, lbTemp.Width - thickness, halfThickness);
                e.Graphics.DrawLine(p, halfThickness, lbTemp.Height - thickness, lbTemp.Width - thickness, lbTemp.Height - thickness);
            }
            //   }
        }

        public BorderLabel CreateBorderLabel(int nX, int nY, int W, int H, string name, string text, Color borderClr, int borderSize, Font font, Color foreClr, ContentAlignment textAlign = ContentAlignment.MiddleCenter)
        {
            BorderLabel bl = new BorderLabel();

            //bl.AutoSize = true;
            bl.BackColor = Color.Transparent;
            bl.BorderColor = borderClr;
            bl.BorderSize = borderSize;
            bl.Font = font;
            bl.ForeColor = foreClr;
            bl.Location = new Point(nX, nY);
            bl.Size = new Size(W, H);
            bl.Name = name;
            bl.Text = text;
            bl.TextAlign = textAlign;
            return bl;
        }


    }
}
