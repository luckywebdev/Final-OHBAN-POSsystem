using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSetting
{
    public partial class TimeSetting : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Panel mainPanelGlobal = null;
        //private Button buttonGlobal = null;
        private FlowLayoutPanel[] menuFlowLayoutPanelGlobal = new FlowLayoutPanel[4];
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CreateButton customButton = new CreateButton();
        Label tableDateValueGlobal = null;
        Label tableTimeValueGlobal = null;
        SettingDialog setDialog = null;
        int yearGlobal = DateTime.Now.Year;
        int monthGlobal = DateTime.Now.Month;
        int dayGlobal = DateTime.Now.Day;
        int hourGlobal = DateTime.Now.Hour;
        int minuteGlobal = DateTime.Now.Minute;


        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDay;
            public ushort wDayOfWeek;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetLocalTime(out SYSTEMTIME st);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime(ref SYSTEMTIME st);

        public TimeSetting()
        {

            InitializeComponent();
            this.Width = width;
            this.Height = height;

            Panel topPanel = createPanel.CreateMainPanel(this, 0, 30, width, 30, BorderStyle.None, Color.FromArgb(255, 234, 158, 65));
            Panel bottomPanel = createPanel.CreateMainPanel(this, 0, height - 60, width, 30, BorderStyle.None, Color.FromArgb(255, 234, 158, 65));

            mainPanelGlobal = createPanel.CreateMainPanel(this, width / 10, height / 9, width * 8 / 10, height * 7 / 9 - 30, BorderStyle.None, Color.FromArgb(255, 249, 246, 224));

            setDialog = new SettingDialog();
            setDialog.InitTimeSetting(this);

            Panel headerPanel = createPanel.CreateSubPanel(mainPanelGlobal, 0, 0, mainPanelGlobal.Width, mainPanelGlobal.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerLabel", "時刻設定", 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            DateTime now = DateTime.Now;

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanelGlobal, 0, headerPanel.Bottom, mainPanelGlobal.Width, mainPanelGlobal.Height * 9 / 10, BorderStyle.None, Color.Transparent);

            FlowLayoutPanel tableHeaderInUpPanel = createPanel.CreateFlowLayoutPanel(bodyPanel, bodyPanel.Width / 7, bodyPanel.Height / 5, bodyPanel.Width * 5 / 7, bodyPanel.Height / 5, Color.White, new Padding(30, bodyPanel.Height / 20, 30, bodyPanel.Height / 20));
            Label tableDateLabel = createLabel.CreateLabels(tableHeaderInUpPanel, "tableDateLabel", "現在の日付", 0, 0, tableHeaderInUpPanel.Width * 2 / 5 - 30, tableHeaderInUpPanel.Height / 2, Color.White, Color.Black, 22, false, ContentAlignment.MiddleLeft, new Padding(0, 0, 0, 0), 1, Color.Gray);
            Label tableDateValue = createLabel.CreateLabels(tableHeaderInUpPanel, "tableDateValue", now.ToString("yyyy") + "年 " + now.ToString("MM") + "月 " + now.ToString("dd") + "日 ", tableDateLabel.Right, 0, tableHeaderInUpPanel.Width * 3 / 5 - 30, tableHeaderInUpPanel.Height / 2, Color.White, Color.Black, 22, true, ContentAlignment.MiddleCenter, new Padding(0, 0, 0, 0), 1, Color.Gray);
            tableDateValueGlobal = tableDateValue;

            tableDateValue.Click += new EventHandler(SettingDate);

            FlowLayoutPanel tableHeaderInUpPanel2 = createPanel.CreateFlowLayoutPanel(bodyPanel, bodyPanel.Width / 7, tableHeaderInUpPanel.Bottom + bodyPanel.Height / 7, bodyPanel.Width * 5 / 7, bodyPanel.Height / 5, Color.White, new Padding(30, bodyPanel.Height / 20, 30, bodyPanel.Height / 20));
            Label tableTimeLabel = createLabel.CreateLabels(tableHeaderInUpPanel2, "tableTimeLabel", "現在の時刻", 0, 0, tableHeaderInUpPanel2.Width * 2 / 5 - 30, tableHeaderInUpPanel2.Height / 2, Color.White, Color.Black, 22, false, ContentAlignment.MiddleLeft, new Padding(0, 0, 0, 0), 1, Color.Gray);
            Label tableTimeValue = createLabel.CreateLabels(tableHeaderInUpPanel2, "tableTimeValue", now.Hour.ToString("00") + "時 " + now.Minute.ToString("00") + "分 ", tableTimeLabel.Right, 0, tableHeaderInUpPanel2.Width * 3 / 5 - 30, tableHeaderInUpPanel2.Height / 2, Color.White, Color.Black, 22, true, ContentAlignment.MiddleCenter, new Padding(0, 0, 0, 0), 1, Color.Gray);
            tableTimeValueGlobal = tableTimeValue;

            tableTimeValue.Click += new EventHandler(SettingTime);

            string shutdownButtonImage = @"resources\\shutdownbutton.png";

            Button shutdownButton = customButton.CreateButtonWithImage(Image.FromFile(shutdownButtonImage), "shutdownButton", "", bodyPanel.Width - 150, bodyPanel.Height - 100, 100, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(shutdownButton);
            shutdownButton.Click += new EventHandler(this.ShutdownAPP);

        }

        public void ShutdownAPP(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("isAdmin", false);
            }
            Thread.Sleep(1000);

            Process.Start("shutdown", "/s /t 0");
        }

        public void RestartAPP(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("isAdmin", false);
            }
            Thread.Sleep(1000);
            Process.Start("shutdown", "/r /t 0");
        }

        private void SettingTime(object sender, EventArgs e)
        {
            if(setDialog == null)
            {
                setDialog = new SettingDialog();
                setDialog.InitTimeSetting(this);
            }
            setDialog.TimeSetting();
            setDialog.ShowDialog();
        }

        private void SettingDate(object sender, EventArgs e)
        {
            if (setDialog == null)
            {
                setDialog = new SettingDialog();
                setDialog.InitTimeSetting(this);
            }
            setDialog.DateSetting();
            setDialog.ShowDialog();
        }

        public void SetVal(string setItem, string setValue)
        {
            if(setDialog != null)
            {
                setDialog.Close();
                setDialog.Dispose();
                setDialog = null;
            }

            DateTime now = DateTime.Now;
            if (setItem == "setDate")
            {
                string[] getData = setValue.Split('_');
                tableDateValueGlobal.Text = int.Parse(getData[0]).ToString("0000") + "年 " + int.Parse(getData[1]).ToString("00") + "月 " + int.Parse(getData[2]).ToString("00") + "日";
                tableTimeValueGlobal.Text = hourGlobal.ToString("00") + "時 " + minuteGlobal.ToString("00") + "分";
                yearGlobal = int.Parse(getData[0]);
                monthGlobal = int.Parse(getData[1]);
                dayGlobal = int.Parse(getData[2]);
                try
                {
                    SYSTEMTIME st = new SYSTEMTIME();

                    st.wYear = Convert.ToUInt16(yearGlobal); // must be ushort
                    st.wMonth = Convert.ToUInt16(monthGlobal);
                    st.wDayOfWeek = Convert.ToUInt16(dayGlobal);
                    st.wHour = Convert.ToUInt16(hourGlobal); // must be ushort
                    st.wMinute = Convert.ToUInt16(minuteGlobal);
                    st.wSecond = 0;
                    st.wMilliseconds = 0;

                    var ret = SetLocalTime(ref st);
                    Console.WriteLine("SetSystemTime return : " + ret);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            }
            else if (setItem == "setTime")
            {
                string[] getData = setValue.Split('_');
                tableDateValueGlobal.Text = yearGlobal.ToString("00") + "年 " + monthGlobal.ToString("00") + "月 " + dayGlobal.ToString("00") + "日";
                tableTimeValueGlobal.Text = int.Parse(getData[0]).ToString("00") + "時 " + int.Parse(getData[1]).ToString("00") + "分";
                hourGlobal = int.Parse(getData[0]);
                minuteGlobal = int.Parse(getData[1]);
                SYSTEMTIME st = new SYSTEMTIME();

                st.wYear = Convert.ToUInt16(yearGlobal); // must be ushort
                st.wMonth = Convert.ToUInt16(monthGlobal);
                st.wDayOfWeek = Convert.ToUInt16(dayGlobal);
                st.wHour = Convert.ToUInt16(hourGlobal); // must be ushort
                st.wMinute = Convert.ToUInt16(minuteGlobal);
                st.wSecond = 0;
                st.wMilliseconds = 0;

                var ret = SetLocalTime(ref st);

                Console.WriteLine("SetSystemTime return : " + ret);
            }
        }

        //static SQLiteConnection CreateConnection(string dbName)
        //{

        //    SQLiteConnection sqlite_conn;
        //    // Create a new database connection:
        //    string dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //    dbFolder += "\\STV01\\";
        //    if (!Directory.Exists(dbFolder))
        //    {
        //        Directory.CreateDirectory(dbFolder);
        //    }

        //    string dbPath = Path.Combine(dbFolder, dbName + ".db");

        //    sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; Version = 3; New = True; Compress = True; ");

        //    return sqlite_conn;
        //}

    }
}
