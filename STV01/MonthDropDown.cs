using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class MonthDropDown
    {
        Constant constants = new Constant();
        private bool isCollapsed = true;
        Timer timer = null;
        CreatePanel createPanel = new CreatePanel();
        Panel mainPanelGlobal = null;
        Button mainButtonGlobal = null;
        FalsePurchaseCancellation falsePurchaseCancellation = null;
        Button[] submenuButton = null;
        string objecNameGlobal = "";
        public void initFalsePurchaseCancellation(FalsePurchaseCancellation sendHandler)
        {
            falsePurchaseCancellation = sendHandler;
        }
        public void initValue()
        {
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 5;
            timer.Tick += new System.EventHandler(this.timer_Tick);

        }

        public void CreateDropDown(string objectName, Panel mainPanel, string[] menuItemArray, int left, int top, int width, int height, int maxWidth, int maxHeight, int minWidth, int minHeight, Color mainItemBackColor, Color subItemBackColor)
        {
            initValue();
            objecNameGlobal = objectName;
            Panel mainPanels = createPanel.CreateSubPanel(mainPanel, left, top, width, height, BorderStyle.None, Color.Transparent);
            mainPanels.MaximumSize = new Size(maxWidth, maxHeight);
            mainPanels.MinimumSize = new Size(minWidth, minHeight);
            mainPanelGlobal = mainPanels;

            int buttonWidth = maxWidth;
            int buttonHeight = maxHeight / (menuItemArray.Length + 1);

            Button mainButton = new Button();
            mainButton.BackColor = Color.FromArgb(255, 255, 160, 120);
            mainButton.Dock = DockStyle.Top;
            mainButton.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            using (Bitmap img = new Bitmap(constants.dropdownArrowDownIcon))
            {
                mainButton.BackgroundImage = new Bitmap(img);
            }

            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.Location = new Point(0, 0);
            mainButton.Name = "mainButton";
            mainButton.Size = new Size(buttonWidth, buttonHeight);
            mainButton.TabIndex = 0;
            mainButton.Text = menuItemArray[0];
            mainPanels.Controls.Add(mainButton);
            mainButtonGlobal = mainButton;
            mainButton.Click += new EventHandler(this.showDropDown);
            // mainButton.UseVisualStyleBackColor = false;

            //  mainButton.Click += new EventHandler(this.button1_Click);
            int k = 0;
            int len = menuItemArray.Length;
            submenuButton = new Button[len];

            foreach (string menuItem in menuItemArray)
            {
                Button submenuButtons = new Button();
                //submenuButtons.Dock = DockStyle.Top;
                if (k == 0)
                {
                    submenuButtons.BackColor = Color.FromArgb(255, 10, 22, 231);
                    submenuButtons.ForeColor = Color.White;
                }
                submenuButtons.Location = new Point(0, buttonHeight * (k + 1));
                submenuButtons.Name = "submenuButton_" + k;
                submenuButtons.Size = new Size(buttonWidth, buttonHeight);
                submenuButtons.TabIndex = k + 1;
                submenuButtons.Text = menuItemArray[k];
                //submenuButtons.UseVisualStyleBackColor = true;
                mainPanels.Controls.Add(submenuButtons);
                submenuButtons.Click += new EventHandler(this.showTable);
                submenuButton[k] = submenuButtons;

                k++;
            }


        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                using (Bitmap img = new Bitmap(constants.dropdownArrowUpIcon))
                {
                    mainButtonGlobal.BackgroundImage = new Bitmap(img);
                }

                mainPanelGlobal.Height += 10;
                if (mainPanelGlobal.Size == mainPanelGlobal.MaximumSize)
                {
                    timer.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                using (Bitmap img = new Bitmap(constants.dropdownArrowDownIcon))
                {
                    mainButtonGlobal.BackgroundImage = new Bitmap(img);
                }
                mainPanelGlobal.Height -= 10;
                if (mainPanelGlobal.Size == mainPanelGlobal.MinimumSize)
                {
                    timer.Stop();
                    isCollapsed = true;
                }
            }
        }

        private void showDropDown(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void showTable(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            mainButtonGlobal.Text = btnTemp.Text;
            foreach (Button btnItem in submenuButton)
            {
                if (btnItem == btnTemp)
                {
                    btnItem.BackColor = Color.FromArgb(255, 10, 22, 231);
                    btnItem.ForeColor = Color.White;
                }
                else
                {
                    btnItem.BackColor = Color.Transparent;
                    btnItem.ForeColor = Color.Black;
                }
            }
            string sendIndex = btnTemp.Name.Split('_')[1].ToString();
            switch (objecNameGlobal)
            {
                case "falsePurchaseCancellation":
                    //  soldoutSetting1Global.setVal(sendIndex);
                    timer.Start();
                    break;
            }
        }
    }
}
