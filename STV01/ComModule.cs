using ComPC2MC;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;

namespace STV01
{

    public class ComModule
    {
        ComPC dllCom = null;

        Constant constants = new Constant();
        DBClass dbClass = new DBClass();

        private string comport = "";
        private string strLog = "";
        private string logPath = "";

        private byte[] recvData = null;
        private int nRepeat = 0;
        public int[] paperBLdata = new int[5];
        public int errorType = 0;

        private byte[] coinBLdata = new byte[5];

        public int[] cashState = new int[] { 1, 1, 1, 1, 1 };

        private bool bActive = false;
        public bool isError = false;

        public Thread getDepositTh = null;
        private bool bWithdraw = false;

        public bool bPermit = false;

        public int depositAmount = 0;
        public int[] depositArr = new int[8];
        public int[] withdrawArr = new int[8];

        private int orderTotalPrice = 0;
        SaleScreen pSs = null;
        Form1 pMm = null;
        MainMenu pParent = null;
        private string fileName = "";
        public bool noComport = false;

        int NDataReceive = 0;


        public void Initialize(SaleScreen ss)
        {
            pSs = ss;
            GetDetailDepositStatus(0x00);
        }

        public bool InitMicroCom(Form1 mm = null, MainMenu pmainMenu = null)
        {
            pMm = mm;
            pParent = pmainMenu;
            //logPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            //logPath += "\\";
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            logPath += "\\STV01\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            fileName = logPath + "log_Byte_Mode.txt";
            if (dllCom == null)
            {
                dllCom = new ComPC();
            }
            noComport = false;
            return InitAndGetInitStatus();
        }

        public bool InitMicroComForReset(Form1 mm = null)
        {
            return InitAndGetInitStatus();
        }

        private bool InitAndGetInitStatus()
        {
            if (!OpenComport())
            {
                errorType = 1;
                return false;
            }
            if (!InitCommand())
            {
                errorType = 1;
                return false;
            }
            errorType = 0;
            return true;
        }

        public void OrderChange(string orderPrice)
        {
           // if (!bPermit)
            //    PermitAndProhCommand(0x01, 0x01);

            if (getDepositTh == null)
            {
                bActive = false;
                getDepositTh = new Thread(GetDepositStateThread);
                getDepositTh.SetApartmentState(ApartmentState.STA);
                getDepositTh.Start();
            }
        }

        public bool TicketRun(int iAmount)
        {
            bool ret = false;
            StopDepositThread();

            if (dllCom == null)
            {
                if( pSs != null)
                {
                    pSs.bBackShow = true;
                    pSs.BackShow();
                }
                return ret;
            }

            int cnt = 0;
            LogResult("SEND : ", "01-FE-02-00-00-02");
        REPEAT:
            bool bstop = dllCom.SendReceivePermissionAndProhibition(0x00, 0x00);

            Thread.Sleep(500);

            if (bstop)
            {
                bstop = false;
                cnt++;
                bstop = ReceivedDataAnalProc();
                if (!bstop && cnt < 10)
                {
                    goto REPEAT;
                }
                if (bstop)
                {
                    LogResult("RECV : ", dllCom.GetbalanceByteData());
                    Thread.Sleep(100);

                    GetDetailDepositStatus(0x01);
                    while (bPermit)
                        goto REPEAT;
                    ret = WithdrawChangeAmount(iAmount);
                }
            }
            else if( pSs != null )
            {
                pSs.ErrorStatusOfImpoossible();
                ret = true;
            }
            return ret;
        }

        public void StopDepositThread()
        {
            nThreadCnt = 0;
            bActive = true;
            if (getDepositTh != null)
            {
                bActive = true;
                getDepositTh.Abort();
                getDepositTh = null;
            }
        }
        public void OrderCancel(int iAmount = 0)
        {
            int cnt = 0;
            StopDepositThread();
        REPEAT:
            bool bstop = dllCom.SendReceivePermissionAndProhibition(0x00, 0x00);
            LogResult("SEND : ", "01-FE-02-00-00-02");

            if (bstop)
            {
                cnt++;
                LogResult("RECV : ", dllCom.GetbalanceByteData());
                bstop = false;
                bstop = ReceivedDataAnalProc();
                if (!bstop && cnt < 10)
                {
                    goto REPEAT;
                }
                if (bstop)
                {
                    Thread.Sleep(300);

                    GetDetailDepositStatus(0x01);
                    while (bPermit)
                        goto REPEAT;

                    WithdrawRefund(iAmount);
                }
            }
        }

