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
    public partial class BankNoteSetting : Form
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

        bool HighBankNote = true;

        RadioButton r1Global = null;
        RadioButton r2Global = null;

        public BankNoteSetting(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.BankNoteSetTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            bodyPanelGlobal = bodyPanel;

            Label notifyLabel1 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel1", "千円紙幣専用・高額紙幣対応はFirmwareによる Applicationは 自重対応とする", 50, 50, bodyPanel.Width - 50, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel1.Padding = new Padding(0, 0, 20, 0);

            Label notifyLabel2 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel1", "高額紙幣設定  エスクロあり・無し設定 (Harness対応必須)", 50, 200, bodyPanel.Width - 50, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel2.Padding = new Padding(0, 0, 20, 0);

            Label notifyLabel3 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel1", "デフォルト: ", 50, 270, 200, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel2.Padding = new Padding(0, 0, 20, 0);

            RadioButton r1 = new RadioButton();
            r1.Text = "エスクロあり";
            r1.Location = new Point(260, 290);
            r1.Font = new Font("Serif", 20);
            r1.AutoSize = true;
            r1.Checked = true;
            r1.Name = "r1";
            r1.CheckedChanged += new EventHandler(this.HighBankNoteSetting);
            r1Global = r1;
            bodyPanel.Controls.Add(r1);

            RadioButton r2 = new RadioButton();
            r2.Text = "エスクロ無し (返金・取消不可対応必須)";
            r2.Location = new Point(260, 350);
            r2.Font = new Font("Serif", 20);
            r2.AutoSize = true;
            r2.Name = "r2";
            r2.CheckedChanged += new EventHandler(this.HighBankNoteSetting);
            r2Global = r2;
            bodyPanel.Controls.Add(r2);

            Label notifyLabel4 = createLabel.CreateLabelsInPanel(bodyPanel, "notifyLabel1", "金銭システム要確認", 260, 400, 400, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            notifyLabel4.Padding = new Padding(0, 0, 20, 0);

            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("HighBankNote") != null)
                {
                    HighBankNote = Convert.ToBoolean(key.GetValue("HighBankNote"));
                    if (HighBankNote)
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

        private void HighBankNoteSetting(object sender, EventArgs e)
        {
            RadioButton rTemp = (RadioButton)sender;
            switch (rTemp.Name)
            {
                case "r1":
                    HighBankNote = true;
                    break;
                case "r2":
                    HighBankNote = false;
                    break;
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("HighBankNote", HighBankNote);
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
