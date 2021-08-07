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
    public partial class FalsePurchaseCancellation : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel mainPanelGlobal_2 = null;
        Form DialogFormGlobal = null;
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

        DetailView detailView = new DetailView();
        MessageDialog messageDialog = new MessageDialog();
        ComboBox yearComboboxStartGlobal = null;
        ComboBox monthComboboxStartGlobal = null;
        ComboBox dateComboboxStartGlobal = null;

        ComboBox yearComboboxEndGlobal = null;
        ComboBox monthComboboxEndGlobal = null;
        ComboBox dateComboboxEndGlobal = null;

        DateTime now = DateTime.Now;

        //string storeEndTime = "00:00";
        string sumDate = DateTime.Now.ToString("yyyy-MM-dd");

        bool manualProcessState = false;
        public FalsePurchaseCancellation(Form1 mainForm, Panel mainPanel)
        {
            InitializeComponent();
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;
            dropDownMenu.InitFalsePurchaseCancellation(this);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            sumDate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                SQLiteCommand sqlite_cmds = sqlite_conn.CreateCommand();
                string sumIdentify = "SELECT COUNT(id) FROM " + constants.tbNames[7] + " WHERE sumDate=@sumDate";
                sqlite_cmds.CommandText = sumIdentify;
                sqlite_cmds.Parameters.AddWithValue("@sumDate", sumDate);
                int rowNumDaySale = Convert.ToInt32(sqlite_cmds.ExecuteScalar());
                if (rowNumDaySale > 0)
                {
                    manualProcessState = true;
                }
                sqlite_cmds.Dispose();
                sqlite_cmds = null;
            }
            catch (Exception e)
            {
                MessageBox.Show("Database Error:" + e);
            }

            sqlite_conn.Dispose();
            sqlite_conn = null;


            string backImage = constants.soldoutButtonImage1;
            
            Panel headerPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, 0, mainPanelGlobal_2.Width, mainPanelGlobal_2.Height / 8, BorderStyle.None, Color.Transparent);

            Label headerTitle = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.falsePurchaseTitle, headerPanel.Width / 15, headerPanel.Height / 5, headerPanel.Width * 13 / 30, headerPanel.Height * 3 / 5, Color.FromArgb(255, 0, 95, 163), Color.FromArgb(255, 255, 242, 75), 36);

            Button backButton = customButton.CreateButtonWithImage(backImage, "closeButton", constants.backText, headerPanel.Width * 33 / 40, headerPanel.Height / 5, headerPanel.Width * 13 / 120, headerPanel.Height * 3 / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            headerPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShow);

            Panel bodyUpPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, headerPanel.Bottom, mainPanelGlobal_2.Width, mainPanelGlobal_2.Height * 3 / 8, BorderStyle.None, Color.Transparent);
            Label subTitle1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subTitle1", constants.falsePurchaseSubTitle1, bodyUpPanel.Width / 15, bodyUpPanel.Height / 15, bodyUpPanel.Width * 13 / 15, bodyUpPanel.Height / 5, Color.FromArgb(255, 209, 211, 212), Color.Black, 32);
            Label subContent1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subContent1", constants.falsePurchaseSubContent1, bodyUpPanel.Width / 15, subTitle1.Bottom, bodyUpPanel.Width * 13 /15, bodyUpPanel.Height / 3, Color.Transparent, Color.Black, 18);
            string cancelButtonImage = constants.cancelButton;
            Button cancellationButton = customButton.CreateButtonWithImage(cancelButtonImage, "cancellationButton", constants.falsePurchaseButton, bodyUpPanel.Width * 16 / 45, subContent1.Bottom + bodyUpPanel.Height / 15, bodyUpPanel.Width * 13 / 45, bodyUpPanel.Height * 4 / 15, 0, 1, 32, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(cancellationButton);

            if (manualProcessState)
            {
                cancellationButton.Click += new EventHandler(messageDialog.CancelOrderMessage);
            }
            else
            {
                cancellationButton.Click += new EventHandler(detailView.DetailViewIndicator);
            }

            Panel bodyDownPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, bodyUpPanel.Bottom, mainPanelGlobal_2.Width, mainPanelGlobal_2.Height / 2, BorderStyle.None, Color.Transparent);
            Label subTitle2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "subTitle1", constants.falsePurchaseSubTitle2, bodyDownPanel.Width / 15, bodyDownPanel.Height / 20, bodyDownPanel.Width * 13 / 15, bodyDownPanel.Height / 6, Color.FromArgb(255, 209, 211, 212), Color.Black, 32);

            Label startLabel = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabel", constants.falsePurchaseStartLabel, bodyDownPanel.Width * 6 / 25, subTitle2.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 3 / 25, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22);

            // dropDownYear.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, new string[] { "2020", "2019", "2018" }, startLabel.Right + 10, subTitle2.Bottom + 30, 150, 50, 150, 200, 150, 50, Color.Transparent, Color.Transparent);
            int currentY = now.Year;

            ComboBox yearCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox1", new string[] { currentY.ToString(), (currentY - 1).ToString(), (currentY - 2).ToString() }, startLabel.Right, subTitle2.Bottom + bodyDownPanel.Height / 10, bodyDownPanel.Width * 15 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("yyyy"));
            Label yearLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelYear", constants.yearLabel, yearCombobox1.Right, subTitle2.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
           // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxStartGlobal = yearCombobox1;
            yearCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "monthCombobox1", constants.months1, yearLabel1.Right, subTitle2.Bottom + bodyDownPanel.Height / 10, bodyDownPanel.Width * 8 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("MM"));
            Label monthLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelMonth", constants.monthLabel, monthCombobox1.Right, subTitle2.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxStartGlobal = monthCombobox1;
            monthCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            ComboBox dateCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "dateCombobox1", constants.dates1, monthLabel1.Right, subTitle2.Bottom + bodyDownPanel.Height / 10, bodyDownPanel.Width * 8 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("dd"));

            //  dropDownDate.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.dates, startLabel.Right + 440, subTitle2.Bottom + 30, 150, 50, 150, 50 * 32, 150, 50, Color.Transparent, Color.Transparent);
            Label dateLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelDate", constants.dayLabel, dateCombobox1.Right, subTitle2.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxStartGlobal = dateCombobox1;
            dateCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            Label endLabel = createLabel.CreateLabelsInPanel(bodyDownPanel, "endLabel", constants.falsePurchaseEndLabel, bodyDownPanel.Width * 6 / 25, startLabel.Bottom, bodyDownPanel.Width * 3 / 25, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22);

            // dropDownYear.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, new string[] { "2020", "2019", "2018" }, startLabel.Right + 10, subTitle2.Bottom + 30, 150, 50, 150, 200, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox yearCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox2", new string[] { currentY.ToString(), (currentY - 1).ToString(), (currentY - 2).ToString() }, endLabel.Right, startLabel.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 15 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("yyyy"));
            Label yearLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "endLabelYear", constants.yearLabel, yearCombobox2.Right, startLabel.Bottom, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            yearComboboxEndGlobal = yearCombobox2;
            yearCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "monthCombobox2", constants.months2, yearLabel2.Right, startLabel.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 8 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("MM"));
            Label monthLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "endLabelMonth", constants.monthLabel, monthCombobox2.Right, startLabel.Bottom, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxEndGlobal = monthCombobox2;
            monthCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            //  dropDownDate.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.dates, startLabel.Right + 440, subTitle2.Bottom + 30, 150, 50, 150, 50 * 32, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox dateCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "dateCombobox2", constants.dates2, monthLabel2.Right, startLabel.Bottom + bodyDownPanel.Height / 20, bodyDownPanel.Width * 8 / 125, bodyDownPanel.Height / 10, bodyDownPanel.Height / 10, new Font("Comic Sans", 32), now.ToString("dd"));
            Label dateLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "endLabelDate", constants.dayLabel, dateCombobox2.Right, startLabel.Bottom, bodyDownPanel.Width * 5 / 125, bodyDownPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxEndGlobal = dateCombobox2;
            dateCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            Button cancellationShowButton = customButton.CreateButtonWithImage(cancelButtonImage, "cancellationShowButton", constants.falsePurchaseListLabel, bodyDownPanel.Width * 2 / 3, endLabel.Bottom, bodyDownPanel.Width * 12 / 45, bodyDownPanel.Height / 6, 0, 1, 32, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            bodyDownPanel.Controls.Add(cancellationShowButton);
            cancellationShowButton.Click += new EventHandler(this.ShowCanceledResult);
        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal_2.Controls.Clear();
            mainPanelGlobal_2.Hide();
            mainFormGlobal.mainPanelGlobal.Show();
            mainFormGlobal.topPanelGlobal.Show();
            mainFormGlobal.bottomPanelGlobal.Show();
            MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainFormGlobal.mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
        }

        public void BackShowParent(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
        }

        private void ShowCanceledResult(object sender, EventArgs e)
        {
            string startYear = yearComboboxStartGlobal.GetItemText(yearComboboxStartGlobal.SelectedItem);
            string startMonth = monthComboboxStartGlobal.GetItemText(monthComboboxStartGlobal.SelectedItem);
            string startDate = dateComboboxStartGlobal.GetItemText(dateComboboxStartGlobal.SelectedItem);
            string endYear = yearComboboxEndGlobal.GetItemText(yearComboboxEndGlobal.SelectedItem);
            string endMonth = monthComboboxEndGlobal.GetItemText(monthComboboxEndGlobal.SelectedItem);
            string endDate = dateComboboxEndGlobal.GetItemText(dateComboboxEndGlobal.SelectedItem);
            DateTime startDay = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDate), 00, 00, 00);
            DateTime endDay = new DateTime(int.Parse(endYear), int.Parse(endMonth), int.Parse(endDate), 23, 59, 59);
            DateTime now = DateTime.Now;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            try
            {
                string showResult = "SELECT count(*) FROM " + constants.tbNames[9] + " WHERE cancelDate>=@startDay AND cancelDate<=@endDay";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = showResult;
                sqlite_cmd.Parameters.AddWithValue("@startDay", startDay.ToString("yyyy-MM-dd"));
                sqlite_cmd.Parameters.AddWithValue("@endDay", endDay.ToString("yyyy-MM-dd"));
                int countRow = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                if(countRow == 0)
                {
                    ErrorAlert();
                }
                else
                {
                    CancelResultDetail();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void CancelResultDetail()
        {
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            string startYear = yearComboboxStartGlobal.GetItemText(yearComboboxStartGlobal.SelectedItem);
            string startMonth = monthComboboxStartGlobal.GetItemText(monthComboboxStartGlobal.SelectedItem);
            string startDate = dateComboboxStartGlobal.GetItemText(dateComboboxStartGlobal.SelectedItem);
            string endYear = yearComboboxEndGlobal.GetItemText(yearComboboxEndGlobal.SelectedItem);
            string endMonth = monthComboboxEndGlobal.GetItemText(monthComboboxEndGlobal.SelectedItem);
            string endDate = dateComboboxEndGlobal.GetItemText(dateComboboxEndGlobal.SelectedItem);
            DateTime startDay = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDate), 00, 00, 00);
            DateTime endDay = new DateTime(int.Parse(endYear), int.Parse(endMonth), int.Parse(endDate), 23, 59, 59);
            DateTime now = DateTime.Now;

            Form dialogForm = new Form();
            dialogForm.Size = new Size(mainFormGlobal.Width * 2 / 3, mainFormGlobal.Height * 2 / 3);
            dialogForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
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

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.falsePurchasePageTitle, 0, 0, dialogPanel.Width / 2 - 30, 60, Color.Transparent, Color.Black, 22);



            FlowLayoutPanel productTableHeader = createPanel.CreateFlowLayoutPanel(dialogPanel, 10, 60, dialogPanel.Width - 20, 60, Color.Gray, new Padding(1));
            Label prodNameHeader1 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_1", constants.orderTimeField, 0, 0, productTableHeader.Width / 4 + 20, productTableHeader.Height - 5, Color.FromArgb(255, 236, 253, 245), Color.FromArgb(255, 142, 133, 118), 12);
            prodNameHeader1.Margin = new Padding(1, 0, 1, 1);
            Label prodNameHeader2 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_2", constants.saleNumberField, productTableHeader.Width / 4 + 20, 0, productTableHeader.Width / 4 - 35, productTableHeader.Height - 5, Color.FromArgb(255, 236, 253, 245), Color.FromArgb(255, 142, 133, 118), 12);
            prodNameHeader2.Margin = new Padding(1, 0, 1, 1);
            Label prodNameHeader3 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_3", constants.prodNameField, productTableHeader.Width / 2 - 15, 0, productTableHeader.Width / 4 + 30, productTableHeader.Height - 5, Color.FromArgb(255, 236, 253, 245), Color.FromArgb(255, 142, 133, 118), 12);
            prodNameHeader3.Margin = new Padding(1, 0, 1, 1);
            Label prodNameHeader4 = createLabel.CreateLabelsInPanel(productTableHeader, "prodHeader_4", constants.priceField, productTableHeader.Width * 3 / 4 - 15, 0, productTableHeader.Width / 4 - 25, productTableHeader.Height - 5, Color.FromArgb(255, 236, 253, 245), Color.FromArgb(255, 142, 133, 118), 12);
            prodNameHeader4.Margin = new Padding(1, 0, 1, 1);

            Panel tbodyPanel = createPanel.CreateSubPanel(dialogPanel, 0, 120, dialogPanel.Width, dialogPanel.Height - 220, BorderStyle.None, Color.Transparent);
            tbodyPanel.HorizontalScroll.Maximum = 0;
            tbodyPanel.AutoScroll = false;
            tbodyPanel.VerticalScroll.Visible = false;
            tbodyPanel.AutoScroll = true;


            SQLiteCommand sqlite_cmd0;
            SQLiteDataReader sqlite_datareader0;

            int k = 0;
            string showResult0 = "SELECT * FROM " + constants.tbNames[9] + " WHERE cancelDate>=@startDay AND cancelDate<=@endDay ORDER BY id, cancelDate";
            sqlite_cmd0 = sqlite_conn.CreateCommand();
            sqlite_cmd0.CommandText = showResult0;
            sqlite_cmd0.Parameters.AddWithValue("@startDay", startDay.ToString("yyyy-MM-dd"));
            sqlite_cmd0.Parameters.AddWithValue("@endDay", endDay.ToString("yyyy-MM-dd"));
            sqlite_datareader0 = sqlite_cmd0.ExecuteReader();
            while (sqlite_datareader0.Read())
            {
                if (!sqlite_datareader0.IsDBNull(0))
                {

                    string prdDate = sqlite_datareader0.GetDateTime(7).ToString("yyyy/MM/dd HH:mm:ss");
                    string prdNumberRow = sqlite_datareader0.GetInt32(6).ToString("0000000000");
                    string prdName = sqlite_datareader0.GetString(3);
                    string prdPrice = (sqlite_datareader0.GetInt32(4) * sqlite_datareader0.GetInt32(5)).ToString();
                    FlowLayoutPanel productTableRow = createPanel.CreateFlowLayoutPanel(tbodyPanel, 10, 60 * k, dialogPanel.Width - 20, 60, Color.Gray, new Padding(1));
                    Label prodDateRow = createLabel.CreateLabelsInPanel(productTableRow, "prodDate", prdDate, 0, 0, productTableRow.Width / 4 + 20, productTableRow.Height - 2, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodDateRow.Margin = new Padding(1, 0, 1, 2);
                    Label prodNumberRow = createLabel.CreateLabelsInPanel(productTableRow, "prodNumber", prdNumberRow, productTableRow.Width / 4 + 20, 0, productTableRow.Width / 4 - 35, productTableRow.Height - 2, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNumberRow.Margin = new Padding(1, 0, 1, 2);
                    Label prodNameRow = createLabel.CreateLabelsInPanel(productTableRow, "prodName", prdName, productTableRow.Width / 2 - 15, 0, productTableRow.Width / 4 + 30, productTableRow.Height - 2, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameRow.Margin = new Padding(1, 0, 1, 2);
                    Label prodPriceRow = createLabel.CreateLabelsInPanel(productTableRow, "prodPrice", prdPrice, productTableRow.Width * 3 / 4 - 15, 0, productTableRow.Width / 4 - 25, productTableRow.Height - 2, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodPriceRow.Margin = new Padding(1, 0, 1, 2);
                    k++;
                }
            }

            sqlite_datareader0.Close();
            sqlite_datareader0 = null;
            sqlite_cmd0.Dispose();
            sqlite_cmd0 = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

            Button backButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "backButton", constants.backText, dialogPanel.Width - 150, dialogPanel.Height - 100, 100, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);

            dialogPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShowParent);

            dialogForm.ShowDialog();

        }

        private void ErrorAlert()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(mainFormGlobal.Width / 3, mainFormGlobal.Height / 4);
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

            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label dialogTitle1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle1", constants.cancelResultErrorMessage, dialogPanel.Width / 10, 0, dialogPanel.Width * 4 / 5, dialogPanel.Height * 2 / 3, Color.Transparent, Color.Black, 22);

            Button backButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "backButton", constants.backText, dialogPanel.Width / 2 - 75, dialogPanel.Height * 2 / 3, 150, 40, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);

            dialogPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShowParent);

            dialogForm.ShowDialog();
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
