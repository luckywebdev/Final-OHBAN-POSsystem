using BanknoteTester;
using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TimeSetting;

namespace STV01
{
    public partial class  MaintaneceMenu: Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel mainPanelGlobal_2 = null;
        //private Button buttonGlobal = null;
        private FlowLayoutPanel[] menuFlowLayoutPanelGlobal = new FlowLayoutPanel[4];
        Constant constants = new Constant();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton customButton = new CustomButton();
        MessageDialog messageDialog = new MessageDialog();
        DBClass dbClass = new DBClass();

        int userRole = 0;
        
        public MaintaneceMenu(Form1 mainForm, Panel mainPanel)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");

            if(key != null && key.GetValue("userRole") != null)
            {
                userRole = Convert.ToInt32(key.GetValue("userRole"));
            }

            mainForm.Width = width;
            mainForm.Height = height;
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;

            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));

            Label MainTitle = new Label();
            MainTitle.Location = new Point(0, 0);
            MainTitle.Width = headerPanel.Width;
            MainTitle.Height = headerPanel.Height;
            MainTitle.TextAlign = ContentAlignment.MiddleCenter;
            MainTitle.Font = new Font("Seri", 36, FontStyle.Bold);
            MainTitle.ForeColor = Color.FromArgb(255, 0, 0, 0);
            MainTitle.Text = constants.main_Menu_Title;
            headerPanel.Controls.Add(MainTitle);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            dbClass.InsertLog(5, "メンテナンス画面操作", "");
            for (int i = 0; i < 4; i++)
            {
                FlowLayoutPanel menuFlowLayoutPanel = createPanel.CreateFlowLayoutPanel(bodyPanel, bodyPanel.Width / 14, bodyPanel.Height / 10 + ((bodyPanel.Height * 1) / 5) * i, bodyPanel.Width * 7 / 8, bodyPanel.Height * 1 / 5 - 10, Color.White, new Padding(0));
                if(i == 0)
                {

                    Label menuLabel = createLabel.CreateLabels(menuFlowLayoutPanel, "menuLabel_" + i, constants.maintanenceLabel[i], 0, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height, Color.Transparent, Color.FromArgb(100, 255, 0, 0), 22, false, ContentAlignment.MiddleCenter);
                    string btnImage = constants.maitanenceButtonImage[i];
                    Button menuButton_1 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_1", constants.maintanenceButton[i][0], menuLabel.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.FromArgb(250, 235, 236, 243), ContentAlignment.MiddleCenter, 0);
                    menuButton_1.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);

                    menuFlowLayoutPanel.Controls.Add(menuButton_1);
                    menuButton_1.Click += new EventHandler(this.showSetting);

                    Button menuButton_2 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_2", constants.maintanenceButton[i][1], menuButton_1.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.FromArgb(250, 235, 236, 243), ContentAlignment.MiddleCenter, 0);
                    menuButton_2.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                    menuFlowLayoutPanel.Controls.Add(menuButton_2);
                    menuButton_2.Click += new EventHandler(this.showSetting);

                    Button menuButton_3 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_3", constants.maintanenceButton[i][2], menuButton_2.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.FromArgb(250, 235, 236, 243), ContentAlignment.MiddleCenter, 0);
                    menuButton_3.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                    menuFlowLayoutPanel.Controls.Add(menuButton_3);
                    menuButton_3.Click += new EventHandler(this.showSetting);

                    Button menuButton_4 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_4", constants.maintanenceButton[i][3], menuButton_3.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.FromArgb(250, 235, 236, 243), ContentAlignment.MiddleCenter, 0);
                    menuButton_4.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                    menuFlowLayoutPanel.Controls.Add(menuButton_4);
                    menuButton_4.Click += new EventHandler(this.showSetting);
                }
                else
                {
                    if(userRole != 1)
                    {
                        if(i == 1)
                        {
                            Label menuLabel = createLabel.CreateLabels(menuFlowLayoutPanel, "menuLabel_" + i, constants.maintanenceLabel[i], 0, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height, Color.Transparent, Color.Red, 22, false, ContentAlignment.MiddleCenter);

                            string btnImage = constants.maitanenceButtonImage[i];

                            Button menuButton_1 = customButton.CreateButtonWithImage(btnImage, "menuButton_3_3", constants.maintanenceButton[3][2], menuLabel.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                            menuButton_1.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                            menuFlowLayoutPanel.Controls.Add(menuButton_1);
                            menuButton_1.Click += new EventHandler(this.showSetting);

                            Button menuButton_2 = customButton.CreateButtonWithImage(btnImage, "menuButton_1_3", constants.maintanenceButton[1][2], menuButton_1.Right, 0, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 16, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                            menuButton_2.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                            menuFlowLayoutPanel.Controls.Add(menuButton_2);
                            menuButton_2.Click += new EventHandler(this.showSetting);

                            //Button menuButton_3 = customButton.CreateButtonWithImage(btnImage, "menuButton_2_1", constants.maintanenceButton[2][0], menuButton_2.Right, 0, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 16, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                            //menuButton_3.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                            //menuFlowLayoutPanel.Controls.Add(menuButton_3);
                            //menuButton_3.Click += new EventHandler(this.showSetting);
                        }
                    }
                    else if (userRole == 1)
                    {
                        int ftSize = 18;
                        if (i == 1)
                        {
                            ftSize = 14;
                        }
                        Label menuLabel = createLabel.CreateLabels(menuFlowLayoutPanel, "menuLabel_" + i, constants.maintanenceLabel[i], 0, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height, Color.Transparent, Color.Red, 22, false, ContentAlignment.MiddleCenter);
                        string btnImage = constants.maitanenceButtonImage[i];
                        Button menuButton_1 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_1", constants.maintanenceButton[i][0], menuLabel.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                        menuButton_1.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);

                        menuFlowLayoutPanel.Controls.Add(menuButton_1);
                        menuButton_1.Click += new EventHandler(this.showSetting);

                        Button menuButton_2 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_2", constants.maintanenceButton[i][1], menuButton_1.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                        menuButton_2.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                        menuFlowLayoutPanel.Controls.Add(menuButton_2);
                        menuButton_2.Click += new EventHandler(this.showSetting);

                        Button menuButton_3 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_3", constants.maintanenceButton[i][2], menuButton_2.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, ftSize, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                        menuButton_3.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                        menuFlowLayoutPanel.Controls.Add(menuButton_3);
                        menuButton_3.Click += new EventHandler(this.showSetting);

                        Button menuButton_4 = customButton.CreateButtonWithImage(btnImage, "menuButton_" + i + "_4", constants.maintanenceButton[i][3], menuButton_3.Right, menuFlowLayoutPanel.Height * i, menuFlowLayoutPanel.Width / 6, menuFlowLayoutPanel.Height * 2 / 3, 0, 50, ftSize, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 0);
                        menuButton_4.Margin = new Padding(menuFlowLayoutPanel.Width / 20 - 10, menuFlowLayoutPanel.Height * 1 / 6, 0, menuFlowLayoutPanel.Height * 1 / 6);
                        menuFlowLayoutPanel.Controls.Add(menuButton_4);

                        if (i == 3)
                        {
                            menuButton_4.Click += new EventHandler(this.BankNoteTest);
                        }
                        else
                        {
                            menuButton_4.Click += new EventHandler(this.showSetting);
                        }
                    }
                }
                menuFlowLayoutPanelGlobal[i] = menuFlowLayoutPanel;
            }

            Button shutdownButton = customButton.CreateButtonWithImage(constants.shutdownButton, "shutdownButton", "", bodyPanel.Width - 160, bodyPanel.Height - 60, 120, 50, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            shutdownButton.BackgroundImageLayout = ImageLayout.Stretch;
            shutdownButton.Padding = new Padding(0);
            bodyPanel.Controls.Add(shutdownButton);
            shutdownButton.Click += new EventHandler(ShutdownSystem);
        }

        private void ShutdownSystem(object sender, EventArgs e)
        {
            if(messageDialog != null)
            {
                messageDialog.PowerApplication();
                messageDialog.Shutdown_windows();
            }
        }


        private void BankNoteTest(object sender, EventArgs e)
        {
            Test bt = new Test();
            bt.Show();
        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal.Controls.Clear();
            MainMenu mainMenu = new MainMenu();
            mainMenu.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
        }

        private void showSetting(object sender, EventArgs e)
        {
            Button buttonObj = (Button)sender;
            string buttonName = buttonObj.Name;
            if (buttonName != "menuButton_2_1")
            {
                mainPanelGlobal.Controls.Clear();
            }
            switch (buttonName)
            {
                case "menuButton_0_1":
                    SoldoutSetting1 soldSetting = new SoldoutSetting1(mainFormGlobal, mainPanelGlobal);
                    soldSetting.TopLevel = false;
                    mainPanelGlobal.Controls.Add(soldSetting);
                    soldSetting.FormBorderStyle = FormBorderStyle.None;
                    soldSetting.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    soldSetting.Show();
                    break;
                case "menuButton_0_2":
                    ClosingProcess closingProcess = new ClosingProcess(mainFormGlobal, mainPanelGlobal);
                    closingProcess.TopLevel = false;
                    mainPanelGlobal.Controls.Add(closingProcess);
                    closingProcess.FormBorderStyle = FormBorderStyle.None;
                    closingProcess.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    closingProcess.Show();
                    break;
                case "menuButton_0_3":
                    mainFormGlobal.topPanelGlobal.Hide();
                    mainFormGlobal.bottomPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal_2.Show();
                    FalsePurchaseCancellation falsePurchaseCancellation = new FalsePurchaseCancellation(mainFormGlobal, mainPanelGlobal);
                    falsePurchaseCancellation.TopLevel = false;
                    mainPanelGlobal_2.Controls.Add(falsePurchaseCancellation);
                    falsePurchaseCancellation.FormBorderStyle = FormBorderStyle.None;
                    falsePurchaseCancellation.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    falsePurchaseCancellation.Show();
                    break;
                case "menuButton_0_4":
                    mainFormGlobal.topPanelGlobal.Hide();
                    mainFormGlobal.bottomPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal_2.Show();
                    LogManage logManage = new LogManage(mainFormGlobal, mainPanelGlobal);
                    logManage.TopLevel = false;
                    mainPanelGlobal_2.Controls.Add(logManage);
                    logManage.FormBorderStyle = FormBorderStyle.None;
                    logManage.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    logManage.Show();
                    break;
                case "menuButton_1_1":
                    ProductItemManagement productItemManagement = new ProductItemManagement(mainFormGlobal, mainPanelGlobal);
                    productItemManagement.TopLevel = false;
                    mainPanelGlobal.Controls.Add(productItemManagement);
                    productItemManagement.FormBorderStyle = FormBorderStyle.None;
                    productItemManagement.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    productItemManagement.Show();
                    break;
                case "menuButton_1_2":
                    CategoryList categoryList = new CategoryList(mainFormGlobal, mainPanelGlobal);
                    categoryList.TopLevel = false;
                    mainPanelGlobal.Controls.Add(categoryList);
                    categoryList.FormBorderStyle = FormBorderStyle.None;
                    categoryList.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    categoryList.Show();
                    break;
                case "menuButton_1_3":
                    AutoRestartSetting groupList = new AutoRestartSetting(mainFormGlobal, mainPanelGlobal);
                    groupList.TopLevel = false;
                    mainPanelGlobal.Controls.Add(groupList);
                    groupList.FormBorderStyle = FormBorderStyle.None;
                    groupList.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    groupList.Show();
                    break;
                case "menuButton_1_4":
                    TimeoutSetting timeoutSet = new TimeoutSetting(mainFormGlobal, mainPanelGlobal);
                    timeoutSet.TopLevel = false;
                    timeoutSet.FormBorderStyle = FormBorderStyle.None;
                    timeoutSet.Dock = DockStyle.Fill;
                    mainPanelGlobal.Controls.Add(timeoutSet);
                    Thread.Sleep(200);
                    timeoutSet.Show();
                    break;
                case "menuButton_2_1":
                    if (!Program.IsAdministrator())
                    {
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
                        if(key != null)
                        {
                            key.SetValue("isAdmin", true);
                        }
                        Thread.Sleep(1000);
                        Application.Restart();
                    }
                    else
                    {
                        TimeSetting.TimeSetting ts = new TimeSetting.TimeSetting();
                        ts.Show();
                    }
                    break;
                case "menuButton_2_2":
                    PasswordSetting passwordSetting = new PasswordSetting(mainFormGlobal, mainPanelGlobal);
                    passwordSetting.TopLevel = false;
                    mainPanelGlobal.Controls.Add(passwordSetting);
                    //timeSetting.Owner = 
                    passwordSetting.FormBorderStyle = FormBorderStyle.None;
                    passwordSetting.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    passwordSetting.Show();
                    break;
                case "menuButton_2_3":
                    NetConfig netConfig = new NetConfig(mainFormGlobal, mainPanelGlobal);
                    netConfig.TopLevel = false;
                    mainPanelGlobal.Controls.Add(netConfig);
                    //timeSetting.Owner = 
                    netConfig.FormBorderStyle = FormBorderStyle.None;
                    netConfig.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    netConfig.Show();
                    break;
                case "menuButton_2_4":
                    BackRestore backRestore = new BackRestore(mainFormGlobal, mainPanelGlobal);
                    backRestore.TopLevel = false;
                    mainPanelGlobal.Controls.Add(backRestore);
                    //timeSetting.Owner = 
                    backRestore.FormBorderStyle = FormBorderStyle.None;
                    backRestore.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    backRestore.Show();
                    break;
                case "menuButton_3_1":
                    LoadingItemSetting loadingItem = new LoadingItemSetting(mainFormGlobal, mainPanelGlobal);
                    loadingItem.TopLevel = false;
                    mainPanelGlobal.Controls.Add(loadingItem);
                    //timeSetting.Owner = 
                    loadingItem.FormBorderStyle = FormBorderStyle.None;
                    loadingItem.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    loadingItem.Show();
                    break;
                case "menuButton_3_2":
                    BankNoteSetting highBankNote = new BankNoteSetting(mainFormGlobal, mainPanelGlobal);
                    highBankNote.TopLevel = false;
                    mainPanelGlobal.Controls.Add(highBankNote);
                    //timeSetting.Owner = 
                    highBankNote.FormBorderStyle = FormBorderStyle.None;
                    highBankNote.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    highBankNote.Show();
                    break;
                case "menuButton_3_3":
                    mainFormGlobal.topPanelGlobal.Hide();
                    mainFormGlobal.bottomPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal.Hide();
                    mainFormGlobal.mainPanelGlobal_2.Show();
                    ProductInfoSetting productInfoSetting = new ProductInfoSetting(mainFormGlobal, mainPanelGlobal);
                    productInfoSetting.TopLevel = false;
                    mainFormGlobal.mainPanelGlobal_2.Controls.Add(productInfoSetting);
                    productInfoSetting.FormBorderStyle = FormBorderStyle.None;
                    productInfoSetting.Dock = DockStyle.Fill;
                    Thread.Sleep(200);
                    productInfoSetting.Show();

                    break;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MaintaneceMenu
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MaintaneceMenu";
            this.ResumeLayout(false);

        }


    }
}
