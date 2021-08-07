using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class LogManage : Form
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

        Panel bodyUpPanel = null;
        Panel bodyDownPanel = null;

        LogDetailView logDetailView = new LogDetailView();
        MessageDialog messageDialog = new MessageDialog();
        public ComboBox yearComboboxGlobal = null;
        public ComboBox monthComboboxGlobal = null;
        public ComboBox dateComboboxGlobal = null;
        public ComboBox yearComboboxOnlyGlobal = null;
        public ComboBox yearComboboxTwoGlobal = null;
        public ComboBox monthComboboxTwoGlobal = null;
        public ComboBox yearComboboxStartGlobal = null;
        public ComboBox monthComboboxStartGlobal = null;
        public ComboBox dateComboboxStartGlobal = null;
        public ComboBox yearComboboxEndGlobal = null;
        public ComboBox monthComboboxEndGlobal = null;
        public ComboBox dateComboboxEndGlobal = null;
        public RadioButton r1Global = null;
        public RadioButton r2Global = null;
        public RadioButton r3Global = null;

        DateTime now = DateTime.Now;

        string sumDate = DateTime.Now.ToString("yyyy-MM-dd");

        public LogManage(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;
            logDetailView.InitLogManage(this);

            Panel headerPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, 0, mainPanelGlobal_2.Width, 70, BorderStyle.None, Color.Transparent);

            Label headerTitle = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.logManage, 20, 0, headerPanel.Width / 2, headerPanel.Height, Color.FromArgb(255, 0, 95, 163), Color.FromArgb(255, 255, 242, 75), 36);

            string errorLogImage = constants.soldoutButtonImage2;
            Button errorLogBtn = customButton.CreateButtonWithImage(errorLogImage, "closeButton", constants.errorLogShow, headerPanel.Width - 200, 10, 150, 50, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            headerPanel.Controls.Add(errorLogBtn);
            errorLogBtn.Click += new EventHandler(ErrorLogDetail);

            bodyUpPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, headerPanel.Bottom, mainPanelGlobal_2.Width, (mainPanelGlobal_2.Height - 70) * 2 / 5, BorderStyle.None, Color.Transparent);

            bodyDownPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, bodyUpPanel.Bottom, mainPanelGlobal_2.Width, (mainPanelGlobal_2.Height - 70) * 3 / 5, BorderStyle.None, Color.Transparent);

            /** up panel section */
            CreateUpPanel();
            /** down panel section*/
            CreateDownPanel();

            string backImage = constants.soldoutButtonImage1;
            Button backButton = customButton.CreateButtonWithImage(backImage, "closeButton", constants.backText, bodyDownPanel.Width - 200, bodyDownPanel.Height - 70, 150, 50, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyDownPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShow);

            InitializeComponent();
        }

        private void CreateUpPanel()
        {
            Label subTitle1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subTitle1", constants.logDetailView, bodyUpPanel.Width / 15, bodyUpPanel.Height / 15, bodyUpPanel.Width * 13 / 15, bodyUpPanel.Height / 6, Color.FromArgb(255, 209, 211, 212), Color.Black, 30);
            ComboBox yearCombobox1 = createCombobox.CreateComboboxs(bodyUpPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, 100, subTitle1.Bottom + bodyUpPanel.Height * 4 / 15 + 10, bodyUpPanel.Width * 15 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("yyyy"));
            Label yearLabel1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "startLabelYear", constants.yearLabel, yearCombobox1.Right, subTitle1.Bottom + bodyUpPanel.Height / 4, bodyUpPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxGlobal = yearCombobox1;
            yearCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox1 = createCombobox.CreateComboboxs(bodyUpPanel, "monthCombobox1", constants.months1, yearLabel1.Right, subTitle1.Bottom + bodyUpPanel.Height * 4 / 15 + 10, bodyUpPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("MM"));
            Label monthLabel1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "startLabelMonth", constants.monthLabel, monthCombobox1.Right, subTitle1.Bottom + bodyUpPanel.Height / 4, bodyUpPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxGlobal = monthCombobox1;
            monthCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            ComboBox dateCombobox1 = createCombobox.CreateComboboxs(bodyUpPanel, "dateCombobox1", constants.dates1, monthLabel1.Right, subTitle1.Bottom + bodyUpPanel.Height * 4 / 15 + 10, bodyUpPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("dd"));

            //  dropDownDate.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.dates, startLabel.Right + 440, subTitle2.Bottom + 30, 150, 50, 150, 50 * 32, 150, 50, Color.Transparent, Color.Transparent);
            Label dateLabel1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "startLabelDate", constants.dayLabel, dateCombobox1.Right, subTitle1.Bottom + bodyUpPanel.Height / 4, bodyUpPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxGlobal = dateCombobox1;
            dateCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            Label subLabel1 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subLabel1", constants.dailyReportLabel, bodyUpPanel.Width / 2, subTitle1.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7, bodyUpPanel.Height / 6, Color.Transparent, Color.Black, 28);
            Label subLabel2 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subLabel2", constants.receiptButtonText, bodyUpPanel.Width * 9 / 14, subTitle1.Bottom + bodyUpPanel.Height / 20, bodyDownPanel.Width / 7, bodyUpPanel.Height / 6, Color.Transparent, Color.Black, 28);
            Label subLabel3 = createLabel.CreateLabelsInPanel(bodyUpPanel, "subLabel3", constants.logReportLabel, bodyUpPanel.Width * 77 / 98, subTitle1.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7, bodyUpPanel.Height / 6, Color.Transparent, Color.Black, 28);

            Button dailyViewBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[0], "dailyViewBtn", constants.viewLabel, bodyUpPanel.Width / 2, subLabel1.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(dailyViewBtn);
            dailyViewBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button receiptViewBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[0], "receiptViewBtn", constants.viewLabel, bodyUpPanel.Width * 9 / 14, subLabel2.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(receiptViewBtn);
            receiptViewBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button logViewBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[0], "logViewBtn", constants.viewLabel, bodyUpPanel.Width * 77 / 98, subLabel3.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(logViewBtn);
            logViewBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button dailyPrintBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[1], "dailyPrintBtn", constants.printLabel, bodyUpPanel.Width / 2, dailyViewBtn.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(dailyPrintBtn);
            dailyPrintBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button receiptPrintBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[1], "receiptPrintBtn", constants.printLabel, bodyUpPanel.Width * 9 / 14, receiptViewBtn.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(receiptPrintBtn);
            receiptPrintBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button logPrintBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[1], "logPrintBtn", constants.printLabel, bodyUpPanel.Width * 77 / 98, logViewBtn.Bottom + bodyUpPanel.Height / 20, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyUpPanel.Controls.Add(logPrintBtn);
            logPrintBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

        }

        private void CreateDownPanel()
        {
            Label subTitle2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "subTitle2", constants.logSumView, bodyDownPanel.Width / 15, bodyDownPanel.Height / 20, bodyDownPanel.Width * 13 / 15, bodyDownPanel.Height / 7, Color.FromArgb(255, 209, 211, 212), Color.Black, 30);
            RadioButton r1 = new RadioButton();
            r1.Text = constants.yearLabel;
            r1.Location = new Point(100, subTitle2.Bottom + 30);
            r1.Font = new Font("Seri", 28);
            r1.AutoSize = true;
            bodyDownPanel.Controls.Add(r1);
            r1Global = r1;

            ComboBox yearCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, r1.Right + 150, subTitle2.Bottom + 30, bodyUpPanel.Width * 15 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("yyyy"));
            Label yearLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelYear", constants.yearLabel, yearCombobox1.Right, subTitle2.Bottom + 15, bodyUpPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxOnlyGlobal = yearCombobox1;
            yearCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);



            RadioButton r2 = new RadioButton();
            r2.Text = constants.monthLabel;
            r2.Location = new Point(100, r1.Bottom + 30);
            r2.Font = new Font("Seri", 28);
            r2.AutoSize = true;
            bodyDownPanel.Controls.Add(r2);
            r2Global = r2;

            ComboBox yearCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, r2.Right + 150, yearCombobox1.Bottom + 35, bodyUpPanel.Width * 15 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("yyyy"));
            Label yearLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelYear", constants.yearLabel, yearCombobox1.Right, yearLabel1.Bottom + 5, bodyUpPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxTwoGlobal = yearCombobox2;
            yearCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "monthCombobox1", constants.months1, yearLabel1.Right, yearCombobox1.Bottom + 35, bodyUpPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("MM"));
            Label monthLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelMonth", constants.monthLabel, monthCombobox1.Right, yearLabel1.Bottom + 5, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxTwoGlobal = monthCombobox1;
            monthCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);


            RadioButton r3 = new RadioButton();
            r3.Text = constants.dateRangeLabel;
            r3.Location = new Point(100, r2.Bottom + 30);
            r3.Font = new Font("Seri", 28);
            r3.AutoSize = true;
            bodyDownPanel.Controls.Add(r3);
            r3Global = r3;

            Label startLabel = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabel", constants.falsePurchaseStartLabel, r3.Right + 5, r2.Bottom + 18, bodyDownPanel.Width * 2 / 25, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleLeft);

            ComboBox yearCombobox3 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, startLabel.Right + 5, r2.Bottom + 35, bodyDownPanel.Width * 15 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("yyyy"));
            Label yearLabel3 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelYear", constants.yearLabel, yearCombobox3.Right, r2.Bottom + 20, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxStartGlobal = yearCombobox3;
            yearCombobox3.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox3.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "monthCombobox1", constants.months1, yearLabel1.Right, r2.Bottom + 35, bodyDownPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("MM"));
            Label monthLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelMonth", constants.monthLabel, monthCombobox2.Right, r2.Bottom + 20, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxStartGlobal = monthCombobox2;
            monthCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            ComboBox dateCombobox1 = createCombobox.CreateComboboxs(bodyDownPanel, "dateCombobox1", constants.dates1, monthLabel1.Right, r2.Bottom + 35, bodyDownPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("dd"));

            //  dropDownDate.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.dates, startLabel.Right + 440, subTitle2.Bottom + 30, 150, 50, 150, 50 * 32, 150, 50, Color.Transparent, Color.Transparent);
            Label dateLabel1 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelDate", constants.dayLabel, dateCombobox1.Right, r2.Bottom + 20, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxStartGlobal = dateCombobox1;
            dateCombobox1.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox1.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);


            Label endLabel = createLabel.CreateLabelsInPanel(bodyDownPanel, "endLabel", constants.falsePurchaseEndLabel, r3.Right + 5, startLabel.Bottom + 5, bodyDownPanel.Width * 2 / 25, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleLeft);
            ComboBox yearCombobox4 = createCombobox.CreateComboboxs(bodyDownPanel, "yearCombobox1", new string[] { now.ToString("yyyy"), now.AddYears(-1).ToString("yyyy"), now.AddYears(-2).ToString("yyyy") }, endLabel.Right + 5, r3.Bottom + 40, bodyDownPanel.Width * 15 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("yyyy"));
            Label yearLabel4 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelYear", constants.yearLabel, yearCombobox4.Right, r3.Bottom + 25, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            // yearLabel1.Margin = new Padding(5, 0, 0, 0);
            yearComboboxEndGlobal = yearCombobox4;
            yearCombobox4.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            yearCombobox4.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            // dropDownMonth.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.months, startLabel.Right + 230, subTitle2.Bottom + 30, 150, 50, 150, 50 * 13, 150, 50, Color.Transparent, Color.Transparent);
            ComboBox monthCombobox3 = createCombobox.CreateComboboxs(bodyDownPanel, "monthCombobox1", constants.months1, yearLabel1.Right, r3.Bottom + 40, bodyDownPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("MM"));
            Label monthLabel3 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelMonth", constants.monthLabel, monthCombobox3.Right, r3.Bottom + 25, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            monthComboboxEndGlobal = monthCombobox3;
            monthCombobox3.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            monthCombobox3.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            ComboBox dateCombobox2 = createCombobox.CreateComboboxs(bodyDownPanel, "dateCombobox1", constants.dates1, monthLabel1.Right, r3.Bottom + 40, bodyDownPanel.Width * 8 / 125, bodyUpPanel.Height / 10, bodyUpPanel.Height / 10, new Font("Comic Sans", 30), now.ToString("dd"));

            //  dropDownDate.CreateDropDown("falsePurchaseCancellation", bodyDownPanel, constants.dates, startLabel.Right + 440, subTitle2.Bottom + 30, 150, 50, 150, 50 * 32, 150, 50, Color.Transparent, Color.Transparent);
            Label dateLabel2 = createLabel.CreateLabelsInPanel(bodyDownPanel, "startLabelDate", constants.dayLabel, dateCombobox2.Right, r3.Bottom + 25, bodyDownPanel.Width * 5 / 125, bodyUpPanel.Height / 5, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);
            dateComboboxEndGlobal = dateCombobox2;
            dateCombobox2.DrawItem += new DrawItemEventHandler(createCombobox.dateCombobox_DrawItem);
            dateCombobox2.MeasureItem += new MeasureItemEventHandler(createCombobox.dateCombobox_MeasureItem);

            Button allViewBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[0], "allViewBtn", constants.falsePurchaseListLabel, bodyDownPanel.Width * 2 / 3, subTitle2.Bottom + 30, bodyUpPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyDownPanel.Controls.Add(allViewBtn);
            allViewBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);

            Button allSaveBtn = customButton.CreateButtonWithImage(constants.closingProcessButtonImage[2], "allSaveBtn", constants.allSaveLabel, bodyDownPanel.Width * 2 / 3, allViewBtn.Bottom + bodyUpPanel.Height / 20, bodyDownPanel.Width / 7 - 10, bodyUpPanel.Height / 5, 0, 10, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyDownPanel.Controls.Add(allSaveBtn);
            allSaveBtn.Click += new EventHandler(logDetailView.DetailViewIndicator);
        }

        private void ErrorLogDetail(object sender, EventArgs e)
        {
            mainPanelGlobal_2.Controls.Clear();
            ErrorLogDetail errorLogDetail = new ErrorLogDetail(mainFormGlobal, mainPanelGlobal);
            errorLogDetail.TopLevel = false;
            mainPanelGlobal_2.Controls.Add(errorLogDetail);
            errorLogDetail.FormBorderStyle = FormBorderStyle.None;
            errorLogDetail.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            errorLogDetail.Show();
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
            mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
        }



    }
}
