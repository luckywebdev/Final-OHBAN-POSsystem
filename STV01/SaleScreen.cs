using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace STV01
{
    public partial class SaleScreen : Form
    {

        //  Form FormPanel = null;
        ComModule comModule = new ComModule();

        public Form1 mainFormGlobal = null;
        Panel LeftPanelGlobal = null;
        Panel LeftSubPanel = null;
        Panel RightPanelGlobal = null;
        Panel MainBodyPanelGlobal = null;
        Panel categoryPanel = null;
        Panel mainPanelGlobal = null;
        Panel mainPanelGlobal2 = null;
        Panel ScreenMsgPanelGlobal = null;
        Panel downPanel = null;
        public Label StatusMsgLabel = null;

        Constant constants = new Constant();
        CustomButton customButton = new CustomButton();
        MessageDialog messageDialog = new MessageDialog();
        DBClass dbClass = new DBClass();
        PasswordInput passwordInput = new PasswordInput();
        BackgroundWorker backgroundWorker = null;


        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        OrderDialog orderDialog = new OrderDialog();
        Panel TableBackPanelGlobal = null;
        Label TempLabel = null;

        public int orderTotalPrice = 0;
        public int orderEnPrice = 0;
        public int currentSelectedId = 0;

        private int selectedCategoryIndex = -1;
        private int selectedCategoryID = 0;
        private int selectedCategoryLayout = 0;

        private Label[] orderProductNameLabel = new Label[10];
        private Label[] orderAmountLabel = new Label[10];
        private Button[] orderDeleteLabel = new Button[10];
        private Button[] orderIncreaseButton = new Button[10];
        private Button[] orderDecreaseButton = new Button[10];
        private Button[] categoryButton = new Button[10];
        private Button ticketingButtonGlobal = null;
        private Button receiptButtonGlobal = null;
        private Button orderCancelButtonGlobal = null;

        private Button banknotGlobalLabel = null;
        private Label orderAmountTotalLabel = null;
        private Label orderPriceTotalLabel = null;
        private Label orderPriceEnterLabel = null;
        private Label orderRestPriceLabel = null;

        private int orderTotalAmount = 0;

        private string[] orderProductNameArray = new string[8];
        private string[] orderPrintNameArray = new string[8];
        private string[] orderAmountArray = new string[8];
        private string[] orderProductPriceArray = new string[8];
        private int[] orderProductIDArray = new int[8];
        private int[] orderRealProductIDArray = new int[8];
        private int[] orderProuctIndexArray = new int[8];
        private int[] orderCategoryIDArray = new int[8];
        private int[] orderCategoryIndexArray = new int[8];
        List<string>[] orderProductOptionArray = new List<string>[8];

        private int[,] productRestAmountArray = new int[30, 50];
        private int selectedPrdIndex = 0;
        private int[,] productLimitedCntArray = new int[30, 50];
        private int[] productIDArray = new int[50];
        private int[] realProductIDArray = new int[50];
        private int[] categoryIDArray = new int[50];
        private string[] productTimeArray = new string[50];
        private string[] categoryBackImageArray = new string[50];
        private string[] categoryBackColor = new string[10];
        private string[] categoryTextColor = new string[10];
        private string[] screenMsgBackColor = new string[10];
        private string[] screenMsgTextColor = new string[10];
        private int categoryDisAmount = 0;
        private int callNumber = 0;
        private int callNumbers = 0;

        public string formType = "";
        
        private Bitmap BackgroundBitmap = null;
        Color borderClr = Color.FromArgb(255, 23, 55, 94);
        Pen borderPen = null;
        Color penClr = Color.FromArgb(255, 23, 55, 94);
        int nWidth = 0, nHeight = 0;
        int nWidth1 = 0, nHeight1 = 0;
        int nWidth2 = 0, nHeight2 = 0;
        int nWidth3 = 0, nHeight3 = 0;
        Panel[] p_ProductCon = null;
        PictureBox[] pb_Image = new PictureBox[25] ;
        string[] p_url = null;
        string[] bl_Name = null;
        string[] bl_PrintName = null;
        string[] bl_Price = null;
        bool bLoad = false;
        int curProduct = -1;
        int currentSerialNo = 1;
        int currentTicketNo = 1;
        int orderTotalTicketForReceipt = 0;
        int orderTotalPriceForReceipt = 0;
        string receiptPrinter = "";

        int PurchaseType = 0;   //if it set 1, then multiple tickets, and if it set 0, then single ticket
        int ReturnTime = 30;    //return time after payment. 0=>auto refund, if nothing is done after payment, time until automatic return 0-255 seconds.
        int MultiPurchase = 1;  //1=> the multiple purchase button is valid, 0=> invalid
        public int PurchaseAmount = 10; //the maximum number of pieces that can be purchased (2 to 10 Initial value is 10)
        int SerialNo = 1;   //1=>print the serial number, 0=>don't print the serial number
        int TelPrefix = 1;
        int StartSerialNo = 1;  //specity the start serial number
        int NoAfterTight = 1;   //1=>Initial value, 0=>continue number
        int FontSize = 1;   //1=>small, 0=>big
        int ValidDate = 0;//specify the valid date range of the product
        string TicketMsg1 = "";
        string TicketMsg2 = "";

        string ReceiptValid = "true";   //true=> valid, false => invalid
        string TicketTime = "10";       //ticketing time
        string StoreName = "";
        string Address = "";
        string PhoneNumber = "";
        string Other1 = "";
        string Other2 = "";

        string StoreRealName = "";

        int lineNumber = 0;
        int lineNumbers = 0;
        public bool dataEmpty = false;

        string currentDir = "";

        RegistryKey key = null;

        string ipAddress1 = "192.168.1.250";
        string port1 = "9100";
        string ipAddress2 = "";
        string port2 = "";
        string ipAddress3 = "";
        string port3 = "";
        string ipAddress = "192.168.1.250";
        string port = "9100";

        string restartFlag = "restart";
        DateTime restartTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), 23, 59, 00);

        int height = 1024;
        int width = 1280;
        public Thread sumThread = null;
        int touch_key = 0;
        bool backshowEnable = true;
        MainMenu pMm = null;
        private bool bOrderCancel = false;
        private bool callNumbering = true;
        private bool HDCheck = true;
        private bool highBank = true;
        private bool categoryMain = false;

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

        public SaleScreen(Form1 mainFrom, MainMenu mm, Panel panel, ComModule cm)
        {
            mainFormGlobal = mainFrom;
            pMm = mm;
            mainPanelGlobal = panel;
            mainPanelGlobal2 = mainFrom.mainPanelGlobal_2;
            mainPanelGlobal2.Visible = false;
            orderDialog.InitValue(this);
            messageDialog.InitSaleScreen(this);
            InitIntArray(productRestAmountArray);
            passwordInput.InitSaleScreen(this);

            mainFrom.KeyPreview = true;
            mainFrom.KeyDown += new KeyEventHandler(FormKeyReturnEventHandler);

            //currentDir = Directory.GetCurrentDirectory();
            currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDir += "\\STV01\\";
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }

            dbClass.CreateSaleTB();
            dbClass.CreateProductOptionTB();
            dbClass.CreateProductOptionValueTB();

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            try
            {
                string selectTicketSql = "SELECT * FROM " + constants.tbNames[4];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectTicketSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        PurchaseType = sqlite_datareader.GetInt32(0);
                        ReturnTime = sqlite_datareader.GetInt32(1);
                        MultiPurchase = sqlite_datareader.GetInt32(2);
                        PurchaseAmount = sqlite_datareader.GetInt32(3);
                        TelPrefix = sqlite_datareader.GetInt32(4);
                        SerialNo = sqlite_datareader.GetInt32(5);
                        StartSerialNo = sqlite_datareader.GetInt32(6);
                        NoAfterTight = sqlite_datareader.GetInt32(7);
                        FontSize = sqlite_datareader.GetInt32(8);
                        ValidDate = sqlite_datareader.GetInt32(9);
                        TicketMsg1 = sqlite_datareader.GetString(10);
                        TicketMsg2 = sqlite_datareader.GetString(11);
                    }
                }

                string selectReceiptSql = "SELECT * FROM " + constants.tbNames[5];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectReceiptSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        ReceiptValid = sqlite_datareader.GetString(0);
                        TicketTime = sqlite_datareader.GetString(1);
                        StoreName = sqlite_datareader.GetString(2);
                        Address = sqlite_datareader.GetString(3);
                        PhoneNumber = sqlite_datareader.GetString(4);
                        Other1 = sqlite_datareader.GetString(5);
                        Other2 = sqlite_datareader.GetString(6);
                    }
                }

                string selectStoreSql = "SELECT * FROM " + constants.tbNames[6];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectStoreSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        StoreRealName = sqlite_datareader.GetString(0);
                    }
                }

                currentTicketNo = 1;
                currentSerialNo = StartSerialNo;
                key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
                if (key != null)
                {
                    if (key.GetValue("ipAddress1") != null)
                    {
                        ipAddress1 = Convert.ToString(key.GetValue("ipAddress1"));
                        ipAddress = ipAddress1;
                    }
                    if (key.GetValue("port1") != null)
                    {
                        port1 = Convert.ToString(key.GetValue("port1"));
                    }

                    if (key.GetValue("ipAddress2") != null)
                    {
                        ipAddress2 = Convert.ToString(key.GetValue("ipAddress2"));
                    }
                    if (key.GetValue("port2") != null)
                    {
                        port2 = Convert.ToString(key.GetValue("port2"));
                    }

                    if (key.GetValue("ipAddress3") != null)
                    {
                        ipAddress3 = Convert.ToString(key.GetValue("ipAddress3"));
                    }
                    if (key.GetValue("port3") != null)
                    {
                        port3 = Convert.ToString(key.GetValue("port3"));
                    }

                    if (key.GetValue("callNumbering") != null)
                    {
                        callNumbering = Convert.ToBoolean(key.GetValue("callNumbering"));
                    }

                    if (key.GetValue("timeoutValue") == null)
                    {
                        key.SetValue("timeoutValue", 3);
                    }

                    if (key.GetValue("HDCheck") == null)
                    {
                        key.SetValue("HDCheck", true);
                    }
                    else
                    {
                        HDCheck = Convert.ToBoolean(key.GetValue("HDCheck"));
                    }
                    if (key.GetValue("HighBankNote") != null)
                    {
                        highBank = Convert.ToBoolean(key.GetValue("HighBankNote"));
                    }
                    if (key.GetValue("categoryMain") != null)
                    {
                        categoryMain = Convert.ToBoolean(key.GetValue("categoryMain"));
                    }                    
                    if (key.GetValue("restartFlag") != null)
                    {
                        restartFlag = Convert.ToString(key.GetValue("restartFlag"));
                    }
                }

                /** 2020-09-01 stopped */
                string queryCmd = "SELECT * FROM " + constants.tbNames[0] + " ORDER BY id";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = queryCmd;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (sqlite_datareader.GetInt32(10) != 1 && selectedCategoryIndex == -1)
                    {
                        selectedCategoryIndex = categoryDisAmount;
                    }
                    categoryDisAmount++;
                }

                int k = 0;
                string selectGeneralSql = "SELECT * FROM " + constants.tbNames[12];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = selectGeneralSql;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0) && k < 10)
                    {
                        categoryBackColor[k] = sqlite_datareader.GetString(0);
                        categoryTextColor[k] = sqlite_datareader.GetString(1);
                        screenMsgBackColor[k] = sqlite_datareader.GetString(2);
                        screenMsgTextColor[k] = sqlite_datareader.GetString(3);
                        k++;
                    }
                }
                currentTime = DateTime.Now.ToString("HH:mm:ss");
                dbClass.InsertLog(5, "販売開始", "販売時間： " + currentTime);  //currentSaleTime

                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_0_1", "" + ex.ToString());
                string errorMsg1 = "データ読み込みエラー";
                string errorMsg2 = "データベースのチェックに失敗しました。\n「メニュー読込」から設定データをインポートしてください。";  // "Please go to Menu Reading section and get loading data.";
                dbClass.InsertLog(4, "システムエラー", "データの読み込みに失敗しました。");
                string audio_file_name = dbClass.AudioFile("ErrorOccur");
                this.PlaySound(audio_file_name);
                messageDialog.ShowOtherErrorMessage(errorMsg1, errorMsg2);
                this.BackShow();
                return;
            }
            constants.SaveLogData("saleScreen_0", "salePage success");

            if ( sqlite_conn != null)
            {
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }

            try
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork_loading);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted_loading);
                backgroundWorker.RunWorkerAsync();
                this.LoadingForm();
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_0_2", "" + ex.ToString());
                dbClass.InsertLog(4, "システムエラー", "データの読み込みに失敗しました。");
                return;
            }

            //}

            //messageDialog.ShowErrorMessage("111",  "\n ErrorNo: 004");
        }


        private void ComModule_start()
        {
            if( mainFormGlobal.comport == "")
            {
                bBackShow = true;
                BackShow();
                return;
            }
            try
            {
                if (comModule == null)
                    comModule = new ComModule();

                if (comModule.InitMicroCom(mainFormGlobal))
                {
                    Binddata();
                }
                else
                    ErrorStatusOfImpoossible();

                if (comModule.getDepositTh == null)
                {
                    bBackShow = true;
                    BackShow();
                    return;
                }

            }
            catch (Exception ex)
            {
                dbClass.InsertLog(4, "システムエラー", "データの読み込みに失敗しました。", ex.ToString());
                constants.SaveLogData("saleScreen_0_3", "" + ex.ToString());
                if (comModule != null)
                {
                    comModule.CloseComport();
                    comModule = null;
                }
                bBackShow = true;
                BackShow();
            }
        }

        public bool isError = true;

        Stopwatch sw = null;

        private void Panel_Click(object sender, EventArgs e)
        {
            if (HDCheck && comModule == null)
                return;
            if(HDCheck && (messageDialog.DialogFormGlobal != null || isError ))
                return;

            Button lbTemp = (Button)sender;
            lbTemp.FlatStyle = FlatStyle.Popup;
            if (backshowEnable)
            {
                if (sw == null)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }
                if (sw.ElapsedMilliseconds > 2000)
                {
                    sw.Stop();
                    sw = null;
                    touch_key = 0;
                }
                else
                {
                    string soundPlay = @"resources\\btn_beep.wav";
                    this.PlaySound(soundPlay);
                    if (touch_key < 4)
                    {
                        touch_key++;
                    }
                    else
                    {
                        touch_key++;
                        sw.Stop();
                        sw = null;
                        bBackShow = true;
                        this.BackShow();
                    }
                }
            }
        }


        Form LoadingFormG = null;

        private void LoadingForm()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width, height);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Maximized;
            dialogForm.ControlBox = false;
            //dialogForm.TopLevel = true;
            //dialogForm.TopMost = true;

            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.BackColor = Color.LightSkyBlue;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "loadingMessageLabel0", "ローディング中...", 50, 0, mainPanel.Width - 100, mainPanel.Height - 50, Color.Transparent, Color.Black, 20, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);

            LoadingFormG = dialogForm;


            dialogForm.ShowDialog();
        } 

        private void BackgroundWorker_DoWork_loading(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.DisplayData();
                    });
                }
                else
                {
                    this.DisplayData();
                }
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_2", ex.ToString());
                return;
            }
            //Thread.Sleep(1000);
        }

        private void BackgroundWorker_RunWorkerCompleted_loading(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //MessageBox.Show("background worker is cancelled!");
                bBackShow = true;
                this.BackShow();
            }
            else if (e.Error != null)
            {
                //MessageBox.Show(e.Error.Message);
                bBackShow = true;
                this.BackShow();
            }
            else
            {
                mainPanelGlobal2.Visible = true;
                LoadingFormG.Close();
                //LoadingFormG.Visible = false;
                LoadingFormG = null;
                if (!backgroundWorker.IsBusy)
                    backgroundWorker.CancelAsync();
                
                Thread.Sleep(2000);
                if (HDCheck)
                    ComModule_start();
                /** 2020-09-01 stopped */
                sumThread = new Thread(SumThreadRunings);
                sumThread.SetApartmentState(ApartmentState.STA);
                //sumThread.IsBackground = true;
                sumThread.Start();
            }

        }


        bool flag = true;
        bool initFlag = false;
        int initCounter = 0;
        
        /**
         * Main Thread
         * Checkout printer status, restart time, auto summing up time
         **/
        private void SumThreadRunings()
        {
            while (flag)
            {
                Thread.Sleep(10000);
                if (key.GetValue("restartTime") != null)
                {
                    restartTime = Convert.ToDateTime(key.GetValue("restartTime"));
                }
                DateTime startTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(DateTime.Now.ToString("HH")), int.Parse(DateTime.Now.ToString("mm")), 00);

                DateTime endTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(DateTime.Now.ToString("HH")), int.Parse(DateTime.Now.ToString("mm")), 59);

                if(startTime <= restartTime && endTime >= restartTime)
                {
                    messageDialog.Shutdown_windows();
                }

                int limitCounter = 3;
                if (backshowEnable && initFlag && !isError)
                {
                    if(initCounter < limitCounter)
                    {
                        initCounter++;
                    }
                    else
                    {
                        InitState();
                        initFlag = false;
                        initCounter = 0;
                    }
                }
                if (HDCheck)
                {
                    if (!USBPrintStatus())
                    {
                        flag = false;
                        sumThread = null;
                    }
                    if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
                    {
                        if (ipAddress1 != "" && port1 != "")
                        {
                            ipAddress = ipAddress1;
                            if (!TcpPrintStatus())
                            {
                                flag = false;
                                sumThread = null;
                            }
                        }
                        else if (ipAddress2 != "" && port2 != "")
                        {
                            ipAddress = ipAddress2;
                            if (!TcpPrintStatus())
                            {
                                flag = false;
                                sumThread = null;
                            }
                        }
                        else if (ipAddress3 != "" && port3 != "")
                        {
                            ipAddress = ipAddress3;
                            if (!TcpPrintStatus())
                            {
                                flag = false;
                                sumThread = null;
                            }
                        }
                    }
                }
                string currentTime = DateTime.Now.ToString("HH:mm");
                if (String.Compare(currentTime, "00:00") >= 0 && String.Compare(currentTime, "00:10") <= 0)
                {
                    //if (StatusMsgLabel.Text == constants.statusMessage_ok)
                    if (orderAmountArray.Length > 0 && orderAmountArray[0] == null && !isError)
                    {
                        if (dbClass.ProcessState())
                        {
                            //flag = false;
                            messageDialog.SummingMessage();
                            //flag = true;
                            Thread.Sleep(60000);
                        }
                    }
                }
            }

            if (!flag && HDCheck)
                ShowPrintErrorMsgs();
        }

        /**
         * Create Right Panel
         **/
        private void CreateRightPanel()
        {
            try
            {
                constants.SaveLogData("saleScreen_3", "salePage create right panel");
                /** right panel  */
                RightPanelGlobal.Padding = new Padding(0, 0, 0, 0);

                Button BankNoteStatus = customButton.CreateButtonWithImage(constants.depositeDisableButton, "BankNoteStatus", "", 10, 30, RightPanelGlobal.Width - 20, RightPanelGlobal.Height * 3 / 50 + 35, 0, 20, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
                RightPanelGlobal.Controls.Add(BankNoteStatus);
                BankNoteStatus.FlatAppearance.BorderSize = 0;
                
                banknotGlobalLabel = BankNoteStatus;

                BankNoteStatus.Enabled = true;
                BankNoteStatus.Click += new EventHandler(Panel_Click);

                // Create Tabel 
                Panel TableBackPanel = createPanel.CreateSubPanel(RightPanelGlobal, 10, BankNoteStatus.Bottom + 30, RightPanelGlobal.Width - 20, RightPanelGlobal.Height * 9 / 25, BorderStyle.None, Color.Transparent);
                TableBackPanelGlobal = TableBackPanel;

                FlowLayoutPanel bookProductColumnPanel = createPanel.CreateFlowLayoutPanel(TableBackPanel, 0, 0, TableBackPanel.Width * 2 / 3, TableBackPanel.Height, Color.Transparent, new Padding(0));
                FlowLayoutPanel productNumberColumnPanel = createPanel.CreateFlowLayoutPanel(TableBackPanel, bookProductColumnPanel.Right, 0, TableBackPanel.Width / 6, TableBackPanel.Height, Color.Transparent, new Padding(0));
                FlowLayoutPanel deleteColumnPanel = createPanel.CreateFlowLayoutPanel(TableBackPanel, productNumberColumnPanel.Right, 0, TableBackPanel.Width / 6, TableBackPanel.Height, Color.Transparent, new Padding(0));
                deleteColumnPanel.Margin = new Padding(1);
                for (int i = 0; i < 8; i++)
                {
                    Label bookProductLabel = createLabel.CreateLabels(bookProductColumnPanel, "prd_" + i, "", 0, (bookProductColumnPanel.Height / 8) * i, bookProductColumnPanel.Width, bookProductColumnPanel.Height / 8, Color.White, Color.Black, 12, false, ContentAlignment.MiddleLeft, new Padding(0), 1, Color.Gray);
                    bookProductLabel.Padding = new Padding(5, 0, 0, 0);
                    if (bookProductLabel.InvokeRequired)
                    {
                        bookProductLabel.Invoke((MethodInvoker)delegate
                        {
                            bookProductLabel.BorderStyle = BorderStyle.FixedSingle;
                        });
                    }
                    else
                    {
                        bookProductLabel.BorderStyle = BorderStyle.FixedSingle;
                    }

                    Label productNumberLabel = createLabel.CreateLabels(productNumberColumnPanel, "prdNum_" + i, "", 0, (productNumberColumnPanel.Height / 8) * i, productNumberColumnPanel.Width, productNumberColumnPanel.Height / 8, Color.White, Color.Black, 12, false, ContentAlignment.MiddleCenter, new Padding(0), 2, Color.Black);
                    if (productNumberLabel.InvokeRequired)
                    {
                        productNumberLabel.Invoke((MethodInvoker)delegate
                        {
                            productNumberLabel.BorderStyle = BorderStyle.FixedSingle;
                        });
                    }
                    else
                    {
                        productNumberLabel.BorderStyle = BorderStyle.FixedSingle;
                    }
                    Label deleteLabel = createLabel.CreateLabels(deleteColumnPanel, "dell_" + i, "", 0, deleteColumnPanel.Height * i / 8, deleteColumnPanel.Width, deleteColumnPanel.Height / 8, Color.White, Color.Black, 12, false, ContentAlignment.MiddleCenter, new Padding(0), 1, Color.Gray);
                    if (deleteLabel.InvokeRequired)
                    {
                        deleteLabel.Invoke((MethodInvoker)delegate
                        {
                            deleteLabel.BorderStyle = BorderStyle.FixedSingle;
                            deleteLabel.Padding = new Padding(1);
                        });
                    }
                    else
                    {
                        deleteLabel.BorderStyle = BorderStyle.FixedSingle;
                        deleteLabel.Padding = new Padding(1);
                    }
                    Button deleteButton = new Button();
                    deleteButton.Location = new Point(0, 0);
                    deleteButton.Size = new Size(deleteLabel.Width - 4, deleteLabel.Height - 2);
                    deleteButton.Name = "del_" + i;
                    deleteButton.Text = constants.cancelButtonText;
                    deleteButton.ForeColor = Color.White;
                    deleteButton.BackColor = Color.Red;
                    deleteButton.FlatAppearance.BorderSize = 0;
                    deleteButton.Font = new Font("Serif", 8, FontStyle.Bold);
                    deleteButton.FlatAppearance.BorderColor = Color.White;
                    deleteButton.FlatAppearance.MouseOverBackColor = Color.White;
                    deleteLabel.Controls.Add(deleteButton);
                    deleteButton.Click += new EventHandler(this.OrderDelete);



                    orderDeleteLabel[i] = deleteButton;
                    orderAmountLabel[i] = productNumberLabel;
                    orderProductNameLabel[i] = bookProductLabel;

                }

                Button cancelButton = customButton.CreateButtonWithImage(constants.orderCancelDisableButton, "cancelButton", "", 10, TableBackPanel.Bottom + 30, RightPanelGlobal.Width - 20, 80 - 5, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
                cancelButton.FlatAppearance.BorderSize = 0;
                if (RightPanelGlobal.InvokeRequired)
                {
                    RightPanelGlobal.Invoke((MethodInvoker)delegate
                    {
                        RightPanelGlobal.Controls.Add(cancelButton);
                    });
                }
                else
                {
                    RightPanelGlobal.Controls.Add(cancelButton);
                }
                orderCancelButtonGlobal = cancelButton;
                cancelButton.Enabled = false;
                cancelButton.Click += new EventHandler(this.CancelOrder);

                // transaction result show
                Panel TransactionPanel = createPanel.CreateSubPanel(RightPanelGlobal, 10, cancelButton.Bottom + 10, RightPanelGlobal.Width - 20, RightPanelGlobal.Height * 19 * 11 / 1500, BorderStyle.None, Color.Transparent);

                FlowLayoutPanel transactionLabelPanel_1 = createPanel.CreateFlowLayoutPanel(TransactionPanel, 0, 0, TransactionPanel.Width, TransactionPanel.Height / 3 - 2, Color.FromArgb(255, 32, 23, 20), new Padding(10, 2, 10, 2));
                Label sumLabel = createLabel.CreateLabels(transactionLabelPanel_1, "transLabel_0", constants.sumLabel, 0, 0, transactionLabelPanel_1.Width / 6 + 5, transactionLabelPanel_1.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleRight);
                Label sumAmountLabel = createLabel.CreateLabels(transactionLabelPanel_1, "transAmount_1_0", "", sumLabel.Right, 0, transactionLabelPanel_1.Width / 6, transactionLabelPanel_1.Height, Color.Transparent, Color.White, 16, false, ContentAlignment.MiddleCenter);
                orderAmountTotalLabel = sumAmountLabel;

                Label amountUnitLabel = createLabel.CreateLabels(transactionLabelPanel_1, "transUnit_1_0", constants.amountUnit1, sumAmountLabel.Right, 0, transactionLabelPanel_1.Width / 6 - 10, transactionLabelPanel_1.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleLeft);
                Label sumPriceLabel = createLabel.CreateLabels(transactionLabelPanel_1, "transResult_1_0", "", amountUnitLabel.Right, 0, transactionLabelPanel_1.Width * 3 / 8 - 15, transactionLabelPanel_1.Height, Color.Transparent, Color.White, 16, false, ContentAlignment.MiddleRight);
                Label priceUnitLabel = createLabel.CreateLabels(transactionLabelPanel_1, "transResult_1_0", constants.unit, amountUnitLabel.Right, 0, transactionLabelPanel_1.Width / 8 - 5, transactionLabelPanel_1.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleRight);
                orderPriceTotalLabel = sumPriceLabel;
                orderPriceTotalLabel.TextChanged += new System.EventHandler(DepositAmountChange);

                FlowLayoutPanel transactionLabelPanel_2 = createPanel.CreateFlowLayoutPanel(TransactionPanel, 0, TransactionPanel.Height / 3 - 1, TransactionPanel.Width, TransactionPanel.Height / 3 - 2, Color.FromArgb(255, 32, 23, 20), new Padding(10, 2, 10, 2));
                Label depositeLabel = createLabel.CreateLabels(transactionLabelPanel_2, "transLabel_1", constants.depositeLabel, 5, 0, transactionLabelPanel_2.Width * 2 / 5 - 5, transactionLabelPanel_2.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleCenter);
                Label depositeResultLabel = createLabel.CreateLabels(transactionLabelPanel_2, "transResult_1", "", depositeLabel.Right, 0, transactionLabelPanel_2.Width * 2 / 5 - 10, transactionLabelPanel_2.Height, Color.Transparent, Color.White, 16, false, ContentAlignment.MiddleCenter);
                Label depositeUnitLabel = createLabel.CreateLabels(transactionLabelPanel_2, "transResult_1", constants.unit, depositeResultLabel.Right, 0, transactionLabelPanel_2.Width / 5 - 15, transactionLabelPanel_2.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleRight);
                orderPriceEnterLabel = depositeResultLabel;
                orderPriceEnterLabel.TextChanged += new System.EventHandler(DepositAmountChange);

                FlowLayoutPanel transactionLabelPanel_3 = createPanel.CreateFlowLayoutPanel(TransactionPanel, 0, (TransactionPanel.Height / 3 - 1) * 2, TransactionPanel.Width, TransactionPanel.Height / 3 - 2, Color.FromArgb(255, 32, 23, 20), new Padding(10, 2, 10, 2));
                Label changeLabel = createLabel.CreateLabels(transactionLabelPanel_3, "transLabel_2", constants.changeLabel, 5, 0, transactionLabelPanel_3.Width * 2 / 5 - 5, transactionLabelPanel_3.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleCenter);
                Label changeResultLabel = createLabel.CreateLabels(transactionLabelPanel_3, "transResult_2", "", changeLabel.Right, 0, transactionLabelPanel_3.Width * 2 / 5 - 10, transactionLabelPanel_3.Height, Color.Transparent, Color.White, 16, false, ContentAlignment.MiddleCenter);
                Label changeUnitLabel = createLabel.CreateLabels(transactionLabelPanel_3, "transResult_1", constants.unit, changeResultLabel.Right, 0, transactionLabelPanel_3.Width / 5 - 15, transactionLabelPanel_3.Height, Color.Transparent, Color.White, 14, false, ContentAlignment.MiddleRight);
                orderRestPriceLabel = changeResultLabel;

                // payment button show
                Panel PaymentButtonPanel = createPanel.CreateSubPanel(RightPanelGlobal, 10, TransactionPanel.Bottom + 10, RightPanelGlobal.Width - 20, RightPanelGlobal.Height - TransactionPanel.Bottom - 10, BorderStyle.None, Color.Transparent);

                Button ticketingButton = customButton.CreateButtonWithImage(constants.ticketDisableButton, "ticketingButton", "", 0, 0, PaymentButtonPanel.Width, PaymentButtonPanel.Height / 2 - 10, 1, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
                ticketingButton.Enabled = false;
                PaymentButtonPanel.Controls.Add(ticketingButton);
                ticketingButtonGlobal = ticketingButton;
                ticketingButton.Click += new EventHandler(this.ShowTicketing);

                Button receiptButton = customButton.CreateButtonWithImage(constants.receiptDisableButton, "receiptButton", "", 0, ticketingButton.Bottom + 10, PaymentButtonPanel.Width, PaymentButtonPanel.Height / 2 - 10, 1, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
                PaymentButtonPanel.Controls.Add(receiptButton);
                receiptButton.Enabled = false;
                receiptButtonGlobal = receiptButton;
                receiptButton.Click += new EventHandler(this.ShowTicketingWithReciept);
                //receiptButton.Click += new EventHandler(this.ReceiptRun);
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_4", ex.ToString());
                return;
            }

        }


        private void CoinStatusClick(object sender, EventArgs e)
        {
            string soundPlay = @"resources\\btn_beep.wav";
            this.PlaySound(soundPlay);
            string depositeStatus = banknotGlobalLabel.Name;
            if(depositeStatus == "depositeButton")
            {
                banknotGlobalLabel.Name = "backDepositeButton";
                if(comModule != null && !comModule.bPermit)
                {
                    comModule.PermitCommand();
                    currentbPermit = true;
                }
            }
        }
        /** 
         * Sale page Close when clicked F1
         **/
        private void FormKeyReturnEventHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                this.BackShow();
            }
        }

        /**
         * Create Main Panel(create category, create products list, create right panel) by Threading when loading
         **/
        private void DisplayData()
        {
            try
            {
                constants.SaveLogData("saleScreen_2_1", "salePage loading component");
                if (this.InvokeRequired)
                {
                    //messageDialog.ShowErrorMessage("load complete", "\n ErrorNo: 004");
                    this.Invoke((MethodInvoker)delegate
                    {
                        Panel LeftPanel = createPanel.CreateSubPanel(mainPanelGlobal2, 0, 0, 19 * mainPanelGlobal2.Width / 25, mainPanelGlobal2.Height, BorderStyle.FixedSingle, Color.FromArgb(255, 242, 242, 242));
                        LeftPanelGlobal = LeftPanel;

                        /** Screen Message Panel */
                        ScreenMsgPanelGlobal = createPanel.CreateSubPanel(LeftPanel, 0, 0, LeftPanel.Width, LeftPanel.Height * 3 / 25 - 20, BorderStyle.None, ColorTranslator.FromHtml(screenMsgBackColor[0]));

                        /** Left sub panel  */
                        LeftSubPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 22 / 25 + 20, BorderStyle.None, Color.Transparent);
                        using(Bitmap img = new Bitmap(constants.leftSubPanel))
                        {
                            LeftSubPanel.BackgroundImage = new Bitmap(img);
                        }
                        LeftSubPanel.BackgroundImageLayout = ImageLayout.Stretch;
                        LeftSubPanel.BackgroundImageLayout = ImageLayout.Stretch;

                        /** Category Panel */
                        categoryPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 2 / 25, BorderStyle.None, Color.White);
                        categoryPanel.Hide();

                        downPanel = createPanel.CreateSubPanel(LeftPanel, 0, LeftPanel.Height * 5 / 25 - 20, LeftPanel.Width, LeftPanel.Height * 20 / 25 + 20, BorderStyle.FixedSingle, Color.Transparent);

                        /** Main Product Panel layout */
                        Panel MenuBodyLayout = createPanel.CreateSubPanel(downPanel, 20, 20, downPanel.Width - 20, downPanel.Height - 20, BorderStyle.None, Color.Transparent);
                        MainBodyPanelGlobal = MenuBodyLayout;

                        downPanel.Hide();

                        /** Right Panel */

                        RightPanelGlobal = createPanel.CreateSubPanel(mainPanelGlobal2, mainPanelGlobal2.Width * 19 / 25, 0, mainPanelGlobal2.Width * 6 / 25, mainPanelGlobal2.Height, BorderStyle.FixedSingle, Color.FromArgb(255, 255, 255, 204));

                        this.CreateCategoryList();
                        if (RightPanelGlobal.InvokeRequired)
                        {
                            RightPanelGlobal.Invoke((MethodInvoker)delegate
                            {
                                this.CreateRightPanel();
                            });
                        }
                        else
                        {
                            this.CreateRightPanel();
                        }
                    });
                }
                else
                {
                    Panel LeftPanel = createPanel.CreateSubPanel(mainPanelGlobal2, 0, 0, 19 * mainPanelGlobal2.Width / 25, mainPanelGlobal2.Height, BorderStyle.FixedSingle, Color.FromArgb(255, 242, 242, 242));
                    LeftPanelGlobal = LeftPanel;

                    /** Screen Message Panel */
                    ScreenMsgPanelGlobal = createPanel.CreateSubPanel(LeftPanel, 0, 0, LeftPanel.Width, LeftPanel.Height * 3 / 25 - 20, BorderStyle.None, ColorTranslator.FromHtml(screenMsgBackColor[0]));

                    /** Left sub panel  */
                    LeftSubPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 22 / 25 + 20, BorderStyle.None, Color.Transparent);
                    using (Bitmap img = new Bitmap(constants.leftSubPanel))
                    {
                        LeftSubPanel.BackgroundImage = new Bitmap(img);
                    }
                    LeftSubPanel.BackgroundImageLayout = ImageLayout.Stretch;
                    LeftSubPanel.Click += new EventHandler(ShowSaleScreen);

                    /** Category Panel */
                    categoryPanel = createPanel.CreateSubPanel(LeftPanelGlobal, 0, LeftPanelGlobal.Height * 3 / 25 - 20, LeftPanelGlobal.Width, LeftPanelGlobal.Height * 2 / 25, BorderStyle.None, Color.White);
                    categoryPanel.Hide();

                    downPanel = createPanel.CreateSubPanel(LeftPanel, 0, LeftPanel.Height * 5 / 25 - 20, LeftPanel.Width, LeftPanel.Height * 20 / 25 + 20, BorderStyle.FixedSingle, Color.Transparent);

                    /** Main Product Panel layout */
                    Panel MenuBodyLayout = createPanel.CreateSubPanel(downPanel, 20, 20, downPanel.Width -20, downPanel.Height - 20, BorderStyle.None, Color.Transparent);
                    MainBodyPanelGlobal = MenuBodyLayout;
                    downPanel.Hide();

                    /** Right Panel */

                    RightPanelGlobal = createPanel.CreateSubPanel(mainPanelGlobal2, mainPanelGlobal2.Width * 19 / 25, 0, mainPanelGlobal2.Width * 6 / 25, mainPanelGlobal2.Height, BorderStyle.FixedSingle, Color.FromArgb(255, 255, 255, 204));

                    this.CreateCategoryList();
                    if (RightPanelGlobal.InvokeRequired)
                    {
                        RightPanelGlobal.Invoke((MethodInvoker)delegate
                        {
                            this.CreateRightPanel();
                        });
                    }
                    else
                    {
                        this.CreateRightPanel();
                    }

                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_2_1", ex.ToString());
                return;
            }
        }

        private void ShowSaleScreen(object sender, EventArgs e)
        {
            initCounter = 0;
            if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
            {
                initFlag = true;
            }
            else
            {
                initFlag = false;
            }
            initFlag = true;
            string audio_file_name = dbClass.AudioFile("ValidItemTouch");
            this.PlaySound(audio_file_name);
            LeftSubPanel.Hide();
            downPanel.Show();
            categoryPanel.Show();
            downPanel.Invalidate();
            categoryPanel.Invalidate();
            if (comModule != null)
            {
                comModule.PermitCommand();
                currentbPermit = true;
            }
            if (banknotGlobalLabel.InvokeRequired)
            {
                banknotGlobalLabel.Invoke((MethodInvoker)delegate ()
                {
                    using (Bitmap img = new Bitmap(constants.depositeEnableButton))
                    {
                        banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                    }
                });
            }
            else
            {
                using (Bitmap img = new Bitmap(constants.depositeEnableButton))
                {
                    banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                }
            }

        }

        private void HideSaleScreen()
        {
            categoryPanel.Hide();
            downPanel.Hide();
            LeftSubPanel.Show();
            LeftSubPanel.Invalidate();
        }

        /**
         * Getting Ordering Status in realtime
         **/
        bool prevbPermit = false;
        bool currentbPermit = false;
        private void DepositAmountChange(object sender, EventArgs e)
        {
            initCounter = 0;
            initFlag = true;

            int orderAmount = 0;
            int enterAmount = 0;
            if (orderPriceTotalLabel.Text != "")
            {
                if (orderPriceEnterLabel.Text != "" && int.Parse(orderPriceEnterLabel.Text) > int.Parse(orderPriceTotalLabel.Text))
                {
                    orderRestPriceLabel.Text = (int.Parse(orderPriceEnterLabel.Text) - int.Parse(orderPriceTotalLabel.Text)).ToString();
                }
                else
                {
                    orderRestPriceLabel.Text = "";
                }
                orderAmount = int.Parse(orderPriceTotalLabel.Text);

                //comModule.OrderChange(orderPriceTotalLabel.Text);
            }
            else
            {
                orderRestPriceLabel.Text = "";
            }
            if(orderPriceEnterLabel.Text != "")
            {
                enterAmount = int.Parse(orderPriceEnterLabel.Text);
            }

            if (enterAmount >= orderAmount && orderAmount != 0 && !isError && (enterAmount - orderAmount) <= 20000) //amount
            {
                if (ticketingButtonGlobal.InvokeRequired)
                {
                    ticketingButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.ticketButton))
                        {
                            ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        ticketingButtonGlobal.Enabled = true;
                        if (ReceiptValid == "true")
                        {
                            using (Bitmap img = new Bitmap(constants.receiptButton))
                            {
                                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                            }
                            receiptButtonGlobal.Enabled = true;
                        }

                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.ticketButton))
                    {
                        ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                    ticketingButtonGlobal.Enabled = true;
                    if (ReceiptValid == "true")
                    {
                        using (Bitmap img = new Bitmap(constants.receiptButton))
                        {
                            receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        receiptButtonGlobal.Enabled = true;
                    }
                }
            }
            else
            {
                if (ticketingButtonGlobal.InvokeRequired)
                {
                    ticketingButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.ticketDisableButton))
                        {
                            ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        ticketingButtonGlobal.Enabled = false;
                        if (ReceiptValid == "true")
                        {
                            using (Bitmap img = new Bitmap(constants.receiptDisableButton))
                            {
                                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                            }
                            receiptButtonGlobal.Enabled = false;
                        }
                    }));
                }
                else
                {
                    using(Bitmap img = new Bitmap(constants.ticketDisableButton))
                    {
                        ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                    ticketingButtonGlobal.Enabled = false;
                    if (ReceiptValid == "true")
                    {
                        using(Bitmap img = new Bitmap(constants.receiptDisableButton))
                        {
                            receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        receiptButtonGlobal.Enabled = false;
                    }
                }
            }


            if (enterAmount != 0 && enterAmount <= 20000 )
            {
                orderCancelButtonGlobal.Enabled = true;
                using(Bitmap img = new Bitmap(constants.orderCancelButton))
                {
                    orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                }
            }
            else
            {
                orderCancelButtonGlobal.Enabled = false;
                using(Bitmap img = new Bitmap(constants.orderCancelDisableButton))
                {
                    orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                }
            }

        }

        /**
         * Getting Deposit Status from ComModule using Thread in realtime
         **/
        public void SetDepositAmount(int amount)
        {
            if (orderPriceEnterLabel.InvokeRequired)
            {
                orderPriceEnterLabel.Invoke(new MethodInvoker(delegate
                {
                    orderPriceEnterLabel.Text = amount.ToString();
                    //orderPriceEnterLabel.Text = "15000";
                }));
            }
            else
            {
                orderPriceEnterLabel.Text = amount.ToString();
            }
            if (amount >= orderTotalPrice && orderTotalPrice != 0 && !isError && amount - orderTotalPrice <= 20000) //amount
            {
                if (ticketingButtonGlobal.InvokeRequired)
                {
                    ticketingButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.ticketButton))
                        {
                            ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        ticketingButtonGlobal.Enabled = true;
                        if (ReceiptValid == "true")
                        {
                            using (Bitmap img = new Bitmap(constants.receiptButton))
                            {
                                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                            }
                            receiptButtonGlobal.Enabled = true;
                        }
                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.ticketButton))
                    {
                        ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                    ticketingButtonGlobal.Enabled = true;
                    if (ReceiptValid == "true")
                    {
                        using (Bitmap img = new Bitmap(constants.receiptButton))
                        {
                            receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        receiptButtonGlobal.Enabled = true;
                    }
                }
            }
            else
            {
                if (ticketingButtonGlobal.InvokeRequired)
                {
                    ticketingButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.ticketDisableButton))
                        {
                            ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        ticketingButtonGlobal.Enabled = false;
                        if (ReceiptValid == "true")
                        {
                            using (Bitmap img = new Bitmap(constants.receiptDisableButton))
                            {
                                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                            }
                            receiptButtonGlobal.Enabled = false;
                        }
                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.ticketDisableButton))
                    {
                        ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                    ticketingButtonGlobal.Enabled = false;
                    if (ReceiptValid == "true")
                    {
                        using (Bitmap img = new Bitmap(constants.receiptDisableButton))
                        {
                            receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                        receiptButtonGlobal.Enabled = false;
                    }
                }
            }

            if(amount >= 20000 && !highBank)
            {
                if(orderCancelButtonGlobal.InvokeRequired)
                {
                    orderCancelButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        orderCancelButtonGlobal.Enabled = false;
                        using (Bitmap img = new Bitmap(constants.orderCancelDisableButton))
                        {
                            orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                    }));
                }
                else
                {
                    orderCancelButtonGlobal.Enabled = false;
                    using (Bitmap img = new Bitmap(constants.orderCancelDisableButton))
                    {
                        orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                }
            }
            if (comModule != null && comModule.bPermit && prevbPermit != currentbPermit && currentbPermit)
            {
                if (banknotGlobalLabel.InvokeRequired)
                {
                    banknotGlobalLabel.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.depositeEnableButton))
                        {
                            banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                        }
                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.depositeEnableButton))
                    {
                        banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                    }
                }
                prevbPermit = currentbPermit;
            }
            else if(comModule != null && !comModule.bPermit && prevbPermit != currentbPermit && !currentbPermit)
            {
                if (banknotGlobalLabel.InvokeRequired)
                {
                    banknotGlobalLabel.Invoke(new MethodInvoker(delegate
                    {
                        using (Bitmap img = new Bitmap(constants.depositeDisableButton))
                        {
                            banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                        }
                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.depositeDisableButton))
                    {
                        banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                    }
                }
                prevbPermit = currentbPermit;
            }
            if(amount > 0 )
            {
                backshowEnable = false;
                if (banknotGlobalLabel.InvokeRequired)
                {
                    banknotGlobalLabel.Invoke(new MethodInvoker(delegate
                    {
                        banknotGlobalLabel.Enabled = false;
                    }));
                }
                else
                {
                    banknotGlobalLabel.Enabled = false;
                }
            }
            else
            {
                backshowEnable = true;
                if (banknotGlobalLabel.InvokeRequired)
                {
                    banknotGlobalLabel.Invoke(new MethodInvoker(delegate
                    {
                        banknotGlobalLabel.Enabled = true;
                    }));
                }
                else
                {
                    banknotGlobalLabel.Enabled = true;
                }
            }
        }

        /**
         * Creating Category list
         **/
        int selectedID = 0;

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

                if (MainBodyPanelGlobal.InvokeRequired)
                {
                    MainBodyPanelGlobal.Invoke((MethodInvoker)delegate
                    {
                        MainBodyPanelGlobal.Show();
                        MainBodyPanelGlobal.Controls.Clear();
                    });
                }
                else
                {
                    MainBodyPanelGlobal.Show();
                    MainBodyPanelGlobal.Controls.Clear();
                }

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

                string queryCmd = "SELECT * FROM " + constants.tbNames[0] + " ORDER BY id";
                sqlite_cmd.CommandText = queryCmd;

                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    categoryIDArray[k] = sqlite_datareader.GetInt32(1);
                    categoryBackImageArray[k] = Path.Combine(currentDir, sqlite_datareader.GetString(9));
                    string categoryButtonText = sqlite_datareader.GetString(2);
                    string categoryButtonName = "saleCategoryBtn_" + k + "_" + sqlite_datareader.GetInt32(1).ToString() + "_" + sqlite_datareader.GetInt32(7).ToString();

                    if (sqlite_datareader.GetInt32(10) == 1)
                    {
                        categoryButtonText = constants.saleStopText;
                    }

                    Button categoryBtn;
                    string categoryBtnImg = constants.categoryButton;
                    categoryBtn = customButton.CreateButtonWithImage(constants.categoryButton, categoryButtonName, categoryButtonText, 5 + (categoryPanel.Width / 6) * k, 5, categoryPanel.Width / 6 - 10, categoryPanel.Height - 10, 1, 20, 16, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);

                    //categoryBtn.FlatStyle = FlatStyle.Flat;
                    if (sqlite_datareader.GetInt32(10) == 1)
                    {
                        categoryBtn.Enabled = false;
                    }

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

                    if (categoryMain)
                    {
                        int m = k / 2;

                        Panel categoryPicturePanel = null;
                        if (k % 2 == 0)
                        {
                            categoryPicturePanel = createPanel.CreateSubPanel(MainBodyPanelGlobal, 0, ((MainBodyPanelGlobal.Height / 12 - 45) + m * (MainBodyPanelGlobal.Height / 3)), MainBodyPanelGlobal.Width * 3 / 7 + 30, MainBodyPanelGlobal.Height / 4 + 10, BorderStyle.None, Color.Transparent);
                        }
                        else
                        {
                            categoryPicturePanel = createPanel.CreateSubPanel(MainBodyPanelGlobal, MainBodyPanelGlobal.Width / 2 + 20, ((MainBodyPanelGlobal.Height / 12 - 45) + m * (MainBodyPanelGlobal.Height / 3)), MainBodyPanelGlobal.Width * 3 / 7 + 30, MainBodyPanelGlobal.Height / 4 + 10, BorderStyle.None, Color.Transparent);
                        }
                        categoryPicturePanel.Name = categoryButtonName;

                        categoryPicturePanel.Click += new EventHandler(this.SelectCategoryFromPanel);

                        PictureBox ptBox = new PictureBox();
                        Bitmap ptBitmap = null;
                        if (sqlite_datareader.GetString(9) != null && sqlite_datareader.GetString(9) != "")
                        {
                            using (ptBitmap = new Bitmap(categoryBackImageArray[k]))
                            {
                                ptBox.Image = new Bitmap(ptBitmap);
                            }
                            if (ptBitmap != null)
                            {
                                ptBitmap.Dispose();
                                ptBitmap = null;
                            }
                            ptBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            ptBox.Image = null;
                        }

                        ptBox.Location = new Point(0, 0);
                        ptBox.Size = new Size((categoryPicturePanel.Width - 20) * 2 / 3 + 20, categoryPicturePanel.Height);
                        ptBox.Name = categoryButtonName;
                        categoryPicturePanel.Controls.Add(ptBox);
                        ptBox.Click += new EventHandler(this.SelectCategoryFromPicture);

                        Button categroyBtnSub = customButton.CreateButtonWithImage(categoryBtnImg, categoryButtonName, categoryButtonText, ptBox.Right + 5, ptBox.Bottom - 60, MainBodyPanelGlobal.Width / 7, 60, 1, 20, 16, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                        categroyBtnSub.Click += new EventHandler(this.SelectCategory);

                        categoryPicturePanel.Controls.Add(categroyBtnSub);

                    }
                    else
                    {
                        if (k == 0)
                        {
                            selectedID = k;
                            selectedCategoryID = sqlite_datareader.GetInt32(1);
                            selectedCategoryLayout = sqlite_datareader.GetInt32(7);
                            using(Bitmap img = new Bitmap(constants.categoryActiveButton))
                            {
                                categoryButton[k].BackgroundImage = new Bitmap(img);
                            }
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
                            if(sqlite_conn == null)
                            {
                                sqlite_conn = CreateConnection(constants.dbName);
                                if (sqlite_conn.State == ConnectionState.Closed)
                                {
                                    sqlite_conn.Open();
                                }
                            }
                        }
                    }

                    k++;
                }

                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_2_2", ex.ToString());
                return;
            }
        }

        /**
         * Creating Category list and product list when selected category item using thread
         **/
        private void SelectCategory(object sender, EventArgs e)
        {
            initCounter = 0;
            if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
            {
                initFlag = true;
            }
            else
            {
                initFlag = false;
            }
            initFlag = true;

            if (comModule == null)
                return;
            if (messageDialog.DialogFormGlobal != null)
                return;

            if (comModule != null && !comModule.bPermit)
            {
                comModule.PermitCommand();
                currentbPermit = true;
            }
            MainBodyPanelGlobal.Hide();

            Button btnTemp = (Button)sender;
            selectedID = int.Parse(btnTemp.Name.Split('_')[1]);
            selectedCategoryID = int.Parse(btnTemp.Name.Split('_')[2]);
            selectedCategoryLayout = int.Parse(btnTemp.Name.Split('_')[3]);
            Bitmap tempBitImg = null;

            for (int k = 0; k < categoryButton.Length; k++)
            {
                if (k == selectedID)
                {
                    categoryButton[k].ForeColor = ColorTranslator.FromHtml("#FDD648");
                    categoryButton[k].Font = new Font("Serif", 18, FontStyle.Bold);
                    Thread.Sleep(100);
                    using (tempBitImg = new Bitmap(constants.categoryActiveButton))
                    {
                        categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                    }
                    if (tempBitImg != null)
                    {
                        tempBitImg.Dispose();
                        tempBitImg = null;
                    }
                }
                else
                {
                    if (categoryButton[k] != null)
                    {
                        categoryButton[k].ForeColor = ColorTranslator.FromHtml(categoryTextColor[k]);
                        categoryButton[k].Font = new Font("Serif", 16, FontStyle.Bold);
                        Thread.Sleep(100);
                        using (tempBitImg = new Bitmap(constants.categoryButton))
                        {
                            categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                        }
                        if (tempBitImg != null)
                        {
                            tempBitImg.Dispose();
                            tempBitImg = null;
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
                //MessageBox.Show(ex.ToString());
                //Console.WriteLine("createCategoryError=>" + ex.ToString());
                this.BackShow();

            }
        }

        private void SelectCategoryFromPanel(object sender, EventArgs e)
        {
            initCounter = 0;
            if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
            {
                initFlag = true;
            }
            else
            {
                initFlag = false;
            }
            initFlag = true;

            if (comModule == null)
                return;
            if (messageDialog.DialogFormGlobal != null)
                return;
            if (comModule != null && !comModule.bPermit)
            {
                comModule.PermitCommand();
                currentbPermit = true;
            }

            MainBodyPanelGlobal.Hide();

            Panel btnTemp = (Panel)sender;
            selectedID = int.Parse(btnTemp.Name.Split('_')[1]);
            selectedCategoryID = int.Parse(btnTemp.Name.Split('_')[2]);
            selectedCategoryLayout = int.Parse(btnTemp.Name.Split('_')[3]);
            Bitmap tempBitImg = null;

            for (int k = 0; k < categoryButton.Length; k++)
            {
                if (k == selectedID)
                {
                    categoryButton[k].ForeColor = ColorTranslator.FromHtml("#FDD648");
                    categoryButton[k].Font = new Font("Serif", 18, FontStyle.Bold);
                    Thread.Sleep(100);
                    using (tempBitImg = new Bitmap(constants.categoryActiveButton))
                    {
                        categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                    }
                    if (tempBitImg != null)
                    {
                        tempBitImg.Dispose();
                        tempBitImg = null;
                    }
                }
                else
                {
                    if (categoryButton[k] != null)
                    {
                        categoryButton[k].ForeColor = ColorTranslator.FromHtml(categoryTextColor[k]);
                        categoryButton[k].Font = new Font("Serif", 16, FontStyle.Bold);
                        Thread.Sleep(100);
                        using (tempBitImg = new Bitmap(constants.categoryButton))
                        {
                            categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                        }
                        if (tempBitImg != null)
                        {
                            tempBitImg.Dispose();
                            tempBitImg = null;
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
                this.BackShow();
            }
        }

        private void SelectCategoryFromPicture(object sender, EventArgs e)
        {
            initCounter = 0;
            if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
            {
                initFlag = true;
            }
            else
            {
                initFlag = false;
            }
            initFlag = true;

            if (comModule == null)
                return;
            if (messageDialog.DialogFormGlobal != null)
                return;

            if (comModule != null && !comModule.bPermit)
            {
                comModule.PermitCommand();
                currentbPermit = true;
            }
            MainBodyPanelGlobal.Hide();

            PictureBox btnTemp = (PictureBox)sender;
            selectedID = int.Parse(btnTemp.Name.Split('_')[1]);
            selectedCategoryID = int.Parse(btnTemp.Name.Split('_')[2]);
            selectedCategoryLayout = int.Parse(btnTemp.Name.Split('_')[3]);
            Bitmap tempBitImg = null;
            for (int k = 0; k < categoryButton.Length; k++)
            {
                if (k == selectedID)
                {
                    categoryButton[k].ForeColor = ColorTranslator.FromHtml("#FDD648");
                    categoryButton[k].Font = new Font("Serif", 18, FontStyle.Bold);
                    Thread.Sleep(100);
                    using(tempBitImg = new Bitmap(constants.categoryActiveButton))
                    {
                        categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                    }
                    if (tempBitImg != null)
                    {
                        tempBitImg.Dispose();
                        tempBitImg = null;
                    }
                }
                else
                {
                    if (categoryButton[k] != null)
                    {
                        categoryButton[k].ForeColor = ColorTranslator.FromHtml(categoryTextColor[k]);
                        categoryButton[k].Font = new Font("Serif", 16, FontStyle.Bold);
                        using (tempBitImg = new Bitmap(constants.categoryButton))
                        {
                            categoryButton[k].BackgroundImage = new Bitmap(tempBitImg);
                        }
                        if(tempBitImg != null)
                        {
                            tempBitImg.Dispose();
                            tempBitImg = null;
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
                //MessageBox.Show(ex.ToString());
                //Console.WriteLine("createCategoryError=>" + ex.ToString());
                this.BackShow();

            }
        }

        private void SelectedCategoryLoader()
        {
            SetSubLoading(true);

            selectedCategoryIndex = selectedID;
            ScreenMsgPanelGlobal.BackColor = ColorTranslator.FromHtml(screenMsgBackColor[selectedID]);

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

        /**
         * Create Product list in mainbody panel when loading or selecting category
         **/

        private void CreateProductsList11()
        {
            try
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
                p_url = new string[selectedCategoryLayout];

                bl_Name = new string[selectedCategoryLayout];
                bl_PrintName = new string[selectedCategoryLayout];
                bl_Price = new string[selectedCategoryLayout];

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                for (int i = 0; i < selectedCategoryLayout; i++)
                {
                    string prdName = "";
                    int prdLimitedCnt = 0;
                    int prdPrice = 0;
                    string prdImageUrl = "";
                    int soldFlag = 0;
                    string productTime = "00:00-24:00";

                    SQLiteDataReader sqlite_datareader;
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = sqlite_conn.CreateCommand();
                    string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                    sqlite_cmd.CommandText = queryCmd;
                    sqlite_cmd.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                    sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                    sqlite_datareader = sqlite_cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        productIDArray[i] = sqlite_datareader.GetInt32(0);
                        realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                        prdName = sqlite_datareader.GetString(3);
                        bl_PrintName[i] = sqlite_datareader.GetString(4);
                        productTime = sqlite_datareader.GetString(5);
                        bl_Name[i] = prdName;
                        prdPrice = sqlite_datareader.GetInt32(8);
                        bl_Price[i] = prdPrice.ToString();
                        prdLimitedCnt = sqlite_datareader.GetInt32(9);
                        productLimitedCntArray[selectedCategoryIndex, i] = prdLimitedCnt;
                        prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                        p_url[i] = Path.Combine(currentDir, sqlite_datareader.GetString(11));
                        soldFlag = sqlite_datareader.GetInt32(17);
                        if (productRestAmountArray[selectedCategoryIndex, i] == -1)
                        {
                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and sumFlag='false' and limitFlag=0";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", realProductIDArray[i]);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (productLimitedCntArray[selectedCategoryIndex, i] != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i] - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i];
                                    }
                                }
                                else
                                {
                                    productRestAmountArray[selectedCategoryIndex, i] = 10000;
                                }
                            }
                            sqlite_datareader1.Close();
                            sqlite_datareader1 = null;
                            sqlite_cmd1.Dispose();
                            sqlite_cmd1 = null;
                        }
                        break;
                    }

                    //sqlite_conn.Close();
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                    sqlite_datareader.Close();
                    sqlite_datareader = null;

                    Panel p = new Panel();

                    if (prdImageUrl != "" && prdImageUrl != null)
                    {
                        borderPen = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
                        BackgroundBitmap = null;
                        if (i == 0)
                        {
                            p = createPanel.CreatePanelForProducts(0, 0, nWidth, nHeight, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                        }
                        else if (i == 1)
                        {
                            p = createPanel.CreatePanelForProducts(nWidth + 20, 0, nWidth2, nHeight2, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor3_Paint);
                        }
                        else if (i == 2 || i == 6 || i == 10)
                        {
                            int hh = 2 * h1 + 40;
                            if (i == 6) hh = 3 * h1 + 60;
                            if (i == 10) hh = 4 * h1 + 80;
                            p = createPanel.CreatePanelForProducts(nWidth + 20, hh, nWidth3, nHeight3, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor4_Paint);
                        }
                        else if (i < 6)
                        {
                            p = createPanel.CreatePanelForProducts((i - 3) * w1 + (i - 3) * 20, nHeight + 20, w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                        }
                        else
                        {
                            p = createPanel.CreatePanelForProducts((i - 7) * w1 + (i - 7) * 20, nHeight + h1 + 40, w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
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


                        if (soldFlag == 1 || (prdLimitedCnt != 0 && productRestAmountArray[selectedCategoryIndex, i] <= 0))
                        {
                            PictureBox badge_pb = new PictureBox();
                            badge_pb.Size = new Size(pb_Image[i].Width, pb_Image[i].Height);
                            badge_pb.Location = new Point(0, 0);
                            Bitmap tempBitmap = null;
                            using (tempBitmap = new Bitmap(constants.soldoutBadge))
                            {
                                badge_pb.Image = new Bitmap(tempBitmap);
                            }
                            if (tempBitmap != null)
                            {
                                tempBitmap.Dispose();
                                tempBitmap = null;
                            }
                            pb_Image[i].Controls.Add(badge_pb);
                            pb_Image[i].Enabled = false;
                        }
                        else
                        {
                            string[] dayTimeArr = productTime.Split('-');
                            if (dayTimeArr[0] != "24:00" && dayTimeArr[1] != "24:00")
                            {
                                bool saleAvailable = constants.SaleAvailable(dayTimeArr[0], dayTimeArr[1]);
                                if (!saleAvailable)
                                {
                                    PictureBox badge_pb = new PictureBox();
                                    badge_pb.Size = new Size(pb_Image[i].Width / 2, pb_Image[i].Height / 2);
                                    badge_pb.Location = new Point(pb_Image[i].Width / 2, 20);
                                    badge_pb.SizeMode = PictureBoxSizeMode.StretchImage;
                                    Bitmap tempBitmap = null;
                                    using (tempBitmap = new Bitmap(constants.preparingBadge))
                                    {
                                        badge_pb.Image = new Bitmap(tempBitmap);
                                    }
                                    if (tempBitmap != null)
                                    {
                                        tempBitmap.Dispose();
                                        tempBitmap = null;
                                    }
                                    pb_Image[i].Controls.Add(badge_pb);
                                    pb_Image[i].Enabled = false;
                                }
                            }
                        }

                        pb_Image[i].Click += new EventHandler(OnItemPicClk);

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
                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch(Exception ex)
            {
                constants.SaveLogData("saleScreen_*_1", ex.ToString());
                return;
            }

        }

        private void CreateProductsList13()
        {
            try
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
                curProduct = -1;
                p_ProductCon = new Panel[selectedCategoryLayout];
                pb_Image = new PictureBox[selectedCategoryLayout];
                p_url = new string[selectedCategoryLayout];
                bl_Name = new string[selectedCategoryLayout];
                bl_PrintName = new string[selectedCategoryLayout];
                bl_Price = new string[selectedCategoryLayout];
                string productTime = "00:00-24:00";

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }

                for (int i = 0; i < selectedCategoryLayout; i++)
                {
                    string prdName = "";
                    int prdLimitedCnt = 0;
                    int prdPrice = 0;
                    string prdImageUrl = "";
                    int soldFlag = 0;

                    SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                    string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                    sqlite_cmd.CommandText = queryCmd;
                    sqlite_cmd.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                    sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                    SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        productIDArray[i] = sqlite_datareader.GetInt32(0);
                        realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                        prdName = sqlite_datareader.GetString(3);
                        bl_PrintName[i] = sqlite_datareader.GetString(4);
                        productTime = sqlite_datareader.GetString(5);
                        bl_Name[i] = prdName;
                        prdPrice = sqlite_datareader.GetInt32(8);
                        bl_Price[i] = prdPrice.ToString();
                        prdLimitedCnt = sqlite_datareader.GetInt32(9);
                        productLimitedCntArray[selectedCategoryIndex, i] = prdLimitedCnt;
                        prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                        p_url[i] = Path.Combine(currentDir, sqlite_datareader.GetString(11));
                        soldFlag = sqlite_datareader.GetInt32(17);
                        if (productRestAmountArray[selectedCategoryIndex, i] == -1)
                        {
                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and sumFlag='false' and limitFlag=0";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", realProductIDArray[i]);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (productLimitedCntArray[selectedCategoryIndex, i] != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i] - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i];
                                    }
                                }
                                else
                                {
                                    productRestAmountArray[selectedCategoryIndex, i] = 10000;
                                }
                            }
                            sqlite_cmd1.Dispose();
                            sqlite_cmd1 = null;
                            sqlite_datareader1.Close();
                            sqlite_datareader1 = null;
                        }
                        break;
                    }

                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                    sqlite_datareader.Close();
                    sqlite_datareader = null;

                    Panel p = new Panel();

                    if (prdImageUrl != "" && prdImageUrl != null)
                    {
                        borderPen = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
                        BackgroundBitmap = null;
                        if (i == 0 || i == 1)
                        {
                            p = createPanel.CreatePanelForProducts(i * (2 * w1 + 20) + i * 20, 0, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                        }
                        else if (i == 2 || i == 3)
                        {
                            p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 2) * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                        }
                        else if (i == 4 || i == 5)
                        {
                            p = createPanel.CreatePanelForProducts((i - 4) * (2 * w1 + 20) + (i - 4) * 20, 2 * h1 + 40, 2 * w1 + 20, 2 * h1 + 20, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor2_Paint);
                        }
                        else if (i == 6 || i == 7)
                        {
                            p = createPanel.CreatePanelForProducts(2 * (2 * w1 + 20) + 2 * 20, (i - 4) * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                            p.Paint += new PaintEventHandler(Panelbordercolor_Paint);
                        }
                        else
                        {
                            p = createPanel.CreatePanelForProducts((i - 8) * w1 + (i - 8) * 20, 2 * (2 * h1 + 40), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
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

                        if (soldFlag == 1 || (prdLimitedCnt != 0 && productRestAmountArray[selectedCategoryIndex, i] <= 0))
                        {
                            PictureBox badge_pb = new PictureBox();
                            badge_pb.Size = new Size(pb_Image[i].Width, pb_Image[i].Height);
                            badge_pb.Location = new Point(0, 0);
                            Bitmap tempBitmap = null;
                            using (tempBitmap = new Bitmap(constants.soldoutBadge))
                            {
                                badge_pb.Image = new Bitmap(tempBitmap);
                            }
                            if (tempBitmap != null)
                            {
                                tempBitmap.Dispose();
                                tempBitmap = null;
                            }
                            pb_Image[i].Controls.Add(badge_pb);
                            pb_Image[i].Enabled = false;
                        }
                        else
                        {
                            string[] dayTimeArr = productTime.Split('-');
                            if (dayTimeArr[0] != "24:00" && dayTimeArr[1] != "24:00")
                            {
                                bool saleAvailable = constants.SaleAvailable(dayTimeArr[0], dayTimeArr[1]);
                                if (!saleAvailable)
                                {
                                    PictureBox badge_pb = new PictureBox();
                                    badge_pb.Size = new Size(pb_Image[i].Width / 2, pb_Image[i].Height / 2);
                                    badge_pb.Location = new Point(pb_Image[i].Width / 2, 20);
                                    badge_pb.SizeMode = PictureBoxSizeMode.StretchImage;
                                    Bitmap tempBitmap = null;
                                    using (tempBitmap = new Bitmap(constants.preparingBadge))
                                    {
                                        badge_pb.Image = new Bitmap(tempBitmap);
                                    }
                                    if (tempBitmap != null)
                                    {
                                        tempBitmap.Dispose();
                                        tempBitmap = null;
                                    }
                                    pb_Image[i].Controls.Add(badge_pb);
                                    pb_Image[i].Enabled = false;
                                }
                            }
                        }

                        pb_Image[i].Click += new EventHandler(OnItemPicClk);

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

                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_*_2", ex.ToString());
                return;

            }
        }

        private void CreateProductsList()
        {
            try
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
                string productTime = "00:00-24:00";
                curProduct = -1;
                p_ProductCon = new Panel[selectedCategoryLayout];
                pb_Image = new PictureBox[selectedCategoryLayout];
                p_url = new string[selectedCategoryLayout];
                bl_Name = new string[selectedCategoryLayout];
                bl_PrintName = new string[selectedCategoryLayout];
                bl_Price = new string[selectedCategoryLayout];

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }
                for (int i = 0; i < selectedCategoryLayout; i++)
                {
                    string prdName = "";
                    int prdPrice = 0;
                    string prdImageUrl = "";
                    int prdLimitedCnt = 0;
                    int soldFlag = 0;

                    SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                    string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID and CardNumber=@CardNumber ORDER BY CardNumber";
                    sqlite_cmd.CommandText = queryCmd;
                    sqlite_cmd.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                    sqlite_cmd.Parameters.AddWithValue("@CardNumber", i);

                    SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        productIDArray[i] = sqlite_datareader.GetInt32(0);
                        realProductIDArray[i] = sqlite_datareader.GetInt32(2);
                        prdName = sqlite_datareader.GetString(3);
                        bl_PrintName[i] = sqlite_datareader.GetString(4);
                        productTime = sqlite_datareader.GetString(5);
                        bl_Name[i] = prdName;
                        prdPrice = sqlite_datareader.GetInt32(8);
                        bl_Price[i] = prdPrice.ToString();
                        prdLimitedCnt = sqlite_datareader.GetInt32(9);
                        productLimitedCntArray[selectedCategoryIndex, i] = prdLimitedCnt;
                        prdImageUrl = Path.Combine(currentDir, sqlite_datareader.GetString(16));
                        p_url[i] = Path.Combine(currentDir, sqlite_datareader.GetString(11));
                        soldFlag = sqlite_datareader.GetInt32(17);
                        if (productRestAmountArray[selectedCategoryIndex, i] == -1)
                        {
                            SQLiteDataReader sqlite_datareader1;
                            SQLiteCommand sqlite_cmd1;
                            sqlite_cmd1 = sqlite_conn.CreateCommand();
                            string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and sumFlag='false' and limitFlag=0";
                            sqlite_cmd1.CommandText = queryCmd1;
                            sqlite_cmd1.Parameters.AddWithValue("@CategoryID", selectedCategoryID);
                            sqlite_cmd1.Parameters.AddWithValue("@prdID", realProductIDArray[i]);

                            sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                            while (sqlite_datareader1.Read())
                            {
                                if (productLimitedCntArray[selectedCategoryIndex, i] != 0)
                                {
                                    if (!sqlite_datareader1.IsDBNull(0))
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i] - sqlite_datareader1.GetInt32(0);
                                    }
                                    else
                                    {
                                        productRestAmountArray[selectedCategoryIndex, i] = productLimitedCntArray[selectedCategoryIndex, i];
                                    }
                                }
                                else
                                {
                                    productRestAmountArray[selectedCategoryIndex, i] = 10000;
                                }
                            }
                            sqlite_datareader1.Close();
                            sqlite_datareader1 = null;
                            sqlite_cmd1.Dispose();
                            sqlite_cmd1 = null;

                        }
                        break;
                    }

                    sqlite_datareader.Close();
                    sqlite_datareader = null;
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;

                    int x = i % nWD;
                    int yy = i / nWD;
                    int d = 20;
                    if (selectedCategoryLayout == 10) d = 50;
                    BackgroundBitmap = null;

                    if (prdImageUrl != "" && prdImageUrl != null)
                    {
                        Panel p = createPanel.CreatePanelForProducts(x * (w1 + d), yy * (h1 + 20), w1, h1, i.ToString(), true, ColorTranslator.FromHtml(categoryBackColor[selectedCategoryIndex]), Color.Transparent);
                        borderPen = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
                        p.Paint += new PaintEventHandler(Panelbordercolor_Paint);


                        pb_Image[i] = createPanel.CreatePictureBox(3, 3, p.Width - 6, p.Height - 6, i.ToString(), null);
                        using (BackgroundBitmap = new Bitmap(prdImageUrl))
                        {
                            pb_Image[i].Image = new Bitmap(BackgroundBitmap);
                        }
                        if(BackgroundBitmap != null)
                        {
                            BackgroundBitmap.Dispose();
                            BackgroundBitmap = null;
                        }
                        pb_Image[i].Enabled = true;


                        if (soldFlag == 1 || (prdLimitedCnt != 0 && productRestAmountArray[selectedCategoryIndex, i] <= 0))
                        {
                            PictureBox badge_pb = new PictureBox();
                            badge_pb.Size = new Size(pb_Image[i].Width, pb_Image[i].Height);
                            badge_pb.Location = new Point(0, 0);
                            Bitmap tempBitmap = null;
                            using (tempBitmap = new Bitmap(constants.soldoutBadge))
                            {
                                badge_pb.Image = new Bitmap(tempBitmap);
                            }
                            if(tempBitmap != null)
                            {
                                tempBitmap.Dispose();
                                tempBitmap = null;
                            }
                            pb_Image[i].Controls.Add(badge_pb);
                            pb_Image[i].Enabled = false;

                        }
                        else
                        {
                            string[] dayTimeArr = productTime.Split('-');
                            if (dayTimeArr[0] != "24:00" && dayTimeArr[1] != "24:00")
                            {
                                bool saleAvailable = constants.SaleAvailable(dayTimeArr[0], dayTimeArr[1]);
                                if (!saleAvailable)
                                {
                                    PictureBox badge_pb = new PictureBox();
                                    badge_pb.Size = new Size(pb_Image[i].Width / 2, pb_Image[i].Height / 2);
                                    badge_pb.Location = new Point(pb_Image[i].Width / 2, 20);
                                    badge_pb.SizeMode = PictureBoxSizeMode.StretchImage;
                                    Bitmap tempBitmap = null;
                                    using (tempBitmap = new Bitmap(constants.preparingBadge))
                                    {
                                        badge_pb.Image = new Bitmap(tempBitmap);
                                    }
                                    if (tempBitmap != null)
                                    {
                                        tempBitmap.Dispose();
                                        tempBitmap = null;
                                    }
                                    pb_Image[i].Controls.Add(badge_pb);
                                    pb_Image[i].Enabled = false;
                                }
                            }
                        }

                        pb_Image[i].Click += new EventHandler(OnItemPicClk);

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

                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_*_3", ex.ToString());
                return;
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
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
                    Pen pp = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
                    Graphics g = e.Graphics;
                    g.DrawLine(pp, 0, 1, nWidth3, 1);
                    g.DrawLine(pp, 1, 0, 1, nHeight3);
                    g.DrawLine(pp, nWidth3 - 2, 0, nWidth3 - 2, nHeight3);
                    g.DrawLine(pp, 0, nHeight3 - 2, nWidth3, nHeight3 - 2);
                    g.Dispose();
                }
            }
        }

        /** ================= Ordering Start =============================== **/
        /**
         * Ordering product when product image item from product list panel
         **/
        public void OnItemPicClk(object sender, EventArgs e)
        {
            try
            {
                initCounter = 0;
                if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
                {
                    initFlag = true;
                }
                else
                {
                    initFlag = false;
                }
                initFlag = true;

                if (HDCheck && comModule == null)
                    return;
                if (HDCheck && (messageDialog.DialogFormGlobal != null || isError))
                    return;

                bLoad = true;
                PictureBox p = (PictureBox)sender;
                int index = int.Parse(p.Name);
                //ClickEvent(index);
                selectedPrdIndex = index;

                int chkIndex = Array.IndexOf(orderProductIDArray, productIDArray[selectedPrdIndex]);
                if (currentSelectedId > 7 && chkIndex == -1)
                {
                    messageDialog.OrderOutMessage();
                    //MessageBox.Show("order amount is out of 10.");
                    return;
                }

                if (productRestAmountArray[selectedCategoryIndex, index] <= 0 && productLimitedCntArray[selectedCategoryIndex, index] != 0)
                {
                    messageDialog.RestEmptyMessage();
                    return;
                }
                else
                {
                    string audio_file_name = dbClass.AudioFile("ValidItemTouch");
                    this.PlaySound(audio_file_name);
                    int currentAmount = 0;
                    List<string> optionArr = new List<string>();
                    if (chkIndex != -1)
                    {
                        currentAmount = int.Parse(orderAmountArray[chkIndex]);
                    }
                    if (currentAmount < PurchaseAmount)
                    {
                        ClickEvent(index);
                        string[] orderParams = new string[] { bl_Name[index], "1", bl_Price[index], bl_Price[index], bl_PrintName[index] };
                        SetVal(orderParams);
                    }
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_5", ex.ToString());
                return;
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
                borderPen = new Pen(ColorTranslator.FromHtml("#FFFFFF"), 3);
                p_ProductCon[cur].Name = index.ToString();
                p_ProductCon[curProduct == -1 ? 0 : curProduct].Invalidate(true);

                curProduct = index;
                borderPen = new Pen(ColorTranslator.FromHtml("#FF0000"), 3);
                p_ProductCon[curProduct].Invalidate(true);
                p_ProductCon[cur].Name = cur.ToString();
            }
        }

        /**
         * Getting ordering data from OrderDialog 
         **/
        public void SetVal(string[] s)
        {
            string[] msgValue = s;

            if (comModule != null && !comModule.bPermit)
            {
                comModule.PermitCommand();
                currentbPermit = true;
            }

            int index = Array.IndexOf(orderProductIDArray, productIDArray[selectedPrdIndex]);
            if (index != -1)
            {
                productRestAmountArray[selectedCategoryIndex, selectedPrdIndex]--;
                orderTotalPrice += int.Parse(orderProductPriceArray[index]);
                orderAmountArray[index] = (int.Parse(orderAmountArray[index]) + 1).ToString();

                orderPriceTotalLabel.Text = orderTotalPrice.ToString();
                orderAmountTotalLabel.Text = SumArray(orderAmountArray).ToString();
                CreateOrderTable();
            }
            else
            {
                orderProductNameArray[currentSelectedId] = msgValue[0];
                orderPrintNameArray[currentSelectedId] = msgValue[4];
                orderAmountArray[currentSelectedId] = msgValue[1];
                orderProuctIndexArray[currentSelectedId] = selectedPrdIndex;
                productRestAmountArray[selectedCategoryIndex, selectedPrdIndex] -= int.Parse(orderAmountArray[currentSelectedId]);
                orderProductPriceArray[currentSelectedId] = msgValue[3];
                orderProductIDArray[currentSelectedId] = productIDArray[selectedPrdIndex];
                orderCategoryIDArray[currentSelectedId] = selectedCategoryID;
                orderCategoryIndexArray[currentSelectedId] = selectedCategoryIndex;
                orderRealProductIDArray[currentSelectedId] = realProductIDArray[selectedPrdIndex];
                orderProductNameLabel[currentSelectedId].Text = msgValue[0];
                orderAmountLabel[currentSelectedId].Text = msgValue[1];
                orderTotalPrice += int.Parse(msgValue[2]);
                orderPriceTotalLabel.Text = orderTotalPrice.ToString();
                orderAmountTotalLabel.Text = SumArray(orderAmountArray).ToString();
                //orderPriceEnterLabel.Text = "15000";
                currentSelectedId++;
            }
        }

        
        /**
         * Deleting order item data from order list
         **/

        private void OrderDelete(object sender, EventArgs e)
        {
            initCounter = 0;
            if (orderPriceEnterLabel.Text == "" || int.Parse(orderPriceEnterLabel.Text) == 0)
            {
                initFlag = true;
            }
            else
            {
                initFlag = false;
            }
            initFlag = true;

            if (HDCheck && comModule == null)
                return;
            if (HDCheck && (messageDialog.DialogFormGlobal != null || isError))
                return;
            Button tempBtn = (Button)sender;
            string[] btnName = tempBtn.Name.Split('_');
            int selectedIndex = int.Parse(btnName[1]);
            OrderDeleteRun(selectedIndex);
        }

        public void OrderDeleteRun(int selectedIndex)
        {
            string audio_file_name = this.dbClass.AudioFile("DeleteTouch");
            this.PlaySound(audio_file_name);
            if (orderAmountArray[selectedIndex] != null && int.Parse(orderAmountArray[selectedIndex]) > 0)
            {
                currentSelectedId--;
                if (currentSelectedId >= 0)
                {
                    int selectedPrdIndex = orderProuctIndexArray[selectedIndex];
                    productRestAmountArray[selectedCategoryIndex, selectedPrdIndex] += int.Parse(orderAmountArray[selectedIndex]);
                    orderTotalPrice -= int.Parse(orderProductPriceArray[selectedIndex]) * int.Parse(orderAmountArray[selectedIndex]);
                    orderProductIDArray = orderProductIDArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<int>(ref orderProductIDArray, 8);

                    orderProductNameArray = orderProductNameArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<string>(ref orderProductNameArray, 8);

                    orderPrintNameArray = orderPrintNameArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<string>(ref orderPrintNameArray, 8);

                    orderProductPriceArray = orderProductPriceArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<string>(ref orderProductPriceArray, 8);

                    orderAmountArray = orderAmountArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<string>(ref orderAmountArray, 8);

                    orderProuctIndexArray = orderProuctIndexArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<int>(ref orderProuctIndexArray, 8);

                    orderCategoryIDArray = orderCategoryIDArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<int>(ref orderCategoryIDArray, 8);

                    orderCategoryIndexArray = orderCategoryIndexArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<int>(ref orderCategoryIndexArray, 8);

                    orderRealProductIDArray = orderRealProductIDArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<int>(ref orderRealProductIDArray, 8);

                    orderProductOptionArray = orderProductOptionArray.Where((val, idx) => idx != selectedIndex).ToArray();
                    Array.Resize<List<string>>(ref orderProductOptionArray, 8);

                    if (orderTotalPrice == 0)
                    {
                        orderPriceTotalLabel.Text = "";
                        orderAmountTotalLabel.Text = "";
                    }
                    else
                    {
                        orderAmountTotalLabel.Text = SumArray(orderAmountArray).ToString();
                        orderPriceTotalLabel.Text = orderTotalPrice.ToString();
                    }
                    CreateOrderTable();
                }
            }
        }
        /**
         * Load order list data in order data table
         **/
        private void CreateOrderTable()
        {

            for (int i = 0; i < 8; i++)
            {

                orderProductNameLabel[i].Name = "prd_" + i;
                TempLabel = orderProductNameLabel[i];
                if (orderProductNameArray.Length > i)
                    SetText(orderProductNameArray[i]);
                else
                    SetText("");
                //orderProductNameLabel[i].Text = orderProductNameArray[i + startIndex];
                orderAmountLabel[i].Name = "prdNum_" + i;
                TempLabel = orderAmountLabel[i];
                if (orderAmountArray.Length > i)
                    SetText(orderAmountArray[i]);
                else SetText("");
                //orderAmountLabel[i].Text = orderAmountArray[i + startIndex];
                orderDeleteLabel[i].Name = "del_" + i;
            }

        }

        private void InitState()
        {
            if (comModule != null && comModule.bPermit)
            {
                comModule.PermitStopCommand();
                currentbPermit = false;
            }
            Thread.Sleep(500);
            if (banknotGlobalLabel.InvokeRequired)
            {
                banknotGlobalLabel.Invoke((MethodInvoker)delegate ()
                {
                    using (Bitmap img = new Bitmap(constants.depositeDisableButton))
                    {
                        banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                    }
                });
            }
            else
            {
                using (Bitmap img = new Bitmap(constants.depositeDisableButton))
                {
                    banknotGlobalLabel.BackgroundImage = new Bitmap(img);
                }
            }

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            for (int i = 0; i < categoryDisAmount; i++)
            {
                SQLiteCommand sqlite_cmd;
                SQLiteDataReader sqlite_datareader;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);

                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    SQLiteDataReader sqlite_datareader1;
                    SQLiteCommand sqlite_cmd1;
                    sqlite_cmd1 = sqlite_conn.CreateCommand();
                    string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and limitFlag=0";
                    sqlite_cmd1.CommandText = queryCmd1;
                    sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);
                    sqlite_cmd1.Parameters.AddWithValue("@prdID", sqlite_datareader.GetInt32(2));

                    sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                    while (sqlite_datareader1.Read())
                    {
                        if (productLimitedCntArray[i, k] != 0)
                        {
                            if (!sqlite_datareader1.IsDBNull(0))
                            {
                                productRestAmountArray[i, k] = productLimitedCntArray[i, k] - sqlite_datareader1.GetInt32(0);
                            }
                            else
                            {
                                productRestAmountArray[i, k] = productLimitedCntArray[i, k];
                            }
                        }
                        else
                        {
                            productRestAmountArray[i, k] = 10000;
                        }
                    }
                    sqlite_datareader1.Close();
                    sqlite_datareader1 = null;
                    sqlite_cmd1.Dispose();
                    sqlite_cmd1 = null;
                    k++;
                }
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;

            }

            orderCategoryIDArray = new int[8];
            orderProductIDArray = new int[8];
            orderProductNameArray = new string[8];
            orderPrintNameArray = new string[8];
            orderProductPriceArray = new string[8];
            orderAmountArray = new string[8];
            orderProuctIndexArray = new int[8];
            orderProductOptionArray = new List<string>[8];
            TempLabel = orderPriceTotalLabel;
            SetText("");
            TempLabel = orderAmountTotalLabel;
            SetText("");
            TempLabel = orderRestPriceLabel;
            SetText("");

            orderTotalPrice = 0;
            currentSelectedId = 0;
            CreateOrderTable();
            using(Bitmap img = new Bitmap(constants.ticketDisableButton))
            {
                ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
            }
            using (Bitmap img = new Bitmap(constants.receiptDisableButton))
            {
                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
            }
            backshowEnable = true;
            //sqlite_conn.Close();
            sqlite_conn.Dispose();
            sqlite_conn = null;

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.CreateCategoryList();
                    this.HideSaleScreen();
                });
            }
            else
            {
                this.CreateCategoryList();
                this.HideSaleScreen();
            }
        }


        /**
         * Cancel Ordering
         **/

        private void CancelOrder(object sender, EventArgs e)
        {
            CancelPrepare();
        }

        private void CancelPrepare()
        {
            if (HDCheck && comModule == null)
                return;
            if (HDCheck && (messageDialog.DialogFormGlobal != null || isError))
                return;
            bOrderCancel = true;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            CreateDialogForm();
        }

        public void OrderCancelRun()
        {
            currentbPermit = false;
            int cancelAmount = orderPriceEnterLabel.Text == "" ? 0 : int.Parse(orderPriceEnterLabel.Text);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            for (int i = 0; i < categoryDisAmount; i++)
            {
                SQLiteCommand sqlite_cmd;
                SQLiteDataReader sqlite_datareader;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID ORDER BY CardNumber";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);

                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    SQLiteDataReader sqlite_datareader1;
                    SQLiteCommand sqlite_cmd1;
                    sqlite_cmd1 = sqlite_conn.CreateCommand();
                    string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and limitFlag=0";
                    sqlite_cmd1.CommandText = queryCmd1;
                    sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);
                    sqlite_cmd1.Parameters.AddWithValue("@prdID", sqlite_datareader.GetInt32(2));

                    sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                    while (sqlite_datareader1.Read())
                    {
                        if (productLimitedCntArray[i, k] != 0)
                        {
                            if (!sqlite_datareader1.IsDBNull(0))
                            {
                                productRestAmountArray[i, k] = productLimitedCntArray[i, k] - sqlite_datareader1.GetInt32(0);
                            }
                            else
                            {
                                productRestAmountArray[i, k] = productLimitedCntArray[i, k];
                            }
                        }
                        else
                        {
                            productRestAmountArray[i, k] = 10000;
                        }
                    }
                    sqlite_datareader1.Close();
                    sqlite_datareader1 = null;
                    sqlite_cmd1.Dispose();
                    sqlite_cmd1 = null;
                    k++;
                }
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;

            }

            orderCategoryIDArray = new int[8];
            orderProductIDArray = new int[8];
            orderProductNameArray = new string[8];
            orderPrintNameArray = new string[8];
            orderProductPriceArray = new string[8];
            orderAmountArray = new string[8];
            orderProuctIndexArray = new int[8];
            orderProductOptionArray = new List<string>[8];
            TempLabel = orderPriceTotalLabel;
            SetText("");
            TempLabel = orderAmountTotalLabel;
            SetText("");
            TempLabel = orderRestPriceLabel;
            SetText("");

            orderTotalPrice = 0;
            currentSelectedId = 0;
            CreateOrderTable();
            backshowEnable = true;
            using(Bitmap img = new Bitmap(constants.ticketDisableButton))
            {
                ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
            }
            using(Bitmap img = new Bitmap(constants.receiptDisableButton))
            {
                receiptButtonGlobal.BackgroundImage = new Bitmap(img);
            }
            //sqlite_conn.Close();
            sqlite_conn.Dispose();
            sqlite_conn = null;
            this.CreateCategoryList();
            this.HideSaleScreen();
            try {
                constants.SaveLogData("saleScreen_cancel3", cancelAmount.ToString());
                comModule.OrderCancel(cancelAmount);
            }
            catch(Exception ex)
            {
                constants.SaveLogData("saleScreen_cancel", ex.ToString());
                BackShow();
            }
        }

        Form DialogForms = null;

        private void CreateDialogForm()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 4);
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

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "loadingMessageLabel", constants.loadingMessage, 50, 0, mainPanel.Width - 100, mainPanel.Height / 2 - 10, Color.Transparent, Color.Black, 18, false, ContentAlignment.BottomCenter);
            Label messageLabel_1000 = createLabel.CreateLabelsInPanel(mainPanel, "loadingMessageLabel", constants.loadingMessage_1000, 50, mainPanel.Height / 2, mainPanel.Width - 100, 60, Color.Transparent, Color.FromArgb(255, 0, 0, 255), 18, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);
            if (backgroundWorker.IsBusy != true)
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
                backgroundWorker.RunWorkerAsync();
            }

            DialogForms = dialogForm;
            DialogForms.ShowDialog();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    OrderCancelRun();
                });
            }
            else
            {
                OrderCancelRun();
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogForms.Close();
            DialogForms.Visible = false;
            DialogForms = null;
            backgroundWorker.CancelAsync();
        }
        /** ================ Ordering End ============================== **/
        /** ================ Ticketing Start =========================== **/
        /**
        * Ticketing dialog open
        **/
        private void ShowTicketing(object sender, EventArgs e)
        {
            try
            {
                constants.SaveLogData("saleScreen_6", "saleScreen ticketing start");
                if (HDCheck && comModule == null)
                    return;
                if (HDCheck && (messageDialog.DialogFormGlobal != null || isError))
                    return;

                receiptPrinter = "";
                int orderRestPrice = -1;
                if (orderPriceEnterLabel.Text != "" && orderPriceTotalLabel.Text != "")
                {
                    orderRestPrice = int.Parse(orderPriceEnterLabel.Text) - int.Parse(orderPriceTotalLabel.Text);
                }

                if (orderRestPrice >= 0)
                {
                    orderCancelButtonGlobal.Enabled = false;
                    using(Bitmap img = new Bitmap(constants.orderCancelDisableButton))
                    {
                        orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                    }

                    string audio_file_name = dbClass.AudioFile("TicketValid");
                    this.PlaySound(audio_file_name);

                    //orderDialog.ShowTicketingDetail(orderProductNameArray, orderProductPriceArray, orderAmountArray, currentSelectedId);
                    orderDialog.TicketingRunDialog();
                }
                else
                {
                    string audio_file_name = dbClass.AudioFile("TicketDisable");
                    this.PlaySound(audio_file_name);
                    return;
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_6", ex.ToString());
                return;
            }

        }

        /**
        * Ticketing dialog open
        **/
        private void ShowTicketingWithReciept(object sender, EventArgs e)
        {
            try
            {
                constants.SaveLogData("saleScreen_6_r", "saleScreen ticketing start with recieption");
                if (HDCheck && comModule == null)
                    return;
                if (HDCheck && (messageDialog.DialogFormGlobal != null || isError))
                    return;

                receiptPrinter = "receipt";
                int orderRestPrice = -1;
                if (orderPriceEnterLabel.Text != "" && orderPriceTotalLabel.Text != "")
                {
                    orderRestPrice = int.Parse(orderPriceEnterLabel.Text) - int.Parse(orderPriceTotalLabel.Text);
                }


                if (orderRestPrice >= 0)
                {
                    orderCancelButtonGlobal.Enabled = false;
                    using(Bitmap img = new Bitmap(constants.orderCancelDisableButton))
                    {
                        orderCancelButtonGlobal.BackgroundImage = new Bitmap(img);
                    }

                    string audio_file_name = dbClass.AudioFile("TicketValid");
                    this.PlaySound(audio_file_name);

                    orderDialog.TicketingRunDialog();

                }
                else
                {
                    string audio_file_name = dbClass.AudioFile("TicketDisable");
                    this.PlaySound(audio_file_name);
                    return;
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_6_r", ex.ToString());
                return;
            }
        }

        PaperSize paperSize = new PaperSize("papersize", 203, 800);//set the paper size

        public void TicketingMessageClose ()
        {
            if (orderDialog.DialogFormGlobal_2 != null)
            {
                orderDialog.DialogFormGlobal_2.Close();
                orderDialog.DialogFormGlobal_2.Visible = false;
                orderDialog.DialogFormGlobal_2 = null;
            }
            if (orderDialog.DialogFormGlobal != null)
            {
                orderDialog.DialogFormGlobal.Close();
                orderDialog.DialogFormGlobal.Hide();
                orderDialog.DialogFormGlobal = null;
            }
        }

        /**
         * Withdraw other coin, ticket printing command 
         **/
        public void Ticketing()
        {
            try
            {
                constants.SaveLogData("saleScreen_7", "saleScreen ticketing");
                bool bfinishedWithdraw = false;
                if (sumThread != null)
                {
                    sumThread.Abort();
                    sumThread = null;
                }

                using(Bitmap img = new Bitmap(constants.ticketDisableButton))
                {
                    ticketingButtonGlobal.BackgroundImage = new Bitmap(img);
                }
                using(Bitmap img = new Bitmap(constants.receiptDisableButton))
                {
                    receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                }

                ticketingButtonGlobal.Invalidate();
                receiptButtonGlobal.Invalidate();
                int iChange = int.Parse(orderPriceEnterLabel.Text) - int.Parse(orderPriceTotalLabel.Text);
                if (iChange >= 0)
                {
                    string audio_file_name = dbClass.AudioFile("TicketIssue");
                    this.PlaySound(audio_file_name);
                    try
                    {
                        bTicketing = true;
                        bfinishedWithdraw = comModule.TicketRun(iChange);
                        currentbPermit = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        bBackShow = true;
                        BackShow();
                    }
                }
                Thread.Sleep(500);

                if (bfinishedWithdraw && orderPriceTotalLabel.Text != "" )
                {
                    //Thread.Sleep(1000);
                    orderTotalTicketForReceipt = 0;  // currentSelectedId;
                    foreach (var k in orderAmountArray)
                        if (k != null)
                            orderTotalTicketForReceipt += int.Parse(k);  // currentSelectedId;
                    orderTotalPriceForReceipt = orderTotalPrice;
                    PrintRun();
                }

                if( comModule == null )
                    constants.SaveLogData("saleScreen_ticketing", "====>status of commodule is null///");

                bTicketing = false;
                bDispensorErr = false;

                if (comModule != null)
                {
                    if (!comModule.GetErrorStatus(0x07))
                    {
                        ErrorStatusAnal();
                        TicketingMessageClose();

                        if (bBackShow) return;

                        comModule.LogResult("Error occured :: ", "");
                        if (comModule.getDepositTh == null || bNoError)
                        {
                            try
                            {
                                comModule.processAfterWithdraw();
                                comModule.LogResult("When Error, Proccess run :: ", "");
                                if (comModule.getDepositTh == null)
                                {
                                    bBackShow = true;
                                    BackShow();
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                constants.SaveLogData("saleScreen_7_1", ex.ToString());
                                BackShow();
                                comModule.LogResult("Exception Error 1 :: ", "");
                                return;
                            }
                        }
                        InitErrStatusVariables();
                        InitState();

                        if (sumThread == null)
                        {
                            sumThread = new Thread(SumThreadRunings);
                            sumThread.SetApartmentState(ApartmentState.STA);
                            sumThread.Start();
                        }
                        return;
                    }

                    if (comModule.getDepositTh == null)
                    {
                        try
                        {
                            comModule.processAfterWithdraw();
                            comModule.LogResult("Normal process Run :: ", "");
                            if (comModule.getDepositTh == null)
                            {
                                bBackShow = true;
                                BackShow();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            constants.SaveLogData("saleScreen_7_2", ex.ToString());
                            bBackShow = true;
                            BackShow();
                            comModule.LogResult("Exception Error 2 :: ", "");
                            return;
                        }
                    }
                }
                else
                {
                    ResetComModue();
                    if (comModule.getDepositTh == null)
                    {
                        bBackShow = true;
                        BackShow();
                        return;
                    }
                }

                if (bBackShow) return;

                TicketingMessageClose();
                constants.SaveLogData("saleScreen_7_3", "saleScreen ticketing close");

                InitState();

                if (sumThread == null)
                {
                    sumThread = new Thread(SumThreadRunings);
                    sumThread.SetApartmentState(ApartmentState.STA);
                    sumThread.Start();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_7", ex.ToString());
                return;
            }
        }

        /*
         * Printing Ticket and Ordering data save and Receipt print command
         **/
        public void PrintRun()
        {
            try
            {
                if ( !bDispensorErr )
                {
                    constants.SaveLogData("saleScreen_8", "saleScreen print");
                    if (key != null)
                    {
                        if (key.GetValue("currentTicketNo") == null)
                        {
                            key.SetValue("currentTicketNo", 0);
                        }
                        currentTicketNo = Convert.ToInt32(key.GetValue("currentTicketNo")) + 1;
                        if ((long)currentTicketNo > 10000000000)
                        {
                            key.SetValue("currentTicketNo", 0);
                            currentTicketNo = 1;
                        }
                        if (key.GetValue("currentSerialNo") == null)
                        {
                            key.SetValue("currentSerialNo", StartSerialNo - 1);
                        }
                        currentSerialNo = Convert.ToInt32(key.GetValue("currentSerialNo")) + 1;
                        if (currentSerialNo > 10000)
                        {
                            key.SetValue("currentSerialNo", StartSerialNo - 1);
                            currentSerialNo = 1;
                        }
                    }

                    orderTotalAmount = SumArray(orderAmountArray);

                    bool printChk = true;
                    USBPrint();
                    Thread.Sleep(1000);
                    printChk = USBPrintStatus();
                    constants.SaveLogData("saleScreen_****", "after printing usb==" + printChk);

                    if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
                    {
                        if(ipAddress1 != "" && port1 != "")
                        {
                            ipAddress = ipAddress1;
                            if (TcpPrintStatus())
                            {
                                KichenPrint();
                                printChk = TcpPrintStatus(1);
                            }
                        }
                        else if(ipAddress2 != "" && port2 != "")
                        {
                            ipAddress = ipAddress2;
                            if (TcpPrintStatus())
                            {
                                KichenPrint();
                                printChk = TcpPrintStatus(1);
                            }
                        }
                        else if(ipAddress3 != "" && port3 != "")
                        {
                            ipAddress = ipAddress3;
                            if (TcpPrintStatus())
                            {
                                KichenPrint();
                                printChk = TcpPrintStatus(1);
                            }
                        }
                    }

                    if (printChk)
                    {
                        Thread.Sleep(500);
                        OrderDataSaving();

                        if (receiptPrinter == "receipt")
                        {
                            Thread.Sleep(500);
                            ReceiptRun();
                        }
                    }
                }

                SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }

                DateTime now = DateTime.Now;
                string week = now.ToString("ddd");
                string currentTime = now.ToString("HH:mm");
                for (int i = 0; i < categoryDisAmount; i++)
                {
                    SQLiteCommand sqlite_cmd;
                    SQLiteDataReader sqlite_datareader;
                    sqlite_cmd = sqlite_conn.CreateCommand();
                    string queryCmd = "SELECT * FROM " + constants.tbNames[2] + " WHERE CategoryID=@CategoryID ORDER BY CardNumber";
                    sqlite_cmd.CommandText = queryCmd;
                    sqlite_cmd.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);

                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    int k = 0;
                    while (sqlite_datareader.Read())
                    {
                        SQLiteDataReader sqlite_datareader1;
                        SQLiteCommand sqlite_cmd1;
                        sqlite_cmd1 = sqlite_conn.CreateCommand();
                        string queryCmd1 = "SELECT SUM(prdAmount) as prdRestAmount FROM " + constants.tbNames[3] + " WHERE categoryID=@CategoryID and prdRealID=@prdID and limitFlag=0";
                        sqlite_cmd1.CommandText = queryCmd1;
                        sqlite_cmd1.Parameters.AddWithValue("@CategoryID", categoryIDArray[i]);
                        sqlite_cmd1.Parameters.AddWithValue("@prdID", sqlite_datareader.GetInt32(2));

                        sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                        while (sqlite_datareader1.Read())
                        {
                            if (productLimitedCntArray[selectedCategoryIndex, i] != 0)
                            {
                                if (!sqlite_datareader1.IsDBNull(0))
                                {
                                    productRestAmountArray[i, k] = productLimitedCntArray[i, k] - sqlite_datareader1.GetInt32(0);
                                }
                                else
                                {
                                    productRestAmountArray[i, k] = productLimitedCntArray[i, k];
                                }
                            }
                            else
                            {
                                productRestAmountArray[i, k] = 10000;
                            }

                        }

                        sqlite_datareader1.Close();
                        sqlite_datareader1 = null;
                        sqlite_cmd1.Dispose();
                        sqlite_cmd1 = null;
                        k++;
                    }

                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                    sqlite_datareader.Close();
                    sqlite_datareader = null;
                }

                orderCategoryIDArray = new int[8];
                orderProductIDArray = new int[8];
                orderProductNameArray = new string[8];
                orderPrintNameArray = new string[8];
                orderProductPriceArray = new string[8];
                orderAmountArray = new string[8];
                orderProuctIndexArray = new int[8];
                orderProductOptionArray = new List<string>[8];
                TempLabel = orderPriceEnterLabel;
                SetText("");
                TempLabel = orderPriceTotalLabel;
                SetText("");
                TempLabel = orderAmountTotalLabel;
                SetText("");
                TempLabel = orderRestPriceLabel;
                SetText("");
                //orderPriceTotalLabel.Text = "";
                orderTotalPrice = 0;
                currentSelectedId = 0;
                CreateOrderTable();

                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
                bDispensorErr = false;

            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_8", ex.ToString());
                Console.WriteLine("printer Exception : " + ex);
            }
        }

        private void EndPrintPage(object sender, PrintEventArgs e)
        {
            lineNumbers = 0;
            ticketNumber = 0;
            ticketNumbers = 0;

        }

        int sublines = 0;
        int ticketNumber = 0;
        int ticketNumbers = 0;

        private void Ticket_Print_USB()
        {
            try
            {
                DateTime now = DateTime.Now;
                int a;
                byte[] bytes;
                a = PrintCmd(0x1B);
                a = PrintStr("@");                      //Reset Printer

                Thread.Sleep(300);
                while (lineNumbers <= currentSelectedId)
                {
                    if (lineNumbers > 0 || lineNumbers == 0 && receiptPrinter == "")
                    {
                        if (lineNumbers == 0 && receiptPrinter == "")
                        {
                            lineNumbers++;
                        }
                        sublines = 0;
                        while (sublines < int.Parse(orderAmountArray[lineNumbers - 1]))
                        {

                            a = PrintCmd(0x1A);
                            a = PrintCmd(0x78);
                            a = PrintCmd(0x00);                      //Extended ascii mode 

                            a = PrintCmd(0x1B);
                            a = PrintStr("j");
                            a = PrintCmd(0x70);                      //back feed

                            a = PrintCmd(0x1B);
                            a = PrintStr("M");
                            a = PrintCmd(0x00);                      //Select Korean font

                            a = PrintCmd(0x1B);
                            a = PrintCmd(0x4D);
                            a = PrintCmd(0x20);                      //Select Japanese font

                            a = PrintCmd(0x1B);
                            a = PrintStr("3");
                            a = PrintCmd(0x32);                      //Set the interval of line

                            a = PrintCmd(0x1D);
                            a = PrintStr("L");
                            a = PrintCmd(0x32);
                            a = PrintCmd(0x00);                      //Select the left margin

                            a = PrintCmd(0x1D);
                            a = PrintStr("W");
                            a = PrintCmd(0x5C);
                            a = PrintCmd(0x01);                      //Select the printing width

                            a = PrintCmd(0x1B);
                            a = PrintStr("G");
                            a = PrintCmd(0x01);                     //Set the printing double for font thickness

                            a = PrintCmd(0x1B);
                            a = PrintStr("a");
                            a = PrintCmd(0x00);                      //set left align

                            bytes = Encoding.GetEncoding(932).GetBytes(now.ToString("yyyy/MM/dd HH:mm:ss"));

                            foreach (byte tempB in bytes)
                            {
                                a = PrintCmd(tempB);
                            }

                            if (SerialNo == 1)
                            {
                                //tempList.AddRange(Right);
                                bytes = Encoding.GetEncoding(932).GetBytes("     " + currentSerialNo.ToString("0000"));

                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }

                            }
                            a = PrintCmd(0x0A);

                            if (callNumbering)
                            {
                                callNumber++;
                                bytes = Encoding.GetEncoding(932).GetBytes("呼出番号: " + callNumber.ToString("00"));
                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }
                                bytes = Encoding.GetEncoding(932).GetBytes("             " + (ticketNumber + 1).ToString() + "/" + orderTotalAmount.ToString());

                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }
                            }
                            else
                            {
                                a = PrintCmd(0x1B);
                                a = PrintStr("a");
                                a = PrintCmd(0x02);                      //set right align

                                bytes = Encoding.GetEncoding(932).GetBytes((ticketNumber + 1).ToString() + "/" + orderTotalAmount.ToString());

                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }

                            }


                            a = PrintCmd(0x0A);

                            a = PrintCmd(0x1C);
                            a = PrintStr("S");
                            a = PrintCmd(0x05);
                            a = PrintCmd(0x05);                      //Select the space between Korean characters

                            a = PrintCmd(0x1B);
                            a = PrintStr("a");
                            a = PrintCmd(0x01);                      //center align

                            a = PrintCmd(0x1D);
                            a = PrintStr("!");
                            a = PrintCmd(0x01);                      // set font size twice vertical                


                            bytes = Encoding.GetEncoding(932).GetBytes(orderPrintNameArray[lineNumbers - 1]);

                            foreach (byte tempB in bytes)
                            {
                                a = PrintCmd(tempB);
                            }

                            a = PrintCmd(0x1D);
                            a = PrintStr("!");
                            a = PrintCmd(0x00);

                            a = PrintCmd(0x1C);
                            a = PrintStr("S");
                            a = PrintCmd(0x00);
                            a = PrintCmd(0x00);                      //Select the space between Korean characters

                            a = PrintCmd(0x0A);
                            a = PrintCmd(0x0A);

                            a = PrintCmd(0x1B);
                            a = PrintStr("a");
                            a = PrintCmd(0x02);

                            bytes = Encoding.GetEncoding(932).GetBytes(orderProductPriceArray[lineNumbers - 1] + constants.unit);

                            foreach (byte tempB in bytes)
                            {
                                a = PrintCmd(tempB);
                            }

                            a = PrintCmd(0x0A);

                            string validText = "当日のみ有効";
                            if (ValidDate != 0)
                            {
                                DateTime validDates = now.AddDays(2);
                                validText = validDates.ToString("yyyy/MM/dd") + "まで有効";

                            }

                            bytes = Encoding.GetEncoding(932).GetBytes(validText);
                            foreach (byte tempB in bytes)
                            {
                                a = PrintCmd(tempB);
                            }

                            a = PrintCmd(0x0A);


                            List<string> options = orderProductOptionArray[lineNumbers - 1];
                            if(options != null && options.Count > 0)
                            {
                                a = PrintCmd(0x1B);
                                a = PrintStr("a");
                                a = PrintCmd(0x00);

                                foreach (string option in options)
                                {
                                    bytes = Encoding.GetEncoding(932).GetBytes("    " + option);
                                    foreach (byte tempB in bytes)
                                    {
                                        a = PrintCmd(tempB);
                                    }

                                    a = PrintCmd(0x0A);
                                }
                            }
                            else
                            {
                                a = PrintCmd(0x1B);
                                a = PrintStr("a");
                                a = PrintCmd(0x01);

                                bytes = Encoding.GetEncoding(932).GetBytes(TicketMsg1);
                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }

                                a = PrintCmd(0x0A);

                                bytes = Encoding.GetEncoding(932).GetBytes(StoreRealName);
                                foreach (byte tempB in bytes)
                                {
                                    a = PrintCmd(tempB);
                                }

                                a = PrintCmd(0x0A);

                            }

                            //a = PrintCmd(0x1B);
                            //a = PrintStr("a");
                            //a = PrintCmd(0x02);

                            //bytes = Encoding.GetEncoding(932).GetBytes(currentTicketNo.ToString("0000000000"));
                            //foreach (byte tempB in bytes)
                            //{
                            //    a = PrintCmd(tempB);
                            //}
                            ticketNumber++;
                            sublines++;

                            a = PrintCmd(0x1B);
                            a = PrintStr("G");
                            a = PrintCmd(0x00);

                            a = PrintCmd(0x0A);
                            a = PrintCmd(0x0A);
                            a = PrintCmd(0x0A);
                            a = PrintCmd(0x0A);

                            a = PrintCmd(0x1B);
                            a = PrintStr("i");                                       //Full Cut
                        }

                        lineNumbers++;
                    }
                    else if (lineNumbers == 0 && receiptPrinter == "receipt")
                    {
                        a = PrintCmd(0x1A);
                        a = PrintCmd(0x78);
                        a = PrintCmd(0x00);                      //Extended ascii mode 

                        a = PrintCmd(0x1B);
                        a = PrintStr("j");
                        a = PrintCmd(0x70);                      //back feed

                        a = PrintCmd(0x1B);
                        a = PrintStr("M");
                        a = PrintCmd(0x00);                      //Select Korean font

                        a = PrintCmd(0x1B);
                        a = PrintCmd(0x4D);
                        a = PrintCmd(0x20);                      //Select Japanese font

                        a = PrintCmd(0x1B);
                        a = PrintStr("3");
                        a = PrintCmd(0x32);                      //Set the interval of line

                        a = PrintCmd(0x1B);
                        a = PrintStr("G");
                        a = PrintCmd(0x01);                      //set bold

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x01);                      //set center align

                        a = PrintCmd(0x1D);
                        a = PrintStr("!");
                        a = PrintCmd(0x01);                      // set font size twice vertical      

                        a = PrintCmd(0x1D);
                        a = PrintStr("L");
                        a = PrintCmd(0x32);
                        a = PrintCmd(0x00);                      //Select the left margin

                        a = PrintCmd(0x1D);
                        a = PrintStr("W");
                        a = PrintCmd(0x5B);
                        a = PrintCmd(0x01);                      //Select the printing width

                        a = PrintCmd(0x1C);
                        a = PrintStr("S");
                        a = PrintCmd(0x10);
                        a = PrintCmd(0x10);                      //Select the space between Korean characters

                        bytes = Encoding.GetEncoding(932).GetBytes(constants.receiptButtonText);

                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }

                        a = PrintCmd(0x1C);
                        a = PrintStr("S");
                        a = PrintCmd(0x00);
                        a = PrintCmd(0x00);                      //Select the space between Korean characters

                        a = PrintCmd(0x1D);
                        a = PrintStr("!");
                        a = PrintCmd(0x00);                      // set font size twice vertical                

                        a = PrintCmd(0x0A);
                        a = PrintCmd(0x0A);

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x01);                      //set right align

                        bytes = Encoding.GetEncoding(932).GetBytes("________________________様");

                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x01);                      //set center align

                        bytes = Encoding.GetEncoding(932).GetBytes(now.ToString("yyyy/MM/dd  HH:mm:ss"));

                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        bytes = Encoding.GetEncoding(932).GetBytes(orderTotalTicketForReceipt.ToString() + constants.amountUnit1 + "  " + constants.sumLabel + "  ¥" + orderTotalPriceForReceipt.ToString());
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        bytes = Encoding.GetEncoding(932).GetBytes(constants.receiptInstruction);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x02);                      //set right align

                        bytes = Encoding.GetEncoding(932).GetBytes("   " + StoreName);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        bytes = Encoding.GetEncoding(932).GetBytes(Address);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        bytes = Encoding.GetEncoding(932).GetBytes(PhoneNumber);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x01);                      //set center align

                        bytes = Encoding.GetEncoding(932).GetBytes(Other1);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }
                        a = PrintCmd(0x0A);

                        bytes = Encoding.GetEncoding(932).GetBytes(Other2);
                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }

                        a = PrintCmd(0x1B);
                        a = PrintStr("G");
                        a = PrintCmd(0x00);

                        lineNumbers++;
                        a = PrintCmd(0x0A);
                        a = PrintCmd(0x0A);
                        a = PrintCmd(0x0A);
                        a = PrintCmd(0x0A);

                        a = PrintCmd(0x1B);
                        a = PrintStr("i");                                       //Full Cut

                    }
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_***", "printing Error==>" + ex.ToString());

            }
        }

        public void USBPrint()
        {
            string err_title = "";
            printType = 0;
            try
            {
                int a = UsbOpen("HMK-060");
                int status = NewRealRead();
                constants.SaveLogData("saleScreen_***", "saleScreen print status==>" + status);
                Console.WriteLine("1==>USBPrint Start==>" + a);
                if (a != 0)
                {
                    Console.WriteLine("1_0===> USBPrint Start connect error==>" + a);
                    dbClass.InsertLog(2, "プリンター異常", "チケットプリンター通信エラー");
                    TicketingMessageClose();

                    string type = "チケット";
                    err_title = "チケットプリンター通信エラー ";
                    messageDialog.ShowPrintErrorMessage(type, 0, 0, 1, 1);
                }
                else
                {
                    if ((status >= 1 && status <= 15) || status == 32)
                    {
                        TicketingMessageClose();

                        int errStatus = status;
                        if (status == 32)
                        {
                            errStatus = 16;
                        }
                        string type = "チケット";
                        dbClass.InsertLog(2, "プリンター異常", "チケット" + constants.printErrTitle[errStatus]);

                        err_title = "チケット" + constants.printErrTitle[errStatus];
                        messageDialog.ShowPrintErrorMessage(type, 0, errStatus, 1, 1);
                    }
                    else
                    {
                        Console.WriteLine("2===>USBPrint printing prepare==>");
                        Ticket_Print_USB();
                        lineNumbers = 0;
                        ticketNumber = 0;
                        ticketNumbers = 0;

                    REPEATCOMMAND:
                        if (!USBPrintStatus(1))
                        {
                            ShowPrintErrorMsgs(1);
                        }
                        if (printStatus == 16)
                        {
                            goto REPEATCOMMAND;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                constants.SaveLogData("saleScreen_***", e.ToString());
                dbClass.InsertLog(2, "プリンター異常", "チケット" + constants.printErrTitle[17]);
                Console.WriteLine("printer dll error" + e.ToString());
                messageDialog.ShowPrintErrorMessage("チケット", 0, 17, 1, 1);
            }
        }

        public void KichenPrint()
        {
            printType = 1;
            try
            {
                byte[] data = GetPrintData();
                TcpPrint(ipAddress, port, data);
            }
            catch (Exception er)
            {
                constants.SaveLogData("saleScreen_***", er.ToString());
                dbClass.InsertLog(2, "プリンター異常", "キッチンプリンター通信エラー");
                messageDialog.ShowPrintErrorMessage("キッチン", 1, 17, 1, 1);
            }
        }

        private byte[] GetPrintData()
        {
            DateTime now = DateTime.Now;

            byte[] reset = new byte[] { 0x1B, 0x40 };
            byte[] JapaneseFont = new byte[] { 0x1B, 0x4D, 0x22 };
            byte[] Korean = new byte[] { 0x1B, 0x4D, 0x00 };
            byte[] Linefeed = new byte[] { 0x0A };
            byte[] Linespace = new byte[] { 0x1B, 0x33, 0x48 };
            byte[] LeftMargin = new byte[] { 0x1D, 0x4C, 0x50, 0x00 };
            byte[] PrintArea = new byte[] { 0x1D, 0x57, 0x70, 0x01 };
            byte[] ExtensionGraphic = new byte[] { 0x1A, 0x78, 0x00 };
            byte[] RightSpace = new byte[] { 0x1B, 0x20, 0x10 };
            byte[] RightSpace_release = new byte[] { 0x1B, 0x20, 0x00 };
            byte[] sendBytes;
            byte[] Left = new byte[3] { 0x1b, 0x61, 0x00 };
            byte[] Center = new byte[3] { 0x1b, 0x61, 0x01 };
            byte[] Right = new byte[3] { 0x1b, 0x61, 0x02 };
            //byte[] Cut_full = new byte[] { 0x1D, 0x56, 0x00 };
            //byte[] Cut_partial = new byte[] { 0x1D, 0x56, 0x01 };
            byte[] Cut_full = new byte[] { 0x1B, 0x69 };
            byte[] Cut_partial = new byte[] { 0x1B, 0x6D };
            byte[] print_speed = new byte[] { 0x1A, 0x73, 0x08 };
            byte[] print_feed = new byte[] { 0x1B, 0x64, 0x03 };
            byte[] Bold = new byte[] { 0x1B, 0x45, 0x01 };
            byte[] Emphasize = new byte[] { 0x1B, 0x47, 0x01 };
            byte[] Emphasize_release = new byte[] { 0x1B, 0x47, 0x00 };
            byte[] Extension = new byte[] { 0x1D, 0x21, 0x01 };
            byte[] Extension_release = new byte[] { 0x1D, 0x21, 0x00 };



            List<byte> tempList = new List<byte>();
            int lineNo = 0;

            while (lineNo < currentSelectedId)
            {
                int subline = 0;
                while (subline < int.Parse(orderAmountArray[lineNo]))
                {
                    tempList.AddRange(ExtensionGraphic);
                    tempList.AddRange(JapaneseFont);
                    tempList.AddRange(Linespace);
                    tempList.AddRange(LeftMargin);
                    tempList.AddRange(PrintArea);
                    tempList.AddRange(Emphasize);
                    tempList.AddRange(Left);

                    sendBytes = Encoding.GetEncoding(932).GetBytes(now.ToString("yyyy/MM/dd  HH:mm:ss"));
                    tempList.AddRange(sendBytes);


                    if (SerialNo == 1)
                    {
                        sendBytes = Encoding.GetEncoding(932).GetBytes("                " + currentSerialNo.ToString("0000"));
                        tempList.AddRange(sendBytes);
                        tempList.AddRange(Linefeed);
                    }

                    if (callNumbering)
                    {
                        callNumbers++;
                        tempList.AddRange(Left);
                        sendBytes = Encoding.GetEncoding(932).GetBytes("呼出番号: " + callNumbers.ToString("00"));
                        tempList.AddRange(sendBytes);

                        sendBytes = Encoding.GetEncoding(932).GetBytes("                          " + (ticketNumbers + 1).ToString() + "/" + orderTotalAmount.ToString());
                        tempList.AddRange(sendBytes);
                        tempList.AddRange(Linefeed);
                    }
                    else
                    {
                        tempList.AddRange(Right);
                        sendBytes = Encoding.GetEncoding(932).GetBytes((ticketNumbers + 1).ToString() + "/" + orderTotalAmount.ToString());
                        tempList.AddRange(sendBytes);
                        tempList.AddRange(Linefeed);
                    }
                    tempList.AddRange(Center);
                    tempList.AddRange(Extension);
                    tempList.AddRange(RightSpace);
                    sendBytes = Encoding.GetEncoding(932).GetBytes(orderPrintNameArray[lineNo]);
                    tempList.AddRange(sendBytes);
                    tempList.AddRange(RightSpace_release);
                    tempList.AddRange(Extension_release);
                    tempList.AddRange(Linefeed);
                    tempList.AddRange(Linefeed);

                    sendBytes = Encoding.GetEncoding(932).GetBytes(orderProductPriceArray[lineNo] + constants.unit);
                    tempList.AddRange(sendBytes);
                    tempList.AddRange(Linefeed);

                    List<string> options = orderProductOptionArray[lineNo];
                    if(options != null && options.Count > 0)
                    {
                        tempList.AddRange(Left);
                        foreach(string option in options)
                        {
                            sendBytes = Encoding.GetEncoding(932).GetBytes("   " + option);
                            tempList.AddRange(sendBytes);
                            tempList.AddRange(Linefeed);
                        }
                    }

                    tempList.AddRange(Right);
                    sendBytes = Encoding.GetEncoding(932).GetBytes(currentTicketNo.ToString("0000000000"));
                    tempList.AddRange(sendBytes);

                    tempList.AddRange(Emphasize_release);

                    tempList.AddRange(print_feed);

                    tempList.AddRange(Cut_partial);

                    ticketNumbers++;
                    subline++;
                }
                lineNo++;
            }

            // Initialize printer
            tempList.AddRange(reset);


            return tempList.ToArray();
        }

        Socket s = null;
        private void TcpPrint(string host, string port, byte[] data)
        {
            constants.SaveLogData("saleScreen_***", "Network Printing");
            printType = 1;
            try
            {
                if(s == null)
                {
                    s = new Socket(
                       AddressFamily.InterNetwork,
                       SocketType.Stream,
                       ProtocolType.Tcp);
                    s.Connect(host, int.Parse(port));
                }
                byte[] byData = new byte[] { 0x1D, 0x61, 0x01 };
                int rs = s.Send(byData);
                byte[] buffer = new byte[2];
                int rByte = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                string byteStr = Encoding.Default.GetString(buffer, 0, rByte);
                constants.SaveLogData("saleScreen_***", "Network Printing status" + buffer[0]);
                if (buffer[0] == 0)
                {
                    SendData(data);
                REPEATCOMMAND:
                    if (!TcpPrintStatus(1))
                    {
                        ShowPrintErrorMsgs(1);
                    }
                    if (printStatus == 16)
                    {
                        goto REPEATCOMMAND;
                    }

                    if (s != null)
                    {
                        s.Close();
                        s = null;
                    }
                }
                else
                {
                    TicketingMessageClose();
                    int errStatus = buffer[0];
                    if (buffer[0] == 32)
                    {
                        errStatus = 16;
                    }
                    if (s != null)
                    {
                        s.Close();
                        s = null;
                    }

                    dbClass.InsertLog(2, "プリンター異常", "キッチン" + constants.printErrTitle[errStatus]);
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, errStatus, 1, 1);
                }
            }
            catch (SocketException e)
            {
                if (s != null)
                    s.Close();
                s = null;
                constants.SaveLogData("saleScreen_***", e.ToString());
                dbClass.InsertLog(2, "プリンター異常", "キッチンプリンター通信エラー");
                TicketingMessageClose();
                messageDialog.ShowPrintErrorMessage("キッチン", 1, 0, 1, 1);
                return;
            }

        }

        private void SendData(byte[] data)
        {
            try
            {
                int r = s.Send(data);
            }
            catch (SocketException se)
            {
                constants.SaveLogData("saleScreen_***", se.ToString());
                dbClass.InsertLog(2, "プリンター異常", "キッチンプリンター通信エラー");
                messageDialog.ShowPrintErrorMessage("キッチン", 1, 0, 1, 1);
                return;
            }

        }

        private bool OrderDataSaving()
        {
            // Create a new database connection:
            bool ret = false;
            //string dbPath = Path.Combine(Directory.GetCurrentDirectory(), constants.dbName + ".db");
            string dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dbFolder += "\\STV01\\";
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            string dbPath = Path.Combine(dbFolder, constants.dbName + ".db");

            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; PRAGMA journal_mode=WAL; Version=3; New=True; Compress=True; Connection Timeout=10");
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            try
            {
                constants.SaveLogData("saleScreen_9", "saleScreen order data save");
                DateTime now = DateTime.Now;
                string week = now.ToString("ddd");
                string currentTime = now.ToString("HH:mm");

                key.SetValue("currentSerialNo", currentSerialNo);
                key.SetValue("currentTicketNo", currentTicketNo);

                Console.WriteLine("orderSaving......." + DateTime.Now.ToString("HH:mm:ss"));

                for (int k = 0; k < currentSelectedId; k++)
                {
                    SQLiteCommand sqlite_cmd;
                    string orderRun = "INSERT INTO " + constants.tbNames[3] + " (PrdID, PrdName, PrdPrice, PrdAmount, ticketNo, saleDate, categoryID, serialNo, prdRealID) VALUES (@prdID, @prdName, @prdPrice, @prdAmount, @ticketNo, @saleDate, @categoryID, @serialNo, @realPrdID)";
                    sqlite_cmd = sqlite_conn.CreateCommand();
                    sqlite_cmd.CommandTimeout = 0;
                    sqlite_cmd.CommandText = orderRun;
                    sqlite_cmd.Parameters.AddWithValue("@prdID", orderProductIDArray[k]);
                    sqlite_cmd.Parameters.AddWithValue("@prdName", orderProductNameArray[k]);
                    sqlite_cmd.Parameters.AddWithValue("@prdPrice", orderProductPriceArray[k]);
                    sqlite_cmd.Parameters.AddWithValue("@prdAmount", orderAmountArray[k]);
                    sqlite_cmd.Parameters.AddWithValue("@ticketNo", currentTicketNo);
                    sqlite_cmd.Parameters.AddWithValue("@saleDate", now);
                    sqlite_cmd.Parameters.AddWithValue("@categoryID", orderCategoryIDArray[k]);
                    sqlite_cmd.Parameters.AddWithValue("@serialNo", currentSerialNo);
                    sqlite_cmd.Parameters.AddWithValue("@realPrdID", orderRealProductIDArray[k]);
                    try
                    {
                        sqlite_cmd.ExecuteNonQuery();
                        sqlite_cmd.Dispose();
                        sqlite_cmd = null;
                    }
                    catch (Exception e)
                    {
                        constants.SaveLogData("saleScreen_9", e.ToString());
                        Console.WriteLine("executeError===>" + e.ToString());
                        sqlite_cmd.Dispose();
                        sqlite_cmd = null;
                    }


                    SQLiteCommand sqlite_cmd1;
                    SQLiteDataReader sqlite_datareader1;
                    sqlite_cmd1 = sqlite_conn.CreateCommand();
                    sqlite_cmd1.CommandTimeout = 0;

                    string queryCmd1 = "SELECT sum(" + constants.tbNames[3] + ".prdAmount) as prdRestAmount, " + constants.tbNames[2] + ".LimitedCnt FROM " + constants.tbNames[2] + " LEFT JOIN " + constants.tbNames[3] + " on " + constants.tbNames[3] + ".prdRealID = " + constants.tbNames[2] + ".ProductID WHERE " + constants.tbNames[2] + ".ProductID=@prdID and " + constants.tbNames[3] + ".limitFlag='0'";
                    sqlite_cmd1.CommandText = queryCmd1;
                    sqlite_cmd1.Parameters.AddWithValue("@prdID", orderRealProductIDArray[k]);

                    sqlite_datareader1 = sqlite_cmd1.ExecuteReader();
                    int restAmount = 0;
                    int limitedCnt = 0;
                    while (sqlite_datareader1.Read())
                    {
                        if (!sqlite_datareader1.IsDBNull(0))
                        {
                            limitedCnt = sqlite_datareader1.GetInt32(1);
                            if (sqlite_datareader1.GetInt32(1) != 0)
                                restAmount = sqlite_datareader1.GetInt32(1) - sqlite_datareader1.GetInt32(0);
                            else
                                restAmount = 0;
                        }
                        else
                        {
                            restAmount = sqlite_datareader1.GetInt32(1);
                        }
                    }
                    sqlite_cmd1.Dispose();
                    sqlite_cmd1 = null;
                    sqlite_datareader1.Close();
                    sqlite_datareader1 = null;

                    if (restAmount == 0 && limitedCnt != 0)
                    {
                        SQLiteCommand sqlite_cmd2;
                        sqlite_cmd2 = sqlite_conn.CreateCommand();
                        sqlite_cmd2.CommandTimeout = 0;

                        string queryCmd2 = "UPDATE " + constants.tbNames[2] + " SET SoldFlag=1 WHERE ProductID=@productID";
                        sqlite_cmd2.CommandText = queryCmd2;
                        sqlite_cmd2.Parameters.AddWithValue("@productID", orderRealProductIDArray[k]);
                        sqlite_cmd2.ExecuteNonQuery();
                        sqlite_cmd2.Dispose();
                        sqlite_cmd2 = null;
                    }
                }

                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
                lineNumbers = 0;
                ticketNumber = 0;
                ticketNumbers = 0;
                dataEmpty = false;
                ret = true;
                return ret;
            }
            catch (Exception e)
            {
                ret = false;
                constants.SaveLogData("saleScreen_9", e.ToString());
                Console.WriteLine("DataSavingErro==>" + e.ToString());
                //sqlite_conn.Close();
                sqlite_conn.Dispose();
                sqlite_conn = null;
                return ret;
            }
        }

        int printStatus = 0;
        int printDisconnect = 0;
        int printType = 0;

        public bool USBPrintStatus(int errTime = 0)
        {
            bool ret = true;
            printType = 0;
            try
            {
                int a = UsbOpen("HMK-060");

                if (a != 0)
                {
                    ret = false;
                    printDisconnect = -1;
                    constants.SaveLogData("saleScreen_****_0", "printer_disconnect===" + a);
                    dbClass.InsertLog(2, "プリンター異常", "チケットプリンター通信エラー");
                }
                else
                {
                    printStatus = NewRealRead();
                    if ((printStatus >= 1 && printStatus <= 15) || printStatus == 32)
                    {
                        ret = false;
                        int errStatus = printStatus;
                        if(printStatus == 32)
                        {
                            errStatus = 16;
                        }
                        dbClass.InsertLog(2, "プリンター異常", "チケット" + constants.printErrTitle[errStatus]);
                    }

                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_****_4087", ex.ToString());
                dbClass.InsertLog(2, "プリンター異常", "チケット" + constants.printErrTitle[17]);
                printStatus = 17;
                ret = false;
                return ret;
            }
            return ret;
        }

        private void ShowPrintErrorMsgs(int errTime = 0 )
        {
            if (bBackShow) return;

            if (printDisconnect == -1 )
            {
                constants.SaveLogData("saleScreen_****_1", "printer_disconnect" + printType);
                if(printType == 0) 
                    messageDialog.ShowPrintErrorMessage("チケット", 0, 0, errTime, 1);
                else
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, 0, errTime, 1);
            }
            else
            {
                constants.SaveLogData("saleScreen_****_2", "printError===" + printStatus);
                if ((printStatus >= 1 && printStatus <= 15) || printStatus == 32 || printStatus == 17)
                {
                    int errStatus = printStatus;

                    if (printStatus == 32)
                        errStatus = 16;

                    if (printType == 0)
                        messageDialog.ShowPrintErrorMessage("チケット", 0, errStatus, errTime, 1);
                    else
                        messageDialog.ShowPrintErrorMessage("キッチン", 1, errStatus, errTime, 1);

                }
            }

            if( sumThread == null && errTime == 0 )
            {
                printDisconnect = 0;
                printStatus = 0;
                flag = true;
                sumThread = new Thread(SumThreadRunings);
                sumThread.SetApartmentState(ApartmentState.STA);
                sumThread.Start();
            }
        }

        public bool TcpPrintStatus(int errTime = 0)
        {
            bool ret = true;
            printType = 1;
            try
            {
                if(s == null)
                {
                    s = new Socket(
                       AddressFamily.InterNetwork,
                       SocketType.Stream,
                       ProtocolType.Tcp);
                    s.Connect(ipAddress, int.Parse(port));
                }
                byte[] byData = new byte[] { 0x1D, 0x61, 0x01 };
                int rs = s.Send(byData);
                byte[] buffer = new byte[2];
                int rByte = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                string byteStr = Encoding.Default.GetString(buffer, 0, rByte);
                printStatus = buffer[0];
                if ((printStatus > 0 && printStatus < 16) || printStatus == 32)
                {
                    int errStatus = printStatus;

                    if (printStatus == 32)
                        errStatus = 16;

                    dbClass.InsertLog(2, "プリンター異常", "キッチン" + constants.printErrTitle[errStatus]);

                    if(s != null)
                        s.Close();
                    s = null;
                    ret = false;
                    return ret;
                }
                if (s != null)
                    s.Close();
                s = null;
                return ret;
            }
            catch (Exception)
            {
                if (s != null)
                    s.Close();
                s = null;
                ret = false;
                dbClass.InsertLog(2, "プリンター異常", "キッチンプリンター通信エラー");
                printDisconnect = -1;
                return ret;
            }
        }

        /**
         * Print Receipt and Ordering finish
         **/
        private void ReceiptRun()
        {
            Thread.Sleep(1000);
            //sqlite_conn = CreateConnection(constants.dbName);
            //string dbPath = Path.Combine(Directory.GetCurrentDirectory(), constants.dbName + ".db");
            string dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dbFolder += "\\STV01\\";
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            string dbPath = Path.Combine(dbFolder, constants.dbName + ".db");

            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; PRAGMA journal_mode=WAL; Version=3; New=True; Compress=True; ConnectionTimeout=10");
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            try
            {
                constants.SaveLogData("saleScreen_10", "saleScreen receipt save");
                SQLiteCommand sqlite_cmd;
                DateTime now = DateTime.Now;
                string orderRun = "INSERT INTO ReceiptTB (PurchasePoint, TotalPrice, ReceiptDate, ticketNo) VALUES (@purchasePoint, @totalPrice, @receiptDate, @ticketNo)";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandTimeout = 5;
                sqlite_cmd.CommandText = orderRun;
                sqlite_cmd.Parameters.AddWithValue("@purchasePoint", orderTotalTicketForReceipt);
                sqlite_cmd.Parameters.AddWithValue("@totalPrice", orderTotalPriceForReceipt);
                sqlite_cmd.Parameters.AddWithValue("@receiptDate", now);
                sqlite_cmd.Parameters.AddWithValue("@ticketNo", currentTicketNo);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;

            }
            catch (Exception ex)
            {
                constants.SaveLogData("saleScreen_10", ex.ToString());
                //Console.WriteLine("receiptRun_error==>" + ex.ToString());
                sqlite_conn.Dispose();
                sqlite_conn = null;
                return;
            }
        }

        private void ReceiptButtonChange()
        {
            if (ReceiptValid == "true")
            {

                Thread.Sleep(int.Parse(TicketTime) * 1000);
                if (receiptButtonGlobal.InvokeRequired)
                {
                    receiptButtonGlobal.Invoke(new MethodInvoker(delegate
                    {
                        using(Bitmap img = new Bitmap(constants.receiptDisableButton))
                        {
                            receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                        }
                    }));
                }
                else
                {
                    using (Bitmap img = new Bitmap(constants.receiptDisableButton))
                    {
                        receiptButtonGlobal.BackgroundImage = new Bitmap(img);
                    }
                }
            }
        }

        int printerFlag = 0;
        private void Receipt_PrintPage(object sender, PrintPageEventArgs e)
        {
            DateTime now = DateTime.Now;
            while (printerFlag < 1)
            {
                printerFlag++;
                e.HasMorePages = false;

            }

        }

        /** ================ Ticketing End =========================== **/

        /**
         * return to back page(main page)
         **/
        public void BackShow()
        {
            if (mainFormGlobal != null)
                mainFormGlobal.HideLoading();
            if (HDCheck && comModule == null)
                return;
            if (HDCheck && messageDialog.DialogFormGlobal != null)
                return;

            if (sumThread != null)
            {
                sumThread.Abort();
                sumThread = null;
            }


            if (mainPanelGlobal2 != null)
            {
                if (mainPanelGlobal2.InvokeRequired)
                {
                    mainPanelGlobal2.Invoke(new MethodInvoker(delegate
                    {
                        constants.SaveLogData("saleScreen_*****1", "image initializing");
                        if (pb_Image != null)
                        {
                            foreach (PictureBox pb_Img in pb_Image)
                            {
                                if (pb_Img != null)
                                {
                                    pb_Img.Dispose();
                                }
                            }
                            pb_Image = null;
                        }
                        mainPanelGlobal2.Controls.Clear();
                        mainPanelGlobal2.Hide();
                    }));
                }
                else
                {
                    constants.SaveLogData("saleScreen_*****2", "image initializing");
                    if(pb_Image != null)
                    {
                        foreach (PictureBox pb_Img in pb_Image)
                        {
                            if (pb_Img != null)
                            {
                                pb_Img.Dispose();
                            }
                        }
                        pb_Image = null;
                    }
                    mainPanelGlobal2.Controls.Clear();
                    mainPanelGlobal2.Hide();

                }
            }
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            dbClass.InsertLog(5, "販売終了", "販売時間： " + currentTime); //currentSaleTime

            if (mainFormGlobal.InvokeRequired)
            {
                mainFormGlobal.Invoke(new MethodInvoker(delegate
                {
                    pMm.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
                    mainPanelGlobal.Show();
                    mainFormGlobal.topPanelGlobal.Show();
                    mainFormGlobal.bottomPanelGlobal.Show();
                }));
            }
            else
            {
                pMm.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
                mainPanelGlobal.Show();
                mainFormGlobal.topPanelGlobal.Show();
                mainFormGlobal.bottomPanelGlobal.Show();
            }
            /** 2020-09-01 stopped */
            
            try
            {
                GC.Collect();

                GC.WaitForPendingFinalizers();

                constants.SaveLogData("saleScreen_end", "saleScreen End");
                if(HDCheck)
                {
                    if (comModule != null)
                    {
                        if (comModule.getDepositTh != null)
                        {
                            comModule.PermitAndProhCommand(0x00, 0x00);
                            comModule.StopDepositThread();
                            currentbPermit = false;
                        }
                        comModule.CloseComport();
                        comModule = null;

                        if (pMm != null && comModule == null)
                        {
                            pMm.ReacquireErrorStatus();
                        }
                        if (pMm != null)
                        {
                            pMm.flag = true;
                            pMm.HDChecking = true;
                            pMm.ReRunThread();
                        }
                    }
                }
                else
                {
                    if (pMm != null)
                        pMm.HDChecking = false;
                }
            }
            catch(Exception ex)
            {
                constants.SaveLogData("saleScreen_end", ex.ToString());
                Console.WriteLine(ex.ToString());
                BackShow();
            }

        }

        SoundPlayer player = null;
        private void PlaySound(string sfileName)
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
            string path = sfileName;
            if (!File.Exists(path))
                return;
            player = new SoundPlayer(@path);
            //player.SoundLocation = @path;
            player.Play();
        }

        /**
         * DB Conneting Config
         **/
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


            sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; PRAGMA journal_mode=WAL; Version=3; New=True; Compress=True; ConnectionTimeout=0");

            return sqlite_conn;
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (TempLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                TempLabel.Text = text;
            }
        }

        static void InitIntArray(int[,] arr)
        {
            for (int k = 0; k < arr.GetLength(0); k++)
            {
                for (int i = 0; i < arr.GetLength(1); i++)
                {
                    arr[k, i] = -1;
                }
            }
        }

        static int SumArray(string[] arr)
        {
            int sumResult = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                {
                    sumResult += int.Parse(arr[i]);
                }
            }
            return sumResult;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SaleScreen
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SaleScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Load += new System.EventHandler(this.SaleScreen_Load);
            this.ResumeLayout(false);

        }

        // Error status analysis methods...
        private int[] Errorstatus = null;
        private int[] BV1ErrStatus = null;
        private int[] BV2ErrStatus = null;
        private int[] BDErrStatus = null;
        private int[] CMErrStatus = null;

        private string BV1Error = "";
        private string BV2Error = "";
        private string BDError = "";
        private string CMError = "";

        private string title = "";
        private int content_index = 0;
        private int erroridx = 0;
        private int nErrCnt = 0;

        private int depoAmt = 0;
        private int didwithdraw = 0;
        private int willwithdraw = 0;
        public bool bBackShow = false;
        private bool bNoError = false;

        private bool bTicketing = false;
        private bool bDispensorErr = false;

        private bool InitialComModuleStatusAnal()
        {
            bool ret = true;
            GetBalanceStatus();
            if (bBackShow)
                return false;
            ErrorStatusOfImpoossible();
            return ret;
        }

        int bGetbalance = 0;
        public void GetBalanceStatus()
        {
            if (comModule == null || bTicketing)
                return;
            byte[] res = null;
            int[] ppData = new int[5] { -1, -1, -1, -1, -1 };

            while ( bGetbalance < 2)
            {
                res = comModule.GetCoinMeshBalanceData(0x03);
                Thread.Sleep(200);
                ppData = comModule.paperBLdata;
                if (ppData[0] == 1)
                    break;
                bGetbalance++;
            }
            bGetbalance = 0;

            if (res == null)
                return;
            

            if (res[3] == 0x00 && (ppData[1] < 10 || ppData[3] < 10 || ppData[2] < 10 || ppData[4] < 6 || ppData[0] == 1))
            {
                string title = "釣銭切れ";
                int[] status = new int[5];
                formType = "error_detail";
                status[0] = ppData[0] == 1 ? 0 : 1;
                status[1] = ppData[1] < 10 ? 0 : 1;
                status[2] = ppData[2] < 10 ? 0 : 1;
                status[3] = ppData[3] < 10 ? 0 : 1;
                status[4] = ppData[4] < 6 ? 0 : 1;
                TicketingMessageClose();

                if ( ppData[0] == 1)
                {
                    int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 1, 0 };
                    messageDialog.ShowErrorMessage(title, 0, content_index, tempErrorStatus);
                }
                else if (ppData[1] < 10 || ppData[3] < 10 || ppData[2] < 10 || ppData[4] < 6)
                {
                    int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 0, 1 };
                    messageDialog.ShowErrorMessage(title, 0, content_index, tempErrorStatus);
                }

                if (bBackShow) return;

                if ( !bBackShow)
                {
                    ResetComModue();
                }

            }
            else if (res[3] == 0xEE)
            {
                ErrorStatusOfImpoossible();
            }
        }

        private bool bFirst = false;
        public void ErrorStatusOfImpoossible()
        {
            if (comModule.Errorstatus != null)
            {
                if( !bFirst)
                {
                    bFirst = true;
                    comModule.GetDetailWithdrawStatus();
                }
                if (bBackShow) return;
                ErrorStatusAnal();
                InitErrStatusVariables();
            }
            else if(comModule != null)
            {
                if (!bFirst)
                {
                    bFirst = true;
                    if( !comModule.GetDetailWithdrawStatus())
                    {
                        ResetComModue();
                        return;
                    }
                }
                if (bBackShow) return;
                InitErrStatusVariables();
                comModule.GetErrorStatus(0x07);
                
                ErrorStatusAnal();
            }
        }

        private void InitErrStatusVariables()
        {
            Errorstatus = null;
            BV1ErrStatus = null;
            BV2ErrStatus = null;
            BDErrStatus = null;
            CMErrStatus = null;

            bFirst = false;

            title = "";
            content_index = 0;

            BV1Error = "";
            BV2Error = "";
            BDError = "";
            CMError = "";

            erroridx = 0;
            nErrCnt = 0;
            formType = "";
            bBackShow = false;
            bNoError = false;
        }

        private void SaleScreen_Load(object sender, EventArgs e)
        {
            int ww = this.Width;
            int hh = this.Height;
        }

        private void ErrorStatusAnal()
        {
            if (comModule.Errorstatus == null) return;

            Errorstatus = comModule.Errorstatus;

            if ( Errorstatus[0] == -2 )
            {
                title = "機器エラー";

                content_index = 17;
                int[] depoStatus = comModule.depositArr;
                int[] status = comModule.withdrawAmt;
                int[] withdraw = comModule.withdrawArr;
                depoAmt = comModule.depositAmount;
                if( status != null)
                {
                    willwithdraw = 10 * status[0] + 100 * status[1] + 1000 * status[2];
                    didwithdraw = comModule.withdrawAmount;
                }

                int errorType = comModule.errorType;

                if ( errorType == 1)
                {
                    TicketingMessageClose();
                    messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, depoStatus, errorType);
                }
                else if (errorType == 2)
                {
                    TicketingMessageClose();
                    messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, withdraw, errorType, status);
                }
                else if (errorType == 3)
                {
                    TicketingMessageClose();
                    messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, withdraw, errorType, depoStatus);
                }
                if (bBackShow)
                    return;
                if (errorType != 0 && !bBackShow )
                    ResetComModue();
                return;
            }
            else
            {
                BV1ErrStatus = comModule.BV1ErrStatus;
                BV1Error = comModule.BV1Error;
                BV2ErrStatus = comModule.BV2ErrStatus;
                BV2Error = comModule.BV2Error;
                BDErrStatus = comModule.BDErrStatus;
                BDError = comModule.BDError;
                CMErrStatus = comModule.CMErrStatus;
                CMError = comModule.CMError;

                nErrCnt = GetErrorCnt();

                if ((nErrCnt > 0) || ExistError(Errorstatus))
                {
                    title = "紙幣排出機と通信が出来ません";
                    content_index = 17;
                    int[] depoStatus = comModule.depositArr;
                    int[] status = comModule.withdrawAmt;
                    int[] withdraw = comModule.withdrawArr;
                    depoAmt = comModule.depositAmount;
                    if (status != null)
                        willwithdraw = 10 * status[0] + 100 * status[1] + 1000 * status[2];
                    didwithdraw = comModule.withdrawAmount;

                    int errorType = comModule.errorType;
                    string errorCode = comModule.errorCode;
                    string[] arr_errorCode = errorCode.Split('-');
                    string errC1 = "", errC2 = "";

                    if (Errorstatus[2] == 1)
                    {
                        errC1 = arr_errorCode[5];
                        errC2 = arr_errorCode[6];
                    }
                    else if (Errorstatus[3] == 1)
                    {
                        errC1 = arr_errorCode[7];
                        errC2 = arr_errorCode[8];
                    }
                    else if (Errorstatus[4] == 1)
                    {
                        errC1 = arr_errorCode[9];
                        errC2 = arr_errorCode[10];
                    }
                    else if (Errorstatus[5] == 1)
                    {
                        errC1 = arr_errorCode[11];
                        errC2 = arr_errorCode[12];
                    }

                    if (errorType == 1)
                    {
                        TicketingMessageClose();
                        messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, depoStatus, errorType, null, errC1, errC2);
                    }
                    else if (errorType == 2)
                    {
                        if (bTicketing)
                            bDispensorErr = true;
                        TicketingMessageClose();
                        messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, withdraw, errorType, status, errC1, errC2);
                    }
                    else if (errorType == 3)
                    {
                        TicketingMessageClose();
                        messageDialog.ShowErrorMessage(title, 0, content_index, Errorstatus, withdraw, errorType, depoStatus, errC1, errC2);
                    }
                    
                    if (errorType != 0 && !bBackShow)
                    {
                        ResetComModue();
                    }
                    if (bBackShow) return;
                }
                else
                {
                    bNoError = true;
                }
            }
        }

        private bool ExistError(int[] arr)
        {
            bool ret = false;
            for(int i = 1; i< arr.Length; i++)
                if(arr[i] == 1)
                {
                    ret = true;
                    break;
                }
            return ret;
        }

        private int GetErrorCnt()
        {
            int ncnt = 0, i;
            if (Errorstatus[2] == 1)
                for (i = 0; i < BV1ErrStatus.Length; i++)
                    if (BV1ErrStatus[i] != 0)
                        ncnt++;
            if (Errorstatus[3] == 1)
                for (i = 0; i < BV2ErrStatus.Length; i++)
                    if (BV2ErrStatus[i] != 0)
                        ncnt++;
            if (Errorstatus[4] == 1)
                for (i = 0; i < BDErrStatus.Length; i++)
                    if (BDErrStatus[i] != 0)
                        ncnt++;

            return ncnt;
        }

        public void ProcessCommoduleMsgs(int[] ppData, int[] status)
        {
            TicketingMessageClose();
            if (ppData[0] == 1)
            {
                int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 1, 0 };
                messageDialog.ShowErrorMessage(title, 0, 16, tempErrorStatus);
            }
            else
            {
                int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 0, 1 };
                messageDialog.ShowErrorMessage(title, 0, 16, tempErrorStatus);
            }
            if (!messageDialog.bExit)
            {
                ResetComModue();
                bBackShow = false;
            }
            else
            {
                bBackShow = true;
                BackShow();
            }
        }

        public void ResetComModue()
        {
            if (bBackShow) return;
            if (mainFormGlobal != null)
                mainFormGlobal.ShowLoading();
            Thread.Sleep(40);
            try
            {
                Binddata();
            }
            catch (Exception e)
            {
                constants.SaveLogData("saleScreen_resetCommodule", e.ToString());
                Console.WriteLine(e.ToString());
                Thread.Sleep(40);
                if (mainFormGlobal != null)
                    mainFormGlobal.HideLoading();
                bBackShow = true;
                BackShow();
                return;
            }
            Thread.Sleep(40);
            if (mainFormGlobal != null)
                mainFormGlobal.HideLoading();
        }

        private void Binddata()
        {
            int nRepeat = 0;
            if (messageDialog.bExit)
                return;
            if (comModule == null)
                return;
            if (comModule != null)
                comModule.StopDepositThread();
         ResetModuleStart:
            if( nRepeat > 4)
            {
                bBackShow = true;
                BackShow();
                return;
            }
            if (comModule.ResetModule())
            {
                comModule = null;
                Thread.Sleep(3000);

                comModule = new ComModule();

                if (comModule.InitMicroCom(mainFormGlobal))
                {
                    if(!bTicketing)
                        GetBalanceStatus();
                    if (bBackShow) return;
                    ErrorStatusOfImpoossible();
                    if (bBackShow) return;

                    bBackShow = false;

                    if (comModule != null && comModule.getDepositTh == null )
                    {
                        comModule.Initialize(this);
                        if (depoAmt != 0 && willwithdraw != 0 && !bDispensorErr ) 
                        {
                            comModule.TicketRun(willwithdraw - didwithdraw);
                        }
                        if (depoAmt != 0 && bOrderCancel && !bDispensorErr)
                        {
                            comModule.OrderCancel(depoAmt);
                        }
                        if (comModule.getDepositTh == null)
                            comModule.OrderChange("0");

                        depoAmt = 0;
                        willwithdraw = 0;
                        didwithdraw = 0;
                        bOrderCancel = false;
                    }
                }
                else
                {
                    ErrorStatusOfImpoossible();
                }
            }
            else
            {
                nRepeat++;
                Thread.Sleep(2000);
                goto ResetModuleStart;
            }
            nRepeat = 0;
        }

    }

}