        public bool ResetModule()
        {
            nThreadCnt = 0;
            pSs = null;
            bActive = false;
            errorType = 0;
            bool ret = dllCom.SendResetCommand();
            if(ret)
                dllCom = null;
            return ret;
        }

        int nThreadCnt = 0;
        private void GetDepositStateThread()
        {
            bool highBank = true;
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                highBank = Convert.ToBoolean(key.GetValue("HighBankNote"));
            }
            while (!bActive)
            {
                //if (!bPermit )
                //    PermitAndProhCommand(0x01, 0x01);

                if (dllCom == null)
                {
                    bActive = true;
                    if (pSs != null)
                        pSs.isError = true;
                    break;
                }
                GetDetailDepositStatus(0x01);

                if (dllCom == null)
                {
                    bActive = true;
                    if(pSs !=null)
                        pSs.isError = true;
                    break;
                }
                depositAmount = bPermit? dllCom.moneyAmount : 0;
                pSs.SetDepositAmount(depositAmount);
                Thread.Sleep(100);
                if (highBank)
                {
                    pSs.isError = false;
                    Thread.Sleep(40);
                }
                else
                {
                    if (depositAmount >= 90000)
                    {
                        PermitAndProhCommand(0x00, 0x00);
                        bActive = true;
                        getDepositTh = null;
                        break;
                    }
                    else
                    {
                        pSs.isError = false;
                        Thread.Sleep(40);
                    }
                }
            }

            if ( bActive && pSs != null && bPermit )
            {
                pSs.bBackShow = true;
                pSs.BackShow();
            }
        }

        public void PermitCommand()
        {
            if (!bPermit)
                PermitAndProhCommand(0x01, 0x01);
        }

        public void PermitStopCommand()
        {
            if (bPermit)
                PermitAndProhCommand(0x00, 0x00);
        }

        public void CloseComport()
        {
            if (dllCom != null)
            {
                if (bPermit)
                    dllCom.SendReceivePermissionAndProhibition(0x00, 0x00);
                dllCom.CloseComport();
                dllCom = null;
            }
        }


        public bool OpenComport()
        {
            bool bexist = false;
            List<string> allPorts = new List<String>();
            allPorts = dllCom.GetAllPorts();

            comport = pMm.comport;

            foreach (String portName in allPorts)
            {
                if (portName == comport)
                {
                    bexist = true;
                    break;
                }
            }
                
            bool ret = false;
            if (comport != "" && bexist)
            {
                try
                {
                    dllCom.OpenComport(comport);
                    ret = true;
                }
                catch (Exception)
                {
                    noComport = true;
                    ret = false;
                }
            }
            else
            {
                noComport = true;
                return false;
            }
            dllCom.GetHexArray();
            return ret;
        }

        private bool InitCommand()
        {
            errorType = 0;

            LogResult("SEND : ", "00-FF-01-01-00");
            bool bret = dllCom.SendReceiveInitData();
            if (bret)
            {
                LogResult("RECV : ", dllCom.GetbalanceByteData());
                bret = ReceivedDataAnalProc();
            }
            else
                bret = false;
            return bret;
        }

        private bool InitDataAnal(byte b)
        {
            if (b == 0x22 && nRepeat >= 5)
            {
                nRepeat = 0;
                return false;
            }
            bool ret = false;
            if (b == 0x11)
                ret = true;
            else if ((b == 0x22 || b == 0xDD) && nRepeat < 5)
            {
                nRepeat++;
                Thread.Sleep(2000);
                ret = InitCommand();
            }
            else if (b == 0xEE)
            {
                nRepeat = 0;
                ret = false;
            }
            return ret;
        }

