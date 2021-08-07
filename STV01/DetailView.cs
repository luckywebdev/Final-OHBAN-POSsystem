using Microsoft.Win32;
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
    public partial class DetailView : Form
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
        Panel tbodyPanelGlobal = null;
        Form cancelDialogFormGlobal = null;
        FlowLayoutPanel dialogFlowLayout = null;
        Label currentHourGlobal = null;
        Label currentMinuteGlobal = null;
        Label currentHourGlobal2 = null;
        Label currentMinuteGlobal2 = null;
        Label[] columnGlobal1 = null;
        Label[] columnGlobal2 = null;
        Label[] columnGlobal3 = null;
        Label[] columnGlobal4 = null;
        int totalRowNum = 0;
        PaperSize paperSize = new PaperSize("papersize", 500, 800);//set the paper size
        string logTimeGlobal = "23";
        int[] productTypes = null;
        DateTime now = DateTime.Now;

        string sumDate = DateTime.Now.ToString("yyyy-MM-dd");

        //TimeSetting timeHandlerGlobal = null;
        AutoRestartSetting AutoHandlerGlobal = null;
        ProductInfoSetting productInfoSettingGlobal = null;

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


        public DetailView()
        {
            InitializeComponent();
        }
        public void InitAutoRestartSetting(AutoRestartSetting timeHandler)
        {
            AutoHandlerGlobal = timeHandler;
        }
        public void InitSaleTimeSetting(ProductInfoSetting timeHandler)
        {
            productInfoSettingGlobal = timeHandler;
        }

        public void DetailViewIndicator(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            if (btnTemp.Name == "processButton_1_1")
            {
                DailyReport();
            }
            else if (btnTemp.Name == "processButton_1_2")
            {
                USBPrint("daily");
                //PrintDocument printDocument1 = new PrintDocument();
                //PrintDialog printDialog1 = new PrintDialog();
                //PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
                //printDialogGlobal = printDialog1;
                //printDocumentGlobal = printDocument1;
                //printPreviewDialogGlobal = printPreviewDialog1;
                //paperSize = new PaperSize("papersize", constants.dailyReportPrintPaperWidth, constants.dailyReportPrintPaperHeight);

                //printDocument1.PrintPage += new PrintPageEventHandler(DailyReportPrintPage);
                //printDialog1.Document = printDocument1;
                ////printDocument1.DefaultPageSettings.PaperSize = paperSize;
                //printDocument1.Print();

            }
            else if (btnTemp.Name == "processButton_2_1")
            {
                ReceiptIssueReport();
            }
            else if (btnTemp.Name == "processButton_2_2")
            {
                USBPrint("receipt");
                //PrintDocument printDocument1 = new PrintDocument();
                //PrintDialog printDialog1 = new PrintDialog();
                //PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
                //printDialogGlobal = printDialog1;
                //printDocumentGlobal = printDocument1;
                //printPreviewDialogGlobal = printPreviewDialog1;
                //printDocument1.PrintPage += new PrintPageEventHandler(ReceiptIssueReportPrintPage);
                //printDialog1.Document = printDocument1;
                //paperSize = new PaperSize("papersize", constants.receiptReportPrintPaperWidth, constants.receiptReportPrintPaperHeight);
                ////printDocument1.DefaultPageSettings.PaperSize = paperSize;
                //printDocument1.Print();
            }
            else if (btnTemp.Name == "processButton_3_1")
            {
                LogReport();
            }
            else if(btnTemp.Name == "cancellationButton")
            {
                FalsePurchaseCancellation();
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
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;

            DialogFormGlobal = dialogForm;

            Panel dialogTitlePanel = createPanel.CreateMainPanel(dialogForm, 0, 30, dialogForm.Width, 50, BorderStyle.None, Color.Transparent);
            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle1", constants.dailyReportTitle, 0, 0, dialogTitlePanel.Width / 2 - 30, 60, Color.Transparent, Color.Black, 22);
            Label dialogTitle2 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle2", sumDate, dialogTitlePanel.Width / 2, 0, dialogTitlePanel.Width / 2 - 30, 60, Color.Transparent, Color.Black, 22);

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 80, dialogForm.Width - 30, dialogForm.Height - 180, BorderStyle.None, Color.Transparent);
            dialogPanel.Margin = new Padding(0, 0, 0, 20);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;

            DateTime now = DateTime.Now;


            int soldPriceSum = 0;
            int soldAmountSum = 0;
            int k = 0;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            sumDate = DateTime.Now.ToString("yyyy-MM-dd");

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT * FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate ORDER BY sumDate DESC";
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
                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, 41 * k, dialogPanel.Width, 40, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", prdName, 0, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", prdAmount.ToString() + constants.amountUnit, productTableBody.Width / 4, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3", prdPrice.ToString() + constants.unit, productTableBody.Width / 2, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody4 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_4", prdTotalPrice.ToString() + constants.unit, productTableBody.Width * 3 / 4, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    dialogFlowLayout = productTableBody;
                    k++;

                }
            }

            FlowLayoutPanel dialogSumPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, k * 41 + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(0));
            Label dialogSumLabel1 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel1", "合計", 0, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);
            Label dialogSumLabel2 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", soldAmountSum + constants.amountUnit, dialogSumPanel.Width / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);
            Label dialogSumLabel3 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", soldPriceSum + constants.unit, dialogSumPanel.Width * 2 / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 16);

            FlowLayoutPanel dialogDatePanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, dialogSumPanel.Bottom + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(0));
            Label dialogDateLabel = createLabel.CreateLabelsInPanel(dialogDatePanel, "dialogDateLabel", now.ToLocalTime().ToString(), 0, 0, dialogDatePanel.Width, 50, Color.Transparent, Color.Black, 16, false, ContentAlignment.TopLeft);
            dialogDateLabel.Padding = new Padding(50, 0, 0, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 90, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
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

        public void ReceiptIssueReport()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 2, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            DateTime now = DateTime.Now;

            Panel dialogTitlePanel = createPanel.CreateMainPanel(dialogForm, 0, 30, dialogForm.Width, 50, BorderStyle.None, Color.Transparent);
            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle1", constants.receiptionTitle, 0, 0, dialogTitlePanel.Width / 2 - 30, 50, Color.Transparent, Color.Black, 22);
            Label dialogTitle2 = createLabel.CreateLabelsInPanel(dialogTitlePanel, "dialogTitle2", now.ToString("yyyy/MM/dd"), dialogTitlePanel.Width / 2, 0, dialogTitlePanel.Width / 2 - 30, 50, Color.Transparent, Color.Black, 22);

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 90, dialogForm.Width - 30, dialogForm.Height - 190, BorderStyle.None, Color.Transparent);
            dialogPanel.HorizontalScroll.Maximum = 0;
            dialogPanel.AutoScroll = false;
            dialogPanel.VerticalScroll.Visible = false;
            dialogPanel.AutoScroll = true;


            int k = 0;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
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
            sqlite_cmd.Parameters.AddWithValue("@sumDate", sumDate);
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

                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, 5 + 41 * k, dialogPanel.Width, 40, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", receiptDate.ToString("yyyy/MM/dd"), 0, 0, productTableBody.Width / 3, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", purchasePoint.ToString() + constants.amountUnit1, productTableBody.Width / 3, 0, productTableBody.Width / 3 - 50, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3", totalPrice.ToString() + constants.unit, productTableBody.Width * 2 / 3 - 50, 0, productTableBody.Width / 3 - 50, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    dialogFlowLayout = productTableBody;
                    k++;
                }
            }

            FlowLayoutPanel dialogSumPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, k * 41 + 15, dialogPanel.Width, 50, Color.Transparent, new Padding(10));
            Label dialogSumLabel1 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel1", "合計", 0, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 14);
            Label dialogSumLabel2 = createLabel.CreateLabelsInPanel(dialogSumPanel, "dialogSumLabel2", k + constants.amountUnit, dialogSumPanel.Width / 3, 0, dialogSumPanel.Width / 3 - 20, 50, Color.Transparent, Color.Black, 14);

            FlowLayoutPanel dialogDatePanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, dialogSumPanel.Bottom + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(10));
           // dialogDatePanel.Padding = new Padding(dialogDatePanel.Width * 2 / 3, 0, 0, 0);
            Label dialogDateLabel = createLabel.CreateLabelsInPanel(dialogDatePanel, "dialogDateLabel", now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), 0, 0, dialogDatePanel.Width, 50, Color.Transparent, Color.Black, 14, false, ContentAlignment.TopLeft);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, dialogForm.Width - 150, dialogForm.Height - 90, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
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

        public void LogReport()
        {
            dropDownMenu.InitLogReport(this);
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 3, height * 2 / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height - 100, BorderStyle.None, Color.Transparent);

            DateTime now = DateTime.Now;

            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.logReportLabel + " " + now.ToLocalTime().ToString("yyyy/MM/dd"), 0, 30, dialogPanel.Width * 2 / 3 - 50, 60, Color.Transparent, Color.Black, 20);

            ComboBox dropdownTime = createCombobox.CreateComboboxs(dialogPanel, "logReport", constants.times, dialogPanel.Width * 2 / 3 - 50, 40, 100, 40, 40, new Font("Comic Sans", 24), now.ToString("HH"));
            dropdownTime.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dropdownTime.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            dropdownTime.SelectedIndexChanged += new EventHandler(SelectTime);


            Label titleTimeLabel = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.timeRangeLabel, dropdownTime.Right + 5, 30, 100, 60, Color.Transparent, Color.Black, 20);

            Panel dialogPanelBody = createPanel.CreateSubPanel(dialogPanel, 0, 80, dialogForm.Width - 30, dialogForm.Height - 200, BorderStyle.None, Color.Transparent);
            dialogPanelBody.Padding = new Padding(0, 5, 0, 10);
            dialogPanelBody.HorizontalScroll.Maximum = 0;
            dialogPanelBody.AutoScroll = false;
            dialogPanelBody.VerticalScroll.Visible = false;
            dialogPanelBody.AutoScroll = true;

            dialogPanelGlobal = dialogPanelBody;
            //ComboBox timeCombobox = createCombobox.CreateComboboxs(dialogTitlePanel, "timeCombobox", constants.times, dialogTitlePanel.Width * 2 / 3, 0, 100, 40, 25, new Font("Comic Sans", 18));
            LogReportBody(now.AddHours(1).ToString("HH"), dialogPanelBody);

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
            DateTime sumDayTime2 = DateTime.Now;
            DateTime sumDayTime1 = DateTime.Now;
            if(int.Parse(logReportTime) == 0)
            {
                sumDayTime2 = constants.CurrentDateTimeFromTime("23:59");
                sumDayTime1 = constants.CurrentDateTimeFromTime("00:00");
            }
            else
            {
                sumDayTime2 = constants.CurrentDateTimeFromTime(logReportTime + ":00");
                sumDayTime1 = constants.CurrentDateTimeFromTime((int.Parse(logReportTime) - 1).ToString("00") + ":00");
            }

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
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

        public void FalsePurchaseCancellation()
        {
            DateTime now = DateTime.Now;
            dropDownMenu.InitLogReport(this);
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 2, height / 2);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.BackColor = Color.White;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            //dialogForm.radiusValue = 50;
            //dialogForm.borderColor = Color.White;
            //dialogForm.borderSize = 2;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.AutoScroll = false;
            DialogFormGlobal = dialogForm;
            using(Bitmap img = new Bitmap(constants.roundedFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;


            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 10, dialogForm.Width, dialogForm.Height - 80, BorderStyle.None, Color.Transparent);

            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.falsePurchasePageTitle, 5, 0, dialogPanel.Width / 2, 60, Color.Transparent, Color.Black, 18);
            ComboBox dropdownTime = createCombobox.CreateComboboxs(dialogPanel, "falsePurchase", constants.times, dialogPanel.Width * 2 / 3 - 50, 10, 100, 40, 40, new Font("Comic Sans", 24), now.ToString("HH"));
            dropdownTime.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dropdownTime.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);
            dropdownTime.SelectedIndexChanged += new EventHandler(SelectTime);
            Label titleTimeLabel = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.timeRangeLabel, dropdownTime.Right + 5, 0, 100, 60, Color.Transparent, Color.Black, 20);



            FlowLayoutPanel productTableHeader = createPanel.CreateFlowLayoutPanel(dialogPanel, 30, 60, dialogPanel.Width - 60, 60, Color.FromArgb(255, 209, 211, 212), new Padding(0));
            Label prodNameHeader1 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_1", constants.orderTimeField, 0, 0, productTableHeader.Width / 4 + 20, productTableHeader.Height, Color.Transparent, Color.Black, 12);
            Label prodNameHeader2 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_2", constants.saleNumberField, productTableHeader.Width / 4 + 25, 0, productTableHeader.Width / 4 - 35, productTableHeader.Height, Color.Transparent, Color.Black, 12);
            Label prodNameHeader3 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_3", constants.prodNameField, productTableHeader.Width / 2, 0, productTableHeader.Width / 4 - 10, productTableHeader.Height, Color.Transparent, Color.Black, 12);
            Label prodNameHeader4 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_4", constants.priceField, productTableHeader.Width * 3 / 4, 0, productTableHeader.Width / 4 - 20, productTableHeader.Height, Color.Transparent, Color.Black, 12);
            Panel tbodyPanel = createPanel.CreateSubPanel(dialogPanel, 0, 120, dialogPanel.Width - 30, dialogPanel.Height - 120, BorderStyle.None, Color.Transparent);
            tbodyPanelGlobal = tbodyPanel;
            tbodyPanel.HorizontalScroll.Maximum = 0;
            tbodyPanel.AutoScroll = false;
            tbodyPanel.VerticalScroll.Visible = false;
            tbodyPanel.AutoScroll = true;
            dialogPanelGlobal = dialogPanel;

            ShowProdListForFalsePurchaseCancell(now.AddHours(1).ToString("HH"), tbodyPanel);

            string backImage = constants.soldoutButtonImage1;

            Button backButton = createButton.CreateButtonWithImage(backImage, "backButton", constants.backText, dialogForm.Right - 130, dialogPanel.Bottom + 10, 100, 40, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            dialogForm.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.CloseDialog);

            dialogForm.ShowDialog();

        }

        DateTime sumDayTimeForCancel1 = DateTime.Now;
        DateTime sumDayTimeForCancel2 = DateTime.Now;

        private void ShowProdListForFalsePurchaseCancell(string logTime, Panel mainPanel)
        {
            DateTime now = DateTime.Now;

            int k = 0;

            //sumDayTime1 = constants.sumDayTimeStart(storeEndTime);
            if(int.Parse(logTime) == 0)
            {
                sumDayTimeForCancel1 = constants.CurrentDateTimeFromTime("00:00");
                sumDayTimeForCancel2 = constants.CurrentDateTimeFromTime("23:59");
            }
            else
            {
                sumDayTimeForCancel1 = constants.CurrentDateTimeFromTime((int.Parse(logTime) - 1).ToString("00") + ":00");
                sumDayTimeForCancel2 = constants.CurrentDateTimeFromTime(logTime + ":00");
            }

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd0;
            SQLiteDataReader sqlite_datareader0;
            sqlite_cmd0 = sqlite_conn.CreateCommand();
            string daySumqurey0 = "SELECT count(DISTINCT ticketNo) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 and saleDate<=@saleDate2 ORDER BY saleDate DESC";
            sqlite_cmd0.CommandText = daySumqurey0;
            sqlite_cmd0.Parameters.AddWithValue("@saleDate1", sumDayTimeForCancel1);
            sqlite_cmd0.Parameters.AddWithValue("@saleDate2", sumDayTimeForCancel2);
            sqlite_datareader0 = sqlite_cmd0.ExecuteReader();
            while (sqlite_datareader0.Read())
            {
                if (!sqlite_datareader0.IsDBNull(0))
                {
                    totalRowNum = sqlite_datareader0.GetInt32(0);
                }
            }

            sqlite_cmd0.Dispose();
            sqlite_cmd0 = null;
            sqlite_datareader0.Close();
            sqlite_datareader0 = null;

            columnGlobal1 = new Label[totalRowNum];
            columnGlobal2 = new Label[totalRowNum];
            columnGlobal3 = new Label[totalRowNum];
            columnGlobal4 = new Label[totalRowNum];
            productTypes = new int[totalRowNum];

            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT saleDate, sum(prdPrice * prdAmount), ticketNo, count(id) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 and saleDate<=@saleDate2 GROUP BY ticketNo ORDER BY saleDate DESC";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTimeForCancel1);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTimeForCancel2);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            int addHeight = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    DateTime saleDate = sqlite_datareader.GetDateTime(0);
                    int totalPrice = sqlite_datareader.GetInt32(1);
                    int ticketNo = sqlite_datareader.GetInt32(2);
                    int countRow = sqlite_datareader.GetInt32(3);
                    productTypes[k] = countRow;
                    int PanelRowHeight = 60;
                    if (k != 0)
                    {
                        PictureBox pB = new PictureBox();
                        pB.Location = new Point(60, addHeight);
                        pB.Size = new Size(mainPanel.Width - 120, 10);
                        mainPanel.Controls.Add(pB);
                        Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
                        Graphics g = Graphics.FromImage(image);
                        g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3) { DashPattern = new float[] { 5, 1.5F } }, 5, 5, mainPanel.Width - 160, 5);
                        pB.Image = image;
                        addHeight += 10;

                    }
                    mainPanel.Padding = new Padding(0);
                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(mainPanel, 30, addHeight, mainPanel.Width - 15, PanelRowHeight, Color.Transparent, new Padding(0));
                    addHeight += 60;
                    productTableBody.Name = "prdID_" + k;
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1_" + ticketNo, saleDate.ToString("yyyy/MM/dd HH:mm:ss"), 0, 0, productTableBody.Width / 4 + 20, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody1.Margin = new Padding(0);
                    columnGlobal1[k] = prodNameBody1;

                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2_" + ticketNo, ticketNo.ToString("0000000000"), productTableBody.Width / 4 + 25, 0, productTableBody.Width / 4 - 35, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody2.Margin = new Padding(0);
                    columnGlobal2[k] = prodNameBody2;
                    prodNameBody1.Click += new EventHandler(this.CancellationSetting);
                    prodNameBody2.Click += new EventHandler(this.CancellationSetting);

                    int m = 0;
                    string prdName = "";
                    SQLiteCommand sqlite_cmds;
                    SQLiteDataReader sqlite_datareaders;

                    sqlite_cmds = sqlite_conn.CreateCommand();
                    string daySumqureys = "SELECT * FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 AND ticketNo=@ticketNo ORDER BY id ASC";
                    sqlite_cmds.CommandText = daySumqureys;
                    sqlite_cmds.Parameters.AddWithValue("@saleDate1", sumDayTimeForCancel1);
                    sqlite_cmds.Parameters.AddWithValue("@saleDate2", sumDayTimeForCancel2);
                    sqlite_cmds.Parameters.AddWithValue("@ticketNo", ticketNo);
                    sqlite_datareaders = sqlite_cmds.ExecuteReader();
                    while (sqlite_datareaders.Read())
                    {
                        if (!sqlite_datareaders.IsDBNull(0))
                        {
                            if (countRow >= 2 && m < 2)
                            {
                                if (m == 0)
                                {
                                    prdName += sqlite_datareaders.GetString(3) + "\n";
                                }
                                else
                                {
                                    prdName += sqlite_datareaders.GetString(3);
                                }

                            }
                            else if(countRow == 1)
                            {
                                prdName += sqlite_datareaders.GetString(3);
                            }
                            m++;
                        }
                    }

                    sqlite_datareaders.Close();
                    sqlite_datareaders = null;
                    sqlite_cmds.Dispose();
                    sqlite_cmds = null;

                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3_" + ticketNo, prdName, productTableBody.Width / 2, 0, productTableBody.Width / 4 - 10, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody3.Margin = new Padding(0);
                    columnGlobal3[k] = prodNameBody3;

                    prodNameBody3.Click += new EventHandler(this.CancellationSetting);


                    Label prodNameBody4 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_4_" + ticketNo, totalPrice.ToString() + constants.unit, productTableBody.Width * 3 / 4, 0, productTableBody.Width / 4 - 20, productTableBody.Height, Color.Transparent, Color.FromArgb(255, 142, 133, 118), 12);
                    dialogFlowLayout = productTableBody;
                    prodNameBody4.Margin = new Padding(0);
                    columnGlobal4[k] = prodNameBody4;

                    prodNameBody4.Click += new EventHandler(this.CancellationSetting);


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

        string canceledPrdName = "";
        string saleDate = "";
        string saleTime = "";
        int saleSum = 0;

        private void CancellationSetting(object sender, EventArgs e)
        {

            Label prdTemp = (Label)sender;
            int prdID = int.Parse(prdTemp.Name.Split('_')[1]);
            int ticketNo = int.Parse(prdTemp.Name.Split('_')[3]);
            columnGlobal1[prdID].BackColor = Color.Red;
            columnGlobal1[prdID].ForeColor = Color.White;
            columnGlobal2[prdID].BackColor = Color.Red;
            columnGlobal2[prdID].ForeColor = Color.White;
            columnGlobal3[prdID].BackColor = Color.Red;
            columnGlobal3[prdID].ForeColor = Color.White;
            columnGlobal4[prdID].BackColor = Color.Red;
            columnGlobal4[prdID].ForeColor = Color.White;
            for (int k = 0; k < totalRowNum; k++)
            {
                if (k != prdID)
                {
                    columnGlobal1[k].BackColor = Color.Transparent;
                    columnGlobal1[k].ForeColor = Color.FromArgb(255, 142, 133, 118);
                    columnGlobal2[k].BackColor = Color.Transparent;
                    columnGlobal2[k].ForeColor = Color.FromArgb(255, 142, 133, 118);
                    columnGlobal3[k].BackColor = Color.Transparent;
                    columnGlobal3[k].ForeColor = Color.FromArgb(255, 142, 133, 118);
                    columnGlobal4[k].BackColor = Color.Transparent;
                    columnGlobal4[k].ForeColor = Color.FromArgb(255, 142, 133, 118);
                }
            }

            //int productTypes = 3;
            int frmheight = (7 + productTypes[prdID]) * 50;
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 3, frmheight);
            dialogForm.BackColor = Color.FromArgb(255, 242, 240, 223);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            //DialogFormGlobal = dialogForm;

            Panel titlePanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height / 10, BorderStyle.None, Color.Transparent);
            Label titleLabel = createLabel.CreateLabelsInPanel(titlePanel, "titleLabel", constants.orderCancelDialogTitle, 0, 0, titlePanel.Width, titlePanel.Height, Color.Transparent, Color.Black, 22, false, ContentAlignment.BottomCenter);

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, dialogForm.Width / 11, dialogForm.Height / 10, dialogForm.Width * 9 / 11, dialogForm.Height * 7 / 10, BorderStyle.FixedSingle, Color.Transparent);

            FlowLayoutPanel leftColumn = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, 0, dialogPanel.Width * 2/ 5, dialogPanel.Height, Color.Transparent, new Padding(0));
            FlowLayoutPanel rightColumn = createPanel.CreateFlowLayoutPanel(dialogPanel, dialogPanel.Width * 2 / 5, 0, dialogPanel.Width * 3 / 5, dialogPanel.Height, Color.Transparent, new Padding(0));

            int rowCount = 4 + productTypes[prdID];

            Label column1 = createLabel.CreateLabels(leftColumn, "orderDateColumn", constants.orderDate, 0, 0, leftColumn.Width, leftColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
            column1.Padding = new Padding(20, 0, 0, 0);
            Label column2 = createLabel.CreateLabels(leftColumn, "orderTimeColumn", constants.orderTime, 0, column1.Bottom, leftColumn.Width, leftColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
            column2.Padding = new Padding(20, 0, 0, 0);
            Label column3 = createLabel.CreateLabels(leftColumn, "orderNumberColumn", constants.saleNumberField, 0, column2.Bottom, leftColumn.Width, leftColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
            column3.Padding = new Padding(20, 0, 0, 0);
            Label column4 = createLabel.CreateLabels(leftColumn, "orderNameColumn", constants.orderProductList, 0, column3.Bottom, leftColumn.Width, leftColumn.Height * productTypes[prdID] / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
            column4.Padding = new Padding(20, 0, 0, 0);
            Label column5 = createLabel.CreateLabels(leftColumn, "orderPriceColumn", constants.orderSumPrice, 0, column4.Bottom, leftColumn.Width, leftColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
            column5.Padding = new Padding(20, 0, 0, 0);

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "SELECT sum(prdPrice * prdAmount), saleDate FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 AND ticketNo=@ticketNo ORDER BY id ASC";
            sqlite_cmd.CommandText = daySumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTimeForCancel1);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTimeForCancel2);
            sqlite_cmd.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    saleDate = sqlite_datareader.GetDateTime(1).ToString("yyyy/MM/dd");
                    saleTime = sqlite_datareader.GetDateTime(1).ToString("HH:mm:ss");
                    saleSum = sqlite_datareader.GetInt32(0);
                }
            }
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;

            Label value1 = createLabel.CreateLabels(rightColumn, "orderDateValue", saleDate, 0, 0, rightColumn.Width, rightColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleRight, new Padding(0), 1, Color.Gray);
            value1.Padding = new Padding(0, 0, 20, 0);
            Label value2 = createLabel.CreateLabels(rightColumn, "orderTimeValue", saleTime, 0, column1.Bottom, rightColumn.Width, rightColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleRight, new Padding(0), 1, Color.Gray);
            value2.Padding = new Padding(0, 0, 20, 0);
            Label value3 = createLabel.CreateLabels(rightColumn, "orderNumberValue", ticketNo.ToString("0000000000"), 0, column2.Bottom, rightColumn.Width, rightColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleRight, new Padding(0), 2, Color.Gray);
            value3.Padding = new Padding(0, 0, 20, 0);
            int i = 0;
            SQLiteCommand sqlite_cmds;
            SQLiteDataReader sqlite_datareaders;
            sqlite_cmds = sqlite_conn.CreateCommand();
            string daySumqureys = "SELECT * FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 AND ticketNo=@ticketNo ORDER BY id ASC";
            sqlite_cmds.CommandText = daySumqureys;
            sqlite_cmds.Parameters.AddWithValue("@saleDate1", sumDayTimeForCancel1);
            sqlite_cmds.Parameters.AddWithValue("@saleDate2", sumDayTimeForCancel2);
            sqlite_cmds.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_datareaders = sqlite_cmds.ExecuteReader();
            while (sqlite_datareaders.Read())
            {
                if (!sqlite_datareaders.IsDBNull(0))
                {
                    string prdName = sqlite_datareaders.GetString(3);
                    if (i == 0)
                    {
                        canceledPrdName += prdName;
                    }
                    else
                    {
                        canceledPrdName += "," + prdName;
                    }
                    int prdAmount = sqlite_datareaders.GetInt32(5);
                    Label values = createLabel.CreateLabels(rightColumn, "orderNameValue_" + i, prdName + " ×" + prdAmount, 0, column3.Bottom + rightColumn.Height * i / rowCount, rightColumn.Width, rightColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleRight, new Padding(0), 1, Color.Gray);
                    values.Padding = new Padding(0, 0, 20, 0);
                    i++;
                }
            }
            sqlite_cmds.Dispose();
            sqlite_cmds = null;
            sqlite_datareaders.Close();
            sqlite_datareaders = null;

            Label value = createLabel.CreateLabels(rightColumn, "orderPriceValue", saleSum.ToString(), 0, value3.Bottom + rightColumn.Height * productTypes[prdID] / rowCount, rightColumn.Width, rightColumn.Height / rowCount, Color.White, Color.Black, 12, true, ContentAlignment.MiddleRight, new Padding(0), 1, Color.Gray);
            value.Padding = new Padding(0, 0, 20, 0);

            string closeButtonImage = constants.rectBlueButton;
            string cancelButtonImage = constants.rectRedButton;

            Button closeButton = createButton.CreateButtonWithImage(closeButtonImage, "closeButton", constants.cancelLabel, dialogPanel.Left, dialogPanel.Bottom + 20, dialogPanel.Width / 2 - 30, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            closeButton.Click += new EventHandler(this.CancelCloseDialog);

            Button cancelButton = createButton.CreateButtonWithImage(cancelButtonImage, "cancelButton_" + ticketNo, constants.cancelRun, dialogPanel.Right - dialogPanel.Width / 2 + 30, dialogPanel.Bottom + 20, dialogPanel.Width / 2 - 30, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            cancelDialogFormGlobal = dialogForm;
            cancelButton.Click += new EventHandler(this.CancelRun);
            dialogForm.Controls.Add(closeButton);
            dialogForm.Controls.Add(cancelButton);

            sqlite_conn.Dispose();
            sqlite_conn = null;

            dialogForm.ShowDialog();
        }

        public void AutoRestartSet(object sender, EventArgs e)
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 3, height / 2);
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

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", constants.timeSettingTitle, 30, 50, dialogPanel.Width - 30, 35, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);

            int currentHourValue = now.Hour;
            int currentMinuteValue = now.Minute;
            DateTime restartTime = now;
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if(key != null && key.GetValue("restartTime") != null)
            {
                restartTime  = Convert.ToDateTime(key.GetValue("restartTime"));
                currentHourValue = restartTime.Hour;
                currentMinuteValue = restartTime.Minute;
            }

            Panel houurPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width / 5, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextHour = createButton.CreateButtonWithImage(constants.upBtn, "nextHour", "", 10, 30, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextHour.FlatAppearance.BorderColor = Color.White;
            nextHour.FlatAppearance.BorderSize = 0;
            houurPanel.Controls.Add(nextHour);
            nextHour.Click += new EventHandler(this.HourMinuteSelect);

            Label currentHour = createLabel.CreateLabelsInPanel(houurPanel, "currentHour", currentHourValue.ToString("00"), 0, houurPanel.Width, houurPanel.Width, houurPanel.Height - 2 * houurPanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentHourGlobal = currentHour;

            Button prevHour = createButton.CreateButtonWithImage(constants.downBtn, "prevHour", "", 10, currentHour.Bottom, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            houurPanel.Controls.Add(prevHour);
            prevHour.FlatAppearance.BorderColor = Color.White;
            prevHour.FlatAppearance.BorderSize = 0;
            prevHour.Click += new EventHandler(this.HourMinuteSelect);


            Panel MinutePanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width * 3 / 5 + 10, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);
           
            Button nextMinute = createButton.CreateButtonWithImage(constants.upBtn, "nextMinute", "", 10, 30, MinutePanel.Width -20, MinutePanel.Width -30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextMinute.FlatAppearance.BorderColor = Color.White;
            nextMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(nextMinute);
            nextMinute.Click += new EventHandler(this.HourMinuteSelect);

            Label currentMinute = createLabel.CreateLabelsInPanel(MinutePanel, "currentMinute", currentMinuteValue.ToString("00"), 0, MinutePanel.Width, MinutePanel.Width, MinutePanel.Height - 2 * MinutePanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentMinuteGlobal = currentMinute;

            Button prevMinute = createButton.CreateButtonWithImage(constants.downBtn, "prevMinute", "", 10, currentMinute.Bottom, MinutePanel.Width - 20, MinutePanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            prevMinute.BackgroundImageLayout = ImageLayout.Stretch;
            prevMinute.FlatAppearance.BorderColor = Color.White;
            prevMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(prevMinute);
            prevMinute.Click += new EventHandler(this.HourMinuteSelect);

            string backImage = constants.soldoutButtonImage1;

            Button setButton = createButton.CreateButtonWithImage(backImage, "setAutoButton", "OK", dialogPanel.Width - 110, dialogPanel.Height - 70, 80, 40, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            dialogPanel.Controls.Add(setButton);

            setButton.Click += new EventHandler(this.SetDate);

            dialogForm.ShowDialog();
        }

        public void SaleTimeSet(string startH, string startM, string endH, string endM)
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 2, height / 2);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            DateTime now = DateTime.Now;

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", constants.timeSettingTitle, 30, 50, dialogPanel.Width - 30, 35, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);

            Panel houurPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width / 8, 110, dialogPanel.Width / 6 - 20, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextHour = createButton.CreateButtonWithImage(constants.upBtn, "nextHour", "", 10, 30, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextHour.FlatAppearance.BorderColor = Color.White;
            nextHour.FlatAppearance.BorderSize = 0;
            houurPanel.Controls.Add(nextHour);
            nextHour.Click += new EventHandler(this.HourMinuteSelect);

            Label currentHour = createLabel.CreateLabelsInPanel(houurPanel, "currentHour", startH, 0, houurPanel.Width, houurPanel.Width, houurPanel.Height - 2 * houurPanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentHourGlobal = currentHour;

            Button prevHour = createButton.CreateButtonWithImage(constants.downBtn, "prevHour", "", 10, currentHour.Bottom, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            houurPanel.Controls.Add(prevHour);
            prevHour.FlatAppearance.BorderColor = Color.White;
            prevHour.FlatAppearance.BorderSize = 0;
            prevHour.Click += new EventHandler(this.HourMinuteSelect);


            Panel MinutePanel = createPanel.CreateSubPanel(dialogPanel, houurPanel.Right + 20, 110, dialogPanel.Width / 6 - 20, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextMinute = createButton.CreateButtonWithImage(constants.upBtn, "nextMinute", "", 10, 30, MinutePanel.Width - 20, MinutePanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextMinute.FlatAppearance.BorderColor = Color.White;
            nextMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(nextMinute);
            nextMinute.Click += new EventHandler(this.HourMinuteSelect);

            Label currentMinute = createLabel.CreateLabelsInPanel(MinutePanel, "currentMinute", startM, 0, MinutePanel.Width, MinutePanel.Width, MinutePanel.Height - 2 * MinutePanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentMinuteGlobal = currentMinute;

            Button prevMinute = createButton.CreateButtonWithImage(constants.downBtn, "prevMinute", "", 10, currentMinute.Bottom, MinutePanel.Width - 20, MinutePanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            prevMinute.BackgroundImageLayout = ImageLayout.Stretch;
            prevMinute.FlatAppearance.BorderColor = Color.White;
            prevMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(prevMinute);
            prevMinute.Click += new EventHandler(this.HourMinuteSelect);

            Label interLb = createLabel.CreateLabelsInPanel(dialogPanel, "interLb", ":", dialogPanel.Width / 2 - 10, 110, 20, dialogPanel.Height * 4 / 5 - 120, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            interLb.Padding = new Padding(0, 0, 0, 20);

            Panel houurPanel2 = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width / 2 + 30, 110, dialogPanel.Width / 6 - 20, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextHour2 = createButton.CreateButtonWithImage(constants.upBtn, "nextHour2", "", 10, 30, houurPanel2.Width - 20, houurPanel2.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextHour2.FlatAppearance.BorderColor = Color.White;
            nextHour2.FlatAppearance.BorderSize = 0;
            houurPanel2.Controls.Add(nextHour2);
            nextHour2.Click += new EventHandler(this.HourMinuteSelect);

            Label currentHour2 = createLabel.CreateLabelsInPanel(houurPanel2, "currentHour2", endH, 0, houurPanel2.Width, houurPanel2.Width, houurPanel2.Height - 2 * houurPanel2.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentHourGlobal2 = currentHour2;

            Button prevHour2 = createButton.CreateButtonWithImage(constants.downBtn, "prevHour2", "", 10, currentHour2.Bottom, houurPanel2.Width - 20, houurPanel2.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            houurPanel2.Controls.Add(prevHour2);
            prevHour2.FlatAppearance.BorderColor = Color.White;
            prevHour2.FlatAppearance.BorderSize = 0;
            prevHour2.Click += new EventHandler(this.HourMinuteSelect);


            Panel MinutePanel2 = createPanel.CreateSubPanel(dialogPanel, houurPanel2.Right + 20, 110, dialogPanel.Width / 6 - 20, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextMinute2 = createButton.CreateButtonWithImage(constants.upBtn, "nextMinute2", "", 10, 30, MinutePanel2.Width - 20, MinutePanel2.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextMinute2.FlatAppearance.BorderColor = Color.White;
            nextMinute2.FlatAppearance.BorderSize = 0;
            MinutePanel2.Controls.Add(nextMinute2);
            nextMinute2.Click += new EventHandler(this.HourMinuteSelect);

            Label currentMinute2 = createLabel.CreateLabelsInPanel(MinutePanel2, "currentMinute2", endM, 0, MinutePanel2.Width, MinutePanel2.Width, MinutePanel2.Height - 2 * MinutePanel2.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentMinuteGlobal2 = currentMinute2;

            Button prevMinute2 = createButton.CreateButtonWithImage(constants.downBtn, "prevMinute2", "", 10, currentMinute2.Bottom, MinutePanel2.Width - 20, MinutePanel2.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            prevMinute2.BackgroundImageLayout = ImageLayout.Stretch;
            prevMinute2.FlatAppearance.BorderColor = Color.White;
            prevMinute2.FlatAppearance.BorderSize = 0;
            MinutePanel2.Controls.Add(prevMinute2);
            prevMinute2.Click += new EventHandler(this.HourMinuteSelect);

            string backImage = constants.soldoutButtonImage1;

            Button setButton = createButton.CreateButtonWithImage(backImage, "setSaleButton", "OK", dialogPanel.Width - 110, dialogPanel.Height - 70, 80, 40, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            dialogPanel.Controls.Add(setButton);

            setButton.Click += new EventHandler(this.SetDate);

            dialogForm.ShowDialog();
        }


        public void CloseDialog(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
        }
        public void CancelCloseDialog(object sender, EventArgs e)
        {
            cancelDialogFormGlobal.Close();
        }

        public void CancelRun(object sender, EventArgs e)
        {
            Button prdTemp = (Button)sender;
            int ticketNo = int.Parse(prdTemp.Name.Split('_')[1]);

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey = "INSERT INTO " + constants.tbNames[9] + "(saleID, prdID, realPrdID, prdName, prdPrice, prdAmount, ticketNo, saleDate, sumFlag, sumDate, categoryID, serialNo, limitFlag) SELECT * FROM " + constants.tbNames[3] + " WHERE sumFlag='false' AND ticketNo=@ticketNo";
            sqlite_cmd.CommandText = daySumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_cmd.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd = null;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqurey1 = "UPDATE " + constants.tbNames[9] + " SET cancelDate=@cancelDate WHERE cancelDate isNULL AND ticketNo=@ticketNo";
            sqlite_cmd.CommandText = daySumqurey1;
            sqlite_cmd.Parameters.AddWithValue("@cancelDate", now.ToString("yyyy-MM-dd"));
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_cmd.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd = null;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqureys = "DELETE FROM " + constants.tbNames[3] + " WHERE sumFlag='false' AND ticketNo=@ticketNo";
            sqlite_cmd.CommandText = daySumqureys;
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_cmd.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd = null;

            sqlite_cmd = sqlite_conn.CreateCommand();
            string daySumqureyr = "DELETE FROM " + constants.tbNames[8] + " WHERE (sumDate='' OR sumDate='') AND ticketNo=@ticketNo";
            sqlite_cmd.CommandText = daySumqureyr;
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", sumDayTime1);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", sumDayTime2);
            sqlite_cmd.Parameters.AddWithValue("@ticketNo", ticketNo);
            sqlite_cmd.ExecuteNonQuery();

            dbClass.InsertLog(5, " 誤購入取消" + saleDate + " " + saleTime, saleSum + "/" + ticketNo, canceledPrdName);

            cancelDialogFormGlobal.Close();
            tbodyPanelGlobal.Controls.Clear();
            ShowProdListForFalsePurchaseCancell(logTimeGlobal, tbodyPanelGlobal);

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }
        public void HourMinuteSelect(object sender, EventArgs e)
        {
            Button lTemp = (Button)sender;
            int endValue = 59;
            int currentValue = int.Parse(currentMinuteGlobal.Text);

            if (lTemp.Name == "nextHour")
            {
                endValue = 23;
                currentValue = int.Parse(currentHourGlobal.Text);
                if (currentValue >= endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if(lTemp.Name == "prevHour")
            {
                currentValue = int.Parse(currentHourGlobal.Text);
                if (currentValue == 0)
                {
                    currentValue = 23;
                }
                else
                {
                    currentValue--;
                }
            }
            else if (lTemp.Name == "nextMinute")
            {
                endValue = 59;
                if (currentValue == endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if (lTemp.Name == "prevMinute")
            {
                if (currentValue == 0)
                {
                    currentValue = 59;
                }
                else
                {
                    currentValue--;
                }
            }
            else if (lTemp.Name == "nextHour2")
            {
                endValue = 23;
                currentValue = int.Parse(currentHourGlobal2.Text);
                if (currentValue >= endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if (lTemp.Name == "prevHour2")
            {
                currentValue = int.Parse(currentHourGlobal2.Text);
                if (currentValue == 0)
                {
                    currentValue = 23;
                }
                else
                {
                    currentValue--;
                }
            }
            else if (lTemp.Name == "nextMinute2")
            {
                currentValue = int.Parse(currentMinuteGlobal2.Text);
                endValue = 59;
                if (currentValue == endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if (lTemp.Name == "prevMinute2")
            {
                currentValue = int.Parse(currentMinuteGlobal2.Text);
                if (currentValue == 0)
                {
                    currentValue = 59;
                }
                else
                {
                    currentValue--;
                }
            }


            if (lTemp.Name == "nextHour" || lTemp.Name == "prevHour")
            {
                currentHourGlobal.Text = this.DateTimeFormat(currentValue);
            }
            else if (lTemp.Name == "nextMinute" || lTemp.Name == "prevMinute")
            {
                currentMinuteGlobal.Text = this.DateTimeFormat(currentValue);
            }
            else if (lTemp.Name == "nextHour2" || lTemp.Name == "prevHour2")
            {
                currentHourGlobal2.Text = this.DateTimeFormat(currentValue);
            }
            else if (lTemp.Name == "nextMinute2" || lTemp.Name == "prevMinute2")
            {
                currentMinuteGlobal2.Text = this.DateTimeFormat(currentValue);
            }

        }

        private string DateTimeFormat(int val)
        {
            if(val < 10)
            {
                return "0" + val.ToString();
            }
            return val.ToString();
        }

        public void SetDate(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
            Button btnTemp = (Button)sender;
            if (btnTemp.Name == "setAutoButton")
            {
                string currentHour = currentHourGlobal.Text;
                string currentMinute = currentMinuteGlobal.Text;
                AutoHandlerGlobal.SetVal("setTime", currentHour + "_" + currentMinute);
            }
            if (btnTemp.Name == "setSaleButton")
            {
                string currentHour = currentHourGlobal.Text;
                string currentMinute = currentMinuteGlobal.Text;
                string currentHour2 = currentHourGlobal2.Text;
                string currentMinute2 = currentMinuteGlobal2.Text;
                productInfoSettingGlobal.SetTimeValue(currentHour + "_" + currentMinute + "_" + currentHour2 + "_" + currentMinute2);
            }
        }

        private void DailyReportPrintPage(object sender, PrintPageEventArgs e)
        {
            //DateTime now = DateTime.Now;
            //float currentY = 40;// declare  one variable for height measurement
            //RectangleF rect1 = new RectangleF(5, currentY, constants.dailyReportPrintPaperWidth - 5, 30);
            //StringFormat format1 = new StringFormat();
            //format1.Alignment = StringAlignment.Center;
            //e.Graphics.DrawString(constants.dailyReportTitle + " " + sumDate, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            //currentY += 40;
            //int soldPriceSum = 0;
            //int soldAmountSum = 0;
            //int k = 0;

            //SQLiteConnection sqlite_conn;
            //sqlite_conn = CreateConnection(constants.dbName);
            //if (sqlite_conn.State == ConnectionState.Closed)
            //{
            //    sqlite_conn.Open();
            //}
            //SQLiteCommand sqlite_cmd;
            //SQLiteDataReader sqlite_datareader;

            //string week = now.ToString("ddd");
            //string currentTime = now.ToString("HH:mm");

            //sqlite_cmd = sqlite_conn.CreateCommand();
            //string daySumqurey = "SELECT * FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate";
            //sqlite_cmd.CommandText = daySumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@sumDate", sumDate);
            //sqlite_datareader = sqlite_cmd.ExecuteReader();
            //while (sqlite_datareader.Read())
            //{
            //    if (!sqlite_datareader.IsDBNull(0))
            //    {
            //        string prdName = sqlite_datareader.GetString(2);
            //        int prdPrice = sqlite_datareader.GetInt32(3);
            //        int prdAmount = sqlite_datareader.GetInt32(4);
            //        int prdTotalPrice = sqlite_datareader.GetInt32(5);
            //        soldPriceSum += prdTotalPrice;
            //        soldAmountSum += prdAmount;

            //        RectangleF rect2 = new RectangleF(5, currentY, constants.dailyReportPrintPaperWidth * 2 / 5, 30);
            //        StringFormat format2 = new StringFormat();
            //        format2.Alignment = StringAlignment.Near;
            //        e.Graphics.DrawString(prdName, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//print each item

            //        RectangleF rect3 = new RectangleF(constants.dailyReportPrintPaperWidth * 2 / 5 + 5, currentY, constants.dailyReportPrintPaperWidth / 5 - 5, 30);
            //        StringFormat format3 = new StringFormat();
            //        format3.Alignment = StringAlignment.Center;
            //        e.Graphics.DrawString(prdAmount.ToString() + constants.amountUnit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect3, format3);//print each item

            //        RectangleF rect4 = new RectangleF(constants.dailyReportPrintPaperWidth * 3 / 5, currentY, constants.singleticketPrintPaperWidth / 5, 30);
            //        StringFormat format4 = new StringFormat();
            //        format4.Alignment = StringAlignment.Center;
            //        e.Graphics.DrawString(prdPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

            //        RectangleF rect5 = new RectangleF(constants.dailyReportPrintPaperWidth * 4 / 5, currentY, constants.dailyReportPrintPaperWidth / 5, 30);
            //        StringFormat format5 = new StringFormat();
            //        format5.Alignment = StringAlignment.Far;
            //        e.Graphics.DrawString(prdTotalPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect5, format5);//print each item
            //        currentY += 30;

            //        //if (itemperpages < 21) // check whether  the number of item(per page) is more than 20 or not
            //        //{
            //        //    itemperpages += 1; // increment itemperpage by 1
            //        e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
            //        //}

            //        //else // if the number of item(per page) is more than 20 then add one page
            //        //{
            //        //    itemperpages = 0; //initiate itemperpage to 0 .
            //        //    e.HasMorePages = true; //e.HasMorePages raised the PrintPage event once per page .
            //        //    lineNums = k + 1;
            //        //    return;//It will call PrintPage event again

            //        //}
            //        k++;

            //    }
            //}
            //RectangleF rect6 = new RectangleF(0, currentY, constants.dailyReportPrintPaperWidth * 3 / 7, 30);
            //StringFormat format6 = new StringFormat();
            //format6.Alignment = StringAlignment.Far;
            //e.Graphics.DrawString("合計: " + soldAmountSum + constants.amountUnit, new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect6, format6);//print each item

            //RectangleF rect7 = new RectangleF(constants.dailyReportPrintPaperWidth * 4 / 7, currentY, constants.dailyReportPrintPaperWidth * 3 / 7, 30);
            //StringFormat format7 = new StringFormat();
            //format7.Alignment = StringAlignment.Near;
            //e.Graphics.DrawString(soldPriceSum + " " + constants.unit, new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect7, format7);//print each item
            //currentY += 30;

            //RectangleF rect8 = new RectangleF(constants.dailyReportPrintPaperWidth * 1 / 10, currentY, constants.dailyReportPrintPaperWidth * 9 / 10, 30);
            //StringFormat format8 = new StringFormat();
            //format8.Alignment = StringAlignment.Near;
            //e.Graphics.DrawString(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Regular), Brushes.Black, rect8, format8);//print each item
            //currentY += 30;

            //sqlite_datareader.Close();
            //sqlite_datareader = null;
            //sqlite_cmd.Dispose();
            //sqlite_cmd = null;
            //sqlite_conn.Dispose();
            //sqlite_conn = null;
            //return;
            USBPrint("daily");
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
                        if(pageType == "daily")
                        {
                            DailyReportPrintPageUSB();
                        }
                        else if(pageType == "receipt")
                        {
                            ReceiptIssueReportPrintPageUSB();
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

            bytes = Encoding.GetEncoding(932).GetBytes(" " + constants.dailyReportTitle + " " + sumDate);

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

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
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

            a = PrintCmd(0x1B);
            a = PrintStr("J");
            a = PrintCmd(0x08);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut
        }

        private void ReceiptIssueReportPrintPageUSB()
        {
            DateTime now = DateTime.Now;
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

            bytes = Encoding.GetEncoding(932).GetBytes(" " + constants.receiptionTitle + "  " + sumDate);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x00);                      //set center align

            int k = 0;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
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
            sqlite_cmd.Parameters.AddWithValue("@sumDate", sumDate);
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
            a = PrintCmd(0x00);                      //set right align

            bytes = Encoding.GetEncoding(932).GetBytes(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x1B);
            a = PrintStr("J");
            a = PrintCmd(0x08);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut
        }

        private void ReceiptIssueReportPrintPage(object sender, PrintPageEventArgs e)
        {
            //DateTime now = DateTime.Now;
            //float currentY = 0;// declare  one variable for height measurement
            //RectangleF rect1 = new RectangleF(0, currentY, constants.receiptReportPrintPaperWidth, 30);
            //StringFormat format1 = new StringFormat();
            //format1.Alignment = StringAlignment.Center;
            //e.Graphics.DrawString(constants.receiptionTitle + "  " + sumDate, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            //currentY += 30;

            //RectangleF rect2 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth * 3 / 5, 25);
            //StringFormat format2 = new StringFormat();
            //format2.Alignment = StringAlignment.Near;
            //e.Graphics.DrawString(constants.receiptionField, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect2, format2);//this will print one heading/title in every page of the document
            //RectangleF rect3 = new RectangleF(5 + constants.receiptReportPrintPaperWidth * 4 / 5, currentY, constants.receiptReportPrintPaperWidth * 1 / 5, 25);
            //StringFormat format3 = new StringFormat();
            //format3.Alignment = StringAlignment.Far;
            //e.Graphics.DrawString(constants.priceField, new Font("Seri", constants.fontSizeSmall, FontStyle.Regular), Brushes.Black, rect3, format3);//this will print one heading/title in
            //currentY += 25;
            //int k = 0;
            //SQLiteConnection sqlite_conn;
            //sqlite_conn = CreateConnection(constants.dbName);
            //if (sqlite_conn.State == ConnectionState.Closed)
            //{
            //    sqlite_conn.Open();
            //}
            //SQLiteCommand sqlite_cmd;
            //SQLiteDataReader sqlite_datareader;

            //string week = now.ToString("ddd");
            //string currentTime = now.ToString("HH:mm");

            //sqlite_cmd = sqlite_conn.CreateCommand();
            //string daySumqurey = "SELECT * FROM " + constants.tbNames[8] + " WHERE sumDate=@sumDate";
            //sqlite_cmd.CommandText = daySumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@sumDate", sumDate);
            ////sqlite_cmd.Parameters.AddWithValue("@receiptDate1", sumDayTime1);
            ////sqlite_cmd.Parameters.AddWithValue("@receiptDate2", sumDayTime2);
            //sqlite_datareader = sqlite_cmd.ExecuteReader();
            //while (sqlite_datareader.Read())
            //{
            //    if (!sqlite_datareader.IsDBNull(0))
            //    {
            //        int purchasePoint = sqlite_datareader.GetInt32(1);
            //        int totalPrice = sqlite_datareader.GetInt32(2);
            //        DateTime receiptDate = sqlite_datareader.GetDateTime(3);

            //        RectangleF rect4 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth * 3 / 5, 20);
            //        StringFormat format4 = new StringFormat();
            //        format4.Alignment = StringAlignment.Near;
            //        e.Graphics.DrawString(receiptDate.ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect4, format4);//print each item

            //        RectangleF rect5 = new RectangleF(5 + constants.receiptReportPrintPaperWidth * 3 / 5, currentY, constants.receiptReportPrintPaperWidth / 5 - 5, 20);
            //        StringFormat format5 = new StringFormat();
            //        format5.Alignment = StringAlignment.Center;
            //        e.Graphics.DrawString(purchasePoint.ToString() + constants.amountUnit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect5, format5);//print each item

            //        RectangleF rect6 = new RectangleF(constants.receiptReportPrintPaperWidth * 4 / 5, currentY, constants.receiptReportPrintPaperWidth / 5, 20);
            //        StringFormat format6 = new StringFormat();
            //        format6.Alignment = StringAlignment.Far;
            //        e.Graphics.DrawString(totalPrice.ToString() + constants.unit, new Font("Seri", constants.fontSizeSmaller, FontStyle.Regular), Brushes.Black, rect6, format6);//print each item
            //        currentY += 20;
            //        e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added

            //        k++;
            //    }
            //}

            //RectangleF rect7 = new RectangleF(0, currentY, constants.receiptReportPrintPaperWidth, 30);
            //StringFormat format7 = new StringFormat();
            //format7.Alignment = StringAlignment.Center;
            //e.Graphics.DrawString("合計: " + k + constants.amountUnit, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect7, format7);//print each item
            //currentY += 30;

            //RectangleF rect8 = new RectangleF(5, currentY, constants.receiptReportPrintPaperWidth - 5, 30);
            //StringFormat format8 = new StringFormat();
            //format8.Alignment = StringAlignment.Near;
            //e.Graphics.DrawString(now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect8, format8);//print each item
            //currentY += 30;
            //sqlite_datareader.Close();
            //sqlite_datareader = null;
            //sqlite_cmd.Dispose();
            //sqlite_cmd = null;
            //sqlite_conn.Dispose();
            //sqlite_conn = null;

            //return;
            USBPrint("receipt");
        }


        public void SetVal(string dropdownHandler, string sendVal)
        {
            logTimeGlobal = sendVal;
            if(dropdownHandler == "falsePurchase")
            {
                tbodyPanelGlobal.Controls.Clear();
                ShowProdListForFalsePurchaseCancell(logTimeGlobal, tbodyPanelGlobal);
            }
            else if(dropdownHandler == "logReport")
            {
                dialogPanelGlobal.Controls.Clear();
                this.LogReportBody(logTimeGlobal, dialogPanelGlobal);
            }
        }

        private void SelectTime(object sender, EventArgs e)
        {
            ComboBox tempObj = (ComboBox)sender;
            string objName = tempObj.Name;
            switch (objName)
            {
                case "logReport":
                    SetVal("logReport", (tempObj.SelectedIndex).ToString("00"));
                    break;
                case "falsePurchase":
                    SetVal("falsePurchase", (tempObj.SelectedIndex).ToString("00"));
                    break;
            }
        }

        static SQLiteConnection CreateConnection(string dbName)
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            //string dbPath = Path.Combine(Directory.GetCurrentDirectory(), dbName + ".db");
            string dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dbFolder += "\\STV01\\";
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            string dbPath = Path.Combine(dbFolder, dbName + ".db");

            sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; PRAGMA journal_mode = WAL; Version = 3; New = True; Compress = True; Connection Timeout=0");

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
