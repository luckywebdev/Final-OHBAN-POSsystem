using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.IO;

namespace STV01
{
    public partial class Form1 : Form
    {
        MainMenu mainMenu = new MainMenu();
        DBClass dbClass = new DBClass();
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        ComModule comModule = new ComModule();

        public Form mainFormGlobal = null;
        public Panel mainPanelGlobal = null;
        public Panel mainPanelGlobal_2 = null;
        public Panel topPanelGlobal = null;
        public Panel bottomPanelGlobal = null;

        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;

        public Form1()
        {   
            InitializeComponent();
        }

        private void Init()
        {
            mainFormGlobal = this;
            Panel topPanel = createPanel.CreateMainPanel(this, 0, 30, width, 30, BorderStyle.None, Color.FromArgb(255, 234, 158, 65));
            Panel bottomPanel = createPanel.CreateMainPanel(this, 0, height - 60, width, 30, BorderStyle.None, Color.FromArgb(255, 234, 158, 65));

            topPanelGlobal = topPanel;
            bottomPanelGlobal = bottomPanel;

            Panel mainPanel = createPanel.CreateMainPanel(this, width / 10, height / 9, width * 8 / 10, height * 7 / 9 - 30, BorderStyle.None, Color.FromArgb(255, 249, 246, 224));
            mainPanelGlobal = mainPanel;

            Panel mainPanel_2 = createPanel.CreateMainPanel(this, 0, 0, width, height, BorderStyle.None, Color.White);
            mainPanelGlobal_2 = mainPanel_2;

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null && key.GetValue("POSAdminPassword") == null)
            {
                key.SetValue("POSAdminPassword", "8765");
            }
            if (key != null && key.GetValue("POSPassword") == null)
            {
                key.SetValue("POSPassword", "5678");
            }
            if (key != null)
            {
                key.SetValue("userRole", "0");
            }

            //try
            //{
            //    string logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //    logPath += "\\STV01\\";
            //    string fileName = logPath + "log_process_data.txt";
            //    if (File.Exists(fileName))
            //    {
            //        File.Delete(fileName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}

            this.BackColor = Color.White;
            this.TopLevel = true;

            try
            {
                dbClass.CreateProductOptionTB();
                dbClass.CreateProductOptionValueTB();
                dbClass.DBChecking();
                if (dbClass.dbState)
                {
                    if(key.GetValue("isAdmin") != null && Convert.ToBoolean(key.GetValue("isAdmin")))
                    {
                        mainPanelGlobal_2.Hide();
                        mainMenu.CreateMainMenuScreen(this, mainPanel);
                        topPanelGlobal.Show();
                        bottomPanelGlobal.Show();
                        mainPanelGlobal.Show();
                    }
                    else
                    {
                        topPanelGlobal.Hide();
                        bottomPanelGlobal.Hide();
                        mainPanelGlobal.Hide();
                        dbClass.InsertLog(5, "電源投入", "");
                        SaleScreen saleScreenMenu = new SaleScreen(this, mainMenu, mainPanel, comModule);
                        saleScreenMenu.TopLevel = false;
                        saleScreenMenu.FormBorderStyle = FormBorderStyle.None;
                        saleScreenMenu.Dock = DockStyle.Fill;
                        this.mainPanelGlobal_2.Controls.Add(saleScreenMenu);
                        mainPanelGlobal_2.Show();
                    }
                }
                else
                {
                    mainPanelGlobal_2.Hide();
                    mainMenu.CreateMainMenuScreen(this, mainPanel);
                    topPanelGlobal.Show();
                    bottomPanelGlobal.Show();
                    mainPanelGlobal.Show();
                }
            }
            catch (Exception ex)
            {
                mainPanelGlobal_2.Hide();
                mainMenu.CreateMainMenuScreen(this, mainPanel);
                topPanelGlobal.Show();
                bottomPanelGlobal.Show();
                mainPanelGlobal.Show();
                Console.WriteLine(ex.ToString());
                return;
            }
        }


        protected override void OnClosed(EventArgs e)
        {
        }

        public string comport = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            string maplePortName = GetFullComputerDevices();
            comport = maplePortName.Substring(maplePortName.Length - 5, 4);

            Init();
            panel1.Visible = false;
        }

        public void ShowLoading()
        {
            if (panel1.InvokeRequired)
            {
                panel1.Invoke((MethodInvoker)delegate ()
                {
                    panel1.Visible = true;
                    panel1.BringToFront();
                });
            }
            else
            {
                panel1.Visible = true;
                panel1.BringToFront();
            }
        }

        public void HideLoading()
        {
            if (panel1.InvokeRequired)
            {
                panel1.Invoke((MethodInvoker)delegate ()
                {
                    panel1.Visible = false;
                    panel1.SendToBack();
                });
            }
            else
            {
                panel1.Visible = false;
                panel1.SendToBack();
            }
        }

        private string GetFullComputerDevices()
        {
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");

            ManagementObjectCollection Ports = processClass.GetInstances();
            string device = "No recognized";
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("Maple Serial") &&
                        property.GetPropertyValue("Name").ToString().Contains("COM"))
                    {
                        device = property.GetPropertyValue("Name").ToString();
                        break;
                    }
            }
            return device;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("checkingState") != null)
                {
                    key.SetValue("checkingState", 0);
                }
                else
                {
                    key.SetValue("checkingState", 0);
                }
            }

            if (mainMenu != null)
            {
                if (mainMenu.sumThreadm != null)
                {
                    mainMenu.sumThreadm.Abort();
                    mainMenu.sumThreadm = null;
                    mainMenu.flag = false;
                }

                mainMenu.CloseComport();
                mainMenu.Dispose();
            }
            Application.Exit();
            base.OnClosed(e);
        }

    }
}