using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class ProductInfoSetting : Form
    {
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel mainPanelGlobal_2 = null;
        Panel LeftPanelGlobal = null;
        Panel MainBodyPanelGlobal = null;
        Panel categoryPanel = null;
        NumberInput numberInput = new NumberInput();
        Constant constants = new Constant();
        CustomButton customButton = new CustomButton();
        CreateTextBox createTextBox = new CreateTextBox();
        TextBox prdNameTxtBx = null;
        TextBox prdPrintNameTxtBx = null;
        TextBox prdPriceTxtBx = null;
        TextBox startHourTxtBx = null;
        TextBox startMinuteTxtBx = null;
        TextBox endHourTxtBx = null;
        TextBox endMinuteTxtBx = null;
        Panel optionPanel = null;

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        OrderDialog orderDialog = new OrderDialog();
        DetailView detailView = new DetailView();

        int categoryIndexGlobal = 0;
        int categoryIDGlobal = 0;
        int selectedCategoryLayout = 0;
        int[] categoryIDList = new int[10];
        private Button[] categoryButton = new Button[10];
        Panel timeSettingPanel = null;

        private string[] screenMsgBackColor = new string[10];
        private string[] screenMsgTextColor = new string[10];
        private int[] realProductIDArray = new int[50];

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
        RadioButton r1Global = null;
        RadioButton r2Global = null;
        bool bLoad = false;
        int curProduct = 0;
        string menuTitle1 = "";
        string menuTitle2 = "";
        string currentDir = "";
        int selectedProductIndex = -1;

        string prdNameGlobal = "";
        string prdPrintNameGlobal = "";
        string prdPriceGlobal = "";
        int prdIDGlobal = 0;
        int cardNumberGlobal = 1;
        string prdImgUrlGlobal = "";
        string prdOriginImgUrlGlobal = "";
        string topTextColor = "";
        string bottomBKColor = "";
        string bottomTextColor = "";
        string nameRC = "0:0:0:0";
        string priceRC = "0:0:0:0";
        string[] btBackClr = new string[6];
        string[] btTxtClr = new string[6];
        string[] topBackClr = new string[6];
        string[] topTxtClr = new string[6];

        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;

        SQLiteConnection sqlite_conn;

        public ProductInfoSetting(Form1 mainForm, Panel mainPanel)
        {
            InitializeComponent();
            
            currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;

            detailView.InitSaleTimeSetting(this);
            mainPanelGlobal_2 = mainForm.mainPanelGlobal_2;

            Panel LeftPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, 0, 0, mainPanelGlobal_2.Width * 19 / 25, mainPanelGlobal_2.Height, BorderStyle.FixedSingle, Color.White);
            LeftPanelGlobal = LeftPanel;


            try
            {
                string selectGeneralSql = "SELECT * FROM " + constants.tbNames[12];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectGeneralSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0) && k < 6)
                    {
                        if (k == 0)
                        {
                            menuTitle1 = sqlite_datareader.GetString(8);
                            menuTitle2 = sqlite_datareader.GetString(9);
                        }
                        screenMsgBackColor[k] = sqlite_datareader.GetString(2);
                        screenMsgTextColor[k] = sqlite_datareader.GetString(3);
                        btBackClr[k] = sqlite_datareader.GetString(6);
                        btTxtClr[k] = sqlite_datareader.GetString(7);
                        topBackClr[k] = sqlite_datareader.GetString(4);
                        topTxtClr[k] = sqlite_datareader.GetString(5);
                        k++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            categoryIndexGlobal = 0;
            Panel ScreenMsgPanel = createPanel.CreateSubPanel(LeftPanel, 0, 0, LeftPanel.Width, LeftPanel.Height * 3 / 25 - 20, BorderStyle.None, Color.Transparent);
            ScreenMsgPanel.BackColor = ColorTranslator.FromHtml(screenMsgBackColor[0]);

            Label msgLabel1 = createLabel.CreateLabelsInPanel(ScreenMsgPanel, "msg1", menuTitle1, 30, 0, ScreenMsgPanel.Width / 2, ScreenMsgPanel.Height / 2, Color.Transparent, ColorTranslator.FromHtml(screenMsgTextColor[0]), 14, false, ContentAlignment.BottomLeft);
            Label msgLabel2 = createLabel.CreateLabelsInPanel(ScreenMsgPanel, "msg2", menuTitle2, 30, ScreenMsgPanel.Height / 2, ScreenMsgPanel.Width / 2, ScreenMsgPanel.Height / 2, Color.Transparent, ColorTranslator.FromHtml(screenMsgTextColor[0]), 14, false, ContentAlignment.TopLeft);


            Panel RightPanel = createPanel.CreateSubPanel(mainPanelGlobal_2, mainPanelGlobal_2.Width * 19 / 25, 0, mainPanelGlobal_2.Width * 6 / 25, mainPanelGlobal_2.Height, BorderStyle.FixedSingle, Color.White);

            categoryPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 2 / 25, BorderStyle.None, Color.Transparent);

            Panel downPanel = createPanel.CreateSubPanel(LeftPanel, 0, LeftPanel.Height * 5 / 25 - 20, LeftPanel.Width, LeftPanel.Height * 20 / 25 + 20, BorderStyle.FixedSingle, Color.Transparent);

            Panel MenuBodyLayout = createPanel.CreateSubPanel(downPanel, 20, 20, downPanel.Width - 20, downPanel.Height - 30, BorderStyle.None, Color.White);

            MainBodyPanelGlobal = MenuBodyLayout;

            /** Left category menu button create */

            CreateCategoryList();
            /** Main Product Panel layout */

            /** right panel  */
            RightPanel.Padding = new Padding(10, 0, 10, 0);

            Panel namePricePanel = createPanel.CreateSubPanel(RightPanel, 8, 20, RightPanel.Width - 16, 350, BorderStyle.FixedSingle, Color.Transparent);
            namePricePanel.Paint += new PaintEventHandler(PanelBorderPaint);

            Label namePriceChangTitle = createLabel.CreateLabelsInPanel(namePricePanel, "namePriceChangTitle", "品名・販売金額の変更", 0, 10, namePricePanel.Width, 40, Color.Transparent, Color.Black, 16, false, ContentAlignment.BottomCenter);

            Label prdNameLb = createLabel.CreateLabelsInPanel(namePricePanel, "prdNameLb", constants.viewLabel + constants.prdNameField, 20, namePriceChangTitle.Bottom + 10, namePricePanel.Width - 40, 30, Color.Transparent, Color.Black, 14, false, ContentAlignment.BottomLeft);
            prdNameTxtBx = createTextBox.CreateTextBoxs_panel(namePricePanel, "prdNameTxtBx", 20, prdNameLb.Bottom + 5, namePricePanel.Width - 40, 30, 18, BorderStyle.FixedSingle);

            Label prdPrintNameLb = createLabel.CreateLabelsInPanel(namePricePanel, "prdPrintNameLb", constants.printProductNameField, 20, prdNameTxtBx.Bottom + 10, namePricePanel.Width - 40, 30, Color.Transparent, Color.Black, 14, false, ContentAlignment.BottomLeft);
            prdPrintNameTxtBx = createTextBox.CreateTextBoxs_panel(namePricePanel, "prdPrintNameTxtBx", 20, prdPrintNameLb.Bottom + 5, namePricePanel.Width - 40, 30, 18, BorderStyle.FixedSingle);

            Label prdPriceLb = createLabel.CreateLabelsInPanel(namePricePanel, "prdPriceLb", constants.priceField, 20, prdPrintNameTxtBx.Bottom + 10, namePricePanel.Width - 60, 30, Color.Transparent, Color.Black, 14, false, ContentAlignment.BottomLeft);
            prdPriceTxtBx = createTextBox.CreateTextBoxs_panel(namePricePanel, "prdPriceTxtBx", 20, prdPriceLb.Bottom + 5, namePricePanel.Width - 40, 30, 18, BorderStyle.FixedSingle);
            prdPriceTxtBx.KeyPress += new KeyPressEventHandler(NumberKeyPress);

            Button setButton1 = customButton.CreateButtonWithImage(constants.updateButton, "setButton", "", namePricePanel.Width - 120, namePricePanel.Height - 55, 100, 45, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            namePricePanel.Controls.Add(setButton1);
            setButton1.Click += new EventHandler(this.SetProductChange);

            Button closeButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, RightPanel.Width / 2 - 100, RightPanel.Height - 100, 200, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            RightPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);
        }

        private void CreateCategoryList()
        {
            try
            {
                constants.SaveLogData("saleScreen_2_2", "salePage loading category");
                if (categoryPanel.InvokeRequired)
                {
                    categoryPanel.Invoke((MethodInvoker)delegate
                    {
                        categoryPanel.Controls.Clear();
                    });
                }
                else
                {
                    categoryPanel.Controls.Clear();
                }

                sqlite_conn = CreateConnection(constants.dbName);

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();

                string queryCmd = "SELECT * FROM " + constants.tbNames[0] + " ORDER BY id";
                sqlite_cmd.CommandText = queryCmd;

                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    categoryIDList[k] = sqlite_datareader.GetInt32(1);
                    string categoryButtonText = sqlite_datareader.GetString(2);
                    if(k == categoryIndexGlobal)
                    {
                        selectedCategoryLayout = sqlite_datareader.GetInt32(7);
                        categoryIDGlobal = sqlite_datareader.GetInt32(1);
                    }

                    string categoryButtonName = "saleCategoryBtn_" + k + "_" + sqlite_datareader.GetInt32(1).ToString() + "_" + sqlite_datareader.GetInt32(7).ToString();


                    Button categoryBtn;
                    string categoryBtnImg;
                    if (k == categoryIndexGlobal)
                    {
                        categoryBtnImg = constants.categoryActiveButton;
                    }
                    else
                    {
                        categoryBtnImg = constants.categoryButton;
                    }
                    categoryBtn = customButton.CreateButtonWithImage(categoryBtnImg, categoryButtonName, categoryButtonText, 5 + (categoryPanel.Width / 6) * k, 5, categoryPanel.Width / 6 - 10, categoryPanel.Height - 10, 1, 20, 16, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);

                    categoryButton[k] = categoryBtn;
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

                    categoryBtn.Click += new EventHandler(this.SelectCategory);

                    k++;
                }

                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
                if (selectedCategoryLayout == 13)
                {
                    CreateProductsList13();
                }
                else if (selectedCategoryLayout == 11)
                {
                    CreateProductsList11();
                }
                else
                {
                    CreateProductsList();
                }

            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_2_2", ex.ToString());
                return;
            }
        }

        private void SelectCategory(object sender, EventArgs e)
        {

            MainBodyPanelGlobal.Hide();
            Button btnTemp = (Button)sender;
            categoryIndexGlobal = int.Parse(btnTemp.Name.Split('_')[1]);
            categoryIDGlobal = int.Parse(btnTemp.Name.Split('_')[2]);
            selectedCategoryLayout = int.Parse(btnTemp.Name.Split('_')[3]);
            for (int k = 0; k < categoryButton.Length; k++)
            {
                if (k == categoryIndexGlobal)
                {
                    categoryButton[k].ForeColor = ColorTranslator.FromHtml("#FDD648");
                    categoryButton[k].Font = new Font("Serif", 18, FontStyle.Bold);
                    using (Bitmap img = new Bitmap(constants.categoryActiveButton))
                    {
                        categoryButton[k].BackgroundImage = new Bitmap(img);
                    }
                }
                else
                {
                    if (categoryButton[k] != null)
                    {
                        categoryButton[k].ForeColor = ColorTranslator.FromHtml("#FFFFFF");
                        categoryButton[k].Font = new Font("Serif", 16, FontStyle.Bold);
                        using (Bitmap img = new Bitmap(constants.categoryButton))
                        {
                            categoryButton[k].BackgroundImage = new Bitmap(img);
                        }
                    }
                }
            }

            try
            {
                constants.SaveLogData("saleScreen_*", "saleScreen select category");
                Thread threadInput = new Thread(SelectedCategoryLoader);
                threadInput.Start();
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_*", ex.ToString());
                return;
            }
        }

        private void SelectedCategoryLoader()
        {
            SetSubLoading(true);

            if (selectedCategoryLayout == 13)
            {
                CreateProductsList13();
            }
            else if (selectedCategoryLayout == 11)
            {
                CreateProductsList11();
            }
            else
            {
                CreateProductsList();
            }
            //Thread.Sleep(1000);
            SetSubLoading(false);


        }

        private void SetSubLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        return;
                    });
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MainBodyPanelGlobal.Show();
                    });
                }
                else
                {
                    MainBodyPanelGlobal.Show();
                }
            }
        }

        private void CreateProductsList11()
        {
            if (MainBodyPanelGlobal.InvokeRequired)
            {
                MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                {
                    MainBodyPanelGlobal.Controls.Clear();
                });
            }
            else
            {
                MainBodyPanelGlobal.Controls.Clear();
            }

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

            p_ProductCon = new Panel[selectedCategoryLayout];
            curProduct = -1;
            pb_Image = new PictureBox[selectedCategoryLayout];

            sqlite_conn = CreateConnection(constants.dbName);

            for (int i = 0; i < selectedCategoryLayout; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdLimitedCnt = 0;
                int prdPrice = 0;
                string prdImageUrl = "";

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIndexGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    prdID = sqlite_datareader.GetInt32(0);
                    realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                    prdName = sqlite_datareader.GetString(3);
                    prdPrice = sqlite_datareader.GetInt32(8);
                    prdLimitedCnt = sqlite_datareader.GetInt32(9);
                    prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                }

                Panel p = new Panel();
                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    borderPen = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
                    BackgroundBitmap = null;
                    if (i == 0)
                    {
                        p = createPanel.CreatePanelForProducts(0, 0, nWidth, nHeight, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 1)
                    {
                        p = createPanel.CreatePanelForProducts(nWidth + 20, 0, nWidth2, nHeight2, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor3_Paint);
                    }
                    else if (i == 2 || i == 6 || i == 10)
                    {
                        int hh = 2 * h1 + 40;
                        if (i == 6) hh = 3 * h1 + 60;
                        if (i == 10) hh = 4 * h1 + 80;
                        p = createPanel.CreatePanelForProducts(nWidth + 20, hh, nWidth3, nHeight3, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor4_Paint);
                    }
                    else if (i < 6)
                    {
                        p = createPanel.CreatePanelForProducts((i - 3) * w1 + (i - 3) * 20, nHeight + 20, w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else
                    {
                        p = createPanel.CreatePanelForProducts((i - 7) * w1 + (i - 7) * 20, nHeight + h1 + 40, w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }

                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    using (BackgroundBitmap = new Bitmap(prdImageUrl))
                    {
                        pb_Image[i].Image = new Bitmap(BackgroundBitmap);
                    }
                    if (BackgroundBitmap != null)
                    {
                        BackgroundBitmap.Dispose();
                        BackgroundBitmap = null;
                    }


                    pb_Image[i].Click += new EventHandler(this.SelectProduct);

                    p.Controls.Add(pb_Image[i]);

                    if (MainBodyPanelGlobal.InvokeRequired)
                    {
                        MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                        {
                            MainBodyPanelGlobal.Controls.Add(p);
                        });
                    }
                    else
                    {
                        MainBodyPanelGlobal.Controls.Add(p);
                    }
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
            if (MainBodyPanelGlobal.InvokeRequired)
            {
                MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                {
                    MainBodyPanelGlobal.Controls.Clear();
                });
            }
            else
            {
                MainBodyPanelGlobal.Controls.Clear();
            }

            int w1 = (MainBodyPanelGlobal.Width - 100) / 5;
            int h1 = (MainBodyPanelGlobal.Height - 80) / 5;
            nWidth = 2 * w1 + 20;
            nHeight = 2 * h1 + 20;
            nHeight1 = h1;
            nWidth1 = w1;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            p_ProductCon = new Panel[selectedCategoryLayout];
            curProduct = -1;
            pb_Image = new PictureBox[selectedCategoryLayout];

            sqlite_conn = CreateConnection(constants.dbName);

            for (int i = 0; i < selectedCategoryLayout; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdLimitedCnt = 0;
                int prdPrice = 0;
                string prdImageUrl = "";

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIndexGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    prdID = sqlite_datareader.GetInt32(0);
                    realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                    prdName = sqlite_datareader.GetString(3);
                    prdPrice = sqlite_datareader.GetInt32(8);
                    prdLimitedCnt = sqlite_datareader.GetInt32(9);
                    prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                }

                Panel p = new Panel();
                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    borderPen = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
                    BackgroundBitmap = null;
                    if (i == 0 || i == 1)
                    {
                        p = createPanel.CreatePanelForProducts(i * (2 * w1 + 20) + i * 20, 0, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 2 || i == 3)
                    {
                        p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 2) * (h1 + 20), w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else if (i == 4 || i == 5)
                    {
                        p = createPanel.CreatePanelForProducts((i - 4) * (2 * w1 + 20) + (i - 4) * 20, 2 * h1 + 40, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                    }
                    else if (i == 6 || i == 7)
                    {
                        p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 4) * (h1 + 20), w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }
                    else
                    {
                        p = createPanel.CreatePanelForProducts((i - 8) * w1 + (i - 8) * 20, 2 * (2 * h1 + 40), w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                    }


                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    using (BackgroundBitmap = new Bitmap(prdImageUrl))
                    {
                        pb_Image[i].Image = new Bitmap(BackgroundBitmap);
                    }
                    if (BackgroundBitmap != null)
                    {
                        BackgroundBitmap.Dispose();
                        BackgroundBitmap = null;
                    }

                    pb_Image[i].Click += new EventHandler(this.SelectProduct);

                    p.Controls.Add(pb_Image[i]);
                    if (MainBodyPanelGlobal.InvokeRequired)
                    {
                        MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                        {
                            MainBodyPanelGlobal.Controls.Add(p);
                        });
                    }
                    else
                    {
                        MainBodyPanelGlobal.Controls.Add(p);
                    }
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
            if (MainBodyPanelGlobal.InvokeRequired)
            {
                MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                {
                    MainBodyPanelGlobal.Controls.Clear();
                });
            }
            else
            {
                MainBodyPanelGlobal.Controls.Clear();
            }

            int nWD = 0, nHD = 0;
            if (selectedCategoryLayout == 25 || selectedCategoryLayout == 16 || selectedCategoryLayout == 9 || selectedCategoryLayout == 4)
                nWD = nHD = (int)Math.Sqrt((double)selectedCategoryLayout);
            if (selectedCategoryLayout == 10) { nWD = 2; nHD = 5; }
            if (selectedCategoryLayout == 6) { nWD = 3; nHD = 2; }
            if (selectedCategoryLayout == 8) { nWD = 4; nHD = 2; }
            if (selectedCategoryLayout == 20) { nWD = 5; nHD = 4; }

            int w1 = (MainBodyPanelGlobal.Width - 20 * nWD) / nWD;
            int h1 = (MainBodyPanelGlobal.Height - 20 * (nHD - 1)) / nHD;
            if (selectedCategoryLayout == 10)
                w1 = (MainBodyPanelGlobal.Width - 50 * nWD) / nWD;

            nHeight1 = h1;
            nWidth1 = w1;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            p_ProductCon = new Panel[selectedCategoryLayout];
            curProduct = -1;
            pb_Image = new PictureBox[selectedCategoryLayout];

            sqlite_conn = CreateConnection(constants.dbName);

            for (int i = 0; i < selectedCategoryLayout; i++)
            {
                int prdID = 0;
                string prdName = "";
                int prdPrice = 0;
                string prdImageUrl = "";
                int prdLimitedCnt = 0;

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDList[categoryIndexGlobal]);
                sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    prdID = sqlite_datareader.GetInt32(0);
                    realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                    prdName = sqlite_datareader.GetString(3);
                    prdPrice = sqlite_datareader.GetInt32(8);
                    prdLimitedCnt = sqlite_datareader.GetInt32(9);
                    prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                }


                int x = i % nWD;
                int yy = i / nWD;
                int d = 20;
                if (selectedCategoryLayout == 10) d = 50;
                BackgroundBitmap = null;

                //MessageBox.Show(p.Height.ToString());

                if (prdImageUrl != "" && prdImageUrl != null)
                {
                    Panel p = createPanel.CreatePanelForProducts(x * (w1 + d), yy * (h1 + 20), w1, h1, i.ToString(), true, Color.Transparent, Color.White);
                    borderPen = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
                    p.Paint += new PaintEventHandler(Panelbordercolor_Paint);

                    pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);

                    using (BackgroundBitmap = new Bitmap(prdImageUrl))
                    {
                        pb_Image[i].Image = new Bitmap(BackgroundBitmap);
                    }
                    if (BackgroundBitmap != null)
                    {
                        BackgroundBitmap.Dispose();
                        BackgroundBitmap = null;
                    }

                    pb_Image[i].Click += new EventHandler(this.SelectProduct);
                    p.Controls.Add(pb_Image[i]);
                    if (MainBodyPanelGlobal.InvokeRequired)
                    {
                        MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                        {
                            MainBodyPanelGlobal.Controls.Add(p);
                        });
                    }
                    else
                    {
                        MainBodyPanelGlobal.Controls.Add(p);
                    }

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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth3, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight3);
                    g.DrawLine(pp, nWidth3 - 2, 0, nWidth3 - 2, nHeight3);
                    g.DrawLine(pp, 0, nHeight3 - 2, nWidth3, nHeight3 - 2);
                    g.Dispose();
                }
            }
        }

        private void PanelBorderPaint(object sender, PaintEventArgs e)
        {
            Panel tempPn = (Panel)sender;
            if (tempPn.BorderStyle == BorderStyle.FixedSingle)
            {
                int thickness = 3;//it's up to you
                int halfThickness = thickness / 2;
                using (Pen p = new Pen(Color.FromArgb(255, 40, 179, 81), thickness))
                {
                    e.Graphics.DrawRectangle(p, new Rectangle(halfThickness,
                                                              halfThickness,
                                                              tempPn.ClientSize.Width - thickness,
                                                              tempPn.ClientSize.Height - thickness));
                }
            }
        }

        /**
        * Drawing border color when clicking product item for ordering.
        **/
        private void ClickEvent(int index)
        {
            if (index == curProduct)
            {
                penClr = Color.FromArgb(255, 255, 0, 0);
                borderPen = new Pen(penClr, 3);
                p_ProductCon[curProduct].Invalidate(true);
            }
            else
            {
                int cur = curProduct == -1 ? 0 : curProduct;
                borderPen = new Pen(ColorTranslator.FromHtml("#FDD648"), 3);
                p_ProductCon[cur].Name = index.ToString();
                p_ProductCon[curProduct == -1 ? 0 : curProduct].Invalidate(true);

                curProduct = index;
                borderPen = new Pen(ColorTranslator.FromHtml("#FF0000"), 3);
                p_ProductCon[curProduct].Invalidate(true);
                p_ProductCon[cur].Name = cur.ToString();
            }
        }


        private void PriceChange(object sender, EventArgs e)
        {
            Label tempLb = (Label)sender;
            int prdId = int.Parse(tempLb.Name.Split('_')[1]);
            int prdPrice = int.Parse(tempLb.Name.Split('_')[2]);
            numberInput.CreateNumberInputDialog("productManagement", prdPrice, tempLb.Name);
        }

        AddMenuSetting addMenuSetting = null;
        private void ProductAdditionalMenuSetDialog(object sender, EventArgs e)
        {
            if(selectedProductIndex != -1)
            {
                addMenuSetting = new AddMenuSetting(prdIDGlobal);
                addMenuSetting.InitialProductInfo(this);
                addMenuSetting.ShowDialog();
            }
        }

        public void CloseAddmitionalMenuDialog()
        {
            if(addMenuSetting != null)
            {
                addMenuSetting.Close();
                addMenuSetting.Dispose();
                addMenuSetting = null;
            }
            optionPanel.Controls.Clear();
            LoadOptionList();
        }

        private void LoadOptionList()
        {
            if(sqlite_conn == null)
            {
                sqlite_conn = CreateConnection(constants.dbName);
            }
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd1 = null;

            try
            {
                sqlite_cmd1 = sqlite_conn.CreateCommand();
                string qry1 = "SELECT * FROM " + constants.tbNames[15] + " WHERE productID=@productID ORDER BY id";
                sqlite_cmd1.CommandText = qry1;
                sqlite_cmd1.Parameters.AddWithValue("@productID", prdIDGlobal);
                SQLiteDataReader sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                int k = 0;
                int currentY = 5;
                while (sqlite_datareader1.Read())
                {
                    if (!sqlite_datareader1.IsDBNull(0))
                    {
                        int optionTitleID = sqlite_datareader1.GetInt32(0);
                        string optionTitle = sqlite_datareader1.GetString(2);

                        Label optionTitleLb = createLabel.CreateLabelsInPanel(optionPanel, "optionTitleLb", optionTitle, 20, currentY, optionPanel.Width - 40, 25, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleLeft);
                        currentY += 30;
                        SQLiteCommand sqlite_cmd_sub = null;
                        sqlite_cmd_sub = sqlite_conn.CreateCommand();
                        string qry_sub = "SELECT * FROM " + constants.tbNames[16] + " WHERE productID=@productID AND optionTitle=@optionTitle ORDER BY id";
                        sqlite_cmd_sub.CommandText = qry_sub;
                        sqlite_cmd_sub.Parameters.AddWithValue("@productID", prdIDGlobal);
                        sqlite_cmd_sub.Parameters.AddWithValue("@optionTitle", optionTitleID);
                        SQLiteDataReader sqlite_datareader_sub = sqlite_cmd_sub.ExecuteReader();
                        int m = 0;
                        while (sqlite_datareader_sub.Read())
                        {
                            if (!sqlite_datareader_sub.IsDBNull(0))
                            {
                                int optionValueID = sqlite_datareader_sub.GetInt32(0);
                                string optionValue = sqlite_datareader_sub.GetString(3);
                                if (m > 0 && m % 3 == 0)
                                {
                                    currentY += 25;
                                    m = 0;
                                }
                                Label optionValueLb = createLabel.CreateLabelsInPanel(optionPanel, "optionValueLb", optionValue, 20 + m * (optionPanel.Width - 40) / 3, currentY, (optionPanel.Width - 40) / 3, 20, Color.White, Color.Black, 12, false, ContentAlignment.MiddleLeft);
                                m++;
                            }
                        }
                        sqlite_datareader_sub.Close();
                        sqlite_datareader_sub = null;
                        sqlite_cmd_sub.Dispose();
                        sqlite_cmd_sub = null;
                        currentY += 30;
                        k++;
                    }
                }

                sqlite_datareader1.Close();
                sqlite_datareader1 = null;
                sqlite_cmd1.Dispose();
                sqlite_cmd1 = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                if (sqlite_cmd1 != null)
                {
                    sqlite_cmd1.Dispose();
                    sqlite_cmd1 = null;
                }
                sqlite_conn.Dispose();
                sqlite_conn = null;
                Console.WriteLine(ex.ToString());
                return;
            }

        }

        private void SelectProduct(object sender, EventArgs e)
        {
            PictureBox pt = (PictureBox)sender;
            int index = int.Parse(pt.Name);
            selectedProductIndex = index;
            dayTimeG = "00:00-24:00";
            bLoad = true;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE ProductID=@productID";
            sqlite_cmd.CommandText = queryCmd;
            sqlite_cmd.Parameters.AddWithValue("@productID", realProductIDArray[index]);

            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    prdIDGlobal = sqlite_datareader.GetInt32(2);
                    prdNameGlobal = sqlite_datareader.GetString(3);
                    prdPrintNameGlobal = sqlite_datareader.GetString(4);
                    dayTimeG = sqlite_datareader.GetString(5);
                    prdPriceGlobal = sqlite_datareader.GetInt32(8).ToString(); ;
                    prdImgUrlGlobal = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                    prdOriginImgUrlGlobal = Path.Combine(currentDir, sqlite_datareader.GetString(11));
                    cardNumberGlobal = sqlite_datareader.GetInt32(14);
                    nameRC = sqlite_datareader.GetString(29);
                    priceRC = sqlite_datareader.GetString(30);
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;


            prdNameTxtBx.Text = prdNameGlobal;
            prdPrintNameTxtBx.Text = prdPrintNameGlobal;
            prdPriceTxtBx.Text = prdPriceGlobal;
            ClickEvent(index);

       }

        public void NumberKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public void SetSaleTime(object sender, EventArgs e)
        {
            if(selectedProductIndex != -1)
            {
                string startH = startHourTxtBx.Text;
                string startM = startMinuteTxtBx.Text;
                string endH = endHourTxtBx.Text;
                string endM = endMinuteTxtBx.Text;
                detailView.SaleTimeSet(startH, startM, endH, endM);
            }
        }

        private void TimeFlagSetting(object sender, EventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            if(rd.Name == "r1")
            {
                timeSettingPanel.Hide();
                dayTimeG = "00:00-24:00";
            }
            else
            {
                timeSettingPanel.Show();
            }
        }

        string dayTimeG = "00:00-24:00";

        public void SetTimeValue(string timeValue)
        {
            string[] tempTime = timeValue.Split('_');
            startHourTxtBx.Text = tempTime[0];
            startMinuteTxtBx.Text = tempTime[1];
            endHourTxtBx.Text = tempTime[2];
            endMinuteTxtBx.Text = tempTime[3];
        }

        private void SetProductTimeChange(object sender, EventArgs e)
        {
            if(selectedProductIndex != -1)
            {
                if (r1Global.Checked)
                {
                    dayTimeG = "00:00-24:00";
                }
                else
                {
                    dayTimeG = startHourTxtBx.Text + ":" + startMinuteTxtBx.Text + "-" + endHourTxtBx.Text + ":" + endMinuteTxtBx.Text;
                }
                sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }

                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "UPDATE " + constants.tbNames[2] + " SET DayTime=@dayTime WHERE ProductID=@productID";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@dayTime", dayTimeG);
                sqlite_cmd.Parameters.AddWithValue("@productID", prdIDGlobal);
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
            }
        }

        private void SetProductChange(object sender, EventArgs e)
        {
            if(selectedProductIndex != -1)
            {
                prdNameGlobal = prdNameTxtBx.Text;
                prdPrintNameGlobal = prdPrintNameTxtBx.Text;
                prdPriceGlobal = prdPriceTxtBx.Text;
                sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }

                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "UPDATE " + constants.tbNames[2] + " SET ProductName=@prdName, PrintName=@prdPrintName, ProductPrice=@prdPrice WHERE ProductID=@productID";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@prdName", prdNameGlobal);
                sqlite_cmd.Parameters.AddWithValue("@prdPrintName", prdPrintNameGlobal);
                sqlite_cmd.Parameters.AddWithValue("@prdPrice", int.Parse(prdPriceGlobal));
                sqlite_cmd.Parameters.AddWithValue("@productID", prdIDGlobal);
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.Dispose();
                sqlite_cmd = null;

                CreateDefaultImageUrl(selectedCategoryLayout, cardNumberGlobal);
            }
        }

        double orgTextRate;
        int deltaH, fontSize;
        Rectangle curCanvas = new Rectangle(0, 0, 0, 0);
        private void CreateDefaultImageUrl(int layout, int index)
        {
            Bitmap bitmapImage, textImage, priceImage;
            Rectangle[] myRect = new Rectangle[7];

            Rectangle canvas = new Rectangle(0, 0, 400, 400);
            int cardW = 0, cardH = 0;
            if (layout == 10)
            {
                cardW = 291; cardH = 108;
            }
            else if (layout == 8)
            {
                cardW = 145; cardH = 304;
            }
            else if (layout == 13)
            {
                cardW = 244; cardH = 238;
            }
            else if (layout == 25)
            {
                cardW = 112; cardH = 108;
            }
            else if (layout == 16)
            {
                cardW = 145; cardH = 141;
            }
            else if (layout == 9)
            {
                cardW = 201; cardH = 196;
            }
            else if (layout == 4)
            {
                cardW = 311; cardH = 304;
            }
            else if (layout == 11)
            {
                cardW = 356; cardH = 320;
            }
            else if (layout == 6)
            {
                cardW = 201; cardH = 304;
            }
            else if (layout == 20)
            {
                cardW = 112; cardH = 141;
            }

            double rt = Convert.ToDouble(cardW) / Convert.ToDouble(cardH);
            if (rt < 1)
            {
                canvas.Width = Convert.ToInt32(canvas.Width * rt);
            }
            else
            {
                canvas.Height = Convert.ToInt32(canvas.Height / rt);
            }

            deltaH = canvas.Height / 8;
            if (2 * canvas.Width / 3 >= canvas.Height)
            {
                fontSize = 4 * deltaH / 3;
                myRect[0] = new Rectangle(0, 0, 2 * canvas.Width / 3, canvas.Height);
                myRect[1] = new Rectangle((int)(canvas.Width / 2.7), 2 * deltaH, canvas.Width / 6, canvas.Height / 4);
                myRect[2] = new Rectangle(2 * canvas.Width / 3, (int)(canvas.Height / 1.4), canvas.Width / 3, canvas.Height / 2);

                myRect[4] = new Rectangle(2 * canvas.Width / 3, 2 * deltaH, canvas.Width / 3, canvas.Height - 2 * deltaH);
                myRect[5] = new Rectangle(2 * canvas.Width / 3, 0, canvas.Width / 3, 2 * deltaH);
                myRect[6] = new Rectangle(2 * canvas.Width / 3, 0, canvas.Width / 3, 2 * deltaH);
            }
            else
            {
                fontSize = 2 * deltaH / 3;
                myRect[0] = new Rectangle(0, deltaH, canvas.Width, 6 * deltaH);
                myRect[1] = new Rectangle((int)(canvas.Width / 1.8), 2 * deltaH, canvas.Width / 4, canvas.Height / 4);
                myRect[2] = new Rectangle(0, (int)(canvas.Height / 1.4), canvas.Width, canvas.Height / 2);

                myRect[4] = new Rectangle(0, 6 * deltaH, canvas.Width, canvas.Height - 6 * deltaH);
                myRect[5] = new Rectangle(0, 0, canvas.Width, deltaH);
                myRect[6] = new Rectangle(0, 0, canvas.Width, deltaH);
            }

            if (layout == 10)
            {
                fontSize = 4 * deltaH / 3;

                myRect[0] = new Rectangle(0, 0, canvas.Width / 2, canvas.Height);
                myRect[1] = new Rectangle((int)(canvas.Width / 4), 2 * deltaH, canvas.Width / 6, canvas.Height / 4);
                myRect[2] = new Rectangle(canvas.Width / 2, (int)(canvas.Height / 1.4), canvas.Width / 2, canvas.Height / 2);

                myRect[4] = new Rectangle(canvas.Width / 2, 2 * deltaH, canvas.Width / 2, canvas.Height - 2 * deltaH);
                myRect[5] = new Rectangle(canvas.Width / 2, 0, canvas.Width / 2, 2 * deltaH);
                myRect[6] = new Rectangle(canvas.Width / 2, 0, canvas.Width / 2, 2 * deltaH);

            }

            curCanvas = canvas;
            string bttextTempClr = bottomTextColor == "" || bottomTextColor == null ? btTxtClr[categoryIndexGlobal] : bottomTextColor;
            string tptextTempClr = topTextColor == "" || topTextColor == null ? topTxtClr[categoryIndexGlobal] : topTextColor;

            textImage = readyTextBitmap(prdNameGlobal, bttextTempClr, layout, index);
            orgTextRate = (double)textImage.Height / textImage.Width;
            priceImage = readyTextBitmap(prdPriceGlobal, bttextTempClr, layout, index);

            //            orgPriceWidth = priceImage.Width;
            if (2 * canvas.Width / 3 >= canvas.Height)
            {
                myRect[2] = new Rectangle(2 * canvas.Width / 3, 4 * deltaH, textImage.Width, textImage.Height);
                myRect[3] = new Rectangle(2 * canvas.Width / 3, 6 * deltaH, priceImage.Width, priceImage.Height);
            }
            else
            {
                myRect[2] = new Rectangle(2, 6 * deltaH, textImage.Width, textImage.Height);
                myRect[3] = new Rectangle((canvas.Width - priceImage.Width - 5), 7 * deltaH, priceImage.Width, priceImage.Height);
            }

            if (layout == 11)
            {
                Rectangle canvas1 = new Rectangle(0, 0, 400, 400);

                if (index == 2 || index == 6 || index == 10)
                {

                    int cardW1 = 291, cardH1 = 108;

                    double rt1 = Convert.ToDouble(cardW1) / Convert.ToDouble(cardH1);
                    if (rt1 < 1)
                    {
                        canvas1.Width = Convert.ToInt32(canvas1.Width * rt1);
                    }
                    else
                    {
                        canvas1.Height = Convert.ToInt32(canvas1.Height / rt1);
                    }

                    curCanvas = canvas1;

                    int deltaH1 = canvas1.Height / 8;
                    string bttextTempClr1 = bottomTextColor == "" || bottomTextColor == null ? btTxtClr[categoryIndexGlobal] : bottomTextColor;
                    string tptextTempClr1 = topTextColor == "" || topTextColor == null ? topTxtClr[categoryIndexGlobal] : topTextColor;


                    textImage = readyTextBitmap(prdNameGlobal, bttextTempClr1, layout, index);
                    orgTextRate = (double)textImage.Height / textImage.Width;
                    priceImage = readyTextBitmap(prdPriceGlobal, bttextTempClr1, layout, index);

                    myRect[0] = new Rectangle(0, 0, canvas1.Width / 2, canvas1.Height);
                    myRect[1] = new Rectangle((int)(canvas1.Width / 4), 2 * deltaH1, canvas1.Width / 6, canvas1.Height / 4);

                    myRect[4] = new Rectangle(canvas1.Width / 2, 2 * deltaH1, canvas1.Width / 2, canvas1.Height - 2 * deltaH1);
                    myRect[5] = new Rectangle(canvas1.Width / 2, 0, canvas1.Width / 2, 2 * deltaH1);

                    myRect[2] = new Rectangle(canvas1.Width / 2, 2 * deltaH1, textImage.Width, textImage.Height);
                    myRect[3] = new Rectangle(canvas1.Width - priceImage.Width - 10, 6 * deltaH1, priceImage.Width, priceImage.Height);
                }
                else
                {
                    cardW = 356; cardH = 320;
                    rt = Convert.ToDouble(cardW) / Convert.ToDouble(cardH);
                    if (rt < 1)
                    {
                        canvas.Width = Convert.ToInt32(canvas.Width * rt);
                    }
                    else
                    {
                        canvas.Height = Convert.ToInt32(canvas.Height / rt);
                    }
                    curCanvas = canvas;
                    deltaH = canvas.Height / 8;

                    fontSize = 2 * deltaH / 3;
                    myRect[0] = new Rectangle(0, deltaH, canvas.Width, 6 * deltaH);
                    myRect[1] = new Rectangle((int)(canvas.Width / 1.8), 2 * deltaH, canvas.Width / 4, canvas.Height / 4);
                    myRect[2] = new Rectangle(0, (int)(canvas.Height / 1.4), canvas.Width, canvas.Height / 2);

                    myRect[4] = new Rectangle(0, 6 * deltaH, canvas.Width, canvas.Height - 6 * deltaH);
                    myRect[5] = new Rectangle(0, 0, canvas.Width, deltaH);

                }
            }


            if (layout == 10)
            {
                myRect[2] = new Rectangle(canvas.Width / 2, 2 * deltaH, textImage.Width, textImage.Height);
                myRect[3] = new Rectangle(canvas.Width - priceImage.Width - 10, 6 * deltaH, priceImage.Width, priceImage.Height);
            }

            if (layout == 11 && (index != 2 && index != 6 && index != 10))
            {
                myRect[2] = new Rectangle(2, 6 * deltaH, textImage.Width, textImage.Height);
                myRect[3] = new Rectangle((canvas.Width - priceImage.Width - 5), 7 * deltaH, priceImage.Width, priceImage.Height);
            }

            bitmapImage = new Bitmap(curCanvas.Width, curCanvas.Height);
            Graphics gr = Graphics.FromImage(bitmapImage);

            //------------  Draw product image
            int w = 0, h = 0;
            Image image = null;
            Bitmap mainImage = null;
            using (mainImage = new Bitmap(prdImgUrlGlobal))
            {
                w = mainImage.Width;
                h = mainImage.Height;
                image = new Bitmap(mainImage);
            }
            if (mainImage != null)
            {
                mainImage.Dispose();
                mainImage = null;
            }

            if (w == 0 || h == 0) return;

            //double ratio = (Convert.ToDouble(myRect[0].Height) / Convert.ToDouble(myRect[0].Width));
            //double ratio2 = (Convert.ToDouble(h) / Convert.ToDouble(w));

            //int newW = Convert.ToInt32(h / ratio);
            //if (w > newW)
            //{
            //    left = (w - newW) / 2;
            //    w = newW;
            //}
            //else
            //{
            //    int temp_h = h;
            //    h = Convert.ToInt32(w * ratio2);
            //    top = (temp_h - h) / 2;
            //}

            Rectangle srcRC = new Rectangle(0, 0, w, h);
            GraphicsUnit units = GraphicsUnit.Pixel;

            gr.DrawImage(image, curCanvas, srcRC, units);


            Bitmap bottomBKImg = getColorImage(myRect[4], ColorTranslator.FromHtml(bottomBKColor == "" || bottomBKColor == null ? btBackClr[categoryIndexGlobal] : bottomBKColor));
            gr.DrawImage(bottomBKImg, myRect[4]);

            //---------------   Draw text Image
            if (textImage != null)
                gr.DrawImage(textImage, formatRectangle(nameRC).Width == 0 ? myRect[2] : getRealRC(formatRectangle(nameRC)));
            if (priceImage != null)
                gr.DrawImage(priceImage, formatRectangle(priceRC).Width == 0 ? myRect[3] : getRealRC(formatRectangle(priceRC)));


            // tempBmp[i] = bitmapImage;
            string settingImgUrl = currentDir + "images\\completeImg-" + categoryIDGlobal.ToString() + "-" + cardNumberGlobal.ToString() + ".png";
            string validimgUrl = "images\\completeImg" + "-" + categoryIDGlobal.ToString() + "-" + cardNumberGlobal.ToString() + ".png";
            if (File.Exists(settingImgUrl))
            {
                File.Delete(settingImgUrl);
            }
            bitmapImage.Save(settingImgUrl, System.Drawing.Imaging.ImageFormat.Png);

            using (BackgroundBitmap = new Bitmap(settingImgUrl))
            {
                pb_Image[selectedProductIndex].Image = new Bitmap(BackgroundBitmap);
            }
            if (BackgroundBitmap != null)
            {
                BackgroundBitmap.Dispose();
                BackgroundBitmap = null;
            }
        }

        private Rectangle getRealRC(Rectangle rc)
        {
            int nW = curCanvas.Width;
            int nH = curCanvas.Height;
            Rectangle retRC = new Rectangle(rc.Left * nW / 100, rc.Top * nH / 100, rc.Width * nW / 100, rc.Height * nH / 100);
            return retRC;
        }

        private Rectangle formatRectangle(string str)
        {
            Rectangle ret = new Rectangle(0, 0, 0, 0);
            string[] arr = new string[] { };

            if (str != null)
            {
                arr = str.Split(':');
                ret.X = int.Parse(arr[0]);
                ret.Y = int.Parse(arr[1]);
                ret.Width = int.Parse(arr[2]);
                ret.Height = int.Parse(arr[3]);
            }
            return ret;
        }

        private Bitmap readyTextBitmap(string str, string strClr, int layout, int i)
        {
            int strLen = str.Length + 4;
            int ft = fontSize;
            if (layout == 10 || (layout == 11 && (i == 2 || i == 6 || i == 10)))
            {
                if (strLen * fontSize > curCanvas.Width / 2)
                    ft = (int)(curCanvas.Width) / (strLen * 2) - 1;
            }
            else
            {
                if (strLen * fontSize > curCanvas.Width)
                    ft = (int)(curCanvas.Width) / strLen - 1;
            }

            Font myFont = new Font("sans-serif", ft);
            Bitmap result = new Bitmap(400, 300);
            Graphics gr = Graphics.FromImage(result);
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Near;
            format.Alignment = StringAlignment.Near;

            GraphicsPath p = new GraphicsPath();
            p.AddString(str,             // text to draw
                myFont.FontFamily,  // or any other font family
                (int)FontStyle.Regular,      // font style (bold, italic, etc.)
                gr.DpiY * myFont.Size / 72,       // em size
                new Point(0, 0),              // location where to draw text
                format);          // set options here (e.g. center alignment)

            RectangleF boundRect = p.GetBounds();
            boundRect = new RectangleF(0, 0, boundRect.Width + boundRect.Left * 2, boundRect.Height + boundRect.Top * 2);

            format.LineAlignment = StringAlignment.Near;
            format.Alignment = StringAlignment.Center;
            gr.FillPath(new SolidBrush(ColorTranslator.FromHtml(strClr)), p);


            Bitmap targetImage = (int)boundRect.Width == 0 || (int)boundRect.Height == 0 ? new Bitmap(10, 10) : new Bitmap((int)boundRect.Width, (int)boundRect.Height);
            using (Graphics g = Graphics.FromImage(targetImage))
            {
                g.DrawImage(result, new Rectangle(0, 0, result.Width, result.Height));
            }
            return targetImage;
        }

        private Bitmap getColorImage(Rectangle rc, Color col)
        {
            Bitmap result = new Bitmap(rc.Width, rc.Height);
            Graphics gr = Graphics.FromImage(result);
            gr.FillRectangle(new SolidBrush(col), 0, 0, result.Width, result.Height);
            return result;
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

