
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;

namespace BanknoteTester
{
    public partial class Test : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        Constant cs = new Constant();
        ComModule cm = null; 

        private int statusCode = 0;
        private string strLog = "";
        private string logPath = "";
        private string fileName = "";
        private string currentlogtext = "";
        private Image red = null;
        private Image green = null;

        private string UNIT = "円";
        private int withdrawAmt = 0;

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
        string ipAddress = "192.168.1.250";
        string port = "9100";
        public string comport = "";

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public Test()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string maplePortName = GetFullComputerDevices();
            comport = maplePortName.Substring(maplePortName.Length - 5, 4);

            if (comport == "")
            {
                MessageBox.Show("デバイスが見つかりません。");
                Application.Exit();
                return;
            }

            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.TopLevel = true;

            InitUI();
            InitLogFile();
            UsbPrint();

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");

            if (key.GetValue("netSelect") != null && Convert.ToBoolean(key.GetValue("netSelect")) == true)
            {
                TcpPrint();
            }
            cm = new ComModule();
            if (!cm.Initialize(this))
            {
                initDisable();
                resetDisable();
            }
        }

        private string GetFullComputerDevices()
        {
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");

            ManagementObjectCollection Ports = processClass.GetInstances();
            string device = "";
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("Maple Serial") &&
                        property.GetPropertyValue("Name").ToString().Contains("COM"))
                    {
                        device = property.GetPropertyValue("Name").ToString();
                        break;
                    }
            }
            return device;
        }

        private void InitUI()
        {
            statusDisable();
            permitDisable();
            permitStopDisable();
            changeDisable();
            errorDisable();
            withdrawDisable();
            lbl_out_chon_1.Size = new Size(50, 23);
            out_amount.Text = withdrawAmt.ToString();
            out_amount.Enabled = false;
            label_status.Text = "";
        }

        // window moving by mouse drag
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        // App exit
        private void btn_close_Click(object sender, EventArgs e)
        {
            if( btn_close.Enabled == false)
            {
                return;
            }

            if( cm != null)
            {
                cm.stopDepositThread();
                cm.CloseComport();
                cm = null;
            }
            this.Close();
        }

        private void btn_init_Click(object sender, EventArgs e)
        {
            initDisable();
            resetEnable();
            cm.InitCommand();
        }

        public void InfoReturnDataAnal(string str, byte bFlag)
        {
            LogResult("Recv [05] ", str);

            if (bFlag == 0x11)
            {
                statusCode = 0;
                updateStatus();
                resetEnable();
                permitEnable();
                changeEnable();
                errorEnable();
                cm.GetInitialStatus(0x00);
            }
            else if( bFlag == 0x22)
            {
                statusCode = 1;
                updateStatus();
                resetDisable();
                Thread.Sleep(2000);
                cm.InitCommand();
            }
            else if( bFlag == 0xDD )
            {
                statusCode = 1;
                updateStatus();
                resetDisable();
            }
            else
            {
                statusCode = 2;
                updateStatus();
                cm.GetErrorStatus(0x07);
            }
        }

        public void InitialStatusAnal(string recv, int[] vals, int amt, byte b)
        {
            LogResult("Recv [14] ", recv);
            updateDepositData(vals, amt);
            if (b == 0x11)
                ncnt = 0;
            if (b == 0xDD && ncnt < 10)
            {
                ncnt++;
                Thread.Sleep(300);
                cm.GetInitialStatus(0x00);
            }
            else if (b == 0xEE)
            {
                cm.GetErrorStatus(0x07);
            }
        }


        private void btn_reset_Click(object sender, EventArgs e)
        {
            closeDisable();
            statusDisable();
            label_status.Text = "";
            withdrawAmt = 0;
            int[] empty = new int[5] { 0, 0, 0, 0, 0 };
            int[] empty1 = new int[6] { 0, 0, 0, 0, 0, 0 };
            int[] empty2 = new int[8] { 0, 0, 0, 0, 0, 0,0,0 };
            updateWithdrawStatusData(empty, 0);
            updateErrorStatus(empty1);
            updateDepositData(empty2, 0);
            updateBalanceStatus();
            updateAmontText();
            updateAmountCnt();
            permitDisable();
            permitStopDisable();
            withdrawDisable();
            errorDisable();
            changeDisable();
            cm.stopDepositThread();
            cm.ResetCommand();
        }

        public void ResetDataAnal(string str, byte b)
        {
            LogResult("Recv [05] ", str);
            if( b == 0x11)
            {
                if (cm == null)
                    cm = new ComModule();
                else
                    cm.CloseComport();
                Thread.Sleep(2000);
                if (cm.Initialize(this))
                {
                    initEnable();
                    resetEnable();
                    closeEnable();
                }
                else
                {
                    this.Close();
                }
            }
            else if( b == 0xDD) {
                resetDisable();
                closeEnable();
                statusEnable();
                statusCode = 1;
                updateStatus();
            }
            else if( b== 0xEE)
            {
                statusEnable();
                closeEnable();
                statusCode = 2;
                updateStatus();
                cm.GetErrorStatus(0x07);
            }
        }

        private void btn_permit_Click(object sender, EventArgs e)
        {
            withdrawAmt = 0;
            updateAmontText();
            updateAmountCnt();

            int[] empty = new int[5] { 0, 0, 0, 0, 0 };
            updateWithdrawStatusData(empty, 0);

            cm.PermitCommand(0x01, 0x01);
        }

        bool bPermit = false;
        public void PermitCommandDataAnal(string str, byte b1, byte b2)
        {
            LogResult("Recv [06] ", str);
            bPermit = false;
            if ((b1 == 0x55 || b1 == 0xDD) && ncnt == 10)
            {
                cm.GetInitialStatus(0x01);
                ncnt = 0;
                return;
            }
            else if (b1 == 0x11 && b2 == 0x11)
            {
                permitDisable();
                permitStopEnable();
                withdrawDisable();

                // get deposit thread start
                cm.getDepositThread();
                ncnt = 0;
                bPermit = true;
            }
            else if ((b1 == 0x55 || b1 == 0xDD )&& ncnt < 10 )
            {
                ncnt++;
                Thread.Sleep(300);
                cm.PermitCommand(0x01, 0x01);
            }
            else if (b1 == 0xEE)
            {
                cm.stopDepositThread();
                cm.GetErrorStatus(0x07);
            }
        }


        byte[] strBalanceStatus = new byte[9];
        bool bBalance = false;
        public void getBalanceDataAnal(string str, byte[] recvArr)
        {
            LogResult("Recv [10] ", str);
            strBalanceStatus = recvArr;
            string[] arr = str.Split('-');
            updateBalanceStatus(arr);

            if (int.Parse(arr[7]) >= 10 && int.Parse(arr[5]) >= 10)
                bBalance = true;
            else
            {
                MessageBox.Show(this, "バランスデータは少なくなります。", "警告", MessageBoxButtons.OK);
                bBalance = false;
                withdrawAmt = 0;
                updateAmontText();
                int[] empty = new int[5] { 0, 0, 0, 0, 0 };
                updateWithdrawStatusData(empty, 0);
                withdrawDisable();
                ncnt = 0;
                return;
            }

            if (recvArr[3] == 0x00)
            {
                ncnt = 0;
            }
            else if ( ncnt<10 && recvArr[3] == 0xDD) {
                ncnt++;
                Thread.Sleep(300);
                cm.newGetBalanceData(0x03);
                bBalance = false;
            }
            else if (recvArr[3] == 0xEE)
            {
                cm.GetErrorStatus(0x07);
                bBalance = false;
            }
        }

        private void updateBalanceStatus(string[] arr= null)
        {
            lbl_change_chon.Text = arr == null ? "00": arr[4];
            lbl_change_5bak.Text = arr == null ? "00" : arr[8];
            lbl_change_1bak.Text = arr == null ? "00" : arr[7];
            lbl_change_5sib.Text = arr == null ? "00" : arr[6];
            lbl_change_sib.Text = arr == null ? "00" : arr[5];
            if( arr == null)
            {
                mark_change_chon.Image = green;
                button1.Image = green;
                button2.Image = green;
                button3.Image = green;
                button4.Image = green;
            }
            else
            {
                if (arr[4] == "01") mark_change_chon.Image = red; else mark_change_chon.Image = green;
                if (int.Parse(arr[8]) < 6) button1.Image = red; else button1.Image = green;
                if (int.Parse(arr[7]) < 10 ) button2.Image = red; else button2.Image = green;
                if (int.Parse(arr[6]) < 10 ) button3.Image = red; else button3.Image = green;
                if (int.Parse(arr[5]) < 10 ) button4.Image = red; else button4.Image = green;
            }
            
        }

        private void btn_unpermit_Click(object sender, EventArgs e)
        {
            cm.stopDepositThread();
            cm.ProhCommand(0x00, 0x00);
        }

        public void ProhCommandDataAnal(string str, byte b1, byte b2)
        {
            LogResult("Recv [06] ", str);
            if ((b1 == 0x22 && b2 == 0x22)|| ((b1 == 0x55 || b1 == 0xDD) && ncnt == 10))
            {
                permitEnable();
                permitStopDisable();
                withdrawEnable();
                ncnt = 0;
                bPermit = false;
            }
            else if ((b1 == 0x55 || b1 == 0xDD) && ncnt < 10 )
            {
                ncnt++;
                Thread.Sleep(300);
                cm.ProhCommand(0x00, 0x00);
            }
            else if (b1 == 0xEE)
            {
                cm.GetErrorStatus(0x07);
            }
        }

        private void btn_getchange_Click(object sender, EventArgs e)
        {
            cm.stopDepositThread();
            if( bPermit )
                cm.ProhCommand(0x00, 0x00);
            cm.newGetBalanceData(0x03);
            permitEnable();
            permitStopDisable();
        }

        private void btn_geterr_Click(object sender, EventArgs e)
        {
            cm.stopDepositThread();
            if( bPermit )
                cm.ProhCommand(0x00, 0x00);
            cm.GetErrorStatus(0x07);
            permitEnable();
            permitStopDisable();
        }

        public void errorStatusAnal(string str, int[] status, string BV, string BD, string CM)
        {
            LogResult("Recv [14] ", str + " " + BV + " " + BD + " " + CM);
            string[] arr = str.Split('-');
            updateErrorStatus(status, arr);

            if( arr[3] == "EE")
            {
                resetDisable();
                initDisable();
                permitDisable();
                permitStopDisable();
                changeDisable();
                errorDisable();
                withdrawDisable();
            }
        }

        private void updateErrorStatus(int[] status, string[] arr=null )
        {
            lbl_err_bill_1_up.Text = arr==null? "00": arr[5];
            lbl_err_bill_1_down.Text = arr == null ? "00" : arr[6];
            lbl_err_bill_2_up.Text = arr == null ? "00" : arr[7];
            lbl_err_bill_2_down.Text = arr == null ? "00" : arr[8];
            lbl_err_coin_up.Text = arr == null ? "00" : arr[11];
            lbl_err_coin_down.Text = arr == null ? "00" : arr[12];
            lbl_err_bout_up.Text = arr == null ? "00" : arr[9];
            lbl_err_bout_down.Text = arr==null? "00": arr[10];
            
            if (status[0] == 1) lbl_err_bout.Image = red; else lbl_err_bout.Image = green;
            if (status[2] == 1) lbl_err_bill1.Image = red; else lbl_err_bill1.Image = green;
            if (status[3] == 1) lbl_err_bill2.Image = red; else lbl_err_bill2.Image = green;
            if( status[4] == 1) lbl_err_bout.Image = red; else lbl_err_bout.Image = green;
            if (status[5] == 1) lbl_err_coin.Image = red; else lbl_err_coin.Image = green;
        }

        public void depositDataAnal(string recv, int[] vals, int amt, byte b)
        {
            LogResult("Recv [14] ", recv);
            updateDepositData(vals, amt);
            if (b == 0x11)
                ncnt = 0;
            if( b == 0xDD && ncnt < 10 )
            {
                ncnt++;
                Thread.Sleep(300);
                cm.stopDepositThread();
            }
            else if( b == 0xEE)
            {
                cm.GetErrorStatus(0x07);   
            }
        }

        private void updateDepositData(int[] val, int amt)
        {
            if (depositAmt.InvokeRequired)
            {
                depositAmt.Invoke((MethodInvoker)delegate
                {
                    depositAmt.Text = amt.ToString() + UNIT;
                    lbl_in_10.Text = val[0].ToString();
                    lbl_in_50.Text = val[1].ToString();
                    lbl_in_100.Text = val[2].ToString();
                    lbl_in_500.Text = val[3].ToString();
                    lbl_in_chon.Text = val[4].ToString();
                    lbl_in_2chon.Text = val[5].ToString();
                    lbl_in_5chon.Text = val[6].ToString();
                    lbl_in_man.Text = val[7].ToString();
                });

            }
            else
            {
                depositAmt.Text = amt.ToString() + UNIT;
                lbl_in_10.Text = val[0].ToString();
                lbl_in_50.Text = val[1].ToString();
                lbl_in_100.Text = val[2].ToString();
                lbl_in_500.Text = val[3].ToString();
                lbl_in_chon.Text = val[4].ToString();
                lbl_in_2chon.Text = val[5].ToString();
                lbl_in_5chon.Text = val[6].ToString();
                lbl_in_man.Text = val[7].ToString();
            }
        }

        private void dec10_Click(object sender, EventArgs e)
        {
            if (withdrawAmt < 10) return;
            withdrawAmt -= 10;
            updateAmontText();
            updateAmountCnt();
        }

        private void inc10_Click(object sender, EventArgs e)
        {
            if (withdrawAmt > 99989) return;
            withdrawAmt += 10;
            updateAmontText();
            updateAmountCnt();
        }

        private void dec100_Click(object sender, EventArgs e)
        {
            if (withdrawAmt < 100) return;
            withdrawAmt -= 100;
            updateAmontText();
            updateAmountCnt();
        }

        private void inc100_Click(object sender, EventArgs e)
        {
            if (withdrawAmt > 99899) return;
            withdrawAmt += 100;
            updateAmontText();
            updateAmountCnt();
        }

        private void updateAmountCnt()
        {
            int mancnt = withdrawAmt / 10000;
            int choncnt = (withdrawAmt - mancnt * 10000) / 1000;
            int bakcnt = (withdrawAmt - mancnt * 10000 - choncnt * 1000) / 100;
            int sibcnt = (withdrawAmt - mancnt * 10000 - choncnt * 1000 - bakcnt * 100 ) / 10;

            int outchoncnt = mancnt * 10 + choncnt;
            lbl_out_chon_1.Text = outchoncnt.ToString();
            lbl_out_man.Text = mancnt.ToString();
            lbl_out_chon_2.Text = choncnt.ToString();
            lbl_out_100.Text = bakcnt.ToString();
            lbl_out_10.Text = sibcnt.ToString();
        }

        private void btn_withdraw_Click(object sender, EventArgs e)
        {
            cm.ProhCommand(0x00, 0x00);
            withdrawDisable();
            permitDisable();
            permitStopDisable();
            cm.newGetBalanceData(0x02);

            int[] temp = new int[5]{ 0, 0, 0, 0, 0 };
            updateWithdrawStatusData(temp, 0);
            Thread.Sleep(500);
            if ( bBalance )
                cm.WithdrawChangeAmount(withdrawAmt);
        }

        public void withdrawDataAnal(string str, byte[] arr)
        {
            LogResult("Recv [05] ", str);
            byte b = arr[3];
            
            if( b == 0x11 || (b == 0x55 && ncnt == 10) )
            {
                // get withdraw status;
                withdrawAmt = 0;
                updateAmontText();
                withdrawDisable();
                cm.GetDetailWithdrawStatus();
                ncnt = 0;
            }
            if ((b == 0xDD || b == 0x55) && ncnt < 10 )
            {
                ncnt++;
                Thread.Sleep(300);
                cm.GetDetailWithdrawStatus();
            }
            else if (b == 0xEE)
            {
                cm.GetErrorStatus(0x07);
            }
        }
        int ncnt = 0;
        public void withdrawStatusAnal(string str, int[] vals, int amt, byte b)
        {
            LogResult("Recv [14] ", str);
            updateWithdrawStatusData(vals, amt);
            Thread.Sleep(100);
            if(b == 0x44)
            {
                // completed withdrawal
                permitEnable();
                withdrawEnable();
                ncnt = 0;
            }
            else if(( b == 0xDD || b == 0x33 ) && ncnt < 10 )
            {
                ncnt++;
                Thread.Sleep(300);
                cm.GetDetailWithdrawStatus();
            }
            else if( b == 0xEE)
            {
                cm.GetErrorStatus(0x07);
            }
        }

        private void updateWithdrawStatusData(int[] val, int amt)
        {
            out_status.Text = amt.ToString() + UNIT;

            lbl_out_status_10.Text = val[0].ToString();
            lbl_out_status_50.Text = val[1].ToString();
            lbl_out_status_100.Text = val[2].ToString();
            lbl_out_status_500.Text = val[3].ToString();
            lbl_out_status_chon.Text = val[4].ToString();
        }

        private void lbl_change_bak_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_change_1bak.DisplayRectangle, Color.FromArgb(255, 155, 187, 89), ButtonBorderStyle.Solid);
        }

        private void lbl_change_5sib_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_change_5sib.DisplayRectangle, Color.FromArgb(255, 155, 187, 89), ButtonBorderStyle.Solid);
        }

        private void lbl_change_sib_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_change_sib.DisplayRectangle, Color.FromArgb(255, 155, 187, 89), ButtonBorderStyle.Solid);
        }

        private void lbl_change_5bak_Paint(object sender, PaintEventArgs e)
        {   
            ControlPaint.DrawBorder(e.Graphics, lbl_change_5bak.DisplayRectangle, Color.FromArgb(255, 155, 187, 89), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bill_1_down_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bill_1_down.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bill_2_up_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bill_2_up.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bill_2_down_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bill_2_down.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_coin_up_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_coin_up.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_coin_down_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_coin_down.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bout_up_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bout_up.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bout_down_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bout_down.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void label_status_Click(object sender, EventArgs e)
        {

        }

        private void InitLogFile()
        {
            //logPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            //logPath += "\\";
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            logPath += "\\STV01\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }


            fileName = logPath + "communication_result.txt";
            red = Image.FromFile(@"resources\\red.png");
            green = Image.FromFile(@"resources\\green.png");
        }

        private void updateAmontText()
        {
            out_amount.Text = withdrawAmt.ToString();
        }
        
        private void closeDisable()
        {
            btn_close.Enabled = false;
            btn_close.BackColor = Color.Gray;
            btn_close.Invalidate();
        }

        private void closeEnable()
        {
            btn_close.Enabled = true;
            btn_close.BackColor = ColorTranslator.FromHtml("#4472c4");
            Invalidate();
        }
        private void withdrawDisable()
        {
            btn_withdraw.Enabled = false;
            btn_withdraw.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
            inputBtnsDisable();
        }
        private void withdrawEnable()
        {
            btn_withdraw.Enabled = true;
            btn_withdraw.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[4]);
            inputBtnsEnable();
        }
        private void errorDisable()
        {
            btn_geterr.Enabled = false;
            btn_geterr.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void errorEnable()
        {
            btn_geterr.Enabled = true;
            btn_geterr.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[2]);
        }
        private void changeDisable()
        {
            btn_getchange.Enabled = false;
            btn_getchange.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void changeEnable()
        {
            btn_getchange.Enabled = true;
            btn_getchange.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[1]);
        }

        private void permitStopDisable()
        {
            btn_unpermit.Enabled = false;
            btn_unpermit.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void permitStopEnable()
        {
            btn_unpermit.Enabled = true;
            btn_unpermit.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[2]);
        }

        private void permitDisable()
        {
            btn_permit.Enabled = false;
            btn_permit.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void permitEnable()
        {
            btn_permit.Enabled = true;
            btn_permit.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[1]);
        }

        private void resetDisable()
        {
            btn_reset.Enabled = false;
            btn_reset.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void resetEnable()
        {
            btn_reset.Enabled = true;
            btn_reset.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[3]);
        }
        private void initDisable()
        {
            btn_init.Enabled = false;
            btn_init.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void initEnable()
        {
            btn_init.Enabled = true;
            btn_init.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[0]);
        }

        private void statusDisable()
        {
            label_status.Enabled = false;
            label_status.BackColor = ColorTranslator.FromHtml(cs.disableClr);
        }
        private void statusEnable()
        {
            label_status.Enabled = true;
            updateStatus();
        }

        private void inputBtnsEnable()
        {
            dec10.Enabled = true;
            inc10.Enabled = true;
            dec100.Enabled = true;
            inc100.Enabled = true;
            dec10.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[5]);
            dec100.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[5]);
            inc10.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[6]);
            inc100.ButtonColor = ColorTranslator.FromHtml(cs.actionBtns[6]);
        }

        private void inputBtnsDisable()
        {
            dec10.Enabled = false;
            inc10.Enabled = false;
            dec100.Enabled = false;
            inc100.Enabled = false;
            dec10.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
            dec100.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
            inc10.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
            inc100.ButtonColor = ColorTranslator.FromHtml(cs.disableClr);
        }

        private void updateStatus()
        {
            label_status.BackColor = ColorTranslator.FromHtml(cs.statusBtn[6 + statusCode]);
            label_status.Text = (string)cs.statusBtn[statusCode];
        }

        // draw rect by paint event
        private void lbl_change_chon_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_change_chon.DisplayRectangle, Color.FromArgb(255, 155, 187, 89), ButtonBorderStyle.Solid);
        }

        private void lbl_err_bill_1_up_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_err_bill_1_up.DisplayRectangle, Color.FromArgb(255, 192, 80, 77), ButtonBorderStyle.Solid);
        }

        private void lbl_in_500_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_in_500.DisplayRectangle, Color.FromArgb(155, 186, 223), ButtonBorderStyle.Solid);
        }

        private void lbl_in_man_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lbl_in_man.DisplayRectangle, Color.FromArgb(255, 128, 100, 162), ButtonBorderStyle.Solid);
        }

        int nFlag = 0;
        string strLogTemp = "";
        public void LogResult(string str1, string str2)
        {
            string curtime = cm.getCurrentTime();
            strLogTemp += Environment.NewLine;
            strLogTemp += curtime + " " + str1 + " " + str2;
            nFlag++;

            if (currentlogtext != str2 && nFlag == 2)
            {
                strLog += strLogTemp;

                if (command_text.InvokeRequired)
                {
                    command_text.Invoke((MethodInvoker)delegate
                    {
                        command_text.Text = strLog;
                    });

                }
                else
                {
                    command_text.Text = strLog;
                }

                SaveLogData();
                currentlogtext = str2;
            }

            if (nFlag % 2 == 0)
            {
                strLogTemp = "";
                nFlag = 0;
            }
        }

        private void SaveLogData()
        {
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(strLog);
                    fs.Write(title, 0, title.Length);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private void Test_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cm != null)
            {
                cm.stopDepositThread();
                cm.CloseComport();
            }
            this.Close();
        }

        private void Test_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btn_close.Enabled == false)
            {
                return;
            }
            if (cm != null)
            {
                cm.stopDepositThread();
                cm.CloseComport();
            }

        }

        // check the print status
        private void use_prt_status_Click(object sender, EventArgs e)
        {
            UsbPrint();
        }

        private void usb_test_prt_Click(object sender, EventArgs e)
        {
            byte[] data = GetPrintData();
            UsbPrint(data);
        }

        private void net_prt_status_Click(object sender, EventArgs e)
        {
            TcpPrint();
        }

        private void net_test_prt_Click(object sender, EventArgs e)
        {
            byte[] data = GetPrintData();
            TcpPrint(data);
        }

        private void UsbPrint(byte[] data = null)
        {
            int a = UsbOpen("HMK-060");
            if (a != 0)
            {
                this.usb_printer_status_label.Text = "USBプリンター通信エラー";
            }
            else
            {
                int status = NewRealRead();
                prt_status((byte)status, "USB");
                if(status == 0 && data != null)
                {
                    foreach(byte cmd in data)
                    {
                        a = PrintCmd(cmd);
                    }
                }

            }
        }

        Socket s = null;
        RegistryKey key = null;

        private void TcpPrint(byte[] data = null)
        {
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("ipAddress") != null)
                {
                    ipAddress = Convert.ToString(key.GetValue("ipAddress"));
                }
                if (key.GetValue("port") != null)
                {
                    port = Convert.ToString(key.GetValue("port"));
                }
            }

            try
            {
                s = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                s.Connect(ipAddress, int.Parse(port));
                byte[] byData = new byte[] { 0x1D, 0x61, 0x01 };
                int rs = s.Send(byData);
                Console.WriteLine("statusSendResult===>" + rs);
                byte[] buffer = new byte[2];
                int rByte = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                string byteStr = Encoding.Default.GetString(buffer, 0, rByte);
                Console.WriteLine("statusReceiveResult===>" + byteStr + "&&" + rByte);
                string type = "Network";
                prt_status(buffer[0], type);
                if(data != null && buffer[0] == 0)
                {
                    s.Send(data);
                }
                s.Close();
                s = null;
            }
            catch (SocketException e)
            {
                this.net_printer_status_label.Image = red;
                this.net_printer_status_label.Text = "Networkプリンター通信エラー";
            }
        }

        static byte[] Sjistojis(byte[] data)
        {
            byte[] data2;
            uint i, j, first_code, second_code, str_len;
            data2 = data;
            byte[] JisBuf = new byte[data2.Length];

            str_len = (uint)data2.Length;

            // Shift Jis to Jis 
            i = 0;
            for (j = 0; j < str_len / 2; j++)
            {

                first_code = data2[i++];
                second_code = data2[i++];

                // first code
                if (first_code < 0xe0)
                {
                    first_code -= 0x81;        // 81-a0
                    first_code <<= 1;
                    first_code += 0x21;
                }
                else
                {
                    first_code -= 0xe0;        // E0-a0
                    first_code <<= 1;
                    first_code += 0x5f;
                }
                if (second_code >= 0x9f) first_code += 1;

                // second code
                if (second_code < 0x7f)
                {
                    second_code -= (0x40 - 0x21);
                }
                else if (second_code < 0x9f)
                {
                    second_code -= (0x80 - 0x60);
                }
                else if (second_code < 0xfd)
                {
                    second_code -= (0x9f - 0x21);
                }

                JisBuf[--i] = (byte)second_code;
                JisBuf[--i] = (byte)first_code;
                i += 2;
            }
            return JisBuf;
        }

        private byte[] GetPrintData()
        {
            DateTime now = DateTime.Now;

            byte[] reset = new byte[] { 0x1B, 0x40 };
            byte[] JapaneseFont = new byte[] { 0x1B, 0x4D, 0x20 };
            byte[] Korean = new byte[] { 0x1B, 0x4D, 0x00 };
            byte[] Linefeed = new byte[] { 0x0A };
            byte[] Print_feed = new byte[] { 0x1B, 0x64, 0x02 };
            byte[] Feed = new byte[] { 0x1B, 0x4A, 0x01 };
            byte[] sendBytes, rcvBytes;
            byte[] Left = new byte[3] { 0x1b, 0x61, 0x00 };
            byte[] Center = new byte[3] { 0x1b, 0x61, 0x01 };
            byte[] Right = new byte[3] { 0x1b, 0x61, 0x02 };
            byte[] Cut_full_0 = new byte[] { 0x1D, 0x56, 0x00 };
            //byte[] Cut_partial = new byte[] { 0x1D, 0x56, 0x01 };
            byte[] Cut_full = new byte[] { 0x1B, 0x69 };
            byte[] Cut_partial = new byte[] { 0x1B, 0x6D };
            byte[] print_speed = new byte[] { 0x1A, 0x73, 0x08 };
            byte[] Bold = new byte[] { 0x1B, 0x45, 0x01 };
            byte[] Emphasize = new byte[] { 0x1B, 0x45, 0x02 };
            byte[] Emphasize_release = new byte[] { 0x1B, 0x45, 0x00 };
            byte[] Extension = new byte[] { 0x1D, 0x21, 0x22 };
            byte[] Extension_release = new byte[] { 0x1D, 0x21, 0x00 };

            List<byte> tempList = new List<byte>();

            tempList.AddRange(Center);
            tempList.AddRange(JapaneseFont);
            sendBytes = Encoding.GetEncoding(932).GetBytes("印刷テスト");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Linefeed);
            tempList.AddRange(Linefeed);

            tempList.AddRange(Left);
            sendBytes = Encoding.GetEncoding(932).GetBytes("品名: 梅しそ巻とロース膳");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Linefeed);

            sendBytes = Encoding.GetEncoding(932).GetBytes("単価");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Korean);
            sendBytes = Encoding.GetEncoding(437).GetBytes(":  1200");
            tempList.AddRange(sendBytes);
            tempList.AddRange(JapaneseFont);
            sendBytes = Encoding.GetEncoding(932).GetBytes("円");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Linefeed);

            sendBytes = Encoding.GetEncoding(932).GetBytes("数量");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Korean);
            sendBytes = Encoding.GetEncoding(437).GetBytes(":     3");
            tempList.AddRange(sendBytes);
            tempList.AddRange(JapaneseFont);
            sendBytes = Encoding.GetEncoding(932).GetBytes("点");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Linefeed);

            sendBytes = Encoding.GetEncoding(932).GetBytes("金額");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Korean);
            sendBytes = Encoding.GetEncoding(437).GetBytes(":   3600");
            tempList.AddRange(sendBytes);
            tempList.AddRange(JapaneseFont);
            sendBytes = Encoding.GetEncoding(932).GetBytes("円");
            rcvBytes = Sjistojis(sendBytes);
            tempList.AddRange(rcvBytes);
            tempList.AddRange(Linefeed);
            tempList.AddRange(Linefeed);
            tempList.AddRange(Linefeed);

            tempList.AddRange(Right);
            tempList.AddRange(Korean);
            sendBytes = Encoding.GetEncoding(437).GetBytes(now.ToString("yyyy/MM/dd  HH:mm:ss"));
            tempList.AddRange(sendBytes);

            tempList.AddRange(Left);


            tempList.AddRange(Linefeed);
            tempList.AddRange(Linefeed);
            tempList.AddRange(Linefeed);

            tempList.AddRange(Print_feed);
            tempList.AddRange(Cut_full_0);
            tempList.AddRange(Cut_full);

            return tempList.ToArray();
        }


        private void SetText(string text)
        {
            tempLbl.Text = text;
        }

        Label tempLbl = null;

        private void prt_status(byte status, string type = "")
        {
            Console.WriteLine("PrintError=>" + status);
            if(type == "USB")
            {
                tempLbl = this.usb_printer_status_label;
                if(status == 0)
                {
                    this.usb_printer_status_label.Image = green;
                }
                else
                {
                    this.usb_printer_status_label.Image = red;
                }
            }
            else
            {
                tempLbl = this.net_printer_status_label;
                if (status == 0)
                {
                    this.net_printer_status_label.Image = green;
                }
                else
                {
                    this.net_printer_status_label.Image = red;
                }
            }

            switch (status)
            {
                case 0:
                    SetText(type + "プリンター通常のステータス");
                    break;
                case 1:
                    SetText(type + "プリンター用紙切れ");
                    break;
                case 2:
                    SetText(type + "プリンターカバー開放");
                    break;
                case 3:
                    SetText(type + "プリンター用紙切れ、カバー解放");
                    break;
                case 4:
                    SetText(type + "プリンター用紙詰まり");
                    break;
                case 5:
                    SetText(type + "プリンター用紙詰まり、用紙切れ");
                    break;
                case 6:
                    SetText(type + "プリンター用紙詰まり、カバー解放");
                    break;
                case 7:
                    SetText(type + "プリンター用紙詰まり、用紙切れ、カバー解放");
                    break;
                case 8:
                    SetText(type + "プリンター用紙不足");
                    break;
                case 9:
                    SetText(type + "プリンター用紙切れ");
                    break;
                case 10:
                    SetText(type + "プリンター用紙不足 、カバー解放");
                    break;
                case 11:
                    SetText(type + "プリンター用紙切れ 、カバー解放");
                    break;
                case 12:
                    SetText(type + "プリンター用紙詰まり、用紙切れ");
                    break;
                case 13:
                    SetText(type + "プリンター用紙詰まり、用紙切れ");
                    break;
                case 14:
                    SetText(type + "プリンター用紙詰まり、用紙不足、カバー解放");
                    break;
                case 15:
                    SetText(type + "プリンター用紙詰まり、用紙切れ、カバー解放");
                    break;
                case 16:
                    SetText(type + "Print Running");
                    break;
                case 32:
                    SetText(type + "プリンターカット不良");
                    break;
            }
        }


    }
}
