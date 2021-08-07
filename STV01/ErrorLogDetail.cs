using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class ErrorLogDetail : Form
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
        CreateCombobox createCombobox = new CreateCombobox();
        CustomButton customButton = new CustomButton();
        DropDownMenu dropDownMenu = new DropDownMenu();
        MonthDropDown dropDownMonth = new MonthDropDown();
        DateDropDown dropDownDate = new DateDropDown();

        Panel bodyPanel = null;

        LogDetailView logDetailView = new LogDetailView();
        MessageDialog messageDialog = new MessageDialog();
        public ComboBox yearComboboxGlobal = null;
        public ComboBox monthComboboxGlobal = null;
        public ComboBox dateComboboxGlobal = null;
        public ComboBox timeComboboxGlobal = null;
        public ComboBox typeComboboxGlobal = null;

        DateTime now = DateTime.Now;

        string sumDate = DateTime.Now.ToString("yyyy-MM-dd");

        public ErrorLogDetail(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;

            Panel headerPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, 20, mainPanelGlobal_2.Width, 70, BorderStyle.None, Color.Transparent);

            Label headerTitle = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.errorLogDetail, 0, 0, headerPanel.Width / 3 - 30, headerPanel.Height, Color.FromArgb(255, 0, 95, 163), Color.FromArgb(255, 255, 242, 75), 36);

            ComboBox yearCombobox1 = createCombobox.CreateComboboxs(headerPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, headerTitle.Right + 20, 20, headerPanel.Width / 12, headerPanel.Height - 30, headerPanel.Height - 30, new Font("Comic Sans", 28), now.ToString("yyyy"));
            Label yearLabel1 = createLabel.CreateLabelsInPanel(headerPanel, "startLabelYear", constants.yearLabel, yearCombobox1.Right, 15, headerPanel.Width / 36, headerPanel.Height - 20, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);

            yearComboboxGlobal = yearCombobox1;
            yearCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            yearCombobox1.SelectedIndexChanged += new EventHandler(FilterOptionChanged);

            ComboBox monthCombobox1 = createCombobox.CreateComboboxs(headerPanel, "monthCombobox1", constants.months1, yearLabel1.Right, 20, headerPanel.Width / 15, headerPanel.Height - 30, headerPanel.Height - 30, new Font("Comic Sans", 28), now.ToString("MM"));
            Label monthLabel1 = createLabel.CreateLabelsInPanel(headerPanel, "startLabelMonth", constants.monthLabel, monthCombobox1.Right, 15, headerPanel.Width / 36, headerPanel.Height - 20, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxGlobal = monthCombobox1;
            monthCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            monthCombobox1.SelectedIndexChanged += new EventHandler(FilterOptionChanged);

            ComboBox dateCombobox1 = createCombobox.CreateComboboxs(headerPanel, "dateCombobox1", constants.dates1, monthLabel1.Right, 20, headerPanel.Width / 15, headerPanel.Height - 30, headerPanel.Height - 30, new Font("Comic Sans", 28), now.ToString("dd"));

            Label dateLabel1 = createLabel.CreateLabelsInPanel(headerPanel, "startLabelDate", constants.dayLabel, dateCombobox1.Right, 15, headerPanel.Width / 36, headerPanel.Height - 20, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxGlobal = dateCombobox1;
            dateCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            dateCombobox1.SelectedIndexChanged += new EventHandler(FilterOptionChanged);

            ComboBox timeCombobox1 = createCombobox.CreateComboboxs(headerPanel, "dateCombobox1", constants.times, dateLabel1.Right + 50, 20, headerPanel.Width / 15, headerPanel.Height - 30, headerPanel.Height - 30, new Font("Comic Sans", 28), now.ToString("HH"));

            Label timeLabel1 = createLabel.CreateLabelsInPanel(headerPanel, "startLabelDate", constants.timeRangeLabel, timeCombobox1.Right, 15, headerPanel.Width / 16, headerPanel.Height - 20, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            timeComboboxGlobal = timeCombobox1;
            timeCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            timeCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            timeCombobox1.SelectedIndexChanged += new EventHandler(FilterOptionChanged);

            string backImage = constants.soldoutButtonImage1;
            Button backButton = customButton.CreateButtonWithImage(backImage, "closeButton", constants.backText, headerPanel.Width - 200, 10, 150, 50, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            headerPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShow);

            bodyPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, headerPanel.Bottom, mainPanelGlobal_2.Width, mainPanelGlobal_2.Height - 180, BorderStyle.None, Color.Transparent);
            bodyPanel.Padding = new Padding(0, 5, 0, 10);
            bodyPanel.HorizontalScroll.Maximum = 0;
            bodyPanel.AutoScroll = false;
            bodyPanel.VerticalScroll.Visible = false;
            bodyPanel.AutoScroll = true;

            Panel bottomPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, mainPanelGlobal_2.Bottom - 90, mainPanelGlobal_2.Width, 90, BorderStyle.None, Color.Transparent);

            ComboBox typeCombobox1 = createCombobox.CreateComboboxs(bottomPanel, "dateCombobox1", constants.errorTypes, 20, 20, bottomPanel.Width / 6, bottomPanel.Height - 50, bottomPanel.Height - 50, new Font("Comic Sans", 26), "全て");
            typeCombobox1.SelectedIndexChanged += new EventHandler(FilterOptionChanged);

            Label typeLabel1 = createLabel.CreateLabelsInPanel(bottomPanel, "startLabelDate", constants.timeRangeLabel, typeCombobox1.Right, 15, bottomPanel.Width / 15, bottomPanel.Height - 40, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            typeComboboxGlobal = typeCombobox1;
            typeCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            typeCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            string printBtnImage = constants.maitanenceButtonImage[0];
            Button printBtn = customButton.CreateButtonWithImage(printBtnImage, "printBtn", constants.printButtonLabel, bottomPanel.Width - 540, 10, 150, 50, 0, 10, 12, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bottomPanel.Controls.Add(printBtn);
            printBtn.Click += new EventHandler(ErrorLogPrint);

            string allSaveImage = constants.maitanenceButtonImage[1];
            Button allSaveBtn = customButton.CreateButtonWithImage(allSaveImage, "allSaveBtn", "全てのログを保存", bottomPanel.Width - 370, 10, 150, 50, 0, 10, 12, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bottomPanel.Controls.Add(allSaveBtn);
            allSaveBtn.Click += new EventHandler(LogSave);

            string viewSaveImage = constants.maitanenceButtonImage[2];
            Button viewSaveBtn = customButton.CreateButtonWithImage(viewSaveImage, "viewSaveBtn", "表示中のログを保存", bottomPanel.Width - 200, 10, 150, 50, 0, 10, 9, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bottomPanel.Controls.Add(viewSaveBtn);
            viewSaveBtn.Click += new EventHandler(LogSave);
            ErrorLogDetailView();
            InitializeComponent();
        }

        private void ErrorLogDetailView()
        {
            string logYears = yearComboboxGlobal.GetItemText(yearComboboxGlobal.SelectedItem);
            string logMonths = monthComboboxGlobal.GetItemText(monthComboboxGlobal.SelectedItem);
            string logDays = dateComboboxGlobal.GetItemText(dateComboboxGlobal.SelectedItem);
            string logTimes = timeComboboxGlobal.GetItemText(timeComboboxGlobal.SelectedItem);
            int logTypes = typeComboboxGlobal.SelectedIndex;

            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddDays(1);
            if (logTimes == "全て")
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), 00, 00, 00);
                endTime = startTime.AddDays(1).AddSeconds(-1);
            }
            else
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), int.Parse(logTimes), 00, 00);
                endTime = startTime.AddHours(1);
            }

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string query = null;
            if(logTypes == 0)
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime ORDER BY logDate DESC";
            }
            else
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime AND logType=@logType ORDER BY logDate DESC";
            }
            sqlite_cmd.CommandText = query;
            sqlite_cmd.Parameters.AddWithValue("@startTime", startTime);
            sqlite_cmd.Parameters.AddWithValue("@endTime", endTime);
            if(logTypes != 0)
            {
                sqlite_cmd.Parameters.AddWithValue("@logType", logTypes);
            }
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            int currentY = 10;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string logDate = sqlite_datareader.GetDateTime(5).ToString("yyyy/MM/dd");
                    string logTime = sqlite_datareader.GetDateTime(5).ToString("HH:mm:ss");
                    string logTitle = sqlite_datareader.GetString(2);
                    string logContent_1 = sqlite_datareader.GetString(3);
                    string logContent_2 = sqlite_datareader.GetString(4);

                    if (k != 0)
                    {
                        PictureBox pB = new PictureBox();
                        pB.Location = new Point(60, currentY);
                        pB.Size = new Size(bodyPanel.Width - 150, 5);
                        bodyPanel.Controls.Add(pB);
                        Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
                        Graphics g = Graphics.FromImage(image);
                        g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, bodyPanel.Width - 160, 5);
                        pB.Image = image;
                        currentY += 10;

                    }

                    Label dateLabel = createLabel.CreateLabelsInPanel(bodyPanel, "logDate", logDate, 30, currentY, (bodyPanel.Width - 60) / 4, 30, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    Label titleLabel = createLabel.CreateLabelsInPanel(bodyPanel, "logTitle", logTitle, dateLabel.Right, currentY, (bodyPanel.Width - 60) * 2 / 3, 30, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    currentY += 30;
                    Label timeLabel = createLabel.CreateLabelsInPanel(bodyPanel, "logTime", logTime, 30, currentY, (bodyPanel.Width - 60) / 4, 30, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    Label contentLabel = createLabel.CreateLabelsInPanel(bodyPanel, "logContent", logContent_1, timeLabel.Right, currentY, (bodyPanel.Width - 60) * 2 / 3, 30, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    currentY += 30;
                    if (logContent_2 != "" && logContent_2 != null)
                    {
                        Label contentLabel_2 = createLabel.CreateLabelsInPanel(bodyPanel, "logContent_2", logContent_2, timeLabel.Right, currentY, (bodyPanel.Width - 60) / 3, 30, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                        currentY += 30;
                    }
                    k++;
                }
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void FilterOptionChanged(object sender, EventArgs e)
        {
            bodyPanel.Controls.Clear();
            ErrorLogDetailView();
        }

        PaperSize paperSize = new PaperSize("papersize", 500, 800);//set the paper size

        private void ErrorLogPrint(object sender, EventArgs e)
        {
            PrintDocument printDocument1 = new PrintDocument();
            printDocument1.PrintPage += new PrintPageEventHandler(PrintRun);
            paperSize = new PaperSize("papersize", constants.logPrintPaperWidth, constants.logPrintPaperHeight);
            printDocument1.DefaultPageSettings.PaperSize = paperSize;
            printDocument1.Print();
        }

        private void PrintRun (object sender, PrintPageEventArgs e)
        {
            string logYears = yearComboboxGlobal.GetItemText(yearComboboxGlobal.SelectedItem);
            string logMonths = monthComboboxGlobal.GetItemText(monthComboboxGlobal.SelectedItem);
            string logDays = dateComboboxGlobal.GetItemText(dateComboboxGlobal.SelectedItem);
            string logTimes = timeComboboxGlobal.GetItemText(timeComboboxGlobal.SelectedItem);
            string logDates = logYears + constants.yearLabel + logMonths + constants.monthLabel + logDays + constants.dayLabel + logTimes + constants.timeRangeLabel;
            int logTypes = typeComboboxGlobal.SelectedIndex;
            string logTypeStr = typeComboboxGlobal.GetItemText(typeComboboxGlobal.SelectedItem);
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddDays(1).AddSeconds(-1);
            if(logTimes == "全て")
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), 00, 00, 00);
                endTime = startTime.AddHours(1);
            }
            else
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), int.Parse(logTimes), 00, 00);
                endTime = startTime.AddHours(1);
            }

            float currentY = 0;// declare  one variable for height measurement
            RectangleF rect1 = new RectangleF(5, currentY, constants.logPrintPaperWidth - 5, 30);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(constants.errorLogDetail + "(" + logTypeStr + ") " + logDates, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            currentY += 40;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string query = null;
            if (logTypes == 0)
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime ORDER BY logDate DESC";
            }
            else
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime AND logType=@logType ORDER BY logDate DESC";
            }
            sqlite_cmd.CommandText = query;
            sqlite_cmd.Parameters.AddWithValue("@startTime", startTime);
            sqlite_cmd.Parameters.AddWithValue("@endTime", endTime);
            if (logTypes != 0)
            {
                sqlite_cmd.Parameters.AddWithValue("@logType", logTypes);
            }
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string logDate = sqlite_datareader.GetDateTime(5).ToString("yyyy/MM/dd");
                    string logTime = sqlite_datareader.GetDateTime(5).ToString("HH:mm:ss");
                    string logTitle = sqlite_datareader.GetString(2);
                    string logContent_1 = sqlite_datareader.GetString(3);
                    string logContent_2 = sqlite_datareader.GetString(4);

                    if (k != 0)
                    {
                        // Create pen.
                        Pen blackPen = new Pen(Color.Black, 1);

                        // Create points that define line.
                        PointF point1 = new PointF(10, currentY);
                        PointF point2 = new PointF(Convert.ToSingle(constants.logPrintPaperWidth) - 20, currentY + 1);
                        e.Graphics.DrawLine(blackPen, point1, point2);
                        currentY += 20;
                    }

                    RectangleF rect2 = new RectangleF(5, currentY, constants.logPrintPaperWidth * 2 / 5, 20);
                    StringFormat format2 = new StringFormat();
                    format2.Alignment = StringAlignment.Near;
                    e.Graphics.DrawString(logDate, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//print each item

                    RectangleF rect3 = new RectangleF(constants.logPrintPaperWidth * 2 / 5 + 5, currentY, constants.logPrintPaperWidth * 3 / 5 - 10, 20);
                    StringFormat format3 = new StringFormat();
                    format3.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(logTitle, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect3, format3);//print each item

                    currentY += 20;

                    RectangleF rect4 = new RectangleF(5, currentY, constants.logPrintPaperWidth * 2 / 5, 20);
                    StringFormat format4 = new StringFormat();
                    format4.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(logTime, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

                    RectangleF rect5 = new RectangleF(constants.logPrintPaperWidth * 2 / 5 + 5, currentY, constants.logPrintPaperWidth * 3 / 5 - 10, 20);
                    StringFormat format5 = new StringFormat();
                    format5.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString(logContent_1, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect5, format5);//print each item
                    currentY += 20;

                    if (logContent_2 != "" && logContent_2 != null)
                    {
                        RectangleF rect6 = new RectangleF(constants.logPrintPaperWidth * 2 / 5 + 5, currentY, constants.logPrintPaperWidth * 3 / 5 - 10, 20);
                        StringFormat format6 = new StringFormat();
                        format6.Alignment = StringAlignment.Near;
                        e.Graphics.DrawString(logContent_2, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect6, format6);//print each item
                        currentY += 20;
                    }
                    e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
                    k++;
                }
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void LogSave(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            string logYears = yearComboboxGlobal.GetItemText(yearComboboxGlobal.SelectedItem);
            string logMonths = monthComboboxGlobal.GetItemText(monthComboboxGlobal.SelectedItem);
            string logDays = dateComboboxGlobal.GetItemText(dateComboboxGlobal.SelectedItem);
            string logTimes = timeComboboxGlobal.GetItemText(timeComboboxGlobal.SelectedItem);
            int logTypes = typeComboboxGlobal.SelectedIndex;
            string logTypeStr = typeComboboxGlobal.GetItemText(typeComboboxGlobal.SelectedItem);

            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddDays(1);
            if(logTimes == "全て")
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), 00, 00, 00);
                endTime = startTime.AddDays(1).AddSeconds(-1);
            }
            else
            {
                startTime = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), int.Parse(logTimes), 00, 00);
                endTime = startTime.AddHours(1);
            }

            //string currentDir = Directory.GetCurrentDirectory();
            string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }

            if (!Directory.Exists(currentDir + "/logData"))
            {
                Directory.CreateDirectory(currentDir + "/logData");
            }

            string filePath = "";
            if (logTypes == 0 || btnTemp.Name == "allSaveBtn")
            {
                filePath = Path.Combine(currentDir, "logData", "all" + logYears + logMonths + logDays + logTimes + ".txt");
            }
            else
            {
                filePath = Path.Combine(currentDir, "logData", logTypeStr + logYears + logMonths + logDays + logTimes + ".txt");
            }

            string logStr = "";

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string query = null;
            if (logTypes == 0 || btnTemp.Name == "allSaveBtn")
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime ORDER BY logDate DESC";
            }
            else
            {
                query = "SELECT * FROM " + constants.tbNames[14] + " WHERE logDate>=@startTime AND logDate<=@endTime AND logType=@logType ORDER BY logDate DESC";
            }
            sqlite_cmd.CommandText = query;
            sqlite_cmd.Parameters.AddWithValue("@startTime", startTime);
            sqlite_cmd.Parameters.AddWithValue("@endTime", endTime);
            if (logTypes != 0 || btnTemp.Name == "allSaveBtn")
            {
                sqlite_cmd.Parameters.AddWithValue("@logType", logTypes);
            }
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string logDate = sqlite_datareader.GetDateTime(5).ToString("yyyy/MM/dd");
                    string logTime = sqlite_datareader.GetDateTime(5).ToString("HH:mm:ss");
                    string logTitle = sqlite_datareader.GetString(2);
                    string logContent_1 = sqlite_datareader.GetString(3);
                    string logContent_2 = sqlite_datareader.GetString(4);

                    logStr += Environment.NewLine;
                    logStr += logDate + " : " + logTitle;
                    logStr += Environment.NewLine;
                    logStr += logTime + "    " + logContent_1;
                    if (logContent_2 != "" && logContent_2 != null)
                    {
                        logStr += Environment.NewLine;
                        logStr += "                 " + logContent_2;
                    }
                    k++;
                }
            }
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(filePath))
                    File.Delete(filePath);
                // Create a new file     
                using (FileStream fs = File.Create(filePath))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(logStr);
                    fs.Write(title, 0, title.Length);
                }

                string msg = "";
                if(logTypes == 0 || btnTemp.Name == "allSaveBtn")
                {
                    msg = "データを all" + logYears + logMonths + logDays + logTimes + ".txtに保管しました。";
                }
                else
                {
                    msg = "データを " + logTypeStr + logYears + logMonths + logDays + logTimes + ".txtに保管しました。";
                }
                messageDialog.LogSaveMessage(msg);

                //var DeviceInfo = USBCheck();
                //if (DeviceInfo != null && DeviceInfo.UsbCheck == true)
                //{
                //    string DestinationPath = DeviceInfo.DeviceName + "OHBAN_DB/logData/";
                //    if (!Directory.Exists(DestinationPath))
                //    {
                //        Directory.CreateDirectory(DestinationPath);
                //    }
                //    string cpPath = "";
                //    if (logTypes == 0 || btnTemp.Name == "allSaveBtn")
                //    {
                //        cpPath = Path.Combine(DestinationPath, "all" + logYears + logMonths + logDays + logTimes + ".txt");
                //    }
                //    else
                //    {
                //        cpPath = Path.Combine(DestinationPath, logTypeStr + logYears + logMonths + logDays + logTimes + ".txt");
                //    }
                //    // Check if file already exists. If yes, delete it.     
                //    if (File.Exists(cpPath))
                //        File.Delete(cpPath);
                //    // Create a new file     
                //    using (FileStream fs = File.Create(cpPath))
                //    {
                //        Byte[] title = new UTF8Encoding(true).GetBytes(logStr);
                //        fs.Write(title, 0, title.Length);
                //    }
                //    string msg = "データを " + logTypeStr + logYears + logMonths + logDays + logTimes + ".txtに保管しました。";
                //    messageDialog.LogSaveMessage(msg);
                //}
                //else
                //{
                //    messageDialog.BackupRestoreError("", constants.noUSBError);
                //}
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());

            }
        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal_2.Controls.Clear();
            LogManage frm = new LogManage(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainPanelGlobal_2.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
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
