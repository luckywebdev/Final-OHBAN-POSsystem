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
    class CustomButton
    {
        public int borderSizeGlobal { get; set; }
        public Color borderColorGlobal { get; set; }
        Constant constants = new Constant();

        public Button CreateButton(string btnText, string btnName, int btnLeft, int btnTop, int btnWidth, int btnHeight, Color backColor, Color borderColor, int borderSize, int radiusValue = 20, int fontSize = 12, FontStyle fontStyle = FontStyle.Regular, Color foreColor = default(Color), ContentAlignment textAlign = ContentAlignment.MiddleCenter)
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
        public Button CreateButtonWithImage(string btnImage, string btnName, string btnText, int btnLeft, int btnTop, int btnWidth, int btnHeight, int borderSize, int radiusValue, int fontSize = 12, FontStyle fontStyle = FontStyle.Regular, Color foreColor = default(Color), ContentAlignment textAlign = ContentAlignment.MiddleCenter, int diffValue = 1)
        {
            RoundedButton btn = null;
            try
            {
                btn = new RoundedButton();
                btn.Name = btnName;
                btn.text = btnText;
                btn.ForeColors = foreColor;
                using(Bitmap img = new Bitmap(btnImage))
                {
                    btn.BackgroundImage = new Bitmap(btnImage);
                }
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
            catch (Exception ex)
            {
                constants.SaveLogData("roundedButton====>", ex.ToString());
                return btn;
            }
        }

        public Button CreatePolyButtonWithStringColr(int nX, int nY, Point[] pts, string name, string text, bool hasBD,
          string borderClr, string foreClr, string backClr, int ftSize = 12)
        {
            Button dynamicButton = new Button();
            // Make the GraphicsPath.
            GraphicsPath polygon_path = new GraphicsPath(FillMode.Winding);
            polygon_path.AddPolygon(pts);

            // Convert the GraphicsPath into a Region.
            Region polygon_region = new Region(polygon_path);


            // Constrain the button to the region.
            dynamicButton.Region = polygon_region;

            // Make the button big enough to hold the whole region.
            dynamicButton.SetBounds(dynamicButton.Location.X, dynamicButton.Location.Y, pts[2].X, pts[2].Y);

            dynamicButton.Name = name;
            dynamicButton.BackColor = ColorTranslator.FromHtml(backClr);
            dynamicButton.Text = text;
            dynamicButton.ForeColor = ColorTranslator.FromHtml(foreClr);
            dynamicButton.Font = new Font("Serif", ftSize, FontStyle.Bold);
            dynamicButton.FlatStyle = FlatStyle.Flat;
            dynamicButton.TextAlign = ContentAlignment.MiddleCenter;

            /*//dynamicButton.Location = new Point(nX, nY);
           */
            if (hasBD)
            {
                dynamicButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml(borderClr);
                dynamicButton.FlatAppearance.BorderSize = 4;
            }
            else
                dynamicButton.FlatAppearance.BorderSize = 0;


            //int width = pts[1].X - pts[3].X;
            //int fontCnt = text.Length/2;
            //int fontLen = ftSize * fontCnt;
            //int paddingRight = (width - fontLen) / 2 + 30;
            dynamicButton.Padding = new Padding(pts[0].X, 0, 0, 0);
            return dynamicButton;
        }
    }
}
