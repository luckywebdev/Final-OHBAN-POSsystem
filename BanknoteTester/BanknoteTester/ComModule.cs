using ComPC2MC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanknoteTester
{
    class ComModule
    {
        ComPC dllCom = null;

        Constant constants = new Constant();
        private byte[] recvData = null;
        private int[] paperBLdata = new int[5];
        private byte[] coinBLdata = new byte[5];

        private int[] cashState = new int[] { 1, 1, 1, 1, 1 };

        private bool bActive = false;
        public Thread getDepositTh = null;
        public Thread getBalanceTh = null;

        public int depositAmount = 0;
        public int[] depositArr = new int[8];
        private string COMPORT = "";

        Test pSs = null;

        public bool Initialize(Test ss)
        {
            pSs = ss;
            COMPORT = pSs.comport;
            dllCom = new ComPC();
            dllCom.GetHexArray();
            return OpenComport();
        }
        public bool reInitialize()
        {
            return OpenComport();
        }
        private bool OpenComport()
        {
            bool bexist = false;
            List<string> allPorts = new List<String>();
            allPorts = dllCom.GetAllPorts();

            foreach (String portName in allPorts)
                if (portName == COMPORT)
                {
                    bexist = true;
                    break;
                }

            if (!bexist)
            {
                string errorMsg1 = "COMポートを確認して再起動してください";
                string errorMsg2 = "c-001";
                pSs.LogResult(errorMsg1, errorMsg2);
                return false;
            }
            bool ret = false;
            if (COMPORT != "")
            {
                try
                {
                    dllCom.OpenComport(COMPORT);
                    ret = true;
                }
                catch (Exception)
                {
                    ret = false;
                }
            }
            else
            {
                string errorMsg3 = "COMポートを選択してください！";
                string errorMsg4 = "c-002";
                pSs.LogResult(errorMsg3, errorMsg4);
            }
            return ret;
        }

        public bool InitCommand()
        {
            pSs.LogResult("Send [05] ", "00-FF-01-01-00");
            bool bret = dllCom.SendReceiveInitData();
            if (bret)
            {
                string recv = dllCom.GetbalanceByteData();
                recvData = dllCom.ptReceiveData;
                pSs.InfoReturnDataAnal(recv, recvData[3]);
            }
            else
                MessageBox.Show(dllCom.GetErrorMessage());
            return bret;
        }
        public void ResetCommand()
        {
            bActive = false;
            pSs.LogResult("Send [05] ", "00-FF-01-FF-FE");
            bool ret = dllCom.SendResetCommand();
            if (ret)
            {
                string str = dllCom.GetbalanceByteData();
                recvData = dllCom.ptReceiveData;
                if (dllCom != null)
                    dllCom = null;
                pSs.ResetDataAnal(str, recvData[3]);
            }
            else
                MessageBox.Show(dllCom.GetErrorMessage());
        }

        public bool PermitCommand(byte b1, byte b2)
        {
            pSs.LogResult("Send [06] ", "01-FE-02-01-01-02");
            bool ret = false;
            ret = dllCom.SendReceivePermissionAndProhibition(b1, b2);
            if (ret)
            {
                string str = dllCom.GetbalanceByteData();
                recvData = dllCom.ptReceiveData;
                pSs.PermitCommandDataAnal(str, recvData[3], recvData[4]);
            }
            else
                MessageBox.Show(dllCom.GetErrorMessage());
            return ret;
        }
        public void getDepositThread()
        {
            if (getDepositTh == null)
            {
                getDepositTh = new Thread(GetDepositStateThread);
                getDepositTh.Start();
            }
        }

        private void GetDepositStateThread()
        {
            while (!bActive)
            {
                for (int i = 0; i < 5; i++) cashState[i] = 1;
                GetDetailDepositStatus(0x01);
                Thread.Sleep(40);
                depositAmount = dllCom.moneyAmount;
                depositArr = dllCom.balanceData;
            }
        }

        public void GetInitialStatus(byte b)
        {
            byte byteStatus;
            byte[] tt = new byte[5];

            byteStatus = dllCom.GetDetailStatusData(b);
            pSs.LogResult("Send [05] ", dllCom.tempS);
            string str = dllCom.GetbalanceByteData();
            int[] blData = dllCom.balanceData;
            int amt = dllCom.moneyAmount;

            pSs.InitialStatusAnal(str, blData, amt, byteStatus);
        }

        public void GetDetailDepositStatus(byte b)
        {
            byte byteStatus;
            byte[] tt = new byte[5];

            byteStatus = dllCom.GetDetailStatusData(b);
            pSs.LogResult("Send [05] ", dllCom.tempS);
            string str = dllCom.GetbalanceByteData();
            int[] blData = dllCom.balanceData;
            int amt = dllCom.moneyAmount;
            if (pSs.InvokeRequired)
            {
                pSs.Invoke((MethodInvoker)delegate
                {
                    pSs.depositDataAnal(str, blData, amt, byteStatus);
                });
            }
            else
            {
                pSs.depositDataAnal(str, blData, amt, byteStatus);
            }
        }
        public void stopDepositThread()
        {
            if (getDepositTh != null)
            {
                getDepositTh.Abort();
                getDepositTh = null;
            }
        }

        public bool ProhCommand(byte b1, byte b2)
        {
            pSs.LogResult("Send [06]", "01-FE-02-00-00-02");
            bool ret = false;
            ret = dllCom.SendReceivePermissionAndProhibition(b1, b2);
            if (ret)
            {
                string str = dllCom.GetbalanceByteData();
                recvData = dllCom.ptReceiveData;
                pSs.ProhCommandDataAnal(str, recvData[3], recvData[4]);
            }
            else
                MessageBox.Show(dllCom.GetErrorMessage());
            return ret;
        }

        byte nearEnd = 0x00;
        public void newGetBalanceData(byte b)
        {
            nearEnd = 0x00;
            bool bret = dllCom.SendReceiveBalance(b);
            string send = dllCom.strSendData;
            pSs.LogResult("Send [05]", send);

            if (bret)
            {
                string str = dllCom.GetbalanceByteData();
                byte[] recv = dllCom.ptReceiveData;
                nearEnd = recv[4];
                pSs.getBalanceDataAnal(str, recv);
            }
            else
                MessageBox.Show(dllCom.GetErrorMessage());
        }

        public bool GetErrorStatus(byte b)
        {
            bool ret = true;
            pSs.LogResult("Send [05]", "05-FA-01-07-06");
            int[] status = dllCom.GetErrorState(b);
            string BVError = dllCom.GetBV1ErrorAnalResult();
            string BDError = dllCom.GetBDErrorAnalResult();
            string CMError = dllCom.GetCMErrorAnalResult();
            string errCode = dllCom.GetbalanceByteData();
            pSs.errorStatusAnal(errCode, status, BVError, BDError, CMError);
            return ret;
        }

        public void WithdrawChangeAmount(int amount)
        {
            int nChon = 0, nBak = 0, nSib = 0;
            nChon = (amount) / 1000;
            nBak = (amount - 1000 * nChon) / 100;
            nSib = (amount - 1000 * nChon - 100 * nBak) / 10;

            byte b1 = GetByteTwoInteger(nSib, 0);
            byte b2 = 0x00, bCnt = 0x00;
            if (nearEnd == 0x01)
            {
                b2 = GetByteTwoInteger(nChon, nBak);
            }
            else
            {
                b2 = GetByteTwoInteger(0, nBak);
                bCnt = Convert.ToByte(nChon);
            }

            byte b3 = GetByteTwoInteger(0, 0);

            bool bret = dllCom.SendReceiveWithdrawal(b1, b2, b3, bCnt);
            pSs.LogResult("Send [08]", dllCom.tempS);

            string str = dllCom.GetbalanceByteData();
            recvData = dllCom.ptReceiveData;
            pSs.withdrawDataAnal(str, recvData);
        }

        public void CloseComport()
        {
            if (dllCom != null)
                dllCom.CloseComport();
        }

        public void GetDetailWithdrawStatus()
        {
            byte byteStatus;
            pSs.LogResult("Send [05]", "03-FC-01-02-03");
            byteStatus = dllCom.GetDetailStatusData(0x02);
            int amt = dllCom.moneyAmount;
            int[] deposit = dllCom.GetMoneyAmount();
            string str = dllCom.GetbalanceByteData();
            pSs.withdrawStatusAnal(str, deposit, amt, byteStatus);
        }

        public string getCurrentTime()
        {
            string retTime = "";
            DateTime time = DateTime.Now;
            retTime = time.ToString("yyyy-MM-dd h:mm:ss tt");
            return retTime;
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
    }
}
