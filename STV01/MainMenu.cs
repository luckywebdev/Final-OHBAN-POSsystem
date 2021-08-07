using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace STV01
{
    public partial class MainMenu : Form
    {
        public Form1 mainFomeGlobal = null;
        public Panel FormPanel = null;
        public Panel FormPanel_2 = null;
        Constant constants = new Constant();
        ComModule comModule = new ComModule();

        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        PasswordInput passwordInput = new PasswordInput();
        CustomButton customButton = new CustomButton();
        CreatePanel createPanel = new CreatePanel();
        MessageDialog messageDialog = new MessageDialog();
        SQLiteConnection sqlite_conn;
        DBClass dbClass = new DBClass();
        SoundPlayer player = null;
        public Thread sumThreadm = null;
        public Thread checkThread = null;
        public Thread pgThread = null;
        bool tb_check = true;
        public bool flag = true;
        public string formType = "";

        RegistryKey key = null;

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
        string ipAddress1 = "192.168.1.250";
        string port1 = "9100";

        public void CreateMainMenuScreen(Form1 forms, Panel panels)
        {
            mainFomeGlobal = forms;
            FormPanel = panels;
            FormPanel.BackColor = Color.FromArgb(255, 249, 246, 224);
            FormPanel_2 = forms.mainPanelGlobal_2;

            messageDialog.InitMainMenu(this);
            passwordInput.InitMainMenu(this);


            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;

            foreach (string tbName in constants.tbNames)
            {
                try
                {
                    string query = "SELECT count(*) FROM " + tbName;
                    sqlite_cmd = sqlite_conn.CreateCommand();
                    sqlite_cmd.CommandText = query;
                    int rowCount = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                }
                catch (Exception ex)
                {
                    constants.SaveLogData("mainMenu_0", "" + ex.ToString());
                    tb_check = false;
                    break;
                }
            }
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            int checkingState = 0;
            if (key != null)
            {

                if (key.GetValue("checkingState") != null)
                {
                    checkingState = Convert.ToInt32(key.GetValue("checkingState"));
                }
                else
                {
                    key.SetValue("checkingState", 0);
                    checkingState = 1;
                }
            }
            this.MainMenuContent();
        }

        private int[] StatusErrorAnal(byte b)
        {
            int[] ret = new int[6];
            ret[0] = -1;
            ret[1] = -1;
            ret[2] = -1;
            ret[3] = -1;
            ret[4] = -1;
            ret[5] = -1;
            string sb = Convert.ToString(b);
            string result = Convert.ToString(Convert.ToInt32(sb, 10), 2).PadLeft(8, '0');
            if (result.Substring(0, 1) == "1")
                ret[0] = 1;
            if (result.Substring(3, 1) == "1")
                ret[1] = 1;
            if (result.Substring(4, 1) == "1")
                ret[2] = 1;
            if (result.Substring(5, 1) == "1")
                ret[3] = 1;
            if (result.Substring(6, 1) == "1")
                ret[4] = 1;
            if (result.Substring(7, 1) == "1")
                ret[5] = 1;
            return ret;
        }

        int nrepeat = 0;
        private void MainMenuThread()
        {
            Thread.Sleep(200);
            if (bInit)
            {
            REPEAT:
                if (comModule == null)
                    comModule = new ComModule();
                if (comModule.InitMicroCom(mainFomeGlobal, this))
                {
                    nrepeat = 0;
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            ResetModule();
                        });
                    }
                    else
                    {
                        ResetModule();
                    }
                }
                else
                {
                    if (comModule.noComport && nrepeat < 5)
                    {
                        nrepeat++;
                        if( comModule != null )
                            comModule = null;
                        Thread.Sleep(200);
                        goto REPEAT;
                    }

                    if( comModule.noComport && nrepeat > 4)
                    {
                        int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 0, 0 };
                        int[] tempIndex = new int[] { 18 };
                        messageDialog.ShowErrorDetailMessageOut(title, tempIndex, tempErrorStatus, "", "");
                        return;
                    }
                    nrepeat = 0;
                    if (!comModule.noComport)
                        ErrorStatusOfPossible();
                }
                bInit = false;
                if (comModule != null)
                {
                    nrepeat = 0;
                    CloseComport();
                    comModule = null;
                }
            }

            while(!UsbPrint())
            {
                ShowPrintErrorMsgsStatic();
            }
            if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
            {
                while (!TcpPrint())
                {
                    ShowPrintErrorMsgsStatic();
                }
            }

        }

        CreateLabel createLabel = new CreateLabel();
        public bool HDChecking = false;

        private void MainMenuContent()
        {
            key.SetValue("checkingState", 1);
            Label versionLabel = new Label();
            versionLabel.Location = new Point(30, height - 100);
            versionLabel.Size = new Size(width / 3, 30);
            versionLabel.TextAlign = ContentAlignment.MiddleLeft;
            versionLabel.ForeColor = Color.FromArgb(255, 22, 22, 22);
            versionLabel.Font = new Font("Serif", 18, FontStyle.Bold);
            versionLabel.Text = constants.version;

            mainFomeGlobal.Controls.Add(versionLabel);

            Panel titlePanel = createPanel.CreateSubPanel(FormPanel, 0, 0, FormPanel.Width, FormPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));


            /**  Main Page Screen Title */

            Label MainTitle = new Label();
            MainTitle.Location = new Point(0, 0);
            MainTitle.Width = titlePanel.Width;
            MainTitle.Height = titlePanel.Height;
            MainTitle.TextAlign = ContentAlignment.MiddleCenter;
            MainTitle.Font = new Font("Seri", 36, FontStyle.Bold);
            MainTitle.ForeColor = Color.FromArgb(255, 22, 22, 22);
            MainTitle.Text = constants.main_Menu_Title;
            titlePanel.Controls.Add(MainTitle);


            /** Menu Button Create  */
            int k = 0;
            foreach (string x in constants.main_Menu)
            {
                RoundedButton btn = new RoundedButton();
                btn.Name = constants.main_Menu_Name[k];
                int xCordinator = (FormPanel.Width / 8) + k * (FormPanel.Width * 2 / 8) + k * (FormPanel.Width * 1 / 4);

                btn.Location = new Point(xCordinator, FormPanel.Height / 3);
                btn.Width = FormPanel.Width * 2 / 8;
                btn.Height = FormPanel.Height * 1 / 3;
                switch (k)
                {
                    //case 1:
                    //    btn.BackColor = Color.FromArgb(255, 225, 100, 74);
                    //    btn.ColorTop = Color.FromArgb(255, 227, 111, 87);
                    //    btn.ColorBottom = Color.FromArgb(255, 225, 100, 74);

                    //    break;
                    case 1:
                        btn.BackColor = Color.FromArgb(255, 0, 123, 191);
                        btn.ColorTop = Color.FromArgb(255, 7, 131, 200);
                        btn.ColorBottom = Color.FromArgb(255, 0, 123, 191);
                        break;
                    default:
                        btn.BackColor = Color.FromArgb(255, 94, 162, 83);
                        btn.ColorTop = Color.FromArgb(255, 112, 169, 103);
                        btn.ColorBottom = Color.FromArgb(255, 94, 162, 83);
                        break;
                }
                btn.FlatStyle = FlatStyle.Flat;
                btn.radiusValue = 50;
                btn.ForeColors = Color.White;
                btn.text = x;
                btn.diffValue = 3;
                btn.FlatAppearance.BorderColor = Color.White;
                btn.FlatAppearance.BorderSize = 1;

                btn.Font = new Font("Seri", 28, FontStyle.Bold);

                btn.Click += new EventHandler(this.MainMenuBtn);
                FormPanel.Controls.Add(btn);

                k++;
            }

            string powerImage = constants.shutdownButton;

            Button backButton = customButton.CreateButtonWithImage(powerImage, "powerButton", "", FormPanel.Width - 250, FormPanel.Height - 120, 160, 70, 0, 1, 22, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            backButton.BackgroundImageLayout = ImageLayout.Stretch;
            backButton.Padding = new Padding(0);
            FormPanel.Controls.Add(backButton);
            backButton.Click += new EventHandler(ShutdownSystem);

            this.ClosingProcess();

            /** Mute temrary for now(sale time checking for auto closing)-2020-09-01 stopped */
            sumThreadm = new Thread(SumThreadRuning);
            sumThreadm.SetApartmentState(ApartmentState.STA);
            sumThreadm.Start();

            if (checkThread != null)
            {
                checkThread.Abort();
                checkThread = null;
            }

        }

        private void ShutdownSystem(object sender, EventArgs e)
        {
            if(messageDialog != null)
            {
                messageDialog.PowerApplication();
                messageDialog.Shutdown_windows();
            }
        }

        public void ReRunThread()
        {
            /** Mute temrary for now(sale time checking for auto closing)-2020-09-01 stopped */
            constants.SaveLogData("mainMenu_1-1", "rerun thread" + tb_check + "&&" + flag);

            if (!UsbPrint())
            {
                flag = false;
                sumThreadm = null;
            }
            if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
            {
                if (!TcpPrint())
                {
                    flag = false;
                    sumThreadm = null;
                }
            }
            if (!flag)
            {
                ShowPrintErrorMsgs();
            }

            sumThreadm = new Thread(SumThreadRuning);
            sumThreadm.SetApartmentState(ApartmentState.STA);
            sumThreadm.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ;
            }
            if (comModule != null)
            {
                comModule.CloseComport();
                comModule = null;
            }
            base.Dispose(disposing);
        }

        bool bInit = true;

        int printDisconnect = 0;
        int errStatus = 0;
        int printType = 0;
        public bool UsbPrint()
        {
            printType = 0;
            bool ret = true;
            try
            {
                int a = UsbOpen("HMK-060");
                //Console.WriteLine("mainmenuPrintStatus===start====>");
                if (a != 0)
                {
                    ret = false;
                    printDisconnect = -1;
                }
                else
                {
                    int status = NewRealRead();
                    //Console.WriteLine("mainmenuPrintStatus====checking====>" + status);
                    if ((status >= 1 && status <= 15) || status == 32)
                    {
                        ret = false;
                        errStatus = status;
                        if (status == 32)
                        {
                            errStatus = 16;
                        }
                    }
                }
                return ret;
            }
            catch (Exception e)
            {
                //Console.WriteLine("print_dll_error" + e.ToString());
                constants.SaveLogData("mainMenu_1", e.ToString());
                ret = false;
                errStatus = 17;
                return ret;
            }
        }

        Socket s = null;

        public bool TcpPrint()
        {
            bool ret = true;
            printType = 1;
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("ipAddress1") != null)
                {
                    ipAddress1 = Convert.ToString(key.GetValue("ipAddress1"));
                }
                if (key.GetValue("port") != null)
                {
                    port1 = Convert.ToString(key.GetValue("port1"));
                }
            }

            try
            {
                s = new Socket(
                   AddressFamily.InterNetwork,
                   SocketType.Stream,
                   ProtocolType.Tcp);
                s.Connect(ipAddress1, int.Parse(port1));
                byte[] byData = new byte[] { 0x1D, 0x61, 0x01 };
                int rs = s.Send(byData);
                byte[] buffer = new byte[2];
                int rByte = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                string byteStr = Encoding.Default.GetString(buffer, 0, rByte);
                if (buffer[0] != 0)
                {
                    errStatus = buffer[0];
                    if (buffer[0] == 32)
                    {
                        errStatus = 16;
                    }
                    s.Close();
                    s = null;
                    ret = false;
                    dbClass.InsertLog(2, "プリンター異常", "キッチン" + constants.printErrTitle[errStatus]);

                    return ret;
                }
                s.Close();
                s = null;
                return ret;
            }
            catch (Exception)
            {

                printDisconnect = -1;
                ret = false;
                dbClass.InsertLog(2, "プリンター異常", "キッチンプリンター通信エラー");
                return ret;
            }
        }

        public void CloseComport()
        {
            if( comModule != null)
            {
                comModule.CloseComport();
                comModule = null;
                bInit = true;
            }
        }

        public void ReacquireErrorStatus()
        {
            //if (bInit)
            {
                if (comModule == null)
                    comModule = new ComModule();
                bInit = false;
                ComModuleInit();
            }
        }

        public void ResetModule()
        {
            try
            {
                if (comModule == null)
                    comModule = new ComModule();

                if (comModule.ResetModule())
                {
                    comModule = null;
                    Thread.Sleep(4000);
                    if (comModule == null)
                        comModule = new ComModule();

                    if (comModule.InitMicroCom(mainFomeGlobal))
                    {
                        ErrorStatusOfPossible();
                        GetBalanceStatus();
                    }
                    else
                        ErrorStatusOfPossible();
                }
                else
                {
                    comModule = null;
                    Thread.Sleep(4000);
                    if (comModule == null)
                        comModule = new ComModule();

                    ErrorStatusOfPossible();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("mainMenu_2", ex.ToString());
                if (comModule != null)
                    comModule = null;
                return;
            }
        }

        private void ComModuleInit()
        {
            if (comModule.InitMicroCom(mainFomeGlobal))
            {
                // getting status data
                ErrorStatusOfPossible();
                GetBalanceStatus();
            }
            else
                ErrorStatusOfPossible();

            if( comModule != null)
            {
                CloseComport();
                comModule = null;
            }
        }

        private void GetBalanceStatus()
        {
            byte[] res = comModule.GetCoinMeshBalanceData(0x03);
            Thread.Sleep(100);
            if( res != null)
            {
                if (res[3] == 0x00)
                {
                    formType = "error_detail";
                    int[] ppData = comModule.paperBLdata;
                    int[] status = new int[5];
                    status[0] = ppData[0] == 1 ? 0 : 1;
                    status[1] = ppData[1] < 10 ? 0 : 1;
                    status[2] = ppData[2] < 10 ? 0 : 1;
                    status[3] = ppData[3] < 10 ? 0 : 1;
                    status[4] = ppData[4] < 6 ? 0 : 1;
                    if( ppData[0] == 1)
                    {
                        int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 1, 0 };
                        int[] tempIndex = new int[] { 0 };
                        messageDialog.ShowErrorDetailMessageOut(title, tempIndex, tempErrorStatus, "", "");
                    }
                    else if( ppData[2] < 10 || ppData[4] < 6 || ppData[3] < 10 || ppData[1] < 10)
                    {
                        int[] tempErrorStatus = new int[] { 0, 0, 0, 0, 0, 1 };
                        int[] tempIndex = new int[] { 0 };

                        messageDialog.ShowErrorDetailMessageOut(title, tempIndex, tempErrorStatus, "", "");
                    }
                       
                    formType = "";
                }
                else if (res[3] == 0xEE)
                {
                    ErrorStatusOfPossible();
                }
            }
            else
                ErrorStatusOfPossible();
        }

        private void ErrorStatusOfPossible()
        {
            InitErrStatusVariables();
            comModule.GetErrorStatus(0x07);
            Thread.Sleep(100);
            formType = "error_detail";
            ErrorStatusAnal();
            formType = "";
        }

        private void InitErrStatusVariables()
        {
            Errorstatus = null;
            BV1ErrStatus = null;
            BV2ErrStatus = null;
            BDErrStatus = null;
            CMErrStatus = null;

            title = null;
            content_index = null;

            BV1Error = "";
            BV2Error = "";
            BDError = "";
            CMError = "";

            erroridx = 0;
            nErrCnt = 0;
        }

        private int[] Errorstatus = null;
        private int[] BV1ErrStatus = null;
        private int[] BV2ErrStatus = null;
        private int[] BDErrStatus = null;
        private int[] CMErrStatus = null;

        private string BV1Error = "";
        private string BV2Error = "";
        private string BDError = "";
        private string CMError = "";

        private string[] title = null;
        private int[] content_index = null;
        private int erroridx = 0;
        private int nErrCnt = 0;

        private void ErrorStatusAnal()
        {
            Errorstatus = comModule.Errorstatus;
            if(Errorstatus[0] == -2 )
            {
                string[] title = new string[1] { "機器エラー" };
                int[] content = new int[] { 11 };

                messageDialog.ShowErrorDetailMessageOut(title, content);
                return;
            }
            BV1ErrStatus = comModule.BV1ErrStatus;
            BV1Error = comModule.BV1Error;
            BV2ErrStatus = comModule.BV2ErrStatus;
            BV2Error = comModule.BV2Error;
            BDErrStatus = comModule.BDErrStatus;
            BDError = comModule.BDError;
            CMErrStatus = comModule.CMErrStatus;
            CMError = comModule.CMError;

            nErrCnt = GetErrorCnt() == 0? 1 : GetErrorCnt();
            if (nErrCnt > 50 )
                return;
            title = new string[nErrCnt];
            content_index = new int[nErrCnt];
            /*
            if ( Errorstatus[0] == 1)
            {
                // Near-end sensor is ON. need to display a message
            }
            if( Errorstatus[1] == 1)
            {
                // Electricity is off
            }
            if( Errorstatus[2] == 1 )
                DisplayBV1Error();
            if (Errorstatus[3] == 1)
                DisplayBV2Error();
            if (Errorstatus[4] == 1)
                DisplayBDError();
            if (nErrCnt > 0 && erroridx == 0 )
            {
                title[erroridx] = "紙幣排出機と通信が出来ません";
                content_index[erroridx] = 10;
            }*/
            if( ExistError(Errorstatus) || nErrCnt > 0)
            {
                string errorCode = comModule.errorCode;
                string[] arr_errorCode = errorCode.Split('-');

                if ( Errorstatus[2] == 1 || (arr_errorCode[5] != "00"))
                {
                    messageDialog.ShowErrorDetailMessageOut(title, content_index, Errorstatus, arr_errorCode[5], arr_errorCode[6]);
                }
                if( Errorstatus[3] == 1 || (arr_errorCode[7] != "00"))
                {
                    messageDialog.ShowErrorDetailMessageOut(title, content_index, Errorstatus, arr_errorCode[7], arr_errorCode[8]);
                }
                if (Errorstatus[4] == 1 || (arr_errorCode[9] != "00"))
                {
                    messageDialog.ShowErrorDetailMessageOut(title, content_index, Errorstatus, arr_errorCode[9], arr_errorCode[10]);
                }
                if (Errorstatus[5] == 1 || (arr_errorCode[11] != "00"))
                {
                    messageDialog.ShowErrorDetailMessageOut(title, content_index, Errorstatus, arr_errorCode[11], arr_errorCode[12]);
                }

                
            }
                
        }

        private bool ExistError(int[] arr)
        {
            bool ret = false;
            for (int i = 1; i < arr.Length; i++)
                if (arr[i] == 1)
                {
                    ret = true;
                    break;
                }
            return ret;
        }

        private int GetErrorCnt()
        {
            int ncnt = 0, i;
            if(Errorstatus[2] == 1 )
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
            if (Errorstatus[5] == 1)
                for (i = 0; i < CMErrStatus.Length; i++)
                    if (CMErrStatus[i] != 0)
                        ncnt++;

            return ncnt;
        }

        public void ShowPanels()
        {
            if (mainFomeGlobal.InvokeRequired)
            {
                mainFomeGlobal.Invoke((MethodInvoker)delegate
                {
                    mainFomeGlobal.WindowState = FormWindowState.Maximized;
                    mainFomeGlobal.mainPanelGlobal.Show();
                    mainFomeGlobal.topPanelGlobal.Show();
                    mainFomeGlobal.bottomPanelGlobal.Show();
                });
            }
            else
            {
                mainFomeGlobal.WindowState = FormWindowState.Maximized;
                mainFomeGlobal.mainPanelGlobal.Show();
                mainFomeGlobal.topPanelGlobal.Show();
                mainFomeGlobal.bottomPanelGlobal.Show();
            }
        }

        public void ShowFunc()
        {
            ShowPanels();
        }

        private void MainMenuBtn(object sender, EventArgs e)
        {
            if (comModule != null)
            {
                comModule.CloseComport();
                comModule = null;
            }
            printDisconnect = 0;
            errStatus = 0;

            Button temp = (Button)sender;
            if (temp.Name == "maintenance")
            {
                if (tb_check)
                {
                    if( comModule != null)
                    {
                        comModule.CloseComport();
                        comModule = null;
                    }
                    passwordInput.CreateNumberInputDialog("maintenance", temp.Name);
                    passwordInput.dialogFormGlobal.ShowDialog();
                }
                else
                {
                    string errorMsg1 = "データ読み込みエラー";
                    string errorMsg2 = "データベースのチェックに失敗しました。\n「メニュー読込」から設定データをインポートしてください。";  // "Please go to Menu Reading section and get loading data.";
                    string audio_file_name = dbClass.AudioFile("ErrorOccur");
                    this.PlaySound(audio_file_name);

                    messageDialog.ShowOtherErrorMessage(errorMsg1, errorMsg2);
                }

            }
            else
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
                passwordInput.CreateNumberInputDialog("readingmenu", temp.Name);
                passwordInput.dialogFormGlobal.ShowDialog();
            }
        }

        public void SumThreadRuning()
        {
            if (tb_check)
            {
                while (flag)
                {
                    Console.WriteLine("HDChecking==>", HDChecking.ToString());
                    string currentTime = DateTime.Now.ToString("HH:mm");
                    if (String.Compare(currentTime, "00:00") >= 0 && String.Compare(currentTime, "00:10") <= 0)
                    {
                        if (dbClass.ProcessState())
                        {
                            dbClass.ClosingProcessWork();
                        }
                    }
                    if (HDChecking)
                    {
                        if (!UsbPrint())
                        {
                            flag = false;
                            sumThreadm = null;
                        }
                        if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
                        {
                            if (!TcpPrint())
                            {
                                flag = false;
                                sumThreadm = null;
                            }
                        }
                        if (!flag)
                        {
                            ShowPrintErrorMsgs();
                        }
                    }

                    Thread.Sleep(300000);
                }

            }

        }

        private void ShowPrintErrorMsgsStatic()
        {
            if (printDisconnect != 0)
            {
                constants.SaveLogData("mainMenu_****_1", "printer_disconnect");
                //string audio_file_name = dbClass.AudioFile("ErrorOccur");
                //this.PlaySound(audio_file_name);
                if (printType == 0)
                    messageDialog.ShowPrintErrorMessage("チケット", 0, 0, 0);
                else
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, 0, 0);
            }
            else if (errStatus != 0)
            {
                constants.SaveLogData("mainMenu_****_2", "printError===" + errStatus);
                //string audio_file_name = dbClass.AudioFile("ErrorOccur");
                //this.PlaySound(audio_file_name);
                if (printType == 0)
                    messageDialog.ShowPrintErrorMessage("チケット", 0, errStatus, 0);
                else
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, errStatus, 0);
            }
        }

        private void ShowPrintErrorMsgs()
        {
            if (printDisconnect != 0)
            {
                constants.SaveLogData("mainMenu_****_1", "printer_disconnect");
                //string audio_file_name = dbClass.AudioFile("ErrorOccur");
                //this.PlaySound(audio_file_name);
                if (printType == 0)
                    messageDialog.ShowPrintErrorMessage("チケット", 0, 0, 0);
                else
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, 0, 0);

            }
            else if( errStatus != 0 )
            {
                constants.SaveLogData("mainMenu_****_2", "printError===" + errStatus);
                //string audio_file_name = dbClass.AudioFile("ErrorOccur");
                //this.PlaySound(audio_file_name);
                if (printType == 0)
                    messageDialog.ShowPrintErrorMessage("チケット", 0, errStatus, 0);
                else
                    messageDialog.ShowPrintErrorMessage("キッチン", 1, errStatus, 0);
            }

            if ( !flag && sumThreadm == null)
            {
                printDisconnect = 0;
                errStatus = 0;
                flag = true;
                sumThreadm = new Thread(SumThreadRuning);
                sumThreadm.SetApartmentState(ApartmentState.STA);
                sumThreadm.Start();
            }
        }

        private void ClosingProcess()
        {
            dbClass.DBChecking();
            if (dbClass.dbState)
            {
                tb_check = true;
                dbClass.ClosingProcessWork();
            }
            else
            {
                tb_check = false;
            }

        }

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
            //System.Threading.Thread.Sleep(1000);
            //player.Stop();
        }

        public void GetPassword(string objectName, string passwords)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            string pwd = "5678";
            string adminPwd = "8765";
            if (key != null) {
                
                if (key.GetValue("POSPassword") != null)
                {
                    pwd = key.GetValue("POSPassword").ToString();
                }
                else if (key.GetValue("POSAdminPassword") != null)
                {
                    adminPwd = key.GetValue("POSAdminPassword").ToString();
                }
            }
            else if (key == null)
            {
                pwd = "5678";
                adminPwd = "8765";
            }
            passwordInput.dialogFormGlobal.Close();

            if ((passwords != "" && pwd == passwords) || adminPwd == passwords)
            {
                if(pwd == passwords)
                {
                    key.SetValue("userRole", "2");
                }
                else
                {
                    key.SetValue("userRole", "1");
                }

                if (sumThreadm != null)
                {
                    sumThreadm.Abort();
                    sumThreadm = null;
                    flag = false;
                }
                switch (objectName)
                {
                    case "maintenance":
                        FormPanel.Controls.Clear();
                        MaintaneceMenu maintaneceMenu = new MaintaneceMenu(mainFomeGlobal, FormPanel);
                        maintaneceMenu.TopLevel = false;
                        FormPanel.Controls.Add(maintaneceMenu);
                        maintaneceMenu.FormBorderStyle = FormBorderStyle.None;
                        maintaneceMenu.Dock = DockStyle.Fill;
                        Thread.Sleep(200);
                        maintaneceMenu.Show();
                        break;
                    case "readingmenu":
                        FormPanel.Controls.Clear();
                        MenuReading menuReading = new MenuReading(mainFomeGlobal, FormPanel);
                        menuReading.TopLevel = false;
                        FormPanel.Controls.Add(menuReading);
                        menuReading.FormBorderStyle = FormBorderStyle.None;
                        menuReading.Dock = DockStyle.Fill;
                        Thread.Sleep(200);
                        menuReading.Show();
                        break;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MainMenu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);

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

            sqlite_conn = new SQLiteConnection("Data Source=" + dbPath + "; Version = 3; New = True; Compress = True;  Connection Timeout=0");

            return sqlite_conn;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
