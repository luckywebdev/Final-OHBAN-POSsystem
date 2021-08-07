using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace STV01
{
    public partial class BackRestore : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Form1 mainFormGlobal = null;
        Panel mainPanelGlobal = null;
        Panel bodyPanelGlobal = null;
        Constant constants = new Constant();

        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton customButton = new CustomButton();
        DateTime now = DateTime.Now;
        List<FileInfo> targetFile = null;
        List<DirectoryInfo> targetDirectory = null;
        string DestinationDBPath = null;
        string SourceDBPath = null;
        string DestinationPath = null;
        DBClass dbClass = new DBClass();

        string work_type = "backup";
        public static BackgroundWorker backgroundWorker = null;
        Form mainProgressDialog = null;
        Panel progressDialogPanel = null;
        ProgressBar progressBar1 = null;
        Label progressAlertLabel = null;
        string errorHandler = "";
        int countNum = 0;
        public static int k = 0;
        //public static BackgroundWorker backworkGlobal = null;

        MessageDialog messageDialog = new MessageDialog();
        SQLiteConnection sqlite_conn;

        public BackRestore(Form1 mainForm, Panel mainPanel)
        {
            mainFormGlobal = mainForm;
            mainPanelGlobal = mainPanel;
            messageDialog.InitBackRestore(this);
            Panel headerPanel = createPanel.CreateSubPanel(mainPanel, 0, 0, mainPanel.Width, mainPanel.Height / 10, BorderStyle.None, Color.FromArgb(255, 234, 225, 151));
            Label headerLabel = createLabel.CreateLabelsInPanel(headerPanel, "headerTitle", constants.backRestoreTitle, 0, 0, headerPanel.Width, headerPanel.Height, Color.Transparent, Color.Black, 28, false, ContentAlignment.MiddleCenter);

            Panel bodyPanel = createPanel.CreateSubPanel(mainPanel, 0, headerPanel.Bottom, mainPanel.Width, mainPanel.Height * 9 / 10, BorderStyle.None, Color.Transparent);
            bodyPanelGlobal = bodyPanel;

            Label backupLabel = createLabel.CreateLabelsInPanel(bodyPanel, "backupLabel", constants.backupLabel, 0, bodyPanel.Height / 3, bodyPanel.Width / 2, 60, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);
            backupLabel.Padding = new Padding(0, 0, 50, 0);

            Button backupBtn = customButton.CreateButtonWithImage(constants.menureadingButtonImage, "backupBtn", constants.backupLabel + "開始", bodyPanel.Width / 2, bodyPanel.Height / 3, 300, 60, 2, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(backupBtn);
            backupBtn.Click += new EventHandler(messageDialog.BackupRestore);

            Label restoreLabel = createLabel.CreateLabelsInPanel(bodyPanel, "backupLabel", constants.restoreLabel, 0, backupLabel.Bottom + 30, bodyPanel.Width / 2, 60, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleRight);
            restoreLabel.Padding = new Padding(0, 0, 50, 0);

            Button restoreBtn = customButton.CreateButtonWithImage(constants.rectGreenButton, "restoreBtn", constants.restoreLabel + "開始", bodyPanel.Width / 2, backupBtn.Bottom + 30, 300, 60, 2, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(restoreBtn);
            restoreBtn.Click += new EventHandler(messageDialog.BackupRestore);

            Button backBtn = customButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, bodyPanel.Width - 150, bodyPanel.Height - 100, 100, 50, 2, 1, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            bodyPanel.Controls.Add(backBtn);
            backBtn.Click += new EventHandler(this.BackShow);

            InitializeComponent();
        }

        public void DataBackup()
        {
            var DeviceInfo = USBCheck();
            work_type = "backup";
            errorHandler = "";
            countNum = 0;
            k = 0;
            if (DeviceInfo != null && DeviceInfo.UsbCheck == true)
            {
                if (backuprestoreBrowser.ShowDialog() == DialogResult.OK)
                {
                    //DestinationPath = DeviceInfo.DeviceName + "OHBAN_DB/";
                    DestinationPath = backuprestoreBrowser.SelectedPath + "/";
                    long availableSize = DeviceInfo.DeviceAvailableSize;
                    //string SourcePath = Directory.GetCurrentDirectory();
                    string SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    SourcePath += "\\STV01\\";
                    if (!Directory.Exists(SourcePath))
                    {
                        Directory.CreateDirectory(SourcePath);
                    }


                    DirectoryInfo dir = new DirectoryInfo(SourcePath);
                    DirectoryInfo[] allSubDir = dir.GetDirectories();
                    FileInfo[] allFiles = dir.GetFiles();
                    targetFile = new List<FileInfo>();
                    targetDirectory = new List<DirectoryInfo>();
                    long sourceFileSize = 0;

                    if (Directory.Exists(SourcePath + "/saleData"))
                    {
                        targetDirectory.Add(new DirectoryInfo(SourcePath + "/saleData"));
                    }
                    if (Directory.Exists(SourcePath + "/logData"))
                    {
                        targetDirectory.Add(new DirectoryInfo(SourcePath + "/logData"));
                    }
                    DirectoryCopy(targetDirectory, DestinationPath, true);

                    string TempDBPath = Path.Combine(SourcePath, "ohbandb_temp.db");
                    string[] tbNames = new string[] { "SaleTB", "DaySaleTB", "ReceiptTB", "CancelOrderTB", "logTB" };
                    sqlite_conn = CreateConnection(constants.dbName);
                    dbClass.DBBackup(TempDBPath, "ohbandb_temp", tbNames, work_type);

                    foreach (FileInfo files in allFiles)
                    {
                        if (files.Extension == ".db" && files.Name == "ohbandb_temp.db")
                        {
                            targetFile.Add(files);
                            sourceFileSize += files.Length;
                            if (!Directory.Exists(DestinationPath))
                            {
                                Directory.CreateDirectory(DestinationPath);
                            }
                            DestinationDBPath = Path.Combine(DestinationPath, "oban_backup.db");
                            SourceDBPath = Path.Combine(SourcePath, files.Name);
                        }
                    }

                    if (availableSize.CompareTo(sourceFileSize * 2) > 0)
                    {
                        try
                        {
                            if (SourceDBPath != null && DestinationDBPath != null)
                            {
                                backgroundWorker = new BackgroundWorker();
                                backgroundWorker.WorkerReportsProgress = true;
                                backgroundWorker.WorkerSupportsCancellation = true;
                                ProgressDialog();
                            }
                            else
                            {
                                messageDialog.BackupRestoreError("", constants.backupError);
                            }
                        }
                        catch
                        {
                            messageDialog.BackupRestoreError("", constants.backupError);
                        }
                    }
                    else
                    {
                        messageDialog.BackupRestoreError("", constants.spaceError);
                    }
                }


            }
            else
            {
                messageDialog.BackupRestoreError("", constants.noUSBError);
            }
        }

        public void DataRestore()
        {
            var DeviceInfo = USBCheck();
            work_type = "restore";
            errorHandler = "";
            countNum = 0;
            k = 0;

            if (DeviceInfo != null && DeviceInfo.UsbCheck == true)
            {
                if (backuprestoreBrowser.ShowDialog() == DialogResult.OK)
                {
                    long availableSize = DeviceInfo.DeviceAvailableSize;
                    //string SourcePath = DeviceInfo.DeviceName + "OHBAN_DB/";
                    string SourcePath = backuprestoreBrowser.SelectedPath;
                    //DestinationPath = Directory.GetCurrentDirectory();
                    DestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    DestinationPath += "\\STV01\\";
                    if (!Directory.Exists(DestinationPath))
                    {
                        Directory.CreateDirectory(DestinationPath);
                    }

                    DirectoryInfo dir = new DirectoryInfo(SourcePath);
                    DirectoryInfo[] allSubDir = dir.GetDirectories();
                    FileInfo[] allFiles = dir.GetFiles();
                    targetFile = new List<FileInfo>();
                    targetDirectory = new List<DirectoryInfo>();

                    if (Directory.Exists(SourcePath + "/saleData"))
                    {
                        targetDirectory.Add(new DirectoryInfo(SourcePath + "/saleData"));
                    }
                    if (Directory.Exists(SourcePath + "/logData"))
                    {
                        targetDirectory.Add(new DirectoryInfo(SourcePath + "/logData"));
                    }
                    DirectoryCopy(targetDirectory, DestinationPath + "/", true);

                    foreach (FileInfo files in allFiles)
                    {
                        if (files.Extension == ".db")
                        {
                            SourceDBPath = Path.Combine(SourcePath, files.Name);
                            DestinationDBPath = Path.Combine(DestinationPath, "ohbandb_temp.db");
                        }
                    }
                    try
                    {
                        if (SourceDBPath != null && DestinationDBPath != null)
                        {
                            backgroundWorker = new BackgroundWorker();
                            backgroundWorker.WorkerReportsProgress = true;
                            backgroundWorker.WorkerSupportsCancellation = true;
                            ProgressDialog();
                        }
                        else
                        {
                            messageDialog.BackupRestoreError("", constants.restoreError);
                        }
                    }
                    catch
                    {
                        messageDialog.BackupRestoreError("", constants.restoreError);
                    }
                }
            }
            else
            {
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
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
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

            string alertLabel = constants.backupLabel;
            if (work_type == "restore")
            {
                alertLabel = constants.restoreLabel;
            }
            progressAlertLabel = createLabel.CreateLabels(progressLabelPanel, "progressAlert", alertLabel, 0, progressDialogPanel.Height / 2 + 30, progressDialogPanel.Width - 10, 50, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

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
                string alertLabel = constants.backupLabel;
                if (work_type == "restore")
                {
                    alertLabel = constants.restoreLabel;
                }
                if (progressAlertLabel.InvokeRequired)
                {
                    progressAlertLabel.Invoke((MethodInvoker)delegate
                    {
                        progressAlertLabel.Text = alertLabel;
                    });
                }
                else
                {
                    progressAlertLabel.Text = alertLabel;
                }
                try
                {
                    DBBackUp(SourceDBPath, DestinationDBPath);
                    errorHandler = "";
                }
                catch (Exception err)
                {
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
                if (work_type == "backup")
                {
                    messageDialog.BackupRestoreError("", constants.backupError);
                }
                else
                {
                    messageDialog.BackupRestoreError("", constants.restoreError);
                }
            }
            else
            {
                if (work_type == "backup")
                {
                    messageDialog.BackupRestoreError(constants.backupLabel, constants.dataRunSuccess);
                }
                else
                {
                    messageDialog.BackupRestoreError(constants.restoreLabel, constants.dataRunSuccess);
                }
                SourceDBPath = null;
                DestinationDBPath = null;
            }
        }


        private static void DirectoryCopy(List<DirectoryInfo> sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            foreach (DirectoryInfo dir in sourceDirName)
            {
                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName + dir.Name))
                {
                    Directory.CreateDirectory(destDirName + dir.Name);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName + dir.Name, file.Name);
                    if (File.Exists(temppath)) File.Delete(temppath);
                    file.CopyTo(temppath, false);
                }
            }

        }

        private static void FileCopy(List<FileInfo> sourceFiles, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            foreach (FileInfo fi in sourceFiles)
            {

                if (!fi.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source file does not exist or could not be found: "
                        + fi);
                }
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                string temppath = Path.Combine(destDirName, fi.Name);
                if (File.Exists(temppath)) File.Delete(temppath);
                fi.CopyTo(temppath, false);
                Thread.Sleep(50);
                backgroundWorker.ReportProgress(k / 2);
                k++;
            }

        }

        private void DBBackUp(string sourcePath, string destinationPath)
        {
            using (var location = new SQLiteConnection(string.Format(@"Data Source={0}; Version=3;", sourcePath)))
            using (var destination = new SQLiteConnection(string.Format(@"Data Source={0}; Version=3;", destinationPath)))
            {
                location.Open();
                destination.Open();
                var hash = location.GetHashCode();
                Console.WriteLine("hash===>" + hash);
                var hash1 = destination.GetHashCode();
                Console.WriteLine("hash1===>" + hash1);
                location.BackupDatabase(destination, "main", "main", -1, null, 0);
                location.Dispose();
                destination.Dispose();
            }
            if(work_type == "restore")
            {
                //string SourcePath = Directory.GetCurrentDirectory();
                string SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                SourcePath += "\\STV01\\";
                if (!Directory.Exists(SourcePath))
                {
                    Directory.CreateDirectory(SourcePath);
                }


                string TempDBPath = Path.Combine(SourcePath, "ohbandb_temp.db");
                string[] tbNames = new string[] { "SaleTB", "DaySaleTB", "ReceiptTB", "CancelOrderTB", "logTB" };
                sqlite_conn = CreateConnection(constants.dbName);
                dbClass.DBBackup(TempDBPath, "ohbandb_temp", tbNames, work_type);
            }
        }

        private static USBDeviceInfo USBCheck()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            USBDeviceInfo device = null;
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable && d.IsReady == true)
                {
                    device = new USBDeviceInfo(true, d.Name.ToString(), d.TotalSize, d.TotalFreeSpace);
                    return device;
                }
            }
            return device;
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

        /**
         * DB Conneting Config
         **/
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
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }


    }
    class USBDeviceInfo
    {
        public USBDeviceInfo(bool usbCheck, string deviceName, long deviceTotalSize, long deviceAvailableSize)
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
