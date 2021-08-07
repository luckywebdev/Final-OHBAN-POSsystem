using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace STV01
{
    class DBClass
    {
        Constant constants = new Constant();
        public string storeEndTime = "00:00";
        public string openTime = "00:00";
        bool processState = false;
        public bool processStartState = false;
        public bool dbState = false;
        int messageCounter = 0;

        public DateTime sumDayTime1 = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), 00, 00, 00);
        public DateTime sumDayTime2 = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), 23, 59, 59);
        public DateTime openDayTime = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")), 00, 00, 00);
        public string startTime = "00:00";
        public string endTime = "00:00";
        public string sumDate = DateTime.Now.ToString("yyyy-MM-dd");
        public string runningTime = null;

        public DBClass()
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            DateTime now = DateTime.Now;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            try
            {
                string storeEndqurey = "SELECT * FROM " + constants.tbNames[6]; //store end time checking
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = storeEndqurey;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        if (week == "Sat" || week == "土")
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[1];
                            runningTime = sqlite_datareader.GetString(4);
                            //startTime = (sqlite_datareader.GetString(4).Split('/')[0].Split('-')[0]);
                        }
                        else if (week == "Sun" || week == "日")
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[2];
                            runningTime = sqlite_datareader.GetString(5);
                            //startTime = (sqlite_datareader.GetString(5).Split('/')[0].Split('-')[0]);
                        }
                        else
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[0];
                            runningTime = sqlite_datareader.GetString(3);
                            //startTime = (sqlite_datareader.GetString(3).Split('/')[0].Split('-')[0]);
                        }

                    }
                }
                sumDayTime1 = constants.SumDayTimeStart(storeEndTime);
                sumDayTime2 = constants.SumDayTimeEnd(storeEndTime);
                string startTimeStr = constants.StartTime(runningTime, storeEndTime);
                startTime = startTimeStr.Split('-')[0];
                string endTimeStr = constants.EndTime(runningTime, storeEndTime);
                endTime = endTimeStr.Split('-')[1];
                openDayTime = constants.OpenDateTime(startTime, storeEndTime);
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch(Exception e)
            {
                Console.WriteLine("db_error: " + e);

            }

        }

        public void DBChecking()
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            DateTime now = DateTime.Now;

            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            try
            {
                string storeEndqurey = "SELECT * FROM " + constants.tbNames[6];
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = storeEndqurey;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        dbState = true;
                    }
                }

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch
            {
                if (messageCounter == 0)
                {
                    Console.WriteLine("db_Thread2");
                    dbState = false;
                    messageCounter++;
                }
                sqlite_cmd = null;
                sqlite_datareader = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
                return;
            }

        }

        public void ClosingProcessWork()
        {
            DBChecking();
            if (dbState)
            {
                bool processState = false;
                processState = ProcessState();
                if (processState)
                {
                    this.ClosingProcessRun();
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine("==========> closing process done");
                }
                else
                {
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine("=====new=====> closing process failed");
                }
            }
            else
            {
                Thread.Sleep(3000);
                if (messageCounter == 0)
                {
                    Console.WriteLine("messageDialog");
                    messageCounter += 1;
                }
            }
        }

        public bool ProcessDateState()
        {
            DateTime now = DateTime.Now;
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");
            string runningTime = null;
            try
            {
                string storeEndqurey = "SELECT * FROM " + constants.tbNames[6]; //store end time checking
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = storeEndqurey;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        if (week == "Sat" || week == "土")
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[1];
                            runningTime = sqlite_datareader.GetString(4);
                            //startTime = (sqlite_datareader.GetString(4).Split('/')[0].Split('-')[0]);
                        }
                        else if (week == "Sun" || week == "日")
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[2];
                            runningTime = sqlite_datareader.GetString(5);
                            //startTime = (sqlite_datareader.GetString(5).Split('/')[0].Split('-')[0]);
                        }
                        else
                        {
                            storeEndTime = (sqlite_datareader.GetString(6)).Split('/')[0];
                            runningTime = sqlite_datareader.GetString(3);
                            //startTime = (sqlite_datareader.GetString(3).Split('/')[0].Split('-')[0]);
                        }

                    }
                }
                string[] openTimeArr = runningTime.Split('/');
                foreach (string opentTimeItem in openTimeArr)
                {
                    string[] openTimeSubArr = opentTimeItem.Split('-');
                    bool saleAvailableFlag = constants.SaleAvailable(openTimeSubArr[0], openTimeSubArr[1]);

                    if (saleAvailableFlag)
                    {

                        processStartState = true;
                        break;
                    }
                }
                sumDayTime1 = constants.SumDayTimeStart(storeEndTime);
                sumDayTime2 = constants.SumDayTimeEnd(storeEndTime);
                string startTimeStr = constants.StartTime(runningTime, storeEndTime);
                startTime = startTimeStr.Split('-')[0];
                string endTimeStr = constants.EndTime(runningTime, storeEndTime);
                endTime = endTimeStr.Split('-')[1];
                openDayTime = constants.OpenDateTime(startTime, storeEndTime);

                sumDate = constants.SumDate(startTime, storeEndTime);

                sqlite_cmd.Dispose();
                sqlite_datareader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                processStartState = false;
            }

            sqlite_cmd = null;
            sqlite_datareader = null;
            sqlite_conn.Close();
            sqlite_conn = null;
            return processStartState;

        }

        public bool ProcessState()
        {
            DateTime closingTime = new DateTime(int.Parse(DateTime.Now.AddDays(-1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(-1).ToString("MM")), int.Parse(DateTime.Now.AddDays(-1).ToString("dd")), 23, 59, 59);

            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;

            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            int processRowNum = 0;
            try
            {
                string processqurey = "SELECT * FROM " + constants.tbNames[3] + " WHERE saleDate<=@saleDate AND sumFlag='false' AND (sumDate='' OR sumDate isNULL)"; //processing result checking
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = processqurey;
                sqlite_cmd.Parameters.AddWithValue("@saleDate", closingTime);
                processRowNum = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                sqlite_cmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DaySaleTb Checking Error" + ex);
            }

            if (processRowNum > 0)
            {
                processState = true;
            }
            else
            {
                processState = false;
            }

            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
            return processState;

        }

        private void ClosingProcessRun()
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            try
            {
                sumDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                DateTime closingTime = new DateTime(int.Parse(DateTime.Now.AddDays(-1).ToString("yyyy")), int.Parse(DateTime.Now.AddDays(-1).ToString("MM")), int.Parse(DateTime.Now.AddDays(-1).ToString("dd")), 23, 59, 59);

                SQLiteCommand sqlite_cmd0;
                string sumQuery0 = "UPDATE " + constants.tbNames[8] + " SET sumDate=@sumDate WHERE (sumDate ISNULL OR sumDate='') AND ReceiptDate<=@saleDate ";
                sqlite_cmd0 = sqlite_conn.CreateCommand();
                sqlite_cmd0.CommandTimeout = 5;
                sqlite_cmd0.CommandText = sumQuery0;
                sqlite_cmd0.Parameters.AddWithValue("@sumDate", sumDate);
                sqlite_cmd0.Parameters.AddWithValue("@saleDate", closingTime);
                sqlite_cmd0.ExecuteNonQuery();
                sqlite_cmd0 = null;

                string saleTBquery = "SELECT prdName, prdPrice, sum(prdAmount), prdRealID FROM " + constants.tbNames[3] + " WHERE saleDate<=@saleDate AND sumFlag='false' AND (sumDate='' OR sumDate isNULL) GROUP BY prdRealID";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandTimeout = 5;
                sqlite_cmd.CommandText = saleTBquery;
                sqlite_cmd.Parameters.AddWithValue("@saleDate", closingTime);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int j = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(0);
                        int prdPrice = sqlite_datareader.GetInt32(1);
                        int prdAmount = sqlite_datareader.GetInt32(2);
                        int prdTotalPrice = prdPrice * prdAmount;
                        int prdID = sqlite_datareader.GetInt32(3);
                        SQLiteCommand sqlite_cmds;
                        string sumQuery = "INSERT INTO " + constants.tbNames[7] + " (prdID, prdName, prdPrice, prdAmount, prdTotalPrice, sumDate) VALUES (@prdID, @prdName, @prdPrice, @prdAmount, @prdTotalPrice, @sumDate)";
                        sqlite_cmds = sqlite_conn.CreateCommand();
                        sqlite_cmds.CommandTimeout = 5;

                        sqlite_cmds.CommandText = sumQuery;
                        sqlite_cmds.Parameters.AddWithValue("@prdID", prdID);
                        sqlite_cmds.Parameters.AddWithValue("@prdName", prdName);
                        sqlite_cmds.Parameters.AddWithValue("@prdPrice", prdPrice);
                        sqlite_cmds.Parameters.AddWithValue("@prdAmount", prdAmount);
                        sqlite_cmds.Parameters.AddWithValue("@prdTotalPrice", prdTotalPrice);
                        sqlite_cmds.Parameters.AddWithValue("@sumDate", sumDate);
                        sqlite_cmds.ExecuteNonQuery();
                        sqlite_cmds.Dispose();
                        sqlite_cmds = null;

                        SQLiteCommand sqlite_cmdss;
                        string sumQuerys = "UPDATE " + constants.tbNames[3] + " SET sumFlag='true', sumDate=@sumDate WHERE saleDate<=@saleDate AND sumFlag='false' AND (sumDate='' OR sumDate isNULL) and prdRealID=@realPrdID";
                        sqlite_cmdss = sqlite_conn.CreateCommand();
                        sqlite_cmdss.CommandTimeout = 5;

                        sqlite_cmdss.CommandText = sumQuerys;
                        sqlite_cmdss.Parameters.AddWithValue("@sumDate", sumDate);
                        sqlite_cmdss.Parameters.AddWithValue("@saleDate", closingTime);
                        sqlite_cmdss.Parameters.AddWithValue("@realPrdID", prdID);
                        sqlite_cmdss.ExecuteNonQuery();
                        sqlite_cmdss.Dispose();
                        sqlite_cmdss = null;
                        j++;
                    }

                }

                sqlite_cmd = null;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandTimeout = 2;
                string query = "UPDATE " + constants.tbNames[2] + " SET LimitedCnt=0, SoldFlag=0";
                sqlite_cmd.CommandText = query;
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd = null;
                sqlite_cmd = sqlite_conn.CreateCommand();
                string query_1 = "UPDATE " + constants.tbNames[0] + " SET SoldFlag=0";
                sqlite_cmd.CommandText = query_1;
                sqlite_cmd.CommandTimeout = 5;
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_conn.Close();
                sqlite_conn = null;
                InsertLog(5, "締め処理開始(自動)", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                sqlite_conn.Close();
                sqlite_conn = null;
                return;
            }

        }

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

        public int[] TotalTicketPrice(string year, string month = null, string day = null)
        {
            int[] rst = { 0, 0 };
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (month == null && day == null)
            {
                startDate = new DateTime(int.Parse(year), 01, 01, 00, 00, 00);
                endDate = new DateTime(int.Parse(year), 12, 31, 23, 59, 59);
            }
            else if(month != null && day == null)
            {
                int days = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
                startDate = new DateTime(int.Parse(year), int.Parse(month), 01, 00, 00, 00);
                endDate = new DateTime(int.Parse(year), int.Parse(month), days, 23, 59, 59);
            }
            else
            {
                startDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), 00, 00, 00);
                endDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), 23, 59, 59);
            }

            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            sqlite_cmd = sqlite_conn.CreateCommand();
            //string yearSumqurey = "SELECT count(newSaleTB.ticketNo) FROM (SELECT ticketNo FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY ticketNo) as newSaleTB";
            //sqlite_cmd.CommandText = yearSumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);

            //int rows = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            //rst[0] = rows;

            string yearSumqurey = "SELECT SUM(prdAmount) as prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2";
            sqlite_cmd.CommandText = yearSumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    rst[0] = sqlite_datareader.GetInt32(0);
                }
            }

            sqlite_cmd = sqlite_conn.CreateCommand();
            string yearSumqurey2 = "SELECT sum(prdPrice*prdAmount) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2";
            sqlite_cmd.CommandText = yearSumqurey2;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    rst[1] = sqlite_datareader.GetInt32(0);
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;

            return rst;
        }

        public int[] TotalTicketPriceRange(string years, string months, string days, string yeare, string monthe, string daye)
        {
            int[] rst = { 0, 0 };
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            startDate = new DateTime(int.Parse(years), int.Parse(months), int.Parse(days), 00, 00, 00);
            endDate = new DateTime(int.Parse(yeare), int.Parse(monthe), int.Parse(daye), 23, 59, 59);

            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            sqlite_cmd = sqlite_conn.CreateCommand();
            //string yearSumqurey = "SELECT count(newSaleTB.ticketNo) FROM (SELECT ticketNo FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2 GROUP BY ticketNo) as newSaleTB";
            //sqlite_cmd.CommandText = yearSumqurey;
            //sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            //sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);

            //int rows = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            //rst[0] = rows;

            string yearSumqurey = "SELECT SUM(prdAmount) as prdAmount FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2";
            sqlite_cmd.CommandText = yearSumqurey;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    rst[0] = sqlite_datareader.GetInt32(0);
                }
            }


            sqlite_cmd = sqlite_conn.CreateCommand();
            string yearSumqurey2 = "SELECT sum(prdPrice*prdAmount) FROM " + constants.tbNames[3] + " WHERE saleDate>=@saleDate1 AND saleDate<=@saleDate2";
            sqlite_cmd.CommandText = yearSumqurey2;
            sqlite_cmd.Parameters.AddWithValue("@saleDate1", startDate);
            sqlite_cmd.Parameters.AddWithValue("@saleDate2", endDate);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    rst[1] = sqlite_datareader.GetInt32(0);
                }
            }

            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;

            return rst;
        }

        public void DBCopy(string new_db_path, string new_db, string[] new_tbs)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string Attachedsql = "ATTACH DATABASE '" + new_db_path + "' AS " + new_db;
            sqlite_cmd.CommandText = Attachedsql;
            sqlite_cmd.ExecuteNonQuery();
            foreach (string new_tb in new_tbs)
            {
                string createSql = "";
                switch (new_tb)
                {
                    case "CategoryTB":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (id INTEGER PRIMARY KEY AUTOINCREMENT, CategoryID INT(2) NOT NULL DEFAULT 0, CategoryName VARCHAR(64) NOT NULL, DayTime VARCHAR(256) NOT NULL DEFAULT '09:00-20-59', SatTime VARCHAR(256) NOT NULL DEFAULT '09:00-20-59', SunTime VARCHAR(256) NOT NULL DEFAULT '09:00-20-59', DisplayPosition INT, LayoutType INTEGER, BackgroundImg TEXT DEFAULT NULL, BackImgUrl TEXT DEFAULT NULL, SoldFlag INT(2) NOT NULL DEFAULT 0)";
                        break;
                    case "ProductTB":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (ProductID INT(2) NOT NULL DEFAULT 0, ProductName VARCHAR(32) NOT NULL, PrintName VARCHAR(32) NOT NULL, DayTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', SatTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', SunTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', ProductPrice INT(8), LimitedCnt INT(2) DEFAULT 0, ImgUrl VARCHAR(256), ValidImgUrl VARCHAR(128), ScreenMsg VARCHAR(64), PrintMsg VARCHAR(64))";
                        break;
                    case "CategoryDetailTB":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (id INTEGER PRIMARY KEY AUTOINCREMENT, CategoryID INT(10) NOT NULL DEFAULT 0, ProductID INT(10) NOT NULL DEFAULT 0, ProductName VARCHAR(32) NOT NULL, PrintName VARCHAR(32) NOT NULL, DayTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', SatTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', SunTime VARCHAR(128) NOT NULL DEFAULT '09:00-20-59', ProductPrice INT(8), LimitedCnt INT(2) DEFAULT 0, ImgUrl VARCHAR(256), ValidImgUrl VARCHAR(128), ScreenMsg VARCHAR(64), PrintMsg VARCHAR(64), CardNumber INT(2) NOT NULL DEFAULT -1, BadgePath VARCHAR(256), ValidBadgePath VARCHAR(64), SoldFlag INT(2) NOT NULL DEFAULT 0, ButtonBKColor VARCHAR(16) NOT NULL DEFAULT '', TextColor VARCHAR(16) NOT NULL DEFAULT '', ScreenBKColor VARCHAR(16) NOT NULL DEFAULT '', ScreenMsgColor VARCHAR(16) NOT NULL DEFAULT '', TopBarBKColor VARCHAR(16) NOT NULL DEFAULT '', TopTextColor VARCHAR(16) NOT NULL DEFAULT '', BottomBarBKColor VARCHAR(16) NOT NULL DEFAULT '', BottomTextColor VARCHAR(16) NOT NULL DEFAULT '', Mark INT NOT NULL DEFAULT 0, MarkRC VARCHAR(32) NOT NULL DEFAULT '0:0:0:0', MsgRC VARCHAR(32) NOT NULL DEFAULT '0:0:0:0', NameRC VARCHAR(32) NOT NULL DEFAULT '0:0:0:0', PriceRC VARCHAR(32) NOT NULL DEFAULT '0:0:0:0')";
                        break;
                    case "TableSetTicket":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (PurchaseType INT(2) NOT NULL DEFAULT 1, ReturnTime INT(4) NOT NULL DEFAULT 30, MultiPurchase INT(2) NOT NULL DEFAULT 1, PurchaseAmount INT(4) NOT NULL DEFAULT 10, TelPrefix INT(2) NOT NULL DEFAULT 1, SerialNo INT(2) NOT NULL DEFAULT 1, StartSerialNo INT(4) NOT NULL DEFAULT 0, NoAfterTight INT(4) NOT NULL DEFAULT 1, FontSize INT(2) NOT NULL DEFAULT 1, ValidDate INT(2) NOT NULL DEFAULT 1, TicketMsg1 VARCHAR(16) NOT NULL, TicketMsg2 VARCHAR(16) NOT NULL)";
                        break;
                    case "TableSetReceipt":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (ReceiptValid VARCHAR(8) NOT NULL, TicketTime VARCHAR(4) NOT NULL, StoreName VARCHAR(64) NOT NULL, Address VARCHAR(64) NOT NULL, PhoneNumber VARCHAR(16) NOT NULL, Other1 VARCHAR(64) NOT NULL, Other2 VARCHAR(64) NOT NULL)";
                        break;
                    case "TableSetStore":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (StoreName VARCHAR(32) NOT NULL, Address VARCHAR(32) NOT NULL, PhoneNumber VARCHAR(16) NOT NULL, WeekTime VARCHAR(128) NOT NULL, SaturdayTime VARCHAR(128) NOT NULL, SundayTime VARCHAR(128) NOT NULL, EndTime VARCHAR(32) NOT NULL)";
                        break;
                    case "TableGroupName":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (GroupID INT(2) NOT NULL DEFAULT 0, GroupName VARCHAR(32) NOT NULL)";
                        break;
                    case "TableGroupDetail":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (GroupID INT(2) NOT NULL DEFAULT 0, ProductName VARCHAR(32) NOT NULL, ProductPrice INT(8) NOT NULL DEFAULT 0)";
                        break;
                    case "GeneralTB":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (ButtonBKColor VARCHAR(16) NOT NULL, TextColor VARCHAR(16) NOT NULL, ScreenBKColor VARCHAR(16) NOT NULL, ScreenMsgColor VARCHAR(16) NOT NULL, TopBarBKColor VARCHAR(16) NOT NULL, TopTextColor VARCHAR(16) NOT NULL, BottomBarBKColor VARCHAR(16) NOT NULL, BottomTextColor VARCHAR(16) NOT NULL, MenuMsg1 VARCHAR(64) NOT NULL, MenuMsg2 VARCHAR(64) NOT NULL)";
                        break;
                    case "TableSetAudio":
                        createSql = "CREATE TABLE IF NOT EXISTS " + new_tb + " (WaitingAudio VARCHAR(32) NOT NULL, ButtonTouch VARCHAR(32) NOT NULL, CashInsert VARCHAR(16), ValidItemTouch VARCHAR(16), ReturnTouch VARCHAR(16), RefundCompleted VARCHAR(16), DeleteTouch VARCHAR(16), IncreaseTouch VARCHAR(16), DecreaseTouch VARCHAR(16), TicketDisable VARCHAR(16), TicketValid VARCHAR(16), TicketIssue VARCHAR(16), ErrorOccur VARCHAR(16))";
                        break;
                    case "SaleTB":
                        break;
                    case "DaySaleTB":
                        break;
                    case "ReceiptTB":
                        break;
                    case "CancelOrderTB":
                        break;

                }
                if (createSql != "")
                {
                    sqlite_cmd.CommandText = createSql;
                    sqlite_cmd.ExecuteNonQuery();

                    SQLiteConnection sqlite_conn_chk = CreateConnection(new_db);
                    if(sqlite_conn_chk.State == ConnectionState.Closed)
                    {
                        sqlite_conn_chk.Open();
                    }
                    SQLiteCommand sqlite_cmd_chk = sqlite_conn_chk.CreateCommand();
                    string chk_query = "SELECT COUNT(*) AS QtRecords FROM sqlite_master WHERE type='table' AND name=@tableName";
                    sqlite_cmd_chk.CommandText = chk_query;
                    sqlite_cmd_chk.Parameters.AddWithValue("@tableName", new_tb);
                    int rowCount = Convert.ToInt32(sqlite_cmd_chk.ExecuteScalar());
                    sqlite_cmd_chk.Dispose();
                    sqlite_cmd_chk = null;
                    sqlite_conn_chk.Close();
                    sqlite_conn_chk = null;

                    if(rowCount > 0)
                    {
                        this.DeleteData(sqlite_conn, new_tb);
                        string Createsql1 = "INSERT INTO " + new_tb + " SELECT * FROM " + new_db + "." + new_tb + "";
                        sqlite_cmd.CommandText = Createsql1;
                        sqlite_cmd.ExecuteNonQuery();
                    }

                }

            }
            sqlite_cmd.CommandText = "DETACH DATABASE '" + new_db + "'";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;

            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void DBBackup(string new_db_path, string new_db, string[] new_tbs, string work_type)
        {
            CreateSaleTB();
            CreateDaySaleTB();
            CreateReceiptTB();
            CreateCancelOrderTB();
            CreateLogTB();
            CreateProductOptionTB();
            CreateProductOptionValueTB();

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string Attachedsql = "ATTACH DATABASE '" + new_db_path + "' AS " + new_db;
            sqlite_cmd.CommandText = Attachedsql;
            sqlite_cmd.ExecuteNonQuery();

            string Createsql = "";
            // create SaleTB
            Createsql = "CREATE TABLE IF NOT EXISTS " + new_db + "." + constants.tbNames[3] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, prdID INTEGER NOT NULL, prdRealID INT(10) NOT NULL DEFAULT 0, prdName VARCHAR(200) NOT NULL, prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, ticketNo INTEGER NOT NULL DEFAULT 0, saleDate DATETIME, sumFlag BOOLEAN NOT NULL DEFAULT 'false', sumDate DATETIME, categoryID  INTEGER NOT NULL DEFAULT 0, serialNo INTEGER NOT NULL DEFAULT 1, limitFlag INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            // create DaySaleTB
             Createsql = "CREATE TABLE IF NOT EXISTS " + new_db + "." + constants.tbNames[7] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, prdID INTEGER NOT NULL DEFAULT 0, prdName VARCHAR(128) NOT NULL DEFAULT '', prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, prdTotalPrice INTEGER NOT NULL DEFAULT 0, sumDate VARCHAR(10) NOT NULL DEFAULT '')";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            // create ReceiptTB
            Createsql = "CREATE TABLE IF NOT EXISTS " + new_db + "." + constants.tbNames[8] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, PurchasePoint INTEGER NOT NULL, TotalPrice INTEGER NOT NULL DEFAULT 0, ReceiptDate DATETIME, sumDate DATE, ticketNo INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            // create CancelOrderTB
            Createsql = "CREATE TABLE IF NOT EXISTS " + new_db + "." + constants.tbNames[9] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, saleID INTEGER NOT NULL, prdID INTEGER NOT NULL, prdName VARCHAR(200) NOT NULL, prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, ticketNo INTEGER NOT NULL DEFAULT 0, saleDate DATETIME, sumFlag BOOLEAN NOT NULL DEFAULT 'false', sumDate DATETIME, categoryID INTEGER NOT NULL DEFAULT 0, serialNo INTEGER NOT NULL DEFAULT 1, realPrdID INTEGER DEFAULT 0, cancelDate DATETIME, limitFlag INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            // create LogTB
            Createsql = "CREATE TABLE IF NOT EXISTS " + new_db + "." + constants.tbNames[14] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, logType INTEGER NOT NULL, logTitle TEXT, logContent TEXT, logContent_2 TEXT, logDate DATETIME)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            foreach(string new_tb in new_tbs)
            {
                string Createsql1 = "";
                if(work_type == "backup")
                {
                    this.DeleteData(sqlite_conn, new_tb, new_db);
                    Createsql1 = "INSERT INTO " + new_db + "." + new_tb + " SELECT * FROM " + new_tb + "";
                }
                else
                {
                    this.DeleteData(sqlite_conn, new_tb);
                    Createsql1 = "INSERT INTO " + new_tb + " SELECT * FROM " + new_db + "." + new_tb + "";
                }
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql1;
                sqlite_cmd.ExecuteNonQuery();
            }
            sqlite_cmd.CommandText = "DETACH DATABASE '" + new_db + "'";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;

        }

        public void CreateSaleTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[3] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, prdID INTEGER NOT NULL, prdRealID INT(10) NOT NULL DEFAULT 0, prdName VARCHAR(200) NOT NULL, prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, ticketNo INTEGER NOT NULL DEFAULT 0, saleDate DATETIME, sumFlag BOOLEAN NOT NULL DEFAULT 'false', sumDate DATETIME, categoryID  INTEGER NOT NULL DEFAULT 0, serialNo INTEGER NOT NULL DEFAULT 1, limitFlag INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }
        public void CreateDaySaleTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[7] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, prdID INTEGER NOT NULL DEFAULT 0, prdName VARCHAR(128) NOT NULL DEFAULT '', prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, prdTotalPrice INTEGER NOT NULL DEFAULT 0, sumDate VARCHAR(10) NOT NULL DEFAULT '')";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }
        public void CreateReceiptTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[8] + " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, PurchasePoint INTEGER NOT NULL, TotalPrice INTEGER NOT NULL DEFAULT 0, ReceiptDate DATETIME, sumDate DATE, ticketNo INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void CreateCancelOrderTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[9] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, saleID INTEGER NOT NULL, prdID INTEGER NOT NULL, prdName VARCHAR(200) NOT NULL, prdPrice INTEGER NOT NULL DEFAULT 0, prdAmount INTEGER NOT NULL DEFAULT 0, ticketNo INTEGER NOT NULL DEFAULT 0, saleDate DATETIME, sumFlag BOOLEAN NOT NULL DEFAULT 'false', sumDate DATETIME, categoryID INTEGER NOT NULL DEFAULT 0, serialNo INTEGER NOT NULL DEFAULT 1, realPrdID INTEGER DEFAULT 0, cancelDate DATETIME, limitFlag INTEGER NOT NULL DEFAULT 0)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void CreateLogTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[14] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, logType INTEGER NOT NULL, logTitle TEXT, logContent TEXT, logContent_2 TEXT, logDate DATETIME)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void CreateProductOptionTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[15] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, productID INTEGER NOT NULL, optionTitle TEXT)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void CreateProductOptionValueTB()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS " + constants.tbNames[16] + " (id INTEGER PRIMARY KEY AUTOINCREMENT, productID INTEGER NOT NULL, optionTitle INTEGER, optionValue TEXT)";
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
        }

        public void InsertLog(int logType, string logTitle, string logContent, string logContent_2 = "")
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            try
            {
                DateTime logDate = DateTime.Now;
                SQLiteCommand sqlite_cmd;
                
                string Createsql = "INSERT INTO " + constants.tbNames[14] + " (logType, logTitle, logContent, logContent_2, logDate) VALUES (@logType, @logTitle, @logContent, @logContent_2, @logDate)";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandTimeout = 5;
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.Parameters.AddWithValue("@logType", logType);
                sqlite_cmd.Parameters.AddWithValue("@logTitle", logTitle);
                sqlite_cmd.Parameters.AddWithValue("@logContent", logContent);
                sqlite_cmd.Parameters.AddWithValue("@logContent_2", logContent_2);
                sqlite_cmd.Parameters.AddWithValue("@logDate", logDate);

                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
            }
            catch (Exception e)
            {
                sqlite_conn.Close();
                sqlite_conn = null;
                Console.WriteLine("db_error==" + e);
                return;
            }
            sqlite_conn.Close();
            sqlite_conn = null;
            return;
        }

        public void DeleteData(SQLiteConnection conn, string tb_name, string db_name = null)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "DELETE FROM " + tb_name;
                if (db_name != null)
                {
                    sqlite_cmd.CommandText = "DELETE FROM " + db_name + "." + tb_name;
                }
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("table_delete_error==>" + ex.ToString());
                return;
            }
        }

        public string AudioFile(string audio_type)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path += "\\STV01\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            string audio_file = "";
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT " + audio_type + " FROM " + constants.tbNames[13];
            SQLiteDataReader sqlite_datareader;
            try
            {
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        audio_file = Path.Combine(path, sqlite_datareader.GetString(0));
                    }
                }
                sqlite_datareader.Close();
            }
            catch
            {
                audio_file = "";
            }
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Close();
            sqlite_conn = null;
            return audio_file;
        }
    }
}
