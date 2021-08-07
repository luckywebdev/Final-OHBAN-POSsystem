using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class Constant
    {
        public string storeName = "ラーメン世代";
        public string dbName = "obanp1";
        public string yesStr = "はい";
        public string noStr = "いいえ";
        public string unit = "円";
        public string amountUnit = "枚";
        public string amountUnit1 = "点";
        public string yearLabel = "年";
        public string monthLabel = "月";
        public string dayLabel = "日";
        public string hourLabel = "時";
        public string minuteLabel = "分";
        public string confirmLabel = "確認";
        public string cancelLabel = "キャンセル";
        public string cancelRun = "取消実行";
        public string categoryListPrintMessage = "全てのカテゴリーを印刷 しても宜しいですか？";
        public string categoryListPrintTitle = "カテゴリー一覧";
        public string categoryListTitleLabel = "表示位置/カテゴリーNo";
        public string categoryDiplayLabel = "表示位置";
        public string categoryLabel = "カテゴリー";
        public string groupListTitleLabel = "グループ名";
        public string groupTitleLabel = "グループ";
        public string groupListPrintMessage = "全てのグループを印刷 しても宜しいですか？";
        public string groupListPrintTitle = "グループ一覧";
        public string TimeLabel = "販売時間";
        public string SaleTimeLabel = "販売時刻";
        public string prevButtonLabel = "プレビュー";
        public string printButtonLabel = "一覧印刷";
        public string printProductNameField = "印刷品目名";
        public string salePriceField = "販売価格";
        public string saleLimitField = "限定数";
        public string saleStatusLabel = "販売中";
        public string saleStopLabel = "中止";
        public string saleStopText = "利用停止";
        public string saleStopStatusText = "販売停止";
        public string currentDateLabel = "現在の日付";
        public string currentTimeLabel = "現在の時刻";
        public string timeSettingLabel = "時刻設定";
        public string dateSettingTitle = "日付設定";
        public string timeSettingTitle = "時間設定";
        public string passwordSettingLabel = "暗証番号設定";
        public string oldPasswordLabel = "旧暗証番号";
        public string newPasswordLabel = "新暗証番号";
        public string confirmPasswordLabel = "新暗証番号(確認用)";
        public string charClearLabel = "一文字クリア";
        public string allClearLabel = "全クリア";
        public string settingLabel = "設定";
        public string backupRestore = "データバックアップ";
        public string passwordInputTitle = "パスワードを入力";
        public string receiptionTitle = "領収書発行一覧";
        public string receiptionField = "印刷日時";
        public string dailyReportTitle = "売上日報";
        public string priceField = "金額";
        public string closingProcessTitle = "締め処理";
        public string timeRangeLabel = "時台";
        public string logReportLabel = "ログ表示";
        public string falsePurchaseTitle = "誤購入取消／表示";
        public string falsePurchaseSubTitle1 = "誤購入取消";
        public string falsePurchaseSubContent1 = "誤購入の取消を行う場合は下記のボタンを\nタッチしてください。";
        public string falsePurchaseSubTitle2 = "誤購入一覧表示";
        public string falsePurchaseButton = "誤購入取消";
        public string falsePurchaseStartLabel = "開始";
        public string falsePurchaseEndLabel = "終了";
        public string falsePurchaseListLabel = "一覧表示";
        public string falsePurchasePageTitle = "取り消された注文の一覧表示";
        public string orderTimeField = "注文日付";
        public string prodNameField = "品名";
        public string saleNumberField = "売上連番";
        public string openTimeChangeTitle = "営業変更";
        public string dayType = "曜日タイプ";
        public string startTimeLabel = "営業開始時刻";
        public string endTimeLabel = "営業終了時刻";
        public string menuReadingTitle = "メニュー読込";
        public string menuReadingSubContent1 = "USBメモリをセットしてメニュー読込ボタンを押して、別ウィンドウが開いたら読み込むメニューを選択してください。";
        public string menuReadingSubContent2 = "中止する場合は、取消ボタンを押してください。";
        public string menuReadingButton = "メニュー読込";
        public string menuReadingErrorTitle = "データに問題があります。";
        public string menuReadingErrorContent = "設定ソフトウェアより「USBメモリへ の書込」を行った後に再度試してください。";
        public string restEmptyMessage = "ご注文の商品は売り切れになりました。";
        public string orderDialogRunText = "注文内容確認";
        public string sumLabel = "合計";
        public string receiptInstruction = "上記金額正に領収しました。";

        public string soldoutSettingTitle = "売り切れ設定";
        public string categorylistLabel = "カテゴリー選択";
        public string prdNameField = "品目名";
        public string saleStateSettingField = "状態";

        public string sumProgressAlert = "締め処理中です。終わるまでお待ちください";

        public string orderCancelDialogTitle = "取消確認";
        public string orderDate = "注文日";
        public string orderTime = "注文時間";
        public string orderProductList = "品目";
        public string orderSumPrice = "合計金額";



        public string cancelErrorMessage = "締め処理を行った後は取消出来ません";
        public string cancelResultErrorMessage = "データが無いか、日付が誤っています";

        public string prdCategoryField = "所属カテゴリー";
        public string prdPriceFieldIncludTax = "販売価格(税込)";
        public string prdSaleTimeField = "販売時刻設定";
        public string prdScreenText = "画面メッセージ";
        public string prdPrintText = "印刷メッセージ";

        public string errorMsgTitle = "係員をお呼びください。";
        public string systemErrorMsg = "システムエラーが発生しました。";
        public string systemSubErrorMsg = "マニュアルに従ってサービスをご依頼ください。";
        public string printErrorMsg = "レシート用紙切れです。\nロール紙を補充してください。";
        public string printSubErrorMsg = "完了しましたらエラー解除ボタンを押してください。";
        public string printOfflineErrorMsg = "プリンタは現在オフラインです。";
        public string dbErrorTitle = "資料基地大湯です。";
        public string dbErrorContent = "メニュー読込に行って、まずデータをロード受けてください。";
        public string bankNoteErrorMsg = "紙幣識別機１エラー収納部異常";
        public string bankNoteSubErrorMsg = "完了しましたらエラー解除ボタンを押してください。";
        public string bankNoteDepositeErrorMsg = "紙幣識別機エラー挿入部異常";
        public string bankNoteWithdrawErrorMsg = "紙幣排出機エラーメイン搬送路異常";


        public string unChanged = "未変更";
        public string dayTypeBefore = "デフォルトの曜日タイプ";
        public string dayTypeAfter = "変更後の曜日タイプ";
        public string startTimeBefore = "デフォルトの開始時間";
        public string startTimeAfter = "変更御の開始時間";
        public string endTimeBefore = "デフォルトの終了時間";
        public string endTimeAfter = "変更後の終了時間";
        public string openTimeInstruction1 = "曜日タイプが変更された場合、指定された曜日の販売時間設定、\n商品構成に切り替わります。";
        public string openTimeInstruction2 = "デフォルトの開始時間より前に設定された場合、\nデフォルトの販売開始時間に販売される商品が早期開始対象になります。";
        public string openTimeInstruction3 = "デフォルトの終了時間より後に変更された場合、\nデフォルトの販売終了時間の直前に販売されている商品が延長対象になります。\n（売り切れ商品はそのまま）";
        public string openTimeSettingMessageTitle = "営業変更";
        public string openTimeSettingMessageContent = "設定内容を保存し前の画面に戻ります。変更内容は即時反映されます。宜しいですか？";
        public string openTimeCancelMessageTitle = "営業変更取消";
        public string openTimeCancelMessageContent = "設定内容を元に戻して前の画面に戻ります。宜しいですか？";

        public string gettingLabel = "了解";
        public string shutdownMsg = "券売機ソフトウェアを終了します。\n画面が黒くなったら、30秒ほど待ってメイン電源を切断してください。";
        public string errShutdown = "エラー発生中ですが、電源を切りますか？";
        public string runButtonLabel = "実行";

        public string BanknoteNormalStatus = "高額紙幣使用可能です";
        public string CoinNormalStatus = "全ての硬貨使用可能です";
        public string BanknoteStatus = "紙幣 釣銭切れ";
        public string CoinStatus = "硬貨 釣銭切れ";
        public string HighPaperError = "紙幣は千円札のみ使用出来ます";
        public string selectedMenuTitle = "ご選択中のメニユー";
        public string priceTitle = "価格";
        public string depositeLabel = "入 金 金 額";
        public string changeLabel = "お つ り";

        public string statusMessage_ok = "全金種ご利用になれます。";
        public string statusMessage_no = "全金種利用不可能です。";
        public string statusMessage_paper = "紙幣は千円のみご利用可能です。";
        public string statusMessage_deposite = "お金を投入してください。";
        public string statusMessage_runticket = "発券ボタンを押してください。";
        public string statusMessage_widthraw = "釣銭を払い出します。";
        public string statusMessage_ticketing = "チケットを発券しています。";
        public string statusMessage_receipting = "領収書を発行しています。";
        public string statusMessage_finish = "ありがとうございました。";

        public string loadingMessage = "ただいま取消処理中です。\n 暫くお待ちください。";
        public string loadingMessage_1000 = "千円札は下から出ます";

        public string backRestoreTitle = "データバックアップ";
        public string backupLabel = "データバックアップ";
        public string restoreLabel = "データリストア";
        public string backupDialogText = "券売機の全てのデータをUSBメモリにバックアップします。\n USBメモリを接続してください。\n バックアップを開始してよいですか？";
        public string restoreDialogText = "券売機の全てのデータをUSBメモリからリストアします。\n USBメモリを接続してください。\n 券売機のデータは削除され、USBから読込むデータに置き換わります。\n リストアを開始してよいですか？";
        public string noUSBError = "USBメモリが見つかりません。接続してください。";
        public string backupError = "データのバックアップに失敗しました。再度やり直してください。";
        public string restoreError = "データのリストアに失敗しました。やり直しても同じ場合はデータが破損している可能性があります。";
        public string spaceError = "USBメモリの容量不足です。新しいUSBメモリを接続してください。";
        public string dataRunSuccess = "処理が完了しました。\n USBメモリを抜いてOKを押してください。";

        public string logManage = "ログ表示";
        public string errorLogShow = "エラーログ";
        public string errorLogDetail = "エラーログ表示";
        public string logDetailView = "個別表示/印刷";
        public string logSumView = "一覧表示/保存";
        public string dailyReportLabel = "日報";
        public string viewLabel = "表示";
        public string printLabel = "印刷";
        public string dateRangeLabel = "範囲";
        public string allSaveLabel = "一覧保存";
        public string yearDataLabel = "年データ";
        public string monthDataLabel = "月データ";
        public string rangeDataLabel = "範囲データ";

        public string printerSetTitle = "プリンタの設定";
        public string ipaddressLabel = "IPアドレス";
        public string portLabel = "ポート";

        public string timeoutSetTitle = "タイムアウト・トップ画面変更設定";
        public string HDCheckTitle = "起動項目 設定";
        public string BankNoteSetTitle = "紙 幣 設 定";

        public string version = "Ver 1.8.8-E5";
        //public string version = "Ver 1.2 (" + new DateTime(2020, 12, 22).ToString("yyyyMMdd") + ")";

        public string[] roundedNumber = new string[]
        {
           "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨", "⑩"
        };

        public int singleticketPrintPaperWidth = 203;
        public int singleticketPrintPaperHeight = 35 * 9;
        public int multiticketPrintPaperWidth = 203;
        public int multiticketPrintPaperHeight = 35 * 8;
        public int receiptPrintPaperWidth = 203;
        public int receiptPrintPaperHeight = 1200;

        public int dailyReportPrintPaperWidth = 203;
        public int dailyReportPrintPaperHeight = 560;
        public int receiptReportPrintPaperWidth = 203;
        public int receiptReportPrintPaperHeight = 560;
        public int logPrintPaperWidth = 203;
        public int logPrintPaperHeight = 560;

        public int categorylistPrintPaperWidth = 203;
        public int categorylistPrintPaperHeight = 560;
        public int grouplistPrintPaperWidth = 203;
        public int grouplistPrintPaperHeight = 560;

        public int fontSizeBig = 14;
        public int fontSizeMedium = 9;
        public int fontSizeSmall = 8;
        public int fontSizeSmaller = 6;

        public string[] tbNames = new string[] { "CategoryTB", "ProductTB", "CategoryDetailTB", "SaleTB", "TableSetTicket", "TableSetReceipt", "TableSetStore", "DaySaleTB", "ReceiptTB", "CancelOrderTB", "TableGroupName", "TableGroupDetail", "GeneralTB", "TableSetAudio", "logTB", "ProductOptionTB", "ProductOptionValueTB" };

        public string[] defaultTbNames = new string[] { "CategoryTB", "ProductTB", "CategoryDetailTB", "TableSetTicket", "TableSetReceipt", "TableSetStore", "TableGroupName", "TableGroupDetail", "GeneralTB", "TableSetAudio" };


        public string[] dayTypeValue = new string[] { "平日", "土曜", "日曜" };

        public string[] months1 = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        public string[] dates1 = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };
        public string[] months2 = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        public string[] dates2 = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31" };
        public string[] times = new string[] { "全て", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"};
        public string[] end_times = new string[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };

        public string[] errorTypes = new string[] { "全て", "金銭エラー", "プリンタエラー", "紙幣関係", "その他エラー", "その他" };

        public string[] minutes = new string[] { "00", "15", "30", "45" };
        public string[] end_minutes = new string[] { "00", "15", "30", "45" };
        public string[] main_Menu = new string[2] { "メンテナンス", "メニュー読込" };  // , "販売画面"
        public string[] main_Menu_Name = new string[2] { "maintenance", "readingmenu" }; //, "salescreen"
        public string[] saleCategories = new string[] { "定番ラーメン", "替わり唾ラーメン", "トッピング", "ご飯、餃子", "ドリンク" };
        public int[] saleCategoryLayout = new int[] { 16, 25, 9, 13, 21 };
        public string[] saleCategories_btnName = new string[] { "category_1", "category_2", "category_3", "category_4", "category_5" };
        public string[] transactionLabelName = new string[] { "投入 金額", "購入 金額", "釣銭" };
        public string[] productAmount = new string[] { "1 枚", "2 枚", "3 枚", "4 枚", "5 枚", "6 枚", "7 枚", "8 枚", "9 枚", "10 枚" };
        public int maxOrderAmount = 10;
        public string dialogTitle = "注文メニュー";
        public string dialogInstruction = "複数注文時は▲▼ボタンで選んで決定ボタンを押してください。";

        public string saleScreenTopTitle = "いらっしゃいませ\n定番メニューがおすすめです。";
        public string main_Menu_Title = "処理を選択して下さい。";
        public string upButtonName = "upButton";
        public string downButtonName = "downButton";
        public string ticketingButtonText = "発券";
        public string cancelButtonText = "取消";
        public string receiptButtonText = "領収書";
        public string deleteText = "削除";
        public string backText = "戻る";
        public string orderOutMsg = "注文した商品の種類が最大になりました。これ以上別商品の注文は出来ません。お金を投入するか、投入済みであれば 発券ボタンを押してください。";
        public string[] maintanenceLabel = new string[] { "各種処理", "内容表示", "設  定", "ハードウエア 設定" };
        public string[][] maintanenceButton = new string[][]
        {
            new string[] { "売切れ設定", "締め処理", "誤購入取消", "ログ表示" },
            new string[] { "商品品目", "カテゴリー", "電源・再起動\n設定", "タイムアウト\nトップ画面変更" }, //グループ
            new string[] { "時刻設定", "暗証番号", "プリンタの\n設定", "バックアップ\nリストア" }, //営業変更
            new string[] { "起動項目\n設定", "紙幣設定", "商品情報の\n変更", "ハードウエア\nテスト" } //営業変更, 紙幣設定
        };

        public string upButtonImage = @"resources\\upButton.png";
        public string downButtonImage = @"resources\\downButton.png";
        public string increaseButtonImage = @"resources\\increaseButton.png";
        public string decreaseButtonImage = @"resources\\decreaseButton.png";

        public string errImage = @"resources\\error.png";

        public string errImageOut = @"resources\\error.png";


        // sale stop error
        public string[] unit_error_instruction_1 = new string[]
        {
            "紙幣識別部に異常があります。\nトップドアを開いてゴミや汚れがないか確認してください。\n 確認後、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。\n 高額紙幣はリセット後返却されます。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-1
            "紙幣識別ストックされた紙幣が引き抜かれました。\n 状況を確認して必要な対応を行ってください。\n 復帰する場合はエラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-2
            "紙幣挿入部に異常があります。\n 挿入部にゴミや汚れ、異物が詰まっていないか確認してください。\n 確認後、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。\n 高額紙幣はリセット後返却されます。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-3
            "紙幣が収納出来ませんでした。\n トップドア、バックドアを開いて確認し、紙幣が詰まっていれば取り除いてください。\n 認したらエラーリセットボタンを押してください。\n 紙幣はお客様に返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-4
            "収納途中の紙幣が引き抜かれました。\n 状況を確認して必要な対応を行ってください。\n 復帰する場合はエラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-5
            " 搬送駆動部に異常があります。\n トップドア、バックドアを開いて、搬送経路にゴミや汚れ、\n 異物が詰まっていないか確認してください。\n また、紙幣が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。高額紙幣はリセット後返却されます。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-6
            "金庫が満杯です。\n 金庫から紙幣を取り除き、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。高額紙幣はリセット後返却されます。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-7
            "紙幣識別機2（千円側）と通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 入金があれば、お客様に返金してください。\n 高額紙幣がストックされている場合は再起動後に返却されますが、再起動してもエラーになる場合は、\n メーカーへお問い合わせください。", //err-8
            "紙幣の排出に異常があります。\n 紙幣が通る部分に汚れ、異物、紙幣が詰まっていないか確認してください。\n また、異物や紙幣が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n 残金があれば、お客様に返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-9
            "紙幣のピックアップ部に異常があります。\n 紙幣の格納部から紙幣を取り除いて、汚れ、異物、紙幣が詰まっていないか確認してください。\n また、異物や紙幣が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n 釣銭があれば、お客様に返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-10
            "紙幣排出機の故障を検知しました。\n 一旦電源を落として、再度起動してください。\n 釣銭があれば、お客様に返金してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-11
            "紙幣排出機と通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 釣銭があれば、お客様に返金してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-12
            "硬貨識別部に異常があります。\n 硬貨識別部の部分に汚れ、異物、硬貨が詰まっていないか確認してください。\n また、異物や硬貨が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-13
            "○円硬貨の硬貨エンプティスイッチに異常があります。\n チューブを外し、硬貨を一旦全て出して、ゴミや汚れ異物が無いか確認し、再度入れ直してください。\n 確認後、エラーリセットボタンを押してください。\n 入金があれば、お客様に返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-14
            "硬貨払出部に異常があります。\n チューブを外し、チューブの中に異なった硬貨や異物などが入っていないか確認してください。\n また、チューブを外したコインメックの下部の払出口を塞いでいないか確認してください。\n 確認後、エラーリセットボタンを押してください。\n 釣銭があれば、お客様に硬貨を返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-15
            "カセットチューブが取り外れました。\n チューブをセットし、エラーリセットボタンを押してください。\n 意図して取り外した場合でない場合は、入金があれば、お客様に硬貨を返金してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-16
            "コインメックと通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 入金があれば、お客様に返金してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-17
            "金銭ユニットとの通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 入金や釣銭があれば、お客様に返金してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-18
            "COMポートを確認して再起動してください。", //err-19
            "システムエラーが発生しました。\n マニュアルに従ってサービスをご依頼ください。", //err-20
            "硬貨ユニットに問題があります。\n 硬貨チューブが正しく装着されているか確認して下さい。\n 問題無ければ【復帰】アイコンをクリックしてください。" //err-21(coin mech open)
        };

        // sale no stop error
        public string[] unit_error_instruction_2 = new string[]
        {
            "紙幣識別部に異常があります。\n トップドアを開いてゴミや汚れがないか確認してください。\n 確認後、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-1
            "紙幣識別ストックされた紙幣が引き抜かれました。\n 状況を確認して必要な対応を行ってください。\n 復帰する場合はエラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-2
            "紙幣挿入部に異常があります。\n 挿入部にゴミや汚れ、異物が詰まっていないか確認してください。\n 確認後、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-3
            "紙幣が収納出来ませんでした。\n トップドア、バックドアを開いて確認し、紙幣が詰まっていれば取り除いてください。\n 確認したらエラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-4
            "収納途中の紙幣が引き抜かれました。\n 状況を確認して必要な対応を行ってください。\n 復帰する場合はエラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-5
            "搬送駆動部に異常があります。\n トップドア、バックドアを開いて、搬送経路にゴミや汚れ、\n 異物が詰まっていないか確認してください。\n また、紙幣が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-6
            "金庫が満杯です。\n 金庫から紙幣を取り除き、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-7
            "紙幣識別機1（高額側）と通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-8
            "紙幣の排出に異常があります。\n 紙幣が通る部分に汚れ、異物、紙幣が詰まっていないか確認してください。\n また、異物や紙幣が詰まってる場合は取り除いてください。\n 確認後、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合はメーカーへお問い合わせください。", //err-9
            "紙幣排出機の故障を検知しました。\n 一旦電源を落として、再度起動してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-10
            "紙幣排出機と通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-11
            "金銭ユニットとの通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 入金や釣銭があれば、お客様に返金してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //err-12
        };

        public string[] printErrMsg = new string[]
        {
            "プリンターと通信が出来ません。\n 一旦電源を落として、再度起動してください。\n 再起動してもエラーになる場合は、メーカーへお問い合わせください。", //print_err-1
            "プリンターの用紙がありません。\n 確認し、新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //print-paper-out-2
            "プリンターのカバーが開いています。\n 確認し、カバーを閉じてください。\n 処置が終わったら、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //cover open-3
            "プリンターの用紙がありません。\n 確認して新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper out & cover open-4
            "プリンターの用紙詰まりが発生しました。\n 確認し、用紙を取り除いて、再度セットしてください。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper jam-5
             "プリンターの用紙詰まり、用紙切れが発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。 ", //paper out & jam-6
             "プリンターの用紙詰まりが発生しました。\n 確認し、用紙を取り除き、再度セットしてください。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //cover open & paper jam-7
             "プリンターの用紙詰まり、用紙切れが発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper out & jam & cover open-8
             "プリンターの用紙が残り僅かです。\n 確認し、新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //print-paper near end-9
             "プリンターの用紙が無いか、残り僅かです。\n 確認し、新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。 ", //print-paper-out & near end-10
             "プリンターの用紙が残り僅かです。\n 確認し、新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。 ", //paper near end & cover open-11
             "プリンターの用紙が無いか、残り僅かです。\n 確認して新しい用紙と交換して下さい。\n 交換後、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper out & near end & cover open-12
             "プリンターの用紙詰まり、用紙切れが発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper near end & jam-13
             "プリンターの用紙詰まり、用紙切れが発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper out & near end & jam-14
             "プリンターの用紙詰まり、用紙不足が発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper near end & jam & cover open-15
             "プリンターの用紙詰まり、用紙切れが発生しました。\n 確認し、用紙を取り除き、新しい用紙と交換して下さい。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしてもエラーになる場合は、メーカーへお問い合わせください。", //paper out & near end & jam & cover open-16
             "プリンターに用紙カット不良が発生しました。\n 確認し、用紙を取り除いて、再度セットしてください。\n 処置が終わったら、カバーを閉じ、エラーリセットボタンを押してください。\n リセットしても頻繁に発生する場合は、メーカーへお問い合わせください。", //cutter jam-17
             "「HwaUSB.dll」ライブラリを読み込めません。" // dll error-18
        };

        public string[] printErrTitle = new string[]
        {
            "", //print_err-1
            "プリンター用紙切れ", //print-paper-out-2
            "プリンターカバー開放", //cover open-3
            "プリンター用紙切れ、カバー解放", //paper out & cover open-4
            "プリンター用紙詰まり", //paper jam-5
            "プリンター用紙詰まり、用紙切れ", //paper out & jam-6
            "プリンター用紙詰まり、カバー解放", //cover open & paper jam-7
             "プリンター用紙詰まり、用紙切れ、カバー解放", //paper out & jam & cover open-8
             "プリンター用紙不足", //print-paper near end-9
             "プリンター用紙切れ", //print-paper-out & near end-10
             "プリンター用紙不足 、カバー解放", //paper near end & cover open-11
             "プリンター用紙切れ 、カバー解放", //paper out & near end & cover open-12
             "プリンター用紙詰まり、用紙切れ", //paper near end & jam-13
             "プリンター用紙詰まり、用紙切れ", //paper out & near end & jam-14
             "プリンター用紙詰まり、用紙不足、カバー解放", //paper near end & jam & cover open-15
             "プリンター用紙詰まり、用紙切れ、カバー解放", //paper out & near end & jam & cover open-16
             "プリンターカット不良", //cutter jam-17  
             "プリンターdllエラー", //printer dll error-18
             "",
             "",
        };



        public string[] maitanenceButtonImage = new string[]
        {
            @"resources\\menubutton1.png",
            @"resources\\menubutton3.png",
            @"resources\\menubutton2.png",
            @"resources\\menubutton3.png"
        };

        public string[] closingProcessLabel = new string[] { "手動での締め処理", "日報の処理", "領収書の処理", "ログ表示" };
        public string[][] closingProcessButton = new string[][]
        {
            new string[] {  "手動締め処理開始", "締め処理解除" },
            new string[] { "表示", "印刷" },
            new string[] { "表示", "印刷" },
            new string[] { "表示", "" }
        };
        public string[] closingProcessButtonImage = new string[]
        {
            @"resources\\menubutton1.png",
            @"resources\\menubutton2.png",
            @"resources\\menubutton3.png",
            @"resources\\menubutton1.png"
       };

        public string leftSubPanel = @"resources\\start_screen.png";
        public string warning_1 = @"resources\\warning_1.png";
        public string warning_2 = @"resources\\warning_2.png";
        public string exceed_warning_1 = @"resources\\exceed_warning_1.png";
        public string exceed_warning_2 = @"resources\\exceed_warning_2.png";
        public string addButton = @"resources\\addbutton.png";
        public string updateButton = @"resources\\updatebutton.png";
        public string settingButton = @"resources\\settingbutton.png";
        public string deleteButton = @"resources\\deletebutton.png";
        public string depositeDisableButton = @"resources\\depositeDisable.png";
        public string depositeEnableButton = @"resources\\depositeEnable.png";
        public string backCategoryButton = @"resources\\backCategoryButton.png";
        public string categoryButton = @"resources\\categoryBackground.png";
        public string categoryActiveButton = @"resources\\categoryBackgroundActive.png";
        public string roundBlueButton = @"resources\\menubutton2.png";
        public string roundPurpleButton = @"resources\\menubutton3.png";
        public string roundGreenButton = @"resources\\menubutton1.png";
        public string dropdownArrowUpIcon = @"resources\\arrow_up.png";
        public string dropdownArrowDownIcon = @"resources\\arrow_down.png";
        public string backStaticButton = @"resources\\backbutton.png";
        public string powerButton = @"resources\\power_button.png";
        public string soldoutBadge = @"resources\\soldout.png";
        public string timeoutBadge = @"resources\\timeout.png";
        public string preparingBadge = @"resources\\preparing.png";
        public string dropdownarrowImage = @"resources\\dropdownarrow.png";
        public string dropuparrowImage = @"resources\\dropuparrow.png";
        //public string ticketingImage = @"resources\\ticketing.png";
        public string ticketingImage = @"resources\\ticketRunDialog.png";
        public string processBackImage = @"resources\\processbackground.png";
        //   public string dropdownArrowDownIcon = @"D:\\ovan\\Ovan_P1\\images\\arrow_down_icon.png";
        public string keyboardButtonImage = @"resources/keyboard.png";
        public string numberButtonImage = @"resources/numberbutton.png";
        public string prevkeyButtonImage = @"resources/prevkey.png";
        public string nextkeyButtonImage = @"resources/nextkey.png";
        public string clearkeyButtonImage = @"resources/clearkey.png";
        public string soldoutButtonImage1 = @"resources/soldoutbutton.png";
        public string soldoutButtonImage2 = @"resources/soldoutbutton_2.png";
        public string menureadingButtonImage = @"resources/menureadingbutton.png";
        public string disableButtonImage = @"resources/disablebutton.png";
        public string cancelButton = @"resources\\cancelbutton.png";
        public string roundedFormImage = @"resources\\roundedpanel.png";
        public string dialogFormImage = @"resources\\dialogpanel.png";
        public string errordialogImage = @"resources\\errordialogpanel.png";
        public string errordialogImage_1 = @"resources\\errordialogpanel_1.png";
        public string rectBlueButton = @"resources\\rectblue.png";
        public string rectRedButton = @"resources\\rectred.png";
        public string rectOrangeButton = @"resources\\rectorange.png";
        public string rectGreenButton = @"resources\\rectgreen.png";
        public string rectLightBlueButton = @"resources\\rectlightblue.png";
        public string orderTitleLabel = @"resources\\ordertitle.png";
        public string orderButton = @"resources\\orderbutton.png";
        public string orderCancelButton = @"resources\\ordercancelbutton.png";
        public string orderCancelDisableButton = @"resources\\ordercanceldisablebutton.png";
        public string shutdownButton = @"resources\\shutdownbutton.png";
        public string restartButton = @"resources\\restartbutton.png";
        public string resetButton = @"resources\\resetbutton.png";
        //public string receiptButton = @"resources\\receiptbutton.png";
        public string receiptButton = @"resources\\receiptbutton.png";
        public string ticketButton = @"resources\\ticketbutton.png";
        //public string receiptDisableButton = @"resources\\receiptdisablebutton.png";
        public string receiptDisableButton = @"resources\\receiptdisablebutton.png";
        public string ticketDisableButton = @"resources\\ticketdisablebutton.png";
        //public string greenGradient = @"resources\\greengradientbg.png";
        //public string blueGradient = @"resources\\bluegradientbg.png";
        public string nextIcon = @"resources\\next_icon.png";
        public string prevIcon = @"resources\\prev_icon.png";

        public string upBtn = @"resources\\upBtn.png";
        public string downBtn = @"resources\\downBtn.png";

        public Color[][] pattern_Clr = new Color[][]
        {
            new Color[] { Color.FromArgb(255, 255, 192, 0), Color.FromArgb(255, 255, 153, 204), Color.FromArgb(255, 50, 204, 50),Color.FromArgb(255, 0, 176, 240),Color.FromArgb(255, 255, 255, 204),Color.FromArgb(255, 255, 204, 255),Color.FromArgb(255, 204, 255, 153),Color.FromArgb(255, 204, 255, 255)},
            new Color[] { Color.FromArgb(255, 255, 153, 51), Color.FromArgb(255, 255, 102, 153), Color.FromArgb(255, 0, 153,0),Color.FromArgb(255, 0, 0, 255),Color.FromArgb(255, 255, 192, 0),Color.FromArgb(255, 255, 153, 204),Color.FromArgb(255, 51, 204, 51),Color.FromArgb(255, 0, 176, 240)},
            new Color[] { Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 0, 176, 240), Color.FromArgb(255, 0, 0, 255),Color.FromArgb(255, 0, 204, 255),Color.FromArgb(255, 147, 219, 255),Color.FromArgb(255, 204, 255, 255),Color.FromArgb(255, 147, 219, 255),Color.FromArgb(255, 204, 255, 255)},
            new Color[] { Color.FromArgb(255, 255, 102, 0), Color.FromArgb(255, 251, 151, 0), Color.FromArgb(255, 255, 102, 0),Color.FromArgb(255, 251, 151, 0),Color.FromArgb(255, 253, 232, 141),Color.FromArgb(255, 255, 255, 204),Color.FromArgb(255, 253, 232, 141),Color.FromArgb(255, 255, 255, 204)},
            new Color[] { Color.FromArgb(255, 0, 176, 240), Color.FromArgb(255, 253, 241, 0), Color.FromArgb(255, 0, 204, 255),Color.FromArgb(255, 253, 241, 0),Color.FromArgb(255, 204, 255, 255),Color.FromArgb(255, 255, 255, 204),Color.FromArgb(255, 204, 255, 255),Color.FromArgb(255, 255, 255, 204)},
            new Color[] { Color.FromArgb(255, 51, 204, 51), Color.FromArgb(255, 253, 241, 0), Color.FromArgb(255, 50, 204, 50),Color.FromArgb(255, 253, 241, 0),Color.FromArgb(255, 204, 255, 153),Color.FromArgb(255, 255, 255, 204),Color.FromArgb(255, 204, 255, 153),Color.FromArgb(255, 255, 255, 204)},
            new Color[] { Color.FromArgb(255, 192, 0, 0), Color.FromArgb(255, 120, 147, 60), Color.FromArgb(255, 228, 108, 10),Color.FromArgb(255, 55, 96, 146),Color.FromArgb(255, 242, 220, 220),Color.FromArgb(255, 215, 228, 190),Color.FromArgb(255, 252, 213, 181),Color.FromArgb(255, 142, 180, 227)}
        };


        public DateTime SumDayTimeStart(string storeEndTime)
        {
            DateTime sumDayTime = DateTime.Now;
            if (String.Compare("00:00", storeEndTime) == 0)
            {
                sumDayTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), 00, 00, 00);
            }
            else
            {
                if (int.Parse(DateTime.Now.ToString("HH")) < int.Parse(storeEndTime.Split(':')[0]))
                {
                    sumDayTime = new DateTime(int.Parse(DateTime.Now.AddDays(-1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(-1).ToString("MM")), int.Parse(DateTime.Now.AddDays(-1).ToString("dd")), int.Parse(storeEndTime.Split(':')[0]), int.Parse(storeEndTime.Split(':')[1]), 00);

                }
                else
                {
                    sumDayTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(storeEndTime.Split(':')[0]), int.Parse(storeEndTime.Split(':')[1]), 00);
                }
            }

            return sumDayTime;

        }

        public DateTime SumDayTimeEnd(string storeEndTime)
        {
            DateTime sumDayTime = DateTime.Now;
            sumDayTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(storeEndTime.Split(':')[0]), int.Parse(storeEndTime.Split(':')[1]), 00);
            if (storeEndTime.Split(':')[0] == "00")
            {
                sumDayTime = sumDayTime.AddDays(1).AddSeconds(-1);
            }
            else
            {
                if (int.Parse(DateTime.Now.ToString("HH")) < int.Parse(storeEndTime.Split(':')[0]))
                {

                    sumDayTime = sumDayTime.AddSeconds(-1);
                }
                else
                {
                    sumDayTime = sumDayTime.AddDays(1).AddSeconds(-1);
                }
            }

            return sumDayTime;
        }

        public DateTime CurrentDateTimeFromTime(string time)
        {
            DateTime sumDayTime = DateTime.Now;
            if (int.Parse(time.Split(':')[0]) == 24)
            {
                sumDayTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), 00, int.Parse(time.Split(':')[1]), 00);
            }
            else
            {
                sumDayTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(time.Split(':')[0]), int.Parse(time.Split(':')[1]), 00);
            }
            //sumDayTime = sumDayTime.AddSeconds(-1);

            return sumDayTime;
        }

        public string SumDate(string startTime, string storeEndTime)
        {
            DateTime now = DateTime.Now;
            string sumDate = now.ToString("yyyy-MM-dd");
            if (String.Compare(startTime, storeEndTime) >= 0)
            {
                sumDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            return sumDate;
        }

        //public string sumDate(string storeEndTime)
        //{
        //    DateTime now = DateTime.Now;
        //    string sumDate = now.ToString("yyyy-MM-dd");
        //    if (String.Compare("00:00", storeEndTime) <= 0 && String.Compare("06:00", storeEndTime) >= 0)
        //    {
        //        if (int.Parse(DateTime.Now.ToString("HH")) < int.Parse(storeEndTime.Split(':')[0]))
        //        {
        //            sumDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        //        }
        //        else
        //        {
        //            sumDate = DateTime.Now.ToString("yyyy-MM-dd");
        //        }
        //    }
        //    else
        //    {
        //        if (int.Parse(DateTime.Now.ToString("HH")) < int.Parse(storeEndTime.Split(':')[0]))
        //        {
        //            sumDate = DateTime.Now.ToString("yyyy-MM-dd");
        //        }
        //        else
        //        {
        //            sumDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        //        }
        //    }

        //    return sumDate;
        //}

        public DateTime OpenDateTime(string openTime, string storeEndTime)
        {
            DateTime openDateTime = DateTime.Now;
            if (int.Parse(openTime.Split(':')[0]) >= int.Parse(storeEndTime.Split(':')[0]))
            {
                openDateTime = new DateTime(SumDayTimeStart(storeEndTime).Year, SumDayTimeStart(storeEndTime).Month, SumDayTimeStart(storeEndTime).Day, int.Parse(openTime.Split(':')[0]), int.Parse(openTime.Split(':')[1]), 00);
            }
            else
            {
                openDateTime = new DateTime(SumDayTimeEnd(storeEndTime).Year, SumDayTimeEnd(storeEndTime).Month, SumDayTimeEnd(storeEndTime).Day, int.Parse(openTime.Split(':')[0]), int.Parse(openTime.Split(':')[1]), 00);
            }
            return openDateTime;
        }

        public DateTime GetStartTime(string startTime, string endTime)
        {
            DateTime realStartTime = DateTime.Now;
            string currentTime = DateTime.Now.ToString("HH:mm");
            if (String.Compare(startTime, endTime) < 0)
            {
                realStartTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 00);
            }
            else
            {
                realStartTime = new DateTime(int.Parse(DateTime.Now.AddDays(-1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(-1).ToString("MM")), int.Parse(DateTime.Now.AddDays(-1).ToString("dd")), int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 00);
            }
            return realStartTime;
        }

        public DateTime GetEndTime(string startTime, string endTime)
        {
            DateTime realEndTime = DateTime.Now;
            string currentTime = DateTime.Now.ToString("HH:mm");
            string currentHour = DateTime.Now.ToString("HH");
            string startHour = startTime.Split(':')[0];
            string endHour = endTime.Split(':')[0];
            if (startHour == "00" || endHour == "00")
            {
                if (String.Compare(startHour, "00") == 0 && String.Compare(endHour, "00") != 0)
                {
                    if (String.Compare(currentHour, endHour) < 0)
                    {
                        realEndTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                    }
                    else
                    {
                        realEndTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                    }
                }
                else
                {
                    realEndTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                }
            }
            else
            {
                if (String.Compare(startHour, endHour) > 0)
                {
                    if (String.Compare(currentHour, startHour) >= 0)
                    {
                        realEndTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);

                    }
                    else
                    {
                        if (String.Compare(currentHour, endHour) < 0)
                        {
                            realEndTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                        }
                        else
                        {
                            realEndTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                        }
                    }
                }
                else
                {
                    if (String.Compare(currentHour, endHour) >= 0)
                    {
                        realEndTime = new DateTime(int.Parse(DateTime.Now.AddDays(1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(1).ToString("MM")), int.Parse(DateTime.Now.AddDays(1).ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                    }
                    else
                    {
                        realEndTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 00);
                    }
                }
            }
            return realEndTime;
        }
        public bool SaleAvailable(string startTime, string endTime)
        {
            bool saleAvailableFlag = false;
            string currentTime = DateTime.Now.ToString("HH:mm");

            if (String.Compare(startTime, endTime) <= 0)
            {
                if (String.Compare(startTime, currentTime) <= 0 && String.Compare(currentTime, endTime) < 0)
                {
                    saleAvailableFlag = true;
                }
            }
            else
            {
                if (String.Compare(startTime, currentTime) <= 0 && String.Compare(currentTime, "23:59") < 0)
                {
                    saleAvailableFlag = true;
                }
                else if (String.Compare("00:00", currentTime) <= 0 && String.Compare(currentTime, endTime) < 0)
                {
                    saleAvailableFlag = true;
                }
            }

            return saleAvailableFlag;
        }

        public string StartTime(string runningTime, string storeEndTime)
        {
            string startTime = "00:00-00:00";
            int storeEndHour = Convert.ToInt32(storeEndTime.Split(':')[0]);
            string[] timeArr = runningTime.Split('/');
            List<string> startTimeArr = new List<string>();
            List<int> startHourArr = new List<int>();
            foreach (string timeElem in timeArr)
            {
                string startTimeTemp = timeElem.Split('-')[0];
                int startHourTemp = Convert.ToInt32(timeElem.Split('-')[0].Split(':')[0]);
                startTimeArr.Add(startTimeTemp);
                startHourArr.Add(startHourTemp);
            }
            int rstTemp = -1;
            string rstTimeTemp = "00:00-00:00";
            int k = 0;
            foreach (int hourElem in startHourArr)
            {
                if (storeEndHour <= hourElem)
                {
                    if (rstTemp == -1)
                    {
                        rstTemp = hourElem;
                        rstTimeTemp = timeArr[k];
                    }
                    else if (rstTemp >= hourElem)
                    {
                        rstTemp = hourElem;
                        rstTimeTemp = timeArr[k];
                    }
                }
                k++;
            }
            int m = 0;
            if (rstTemp == -1)
            {
                foreach (int hourElem in startHourArr)
                {
                    if (storeEndHour >= hourElem)
                    {
                        if (rstTemp == -1)
                        {
                            rstTemp = hourElem;
                            rstTimeTemp = timeArr[m];
                        }
                        else
                        {
                            if (rstTemp >= hourElem)
                            {
                                rstTemp = hourElem;
                                rstTimeTemp = timeArr[m];
                            }
                        }
                    }
                    m++;
                }
            }
            startTime = rstTimeTemp;
            return startTime;
        }

        public string EndTime(string runningTime, string storeEndTime)
        {
            string endTime = "00:00-00:00";
            int storeEndHour = Convert.ToInt32(storeEndTime.Split(':')[0]);
            if (storeEndHour == 0)
            {
                storeEndHour = 24;
            }
            string[] timeArr = runningTime.Split('/');
            List<string> endTimeArr = new List<string>();
            List<int> endHourArr = new List<int>();
            foreach (string timeElem in timeArr)
            {
                string endTimeTemp = timeElem.Split('-')[1];
                int endHourTemp = Convert.ToInt32(timeElem.Split('-')[1].Split(':')[0]);
                endTimeArr.Add(endTimeTemp);
                endHourArr.Add(endHourTemp);
            }
            int rstTemp = 24;
            string rstTimeTemp = "00:00-00:00";
            int k = 0;
            foreach (int hourElem in endHourArr)
            {
                if (storeEndHour >= hourElem)
                {
                    if (rstTemp == 24)
                    {
                        rstTemp = hourElem;
                        rstTimeTemp = timeArr[k];
                    }
                    else if (rstTemp <= hourElem)
                    {
                        rstTemp = hourElem;
                        rstTimeTemp = timeArr[k];
                    }
                }
                k++;
            }
            int m = 0;
            if (rstTemp == 24)
            {
                foreach (int hourElem in endHourArr)
                {
                    if (storeEndHour <= hourElem)
                    {
                        if (rstTemp == 24)
                        {
                            rstTemp = hourElem;
                            rstTimeTemp = timeArr[m];
                        }
                        else
                        {
                            if (rstTemp <= hourElem)
                            {
                                rstTemp = hourElem;
                                rstTimeTemp = timeArr[m];
                            }
                        }
                    }
                    m++;
                }
            }
            endTime = rstTimeTemp;
            return endTime;
        }

        public string CurrentSaleTime(string runningTime)
        {
            string currentSaleTime = null;
            string currentTime = DateTime.Now.ToString("HH:mm");
            string[] timeTemp = runningTime.Split('/');
            foreach (string timeEle in timeTemp)
            {
                string[] timeEleTemp = timeEle.Split('-');
                if (String.Compare(timeEleTemp[0], timeEleTemp[1]) <= 0)
                {
                    if (String.Compare(timeEleTemp[0], currentTime) <= 0 && String.Compare(timeEleTemp[1], currentTime) >= 0)
                    {
                        currentSaleTime = timeEle;
                    }
                }
                else
                {
                    if (String.Compare(timeEleTemp[0], currentTime) <= 0 || String.Compare(timeEleTemp[1], currentTime) >= 0)
                    {
                        currentSaleTime = timeEle;
                    }
                }
            }

            return currentSaleTime;
        }

        public string getCurrentTime()
        {
            string retTime = "";
            DateTime time = DateTime.Now;
            retTime = time.ToString("yyyy-MM-dd h:mm:ss tt");
            return retTime;
        }

        public void SaveLogData(string str1 = "", string str2 = "")
        {
            //string logPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            logPath += "\\STV01\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string fileName = logPath + "log_process_data.txt";
            string strTime = getCurrentTime();

            try
            {
                if (!File.Exists(fileName))
                {
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(strTime + ": " + str1 + "===>" + str2);
                    }
                }

                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine(strTime + ": " + str1 + "===>" + str2);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("log data wirte error===> " + Ex.ToString());
            }
        }


    }
}