        public bool PermitAndProhCommand(byte b1, byte b2)
        {      
            errorType = 0;

            if (dllCom == null)
                return false;

            string b1_Str = b1.ToString();
            string b2_str = b2.ToString();

            LogResult("SEND : ", "01-FE-02-" + b1_Str + "-" + b2_str + "-02");
            bool ret = false;
            ret = dllCom.SendReceivePermissionAndProhibition(b1, b2);
            if (ret)
            {
                LogResult("RECV : ", dllCom.GetbalanceByteData());
                ret = ReceivedDataAnalProc();
            }
            return ret;
        }

        private bool PermitAndProhDataAnal(byte b1, byte b2)
        {
            errorType = 0;
            bool ret = false;
            if (nRepeat > 4)
            {
                nRepeat = 0;
                GetErrorStatus(0x07);
                errorType = 1;
                pSs.ErrorStatusOfImpoossible();
                return false;
            }
            if (b1 == 0x11 && b2 == 0x11)
            {
                bPermit = true;
                ret = true;
            }
            if (b1 == 0x22 && b2 == 0x22)
            {
                bPermit = false;
                ret = true;
            }
            else if (b1 == 0x55 && b2 == 0x55)
            {
                Thread.Sleep(1000);
                if (bPermit)
                    GetDetailDepositStatus(0x01);
                else
                    GetDetailWithdrawStatus();
                ret = true;
            }
            else if (b1 == 0xDD && b2 == 0xDD && nRepeat < 5)
            {
                nRepeat++;
                if (bPermit)
                    ret = PermitAndProhCommand(0x01, 0x01);
                else
                    ret = PermitAndProhCommand(0x00, 0x00);
            }
            else if (b1 == 0xEE && b2 == 0xEE)
            {
                errorType = 1;
                nRepeat = 0;
                GetErrorStatus(0x07);
                pSs.ErrorStatusOfImpoossible();
            }
            return ret;
        }

        private void GetDetailDepositStatus(byte b)
        {
            byte byteStatus = 0x00;
            byte[] tt = new byte[5];
            int cnt = 0;

            errorType = 1;

        REPEATCOMMAND:
            try
            {
                byteStatus = dllCom.GetDetailStatusData(b);
            }
            catch(IOException ioe)
            {
                if(pSs!= null)
                {
                    getDepositTh = null;
                    bActive = true;
                    pSs.bBackShow = true;
                    pSs.BackShow();
                    Console.WriteLine(ioe.ToString());
                    return;
                }
            }
            Thread.Sleep(400);
            //while ( NDataReceive < 5)
            //{
            //    Thread.Sleep(400);
            //    if (byteStatus == 0x22 || byteStatus == 0x11)
            //        break;
            //    NDataReceive++;
            //}
            //NDataReceive = 0;

            LogResult("SEND : ", dllCom.tempS);
            LogResult("RECV : ", dllCom.GetbalanceByteData());

            int[] deposit = dllCom.GetMoneyAmount();
            string str1 = "紙幣受付　（総額：" + dllCom.moneyAmount.ToString() + ")";
            string str2 = "10000円×" + deposit[7].ToString() + "、 5000円×" + deposit[6].ToString() + "、 2000円×" + deposit[5].ToString() + "、 1000円×" + deposit[4].ToString();

            depositArr = dllCom.balanceData;
            if (byteStatus == 0x22 || byteStatus == 0x11)
            {
                cnt++;

                if (byteStatus == 0x11)
                    bPermit = true;

                else if (getDepositTh != null && byteStatus != 0x11 && cnt < 20)
                    goto REPEATCOMMAND;

                else if (byteStatus != 0x22 && getDepositTh == null && cnt < 5)
                {
                    Thread.Sleep(300);
                    goto REPEATCOMMAND;
                }
                else if (byteStatus == 0x22)
                {
                    bPermit = false;
                    LogResult("Deposit Amount : ", dllCom.moneyAmount.ToString());
                }
            }
            else if (byteStatus == 0x33 || byteStatus == 0x44)
            {
                if (byteStatus == 0x44)
                {
                    LogResult("Withdraw Amount : ", dllCom.moneyAmount.ToString());
                    bWithdraw = false;
                }
                else bWithdraw = true;
                bPermit = false;
            }
            else if (byteStatus == 0xEE || byteStatus == 0xDD)
            {
                errorType = 1;
                bActive = true;
                getDepositTh = null;
                dbClass.InsertLog(3, str1, str2);
                GetErrorStatus(0x07);
                pSs.ErrorStatusOfImpoossible();
            }
        }

