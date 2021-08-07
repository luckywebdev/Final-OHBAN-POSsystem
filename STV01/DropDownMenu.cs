using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class DropDownMenu
    {
        Constant constants = new Constant();
        private bool isCollapsed = true;
        Timer timer = null;
        CreatePanel createPanel = new CreatePanel();
        Panel mainPanelGlobal = null;
        Panel subPanelGlobal = null;
        Button mainButtonGlobal = null;
        SoldoutSetting1 soldoutSetting1Global = null;
        CategoryList categoryListGlobal = null;
        GroupList groupListGlobal = null;
        FalsePurchaseCancellation falsePurchaseCancellation = null;
        LogDetailView logdetailViewGlobal = null;
        Button[] submenuButton = null;
        DetailView detailViewGlobal = null;
        Color mainButtonColor = default(Color);
        Color subButtonColor = default(Color);
        string objecNameGlobal = "";
        public int clickCounter = 0;

        public void InitSoldoutSetting(SoldoutSetting1 sendHandler)
        {
            soldoutSetting1Global = sendHandler;
        }
        public void InitCategoryList(CategoryList sendHandler)
        {
            categoryListGlobal = sendHandler;
        }
        public void InitGroupList(GroupList sendHandler)
        {
            groupListGlobal = sendHandler;
        }
        public void InitFalsePurchaseCancellation(FalsePurchaseCancellation sendHandler)
        {
            falsePurchaseCancellation = sendHandler;
        }
        public void InitLogReport(DetailView sendHandler)
        {
            detailViewGlobal = sendHandler;
        }
        public void InitLogDetailReport(LogDetailView sendHandler)
        {
            logdetailViewGlobal = sendHandler;
        }
        public void InitValue()
        {
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 1;
            timer.Tick += new System.EventHandler(this.Timer_Tick);

        }

        public Panel CreateDropDown(string objectName, Panel mainPanel, string[] menuItemArray, int left, int top, int width, int height, int maxWidth, int maxHeight, int minWidth, int minHeight, Color mainItemBackColor, Color subItemBackColor, int defaultItem = 0)
        {
            InitValue();
            objecNameGlobal = objectName;
            Panel mainPanels = createPanel.CreateSubPanel(mainPanel, left, top, width, height, BorderStyle.None, Color.Transparent);
            mainPanels.MaximumSize = new Size(maxWidth, maxHeight);
            mainPanels.MinimumSize = new Size(minWidth, minHeight);
            Panel subPanels = createPanel.CreateSubPanel(mainPanels, 0, height, width, height, BorderStyle.None, Color.Transparent);
            subPanels.MaximumSize = new Size(maxWidth + 30, maxHeight - 50);
            subPanels.MinimumSize = new Size(minWidth + 30, 0);
            mainPanelGlobal = mainPanels;
            subPanelGlobal = subPanels;

            int buttonWidth = maxWidth;
            int buttonHeight = maxHeight / (menuItemArray.Length + 1);

            Button mainButton = new Button();
            mainButton.BackColor = Color.White;
            mainButton.Dock = DockStyle.Top;
            mainButton.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            //mainButton.BackgroundImage = Image.FromFile(constants.dropdownArrowDownIcon);
            //mainButton.BackgroundImageLayout = ImageLayout.Stretch;
            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.Location = new Point(0, 0);
            mainButton.Name = "mainButton_0";
            mainButton.Size = new Size(buttonWidth, buttonHeight);
            mainButton.TabIndex = 0;
            mainButton.Text = menuItemArray[0];


            ImageList imageList = new ImageList();
            imageList.Images.Add(Image.FromFile(constants.dropdownarrowImage));
            imageList.ImageSize = new Size(mainButton.Height * 2 / 5, mainButton.Height * 2 / 5);

            mainButton.ImageList = imageList;
            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.ImageIndex = 0;
            mainButton.Padding = new Padding(0, 0, 20, 0);

            if (defaultItem != 0)
            {
                mainButton.Text = menuItemArray[defaultItem];
            }
            mainPanels.Controls.Add(mainButton);
            mainButtonGlobal = mainButton;
            mainButton.Click += new EventHandler(this.ShowDropDown);
            // mainButton.UseVisualStyleBackColor = false;
            mainButtonColor = mainItemBackColor;
            subButtonColor = subItemBackColor;
            //  mainButton.Click += new EventHandler(this.button1_Click);
            int k = 0;
            int len = menuItemArray.Length;
            submenuButton = new Button[len];

            foreach(string menuItem in menuItemArray)
            {
                Button submenuButtons = new Button();
                //submenuButtons.Dock = DockStyle.Top;
                if(k == defaultItem)
                {
                    submenuButtons.BackColor = mainItemBackColor;
                    submenuButtons.ForeColor = Color.White;
                }
                else
                {
                    submenuButtons.BackColor = subItemBackColor;
                    submenuButtons.ForeColor = Color.Black;
                }
                submenuButtons.Location = new Point(0, buttonHeight * k);
                submenuButtons.Name = "submenuButton_" + k;
                submenuButtons.Size = new Size(buttonWidth, buttonHeight);
                submenuButtons.TabIndex = k + 1;
                submenuButtons.Text = menuItemArray[k];
                submenuButtons.FlatStyle = FlatStyle.Flat;
                submenuButtons.FlatAppearance.BorderSize = 0;
                submenuButtons.FlatAppearance.BorderColor = Color.White;
                submenuButtons.TextAlign = ContentAlignment.MiddleLeft;
                submenuButtons.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                //submenuButtons.UseVisualStyleBackColor = true;
                subPanels.Controls.Add(submenuButtons);
                submenuButtons.MouseUp += new MouseEventHandler(this.ShowTable);

                submenuButton[k] = submenuButtons;

                k++;

            }

            //TouchScroll thScroll = new TouchScroll();
            //thScroll.TouchScrollPanel(subPanels);
            return mainPanels;
        }

        public Panel CreateCategoryDropDown(string objectName, Panel mainPanel, string[] menuItemArray, int[] menuIDArray, int[] menuDisplayPositionArray, int[] menuStateArray, int left, int top, int width, int height, int maxWidth, int maxHeight, int minWidth, int minHeight, Color mainItemBackColor, Color subItemBackColor)
        {
            InitValue();
            objecNameGlobal = objectName;
            Panel mainPanels = createPanel.CreateSubPanel(mainPanel, left, top, width, height, BorderStyle.None, Color.Transparent);
            mainPanels.MaximumSize = new Size(maxWidth, maxHeight);
            mainPanels.MinimumSize = new Size(minWidth, minHeight);
            Panel subPanels = createPanel.CreateSubPanel(mainPanels, 0, height, width, height, BorderStyle.None, Color.Transparent);
            subPanels.MaximumSize = new Size(maxWidth, maxHeight);
            subPanels.MinimumSize = new Size(minWidth, 0);
            mainPanelGlobal = mainPanels;
            subPanelGlobal = subPanels;

            int buttonWidth = maxWidth;
            int buttonHeight = maxHeight / (menuItemArray.Length + 1);

            Button mainButton = new Button();
            mainButton.BackColor = Color.White;
            mainButton.Dock = DockStyle.Top;
            mainButton.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            //   mainButton.Image = Image.FromFile(constants.dropdownArrowDownIcon);
            //   mainButton.Image = new Bitmap(Image.FromFile(constants.dropdownArrowDownIcon));
            //mainButton.BackgroundImage = Image.FromFile(constants.dropdownArrowDownIcon);
            //mainButton.BackgroundImageLayout = ImageLayout.Stretch;
            //mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.Padding = new Padding(0, 0, 10, 0);
            mainButton.Location = new Point(0, 0);
            mainButton.Name = "mainButton";
            mainButton.Size = new Size(buttonWidth, buttonHeight);
            mainButton.TabIndex = 0;
            mainButton.Font = new Font("Series", 18, FontStyle.Bold);
            mainButton.Text = menuDisplayPositionArray[0].ToString() + "-" + menuIDArray[0] + "  " +  menuItemArray[0];
            ImageList imageList = new ImageList();
            imageList.Images.Add(Image.FromFile(constants.dropdownarrowImage));
            imageList.ImageSize = new Size(mainButton.Height * 2 / 5, mainButton.Height * 2 / 5);

            mainButton.ImageList = imageList;
            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.ImageIndex = 0;
            mainButton.Padding = new Padding(0, 0, 20, 0);

            mainPanels.Controls.Add(mainButton);
            mainButtonGlobal = mainButton;
            mainButton.Click += new EventHandler(this.ShowDropDown);
            // mainButton.UseVisualStyleBackColor = false;
            mainButtonColor = mainItemBackColor;
            subButtonColor = subItemBackColor;
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
                    submenuButtons.BackColor = mainItemBackColor;
                    submenuButtons.ForeColor = Color.White;
                }
                else
                {
                    if(menuStateArray[k] == 0)
                    {
                        submenuButtons.BackColor = subItemBackColor;
                        submenuButtons.ForeColor = Color.Black;
                        submenuButtons.Enabled = true;
                    }
                    else
                    {
                        submenuButtons.BackColor = Color.FromArgb(255, 217, 217, 217);
                        submenuButtons.ForeColor = Color.Black;
                        submenuButtons.Enabled = false;
                    }
                }
                submenuButtons.Location = new Point(0, buttonHeight * k);
                submenuButtons.Name = "submenuButton_" + k;
                submenuButtons.Size = new Size(buttonWidth, buttonHeight);
                submenuButtons.TabIndex = k + 1;
                submenuButtons.Text = menuDisplayPositionArray[k].ToString() + "-" + menuIDArray[k] + "  " + menuItemArray[k];
                //submenuButtons.UseVisualStyleBackColor = true;
                submenuButtons.FlatStyle = FlatStyle.Flat;
                submenuButtons.FlatAppearance.BorderSize = 0;
                submenuButtons.FlatAppearance.BorderColor = Color.White;
                submenuButtons.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                subPanels.Controls.Add(submenuButtons);
                submenuButtons.Click += new EventHandler(this.ShowTable);
                submenuButton[k] = submenuButtons;

                k++;
            }

            return mainPanels;
        }

        public Panel CreateCategoryDropDown1(string objectName, Panel mainPanel, string[] menuItemArray, int[] menuIDArray, int[] menuDisplayPositionArray, int[] menuStateArray, int left, int top, int width, int height, int maxWidth, int maxHeight, int minWidth, int minHeight, Color mainItemBackColor, Color subItemBackColor)
        {
            InitValue();
            objecNameGlobal = objectName;

            Panel mainPanels = createPanel.CreateSubPanel(mainPanel, left, top, width, height, BorderStyle.None, Color.Transparent);
            mainPanels.MaximumSize = new Size(maxWidth, maxHeight);
            mainPanels.MinimumSize = new Size(minWidth, minHeight);
            Panel subPanels = createPanel.CreateSubPanel(mainPanels, 0, height, width, height, BorderStyle.None, Color.Transparent);
            subPanels.MaximumSize = new Size(maxWidth, maxHeight);
            subPanels.MinimumSize = new Size(minWidth, 0);
            mainPanelGlobal = mainPanels;
            subPanelGlobal = subPanels;

            int buttonWidth = maxWidth;
            int buttonHeight = maxHeight / (menuItemArray.Length + 1);

            Button mainButton = new Button();
            mainButton.BackColor = Color.White;
            mainButton.Dock = DockStyle.Top;
            
            mainButton.Font = new Font("Microsoft Sans Serif", 18, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            //mainButton.BackgroundImage = Image.FromFile(constants.dropdownArrowDownIcon);
            //mainButton.BackgroundImageLayout = ImageLayout.Stretch;
            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.Padding = new Padding(0, 0, 10, 0);
            mainButton.Location = new Point(0, 0);
            mainButton.Name = "mainButton";
            mainButton.Size = new Size(buttonWidth, buttonHeight);
            mainButton.TabIndex = 0;
            ImageList imageList = new ImageList();
            imageList.Images.Add(Image.FromFile(constants.dropdownarrowImage));
            imageList.ImageSize = new Size(mainButton.Height * 2 / 5, mainButton.Height * 2 / 5);

            mainButton.ImageList = imageList;
            mainButton.ImageAlign = ContentAlignment.MiddleRight;
            mainButton.ImageIndex = 0;
            mainButton.Padding = new Padding(0, 0, 20, 0);

            mainButton.Text = menuDisplayPositionArray[0].ToString() + "-" + menuIDArray[0] + "  " + menuItemArray[0];
            mainPanels.Controls.Add(mainButton);
            mainButtonGlobal = mainButton;
            mainButton.Click += new EventHandler(this.ShowDropDown);
            // mainButton.UseVisualStyleBackColor = false;
            mainButtonColor = mainItemBackColor;
            subButtonColor = subItemBackColor;
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
                    submenuButtons.BackColor = mainItemBackColor;
                    submenuButtons.ForeColor = Color.White;
                }
                else
                {
                    submenuButtons.BackColor = subItemBackColor;
                    submenuButtons.ForeColor = Color.Black;
                    submenuButtons.Enabled = true;
                }
                submenuButtons.Location = new Point(0, buttonHeight * k);
                submenuButtons.Name = "submenuButton_" + k;
                submenuButtons.Size = new Size(buttonWidth, buttonHeight);
                submenuButtons.TabIndex = k + 1;
                submenuButtons.Text = menuDisplayPositionArray[k].ToString() + "-" + menuIDArray[k] + "  " + menuItemArray[k];
                //submenuButtons.UseVisualStyleBackColor = true;
                submenuButtons.FlatStyle = FlatStyle.Flat;
                submenuButtons.FlatAppearance.BorderSize = 0;
                submenuButtons.FlatAppearance.BorderColor = Color.White;
                submenuButtons.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                subPanels.Controls.Add(submenuButtons);
                submenuButtons.Click += new EventHandler(this.ShowTable);
                submenuButton[k] = submenuButtons;

                k++;
            }

            return mainPanels;
        }

        string sendIndexGlobal;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                //mainButtonGlobal.BackgroundImage = Image.FromFile(constants.dropdownArrowUpIcon);
                //mainButtonGlobal.BackgroundImageLayout = ImageLayout.Stretch;

                mainPanelGlobal.Height += 20;
                subPanelGlobal.Height += 20;
                if(subPanelGlobal.Height == 200)
                {
                    mainPanelGlobal.MaximumSize = new Size(mainPanelGlobal.Width, 200 + mainButtonGlobal.Height);
                    subPanelGlobal.MaximumSize = new Size(mainPanelGlobal.Width, 200);

                    subPanelGlobal.HorizontalScroll.Maximum = 0;
                    subPanelGlobal.AutoScroll = false;
                    subPanelGlobal.VerticalScroll.Visible = false;

                     //mainPanelGlobal.AutoScrollMargin = new Size(0, 40);
                   subPanelGlobal.AutoScroll = true;
                }
                if (mainPanelGlobal.Size == mainPanelGlobal.MaximumSize)
                {
                    timer.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                //mainButtonGlobal.BackgroundImage = Image.FromFile(constants.dropdownArrowDownIcon);
                //mainButtonGlobal.BackgroundImageLayout = ImageLayout.Stretch;

                mainPanelGlobal.Height -= 20;
                subPanelGlobal.Height -= 20;
                if (subPanelGlobal.Height < 200)
                {
                    subPanelGlobal.AutoScroll = false;
                }

                if (mainPanelGlobal.Size == mainPanelGlobal.MinimumSize)
                {
                    timer.Stop();
                    isCollapsed = true;
                    if(sendIndexGlobal != null)
                    {
                        switch (objecNameGlobal)
                        {
                            case "soldoutSetting1":
                                soldoutSetting1Global.SetVal(sendIndexGlobal);
                                break;
                            case "categoryList":
                                categoryListGlobal.SetVal(sendIndexGlobal);
                                break;
                            case "groupList":
                                groupListGlobal.SetVal(sendIndexGlobal);
                                break;
                            case "logReport":
                                detailViewGlobal.SetVal("logReport", (int.Parse(sendIndexGlobal)).ToString("00"));
                                break;
                            case "logDetailReport":
                                logdetailViewGlobal.SetVal("logReport", (int.Parse(sendIndexGlobal)).ToString("00"));
                                break;
                            case "falsePurchase":
                                detailViewGlobal.SetVal("falsePurchase", (int.Parse(sendIndexGlobal)).ToString("00"));
                                break;
                        }
                    }

                }
            }
        }

        private void ShowDropDown(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void ShowTable(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            mainButtonGlobal.Text = btnTemp.Text;
            int k = 0;
            foreach (Button btnItem in submenuButton)
            {
                if (btnItem == btnTemp)
                {
                    btnItem.BackColor = mainButtonColor;
                    btnItem.ForeColor = Color.White;
                }
                else
                {
                    btnItem.BackColor = subButtonColor;
                    btnItem.ForeColor = Color.Black;
                    if (btnItem.Enabled == false)
                    {
                        btnItem.BackColor = Color.FromArgb(255, 217, 217, 217);
                        btnItem.ForeColor = Color.Black;
                    }
                }
                k++;
            }
            string sendIndex = btnTemp.Name.Split('_')[1].ToString();
            sendIndexGlobal = sendIndex;
            timer.Start();

        }


        static SQLiteConnection CreateConnection(string dbName)
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=" + dbName + ".db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }


    }
}
