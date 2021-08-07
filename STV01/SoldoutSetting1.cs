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
    public partial class SoldoutSetting1 : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        DBClass dbClass = new DBClass();
        int selectedCategoryIndex = 0;
        List<int> soldoutCategoryIndex = new List<int>();
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CustomButton customButton = new CustomButton();
        CreateLabel createLabel = new CreateLabel();
        DropDownMenu dropDownMenu = new DropDownMenu();
        NumberInput numberInput = new NumberInput();
        Panel productListPanelGlobal = null;
        Label labelForLimitAmount = null;
        Button saleStateButtonGlobal = null;
        Button backButtonGlobal = null;
        private Panel bodyPanelGlobal = null;
        private bool saleStateGlobal = true;

        string[] categoryNameList = null;
        int[] categoryIDList = null;
        int[] categoryDisplayPositionList = null;
        int[] categorySoldStateList = null;
        List<int> stopedPrdIDArray = null;
        int categoryRowCount = 0;

        public SoldoutSetting1(Form1 mainForm, Panel mainPanel)
        {
            this.GetCategoryList();
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            Button backButton = customButton.CreateButtonWithImage(constants.soldoutButtonImage1, "backButton", constants.backText, mainForm.Right - 200, mainForm.Bottom - 130, 100, 50, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainForm.Controls.Add(backButton);
            backButton.Click += new EventHandler(this.BackShow);
            backButtonGlobal = backButton;

            mainPanel.Hide();
            Panel mainPanels = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height, BorderStyle.None, Color.Transparent);
            dropDownMenu.InitSoldoutSetting(this);
            numberInput.InitSoldoutSetting(this);
            Panel headerPanel = createPanel.CreateSubPanel(mainPanels, 0, 0, mainPanels.Width, mainPanels.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Panel bodyPanel = createPanel.CreateSubPanel(mainPanels, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);

            Label titleLabel = createLabel.CreateLabelsInPanel(headerPanel, "SoldoutSetting_Title", constants.soldoutSettingTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Label subTitleLabel = createLabel.CreateLabelsInPanel(bodyPanel, "SoldoutSetting_subTitle", constants.categorylistLabel, 0, 0, bodyPanel.Width / 4, bodyPanel.Height / 9, Color.Transparent, Color.Red, 24, false, ContentAlignment.MiddleRight);


            dropDownMenu.CreateCategoryDropDown("soldoutSetting1", bodyPanel, categoryNameList, categoryIDList, categoryDisplayPositionList, categorySoldStateList, bodyPanel.Width / 3, bodyPanel.Height / 18 - 25, bodyPanel.Width / 3, 50, bodyPanel.Width / 3, 50 * (categoryRowCount + 1), bodyPanel.Width / 3, 50, Color.Red, Color.White);

            string saleStateButtonImage = null;
            string saleStatusTxt = constants.saleStatusLabel;
            int soldoutState = soldoutCategoryIndex.Find(elem => elem == 1);

            if (soldoutState != 0)
            {
                saleStateButtonImage = constants.soldoutButtonImage2;
                saleStatusTxt = constants.saleStopLabel;
                saleStateGlobal = false;
            }
            else
            {
                saleStateButtonImage = constants.soldoutButtonImage1;
                saleStatusTxt = constants.saleStatusLabel;
                saleStateGlobal = true;
            }


            Button saleStateButton = customButton.CreateButtonWithImage(saleStateButtonImage, "saleSateButton", saleStatusTxt, bodyPanel.Width * 2 / 3 + 20, bodyPanel.Height / 18 - 25, bodyPanel.Width / 6, 50, 1, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

            saleStateButton.Click += new EventHandler(this.SaleSateSwitching);
            saleStateButtonGlobal = saleStateButton;

            bodyPanel.Controls.Add(saleStateButton);
            bodyPanelGlobal = bodyPanel;


            FlowLayoutPanel productTableHeader = createPanel.CreateFlowLayoutPanel(bodyPanelGlobal, bodyPanel.Width / 10, bodyPanel.Height / 9, bodyPanel.Width * 4 / 5, 60, Color.Transparent, new Padding(0));
            Label prodNameHeader1 = createLabel.CreateLabelsInPanel(productTableHeader, "prodNameHeader1", constants.prdNameField, 0, 0, productTableHeader.Width * 2 / 5, productTableHeader.Height, Color.FromArgb(255, 238, 175, 175), Color.Black, 12);
            prodNameHeader1.Margin = new Padding(0);
            prodNameHeader1.BorderStyle = BorderStyle.FixedSingle;

            Label prodNameHeader2 = createLabel.CreateLabelsInPanel(productTableHeader, "prodNameHeader2", constants.salePriceField, productTableHeader.Width * 2 / 5, 0, productTableHeader.Width / 5, productTableHeader.Height, Color.FromArgb(255, 238, 175, 175), Color.Black, 12);
            prodNameHeader2.Margin = new Padding(0);
            prodNameHeader2.BorderStyle = BorderStyle.FixedSingle;

            Label prodNameHeader3 = createLabel.CreateLabelsInPanel(productTableHeader, "prodNameHeader3", constants.saleLimitField, productTableHeader.Width * 3 / 5, 0, productTableHeader.Width / 5, productTableHeader.Height, Color.FromArgb(255, 238, 175, 175), Color.Black, 12);
            prodNameHeader3.Margin = new Padding(0);
            prodNameHeader3.BorderStyle = BorderStyle.FixedSingle;

            Label prodNameHeader4 = createLabel.CreateLabelsInPanel(productTableHeader, "prodNameHeader4", constants.saleStateSettingField, productTableHeader.Width * 4 / 5, 0, productTableHeader.Width / 6, productTableHeader.Height, Color.FromArgb(255, 238, 175, 175), Color.Black, 12);
            prodNameHeader4.Margin = new Padding(productTableHeader.Width / 5 - productTableHeader.Width / 6, 0, 0, 0);
            prodNameHeader4.BorderStyle = BorderStyle.FixedSingle;

            Panel productListPanel = createPanel.CreateSubPanel(bodyPanelGlobal, bodyPanel.Width / 10, bodyPanel.Height / 9 + 60, bodyPanel.Width * 4 / 5, bodyPanel.Height * 8 / 9 - 61, BorderStyle.None, Color.Transparent);
            productListPanel.AutoScroll = true;
            productListPanelGlobal = productListPanel;
            if (soldoutState == 0 && categorySoldStateList[selectedCategoryIndex] == 0)
            {
                CreateProductTable();
            }
            mainPanel.Show();
            //InitializeComponent();
        }

        private void CreateProductTable()
        {
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string queryCmd0 = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@categoryID ORDER BY CardNumber";
            sqlite_cmd.CommandText = queryCmd0;
            sqlite_cmd.Parameters.AddWithValue("@categoryID", categoryIDList[selectedCategoryIndex]);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            stopedPrdIDArray = new List<int>();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(3);
                    int prdPrice = sqlite_datareader.GetInt32(8);
                    int prdLimitedCnt = sqlite_datareader.GetInt32(9);
                    int prdID = sqlite_datareader.GetInt32(2);
                    int rowID = sqlite_datareader.GetInt32(0);
                    int soldFlag = sqlite_datareader.GetInt32(17);
                    int restAmount = prdLimitedCnt;
                    SQLiteDataReader sqlite_datareader1;
                    SQLiteCommand sqlite_cmd1;
                    sqlite_cmd1 = sqlite_conn.CreateCommand();
                    string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and sumFlag='false' and limitFlag=0";
                    sqlite_cmd1.CommandText = queryCmd1;
                    sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDList[selectedCategoryIndex]);
                    sqlite_cmd1.Parameters.AddWithValue("@prdID", prdID);
                    sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                    while (sqlite_datareader1.Read())
                    {
                        if (!sqlite_datareader1.IsDBNull(0) && prdLimitedCnt != 0 && prdLimitedCnt >= sqlite_datareader1.GetInt32(0))
                        {
                            restAmount = prdLimitedCnt - sqlite_datareader1.GetInt32(0);
                        }
                    }
                    sqlite_datareader1.Close();
                    sqlite_datareader1 = null;
                    sqlite_cmd1.Dispose();
                    sqlite_cmd1 = null;

                    FlowLayoutPanel productTableBody = createPanel.CreateFlowLayoutPanel(productListPanelGlobal, 0, 60 * k, productListPanelGlobal.Width, 60, Color.Transparent, new Padding(0));
                    Label prodNameBody1 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_1", prdName, 0, 0, productTableBody.Width * 2 / 5, productTableBody.Height, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody1.Margin = new Padding(0);
                    prodNameBody1.BorderStyle = BorderStyle.FixedSingle;
                    Label prodNameBody2 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_2", prdPrice.ToString(), productTableBody.Width * 2 / 5, 0, productTableBody.Width / 5, productTableBody.Height, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody2.Margin = new Padding(0);
                    prodNameBody2.BorderStyle = BorderStyle.FixedSingle;
                    Label prodNameBody3 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_3_" + prdID + "_" + prdLimitedCnt, restAmount.ToString() + "(" + prdLimitedCnt.ToString() + ")", productTableBody.Width * 3 / 5, 0, productTableBody.Width / 5, productTableBody.Height, Color.White, Color.FromArgb(255, 142, 133, 118), 12);
                    prodNameBody3.Margin = new Padding(0);
                    prodNameBody3.BorderStyle = BorderStyle.FixedSingle;
                    prodNameBody3.Click += new EventHandler(this.ShowNumberInputDialog);

                    string stateText = constants.saleStatusLabel;
                    Color bgColor = Color.FromArgb(255, 62, 113, 255);
                    if (soldFlag == 1)
                    {
                        bgColor = Color.FromArgb(255, 220, 56, 50);
                        stateText = constants.saleStopLabel;
                        stopedPrdIDArray.Add(prdID);
                    }
                    Label prodNameBody4 = createLabel.CreateLabelsInPanel(productTableBody, "prodBody_" + k + "_4_" + prdID + "_" + rowID + "_" + prdName, stateText, productTableBody.Width * 4 / 5, 0, productTableBody.Width / 6, productTableBody.Height, bgColor, Color.White, 12);
                    prodNameBody4.Click += new EventHandler(this.ProductSaleStateSwitching);
                    prodNameBody4.Margin = new Padding(productTableBody.Width / 5 - productTableBody.Width / 6, 0, 0, 0);
                    prodNameBody4.BorderStyle = BorderStyle.FixedSingle;

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

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal.Controls.Clear();
            mainFormGlobal.Controls.Remove(backButtonGlobal);
            MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
        }

        public void SetVal(string selectedIndex)
        {

            productListPanelGlobal.Controls.Clear();
            selectedCategoryIndex = int.Parse(selectedIndex);
            string saleStateButtonImage = null;
            string saleStatusTxt = constants.saleStatusLabel;
            int soldoutState = soldoutCategoryIndex.Find(elem => elem == selectedCategoryIndex + 1);
            if(soldoutState != 0)
            {
                //saleStateButtonGlobal.BackColor = Color.Red;
                saleStateButtonImage = constants.soldoutButtonImage2;
                saleStatusTxt = constants.saleStopLabel;
                saleStateGlobal = false;
            }
            else
            {
                //saleStateButtonGlobal.BackColor = Color.FromArgb(255, 0, 112, 192);
                saleStateButtonImage = constants.soldoutButtonImage1;
                saleStatusTxt = constants.saleStatusLabel;
                saleStateGlobal = true;
                CreateProductTable();
            }
            bodyPanelGlobal.Controls.Remove(saleStateButtonGlobal);
            Button saleStateButton = customButton.CreateButtonWithImage(saleStateButtonImage, "saleSateButton", saleStatusTxt, bodyPanelGlobal.Width * 2 / 3 + 20, bodyPanelGlobal.Height / 18 - 25, bodyPanelGlobal.Width / 6, 50, 1, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            saleStateButton.Click += new EventHandler(this.SaleSateSwitching);
            bodyPanelGlobal.Controls.Add(saleStateButton);
            saleStateButtonGlobal = saleStateButton;
        }

        private void SaleSateSwitching(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string saleStateButtonImage = null;
            string saleStatusTxt = constants.saleStatusLabel;

            if (saleStateGlobal)
            {
                string queryCmd0 = "UPDATE " + constants.tbNames[0] + " SET SoldFlag=1 WHERE CategoryID=@categoryID";
                sqlite_cmd.CommandText = queryCmd0;
                sqlite_cmd.Parameters.AddWithValue("@categoryID", categoryIDList[selectedCategoryIndex]);
                sqlite_cmd.ExecuteNonQuery();

                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=1 WHERE CategoryID=@categoryID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@categoryID", categoryIDList[selectedCategoryIndex]);
                sqlite_cmd.ExecuteNonQuery();

                dbClass.InsertLog(5, "売り切れ処理", categoryNameList[selectedCategoryIndex] + " " + constants.saleStopLabel);

                soldoutCategoryIndex.Add(selectedCategoryIndex + 1);
                saleStateButtonImage = constants.soldoutButtonImage2;
                saleStatusTxt = constants.saleStopLabel;
                saleStateGlobal = false;
                productListPanelGlobal.Controls.Clear();

            }
            else
            {
                string queryCmd0 = "UPDATE " + constants.tbNames[0] + " SET SoldFlag=0 WHERE CategoryID=@categoryID";
                sqlite_cmd.CommandText = queryCmd0;
                sqlite_cmd.Parameters.AddWithValue("@categoryID", categoryIDList[selectedCategoryIndex]);
                sqlite_cmd.ExecuteNonQuery();

                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=0 WHERE CategoryID=@categoryID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@categoryID", categoryIDList[selectedCategoryIndex]);
                sqlite_cmd.ExecuteNonQuery();
                if(stopedPrdIDArray != null)
                {
                    foreach (int stopedID in stopedPrdIDArray)
                    {
                        string queryCmd2 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=1 WHERE ProductID=@prdID";
                        sqlite_cmd.CommandText = queryCmd2;
                        sqlite_cmd.Parameters.AddWithValue("@prdID", stopedID);
                        sqlite_cmd.ExecuteNonQuery();
                    }
                }

                dbClass.InsertLog(5, "売り切れ処理", categoryNameList[selectedCategoryIndex] + " " + constants.saleStatusLabel);

                soldoutCategoryIndex.Remove(selectedCategoryIndex + 1);
                saleStateButtonImage = constants.soldoutButtonImage1;
                saleStatusTxt = constants.saleStatusLabel;
                saleStateGlobal = true;
                CreateProductTable();
            }
            bodyPanelGlobal.Controls.Remove(saleStateButtonGlobal);
            Button saleStateButton = customButton.CreateButtonWithImage(saleStateButtonImage, "saleSateButton", saleStatusTxt, bodyPanelGlobal.Width * 2 / 3 + 20, bodyPanelGlobal.Height / 18 - 25, bodyPanelGlobal.Width / 6, 50, 1, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            saleStateButton.Click += new EventHandler(this.SaleSateSwitching);
            bodyPanelGlobal.Controls.Add(saleStateButton);
            saleStateButtonGlobal = saleStateButton;

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

        }

        private void ProductSaleStateSwitching(object sender, EventArgs e)
        {
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();

            Label btnTemp = (Label)sender;
            int prdID = int.Parse(btnTemp.Name.Split('_')[3]);
            int rowID = int.Parse(btnTemp.Name.Split('_')[4]);
            string prdName = btnTemp.Name.Split('_')[5];
            if (btnTemp.Text == constants.saleStatusLabel)
            {

                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=1 WHERE ProductID=@productID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@productID", prdID);
                sqlite_cmd.ExecuteNonQuery();
                stopedPrdIDArray.Add(rowID);
                btnTemp.Text = constants.saleStopLabel;
                btnTemp.BackColor = Color.FromArgb(255, 220, 56, 50);
                dbClass.InsertLog(5, "売り切れ処理", categoryNameList[selectedCategoryIndex] + "-" + prdName + " " + constants.saleStopLabel);
            }
            else
            {
                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=0 WHERE ProductID=@productID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@productID", prdID);
                sqlite_cmd.ExecuteNonQuery();
                stopedPrdIDArray.Remove(rowID);
                btnTemp.Text = constants.saleStatusLabel;
                btnTemp.BackColor = Color.FromArgb(255, 62, 113, 255);
                dbClass.InsertLog(5, "売り切れ処理", categoryNameList[selectedCategoryIndex] + "-" + prdName + " " + constants.saleStatusLabel);
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

        }

        private void ShowNumberInputDialog(object sender, EventArgs e)
        {
            Label objectHandler = (Label)sender;
            labelForLimitAmount = objectHandler;
            string objectHandlerText = objectHandler.Text;
            //string limitAmounts = objectHandlerText.Split('(')[1];
            string limitAmounts = objectHandlerText.Substring(objectHandlerText.IndexOf('(') + 1, objectHandlerText.Length - objectHandlerText.IndexOf('(') - 2);
            int limitAmount = int.Parse(limitAmounts);
            //CreatePanel numberInputPanel = createPanel.
            numberInput.CreateNumberInputDialog("soldoutSetting1", limitAmount, objectHandler.Name);
           // MessageBox.Show(limitAmounts);
        }

        public void SetLimitationValue(string limitAmount)
        {
            string objectHandlerText = labelForLimitAmount.Text;
            string prefixName = labelForLimitAmount.Name.Split('_')[0] + "_" + labelForLimitAmount.Name.Split('_')[1] + "_" + labelForLimitAmount.Name.Split('_')[2];
            int prdID = int.Parse(labelForLimitAmount.Name.Split('_')[3]);
            int oldLimitedAmount = int.Parse(labelForLimitAmount.Name.Split('_')[4]);
            string realAmounts = objectHandlerText.Split('(')[0];

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            if (limitAmount != "" )
            {
                SQLiteCommand sqlite_cmd, sqlite_cmd0;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET LimitedCnt=@limitedAmount WHERE ProductID=@prdID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@limitedAmount", int.Parse(limitAmount));
                sqlite_cmd.Parameters.AddWithValue("@prdID", prdID);
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd0 = sqlite_conn.CreateCommand();
                string queryCmd0 = "UPDATE " + constants.tbNames[3] + " SET limitFlag=1 WHERE prdRealID=@prdID";
                sqlite_cmd0.CommandText = queryCmd0;
                sqlite_cmd0.Parameters.AddWithValue("@prdID", prdID);
                sqlite_cmd0.ExecuteNonQuery();

                realAmounts =limitAmount;
                labelForLimitAmount.Name = prefixName + "_" + prdID.ToString() + "_" + limitAmount;
                labelForLimitAmount.Text = realAmounts + "(" + limitAmount + ")";

                sqlite_cmd0.Dispose();
                sqlite_cmd0 = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;

            }
            else if(int.Parse(limitAmount) == 0)
            {
                SQLiteCommand sqlite_cmd, sqlite_cmd0;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd1 = "UPDATE " + constants.tbNames[2] + " SET LimitedCnt=@limitedAmount WHERE ProductID=@prdID";
                sqlite_cmd.CommandText = queryCmd1;
                sqlite_cmd.Parameters.AddWithValue("@limitedAmount", int.Parse(limitAmount));
                sqlite_cmd.Parameters.AddWithValue("@prdID", prdID);
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd0 = sqlite_conn.CreateCommand();
                string queryCmd0 = "UPDATE " + constants.tbNames[3] + " SET limitFlag=1 WHERE prdRealID=@prdID";
                sqlite_cmd0.CommandText = queryCmd0;
                sqlite_cmd0.Parameters.AddWithValue("@prdID", prdID);
                sqlite_cmd0.ExecuteNonQuery();

                labelForLimitAmount.Name = prefixName + "_" + prdID.ToString() + "_" + limitAmount;
                labelForLimitAmount.Text = "0(0)";

                sqlite_cmd0.Dispose();
                sqlite_cmd0 = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;

            }

            sqlite_conn.Dispose();
            sqlite_conn = null;

        }

        private void GetCategoryList()
        {
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string queryCmd0 = "SELECT COUNT(id) FROM " + constants.tbNames[0];
            sqlite_cmd.CommandText = queryCmd0;
            categoryRowCount = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            categoryNameList = new string[categoryRowCount];
            categoryIDList = new int[categoryRowCount];
            categoryDisplayPositionList = new int[categoryRowCount];
            categorySoldStateList = new int[categoryRowCount];

            string queryCmd = "SELECT * FROM " + constants.tbNames[0] + " ORDER BY id";
            sqlite_cmd.CommandText = queryCmd;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    categoryIDList[k] = sqlite_datareader.GetInt32(1);
                    categoryNameList[k] = sqlite_datareader.GetString(2);
                    categoryDisplayPositionList[k] = sqlite_datareader.GetInt32(6);
                    //bool saleFlag = false;
                    //string openTime = "";
                    //if (week == "Sat" || week == "土")
                    //{
                    //    openTime = sqlite_datareader.GetString(4);
                    //}
                    //else if (week == "Sun" || week == "日")
                    //{
                    //    openTime = sqlite_datareader.GetString(5);
                    //}
                    //else
                    //{
                    //    openTime = sqlite_datareader.GetString(3);
                    //}
                    //string[] openTimeArr = openTime.Split('/');
                    //foreach (string openTimeArrItem in openTimeArr)
                    //{
                    //    string[] openTimeSubArr = openTimeArrItem.Split('-');
                    //    bool saleAvailableFlag = constants.saleAvailable(openTimeSubArr[0], openTimeSubArr[1]);

                    //    if (saleAvailableFlag)
                    //    {
                    //        saleFlag = true;
                    //        break;
                    //    }
                    //}
                    //categorySoldStateList[k] = 1;
                    //if(saleFlag == true)
                    //{
                        categorySoldStateList[k] = 0;
                    //}
                    if(sqlite_datareader.GetInt32(10) == 1)
                    {
                        soldoutCategoryIndex.Add(k + 1);
                    }
                }
                k++;
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;

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