        public int withdrawAmount = 0;

        public bool GetDetailWithdrawStatus()
        {
            if (dllCom == null) return false;

            bool ret = false;
            byte byteStatus = 0x00;
            int nrepeat = 0;
            withdrawAmount = 0;
            for (int i = 0; i < 8; i++)
                withdrawArr[i] = 0;

        REPEEAT:
            LogResult("SEND : ", "03-FC-01-02-03");

            byteStatus = dllCom.GetDetailStatusData(0x02);
            Thread.Sleep(300);

            while (byteStatus == 0x33)
            {
                byteStatus = dllCom.GetDetailStatusData(0x02);
                Thread.Sleep(1000);
                LogResult("RECV : ", dllCom.GetbalanceByteData());
                if(byteStatus ==0xEE )
                {
                    LogResult("RECV : ", "Error detect");
                    break;
                }
            }

            LogResult("RECV : ", dllCom.GetbalanceByteData());
            int[] deposit = dllCom.GetMoneyAmount();

            if (byteStatus != 0x33)
            {
                withdrawAmount = dllCom.moneyAmount;
                withdrawArr = dllCom.balanceData;
            }

            if (byteStatus == 0x11 || byteStatus == 0x22)
            {
                ret = true;
                if (byteStatus == 0x11) bPermit = true;
                if (byteStatus == 0x22) bPermit = false;
            }
            else if (byteStatus == 0x44)
            {
                ret = true;
                bWithdraw = false;
            }
            else if ((byteStatus == 0xDD || byteStatus == 0x33) && nrepeat < 5)
            {
                ret = true;
                nrepeat++;
                Thread.Sleep(500);
                goto REPEEAT;
            }
            else if (byteStatus == 0xEE)
            {

                if (pSs == null)
                {
                    LogResult("RECV : ", "Sale screen instance is null");
                }
                ret = true;
                errorType = 2;
                //dbClass.InsertLog(3, str1, str2);
                GetErrorStatus(0x07);
                if (pSs != null)
                    pSs.ErrorStatusOfImpoossible();
            }
            return true;
        }

        private void WithdrawRefund(int amount)
        {
            bool highBank = true;
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                highBank = Convert.ToBoolean(key.GetValue("HighBankNote"));
            }
            int nWan = 0, nChon = 0, nBak = 0, nSib = 0;
            if (highBank)
            {
                nWan = amount / 10000;
                nChon = (amount - nWan * 10000) / 1000;
                nBak = (amount - nWan * 10000 - 1000 * nChon) / 100;
                nSib = (amount - nWan * 10000 - 1000 * nChon - 100 * nBak) / 10;
            }
            else
            {
                nChon = (amount) / 1000;
                nBak = (amount - 1000 * nChon) / 100;
                nSib = (amount - 1000 * nChon - 100 * nBak) / 10;
            }

            bSuccess = false;

            byte b1 = highBank ? GetByteTwoInteger(nSib, 1) : GetByteTwoInteger(nSib, 0);
            byte b2 = GetByteTwoInteger(0, nBak);
            byte b3 = GetByteTwoInteger(0, 0);
            int nRemains = 10000 * dllCom.balanceData[7] + 2000 * dllCom.balanceData[5] + 5000 * dllCom.balanceData[6] + 100 * nBak + 10 * nSib;
            int chonCnt = (amount - nRemains) / 1000;

            byte bCnt = 0x00;
            if (amount - nRemains >= 1000)
                bCnt = Convert.ToByte(chonCnt);
            else
                bCnt = Convert.ToByte(nChon);

            errorType = 3;

