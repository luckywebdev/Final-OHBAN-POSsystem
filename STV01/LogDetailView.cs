using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class LogDetailView : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton createButton = new CustomButton();
        CreateCombobox createCombobox = new CreateCombobox();
        DropDownMenu dropDownMenu = new DropDownMenu();
        MessageDialog messageDialog = new MessageDialog();
        DBClass dbClass = new DBClass();
        Form DialogFormGlobal = null;
        Panel dialogPanelGlobal = null;
        FlowLayoutPanel dialogFlowLayout = null;
        PaperSize paperSize = new PaperSize("papersize", 500, 800);//set the paper size
        string logTimeGlobal = "23";
        DateTime now = DateTime.Now;
        string sumDate = DateTime.Now.ToString("yyyy-MM-dd");

        LogManage logHandlerGlobal = null;

        [DllImport("HwaUSB.dll")]
        public static extern int UsbOpen(string ModelName);
        [DllImport("HwaUSB.dll")]
        public static extern int PrintStr(string Str);
        [DllImport("HwaUSB.dll")]
        public static extern int PrintCmd(short data);
        [DllImport("HwaUSB.dll")]
        public static extern int NewRealRead();
        [DllImport("HwaUSB.dll")]
        public static extern void UsbClose();


        public LogDetailView()
        {
            InitializeComponent();

        }
        public void InitLogManage(LogManage logManageHandler)
        {
            logHandlerGlobal = logManageHandler;
        }

        public void DetailViewIndicator(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            if (btnTemp.Name == "dailyViewBtn")
            {
                DailyReport();
            }
            else if (btnTemp.Name == "dailyPrintBtn")
            {
                USBPrint("daily");
            }
            else if (btnTemp.Name == "receiptViewBtn")
            {
                ReceiptIssueReport();
            }
            else if (btnTemp.Name == "receiptPrintBtn")
            {
                USBPrint("receipt");
            }
            else if (btnTemp.Name == "logViewBtn")
            {
                LogReport();
            }
            else if (btnTemp.Name == "logPrintBtn")
            {
                USBPrint("log");
            }
            else if (btnTemp.Name == "allViewBtn")
            {
                AllLogView();
            }
            else if (btnTemp.Name == "allSaveBtn")
            {
                AllLogSave();
            }
        }

        public void DailyReport()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.Padding = new Padding(0, 0, 0, 20);
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }

            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;

            DialogFormGlobal = dialogForm;

            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            Panel dialogTitlePanel = createPanel.CreateMainPanel(dialogForm, 0, 30, dialogForm.Width, 50, BorderStyle.None, Color.Transparent);
            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle1", constants.dailyReportTitle, 0, 0, dialogTitlePanel.Width / 2 - 30, 60, Color.Transparent, Color.Black, 22);
            Label dialogTitle2 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle2", logDate, dialogTitlePanel.Width / 2, 0, dialogTitlePanel.Width / 2 - 30, 60, Color.Transparent, Color.Black, 22);

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 10, 90, dialogForm.Width - 30, dialogForm.Height - 190, BorderStyle.None, Color.Transparent);
            dialogPanel.Margin = new Padding(0, 20, 0, 20);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            int soldPriceSum = 0;
            int soldAmountSum = 0;
            int k = 0;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate ORDER BY sumDate DESC";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(2);
                    int prdPrice = sqlite_datareader.GetInt32(3);
                    int prdAmount = sqlite_datareader.GetInt32(4);
                    int prdTotalPrice = sqlite_datareader.GetInt32(5);
                    soldPriceSum += prdTotalPrice;
                    soldAmountSum += prdAmount;
                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, 41 * k, dialogPanel.Width, 40, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", prdName, 0, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", prdAmount.ToString() + constants.amountUnit, productTableBody.Width / 4, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3", prdPrice.ToString() + constants.unit, productTableBody.Width / 2, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody4 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_4", prdTotalPrice.ToString() + constants.unit, productTableBody.Width * 3 / 4, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    dialogFlowLayout = productTableBody;
                    k++;

                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            FlowLayoutPanel dialogSumPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, k * 41 + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(0));
            Label dialogSumLabel1 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel1", "合計", 0, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);
            Label dialogSumLabel2 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", soldAmountSum + constants.amountUnit, dialogSumPanel.Width / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);
            Label dialogSumLabel3 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", soldPriceSum + constants.unit, dialogSumPanel.Width * 2 / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);

            FlowLayoutPanel dialogDatePanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, dialogSumPanel.Bottom + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(0));
            Label dialogDateLabel = createLabel.CreateLabelsInPanel(dialogDatePanel, "dialogDateLabel", now.ToLocalTime().ToString(), 0, 0, dialogDatePanel.Width, 50, Color.Transparent, Color.Black, 16, false, ContentAlignment.TopLeft);
            dialogDateLabel.Padding = new Padding(50, 0, 0, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);

            dialogForm.ShowDialog();

        }

        public void ReceiptIssueReport()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }


            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;
            //DateTime sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 00);
            //DateTime sumDayTime2 = sumDayTime1.AddDays(1).AddSeconds(-1);

            Panel dialogTitlePanel = createPanel.CreateMainPanel(dialogForm, 0, 30, dialogForm.Width, 50, BorderStyle.None, Color.Transparent);
            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle1", constants.receiptionTitle, 0, 0, dialogTitlePanel.Width / 2 - 30, 50, Color.Transparent, Color.Black, 22);
            Label dialogTitle2 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle2", logDate, dialogTitlePanel.Width / 2, 0, dialogTitlePanel.Width / 2 - 30, 50, Color.Transparent, Color.Black, 22);

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 10, 80, dialogForm.Width - 30, dialogForm.Height - 180, BorderStyle.None, Color.Transparent);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            int k = 0;
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[8] + " WHERE sumDate=@sumDate ORDER BY ReceiptDate DESC";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    int purchasePoint = sqlite_datareader.GetInt32(1);
                    int totalPrice = sqlite_datareader.GetInt32(2);
                    DateTime receiptDate = sqlite_datareader.GetDateTime(3);

                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, 41 * k, dialogPanel.Width, 40, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", receiptDate.ToString("yyyy/MM/dd HH:mm:dd"), 0, 0, productTableBody.Width / 3, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", purchasePoint.ToString() + constants.amountUnit1, productTableBody.Width / 3, 0, productTableBody.Width / 3 - 50, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3", totalPrice.ToString() + constants.unit, productTableBody.Width * 2 / 3 - 50, 0, productTableBody.Width / 3 - 50, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    dialogFlowLayout = productTableBody;
                    k++;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            FlowLayoutPanel dialogSumPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, k * 41 + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(10));
            Label dialogSumLabel1 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel1", "合計", 0, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 14);
            Label dialogSumLabel2 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", k + constants.amountUnit, dialogSumPanel.Width / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 14);

            FlowLayoutPanel dialogDatePanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, dialogSumPanel.Bottom + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(10));
            // dialogDatePanel.Padding = new Padding(dialogDatePanel.Width * 2 / 3, 0, 0, 0);
            Label dialogDateLabel = createLabel.CreateLabelsInPanel(dialogDatePanel, "dialogDateLabel", now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), 0, 0, dialogDatePanel.Width, 50, Color.Transparent, Color.Black, 14, false, ContentAlignment.TopLeft);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);

            dialogForm.ShowDialog();
        }

        public void LogReport()
        {
            dropDownMenu.InitLogDetailReport(this);
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }

            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height - 100, BorderStyle.None, Color.Transparent);

            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;
            DateTime now = DateTime.Now;

            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.logReportLabel + " " + logDate, 0, 30, dialogPanel.Width * 2 / 3 - 50, 60, Color.Transparent, Color.Black, 20);

            Panel dialogPanelBody = createPanel.CreateSubPanel(dialogPanel, 0, 80, dialogForm.Width - 30, dialogForm.Height - 200, BorderStyle.None, Color.Transparent);
            dialogPanelBody.Padding = new Padding(0, 5, 0, 10);
            dialogPanelBody.HorizontalScroll.Maximum = 0;
            dialogPanelBody.AutoScroll = false;
            dialogPanelBody.VerticalScroll.Visible = false;
            dialogPanelBody.AutoScroll = true;

            dialogPanelGlobal = dialogPanelBody;
            LogReportBody("00", dialogPanelBody);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);


            dialogForm.ShowDialog();

        }

        private void LogReportBody(string logReportTime, Panel dialogPanel)
        {
            //bottomPosition = new int[10];
            int bottomPositions = 0;
            int k = 0;
            int m = 0;

            //sumDayTime1 = constants.sumDayTimeStart(storeEndTime);
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            DateTime sumDayTime1 = DateTime.Now;
            DateTime sumDayTime2 = DateTime.Now;

            if(int.Parse(logReportTime) == 0)
            {
                sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), 00, 00, 00);
                sumDayTime2 = sumDayTime1.AddDays(1).AddSeconds(-1);
            }
            else
            {
                sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), (int.Parse(logReportTime) - 1), 00, 00);
                sumDayTime2 = sumDayTime1.AddHours(1).AddSeconds(-1);
            }

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT saleDate, sum(prdPrice * prdAmount), ticketNo, count(id) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 and saleDate<=@saleDate2 GROUP BY ticketNo ORDER BY saleDate DESC";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    DateTime saleDate = sqlite_datareader.GetDateTime(0);
                    int totalPrice = sqlite_datareader.GetInt32(1);
                    int ticketNo = sqlite_datareader.GetInt32(2);

                    //if (k != 9)
                    //{
                    //bottomPosition[k + 1] = bottomPosition[k] + 51 + m * 51;
                    //}

                    int addHeight = 0;
                    if (k != 0)
                    {
                        addHeight = 10;
                        PictureBox pB = new PictureBox();
                        pB.Location = new Point(50, bottomPositions + 10);
                        pB.Size = new Size(dialogPanel.Width - 100, 10);
                        dialogPanel.Controls.Add(pB);
                        Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
                        Graphics g = Graphics.FromImage(image);
                        g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, dialogPanel.Width - 160, 5);
                        pB.Image = image;

                    }
                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(dialogPanel, dialogPanel.Width / 10, bottomPositions + addHeight, dialogPanel.Width * 4 / 5, 50, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", saleDate.ToString("yyyy/MM/dd HH:mm:ss"), 0, 0, productTableBody.Width / 3 + 30, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12, false, ContentAlignment.MiddleLeft);
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", "", productTableBody.Width / 3 + 40, 0, productTableBody.Width / 3 - 60, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3", "計 " + totalPrice + constants.unit, productTableBody.Width * 2 / 3 - 20, 0, productTableBody.Width / 3 - 40, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12, false, ContentAlignment.MiddleRight);
                    dialogFlowLayout = productTableBody;
                    bottomPositions += 51;
                    m = 0;
                    SQLiteCommand sqlite_cmds;
                    SQLiteDataReader sqlite_datareaders;

                    sqlite_cmds = sqlite_conn.CreateCommand();
                    string daySumqureys = "SELECT * FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 AND ticketNo=@ticketNo ORDER BY saleDate DESC";
                    sqlite_cmds.CommandText = daySumqureys;
                    sqlite_cmds.Parameters.AddWithValue("@saleDate1", sumDayTime1);
                    sqlite_cmds.Parameters.AddWithValue("@saleDate2", sumDayTime2);
                    sqlite_cmds.Parameters.AddWithValue("@ticketNo", ticketNo);
                    sqlite_datareaders = sqlite_cmds.ExecuteReader();
                    while (sqlite_datareaders.Read())
                    {
                        if (!sqlite_datareaders.IsDBNull(0))
                        {
                            string prdName = sqlite_datareaders.GetString(3);
                            int prdAmount = sqlite_datareaders.GetInt32(5);
                            int prdPrice = sqlite_datareaders.GetInt32(4);

                            FlowLayoutPanel productTableBodyContent = createPanel.CreateFlowLayoutPanel(dialogPanel, dialogPanel.Width / 10, productTableBody.Bottom + 41 * m, dialogPanel.Width * 4 / 5, 40, Color.Transparent, new Padding(0));
                            Label prodNameBodyContent1 = createLabel.CreateLabelsInPanel(productTableBodyContent, "prodBody_" + m + "_1", prdName, 0, 0, productTableBodyContent.Width / 3 - 10, productTableBodyContent.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12, false, ContentAlignment.MiddleLeft);
                            Label prodNameBodyContent2 = createLabel.CreateLabelsInPanel(productTableBodyContent, "prodBody_" + m + "_2", prdAmount.ToString() + constants.amountUnit, productTableBodyContent.Width / 3, 0, productTableBodyContent.Width / 3 - 30, productTableBodyContent.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                            Label prodNameBodyContent3 = createLabel.CreateLabelsInPanel(productTableBodyContent, "prodBody_" + m + "_3", prdPrice.ToString() + constants.unit, productTableBodyContent.Width * 2 / 3 - 30, 0, productTableBodyContent.Width / 3 - 30, productTableBodyContent.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12, false, ContentAlignment.MiddleRight);
                            dialogFlowLayout = productTableBodyContent;

                            bottomPositions += 41;
                            m++;
                        }
                    }

                    sqlite_datareaders.Close();
                    sqlite_datareaders = null;
                    sqlite_cmds.Dispose();
                    sqlite_cmds = null;

                    k++;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void AllLogView()
        {
            if(logHandlerGlobal.r1Global.Checked == true)
            {
                YearLogView();
            }
            else if (logHandlerGlobal.r2Global.Checked == true)
            {
                MonthLogView();
            }
            else if(logHandlerGlobal.r3Global.Checked == true)
            {
                RangeLogView();
            }
        }

        private void YearLogView()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.Padding = new Padding(0, 0, 0, 20);
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }

            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;

            DialogFormGlobal = dialogForm;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 10, 20, dialogForm.Width - 30, dialogForm.Height - 100, BorderStyle.None, Color.Transparent);
            dialogPanel.Margin = new Padding(0, 20, 0, 20);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            string logYear = logHandlerGlobal.yearComboboxOnlyGlobal.GetItemText(logHandlerGlobal.yearComboboxOnlyGlobal.SelectedItem);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", constants.yearDataLabel + "          " + logYear + "年度", 30, 20, dialogPanel.Width - 60, 60, Color.Transparent, Color.Black, 26);

            int totalTickets = dbClass.TotalTicketPrice(logYear)[0];
            int totalPrice = dbClass.TotalTicketPrice(logYear)[1];

            Label subtitleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, 30, titleLabel.Bottom + 10, dialogPanel.Width - 60, 40, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);

            DateTime startDate = new DateTime(int.Parse(logYear), 01, 01, 00, 00, 00);
            DateTime endDate = new DateTime(int.Parse(logYear), 12, 31, 23, 59, 59);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string yearSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
            sqlite_cmd.CommandText = yearSumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int currentY = 10;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(0);
                    int prdPrice = sqlite_datareader.GetInt32(1);
                    int prdAmount = sqlite_datareader.GetInt32(2);
                    int prdTotal = prdPrice * prdAmount;
                    Label prdNamelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdNamelb", prdName, 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 + 100, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    Label prdAmountlb = createLabel.CreateLabelsInPanel(dialogPanel, "prdAmountlb", prdAmount + constants.amountUnit, prdNamelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 50, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdPricelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdPricelb", prdPrice + constants.unit, prdAmountlb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 30, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdTotallb = createLabel.CreateLabelsInPanel(dialogPanel, "prdTotallb", String.Format("{0:0,0}", prdTotal) + constants.unit, prdPricelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 20, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                    currentY += 30;
                }
            }

            sqlite_datareader = null;
            sqlite_cmd = null;

            PictureBox pB = new PictureBox();
            pB.Location = new Point(50, subtitleLabel.Bottom + currentY + 10);
            pB.Size = new Size(dialogPanel.Width - 100, 10);
            dialogPanel.Controls.Add(pB);
            Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
            Graphics g = Graphics.FromImage(image);
            g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, dialogPanel.Width - 160, 5);
            pB.Image = image;
            currentY += 20;

            for(int k = 1; k <= 12; k++)
            {
                totalTickets = dbClass.TotalTicketPrice(logYear, k.ToString("00"))[0];
                totalPrice = dbClass.TotalTicketPrice(logYear, k.ToString("00"))[1];
                int mdays = DateTime.DaysInMonth(int.Parse(logYear), k);
                Label subtitleLabel1 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel1", logYear+ "/" + k.ToString("00"), 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                Label subtitleLabel2 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel2", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, subtitleLabel1.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) * 2 / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);

                startDate = new DateTime(int.Parse(logYear), k, 01, 00, 00, 00);
                endDate = new DateTime(int.Parse(logYear), k, mdays, 23, 59, 59);

                sqlite_cmd = sqlite_conn.CreateCommand();
                string monthSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
                sqlite_cmd.CommandText = yearSumqurey;
                sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
                sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                currentY += 30;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(0);
                        int prdPrice = sqlite_datareader.GetInt32(1);
                        int prdAmount = sqlite_datareader.GetInt32(2);
                        int prdTotal = prdPrice * prdAmount;
                        Label prdNamelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdNamelb", prdName, 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 + 100, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                        Label prdAmountlb = createLabel.CreateLabelsInPanel(dialogPanel, "prdAmountlb", prdAmount + constants.amountUnit, prdNamelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 50, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                        Label prdPricelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdPricelb", prdPrice + constants.unit, prdAmountlb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 30, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                        Label prdTotallb = createLabel.CreateLabelsInPanel(dialogPanel, "prdTotallb", String.Format("{0:0,0}", prdTotal) + constants.unit, prdPricelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 20, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                        currentY += 30;
                    }
                }

            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);

            dialogForm.ShowDialog();

        }

        private void MonthLogView()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.Padding = new Padding(0, 0, 0, 20);
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }

            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;

            DialogFormGlobal = dialogForm;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 10, 20, dialogForm.Width - 30, dialogForm.Height - 100, BorderStyle.None, Color.Transparent);
            dialogPanel.Margin = new Padding(0, 20, 0, 20);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            string logYear = logHandlerGlobal.yearComboboxTwoGlobal.GetItemText(logHandlerGlobal.yearComboboxTwoGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxTwoGlobal.GetItemText(logHandlerGlobal.monthComboboxTwoGlobal.SelectedItem);
            int monthDays = DateTime.DaysInMonth(int.Parse(logYear), int.Parse(logMonth));

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", constants.monthDataLabel + "          " + logYear + constants.yearLabel + logMonth + constants.monthLabel, 30, 20, dialogPanel.Width - 60, 60, Color.Transparent, Color.Black, 26);

            int totalTickets = dbClass.TotalTicketPrice(logYear, logMonth)[0];
            int totalPrice = dbClass.TotalTicketPrice(logYear, logMonth)[1];
            
            Label subtitleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, 30, titleLabel.Bottom + 10, dialogPanel.Width - 60, 40, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);

            DateTime startDate = new DateTime(int.Parse(logYear), int.Parse(logMonth), 01, 00, 00, 00);
            DateTime endDate = new DateTime(int.Parse(logYear), int.Parse(logMonth), monthDays, 23, 59, 59);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string yearSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
            sqlite_cmd.CommandText = yearSumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int currentY = 10;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(0);
                    int prdPrice = sqlite_datareader.GetInt32(1);
                    int prdAmount = sqlite_datareader.GetInt32(2);
                    int prdTotal = prdPrice * prdAmount;
                    Label prdNamelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdNamelb", prdName, 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 + 100, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    Label prdAmountlb = createLabel.CreateLabelsInPanel(dialogPanel, "prdAmountlb", prdAmount + constants.amountUnit, prdNamelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 50, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdPricelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdPricelb", prdPrice + constants.unit, prdAmountlb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 30, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdTotallb = createLabel.CreateLabelsInPanel(dialogPanel, "prdTotallb", String.Format("{0:0,0}", prdTotal) + constants.unit, prdPricelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 20, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                    currentY += 30;
                }
            }

            PictureBox pB = new PictureBox();
            pB.Location = new Point(50, subtitleLabel.Bottom + currentY + 10);
            pB.Size = new Size(dialogPanel.Width - 100, 10);
            dialogPanel.Controls.Add(pB);
            Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
            Graphics g = Graphics.FromImage(image);
            g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, dialogPanel.Width - 160, 5);
            pB.Image = image;
            currentY += 20;

            for (int k = 1; k <= monthDays; k++)
            {
                totalTickets = dbClass.TotalTicketPrice(logYear, logMonth, k.ToString("00"))[0];
                totalPrice = dbClass.TotalTicketPrice(logYear, logMonth, k.ToString("00"))[1];
                Label subtitleLabel1 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel1", logYear + "/" + logMonth + "/" + k.ToString("00"), 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                Label subtitleLabel2 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel2", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, subtitleLabel1.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) * 2 / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                currentY += 30;
            }

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            dialogForm.ShowDialog();
        }

        private void RangeLogView()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.Padding = new Padding(0, 0, 0, 20);
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }

            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;

            DialogFormGlobal = dialogForm;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 10, 20, dialogForm.Width - 30, dialogForm.Height - 100, BorderStyle.None, Color.Transparent);
            dialogPanel.Margin = new Padding(0, 20, 0, 20);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            string logYears = logHandlerGlobal.yearComboboxStartGlobal.GetItemText(logHandlerGlobal.yearComboboxStartGlobal.SelectedItem);
            string logMonths = logHandlerGlobal.monthComboboxStartGlobal.GetItemText(logHandlerGlobal.monthComboboxStartGlobal.SelectedItem);
            string logDays = logHandlerGlobal.dateComboboxStartGlobal.GetItemText(logHandlerGlobal.dateComboboxStartGlobal.SelectedItem);

            string logYeare = logHandlerGlobal.yearComboboxEndGlobal.GetItemText(logHandlerGlobal.yearComboboxEndGlobal.SelectedItem);
            string logMonthe = logHandlerGlobal.monthComboboxEndGlobal.GetItemText(logHandlerGlobal.monthComboboxEndGlobal.SelectedItem);
            string logDaye = logHandlerGlobal.dateComboboxEndGlobal.GetItemText(logHandlerGlobal.dateComboboxEndGlobal.SelectedItem);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", constants.rangeDataLabel + "            " + logYears + "/" + logMonths + "/" + logDays + "-" + logYeare + "/" + logMonthe + "/" + logDaye, 30, 20, dialogPanel.Width - 60, 60, Color.Transparent, Color.Black, 24);

            int totalTickets = dbClass.TotalTicketPriceRange(logYears, logMonths, logDays, logYeare, logMonthe, logDaye)[0];
            int totalPrice = dbClass.TotalTicketPriceRange(logYears, logMonths, logDays, logYeare, logMonthe, logDaye)[1];

            Label subtitleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, 30, titleLabel.Bottom + 10, dialogPanel.Width - 60, 40, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);

            DateTime startDate = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), 00, 00, 00);
            DateTime endDate = new DateTime(int.Parse(logYeare), int.Parse(logMonthe), int.Parse(logDaye), 23, 59, 59);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string yearSumqurey = "SELECT prdName, prdPrice, SUM(prdAmount) as prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
            sqlite_cmd.CommandText = yearSumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int currentY = 10;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(0);
                    int prdPrice = sqlite_datareader.GetInt32(1);
                    int prdAmount = sqlite_datareader.GetInt32(2);
                    int prdTotal = prdPrice * prdAmount;
                    Label prdNamelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdNamelb", prdName, 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 + 100, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                    Label prdAmountlb = createLabel.CreateLabelsInPanel(dialogPanel, "prdAmountlb", prdAmount + constants.amountUnit, prdNamelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 50, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdPricelb = createLabel.CreateLabelsInPanel(dialogPanel, "prdPricelb", prdPrice + constants.unit, prdAmountlb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 30, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    Label prdTotallb = createLabel.CreateLabelsInPanel(dialogPanel, "prdTotallb", String.Format("{0:0,0}", prdTotal) + constants.unit, prdPricelb.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 4 - 20, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                    currentY += 30;
                }
            }

            PictureBox pB = new PictureBox();
            pB.Location = new Point(50, subtitleLabel.Bottom + currentY + 10);
            pB.Size = new Size(dialogPanel.Width - 100, 10);
            dialogPanel.Controls.Add(pB);
            Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
            Graphics g = Graphics.FromImage(image);
            g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, dialogPanel.Width - 160, 5);
            pB.Image = image;
            currentY += 20;

            foreach (DateTime day in EachCalendarDay(startDate, endDate))
            {
                totalTickets = dbClass.TotalTicketPrice(day.ToString("yyyy"), day.ToString("MM"), day.ToString("dd"))[0];
                totalPrice = dbClass.TotalTicketPrice(day.ToString("yyyy"), day.ToString("MM"), day.ToString("dd"))[1];
                Label subtitleLabel1 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel1", day.ToString("yyyy/MM/dd"), 30, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
                Label subtitleLabel2 = createLabel.CreateLabelsInPanel(dialogPanel, "subtitleLabel2", constants.sumLabel + totalTickets + constants.amountUnit + " " + String.Format("{0:0,0}", totalPrice) + constants.unit, subtitleLabel1.Right, subtitleLabel.Bottom + currentY, (dialogPanel.Width - 60) * 2 / 3, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleRight);
                currentY += 30;
            }

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            closeButton.Click += new EventHandler(this.CloseDialog);
            dialogForm.Controls.Add(closeButton);

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            dialogForm.ShowDialog();
        }

        private void AllLogSave()
        {
            if (logHandlerGlobal.r1Global.Checked == true)
            {
                YearLogSave();
            }
            else if (logHandlerGlobal.r2Global.Checked == true)
            {
                MonthLogSave();
            }
            else if (logHandlerGlobal.r3Global.Checked == true)
            {
                RangeLogSave();
            }
        }

        private void YearLogSave()
        {
            string logYear = logHandlerGlobal.yearComboboxOnlyGlobal.GetItemText(logHandlerGlobal.yearComboboxOnlyGlobal.SelectedItem);
            int totalTickets = dbClass.TotalTicketPrice(logYear)[0];
            int totalPrice = dbClass.TotalTicketPrice(logYear)[1];

            //string currentDir = Directory.GetCurrentDirectory();
            string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }

            if (!Directory.Exists(currentDir + "/saleData"))
            {
                Directory.CreateDirectory(currentDir + "/saleData");
            }


            string csvPath = Path.Combine(currentDir, "saleData", "yearLog_" + logYear + ".csv");

            using (var writer = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.Configuration.Delimiter = ",";

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.yearDataLabel);
                csvWriter.WriteField("");
                csvWriter.WriteField(logYear + "年度");
                csvWriter.WriteField("");
                csvWriter.NextRecord();

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.sumLabel);
                csvWriter.WriteField(totalTickets + constants.amountUnit);
                csvWriter.WriteField("");
                csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                csvWriter.NextRecord();

                csvWriter.WriteField(constants.prdNameField);
                csvWriter.WriteField("");
                csvWriter.WriteField("数量");
                csvWriter.WriteField(constants.priceTitle);
                csvWriter.WriteField(constants.priceField);
                csvWriter.NextRecord();
                DateTime startDate = new DateTime(int.Parse(logYear), 01, 01, 00, 00, 00);
                DateTime endDate = new DateTime(int.Parse(logYear), 12, 31, 23, 59, 59);

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteCommand sqlite_cmd;
                SQLiteDataReader sqlite_datareader;

                sqlite_cmd = sqlite_conn.CreateCommand();
                string yearSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
                sqlite_cmd.CommandText = yearSumqurey;
                sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
                sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int currentY = 10;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(0);
                        int prdPrice = sqlite_datareader.GetInt32(1);
                        int prdAmount = sqlite_datareader.GetInt32(2);
                        int prdTotal = prdPrice * prdAmount;
                        csvWriter.WriteField(prdName);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(prdAmount + constants.amountUnit);
                        csvWriter.WriteField(prdPrice + constants.unit);
                        csvWriter.WriteField(String.Format("{0:0,0}", prdTotal) + constants.unit);
                        csvWriter.NextRecord();
                    }
                }
                csvWriter.NextRecord();
                for (int k = 1; k <= 12; k++)
                {
                    totalTickets = dbClass.TotalTicketPrice(logYear, k.ToString("00"))[0];
                    totalPrice = dbClass.TotalTicketPrice(logYear, k.ToString("00"))[1];
                    int mdays = DateTime.DaysInMonth(int.Parse(logYear), k);
                    csvWriter.WriteField(logYear + "/" + k.ToString("00"));
                    csvWriter.WriteField("");
                    csvWriter.WriteField(constants.sumLabel);
                    csvWriter.WriteField(totalTickets + constants.amountUnit);
                    csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                    csvWriter.NextRecord();


                    startDate = new DateTime(int.Parse(logYear), k, 01, 00, 00, 00);
                    endDate = new DateTime(int.Parse(logYear), k, mdays, 23, 59, 59);

                    sqlite_cmd = sqlite_conn.CreateCommand();
                    string monthSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
                    sqlite_cmd.CommandText = yearSumqurey;
                    sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
                    sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    currentY += 30;
                    while (sqlite_datareader.Read())
                    {
                        if (!sqlite_datareader.IsDBNull(0))
                        {
                            string prdName = sqlite_datareader.GetString(0);
                            int prdPrice = sqlite_datareader.GetInt32(1);
                            int prdAmount = sqlite_datareader.GetInt32(2);
                            int prdTotal = prdPrice * prdAmount;
                            csvWriter.WriteField(prdName);
                            csvWriter.WriteField("");
                            csvWriter.WriteField(prdAmount + constants.amountUnit);
                            csvWriter.WriteField(prdPrice + constants.unit);
                            csvWriter.WriteField(String.Format("{0:0,0}", prdTotal) + constants.unit);
                            csvWriter.NextRecord();

                        }
                    }

                }
                writer.Flush();
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;

            }

            string msg = "範囲データを yearLog_" + logYear + ".csvに保管しました。";

            messageDialog.LogSaveMessage(msg);

        }

        private void MonthLogSave()
        {
            string logYear = logHandlerGlobal.yearComboboxTwoGlobal.GetItemText(logHandlerGlobal.yearComboboxTwoGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxTwoGlobal.GetItemText(logHandlerGlobal.monthComboboxTwoGlobal.SelectedItem);
            int monthDays = DateTime.DaysInMonth(int.Parse(logYear), int.Parse(logMonth));
            int totalTickets = dbClass.TotalTicketPrice(logYear, logMonth)[0];
            int totalPrice = dbClass.TotalTicketPrice(logYear, logMonth)[1];

            //string currentDir = Directory.GetCurrentDirectory();
            string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            if (!Directory.Exists(currentDir + "/saleData"))
            {
                Directory.CreateDirectory(currentDir + "/saleData");
            }
            string csvPath = Path.Combine(currentDir, "saleData", "monthLog_"+ logYear + "_" + logMonth + ".csv");

            using (var writer = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.Configuration.Delimiter = ",";

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.monthDataLabel);
                csvWriter.WriteField("");
                csvWriter.WriteField(logYear + constants.yearLabel + logMonth + constants.monthLabel);
                csvWriter.WriteField("");
                csvWriter.NextRecord();

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.sumLabel);
                csvWriter.WriteField(totalTickets + constants.amountUnit);
                csvWriter.WriteField("");
                csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                csvWriter.NextRecord();

                csvWriter.WriteField(constants.prdNameField);
                csvWriter.WriteField("");
                csvWriter.WriteField("数量");
                csvWriter.WriteField(constants.priceTitle);
                csvWriter.WriteField(constants.priceField);
                csvWriter.NextRecord();

                DateTime startDate = new DateTime(int.Parse(logYear), int.Parse(logMonth), 01, 00, 00, 00);
                DateTime endDate = new DateTime(int.Parse(logYear), int.Parse(logMonth), monthDays, 23, 59, 59);

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteCommand sqlite_cmd;
                SQLiteDataReader sqlite_datareader;

                sqlite_cmd = sqlite_conn.CreateCommand();
                string yearSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
                sqlite_cmd.CommandText = yearSumqurey;
                sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
                sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(0);
                        int prdPrice = sqlite_datareader.GetInt32(1);
                        int prdAmount = sqlite_datareader.GetInt32(2);
                        int prdTotal = prdPrice * prdAmount;
                        csvWriter.WriteField(prdName);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(prdAmount + constants.amountUnit);
                        csvWriter.WriteField(prdPrice + constants.unit);
                        csvWriter.WriteField(String.Format("{0:0,0}", prdTotal) + constants.unit);
                        csvWriter.NextRecord();

                    }

                }
                for (int k = 1; k <= monthDays; k++)
                {
                    totalTickets = dbClass.TotalTicketPrice(logYear, logMonth, k.ToString("00"))[0];
                    totalPrice = dbClass.TotalTicketPrice(logYear, logMonth, k.ToString("00"))[1];
                    csvWriter.WriteField(logYear + "/" + logMonth + "/" + k.ToString("00"));
                    csvWriter.WriteField("");
                    csvWriter.WriteField(constants.sumLabel);
                    csvWriter.WriteField(totalTickets + constants.amountUnit);
                    csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                    csvWriter.NextRecord();
                }

                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;


                string msg = "範囲データを monthLog_" + logYear + "_" + logMonth + ".csvに保管しました。";
                messageDialog.LogSaveMessage(msg);
            }
        }

        private void RangeLogSave()
        {
            string logYears = logHandlerGlobal.yearComboboxStartGlobal.GetItemText(logHandlerGlobal.yearComboboxStartGlobal.SelectedItem);
            string logMonths = logHandlerGlobal.monthComboboxStartGlobal.GetItemText(logHandlerGlobal.monthComboboxStartGlobal.SelectedItem);
            string logDays = logHandlerGlobal.dateComboboxStartGlobal.GetItemText(logHandlerGlobal.dateComboboxStartGlobal.SelectedItem);

            string logYeare = logHandlerGlobal.yearComboboxEndGlobal.GetItemText(logHandlerGlobal.yearComboboxEndGlobal.SelectedItem);
            string logMonthe = logHandlerGlobal.monthComboboxEndGlobal.GetItemText(logHandlerGlobal.monthComboboxEndGlobal.SelectedItem);
            string logDaye = logHandlerGlobal.dateComboboxEndGlobal.GetItemText(logHandlerGlobal.dateComboboxEndGlobal.SelectedItem);
            int totalTickets = dbClass.TotalTicketPriceRange(logYears, logMonths, logDays, logYeare, logMonthe, logDaye)[0];
            int totalPrice = dbClass.TotalTicketPriceRange(logYears, logMonths, logDays, logYeare, logMonthe, logDaye)[1];

            //string currentDir = Directory.GetCurrentDirectory();
            string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            if (!Directory.Exists(currentDir + "/saleData"))
            {
                Directory.CreateDirectory(currentDir + "/saleData");
            }
            string csvPath = Path.Combine(currentDir, "saleData", "rangeLog_" + logYears + "_" + logMonths + "_" + logDays + "_" + logYeare + "_" + logMonthe + "_" + logDaye + ".csv");

            using (var writer = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.Configuration.Delimiter = ",";

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.rangeDataLabel);
                csvWriter.WriteField("");
                csvWriter.WriteField(logYears + "/" + logMonths + "/" + logDays + "-" + logYeare + "/" + logMonthe + "/" + logDaye);
                csvWriter.WriteField("");
                csvWriter.NextRecord();

                csvWriter.WriteField("");
                csvWriter.WriteField(constants.sumLabel);
                csvWriter.WriteField(totalTickets + constants.amountUnit);
                csvWriter.WriteField("");
                csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                csvWriter.NextRecord();

                csvWriter.WriteField(constants.prdNameField);
                csvWriter.WriteField("");
                csvWriter.WriteField("数量");
                csvWriter.WriteField(constants.priceTitle);
                csvWriter.WriteField(constants.priceField);
                csvWriter.NextRecord();
                DateTime startDate = new DateTime(int.Parse(logYears), int.Parse(logMonths), int.Parse(logDays), 00, 00, 00);
                DateTime endDate = new DateTime(int.Parse(logYeare), int.Parse(logMonthe), int.Parse(logDaye), 23, 59, 59);

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteCommand sqlite_cmd;
                SQLiteDataReader sqlite_datareader;

                sqlite_cmd = sqlite_conn.CreateCommand();
                string yearSumqurey = "SELECT prdName, prdPrice, prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY prdRealID";
                sqlite_cmd.CommandText = yearSumqurey;
                sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
                sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(0);
                        int prdPrice = sqlite_datareader.GetInt32(1);
                        int prdAmount = sqlite_datareader.GetInt32(2);
                        int prdTotal = prdPrice * prdAmount;
                        csvWriter.WriteField(prdName);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(prdAmount + constants.amountUnit);
                        csvWriter.WriteField(prdPrice + constants.unit);
                        csvWriter.WriteField(String.Format("{0:0,0}", prdTotal) + constants.unit);
                        csvWriter.NextRecord();

                    }
                }
                foreach (DateTime day in EachCalendarDay(startDate, endDate))
                {
                    totalTickets = dbClass.TotalTicketPrice(day.ToString("yyyy"), day.ToString("MM"), day.ToString("dd"))[0];
                    totalPrice = dbClass.TotalTicketPrice(day.ToString("yyyy"), day.ToString("MM"), day.ToString("dd"))[1];
                    csvWriter.WriteField(day.ToString("yyyy/MM/dd"));
                    csvWriter.WriteField("");
                    csvWriter.WriteField(constants.sumLabel);
                    csvWriter.WriteField(totalTickets + constants.amountUnit);
                    csvWriter.WriteField(String.Format("{0:0,0}", totalPrice) + constants.unit);
                    csvWriter.NextRecord();

                }

                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;


                string msg = "範囲データを rangeLog_" + logYears + "_" + logMonths + "_" + logDays + "_" + logYeare + "_" + logMonthe + "_" + logDaye + ".csvに保管しました。";
                messageDialog.LogSaveMessage(msg);
            }

        }

        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }


        public void CloseDialog(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
        }

        private string DateTimeFormat(int val)
        {
            if (val < 10)
            {
                return "0" + val.ToString();
            }
            return val.ToString();
        }


        private void DailyReportPrintPage(object sender, PrintPageEventArgs e)
        {
            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            float currentY = 40;// declare  one variable for height measurement
            RectangleF rect1 = new RectangleF(5, currentY, constants.dailyReportPrintPaperWidth - 5, 30);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(constants.dailyReportTitle + " " + logDate, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            currentY += 40;
            int soldPriceSum = 0;
            int soldAmountSum = 0;
            int k = 0;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", sumDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(2);
                    int prdPrice = sqlite_datareader.GetInt32(3);
                    int prdAmount = sqlite_datareader.GetInt32(4);
                    int prdTotalPrice = sqlite_datareader.GetInt32(5);
                    soldPriceSum += prdTotalPrice;
                    soldAmountSum += prdAmount;

                    RectangleF rect2 = new RectangleF(5, currentY, constants.dailyReportPrintPaperWidth * 2 / 5, 30);
                    StringFormat format2 = new StringFormat();
                    format2.Alignment = StringAlignment.Near;
                    e.Graphics.DrawString(prdName, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//print each item

                    RectangleF rect3 = new RectangleF(constants.dailyReportPrintPaperWidth * 2 / 5 + 5, currentY, constants.dailyReportPrintPaperWidth / 5 - 5, 30);
                    StringFormat format3 = new StringFormat();
                    format3.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(prdAmount.ToString() + constants.amountUnit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect3, format3);//print each item

                    RectangleF rect4 = new RectangleF(constants.dailyReportPrintPaperWidth * 3 / 5, currentY, constants.singleticketPrintPaperWidth / 5, 30);
                    StringFormat format4 = new StringFormat();
                    format4.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(prdPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

                    RectangleF rect5 = new RectangleF(constants.dailyReportPrintPaperWidth * 4 / 5, currentY, constants.dailyReportPrintPaperWidth / 5, 30);
                    StringFormat format5 = new StringFormat();
                    format5.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString(prdTotalPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect5, format5);//print each item
                    currentY += 30;

                    e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
                    k++;

                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            RectangleF rect6 = new RectangleF(0, currentY, constants.dailyReportPrintPaperWidth * 3 / 7, 30);
            StringFormat format6 = new StringFormat();
            format6.Alignment = StringAlignment.Far;
            e.Graphics.DrawString("合計: " + soldAmountSum + constants.amountUnit, new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect6, format6);//print each item

            RectangleF rect7 = new RectangleF(constants.dailyReportPrintPaperWidth * 4 / 7, currentY, constants.dailyReportPrintPaperWidth * 3 / 7, 30);
            StringFormat format7 = new StringFormat();
            format7.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(soldPriceSum + " " + constants.unit, new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect7, format7);//print each item
            currentY += 30;

            RectangleF rect8 = new RectangleF(constants.dailyReportPrintPaperWidth * 1 / 10, currentY, constants.dailyReportPrintPaperWidth * 9 / 10, 30);
            StringFormat format8 = new StringFormat();
            format8.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect8, format8);//print each item
            currentY += 30;
            return;

        }

        private void ReceiptIssueReportPrintPage(object sender, PrintPageEventArgs e)
        {
            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;
            //sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 00);
            //sumDayTime2 = sumDayTime1.AddDays(1).AddSeconds(-1);


            float currentY = 0;// declare  one variable for height measurement
            RectangleF rect1 = new RectangleF(0, currentY, constants.receiptReportPrintPaperWidth, 30);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(constants.receiptionTitle + "  " + logDate, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            currentY += 30;

            RectangleF rect2 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth * 3 / 5, 25);
            StringFormat format2 = new StringFormat();
            format2.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(constants.receiptionField, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//this will print one heading/title in every page of the document
            RectangleF rect3 = new RectangleF(5 + constants.receiptReportPrintPaperWidth * 4 / 5, currentY, constants.receiptReportPrintPaperWidth * 1 / 5, 25);
            StringFormat format3 = new StringFormat();
            format3.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(constants.priceField, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect3, format3);//this will print one heading/title in
            currentY += 25;
            int k = 0;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[8] + " WHERE sumDate=@sumDate";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    int purchasePoint = sqlite_datareader.GetInt32(1);
                    int totalPrice = sqlite_datareader.GetInt32(2);
                    DateTime receiptDate = sqlite_datareader.GetDateTime(3);

                    RectangleF rect4 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth * 3 / 5, 20);
                    StringFormat format4 = new StringFormat();
                    format4.Alignment = StringAlignment.Near;
                    e.Graphics.DrawString(receiptDate.ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

                    RectangleF rect5 = new RectangleF(5 + constants.receiptReportPrintPaperWidth * 3 / 5, currentY, constants.receiptReportPrintPaperWidth / 5 - 5, 20);
                    StringFormat format5 = new StringFormat();
                    format5.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(purchasePoint.ToString() + constants.amountUnit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect5, format5);//print each item

                    RectangleF rect6 = new RectangleF(constants.receiptReportPrintPaperWidth * 4 / 5, currentY, constants.receiptReportPrintPaperWidth / 5, 20);
                    StringFormat format6 = new StringFormat();
                    format6.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString(totalPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect6, format6);//print each item
                    currentY += 20;
                    e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added

                    k++;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            RectangleF rect7 = new RectangleF(0, currentY, constants.receiptReportPrintPaperWidth, 30);
            StringFormat format7 = new StringFormat();
            format7.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("合計: " + k + constants.amountUnit, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect7, format7);//print each item
            currentY += 30;

            RectangleF rect8 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth - 5, 30);
            StringFormat format8 = new StringFormat();
            format8.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect8, format8);//print each item
            currentY += 30;
        }

        private void LogPrint(object sender, PrintPageEventArgs e)
        {

            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            DateTime sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), 00, 00, 00);
            DateTime sumDayTime2 = sumDayTime1.AddDays(1).AddSeconds(-1);


            float currentY = 10;// declare  one variable for height measurement
            RectangleF rect1 = new RectangleF(15, currentY, constants.logPrintPaperWidth - 15, 30);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(constants.logReportLabel + " " + logDate, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            currentY += 30;
            int k = 0;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT saleDate, sum(prdPrice * prdAmount), ticketNo, count(id) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 and saleDate<=@saleDate2 GROUP BY ticketNo ORDER BY saleDate DESC";

            sqlite_cmd.CommandText = daySumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    DateTime saleDate = sqlite_datareader.GetDateTime(0);
                    int totalPrice = sqlite_datareader.GetInt32(1);
                    int ticketNo = sqlite_datareader.GetInt32(2);

                    RectangleF rect1_1 = new RectangleF(15, currentY, constants.dailyReportPrintPaperWidth * 3 / 5, 30);
                    StringFormat format1_1 = new StringFormat();
                    format1_1.Alignment = StringAlignment.Near;
                    e.Graphics.DrawString(saleDate.ToString("yyyy-MM-dd HH:mm:ss"), new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect1_1, format1_1);//print each item

                    RectangleF rect1_2 = new RectangleF(constants.dailyReportPrintPaperWidth * 3 / 5 + 15, currentY, constants.dailyReportPrintPaperWidth * 2 / 5 - 15, 30);
                    StringFormat format1_2 = new StringFormat();
                    format1_2.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString("計 " + totalPrice + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect1_2, format1_2);//print each item
                    currentY += 30;

                    SQLiteCommand sqlite_cmds;
                    SQLiteDataReader sqlite_datareaders;

                    sqlite_cmds = sqlite_conn.CreateCommand();
                    string daySumqureys = "SELECT * FROM " + constants.tbNames[3] + " WHERE sumDate=@sumDate AND ticketNo=@ticketNo ORDER BY saleDate DESC";
                    sqlite_cmds.CommandText = daySumqureys;
                    sqlite_cmds.Parameters.AddWithValue("@sumDate", logDate);
                    //sqlite_cmds.Parameters.AddWithValue("@saleDate1", sumDayTime1);
                    //sqlite_cmds.Parameters.AddWithValue("@saleDate2", sumDayTime2);
                    sqlite_cmds.Parameters.AddWithValue("@ticketNo", ticketNo);
                    sqlite_datareaders = sqlite_cmds.ExecuteReader();
                    while (sqlite_datareaders.Read())
                    {
                        if (!sqlite_datareaders.IsDBNull(0))
                        {
                            string prdName = sqlite_datareaders.GetString(3);
                            int prdAmount = sqlite_datareaders.GetInt32(5);
                            int prdPrice = sqlite_datareaders.GetInt32(4);
                            RectangleF rect2 = new RectangleF(15, currentY, constants.logPrintPaperWidth * 3 / 5, 30);
                            StringFormat format2 = new StringFormat();
                            format2.Alignment = StringAlignment.Near;
                            e.Graphics.DrawString(prdName, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//print each item

                            RectangleF rect3 = new RectangleF(constants.logPrintPaperWidth * 3 / 5 + 15, currentY, constants.logPrintPaperWidth / 5 - 15, 30);
                            StringFormat format3 = new StringFormat();
                            format3.Alignment = StringAlignment.Center;
                            e.Graphics.DrawString(prdAmount.ToString() + constants.amountUnit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect3, format3);//print each item

                            RectangleF rect4 = new RectangleF(constants.logPrintPaperWidth * 4 / 5, currentY, constants.logPrintPaperWidth / 5, 30);
                            StringFormat format4 = new StringFormat();
                            format4.Alignment = StringAlignment.Far;
                            e.Graphics.DrawString(prdPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

                            currentY += 20;

                            e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
                            k++;

                        }

                    }
                    PointF p1 = new PointF(constants.logPrintPaperWidth * 1 / 10, currentY);
                    PointF p2 = new PointF(constants.logPrintPaperWidth * 9 / 10, currentY + 1);
                    e.Graphics.DrawLine(new Pen(Color.Black, 1), p1, p2);
                    currentY += 20;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            RectangleF rect8 = new RectangleF(constants.logPrintPaperWidth * 1 / 10, currentY, constants.logPrintPaperWidth * 9 / 10, 30);
            StringFormat format8 = new StringFormat();
            format8.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect8, format8);//print each item
            currentY += 30;
            return;

        }

        public void SetVal(string dropdownHandler, string sendVal)
        {
            logTimeGlobal = sendVal;
            if (dropdownHandler == "logDetailReport")
            {
                dialogPanelGlobal.Controls.Clear();
                this.LogReportBody(logTimeGlobal, dialogPanelGlobal);
            }
        }

        private void SelectTime(object sender, EventArgs e)
        {
            ComboBox tempObj = (ComboBox)sender;
            string objName = tempObj.Name;
            SetVal(objName, (tempObj.SelectedIndex).ToString("00"));
        }

        private void USBPrint(string pageType = "daily")
        {
            try
            {
                int a = UsbOpen("HMK-060");
                int status = NewRealRead();
                constants.SaveLogData("dailyReport_***", "dailyReport print status==>" + status);
                if (a != 0)
                {
                    string audio_file_name = dbClass.AudioFile("ErrorOccur");
                    this.PlaySound(audio_file_name);
                    messageDialog.ShowPrintErrorMessageOther(0);
                }
                else
                {
                    if ((status >= 1 && status <= 15) || status == 32)
                    {
                        int errStatus = status;
                        if (status == 32)
                        {
                            errStatus = 16;
                        }

                        string audio_file_name = dbClass.AudioFile("ErrorOccur");
                        this.PlaySound(audio_file_name);
                        messageDialog.ShowPrintErrorMessageOther(errStatus);
                    }
                    else
                    {
                        if (pageType == "daily")
                        {
                            DailyReportPrintPageUSB();
                        }
                        else if (pageType == "receipt")
                        {
                            ReceiptIssueReportPrintPageUSB();
                        }
                        else if(pageType == "log")
                        {
                            LogPrintUSB();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                constants.SaveLogData("dailyReport_***", e.ToString());
                string audio_file_name = dbClass.AudioFile("ErrorOccur");
                this.PlaySound(audio_file_name);
                messageDialog.ShowPrintErrorMessageOther(0);
            }

        }

        private void DailyReportPrintPageUSB()
        {
            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            int a;
            byte[] bytes;
            a = PrintCmd(0x1B);
            a = PrintStr("@");                      //Reset Printer
            Thread.Sleep(200);

            a = PrintCmd(0x1A);
            a = PrintStr("x");
            a = PrintCmd(0x00);                      //Extended ascii mode 

            a = PrintCmd(0x1B);
            a = PrintStr("j");
            a = PrintCmd(0x70);                      //back feed

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x00);                      //Select Korean font

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x20);                      //Select Japanese font

            a = PrintCmd(0x1B);
            a = PrintStr("3");
            a = PrintCmd(0x28);                      //Set the interval of line

            a = PrintCmd(0x1D);
            a = PrintStr("L");
            a = PrintCmd(0x32);
            a = PrintCmd(0x00);                      //Select the left margin

            a = PrintCmd(0x1D);
            a = PrintStr("W");
            a = PrintCmd(0x5C);
            a = PrintCmd(0x01);                      //Select the printing width

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x01);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes(" " + constants.dailyReportTitle + " " + logDate);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set left align

            int soldPriceSum = 0;
            int soldAmountSum = 0;
            int k = 0;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(2);
                    int prdPrice = sqlite_datareader.GetInt32(3);
                    int prdAmount = sqlite_datareader.GetInt32(4);
                    int prdTotalPrice = sqlite_datareader.GetInt32(5);
                    soldPriceSum += prdTotalPrice;
                    soldAmountSum += prdAmount;

                    bytes = Encoding.GetEncoding(932).GetBytes(prdName + "/" + prdAmount.ToString() + constants.amountUnit + "/" + prdPrice.ToString() + constants.unit + "/" + prdTotalPrice.ToString() + constants.unit);

                    foreach (byte tempB in bytes)
                    {
                        a = PrintCmd(tempB);
                    }

                    a = PrintCmd(0x0A);

                    k++;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x01);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes("合計: " + soldAmountSum + constants.amountUnit + "  " + soldPriceSum + " " + constants.unit);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x02);                      //set right align

            bytes = Encoding.GetEncoding(932).GetBytes(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut
        }

        private void ReceiptIssueReportPrintPageUSB()
        {
            int a;
            byte[] bytes;
            a = PrintCmd(0x1B);
            a = PrintStr("@");                      //Reset Printer
            Thread.Sleep(200);

            DateTime now = DateTime.Now;
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;


            a = PrintCmd(0x1A);
            a = PrintStr("x");
            a = PrintCmd(0x00);                      //Extended ascii mode 

            a = PrintCmd(0x1B);
            a = PrintStr("j");
            a = PrintCmd(0x70);                      //back feed

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x00);                      //Select Korean font

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x20);                      //Select Japanese font

            a = PrintCmd(0x1B);
            a = PrintStr("3");
            a = PrintCmd(0x28);                      //Set the interval of line

            a = PrintCmd(0x1D);
            a = PrintStr("L");
            a = PrintCmd(0x32);
            a = PrintCmd(0x00);                      //Select the left margin

            a = PrintCmd(0x1D);
            a = PrintStr("W");
            a = PrintCmd(0x5C);
            a = PrintCmd(0x01);                      //Select the printing width

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes(" " + constants.receiptionTitle + "  " + logDate);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set left align

            int k = 0;
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[8] + " WHERE sumDate=@sumDate";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@receiptDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    int purchasePoint = sqlite_datareader.GetInt32(1);
                    int totalPrice = sqlite_datareader.GetInt32(2);
                    DateTime receiptDate = sqlite_datareader.GetDateTime(3);

                    bytes = Encoding.GetEncoding(932).GetBytes(receiptDate.ToString("yyyy/MM/dd HH:mm:ss") + "/" + purchasePoint.ToString() + constants.amountUnit + "/" + totalPrice.ToString() + constants.unit);

                    foreach (byte tempB in bytes)
                    {
                        a = PrintCmd(tempB);
                    }

                    a = PrintCmd(0x0A);

                    k++;
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x01);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes("合計: " + k + constants.amountUnit);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set left align

            bytes = Encoding.GetEncoding(932).GetBytes(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut
        }

        private void LogPrintUSB()
        {
            string logYear = logHandlerGlobal.yearComboboxGlobal.GetItemText(logHandlerGlobal.yearComboboxGlobal.SelectedItem);
            string logMonth = logHandlerGlobal.monthComboboxGlobal.GetItemText(logHandlerGlobal.monthComboboxGlobal.SelectedItem);
            string logDay = logHandlerGlobal.dateComboboxGlobal.GetItemText(logHandlerGlobal.dateComboboxGlobal.SelectedItem);
            string logDate = logYear + "-" + logMonth + "-" + logDay;

            DateTime sumDayTime1 = new DateTime(int.Parse(logYear), int.Parse(logMonth), int.Parse(logDay), 00, 00, 00);
            DateTime sumDayTime2 = sumDayTime1.AddDays(1).AddSeconds(-1);

            int a;
            byte[] bytes;
            a = PrintCmd(0x1B);
            a = PrintStr("@");                      //Reset Printer

            Thread.Sleep(200);

            a = PrintCmd(0x1A);
            a = PrintStr("x");
            a = PrintCmd(0x00);                      //Extended ascii mode 

            a = PrintCmd(0x1B);
            a = PrintStr("j");
            a = PrintCmd(0x70);                      //back feed

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x00);                      //Select Korean font

            a = PrintCmd(0x1B);
            a = PrintStr("M");
            a = PrintCmd(0x20);                      //Select Japanese font

            a = PrintCmd(0x1B);
            a = PrintStr("3");
            a = PrintCmd(0x28);                      //Set the interval of line

            a = PrintCmd(0x1D);
            a = PrintStr("L");
            a = PrintCmd(0x32);
            a = PrintCmd(0x00);                      //Select the left margin

            a = PrintCmd(0x1D);
            a = PrintStr("W");
            a = PrintCmd(0x5C);
            a = PrintCmd(0x01);                      //Select the printing width

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes(constants.logReportLabel + " " + logDate);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            int k = 0;
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT saleDate, sum(prdPrice * prdAmount), ticketNo, count(id) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 and saleDate<=@saleDate2 GROUP BY ticketNo ORDER BY saleDate DESC";

            sqlite_cmd.CommandText = daySumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@sumDate", logDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    DateTime saleDate = sqlite_datareader.GetDateTime(0);
                    int totalPrice = sqlite_datareader.GetInt32(1);
                    int ticketNo = sqlite_datareader.GetInt32(2);

                    a = PrintCmd(0x1B);
                    a = PrintStr("a");
                    a = PrintCmd(0x00);                      //set left align

                    bytes = Encoding.GetEncoding(932).GetBytes(saleDate.ToString("yyyy-MM-dd HH:mm:ss"));

                    foreach (byte tempB in bytes)
                    {
                        a = PrintCmd(tempB);
                    }

                    bytes = Encoding.GetEncoding(932).GetBytes("  計 " + totalPrice + constants.unit);

                    foreach (byte tempB in bytes)
                    {
                        a = PrintCmd(tempB);
                    }

                    a = PrintCmd(0x0A);

                    SQLiteCommand sqlite_cmds;
                    SQLiteDataReader sqlite_datareaders;

                    sqlite_cmds = sqlite_conn.CreateCommand();
                    string daySumqureys = "SELECT * FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 AND ticketNo=@ticketNo ORDER BY saleDate DESC";
                    sqlite_cmds.CommandText = daySumqureys;
                    //sqlite_cmds.Parameters.AddWithValue("@sumDate", logDate);
                    sqlite_cmds.Parameters.AddWithValue("@saleDate1", sumDayTime1);
                    sqlite_cmds.Parameters.AddWithValue("@saleDate2", sumDayTime2);
                    sqlite_cmds.Parameters.AddWithValue("@ticketNo", ticketNo);
                    sqlite_datareaders = sqlite_cmds.ExecuteReader();
                    while (sqlite_datareaders.Read())
                    {
                        if (!sqlite_datareaders.IsDBNull(0))
                        {
                            string prdName = sqlite_datareaders.GetString(3);
                            int prdAmount = sqlite_datareaders.GetInt32(5);
                            int prdPrice = sqlite_datareaders.GetInt32(4);

                            bytes = Encoding.GetEncoding(932).GetBytes(prdName + "/" + prdAmount.ToString() + constants.amountUnit + "/" + prdPrice.ToString() + constants.unit);

                            foreach (byte tempB in bytes)
                            {
                                a = PrintCmd(tempB);
                            }

                            a = PrintCmd(0x0A);

                            k++;
                        }
                    }

                    a = PrintCmd(0x1B);
                    a = PrintStr("a");
                    a = PrintCmd(0x01);                      //set left align

                    bytes = Encoding.GetEncoding(932).GetBytes("============================");

                    foreach (byte tempB in bytes)
                    {
                        a = PrintCmd(tempB);
                    }

                    a = PrintCmd(0x0A);
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set left align

            bytes = Encoding.GetEncoding(932).GetBytes(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut
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

        SoundPlayer player = null;
        private void PlaySound(string sfileName)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //path += sfileName;
            string path = sfileName;
            if (!File.Exists(path))
                return;
            player = new SoundPlayer();
            player.SoundLocation = @path;
            player.Play();
        }

    }

}
