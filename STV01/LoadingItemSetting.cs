using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class LoadingItemSetting : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel bodyPanelGlobal = null;
        Constant constants = new Constant();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton customButton = new CustomButton();
        RegistryKey key = null;

        bool HDCheck = true;

        RadioButton r1Global = null;
        RadioButton r2Global = null;

        public LoadingItemSetting(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.HDCheckTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            bodyPanelGlobal = bodyPanel;

            Label notifyLabel1 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel1", "デフォルト：全てのハードウエアはマウント", bodyPanel.Width / 2, 100, bodyPanel.Width / 2, 60, Color.Transparent, Color.Red, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel1.Padding = new Padding(0, 0, 20, 0);

            Label notifyLabel2 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel2", "リブート後：全てのハードウエアはマウント", bodyPanel.Width / 2, 160, bodyPanel.Width / 2, 60, Color.Transparent, Color.Red, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel2.Padding = new Padding(0, 0, 20, 0);

            Label titleLabel = createLabel.CreateLabelsInPanel(bodyPanel, "titleLabel", "リブート後：全てのハードウエアはマウント", 0, bodyPanel.Height / 2 - 60, bodyPanel.Width, 60, Color.Transparent, Color.BlueViolet, 20, false, ContentAlignment.MiddleCenter);

            Panel radioPanel = new Panel();
            radioPanel.Location = new Point(bodyPanel.Width / 2, titleLabel.Bottom + 20);
            radioPanel.Size = new Size(bodyPanel.Width / 2, 60);
            bodyPanel.Controls.Add(radioPanel);

            RadioButton r1 = new RadioButton();
            r1.Text = "全て Mount";
            r1.Location = new Point(0, 10);
            r1.Font = new Font("Serif", 26);
            r1.AutoSize = true;
            r1.Checked = true;
            r1.Name = "r1";
            r1.CheckedChanged += new EventHandler(this.ItemSetting);
            r1Global = r1;
            radioPanel.Controls.Add(r1);

            RadioButton r2 = new RadioButton();
            r2.Text = "全て Un Mount";
            r2.Location = new Point(r1.Right + 20, 10);
            r2.Font = new Font("Serif", 26);
            r2.AutoSize = true;
            r2.Name = "r2";
            r2.CheckedChanged += new EventHandler(this.ItemSetting);
            r2Global = r2;
            radioPanel.Controls.Add(r2);

            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("HDCheck") != null)
                {
                    HDCheck = Convert.ToBoolean(key.GetValue("HDCheck"));
                    if (HDCheck)
                    {
                        r1.Checked = true;
                    }
                    else
                    {
                        r2.Checked = true;
                    }
                }
            }

            Button saveBtn = customButton.CreateButtonWithImage(constants.rectRedButton, "saveButton", constants.confirmLabel, bodyPanel.Width - 300, bodyPanel.Height - 100, 100, 50, 2, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(saveBtn);
            saveBtn.Click += new EventHandler(this.SaveData);

            Button backBtn = customButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, bodyPanel.Width - 150, bodyPanel.Height - 100, 100, 50, 2, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(backBtn);
            backBtn.Click += new EventHandler(this.BackShow);

            InitializeComponent();
        }

        private void ItemSetting(object sender, EventArgs e)
        {
            RadioButton rTemp = (RadioButton)sender;
            switch (rTemp.Name)
            {
                case "r1":
                    HDCheck = true;
                    break;
                case "r2":
                    HDCheck = false;
                    break;
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("HDCheck", HDCheck);
            }

            MainMenu pMm = new MainMenu();
            pMm.HDChecking = HDCheck;

            mainPanelGlobal.Controls.Clear();
            MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();

        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal.Controls.Clear();
            MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
        }

    }
}