            bool bret = dllCom.SendReceiveWithdrawal(b1, b2, b3, bCnt);
            LogResult("SEND: ", dllCom.tempS);
            if (bret)
            {
                LogResult("RECV: ", dllCom.GetbalanceByteData());

                bSuccess = true;
                bret = false;
                Thread.Sleep(1000);
                bret = ReceivedDataAnalProc();
                if (bret)
                {
                    Thread.Sleep(4000);
                    if (!GetDetailWithdrawStatus())
                        return;

                    bBefore = true;
                    byte[] status = GetCoinMeshBalanceData(0x03);
                    bBefore = false;
                    if (!bWithdraw)
                    {
                        // PermitAndProhCommand(0x01, 0x01);
                        GetDetailDepositStatus(0x01);
                        if (getDepositTh == null && bActive)
                        {
                            bActive = false;
                            bPermit = false;
                            getDepositTh = new Thread(GetDepositStateThread);
                            getDepositTh.SetApartmentState(ApartmentState.STA);

                            getDepositTh.Start();
                        }
                    }
                }
            }
        }

        public int[] withdrawAmt = null;
        public bool bSuccess = false;
        private bool bBefore = false;
        private bool WithdrawChangeAmount(int amount)
        {
            if (dllCom == null)
                return false;

            int nChon = 0, nBak = 0, nSib = 0;
            withdrawAmt = null;
            bSuccess = false;
            bBefore = true;
            byte[] recv = GetCoinMeshBalanceData(0x03);

            bBefore = false;

            if (recv[3] == 0x00 && !bErrorOcure)
            {
                nChon = (amount) / 1000;
                nBak = (amount - 1000 * nChon) / 100;
                nSib = (amount - 1000 * nChon - 100 * nBak) / 10;

                byte b1 = GetByteTwoInteger(nSib, 0);
                byte b2 = 0x00, bCnt = 0x00;
                byte b3 = GetByteTwoInteger(0, 0);

                //if (paperBLdata[0] == 1)
                //{
                //    b2 = GetByteTwoInteger(nChon, nBak);
                //}
                //else
                {
                    b2 = GetByteTwoInteger(0, nBak);
                    bCnt = Convert.ToByte(nChon);
                }

                bool bret = dllCom.SendReceiveWithdrawal(b1, b2, b3, bCnt);
                Thread.Sleep(1000);
                LogResult("SEND : ", dllCom.tempS);
                LogResult("RECV : ", dllCom.GetbalanceByteData());

                withdrawAmt = new int[3] { nSib, nBak, nChon };
                errorType = 2;

                bSuccess = bret;
            }
            else if(pSs != null)
            {
                pSs.ErrorStatusOfImpoossible();

                errorType = 2;
                bSuccess = true;
                LogResult("Coin mesh error ===>", "tube break");
            }
            GetDetailWithdrawStatus();

            return true;
        }

        public void processAfterWithdraw()
        {
            if (!GetDetailWithdrawStatus())
                return;

            if (dllCom == null)
                return;

            GetCoinMeshBalanceData(0x03);

            Thread.Sleep(1000);
            if (!bWithdraw)
            {
                if (getDepositTh == null && bActive )
                {
                    bActive = false;
                    bPermit = false;
                    getDepositTh = new Thread(GetDepositStateThread);
                    getDepositTh.SetApartmentState(ApartmentState.STA);

                    getDepositTh.Start();
                }
            }
            else
            {
                processAfterWithdraw();
            }
        }

        private bool WithdrawalDataAnal(byte b)
        {
            bool ret = false;
            if (b == 0x11) ret = true;
            else if ((b == 0xDD || b == 0x55) && bWithdraw)
                ret = GetDetailWithdrawStatus();
            else
            {
                errorType = 2;
                GetErrorStatus(0x07);
                pSs.ErrorStatusOfImpoossible();
            }
            return ret;
        }

        private string getCurrentTime()
        {
            string retTime = "";
            DateTime time = DateTime.Now;
            retTime = time.ToString("yyyy-MM-dd h:mm:ss tt");
            return retTime;
        }

        private void LogErrorData(int nCase)
        {
            string strError = "";
            if (nCase == 0)
            {
                strError = dllCom.GetErrorMessage();
                LogResult("Init Reset Error: ", strError);
            }
            if (nCase == 1)
            {
                strError = dllCom.GetErrorMessage();
                LogResult("Deposit Permission and Prohibition Error: ", strError);
            }
            if (nCase == 2)
            {
                strError = dllCom.GetErrorMessage();
                LogResult("Withdrawal Error: ", strError);
            }
            if (nCase == 3)
            {
                strError = dllCom.GetErrorMessage();
                LogResult("Balance state: ", strError);
            }
        }

        public byte[] GetCoinMeshBalanceData(byte b)
        {
            for (int i = 0; i < 5; i++)
            {
                paperBLdata[i] = 0;
                coinBLdata[i] = 0x00;
                cashState[i] = 1;
            }
            LogResult("Balance state1: ", "");
            if (dllCom == null)
                return null;
            bool bret = false;
            NDataReceive = 0;
            while (NDataReceive < 4)
            {
                bret = dllCom.SendReceiveBalance(b);
                LogResult("Balance state1: ", "-"+NDataReceive.ToString());
                Thread.Sleep(80);
                NDataReceive++;
            }

            NDataReceive = 0;

            string send = dllCom.strSendData;
            LogResult("SEND : ", send);

            byte[] recv = null;
            string str = dllCom.GetbalanceByteData();

            string[] arrstr = str.Split('-');
            LogResult("RECV : ", str);
            recv = dllCom.ptReceiveData;

            if (recv[0] == 0x04)
            {
                if (recv[3] == 0x00)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        paperBLdata[i] = int.Parse(arrstr[i + 4]);
                        coinBLdata[i] = recv[i + 4];
                    }
                    if (paperBLdata[0] == 1)
                    {
                        cashState[0] = 2;
                        cashState[2] = 2;
                    }
                    if (paperBLdata[1] < 10 || paperBLdata[3] < 10)
                        cashState[3] = 2;
                    else if (paperBLdata[2] < 10 || paperBLdata[4] < 6)
                        cashState[3] = 0;
                    if ((cashState[3] == 2 || cashState[3] == 0 || cashState[0] == 2) && pSs != null && !bBefore )
                    {
                        if (paperBLdata[0] == 1 || paperBLdata[1] < 10 || paperBLdata[3] < 10 || paperBLdata[2] < 10 || paperBLdata[4] < 6)
                        {
                            int[] status = new int[5];
                            status[0] = paperBLdata[0] == 1 ? 0 : 1;
                            status[1] = paperBLdata[1] < 10 ? 0 : 1;
                            status[2] = paperBLdata[2] < 10 ? 0 : 1;
                            status[3] = paperBLdata[3] < 10 ? 0 : 1;
                            status[4] = paperBLdata[4] < 6 ? 0 : 1;
                            LogResult("error ====>", "");
                            pSs.ProcessCommoduleMsgs(paperBLdata, status);
                            bErrorOcure = true;
                        }
                    }
                }
                else if (recv[3] == 0xDD)
                    GetCoinMeshBalanceData(b);
                else if (recv[3] == 0xEE)
                {
                    errorType = 1;
                    bErrorOcure = true;
                    LogErrorData(3);
                    GetErrorStatus(0x04);
                }
            }

            return recv;
        }

        public int[] Errorstatus = null;
        public int[] BV1ErrStatus = null;
        public int[] BV2ErrStatus = null;
        public int[] BDErrStatus = null;
        public int[] CMErrStatus = null;

        public string BV1Error = "";
        public string BV2Error = "";
        public string BDError = "";
        public string CMError = "";
        public string errorCode = "";
        private string errCode1 = "";
        private string errCode2 = "";
        private bool bErrorOcure = false;

        private void InitErrStatusVariables()
        {
            Errorstatus = null;
            BV1ErrStatus = null;
            BV2ErrStatus = null;
            BDErrStatus = null;
            CMErrStatus = null;
            BV1Error = "";
            BV2Error = "";
            BDError = "";
            CMError = "";
            errorCode = "";
            errCode1 = "";
            errCode2 = "";

            for (int i = 0; i < 5; i++)
                cashState[i] = 1;
        }

        public bool GetErrorStatus(byte b)
        {
            bool ret = true;
            //LogErrorData(0);
            InitErrStatusVariables();
            if (dllCom == null)
                return false;
            Errorstatus = dllCom.GetErrorState(b);
            Thread.Sleep(300);

            //while (NDataReceive < 5)
            //{
            //    if (Errorstatus[0] == 1 || Errorstatus[1] == 1 || Errorstatus[2] == 1 || Errorstatus[3] == 1 || Errorstatus[4] == 1 || Errorstatus[5] == 1)
            //        break;
            //    NDataReceive++;
            //}
            //NDataReceive = 0;

            string send = dllCom.strSendData;
            LogResult("SEND : ", send);

            //Thread.Sleep(1000);
            if (Errorstatus[0] == -2)
                return false;
            BV1Error = dllCom.GetBV1ErrorAnalResult();
            BV1ErrStatus = dllCom.BVState1;
            BV2Error = dllCom.GetBV2ErrorAnalResult();
            BV2ErrStatus = dllCom.BVState2;
            BDError = dllCom.GetBDErrorAnalResult();
            BDErrStatus = dllCom.BDState;
            errorCode = dllCom.GetbalanceByteData();
            LogResult("RECV : ", errorCode);

            CMError = dllCom.GetCMErrorAnalResult();
            CMErrStatus = dllCom.CMState;

            string errCode = dllCom.GetbalanceByteData();

            string[] arr_BVError = BV1Error.Split('-');
            string[] arr_BV2Error = BV2Error.Split('-');
            string[] arr_BDError = BDError.Split('-');
            string[] arr_CMError = CMError.Split('-');

            string dispErrorMsg = "";

            //LogResult(BV1Error, "");
            //LogResult(BV2Error, "");
            //LogResult(BDError, "");
            //LogResult(CMError, "");

            string[] arr_errorCode = errorCode.Split('-');

            string errorMsg = null;
            if (Errorstatus[0] == 1)
            {
                // near-end sensor error
                cashState[0] = 2;
                errorType = 2;
                ret = false;
            }
            if (Errorstatus[1] == 1)
            {
                // turn off electricity
                dbClass.InsertLog(1, "紙幣識別機1 異常(営業中止)", "(Turn off Electricity" + " : " + errCode + ")");
                cashState[4] = 0;
                ret = false;
            }
            if (Errorstatus[2] == 1)
            {
                // bill validator1 error
                cashState[0] = 0;
                int[] temp = dllCom.BVState1; // temp[0]: strange bit, 1 : act possibe, 2 : validator, 3: escrow, 4: insertion error, 5: kan error, 6: misukum error, 7 : inbal error, 8: sender error

                for (int a = 1; a < arr_BVError.Length; a++)
                    dbClass.InsertLog(1, "紙幣識別機1 異常(営業継続)", "(" + arr_BVError[a] + " : " + errCode + ")");
                errorMsg = constants.bankNoteErrorMsg;

                if (temp[0] == 1)
                {
                    dispErrorMsg += BV1Error + "\n";
                }

                if (temp[5] == 1)
                {
                    errorMsg = "紙幣識別機１エラー収納部異常 \n Ex-001";
                }
                errorType = 1;
                errCode1 = arr_errorCode[5];
                errCode2 = arr_errorCode[6];
                ret = false;
            }
            if (Errorstatus[3] == 1)
            {
                // bill validator2 error
                string stramount = depositAmount.ToString();
                int[] deposit = dllCom.balanceData;
                string str2 = "1万×" + deposit[7].ToString() + "、五千×" + deposit[6].ToString() + "、二千×" + deposit[5].ToString() + "、千×" + deposit[4].ToString() + "、500×" + deposit[3].ToString() + "、100×" + deposit[2].ToString() + "、50×" + deposit[1].ToString() + "、10×" + deposit[0].ToString();
                for (int a = 1; a < arr_BV2Error.Length; a++)
                    dbClass.InsertLog(1, "紙幣識別機2 異常(入金中:" + stramount + ")", str2, "(" + arr_BV2Error[a] + " : " + errCode + ")");

                int[] temp = dllCom.BVState2;
                cashState[1] = 0;
                if (temp[0] == 1)
                {
                    dispErrorMsg += BV1Error + "\n";
                }
                if (temp[4] == 1)
                {
                    cashState[1] = 2;
                    errorMsg = constants.bankNoteDepositeErrorMsg + "\n購入金額 = " + orderTotalPrice.ToString() + "円\n入金金額 = " + depositAmount.ToString() + "円\n釣銭金額 = 0円 \n払出済み = 0円";
                }
                errorType = 1;
                ret = false;
            }
            if (Errorstatus[4] == 1)
            {
                string stramount = depositAmount.ToString();
                int restPrice = depositAmount - orderTotalPrice;
                int[] deposit = dllCom.balanceData;
                string str2 = "高額×" + (deposit[7] + deposit[6] + deposit[5]).ToString() + deposit[4].ToString() + "、500×" + deposit[3].ToString() + "、100×" + deposit[2].ToString() + "、50×" + deposit[1].ToString() + "、10×" + deposit[0].ToString();
                for (int a = 1; a < arr_BDError.Length; a++)
                    dbClass.InsertLog(1, "紙幣排出機 異常 (出金中: " + stramount + "/" + restPrice.ToString() + " 紙幣:" + deposit[4].ToString() + ")", str2, "(" + arr_BDError[a] + " : " + errCode + ")");

                // bill dispenser error
                int[] temp = dllCom.BDState; //0: main sender path, 1:Pick-up detail, 2:communication Error, 3: Hardware Error, 4: Pick-up Empty

                dispErrorMsg += BDError + "\n";

                cashState[2] = 0;
                if (temp[2] == 1)
                {
                    errorMsg = constants.bankNoteWithdrawErrorMsg + "\n購入金額 = " + orderTotalPrice.ToString() + "円\n入金金額 = " + depositAmount.ToString() + "円\n釣銭金額 = " + restPrice.ToString() + "円 \n払出済み = " + depositAmount + "円";
                }
                errorType = 2;
                ret = false;
            }
            if (Errorstatus[5] == 1)
            {
                // coin mesh error
                int[] temp = dllCom.CMState;
                cashState[3] = 3;
                int restPrice = depositAmount - orderTotalPrice;
                dispErrorMsg += CMError + "\n";
                errorType = 3;
                ret = false;
            }

           // LogResult("Error status code : ",Errorstatus[0].ToString() + "::" + Errorstatus[1].ToString() + "::" + Errorstatus[2].ToString() + "::" + Errorstatus[3].ToString() + "::" + Errorstatus[4].ToString() + "::" + Errorstatus[5].ToString());
            return ret;
        }

        private bool ReceivedDataAnalProc()
        {
            bool ret = false;
            recvData = dllCom.ptReceiveData;
            byte btManage = recvData[0];
            switch (btManage)
            {
                case 0x00:
                    ret = InitDataAnal(recvData[3]);
                    break;
                case 0x01:
                    ret = PermitAndProhDataAnal(recvData[3], recvData[4]);
                    break;
                case 0x02:
                    ret = WithdrawalDataAnal(recvData[3]);
                    break;
                case 0x03:
                    break;
                case 0x04:
                    break;
                case 0x05:
                    break;
            }
            return ret;
        }

        private byte GetByteTwoInteger(int val1, int val2)
        {
            byte ret = 0x00;

            string s1 = Convert.ToString(val1, 2);
            string s2 = Convert.ToString(val2, 2);
            int[] bits1 = s1.PadLeft(4, '0') // Add 0's from left
                         .Select(c => int.Parse(c.ToString())) // convert each char to int
                         .ToArray(); // Convert IEnumerable from select to Array
            int[] bits2 = s2.PadLeft(4, '0') // Add 0's from left
                         .Select(c => int.Parse(c.ToString())) // convert each char to int
                         .ToArray(); // Convert IEnumerable from select to Array
            int[] newbit = new int[8];
            Array.Copy(bits1, newbit, 4);
            Array.Copy(bits2, 0, newbit, 4, 4);

            string s = "";
            for (int i = 0; i < 8; i++)
                s += newbit[i].ToString();
            byte[] bytes = new byte[1];
            bytes[0] = Convert.ToByte(s, 2);
            ret = bytes[0];
            return ret;
        }

        int nFlag = 0;
        string strLogTemp = "";
        private string currentlogtext = "";

        public void LogResult(string str1, string str2)
        {
            string strTime = getCurrentTime();

            strLogTemp += Environment.NewLine;
            strLogTemp += strTime + " " + str1 + " " + str2;
            nFlag++;
            //strLog += strLogTemp;
            //SaveLogData();
            if (currentlogtext != str2 && nFlag == 2)
            {
                strLog += strLogTemp;

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
                if (File.Exists(fileName))
                    File.Delete(fileName);

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
    }
}

