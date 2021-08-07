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
    public partial class NetConfig : Form
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
        CreateTextBox createTextBox = new CreateTextBox();
        RadioButton r1Global = null;
        RadioButton r2Global = null;
        RadioButton r3Global = null;
        RadioButton r4Global = null;
        TextBox ipAddressGlobal1 = null;
        TextBox portGlobal1 = null;

        string ipAddress1 = "192.168.1.250";
        string port1 = "9100";

        TextBox ipAddressGlobal2 = null;
        TextBox portGlobal2 = null;

        string ipAddress2 = "";
        string port2 = "";

        TextBox ipAddressGlobal3 = null;
        TextBox portGlobal3 = null;

        string ipAddress3 = "";
        string port3 = "";

        bool netSelect = false;
        bool callNumbering = true;

        RegistryKey key = null;

        DateTime now = DateTime.Now;

        public NetConfig(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.printerSetTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            bodyPanelGlobal = bodyPanel;

            Label printerLabel1 = createLabel.CreateLabelsInPanel(bodyPanel, "printer_1", "マスタープリンタ", 20, 50, bodyPanel.Width / 3, 110, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            printerLabel1.Padding = new Padding(0, 0, 20, 0);


            Label ipAddressLabel1 = createLabel.CreateLabelsInPanel(bodyPanel, "ipAddressLabel_1", constants.ipaddressLabel, bodyPanel.Width / 3 + 20, 50, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            ipAddressLabel1.Padding = new Padding(0, 0, 50, 0);

            TextBox ipAddressTextBox1 = createTextBox.CreateTextBoxs_panel(bodyPanel, "ipAddress_1", bodyPanel.Width * 2 / 3, 53, 300, 60, 24, BorderStyle.FixedSingle);
            ipAddressTextBox1.Margin = new Padding(0, 15, 0, 0);
            ipAddressTextBox1.Enabled = false;
            ipAddressGlobal1 = ipAddressTextBox1;

            Label portLabel1 = createLabel.CreateLabelsInPanel(bodyPanel, "portLabel_1", constants.portLabel, bodyPanel.Width / 3 + 20, ipAddressLabel1.Bottom + 10, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            portLabel1.Padding = new Padding(0, 0, 20, 0);

            TextBox portTextBox1 = createTextBox.CreateTextBoxs_panel(bodyPanel, "port_1", bodyPanel.Width * 2 / 3, ipAddressTextBox1.Bottom + 18, 300, 60, 24, BorderStyle.FixedSingle);
            portTextBox1.Margin = new Padding(0, 15, 0, 0);
            portTextBox1.Enabled = false;
            portGlobal1 = portTextBox1;

            Label printerLabel2 = createLabel.CreateLabelsInPanel(bodyPanel, "printer_2", "セカントプリンタ 1", 20, 190, bodyPanel.Width / 3, 110, Color.Transparent, Color.Gray, 20, false, ContentAlignment.MiddleLeft);
            printerLabel2.Padding = new Padding(0, 0, 20, 0);


            Label ipAddressLabel2 = createLabel.CreateLabelsInPanel(bodyPanel, "ipAddressLabe_2", constants.ipaddressLabel, bodyPanel.Width / 3 + 20, 190, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Gray, 18, false, ContentAlignment.MiddleCenter);
            ipAddressLabel2.Padding = new Padding(0, 0, 20, 0);

            TextBox ipAddressTextBox2 = createTextBox.CreateTextBoxs_panel(bodyPanel, "ipAddress_2", bodyPanel.Width * 2 / 3, 193, 300, 60, 24, BorderStyle.FixedSingle);
            ipAddressTextBox2.Margin = new Padding(0, 15, 0, 0);
            ipAddressTextBox2.Enabled = false;
            ipAddressGlobal2 = ipAddressTextBox2;

            Label portLabel2 = createLabel.CreateLabelsInPanel(bodyPanel, "portLabel_2", constants.portLabel, bodyPanel.Width / 3 + 20, ipAddressLabel2.Bottom + 10, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Gray, 18, false, ContentAlignment.MiddleCenter);
            portLabel2.Padding = new Padding(0, 0, 20, 0);

            TextBox portTextBox2 = createTextBox.CreateTextBoxs_panel(bodyPanel, "port_2", bodyPanel.Width * 2 / 3, ipAddressTextBox2.Bottom + 18, 300, 60, 24, BorderStyle.FixedSingle);
            portTextBox2.Margin = new Padding(0, 15, 0, 0);
            portTextBox2.Enabled = false;
            portGlobal2 = portTextBox2;

            Label printerLabel3 = createLabel.CreateLabelsInPanel(bodyPanel, "printer_3", "セカントプリンタ 2", 20, 330, bodyPanel.Width / 3, 110, Color.Transparent, Color.Gray, 20, false, ContentAlignment.MiddleLeft);
            printerLabel3.Padding = new Padding(0, 0, 20, 0);


            Label ipAddressLabel3 = createLabel.CreateLabelsInPanel(bodyPanel, "ipAddressLabel_3", constants.ipaddressLabel, bodyPanel.Width / 3 + 20, 330, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Gray, 18, false, ContentAlignment.MiddleCenter);
            ipAddressLabel3.Padding = new Padding(0, 0, 20, 0);

            TextBox ipAddressTextBox3 = createTextBox.CreateTextBoxs_panel(bodyPanel, "ipAddress_3", bodyPanel.Width * 2 / 3, 333, 300, 60, 24, BorderStyle.FixedSingle);
            ipAddressTextBox3.Margin = new Padding(0, 15, 0, 0);
            ipAddressTextBox3.Enabled = false;
            ipAddressGlobal3 = ipAddressTextBox3;

            Label portLabel3 = createLabel.CreateLabelsInPanel(bodyPanel, "portLabel_3", constants.portLabel, bodyPanel.Width / 3 + 20, ipAddressLabel3.Bottom + 10, bodyPanel.Width / 3 - 40, 50, Color.Transparent, Color.Gray, 18, false, ContentAlignment.MiddleCenter);
            portLabel3.Padding = new Padding(0, 0, 20, 0);

            TextBox portTextBox3 = createTextBox.CreateTextBoxs_panel(bodyPanel, "port_3", bodyPanel.Width * 2 / 3, ipAddressTextBox3.Bottom + 18, 300, 60, 24, BorderStyle.FixedSingle);
            portTextBox3.Margin = new Padding(0, 15, 0, 0);
            portTextBox3.Enabled = false;
            portGlobal3 = portTextBox3;

            Panel topPanel = new Panel();
            topPanel.Location = new Point(0, portLabel3.Bottom + 10);
            topPanel.Size = new Size(bodyPanel.Width, 60);
            bodyPanel.Controls.Add(topPanel);

            Label netsetlabel = createLabel.CreateLabelsInPanel(topPanel, "netsetlabel", "ネシトワークプリンタ", 20, 0, 330, topPanel.Height, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            netsetlabel.Padding = new Padding(0, 0, 20, 0);

            RadioButton r1 = new RadioButton();
            r1.Text = "使用する";
            r1.Location = new Point(350, 10);
            r1.Font = new Font("Serif", 28);
            r1.AutoSize = true;
            r1.Name = "r1";
            r1.CheckedChanged += new EventHandler(this.NetSelectChanged);
            r1Global = r1;
            topPanel.Controls.Add(r1);

            RadioButton r2 = new RadioButton();
            r2.Text = "使用しない";
            r2.Location = new Point(r1.Right + 20, 10);
            r2.Font = new Font("Serif", 26);
            r2.AutoSize = true;
            r2.Name = "r2";
            r2.Checked = true;
            r2.CheckedChanged += new EventHandler(this.NetSelectChanged);
            r2Global = r2;
            topPanel.Controls.Add(r2);

            Label callnumberlabel = createLabel.CreateLabelsInPanel(bodyPanel, "callnumberlabel", "呼出番号", 20, topPanel.Bottom + 10, 330, 60, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleLeft);
            callnumberlabel.Padding = new Padding(0, 0, 20, 0);


            RadioButton r3 = new RadioButton();
            r3.Text = "印刷する";
            r3.Location = new Point(350, topPanel.Bottom + 20);
            r3.Font = new Font("Serif", 26);
            r3.AutoSize = true;
            r3.Checked = true;
            r3.Name = "r3";
            r3.CheckedChanged += new EventHandler(this.CallNumberingSelectChanged);
            r3Global = r3;
            bodyPanel.Controls.Add(r3);

            RadioButton r4 = new RadioButton();
            r4.Text = "印刷しない";
            r4.Location = new Point(r3.Right + 20, topPanel.Bottom + 20);
            r4.Font = new Font("Serif", 28);
            r4.AutoSize = true;
            r4.Name = "r4";
            r4.CheckedChanged += new EventHandler(this.CallNumberingSelectChanged);
            r4Global = r4;
            bodyPanel.Controls.Add(r4);

            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("ipAddress1") != null)
                {
                    ipAddress1 = Convert.ToString(key.GetValue("ipAddress1"));
                    ipAddressTextBox1.Text = ipAddress1;

                }
                if (key.GetValue("port1") != null)
                {
                    port1 = Convert.ToString(key.GetValue("port1"));
                    portTextBox1.Text = port1;
                }
                if (key.GetValue("ipAddress2") != null)
                {
                    ipAddress2 = Convert.ToString(key.GetValue("ipAddress2"));
                    ipAddressTextBox2.Text = ipAddress2;

                }
                if (key.GetValue("port2") != null)
                {
                    port2 = Convert.ToString(key.GetValue("port2"));
                    portTextBox2.Text = port2;
                }
                if (key.GetValue("ipAddress3") != null)
                {
                    ipAddress3 = Convert.ToString(key.GetValue("ipAddress3"));
                    ipAddressTextBox3.Text = ipAddress3;

                }
                if (key.GetValue("port3") != null)
                {
                    port3 = Convert.ToString(key.GetValue("port3"));
                    portTextBox3.Text = port3;
                }
                if (key.GetValue("netSelect") != null)
                {
                    netSelect = Convert.ToBoolean(key.GetValue("netSelect"));
                    if (netSelect)
                    {
                        ipAddressTextBox1.Enabled = true;
                        portTextBox1.Enabled = true;
                        ipAddressTextBox2.Enabled = true;
                        portTextBox2.Enabled = true;
                        ipAddressTextBox3.Enabled = true;
                        portTextBox3.Enabled = true;
                        r1.Checked = true;

                    }
                    else
                    {
                        ipAddressTextBox1.Enabled = false;
                        portTextBox1.Enabled = false;
                        ipAddressTextBox2.Enabled = false;
                        portTextBox2.Enabled = false;
                        ipAddressTextBox3.Enabled = false;
                        portTextBox3.Enabled = false;
                        r2.Checked = true;
                    }
                }
                if (key.GetValue("callNumbering") != null)
                {
                    callNumbering = Convert.ToBoolean(key.GetValue("callNumbering"));
                    if (callNumbering)
                    {
                        r3.Checked = true;
                    }
                    else
                    {
                        r4.Checked = true;
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

        private void NetSelectChanged(object sender, EventArgs e)
        {
            if (r1Global.Checked == true)
            {
                ipAddressGlobal1.Enabled = true;
                portGlobal1.Enabled = true;
                ipAddressGlobal2.Enabled = true;
                portGlobal2.Enabled = true;
                ipAddressGlobal3.Enabled = true;
                portGlobal3.Enabled = true;
                netSelect = true;
            }
            else
            {
                ipAddressGlobal1.Enabled = false;
                portGlobal1.Enabled = false;
                ipAddressGlobal2.Enabled = false;
                portGlobal2.Enabled = false;
                ipAddressGlobal3.Enabled = false;
                portGlobal3.Enabled = false;
                netSelect = false;
            }
        }

        private void CallNumberingSelectChanged(object sender, EventArgs e)
        {
            if(r3Global.Checked == true)
            {
                callNumbering = true;
            }
            else
            {
                callNumbering = false;
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            ipAddress1 = ipAddressGlobal1.Text;
            port1 = portGlobal1.Text;
            ipAddress2 = ipAddressGlobal2.Text;
            port2 = portGlobal2.Text;
            ipAddress3 = ipAddressGlobal3.Text;
            port3 = portGlobal3.Text;

            key.SetValue("ipAddress1", ipAddress1);
            key.SetValue("port1", port1);
            key.SetValue("ipAddress2", ipAddress2);
            key.SetValue("port2", port2);
            key.SetValue("ipAddress3", ipAddress3);
            key.SetValue("port3", port3);
            key.SetValue("netSelect", netSelect);
            key.SetValue("callNumbering", callNumbering);

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
