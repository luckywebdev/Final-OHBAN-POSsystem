using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class CreateTextBox
    {
        public TextBox CreateTextBoxs_panel(Panel mainPanel, string tBoxName, int tBoxLeft, int tBoxTop, int tBoxWidth, int tBoxHeight, int tBoxFontSize, BorderStyle tBoxBorderStyle, string tBoxText = "")
        {
            TextBox tBox = new TextBox();
            tBox.Location = new Point(tBoxLeft, tBoxTop);
            tBox.Size = new Size(tBoxWidth, tBoxHeight);
            tBox.Font = new Font("Microsoft Sans Serif", tBoxFontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            tBox.BorderStyle = tBoxBorderStyle;
            tBox.TextAlign = HorizontalAlignment.Right;
            tBox.Name = tBoxName;
            tBox.Text = tBoxText;
            mainPanel.Controls.Add(tBox);
            return tBox;

        }

        public TextBox CreateTextBoxs(FlowLayoutPanel mainPanel, string tBoxName, int tBoxLeft, int tBoxTop, int tBoxWidth, int tBoxHeight, int tBoxFontSize, BorderStyle tBoxBorderStyle, string tBoxText = "")
        {
            TextBox tBox = new TextBox();
            tBox.Location = new Point(tBoxLeft, tBoxTop);
            tBox.Size = new Size(tBoxWidth, tBoxHeight);
            tBox.Font = new Font("Microsoft Sans Serif", tBoxFontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            tBox.BorderStyle = tBoxBorderStyle;
            tBox.TextAlign = HorizontalAlignment.Right;
            tBox.Name = tBoxName;
            tBox.Text = tBoxText;
            mainPanel.Controls.Add(tBox);
            return tBox;

        }
    }
}
