using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class GroupList : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        //private Button buttonGlobal = null;
        private FlowLayoutPanel[] menuFlowLayoutPanelGlobal = new FlowLayoutPanel[4];
        Constant constants = new Constant();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CreateCombobox createCombobox = new CreateCombobox();
        CustomButton customButton = new CustomButton();
        DropDownMenu dropDownMenu = new DropDownMenu();
        MessageDialog messageDialog = new MessageDialog();
        Panel detailPanelGlobal = null;
        Panel tBodyPanelGlobal = null;
        DetailView detailView = new DetailView();
        PaperSize paperSize = new PaperSize("papersize", 500, 800);//set the paper size
        PrintDocument printDocument1 = new PrintDocument();
        PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
        PrintDialog printDialog1 = new PrintDialog();
        int totalNumber = 0;
        int groupRowCount = 0;
        int[] groupIDList = null;
        string[] groupNameList = null;
        int itemperpage = 0;//this is for no of item per page 
        int groupIDGlobal = 0;
        string storeName = "";
        DateTime now = DateTime.Now;
        DBClass dbClass = new DBClass();

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


        public GroupList(Form1 mainForm, Panel mainPanel)
        {
            InitializeComponent();
            dropDownMenu.InitGroupList(this);
            messageDialog.InitGroupList(this);

            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);

            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteCommand sqlite_cmd;
            string week = now.ToString("ddd");
            string currentTime = now.ToString("HH:mm");

            try
            {
                string productQuery = "SELECT count(*) FROM " + constants.tbNames[10] + "";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = productQuery;
                totalNumber = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                string productQuery1 = "SELECT count(*) FROM " + constants.tbNames[11] + "";
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = productQuery1;
                totalNumber += Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            SQLiteDataReader sqlite_datareader;


            string storeEndqurey = "SELECT * FROM " + constants.tbNames[6];
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = storeEndqurey;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    storeName = sqlite_datareader.GetString(1);

                }
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;


            this.GetGroupList();
            if(groupNameList.Length > 0)
            {
                Panel mainPanels = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height, BorderStyle.None, Color.Transparent);
                Label categoryLabel = createLabel.CreateLabelsInPanel(mainPanels, "groupLabel", constants.groupListTitleLabel, 0, 50, mainPanels.Width / 2, 50, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);

                dropDownMenu.CreateDropDown("groupList", mainPanels, groupNameList, mainPanels.Width / 2, 50, 200, 50, 200, 50 * (groupRowCount + 1), 200, 50, Color.Red, Color.White);


                Button printButton = customButton.CreateButtonWithImage(constants.rectLightBlueButton, "groupPrintButton", constants.printButtonLabel, mainPanelGlobal.Width - 200, mainPanels.Height * 3 / 5 + 110, 150, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

                mainPanels.Controls.Add(printButton);
                printButton.Click += new EventHandler(messageDialog.MessageDialogInit);
                //printButton.Click += new EventHandler(this.btnprintpreview_Click);

                Button closeButton = customButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, mainPanelGlobal.Width - 200, mainPanels.Height * 3 / 5 + 190, 150, 50, 0, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);

                mainPanels.Controls.Add(closeButton);
                closeButton.Click += new EventHandler(this.BackShow);

                Panel detailPanel = createPanel.CreateSubPanel(mainPanels, 50, 150, mainPanelGlobal.Width * 5 / 7, mainPanelGlobal.Height - 200, BorderStyle.None, Color.Transparent);
                detailPanelGlobal = detailPanel;

                FlowLayoutPanel tableHeaderInUpPanel = createPanel.CreateFlowLayoutPanel(detailPanel, 0, 0, detailPanel.Width, 50, Color.Transparent, new Padding(0));
                Label tableHeaderLabel1 = createLabel.CreateLabels(tableHeaderInUpPanel, "tableHeaderLabel1", constants.printProductNameField, 0, 0, tableHeaderInUpPanel.Width * 2 / 3, 50, Color.Transparent, Color.Black, 16, true, ContentAlignment.MiddleCenter, new Padding(0), 1, Color.Gray);
                tableHeaderLabel1.Paint += new PaintEventHandler(this.Set_background);
                Label tableHeaderLabel2 = createLabel.CreateLabels(tableHeaderInUpPanel, "tableHeaderLabel2", constants.salePriceField, tableHeaderLabel1.Right, 0, tableHeaderInUpPanel.Width / 3, 50, Color.FromArgb(255, 147, 184, 219), Color.Black, 16, true, ContentAlignment.MiddleCenter, new Padding(0), 1, Color.Gray);
                tableHeaderLabel2.Paint += new PaintEventHandler(this.Set_background);
                Panel tBodyPanel = createPanel.CreateSubPanel(detailPanel, 0, 50, detailPanel.Width, detailPanel.Height - 50, BorderStyle.None, Color.Transparent);
                tBodyPanelGlobal = tBodyPanel;

                //printDocument1.PrintPage += new PrintPageEventHandler(PrintDocument1_PrintPage);
                //printDocument1.EndPrint += new PrintEventHandler(PrintEnd);

                ShowGroupDetail(0);
            }
            else
            {
                mainPanelGlobal.Controls.Clear();
                MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
                frm.TopLevel = false;
                mainPanelGlobal.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                Thread.Sleep(200);
                frm.Show();

            }

        }

        private void GetGroupList()
        {
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string queryCmd0 = "SELECT COUNT(GroupID) FROM " + constants.tbNames[10];
            sqlite_cmd.CommandText = queryCmd0;
            groupRowCount = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            groupNameList = new string[groupRowCount];
            groupIDList = new int[groupRowCount];

            string queryCmd = "SELECT * FROM " + constants.tbNames[10] + " ORDER BY GroupID";
            sqlite_cmd.CommandText = queryCmd;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    groupIDList[k] = sqlite_datareader.GetInt32(0);
                    groupNameList[k] = sqlite_datareader.GetString(1);
                    k++;
                }
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void PrintEnd(object sender, PrintEventArgs e)
        {
            itemperpage = 0;
        }

        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintDocument printDocument = (PrintDocument)sender;
            float currentY = 0;// declare  one variable for height measurement
            RectangleF rect1 = new RectangleF(20, currentY, constants.grouplistPrintPaperWidth, 25);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(constants.groupListPrintTitle, new Font("Seri", constants.fontSizeBig, FontStyle.Bold), Brushes.Black, rect1, format1);//this will print one heading/title in every page of the document
            currentY += 25;

            RectangleF rect2 = new RectangleF(20, currentY, constants.grouplistPrintPaperWidth, 25);
            StringFormat format2 = new StringFormat();
            format2.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(now.ToString("yyyy/MM/dd HH:mm:ss"), new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect2, format2);//this will print one heading/title in every page of the document
            currentY += 25;

            RectangleF rect3 = new RectangleF(5, currentY, constants.grouplistPrintPaperWidth, 25);
            StringFormat format3 = new StringFormat();
            format3.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(storeName, new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rect3, format3);//this will print one heading/title in every page of the document
            currentY += 25;
            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            int k = 0;
            foreach (string groupName in groupNameList)
            {
                RectangleF rect4 = new RectangleF(20, currentY, constants.grouplistPrintPaperWidth / 2, 25);
                StringFormat format4 = new StringFormat();
                format4.Alignment = StringAlignment.Near;
                e.Graphics.DrawString(constants.groupTitleLabel + groupIDList[k].ToString("00") + ": " + groupName, DefaultFont, Brushes.Black, rect4, format4);
                currentY += 25;

                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[11] + " WHERE GroupID=@groupID";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@groupID", groupIDList[k]);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int m = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(1);
                        int prdPrice = sqlite_datareader.GetInt32(2);
                        RectangleF rect5 = new RectangleF(20, currentY, constants.grouplistPrintPaperWidth * 2 / 3, 20);
                        StringFormat format5 = new StringFormat();
                        format5.Alignment = StringAlignment.Near;
                        e.Graphics.DrawString(prdName, DefaultFont, Brushes.Black, rect5, format5);//print each item

                        RectangleF rect6 = new RectangleF(20 + constants.grouplistPrintPaperWidth * 2 / 3, currentY, constants.grouplistPrintPaperWidth / 3 - 20, 20);
                        StringFormat format6 = new StringFormat();
                        format6.Alignment = StringAlignment.Near;
                        e.Graphics.DrawString(prdPrice.ToString() + constants.unit, DefaultFont, Brushes.Black, rect6, format6);

                        currentY += 20;
                        itemperpage += 1; // increment itemperpage by 1
                        e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
                        m++;
                    }
                }
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_datareader.Close();
                sqlite_datareader = null;
                k++;
            }
            sqlite_conn.Dispose();
            sqlite_conn = null;

            currentY += 20;
            RectangleF rects = new RectangleF(5, currentY, constants.grouplistPrintPaperWidth, 40);
            StringFormat formats = new StringFormat();
            formats.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("", new Font("Seri", constants.fontSizeMedium, FontStyle.Bold), Brushes.Black, rects, formats);//this will print one heading/title in every page of the document
            currentY += 35;

        }

        private void PrintDocument1_PrintPageUSB()
        {
            int a;
            byte[] bytes;
            a = PrintCmd(0x1B);
            a = PrintStr("@");                      //Reset Printer

            a = PrintCmd(0x1A);
            a = PrintCmd(0x78);
            a = PrintCmd(0x00);                      //Extended ascii mode 

            a = PrintCmd(0x1B);
            a = PrintCmd(0x4D);
            a = PrintCmd(0x20);                      //Select Japanese font

            a = PrintCmd(0x1B);
            a = PrintStr("3");
            a = PrintCmd(0x28);                      //Set the interval of line

            a = PrintCmd(0x1D);
            a = PrintStr("L");
            a = PrintCmd(0x32);
            a = PrintCmd(0x00);                      //Select the left margin

            a = PrintCmd(0x1D);
            a = PrintStr("W");
            a = PrintCmd(0x5C);
            a = PrintCmd(0x01);                      //Select the printing width

            a = PrintCmd(0x1B);
            a = PrintStr("a");
            a = PrintCmd(0x01);                      //set center align

            bytes = Encoding.GetEncoding(932).GetBytes(constants.groupListPrintTitle);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            bytes = Encoding.GetEncoding(932).GetBytes(now.ToString("yyyy/MM/dd HH:mm:ss"));

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            bytes = Encoding.GetEncoding(932).GetBytes(storeName);

            foreach (byte tempB in bytes)
            {
                a = PrintCmd(tempB);
            }

            a = PrintCmd(0x0A);

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            int k = 0;
            foreach (string groupName in groupNameList)
            {
                a = PrintCmd(0x1B);
                a = PrintStr("a");
                a = PrintCmd(0x00);                      //set left align

                bytes = Encoding.GetEncoding(932).GetBytes(constants.groupTitleLabel + groupIDList[k].ToString("00") + ": " + groupName);

                foreach (byte tempB in bytes)
                {
                    a = PrintCmd(tempB);
                }

                a = PrintCmd(0x0A);

                sqlite_cmd = sqlite_conn.CreateCommand();
                string queryCmd = "SELECT * FROM " + constants.tbNames[11] + " WHERE GroupID=@groupID";
                sqlite_cmd.CommandText = queryCmd;
                sqlite_cmd.Parameters.AddWithValue("@groupID", groupIDList[k]);
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                int m = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        string prdName = sqlite_datareader.GetString(1);
                        int prdPrice = sqlite_datareader.GetInt32(2);

                        a = PrintCmd(0x1B);
                        a = PrintStr("a");
                        a = PrintCmd(0x00);                      //set left align

                        bytes = Encoding.GetEncoding(932).GetBytes(prdName);

                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }

                        bytes = Encoding.GetEncoding(932).GetBytes("/" + prdPrice.ToString() + constants.unit);

                        foreach (byte tempB in bytes)
                        {
                            a = PrintCmd(tempB);
                        }

                        a = PrintCmd(0x0A);

                        m++;
                    }
                }
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_datareader.Close();
                sqlite_datareader = null;
                k++;
            }
            sqlite_conn.Dispose();
            sqlite_conn = null;

            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);
            a = PrintCmd(0x0A);

            a = PrintCmd(0x1B);
            a = PrintStr("i");                                       //Full Cut

        }

        private void ShowGroupDetail(int groupID)
        {

            tBodyPanelGlobal.HorizontalScroll.Maximum = 0;
            tBodyPanelGlobal.AutoScroll = false;
            tBodyPanelGlobal.VerticalScroll.Visible = false;
            tBodyPanelGlobal.AutoScroll = true;

            SQLiteConnection sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();

            string queryCmd = "SELECT * FROM " + constants.tbNames[11] + " WHERE GroupID=@groupID";
            sqlite_cmd.CommandText = queryCmd;
            sqlite_cmd.Parameters.AddWithValue("@groupID", groupIDList[groupID]);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    string prdName = sqlite_datareader.GetString(1);
                    int prdPrice = sqlite_datareader.GetInt32(2);
                    FlowLayoutPanel tableRowPanel = createPanel.CreateFlowLayoutPanel(tBodyPanelGlobal, 0, 50 * k, tBodyPanelGlobal.Width, 50, Color.Transparent, new Padding(0));
                    Label tdLabel1 = createLabel.CreateLabels(tableRowPanel, "tdLabel1_" + k, prdName, 0, 0, tableRowPanel.Width * 2 / 3, 50, Color.White, Color.Black, 16, true, ContentAlignment.MiddleCenter, new Padding(0), 1, Color.Gray);
                    Label tdLabel2 = createLabel.CreateLabels(tableRowPanel, "tdLabel2_" + k, prdPrice.ToString(), tdLabel1.Right, 0, tableRowPanel.Width / 3, 50, Color.White, Color.Black, 16, true, ContentAlignment.MiddleCenter, new Padding(0), 1, Color.Gray);
                    k++;
                }
            }

            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        public void SetVal(string groupID)
        {
            tBodyPanelGlobal.Controls.Clear();
            groupIDGlobal = int.Parse(groupID);
            ShowGroupDetail(int.Parse(groupID));
        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal.Controls.Clear();
            MaintaneceMenu frm = new MaintaneceMenu(mainFormGlobal, mainPanelGlobal);
            frm.TopLevel = false;
            mainPanelGlobal.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            Thread.Sleep(200);
            frm.Show();
        }
        private void Set_background(Object sender, PaintEventArgs e)
        {
            Label lTemp = (Label)sender;
            Graphics graphics = e.Graphics;

            //the rectangle, the same size as our Form
            Rectangle gradient_rectangle = new Rectangle(0, 0, lTemp.Width, lTemp.Height);
            //define gradient's properties
            Brush b = new LinearGradientBrush(gradient_rectangle, Color.FromArgb(255, 164, 206, 235), Color.FromArgb(255, 87, 152, 199), 90f);

            //apply gradient         
            graphics.FillRectangle(b, gradient_rectangle);

            graphics.DrawRectangle(new Pen(Brushes.Gray), gradient_rectangle);

            e.Graphics.DrawString(lTemp.Text, new Font("Seri", 18, FontStyle.Bold), Brushes.White, gradient_rectangle, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

        }

        public void Btnprintpreview_Click()

        {
            ////  printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            ////printPreviewDialog1.Document = printDocument1;
            //printDialog1.Document = printDocument1;

            //((ToolStripButton)((ToolStrip)printPreviewDialog1.Controls[1]).Items[0]).Enabled = true;
            //paperSize = new PaperSize("papersize", constants.grouplistPrintPaperWidth, constants.grouplistPrintPaperHeight);


            //printDocument1.DefaultPageSettings.PaperSize = paperSize;
            //printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 15);
            ////printPreviewDialog1.ShowDialog();
            //printDocument1.Print();
            USBPrint();
        }

        private void USBPrint()
        {
            try
            {
                int a = UsbOpen("HMK-060");
                int status = NewRealRead();
                constants.SaveLogData("groupList_***", "groupList print status==>" + status);
                if (a != 0)
                {
                    string audio_file_name = dbClass.AudioFile("ErrorOccur");
                    this.PlaySound(audio_file_name);
                    messageDialog.ShowPrintErrorMessageOther(0);
                }
                else
                {
                    if ((status >= 1 && status <= 15) || status == 32)
                    {
                        int errStatus = status;
                        if (status == 32)
                        {
                            errStatus = 16;
                        }

                        string audio_file_name = dbClass.AudioFile("ErrorOccur");
                        this.PlaySound(audio_file_name);
                        messageDialog.ShowPrintErrorMessageOther(errStatus);
                    }
                    else
                    {
                        PrintDocument1_PrintPageUSB();
                    }
                }
            }
            catch (Exception e)
            {
                constants.SaveLogData("groupList_***", e.ToString());
                string audio_file_name = dbClass.AudioFile("ErrorOccur");
                this.PlaySound(audio_file_name);
                messageDialog.ShowPrintErrorMessageOther(0);
            }

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

        SoundPlayer player = null;
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
        }


    }
}
