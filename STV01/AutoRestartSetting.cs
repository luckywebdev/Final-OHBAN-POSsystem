using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class AutoRestartSetting : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        //private Button buttonGlobal = null;
        private FlowLayoutPanel[] menuFlowLayoutPanelGlobal = new FlowLayoutPanel[4];
        Constant constants = new Constant();
        DBClass dbClass = new DBClass();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton customButton = new CustomButton();
        Label tableRestartGlobal = null;
        Label tableShutdownGlobal = null;
        DetailView detailView = new DetailView();
        RadioButton r2Global = null;
        RegistryKey key = null;
        string restartFlag = "restart";
        
        int hourGlobal = 23;
        int minuteGlobal = 59;
        SQLiteConnection sqlite_conn;

        public AutoRestartSetting(Form1 mainForm, Panel mainPanel)
        {
            InitializeComponent();
            mainForm.Width = width;
            mainForm.Height = height;
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            detailView.InitAutoRestartSetting(this);

            sqlite_conn = CreateConnection(constants.dbName);

            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerLabel", "電源OFF設定", 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            DateTime now = DateTime.Now;

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);

            FlowLayoutPanel tableHeaderInUpPanel2 = createPanel.CreateFlowLayoutPanel(bodyPanel, bodyPanel.Width / 7, bodyPanel.Height * 2 / 5, bodyPanel.Width * 5 / 7, bodyPanel.Height / 5, Color.White, new Padding(30, bodyPanel.Height / 20, 30, bodyPanel.Height / 20));
            r2Global = new RadioButton();
            r2Global.Text = "電源OFF時刻";
            r2Global.Location = new Point(0, 10);
            r2Global.Font = new Font("Serif", 26);
            r2Global.ForeColor = Color.Gray;
            r2Global.Checked = true;
            r2Global.AutoSize = true;
            r2Global.Name = "r2";
            r2Global.Margin = new Padding(0, 10, 5, 0);
            r2Global.Click += new EventHandler(this.TimeoutSet);
            tableHeaderInUpPanel2.Controls.Add(r2Global);

            tableShutdownGlobal = createLabel.CreateLabels(tableHeaderInUpPanel2, "tableTimeValue", " 時 " + "    " + " 分 ", tableHeaderInUpPanel2.Width / 2 - 20, 0, tableHeaderInUpPanel2.Width * 3 / 5 - 30, tableHeaderInUpPanel2.Height / 2, Color.White, Color.Black, 22, true, ContentAlignment.MiddleCenter, new Padding(0, 0, 0, 0), 1, Color.Gray);
            tableShutdownGlobal.Enabled = false;
            tableShutdownGlobal.Click += new EventHandler(detailView.AutoRestartSet);


            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if(key.GetValue("restartTime") != null)
                {
                    DateTime restartTime = Convert.ToDateTime(key.GetValue("restartTime"));
                    hourGlobal = restartTime.Hour;
                    minuteGlobal = restartTime.Minute;
                }
                else
                {
                    key.SetValue("restartTime", hourGlobal.ToString("00") + ":" + minuteGlobal.ToString("10"));
                }
                if (key.GetValue("restartFlag") != null)
                {
                    restartFlag = Convert.ToString(key.GetValue("restartFlag"));
                    switch (restartFlag)
                    {
                        case "shutdown":
                            r2Global.Checked = true;
                            r2Global.ForeColor = Color.Black;
                            tableShutdownGlobal.Text = hourGlobal.ToString("00") + " 時 " + minuteGlobal.ToString("00") + " 分 ";
                            tableShutdownGlobal.Enabled = true;
                            break;
                        default:
                            r2Global.Checked = true;
                            r2Global.ForeColor = Color.Black;
                            tableShutdownGlobal.Text = hourGlobal.ToString("00") + " 時 " + minuteGlobal.ToString("00") + " 分 ";
                            tableShutdownGlobal.Enabled = true;
                            break;
                    }
                }
                else
                {
                    key.SetValue("restartFlag", "shutdown");
                }

            }


            Button backButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "backButton", constants.backText, bodyPanel.Width - 150, bodyPanel.Height - 100, 100, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShow);
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

        public void SetVal(string setItem, string setValue)
        {
            string[] getData = setValue.Split('_');
            hourGlobal = int.Parse(getData[0]);
            minuteGlobal = int.Parse(getData[1]);
            tableShutdownGlobal.Text = hourGlobal.ToString("00") + " 時 " + minuteGlobal.ToString("00") + " 分 ";
            key.SetValue("restartTime", hourGlobal.ToString("00") + ":" + minuteGlobal.ToString("00"));
        }

        private void TimeoutSet(object sender, EventArgs e)
        {
            RadioButton rTemp = (RadioButton)sender;
            switch (rTemp.Name)
            {
                case "r2":
                    r2Global.Checked = true;
                    r2Global.ForeColor = Color.Black;
                    tableShutdownGlobal.Text = hourGlobal.ToString("00") + " 時 " + minuteGlobal.ToString("00") + " 分 ";
                    key.SetValue("restartFlag", "shutdown");
                    tableShutdownGlobal.Enabled = true;
                    restartFlag = "shutdown";
                    break;
                default:
                    r2Global.Checked = true;
                    r2Global.ForeColor = Color.Black;
                    tableShutdownGlobal.Text = hourGlobal.ToString("00") + " 時 " + minuteGlobal.ToString("00") + " 分 ";
                    key.SetValue("restartFlag", "shutdown");
                    tableShutdownGlobal.Enabled = true;
                    restartFlag = "shutdown";
                    break;
            }
        }


        static SQLiteConnection CreateConnection(string dbName)
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            string dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dbFolder += "\\STV01\\";
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            string dbPath = Path.Combine(dbFolder, dbName + ".db");

            sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; Version = 3; New = True; Compress = True; ");

            return sqlite_conn;
        }


    }
}
