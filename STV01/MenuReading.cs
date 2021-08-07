using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;

namespace STV01
{
    public partial class MenuReading : Form
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
        CustomButton customButton = new CustomButton();
        DBClass dbClass = new DBClass();
        string SourcePath = null;
        string DestinationPath = null;
        List<FileInfo> targetFile = null;
        List<DirectoryInfo> targetDirectory = null;
        string SourceDBName = null;
        string DestinationDBName = null;
        string SourceDBFile = null;

        public static BackgroundWorker backgroundWorker = null;
        Form mainProgressDialog = null;
        Panel progressDialogPanel = null;
        ProgressBar progressBar1 = null;
        Label progressAlertLabel = null;
        string errorHandler = "";
        int countNum = 0;
        public static int k = 0;

        MessageDialog messageDialog = new MessageDialog();
        SQLiteConnection sqlite_conn;
        public MenuReading(Form1 mainForm, Panel mainPanel)
        {
            InitializeComponent();
            messageDialog.InitMenuReading(this);
            sqlite_conn = CreateConnection(constants.dbName);

            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;

            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.menuReadingTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);
            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);

            Label subContent1 = createLabel.CreateLabelsInPanel(bodyPanel, "subContent1", constants.menuReadingSubContent1, bodyPanel.Width / 5, 50, bodyPanel.Width * 3 / 5, bodyPanel.Height / 8 + 30, Color.Transparent, Color.Black, 24);

            Label subContent2 = createLabel.CreateLabelsInPanel(bodyPanel, "subContent1", constants.menuReadingSubContent2, bodyPanel.Width / 5 + 50, subContent1.Bottom, bodyPanel.Width * 3 / 5 - 100, bodyPanel.Height / 8 + 30, Color.Transparent, Color.Black, 24);

            Button readButton = customButton.CreateButtonWithImage(constants.menureadingButtonImage, "readButton", constants.menuReadingButton, bodyPanel.Width / 2 - 100, subContent2.Bottom + 50, 200, 50, 0, 20, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(readButton);
            //readButton.Click += new EventHandler(this.DataLoading);
            readButton.Click += new EventHandler(this.OpenFileDialog);

            Button cancelButton = customButton.CreateButtonWithImage(constants.cancelButton, "readButton", constants.cancelButtonText, bodyPanel.Width - 150, bodyPanel.Height - 100, 100, 50, 0, 20, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(cancelButton);
            cancelButton.Click += new EventHandler(this.BackShow);

        }


        private void MenuReading_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private static USBDeviceInfos USBCheck()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            USBDeviceInfos device = null;
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable && d.IsReady == true)
                {
                    device = new USBDeviceInfos(true, d.Name.ToString(), d.TotalSize, d.TotalFreeSpace);
                    return device;
                }
            }
            return device;
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            //openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "zip files (*.zip) | *.zip";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog1.FileName;
                    string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    currentDir += "\\STV01\\";
                    if (!Directory.Exists(currentDir))
                    {
                        Directory.CreateDirectory(currentDir);
                    }

                    //string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp";
                    if (!Directory.Exists(currentDir + "\\temp"))
                    {
                        Directory.CreateDirectory(currentDir + "\\temp");
                    }



                    string tempPath = currentDir + "\\temp";
                    UnZipFile(filePath, tempPath);

                    SourcePath = tempPath;
                    //DestinationPath = Directory.GetCurrentDirectory();
                    DestinationPath = currentDir;
                    DirectoryInfo dir = new DirectoryInfo(SourcePath);
                    FileInfo[] allFiles = dir.GetFiles();
                    targetFile = new List<FileInfo>();
                    targetDirectory = new List<DirectoryInfo>();
                    foreach (FileInfo files in allFiles)
                    {
                        if (files.Extension == ".db")
                        {
                            SourceDBName = files.Name.Split('.')[0];
                            SourceDBFile = files.Name;
                            DestinationDBName = "obanp1";
                        }
                    }

                    if (SourceDBName != null)
                    {
                        backgroundWorker = new BackgroundWorker();
                        backgroundWorker.WorkerReportsProgress = true;
                        backgroundWorker.WorkerSupportsCancellation = true;
                        ProgressDialog();
                    }
                    else
                    {
                        constants.SaveLogData("menuReading_1", "sourceDBName null error");
                        messageDialog.ShowMenuReadingMessage();
                    }
                }
                catch(Exception ex)
                {
                    constants.SaveLogData("menuReading_2", ex.ToString());
                    messageDialog.ShowMenuReadingMessage();
                }
            }
        }

        private void UnZipFile(string zipFile, string dest)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipFile, dest);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void DataLoading(object sender, EventArgs e)
        {
            var DeviceInfo = USBCheck();
            if (DeviceInfo != null && DeviceInfo.UsbCheck == true)
            {
                long availableSize = DeviceInfo.DeviceAvailableSize;
                SourcePath = DeviceInfo.DeviceName + "NEW_DB/";
                try
                {
                    if (SourcePath != null)
                    {
                        DestinationPath = Directory.GetCurrentDirectory();
                        DirectoryInfo dir = new DirectoryInfo(SourcePath);
                        FileInfo[] allFiles = dir.GetFiles();
                        targetFile = new List<FileInfo>();
                        targetDirectory = new List<DirectoryInfo>();
                        foreach (FileInfo files in allFiles)
                        {
                            if (files.Extension == ".db")
                            {
                                SourceDBName = files.Name.Split('.')[0];
                                SourceDBFile = files.Name;
                                DestinationDBName = "obanp1";
                                sqlite_conn = CreateConnection(DestinationDBName);
                                if (sqlite_conn.State == ConnectionState.Closed)
                                {
                                    sqlite_conn.Open();
                                }
                            }
                        }

                        if (SourceDBName != null)
                        {
                            backgroundWorker = new BackgroundWorker();
                            backgroundWorker.WorkerReportsProgress = true;
                            backgroundWorker.WorkerSupportsCancellation = true;
                            ProgressDialog();

                        }
                        else
                        {
                            constants.SaveLogData("menuReading_3", "SourceDBName null error");
                            messageDialog.ShowMenuReadingMessage();
                        }
                    }
                    else
                    {
                        constants.SaveLogData("menuReading_4", "sourcePath null error");
                        messageDialog.ShowMenuReadingMessage();
                    }
                }
                catch(Exception ex)
                {
                    constants.SaveLogData("menuReading_5", ex.ToString());
                    messageDialog.ShowMenuReadingMessage();
                }

            }
            else
            {
                constants.SaveLogData("menuReading_6", "USB Check failed");
                messageDialog.BackupRestoreError("", constants.noUSBError);
            }
        }

        private void ProgressDialog()
        {
            Form progressDialog = new Form();
            mainProgressDialog = progressDialog;
            progressDialog.Size = new Size(width / 3, height / 4);
            progressDialog.BackColor = Color.White;
            progressDialog.StartPosition = FormStartPosition.CenterParent;
            progressDialog.WindowState = FormWindowState.Normal;
            progressDialog.ControlBox = false;
            progressDialog.TopLevel = true;
            progressDialog.TopMost = true;
            progressDialog.FormBorderStyle = FormBorderStyle.None;
            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                progressDialog.BackgroundImage = new Bitmap(img);
            }

            progressDialog.BackgroundImageLayout = ImageLayout.Stretch;

            progressDialogPanel = createPanel.CreateMainPanel(progressDialog, 0, 0, progressDialog.Width, progressDialog.Height, BorderStyle.None, Color.Transparent);

            //progressAlertLabel = progressAlert;

            progressBar1 = new ProgressBar();
            progressBar1.Location = new Point(progressDialogPanel.Width / 5, progressDialogPanel.Height / 2 - 20);
            progressBar1.Size = new Size(progressDialogPanel.Width * 3 / 5, 20);
            progressDialogPanel.Controls.Add(progressBar1);
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            progressBar1.SendToBack();

            FlowLayoutPanel progressLabelPanel = createPanel.CreateFlowLayoutPanel(progressDialogPanel, 0, progressDialogPanel.Height / 2 + 10, progressDialogPanel.Width, progressDialogPanel.Height / 2 - 20, Color.Transparent, new Padding(0));
            string notify = "メニュー設定データの読み込み";
            progressAlertLabel = createLabel.CreateLabels(progressLabelPanel, "progressAlert", notify, 0, progressDialogPanel.Height / 2 + 30, progressDialogPanel.Width - 10, 50, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);


            if (backgroundWorker.IsBusy != true)
            {
                countNum++;
                Console.WriteLine(countNum);
                backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
                backgroundWorker.RunWorkerAsync();
            }


            progressDialog.ShowDialog();

        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (countNum == 1)
            {

                backgroundWorker = sender as BackgroundWorker;
                try
                {
                    DirectoryCopy(SourcePath, DestinationPath, true);
                    dbClass.DBCopy(Path.Combine(DestinationPath, SourceDBFile), SourceDBName, constants.tbNames);

                    dbClass.CreateSaleTB();
                    dbClass.CreateDaySaleTB();
                    dbClass.CreateReceiptTB();
                    dbClass.CreateCancelOrderTB();
                    dbClass.CreateLogTB();
                    dbClass.CreateProductOptionTB();
                    dbClass.CreateProductOptionValueTB();
                    sqlite_conn = CreateConnection(DestinationDBName);
                    if (sqlite_conn.State == ConnectionState.Closed)
                    {
                        sqlite_conn.Open();
                    }

                    dbClass.DeleteData(sqlite_conn, constants.tbNames[15]);
                    dbClass.DeleteData(sqlite_conn, constants.tbNames[16]);

                    sqlite_conn.Close();
                    sqlite_conn.Dispose();
                    sqlite_conn = null;

                    errorHandler = "";
                }
                catch (Exception err)
                {
                    constants.SaveLogData("menuReading_7", err.ToString());
                    Console.WriteLine(err);
                    errorHandler = "error";
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundWorker.CancelAsync();
                    }

                }

                if (k < 100)
                {
                    while (k <= 100)
                    {
                        Thread.Sleep(50);
                        backgroundWorker.ReportProgress(k * 1);
                        k++;
                    }
                }
                countNum++;
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage <= 100)
                progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mainProgressDialog.Close();
            backgroundWorker.CancelAsync();

            countNum++;
            if (errorHandler == "error")
            {
                dbClass.InsertLog(5, " メニュー読込", "失敗()");
                constants.SaveLogData("menuReading_8", "menu reading failed");
                messageDialog.ShowMenuReadingMessage();
            }
            else
            {
                dbClass.InsertLog(5, " メニュー読込", "成功");
                //string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp";
                string currentDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                currentDir += "\\STV01\\";
                if (!Directory.Exists(currentDir))
                {
                    Directory.CreateDirectory(currentDir);
                }

                //string tempPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\temp";
                if (!Directory.Exists(currentDir + "\\temp"))
                {
                    Directory.CreateDirectory(currentDir + "\\temp");
                }



                string tempPath = currentDir + "\\temp";
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
                if(File.Exists(Path.Combine(currentDir, "ohbandb_temp.db")))
                {
                    File.Delete(Path.Combine(currentDir, "ohbandb_temp.db"));
                }
                mainPanelGlobal.Controls.Clear();
                MainMenu mainMenu = new MainMenu();
                mainMenu.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
            }
        }



        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath)) File.Delete(temppath);
                file.CopyTo(temppath, false);
                Thread.Sleep(50);
                backgroundWorker.ReportProgress(k / 2);
                k++;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    if (Directory.Exists(temppath))
                    {
                        Directory.Delete(temppath, true);
                    }
                    if (!Directory.Exists(temppath))
                    {
                        Directory.CreateDirectory(temppath);
                    }

                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    Thread.Sleep(50);
                    backgroundWorker.ReportProgress(k / 2);
                    k++;
                }
            }
        }

        public void BackShow(object sender, EventArgs e)
        {
            mainPanelGlobal.Controls.Clear();
            MainMenu mainMenu = new MainMenu();
            mainMenu.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
        }

        public void BackShowStart()
        {
            mainPanelGlobal.Controls.Clear();
            MainMenu mainMenu = new MainMenu();
            mainMenu.CreateMainMenuScreen(mainFormGlobal, mainPanelGlobal);
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
        private void RestoreDB(string filePath, string srcFilename, string destFileName, bool IsCopy = false)
        {
            var srcfile = srcFilename;
            var destfile = Path.Combine(filePath, destFileName);

            if (File.Exists(destfile)) File.Delete(destfile);

            if (IsCopy)
                BackupDB(filePath, srcFilename, destFileName);
            else
                File.Move(srcfile, destfile);
        }

        private void BackupDB(string filePath, string srcFilename, string destFileName)
        {
            var srcfile = Path.Combine(filePath, srcFilename);
            var destfile = Path.Combine(filePath, destFileName);

            if (File.Exists(destfile)) File.Delete(destfile);

            File.Copy(srcfile, destfile);
        }

    }

    class USBDeviceInfos
    {
        public USBDeviceInfos(bool usbCheck, string deviceName, long deviceTotalSize, long deviceAvailableSize)
        {
            this.UsbCheck = usbCheck;
            this.DeviceName = deviceName;
            this.DeviceTotalSize = deviceTotalSize;
            this.DeviceAvailableSize = deviceAvailableSize;
        }
        public bool UsbCheck { get; private set; }
        public string DeviceName { get; private set; }
        public long DeviceTotalSize { get; private set; }
        public long DeviceAvailableSize { get; private set; }
    }


}
