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
    public partial class TimeoutSetting : Form
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

        int timeoutValue = 4;
        bool categoryMain = false;

        RadioButton r1Global = null;
        RadioButton r5Global = null;
        RadioButton r6Global = null;

        DateTime now = DateTime.Now;

        public TimeoutSetting(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.timeoutSetTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            bodyPanelGlobal = bodyPanel;

            Label titleLabel = createLabel.CreateLabelsInPanel(bodyPanel, "titleLabal", "タイムアウト設定", 50, 100, bodyPanel.Width / 4, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            titleLabel.Padding = new Padding(0, 0, 20, 0);

            Label notifyLabel = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel", "※金銭入金状態は適応外", titleLabel.Right + 10, 100, bodyPanel.Width / 3, 60, Color.Transparent, Color.Red, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel.Padding = new Padding(0, 0, 20, 0);

            Label settingLabel = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel", "無操作時間 :", 80, titleLabel.Bottom + 20, bodyPanel.Width / 4, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel.Padding = new Padding(0, 0, 20, 0);

            Panel radioPanel = new Panel();
            radioPanel.Location = new Point(settingLabel.Right + 10, titleLabel.Bottom + 20);
            radioPanel.Size = new Size(bodyPanel.Width * 2 / 3 - 80, 60);
            bodyPanel.Controls.Add(radioPanel);

            RadioButton r1 = new RadioButton();
            r1.Text = "40秒";
            r1.Location = new Point(0, 10);
            r1.Font = new Font("Serif", 26);
            r1.AutoSize = true;
            r1.Checked = true;
            r1.Name = "r1";
            r1.CheckedChanged += new EventHandler(this.TimeoutSet);
            r1Global = r1;
            radioPanel.Controls.Add(r1);

            Panel categorySettingPanel = createPanel.CreateSubPanel(bodyPanel, 30, bodyPanel.Height / 2, bodyPanel.Width - 30, bodyPanel.Height / 3, BorderStyle.None, Color.Transparent);

            Label categorySettingLabel = createLabel.CreateLabelsInPanel(categorySettingPanel, "categorySettingLabel", "トップ画面変更設定 :", 50, 0, categorySettingPanel.Width / 3 - 30, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            categorySettingLabel.Padding = new Padding(0, 0, 20, 0);

            RadioButton r5 = new RadioButton();
            r5.Text = "トップ画面あり";
            r5.Location = new Point(categorySettingLabel.Right + 10, 5);
            r5.Font = new Font("Serif", 26);
            r5.AutoSize = true;
            //r5.Checked = true;
            r5.Name = "r5";
            r5.CheckedChanged += new EventHandler(this.CategoryMainSet);
            r5Global = r5;
            categorySettingPanel.Controls.Add(r5);

            RadioButton r6 = new RadioButton();
            r6.Text = "トップ画面無し";
            r6.Location = new Point(categorySettingLabel.Right + 10, r5.Bottom + 10);
            r6.Font = new Font("Serif", 26);
            r6.AutoSize = true;
            r6.Name = "r6";
            r6.Checked = true;
            r6.CheckedChanged += new EventHandler(this.CategoryMainSet);
            r6Global = r6;
            categorySettingPanel.Controls.Add(r6);

            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("timeoutValue") != null)
                {
                    timeoutValue = Convert.ToInt32(key.GetValue("timeoutValue"));
                    switch (timeoutValue)
                    {
                        case 4:
                            r1.Checked = true;
                            break;
                        default:
                            r1.Checked = true;
                            break;
                    }
                }
                if (key.GetValue("categoryMain") != null)
                {
                    categoryMain = Convert.ToBoolean(key.GetValue("categoryMain"));
                    switch (categoryMain)
                    {
                        case true:
                            r5.Checked = true;
                            break;
                        default:
                            r6.Checked = true;
                            break;
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

        private void TimeoutSet(object sender, EventArgs e)
        {
            RadioButton rTemp = (RadioButton)sender;
            switch (rTemp.Name)
            {
                case "r1":
                    timeoutValue = 4;
                    break;
                default:
                    timeoutValue = 4;
                    break;
            }
        }

        private void CategoryMainSet(object sender, EventArgs e)
        {
            RadioButton rTemp = (RadioButton)sender;
            switch (rTemp.Name)
            {
                case "r5":
                    categoryMain = true;
                    break;
                case "r6":
                    categoryMain = false;
                    break;
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("timeoutValue", timeoutValue);
                key.SetValue("categoryMain", categoryMain);
            }

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
