using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class PreviewSalePage : Form
    {
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel mainPanelGlobal_2 = null;
        Panel LeftPanelGlobal = null;
        Panel MainBodyPanelGlobal = null;
        Constant constants = new Constant();
        CustomButton customButton = new CustomButton();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        OrderDialog orderDialog = new OrderDialog();

        int categoryIDGlobal = 0;
        string[] categoryNameList = null;
        int[] categoryIDList = null;
        int[] categoryDisplayPositionList = null;
        int[] categoryLayoutList = null;
        string[] categoryBackImageList = null;
        private string[] categoryBackColor = new string[10];
        private string[] categoryTextColor = new string[10];
        private string[] screenMsgBackColor = new string[10];
        private string[] screenMsgTextColor = new string[10];

        private Bitmap BackgroundBitmap = null;
        Color borderClr = Color.FromArgb(255, 23, 55, 94);
        Pen borderPen = null;
        Color penClr = Color.FromArgb(255, 23, 55, 94);
        int nWidth = 0, nHeight = 0;
        int nWidth1 = 0, nHeight1 = 0;
        int nWidth2 = 0, nHeight2 = 0;
        int nWidth3 = 0, nHeight3 = 0;
        Rectangle rc = new Rectangle(0, 0, 0, 0);
        Panel[] p_ProductCon = null;
        PictureBox[] pb_Image = null;
        bool bLoad = false;
        int curProduct = 0;
        string menuTitle1 = "";
        string menuTitle2 = "";
        string currentDir = "";


        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;

        SQLiteConnection sqlite_conn;

        public PreviewSalePage(Form1 mainForm, Panel mainPanel, int categoryIndex, int[] categoryIDArray, string[] categoryNameArray, int[] categoryDisplayPositionArray, int[] categoryLayoutArray, string[] categoryBackImageArray)
        {
            InitializeComponent();

            currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }

            categoryIDGlobal = categoryIndex;
            categoryIDList = categoryIDArray;
            categoryNameList = categoryNameArray;
            categoryDisplayPositionList = categoryDisplayPositionArray;
            categoryLayoutList = categoryLayoutArray;
            categoryBackImageList = categoryBackImageArray;

            sqlite_conn = CreateConnection(constants.dbName);

            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;

            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;

            Panel LeftPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, 0, mainPanelGlobal_2.Width * 19 / 25, mainPanelGlobal_2.Height, BorderStyle.FixedSingle, Color.Transparent);
            LeftPanelGlobal = LeftPanel;


            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            try
            {
                string selectGeneralSql = "SELECT * FROM " + constants.tbNames[12];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectGeneralSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0) && k < 10)
                    {
                        if (k == 0)
                        {
                            menuTitle1 = sqlite_datareader.GetString(8);
                            menuTitle2 = sqlite_datareader.GetString(9);
                        }
                        categoryBackColor[k] = sqlite_datareader.GetString(0);
                        categoryTextColor[k] = sqlite_datareader.GetString(1);
                        screenMsgBackColor[k] = sqlite_datareader.GetString(2);
                        screenMsgTextColor[k] = sqlite_datareader.GetString(3);
                        k++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Panel ScreenMsgPanel = createPanel.CreateSubPanel(LeftPanel, 0, 0, LeftPanel.Width, LeftPanel.Height * 3 / 25 - 20, BorderStyle.None, Color.Transparent);
            ScreenMsgPanel.BackColor = ColorTranslator.FromHtml(screenMsgBackColor[categoryIDGlobal]);

            Label msgLabel1 = createLabel.CreateLabelsInPanel(ScreenMsgPanel, "msg1", menuTitle1, 30, 0, ScreenMsgPanel.Width / 2, ScreenMsgPanel.Height / 2, Color.Transparent, ColorTranslator.FromHtml(screenMsgTextColor[categoryIDGlobal]), 14, false, ContentAlignment.BottomLeft);
            Label msgLabel2 = createLabel.CreateLabelsInPanel(ScreenMsgPanel, "msg2", menuTitle2, 30, ScreenMsgPanel.Height / 2, ScreenMsgPanel.Width / 2, ScreenMsgPanel.Height / 2, Color.Transparent, ColorTranslator.FromHtml(screenMsgTextColor[categoryIDGlobal]), 14, false, ContentAlignment.TopLeft);


            Panel RightPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, mainPanelGlobal_2.Width * 19 / 25, 0, mainPanelGlobal_2.Width * 6 / 25, mainPanelGlobal_2.Height, BorderStyle.FixedSingle, Color.White);

            Panel MenuBodyLayout = createPanel.CreateSubPanel(LeftPanel, 20, LeftPanel.Height * 5 / 25 + 20, LeftPanel.Width - 20, LeftPanel.Height * 20 / 25 - 30, BorderStyle.None, Color.Transparent);

            MainBodyPanelGlobal = MenuBodyLayout;

            /** Left category menu button create */

            CreateCategoryList();
            if (categoryLayoutList[categoryIDGlobal] == 13)
            {
                CreateProductsList13();
            }
            else if (categoryLayoutList[categoryIDGlobal] == 11)
            {
                CreateProductsList11();
            }
            else
            {
                CreateProductsList();
            }

            /** Main Product Panel layout */

            /** right panel  */
            RightPanel.Padding = new Padding(10, 0, 10, 0);
            Button closeButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, RightPanel.Width / 2 - 100, RightPanel.Height * 6 / 7, 200, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            RightPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

        }


        private void CreateCategoryList()
        {
            Panel categoryPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 2 / 25, BorderStyle.None, Color.Transparent);
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            int categoryAmount = categoryNameList.Length;
            int k = 0;
            foreach(string categoryName in categoryNameList)
            {
                string backColor = categoryBackColor[k];
                string textColor = categoryTextColor[k];
                int nX = 0;
                int nY = 0;
                int w = (categoryPanel.Width + 30) / 6;
                int refVal = (categoryPanel.Width + 30) % 6;
                int btnH = categoryPanel.Height;
                int offset = 30;
                Point[] pts = {
                        new Point(0, 0),
                        new Point(w-offset, 0),
                        new Point(w, btnH),
                        new Point(0, btnH)
                    };

                if (k == 0)
                {
                    pts = new Point[] { new Point(0, 0), new Point(w - offset, 0), new Point(w, btnH), new Point(0, btnH) };
                    nX = 0;
                }
                else if (k == 5)
                {
                    nX = k * w - offset;
                    pts = new Point[] { new Point(nX, 0), new Point(nX + w + refVal, 0), new Point(nX + w + refVal, btnH), new Point(nX + offset, btnH) };
                }
                else
                {
                    nX = k * w - offset;
                    pts = new Point[] { new Point(nX, 0), new Point(nX + w, 0), new Point(nX + w + offset, btnH), new Point(nX + offset, btnH) };
                }


                Button categoryBtn = customButton.CreatePolyButtonWithStringColr(nX, nY, pts, categoryName, categoryName, false,
            "", textColor, backColor, 14);
                categoryBtn.FlatStyle = FlatStyle.Flat;

                if (categoryPanel.InvokeRequired)
                {
                    categoryPanel.Invoke((MethodInvoker)delegate
                    {
                        categoryPanel.Controls.Add(categoryBtn);
                    });
                }
                else
                {
                    categoryPanel.Controls.Add(categoryBtn);
                }
                categoryBtn.Invalidate();
                k++;
            }
            if (k < 6)
            {
                for (int m = k; m < 6; m++)
                {
                    string backColor = categoryBackColor[m];
                    string textColor = categoryTextColor[m];

                    int nX = 0;
                    int nY = 0;
                    int w = (categoryPanel.Width + 30) / 6;
                    int refVal = (categoryPanel.Width + 30) % 6;
                    int btnH = categoryPanel.Height;
                    int offset = 30;
                    Point[] pts = {
                        new Point(0, 0),
                        new Point(w-offset, 0),
                        new Point(w, btnH),
                        new Point(0, btnH)
                    };

                    if (m == 1)
                    {
                        pts = new Point[] { new Point(0, 0), new Point(w - offset, 0), new Point(w, btnH), new Point(0, btnH) };
                        nX = 0;
                    }
                    else if (m == 5)
                    {
                        nX = m * w - offset;
                        pts = new Point[] { new Point(nX, 0), new Point(nX + w + refVal, 0), new Point(nX + w + refVal, btnH), new Point(nX + offset, btnH) };
                    }
                    else
                    {
                        nX = m * w - offset;
                        pts = new Point[] { new Point(nX, 0), new Point(nX + w, 0), new Point(nX + w + offset, btnH), new Point(nX + offset, btnH) };
                    }

                    Button categoryBtn = customButton.CreatePolyButtonWithStringColr(nX, nY, pts, "empty_" + m, "", false,
                "", textColor, backColor, 14);
                    categoryBtn.FlatStyle = FlatStyle.Flat;
                    categoryBtn.Enabled = false;

                    if (categoryPanel.InvokeRequired)
                    {
                        categoryPanel.Invoke((MethodInvoker)delegate
                        {
                            categoryPanel.Controls.Add(categoryBtn);
                        });
                    }
                    else
                    {
                        categoryPanel.Controls.Add(categoryBtn);
                    }
                    categoryBtn.Invalidate();

                }
            }

        }


        private void CreateProductsList11()
        {
            MainBodyPanelGlobal.Controls.Clear();
            int w1 = (MainBodyPanelGlobal.Width - 100) / 5;
            int h1 = (MainBodyPanelGlobal.Height - 80) / 5;
            nWidth = 3 * w1 + 40;
            nHeight = 3 * h1 + 40;
            nHeight1 = h1;
            nWidth1 = w1;
            nHeight2 = 2 * h1 + 20;
            nWidth2 = 2 * w1 + 20;
            nHeight3 = h1;
            nWidth3 = 2 * w1 + 20;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            p_ProductCon = new Panel[categoryLayoutList[categoryIDGlobal]];
            pb_Image = new PictureBox[categoryLayoutList[categoryIDGlobal]];

            for (int i = 0; i < categoryLayoutList[categoryIDGlobal]; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdLimitedCnt = 0;
                int prdPrice = 0;
                string prdImageUrl = "";
                int soldFlag = 0;
                int prdRestAmount = 0;

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    /** 2020-09-01 stopped */
                    //string openTime = "";
                    //if (week == "Sat" || week == "土")
                    //{
                    //    openTime = sqlite_datareader.GetString(6);
                    //}
                    //else if (week == "Sun" || week == "日")
                    //{
                    //    openTime = sqlite_datareader.GetString(7);
                    //}
                    //else
                    //{
                    //    openTime = sqlite_datareader.GetString(5);
                    //}
                    //string[] openTimeArr = openTime.Split('/');
                    //foreach (string openTimeArrItem in openTimeArr)
                    //{
                    //    string[] openTimeSubArr = openTimeArrItem.Split('-');
                    //    bool saleAvailableFlag = constants.saleAvailable(openTimeSubArr[0], openTimeSubArr[1]);
                    //    if (saleAvailableFlag)
                    //    {
                            prdID = sqlite_datareader.GetInt32(0);
                            prdName = sqlite_datareader.GetString(3);
                            prdPrice = sqlite_datareader.GetInt32(8);
                            prdLimitedCnt = sqlite_datareader.GetInt32(9);
                            prdImageUrl = sqlite_datareader.GetString(16);
                            soldFlag = sqlite_datareader.GetInt32(17);
                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdID=@prdID";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", prdID);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (prdLimitedCnt != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        prdRestAmount = prdLimitedCnt - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        prdRestAmount = prdLimitedCnt;
                                    }
                                }
                                else
                                {
                                    prdRestAmount = 10000;
                                }
                            }

                    //        break;
                    //    }
                    //}

                }

                Panel p = new Panel();
                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    borderPen = new Pen(ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), 3);
                    BackgroundBitmap = null;
                    if (i == 0)
                    {
                        p = createPanel.CreatePanelForProducts(0, 0, nWidth, nHeight, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 1)
                    {
                        p = createPanel.CreatePanelForProducts(nWidth + 20, 0, nWidth2, nHeight2, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor3_Paint);
                    }
                    else if (i == 2 || i == 6 || i == 10)
                    {
                        int hh = 2 * h1 + 40;
                        if (i == 6) hh = 3 * h1 + 60;
                        if (i == 10) hh = 4 * h1 + 80;
                        p = createPanel.CreatePanelForProducts(nWidth + 20, hh, nWidth3, nHeight3, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor4_Paint);
                    }
                    else if (i < 6)
                    {
                        p = createPanel.CreatePanelForProducts((i - 3) * w1 + (i - 3) * 20, nHeight + 20, w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else
                    {
                        p = createPanel.CreatePanelForProducts((i - 7) * w1 + (i - 7) * 20, nHeight + h1 + 40, w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }

                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    BackgroundBitmap = new Bitmap(prdImageUrl);
                    pb_Image[i].Image = BackgroundBitmap;

                    pb_Image[i].Enabled = false;

                    p.Controls.Add(pb_Image[i]);

                    MainBodyPanelGlobal.Controls.Add(p);
                    p_ProductCon[i] = p;
                    if (prdImageUrl == null)
                    {
                        p.Visible = false;
                    }
                }
            }
        }

        private void CreateProductsList13()
        {
            MainBodyPanelGlobal.Controls.Clear();
            int w1 = (MainBodyPanelGlobal.Width - 100) / 5;
            int h1 = (MainBodyPanelGlobal.Height - 80) / 5;
            nWidth = 2 * w1 + 20;
            nHeight = 2 * h1 + 20;
            nHeight1 = h1;
            nWidth1 = w1;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            p_ProductCon = new Panel[categoryLayoutList[categoryIDGlobal]];
            pb_Image = new PictureBox[categoryLayoutList[categoryIDGlobal]];


            for (int i = 0; i < categoryLayoutList[categoryIDGlobal]; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdLimitedCnt = 0;
                int prdPrice = 0;
                string prdImageUrl = "";
                int soldFlag = 0;
                int prdRestAmount = 0;

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    //string openTime = "";
                    //if (week == "Sat" || week == "土")
                    //{
                    //    openTime = sqlite_datareader.GetString(6);
                    //}
                    //else if (week == "Sun" || week == "日")
                    //{
                    //    openTime = sqlite_datareader.GetString(7);
                    //}
                    //else
                    //{
                    //    openTime = sqlite_datareader.GetString(5);
                    //}
                    //string[] openTimeArr = openTime.Split('/');
                    //foreach (string openTimeArrItem in openTimeArr)
                    //{
                    //    string[] openTimeSubArr = openTimeArrItem.Split('-');
                    //    bool saleAvailableFlag = constants.saleAvailable(openTimeSubArr[0], openTimeSubArr[1]);
                    //    if (saleAvailableFlag)
                    //    {
                            prdID = sqlite_datareader.GetInt32(0);
                            prdName = sqlite_datareader.GetString(3);
                            prdPrice = sqlite_datareader.GetInt32(8);
                            prdLimitedCnt = sqlite_datareader.GetInt32(9);
                            prdImageUrl = sqlite_datareader.GetString(16);
                            soldFlag = sqlite_datareader.GetInt32(17);

                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdID=@prdID";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", prdID);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (prdLimitedCnt != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        prdRestAmount = prdLimitedCnt - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        prdRestAmount = prdLimitedCnt;
                                    }
                                }
                                else
                                {
                                    prdRestAmount = 1000;
                                }
                            }

                    //        break;
                    //    }
                    //}

                }

                Panel p = new Panel();
                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    borderPen = new Pen(ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), 3);
                    BackgroundBitmap = null;
                    if (i == 0 || i == 1)
                    {
                        p = createPanel.CreatePanelForProducts(i * (2 * w1 + 20) + i * 20, 0, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 2 || i == 3)
                    {
                        p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 2) * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else if (i == 4 || i == 5)
                    {
                        p = createPanel.CreatePanelForProducts((i - 4) * (2 * w1 + 20) + (i - 4) * 20, 2 * h1 + 40, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 6 || i == 7)
                    {
                        p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 4) * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else
                    {
                        p = createPanel.CreatePanelForProducts((i - 8) * w1 + (i - 8) * 20, 2 * (2 * h1 + 40), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }


                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    BackgroundBitmap = new Bitmap(prdImageUrl);
                    pb_Image[i].Image = BackgroundBitmap;

                    pb_Image[i].Enabled = false;

                    p.Controls.Add(pb_Image[i]);
                    MainBodyPanelGlobal.Controls.Add(p);
                    p_ProductCon[i] = p;
                    if (prdImageUrl == null)
                    {
                        p.Visible = false;
                    }

                }
            }
        }


        private void CreateProductsList()
        {
            MainBodyPanelGlobal.Controls.Clear();
            int nWD = 0, nHD = 0;
            if (categoryLayoutList[categoryIDGlobal] == 25 || categoryLayoutList[categoryIDGlobal] == 16 || categoryLayoutList[categoryIDGlobal] == 9 || categoryLayoutList[categoryIDGlobal] == 4)
                nWD = nHD = (int)Math.Sqrt((double)categoryLayoutList[categoryIDGlobal]);
            if (categoryLayoutList[categoryIDGlobal] == 10) { nWD = 2; nHD = 5; }
            if (categoryLayoutList[categoryIDGlobal] == 6) { nWD = 3; nHD = 2; }
            if (categoryLayoutList[categoryIDGlobal] == 8) { nWD = 4; nHD = 2; }
            if (categoryLayoutList[categoryIDGlobal] == 20) { nWD = 5; nHD = 4; }

            int w1 = (MainBodyPanelGlobal.Width - 20 * nWD) / nWD;
            int h1 = (MainBodyPanelGlobal.Height - 20 * (nHD - 1)) / nHD;
            if (categoryLayoutList[categoryIDGlobal] == 10)
                w1 = (MainBodyPanelGlobal.Width - 50 * nWD) / nWD;

            nHeight1 = h1;
            nWidth1 = w1;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            p_ProductCon = new Panel[categoryLayoutList[categoryIDGlobal]];
            pb_Image = new PictureBox[categoryLayoutList[categoryIDGlobal]];

            for (int i = 0; i < categoryLayoutList[categoryIDGlobal]; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdPrice = 0;
                string prdImageUrl = "";
                int soldFlag = 0;
                int prdLimitedCnt = 0;
                int prdRestAmount = 0;

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    //string openTime = "";
                    //if (week == "Sat" || week == "土")
                    //{
                    //    openTime = sqlite_datareader.GetString(6);
                    //}
                    //else if (week == "Sun" || week == "日")
                    //{
                    //    openTime = sqlite_datareader.GetString(7);
                    //}
                    //else
                    //{
                    //    openTime = sqlite_datareader.GetString(5);
                    //}
                    //string[] openTimeArr = openTime.Split('/');
                    //foreach (string openTimeArrItem in openTimeArr)
                    //{
                    //    string[] openTimeSubArr = openTimeArrItem.Split('-');
                    //    bool saleAvailableFlag = constants.saleAvailable(openTimeSubArr[0], openTimeSubArr[1]);
                    //    if (saleAvailableFlag)
                    //    {
                            prdID = sqlite_datareader.GetInt32(0);
                            prdName = sqlite_datareader.GetString(3);
                            prdPrice = sqlite_datareader.GetInt32(8);
                            prdLimitedCnt = sqlite_datareader.GetInt32(9);
                            prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                            soldFlag = sqlite_datareader.GetInt32(17);
                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdID=@prdID";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIDGlobal]);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", prdID);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (prdLimitedCnt != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        prdRestAmount = prdLimitedCnt - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        prdRestAmount = prdLimitedCnt;
                                    }
                                }
                                else
                                {
                                    prdRestAmount = 1000;
                                }
                            }

                    //        break;
                    //    }
                    //}

                }


                int x = i % nWD;
                int yy = i / nWD;
                int d = 20;
                if (categoryDisplayPositionList[categoryIDGlobal] == 10) d = 50;
                BackgroundBitmap = null;

                //MessageBox.Show(p.Height.ToString());

                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    Panel p = createPanel.CreatePanelForProducts(x * (w1 + d), yy * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), Color.White);
                    borderPen = new Pen(ColorTranslator.FromHtml(categoryBackColor[categoryIDGlobal]), 3);
                    p.Paint += new PaintEventHandler(Panelbordercolor_Paint);

                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    BackgroundBitmap = new Bitmap(prdImageUrl);
                    pb_Image[i].Image = BackgroundBitmap;

                    pb_Image[i].Enabled = false;
                    p.Controls.Add(pb_Image[i]);
                    MainBodyPanelGlobal.Controls.Add(p);
                    p_ProductCon[i] = p;
                    if (prdImageUrl == null)
                    {
                        p.Visible = false;
                    }
                }
            }
        }

        private void Panelbordercolor_Paint(object sender, PaintEventArgs e)
        {

            Panel p = (Panel)sender;
            int index = int.Parse(p.Name);
            if (!bLoad)
            {
                Graphics g = e.Graphics;
                g.DrawLine(borderPen, 0, 1, nWidth1, 1);
                g.DrawLine(borderPen, 1, 0, 1, nHeight1);
                g.DrawLine(borderPen, nWidth1 - 2, 0, nWidth1 - 2, nHeight1);
                g.DrawLine(borderPen, 0, nHeight1 - 2, nWidth1, nHeight1 - 2);
                g.Dispose();
            }
            else
            {
                if (curProduct == index)
                {
                    borderPen = new Pen(Color.Red, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(borderPen, 0, 1, nWidth1, 1);
                    g.DrawLine(borderPen, 1, 0, 1, nHeight1);
                    g.DrawLine(borderPen, nWidth1 - 2, 0, nWidth1 - 2, nHeight1);
                    g.DrawLine(borderPen, 0, nHeight1 - 2, nWidth1, nHeight1 - 2);
                    g.Dispose();
                }
                else
                {
                    Pen pp = new Pen(borderClr, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth1, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight1);
                    g.DrawLine(pp, nWidth1 - 2, 0, nWidth1 - 2, nHeight1);
                    g.DrawLine(pp, 0, nHeight1 - 2, nWidth1, nHeight1 - 2);
                    g.Dispose();
                }
            }

        }

        private void Panelbordercolor2_Paint(object sender, PaintEventArgs e)
        {

            Panel p = (Panel)sender;
            int index = int.Parse(p.Name);

            if (!bLoad)
            {
                Graphics g = e.Graphics;
                g.DrawLine(borderPen, 0, 1, nWidth, 1);
                g.DrawLine(borderPen, 1, 0, 1, nHeight);
                g.DrawLine(borderPen, nWidth - 2, 0, nWidth - 2, nHeight);
                g.DrawLine(borderPen, 0, nHeight - 2, nWidth, nHeight - 2);
                g.Dispose();
            }
            else
            {
                if (curProduct == index)
                {
                    borderPen = new Pen(Color.Red, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(borderPen, 0, 1, nWidth, 1);
                    g.DrawLine(borderPen, 1, 0, 1, nHeight);
                    g.DrawLine(borderPen, nWidth - 2, 0, nWidth - 2, nHeight);
                    g.DrawLine(borderPen, 0, nHeight - 2, nWidth, nHeight - 2);
                    g.Dispose();
                }
                else
                {
                    Pen pp = new Pen(borderClr, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight);
                    g.DrawLine(pp, nWidth - 2, 0, nWidth - 2, nHeight);
                    g.DrawLine(pp, 0, nHeight - 2, nWidth, nHeight - 2);
                    g.Dispose();
                }

            }
        }
        private void Panelbordercolor3_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            int index = int.Parse(p.Name);
            if (!bLoad)
            {
                Graphics g = e.Graphics;
                g.DrawLine(borderPen, 0, 1, nWidth2, 1);
                g.DrawLine(borderPen, 1, 0, 1, nHeight2);
                g.DrawLine(borderPen, nWidth2 - 2, 0, nWidth2 - 2, nHeight2);
                g.DrawLine(borderPen, 0, nHeight2 - 2, nWidth2, nHeight2 - 2);
                g.Dispose();
            }
            else
            {
                if (curProduct == index)
                {
                    borderPen = new Pen(Color.Red, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(borderPen, 0, 1, nWidth2, 1);
                    g.DrawLine(borderPen, 1, 0, 1, nHeight2);
                    g.DrawLine(borderPen, nWidth2 - 2, 0, nWidth2 - 2, nHeight2);
                    g.DrawLine(borderPen, 0, nHeight2 - 2, nWidth2, nHeight2 - 2);
                    g.Dispose();
                }
                else
                {
                    Pen pp = new Pen(borderClr, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth2, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight2);
                    g.DrawLine(pp, nWidth2 - 2, 0, nWidth2 - 2, nHeight2);
                    g.DrawLine(pp, 0, nHeight2 - 2, nWidth2, nHeight2 - 2);
                    g.Dispose();
                }
            }
        }
        private void Panelbordercolor4_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            int index = int.Parse(p.Name);
            if (!bLoad)
            {
                Graphics g = e.Graphics;
                g.DrawLine(borderPen, 0, 1, nWidth3, 1);
                g.DrawLine(borderPen, 1, 0, 1, nHeight3);
                g.DrawLine(borderPen, nWidth3 - 2, 0, nWidth3 - 2, nHeight3);
                g.DrawLine(borderPen, 0, nHeight3 - 2, nWidth3, nHeight3 - 2);
                g.Dispose();
            }
            else
            {
                if (curProduct == index)
                {
                    borderPen = new Pen(Color.Red, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(borderPen, 0, 1, nWidth3, 1);
                    g.DrawLine(borderPen, 1, 0, 1, nHeight3);
                    g.DrawLine(borderPen, nWidth3 - 2, 0, nWidth3 - 2, nHeight3);
                    g.DrawLine(borderPen, 0, nHeight3 - 2, nWidth3, nHeight3 - 2);
                    g.Dispose();
                }
                else
                {
                    Pen pp = new Pen(borderClr, 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth3, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight3);
                    g.DrawLine(pp, nWidth3 - 2, 0, nWidth3 - 2, nHeight3);
                    g.DrawLine(pp, 0, nHeight3 - 2, nWidth3, nHeight3 - 2);
                    g.Dispose();
                }
            }
        }


        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal_2.Controls.Clear();
            mainPanelGlobal_2.Hide();
            mainFormGlobal.mainPanelGlobal.Show();
            mainFormGlobal.topPanelGlobal.Show();
            mainFormGlobal.bottomPanelGlobal.Show();
            CategoryList frm = new CategoryList(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainFormGlobal.mainPanelGlobal.Controls.Add(frm);
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
