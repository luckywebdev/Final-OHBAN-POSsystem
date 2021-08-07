using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanknoteTester
{
    class Constant
    {
        public string bankNoteErrorMsg = "紙幣識別機１エラー収納部異常";
        public string bankNoteDepositeErrorMsg = "紙幣識別機エラー挿入部異常";
        public string bankNoteWithdrawErrorMsg = "紙幣排出機エラーメイン搬送路異常";
        public string systemErrorMsg = "システムエラーが発生しました。";
        public string systemSubErrorMsg = "マニュアルに従ってサービスをご依頼ください。";

        public string[] statusBtn = new string[]{"正常", "通信", "異常", "#00b0f0", "#b66d31", "#8c3836", "#00b0f0", "#f79646", "#c0504d" };
        public string[] actionBtns = new string[] { "#8064a2", "#4bacc6", "#c0504d", "#f79646", "#9bbb59", "#f479ca", "#82f873" };
        public string disableClr = "#cbcbcb";
    }
}
