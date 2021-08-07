using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSetting
{
    class CreateButton
    {
        public int borderSizeGlobal { get; set; }
        public Color borderColorGlobal { get; set; }
        public Button CreateCustomButton(string btnText, string btnName, int btnLeft, int btnTop, int btnWidth, int btnHeight, Color backColor, Color borderColor, int borderSize, int radiusValue = 20, int fontSize = 12, FontStyle fontStyle = FontStyle.Regular, Color foreColor = default(Color), ContentAlignment textAlign = ContentAlignment.MiddleCenter)
        {
            RoundedButton btn = new RoundedButton();
            btn.Name = btnName;
            btn.Text = btnText;
            btn.ForeColors = foreColor;
            int xCordinator = btnLeft;

            btn.Location = new Point(btnLeft, btnTop);
            btn.Width = btnWidth;
            btn.Height = btnHeight;
            btn.BackColor = backColor;
            //btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            // btn.FlatAppearance.BorderColor = borderColor;
            // btn.FlatAppearance.BorderSize = borderSize;
            borderSizeGlobal = borderSize;
            btn.Font = new Font("Seri", fontSize, fontStyle);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.radiusValue = radiusValue;
            borderColorGlobal = borderColor;
            btn.borderColor = borderColorGlobal;
            btn.borderSize = borderSizeGlobal;
            return btn;
        }

        public Button CreateButtonWithImage(Image btnImage, string btnName, string btnText, int btnLeft, int btnTop, int btnWidth, int btnHeight, int borderSize, int radiusValue, int fontSize = 12, FontStyle fontStyle = FontStyle.Regular, Color foreColor = default(Color), ContentAlignment textAlign = ContentAlignment.MiddleCenter, int diffValue = 1)
        {
            RoundedButton btn = new RoundedButton();
            btn.Name = btnName;
            btn.text = btnText;
            btn.ForeColors = foreColor;
            btn.BackgroundImage = btnImage;
            btn.BackgroundImageLayout = ImageLayout.Stretch;
            btn.Location = new Point(btnLeft, btnTop);
            btn.Size = new Size(btnWidth, btnHeight);
            btn.Font = new Font("Seri", fontSize, fontStyle);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.radiusValue = radiusValue;
            btn.borderColor = Color.Transparent;
            btn.borderSize = borderSize;
            btn.TabStop = false;
            btn.diffValue = diffValue;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255, 255);
            return btn;

        }
    }
}
