using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class MessageDialog : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton createButton = new CustomButton();
        SaleScreen saleScreenGlobal = null;
        CategoryList categoryList = null;
        GroupList groupList = null;
        MenuReading menuReading = null;
        BackRestore backRestore = null;
        MainMenu mainMenu = null;
        string backupIndicator = "";
        string errorHandler = null;
        public Form DialogFormGlobal = null;
        string indicator = "";
        public MessageDialog()
        {
            InitializeComponent();
        }

        public void InitMainMenu(MainMenu sendHandler)
        {
            mainMenu = sendHandler;
        }

        public void InitSaleScreen(SaleScreen sendHandler)
        {
            saleScreenGlobal = sendHandler;
        }
        public void InitCategoryList(CategoryList sendHandler)
        {
            categoryList = sendHandler;
        }
        public void InitGroupList(GroupList sendHandler)
        {
            groupList = sendHandler;
        }
        public void InitMenuReading(MenuReading sendHandler)
        {
            menuReading = sendHandler;
        }

        public void InitBackRestore(BackRestore sendHandler)
        {
            backRestore = sendHandler;
        }

        public void MessageDialogInit(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            indicator = btnTemp.Name;
            if (indicator == "categoryPrintButton")
            {
                ShowCategoryPrintMessage();
            }
            else if (indicator == "groupPrintButton")
            {

                ShowGroupPrintMessage();
            }
        }
        public void PowerApplication()
        {
            try
            {
                Form dialogForm = new Form();
                dialogForm.Size = new Size(width * 2 / 5, height / 3);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.ControlBox = false;
                dialogForm.TopLevel = true;
                dialogForm.TopMost = true;

                dialogForm.FormBorderStyle = FormBorderStyle.None;
                DialogFormGlobal = dialogForm;

                Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.dialogFormImage))
                {
                    mainPanel.BackgroundImage = new Bitmap(img);
                }
                mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label messageLabel1 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", "", 50, 0, mainPanel.Width - 100, (mainPanel.Height - 100) / 5, Color.Transparent, Color.Red, 18, false, ContentAlignment.BottomCenter);
                messageLabel1.Padding = new Padding(30, 0, 30, 0);

                Label messageLabel2 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel2", constants.shutdownMsg, 50, (mainPanel.Height - 100) / 5, mainPanel.Width - 100, (mainPanel.Height - 100) * 4 / 5, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
                messageLabel2.Padding = new Padding(30, 0, 30, 0);

                dialogForm.FormClosed += new FormClosedEventHandler(CloseFormHandle);

                sumThreadmm = new Thread(PowerProc);
                sumThreadmm.SetApartmentState(ApartmentState.STA);
                sumThreadmm.Start();

                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    DialogFormGlobal.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        DialogFormGlobal.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    DialogFormGlobal.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            DialogFormGlobal.Owner = null;
                        }
                    });
                    DialogFormGlobal.ShowDialog();

                }
                else
                {
                    DialogFormGlobal.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_1", ex.ToString());
                return;
            }
        }

        private void PowerProc()
        {
            Thread.Sleep(3000);
            if (DialogFormGlobal.InvokeRequired)
            {
                DialogFormGlobal.Invoke((MethodInvoker)delegate
                {
                    DialogFormGlobal.Dispose();
                    DialogFormGlobal = null;
                });
            }
            else
            {
                DialogFormGlobal.Dispose();
                DialogFormGlobal = null;
            }
        }



        Form DialogFormGlobalR;

        private void BackShowErr(object sender, EventArgs e)
        {
            DialogFormGlobalR.Close();
            DialogFormGlobalR.Hide();
            DialogFormGlobalR = null;
        }

        private void ShowCategoryPrintMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 3, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", constants.categoryListPrintMessage, 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button okButton = createButton.CreateButtonWithImage(constants.rectRedButton, "categoryPrintOkButton", constants.yesStr, mainPanel.Width / 8, messageLabel.Bottom, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(okButton);
            okButton.Click += new EventHandler(this.CategoryPrintView);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.noStr, mainPanel.Width * 5 / 8, messageLabel.Bottom, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            closeButton.ForeColor = Color.White;
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        private void ShowGroupPrintMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", constants.groupListPrintMessage, 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button okButton = createButton.CreateButtonWithImage(constants.rectRedButton, "categoryPrintOkButton", constants.yesStr, mainPanel.Width / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(okButton);
            okButton.Click += new EventHandler(this.GroupPrintView);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.noStr, mainPanel.Width * 5 / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        public void ShowMenuReadingMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                dialogForm.BackgroundImage = new Bitmap(img);
            }
            dialogForm.BackgroundImageLayout = ImageLayout.Stretch;
            DialogFormGlobal = dialogForm;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            Label messageLabel1 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", constants.menuReadingErrorTitle, 50, 0, mainPanel.Width - 100, (mainPanel.Height - 100) / 3, Color.Transparent, Color.Red, 18, false, ContentAlignment.BottomCenter);
            messageLabel1.Padding = new Padding(30, 0, 30, 0);
            Label messageLabel2 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel2", constants.menuReadingErrorContent, 50, (mainPanel.Height - 100) / 3, mainPanel.Width - 100, (mainPanel.Height - 100) * 2 / 3, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleCenter);
            messageLabel2.Padding = new Padding(30, 0, 30, 0);


            Button closeButton = createButton.CreateButtonWithImage(constants.cancelButton, "closeButton", constants.noStr, mainPanel.Width / 2 - 75, mainPanel.Height - 100, 150, 50, 0, 20, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShowMenuReading);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        /**
         * @param
         * string title: error title
         * int type: error type: banknote unit error=>0, Out of change=>1
         * int content_index: error content array index
         * int[] status: deposite or withdraw status
         * banknote unit error: status = [10000 x n, 5000 x n, 2000 x n, 1000 x n, 500 x n, 100 x n, 50 x n, 10 x n] n = amount of coin or papers
         * out of change: status = [1000 => 0, 500 => 0, 100 => 1, 50 => 1, 10 => 0] ok->0, 補充->1
         * */
        public void ShowErrorMessage(string title, int type = 0, int content_index = 0, int[] errStatus = null, int[] status = null, int errorType = 0, int[] wdrAnt = null, string errCode = "", string errCode2 = "")
        {
            if (type != 1)
            {
                ShowErrorDetailMessage1(title, content_index, type, errStatus, status, errorType, wdrAnt, errCode, errCode2);
            }
            else
            {
                title = "釣銭切れ";
                ShowErrorDetailMessage2(title, status);
            }

        }
        /**
         * for stop sale
         * */
        
        public void ShowErrorDetailMessage2(string title, int[] status = null)
        {
            try
            {
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                string[] status_msg = new string[] { "ok", "補充" };
                Color[] status_color = new Color[] { Color.Green, Color.Red };

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "errTitle", title, 100, 30, dialogPanel.Width - 200, dialogPanel.Height / 6, Color.FromArgb(255, 0, 88, 150), Color.FromArgb(255, 255, 242, 75), dialogPanel.Height / 16);

                Panel errorDetailPanel = null;


                if (status != null)
                {
                    errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 50, dialogTitle.Bottom + 10, dialogPanel.Width - 100, 140, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                    Label itemLbl1_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item1_1", "500円-", 0, 0, errorDetailPanel.Width / 2 - 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl1_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value1_1", status_msg[status[4] == 0 ? 1 : 0], errorDetailPanel.Width / 2 - 30, 0, 60, 40, Color.Transparent, status_color[status[4] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);
                    Label itemLbl1_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item1_2", "100円-", errorDetailPanel.Width / 2 + 30, 0, 70, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl1_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value1_2", status_msg[status[3] == 0 ? 1 : 0], errorDetailPanel.Width / 2 + 100, 0, 60, 40, Color.Transparent, status_color[status[3] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                    Label itemLbl2_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item2_1", "50円-", 0, 40, errorDetailPanel.Width / 2 - 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl2_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value2_1", status_msg[status[2] == 0 ? 1 : 0], errorDetailPanel.Width / 2 - 30, 40, 60, 40, Color.Transparent, status_color[status[2] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);
                    Label itemLbl2_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item2_2", "10円-", errorDetailPanel.Width / 2 + 30, 40, 70, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl2_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value2_2", status_msg[status[1] == 0 ? 1 : 0], errorDetailPanel.Width / 2 + 100, 40, 60, 40, Color.Transparent, status_color[status[1] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                    Label itemLbl3_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item3_1", "1000円-", 0, 80, errorDetailPanel.Width / 2, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl3_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value3_1", status_msg[status[0] == 0 ? 1 : 0], errorDetailPanel.Width / 2, 80, 60, 40, Color.Transparent, status_color[status[0] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                }
                else
                {
                    errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 50, dialogTitle.Bottom + 10, dialogPanel.Width - 100, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));
                }


                Label instruct1 = createLabel.CreateLabelsInPanel(dialogPanel, "item1", "赤字", 70, errorDetailPanel.Bottom + 20, 50, 40, Color.Transparent, Color.Red, 12, false, ContentAlignment.MiddleLeft);
                Label instruct2 = createLabel.CreateLabelsInPanel(dialogPanel, "item2", "で", 122, errorDetailPanel.Bottom + 20, 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);
                Label instruct3 = createLabel.CreateLabelsInPanel(dialogPanel, "item3", "補充", 152, errorDetailPanel.Bottom + 20, 50, 40, Color.Transparent, Color.Red, 12, false, ContentAlignment.MiddleLeft);
                Label instruct4 = createLabel.CreateLabelsInPanel(dialogPanel, "item4", "となっている硬貨、紙幣をを補充してください。", 202, errorDetailPanel.Bottom + 20, dialogPanel.Width - 160, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);
                Label instruct5 = createLabel.CreateLabelsInPanel(dialogPanel, "item5", "補充が終わりましたらエラーリセットを押してください。", 70, errorDetailPanel.Bottom + 60, dialogPanel.Width - 140, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);

                Button powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "powerBtn", "", dialogPanel.Right - 200, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(powerBtn);

                powerBtn.MouseDown += new MouseEventHandler(this.ErrPowerApplication);

                Button errCancelBtn = createButton.CreateButtonWithImage(constants.resetButton, "errCancelBtn", "", 50, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(errCancelBtn);


                errCancelBtn.Click += new EventHandler(this.ResetComModule);
                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();

                }
                else
                {
                    dialogForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_3", ex.ToString());
                return;
            }

        }

        int content_index_param = 0;
        private void DeactiveForm(Object obj, EventArgs e)
        {
            if( DialogFormGlobal != null )
            {
                if (DialogFormGlobal.InvokeRequired)
                {
                    DialogFormGlobal.Invoke(new Action(() => DialogFormGlobal.Activate()));
                }
                else
                {
                    DialogFormGlobal.Activate();
                }
            }
        }

        public void ShowErrorDetailMessage1(string title, int content_index = 0, int type = 0, int[] errStatus = null, int[] status = null, int errorType = 0, int[] wdrAnt = null, string errCode = "00", string errCode2 = "00")
        {
            try
            {
                content_index_param = content_index;
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;

                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;

                dialogForm.TopLevel = true;
                dialogForm.TopMost = true;
                //dialogForm.Shown += new EventHandler(this.DeactiveForm);
                dialogForm.Deactivate += new EventHandler(this.DeactiveForm);

                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;
                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                string errTitle = "";
                Color errTitleBackColor = Color.FromArgb(255, 0, 88, 150);

                if (errStatus != null && errStatus[2] == 1)
                {
                    errTitle = "高額紙幣識別機を点検して下さい";
                    errTitleBackColor = Color.FromArgb(255, 63, 72, 204);
                }
                else if (errStatus != null && errStatus[3] == 1)
                {
                    errTitle = "千円札識別機を点検して下さい";
                    errTitleBackColor = Color.FromArgb(255, 63, 72, 204);
                }
                else if (errStatus != null && errStatus[4] == 1)
                {
                    errTitle = "紙幣排出機を点検して下さい";
                    errTitleBackColor = Color.FromArgb(255, 0, 162, 232);
                }
                else if (errStatus != null && errStatus[5] == 1)
                {
                    errTitle = "コインメック・チューブを点検して下さい";
                    errTitleBackColor = Color.FromArgb(255, 34, 177, 76);
                }

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errTitle, 70, 20, dialogPanel.Width - 140, dialogPanel.Height / 6, errTitleBackColor, Color.White, dialogPanel.Height / 20);

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImage))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);

                Panel errorDetailPanel = null;
                string errorDetail = "";
                Color high_fontColor = Color.Black;

                Color errBackColor = Color.FromArgb(255, 215, 228, 189);
                Color errFontColor = Color.Black;
                if (errorType == 2 || errorType == 3)
                {
                    errBackColor = Color.FromArgb(255, 252, 213, 181);
                    if (status[7] != 0 || status[6] != 0 || status[5] != 0)
                    {
                        high_fontColor = Color.Red;
                    }
                }

                Label dialogInstruction;
                Label dialogInstruction_2;
                Button errCancelBtn;
                Button powerBtn;

                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                errorDetail = "点検終了後【復帰】アイコンを\nクリックしてください";

                dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errorDetail, 20, errorDetailPanel.Bottom + 15, dialogPanel.Width * 2 / 3 - 30, 50, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

                errCancelBtn = createButton.CreateButtonWithImage(constants.resetButton, "errCancelBtn", "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(errCancelBtn);

                dialogInstruction_2 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", "【終了】アイコンを長押しすると\n開始画面に戻ります", 20, errCancelBtn.Bottom + 30, dialogPanel.Width * 2 / 3 - 30, 60, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

                powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "unitBtn_" + errCode + "_" + errCode2, "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction_2.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);

                powerBtn.Name = "unitBtn_" + errCode + "_" + errCode2;
                dialogPanel.Controls.Add(powerBtn);

                powerBtn.Click += new EventHandler(this.ErrPowerApplication);


                errCancelBtn.Click += new EventHandler(this.ResetComModule);

                
                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    if( printDialogForm != null)
                    {
                        printDialogForm.TopMost = false;
                        printDialogForm.TopLevel = false;
                    }
                    dialogForm.ShowDialog();

                }
                else
                {
                    if (printDialogForm != null)
                    {
                        printDialogForm.TopMost = false;
                        printDialogForm.TopLevel = false;
                    }
                    dialogForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_4", ex.ToString());
                return;
            }
        }

        int err_count = 0;
        int[] contentGlobal = null;
        string[] titleGlobal = null;
        string printType = "";
        Label TitleLb = null;
        Label ContentLb = null;
        PictureBox pBGlobal = null;

        /**
         * @param
         * string[] title: error title array
         * int[] content_index: error content array index array
         * main men
         * */
        public void ShowErrorDetailMessageOut(string[] title, int[] content_index, int[] errStatus = null, string errCode = "00", string errCode2 = "00")
        {
            try
            {
                this.Width = width;
                this.Height = height;
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;
                //mainMenu.formType = "error_detail_out";
                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                err_count = content_index.Length;
                contentGlobal = content_index;
                titleGlobal = title;

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                string errTitle = "高額紙幣識別機を点検して下さい";
                Color errTitleBackColor = Color.FromArgb(255, 63, 72, 204);
                int titleTop = 20;
                int titleHeight = dialogPanel.Height / 6;
                int titleFontSize = 22;

                if (content_index[0] == 18)
                {
                    errTitle = "ポートエラー\n再起動アイコンをクリックしてください";
                    errTitleBackColor = Color.FromArgb(255, 208, 96, 251);
                    titleTop = 35;
                    titleFontSize = 18;
                    titleHeight = dialogPanel.Height / 5;
                }
                else
                {
                    if (errStatus != null && errStatus[2] == 1)
                    {
                        errTitle = "高額紙幣識別機を点検して下さい";
                        errTitleBackColor = Color.FromArgb(255, 63, 72, 204);
                    }
                    else if (errStatus != null && errStatus[3] == 1)
                    {
                        errTitle = "千円札識別機を点検して下さい";
                        errTitleBackColor = Color.FromArgb(255, 63, 72, 204);
                    }
                    else if (errStatus != null && errStatus[4] == 1)
                    {
                        errTitle = "紙幣排出機を点検して下さい";
                        errTitleBackColor = Color.FromArgb(255, 0, 162, 232);
                    }
                    else if (errStatus != null && errStatus[5] == 1)
                    {
                        errTitle = "コインメック・チューブを点検して下さい";
                        errTitleBackColor = Color.FromArgb(255, 34, 177, 76);
                    }

                }

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errTitle, 70, titleTop, dialogPanel.Width - 140, dialogPanel.Height / 6, errTitleBackColor, Color.White, titleFontSize);
                TitleLb = dialogTitle;

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImageOut))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);
                pBGlobal = pBox;

                Panel errorDetailPanel = null;
                string errorDetail = "";

                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                string errCodeStr = "";
                if (errCode != "")
                {
                    errCodeStr = " （コード：" + errCode + ")";
                }

                int currentY = 0;
                if (content_index[0] != 18)
                {
                    errorDetail = "点検終了後【復帰】アイコンを\nクリックしてください";

                    Label dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errorDetail, 30, errorDetailPanel.Bottom + 20, dialogPanel.Width * 2 / 3 - 50, 60, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);
                    ContentLb = dialogInstruction;

                    Button errCancelBtn = createButton.CreateButtonWithImage(constants.resetButton, "errCancelBtn", "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                    dialogPanel.Controls.Add(errCancelBtn);

                    currentY = errCancelBtn.Bottom + 30;
                    errCancelBtn.Click += new EventHandler(this.MainResetComModule);
                    Label dialogInstruction_2 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction_2", "【終了】アイコンを長押しすると\n開始画面に戻ります", 30, currentY, dialogPanel.Width * 2 / 3 - 50, 60, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

                    Button powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "powerButton", "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction_2.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                    dialogPanel.Controls.Add(powerBtn);

                    powerBtn.MouseDown += new MouseEventHandler(this.ShutDownApp);

                }
                else
                {
                    currentY = errorDetailPanel.Bottom + 60;
                    Button powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "unitBtn_" + errCode + "_" + errCode2, "", dialogPanel.Width * 1 / 3 - 80, currentY, 230, 80, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                    dialogPanel.Controls.Add(powerBtn);
                    powerBtn.MouseDown += new MouseEventHandler(this.ShutDownApp);
                }

                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();

                }
                else
                {
                    dialogForm.ShowDialog(this);
                }

            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_5", ex.ToString());
                return;
            }
        }

        public void ShowErrorDetailMessageOut2(string title, int[] status = null)
        {
            try
            {
                Form dialogForm = new Form();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                string[] status_msg = new string[] { "ok", "補充" };
                Color[] status_color = new Color[] { Color.Green, Color.Red };

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(dialogPanel.BackgroundImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "errTitle", title, 100, 30, dialogPanel.Width - 200, dialogPanel.Height / 6, Color.FromArgb(255, 0, 88, 150), Color.FromArgb(255, 255, 242, 75), dialogPanel.Height / 16);

                Panel errorDetailPanel = null;


                if (status != null)
                {
                    errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 50, dialogTitle.Bottom + 10, dialogPanel.Width - 100, 140, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                    Label itemLbl1_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item1_1", "500円-", 0, 0, errorDetailPanel.Width / 2 - 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl1_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value1_1", status_msg[status[4] == 0 ? 1 : 0], errorDetailPanel.Width / 2 - 30, 0, 60, 40, Color.Transparent, status_color[status[4] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);
                    Label itemLbl1_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item1_2", "100円-", errorDetailPanel.Width / 2 + 30, 0, 70, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl1_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value1_2", status_msg[status[3] == 0 ? 1 : 0], errorDetailPanel.Width / 2 + 100, 0, 60, 40, Color.Transparent, status_color[status[3] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                    Label itemLbl2_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item2_1", "50円-", 0, 40, errorDetailPanel.Width / 2 - 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl2_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value2_1", status_msg[status[2] == 0 ? 1 : 0], errorDetailPanel.Width / 2 - 30, 40, 60, 40, Color.Transparent, status_color[status[2] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);
                    Label itemLbl2_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item2_2", "10円-", errorDetailPanel.Width / 2 + 30, 40, 70, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl2_2 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value2_2", status_msg[status[1] == 0 ? 1 : 0], errorDetailPanel.Width / 2 + 100, 40, 60, 40, Color.Transparent, status_color[status[1] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                    Label itemLbl3_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "item3_1", "1000円-", 0, 80, errorDetailPanel.Width / 2, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.BottomRight);
                    Label valueLbl3_1 = createLabel.CreateLabelsInPanel(errorDetailPanel, "value3_1", status_msg[status[0] == 0 ? 1 : 0], errorDetailPanel.Width / 2, 80, 60, 40, Color.Transparent, status_color[status[0] == 0 ? 1 : 0], 12, false, ContentAlignment.BottomLeft);

                }
                else
                {
                    errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 50, dialogTitle.Bottom + 10, dialogPanel.Width - 100, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));
                }


                Label instruct1 = createLabel.CreateLabelsInPanel(dialogPanel, "item1", "赤字", 70, errorDetailPanel.Bottom + 20, 50, 40, Color.Transparent, Color.Red, 12, false, ContentAlignment.MiddleLeft);
                Label instruct2 = createLabel.CreateLabelsInPanel(dialogPanel, "item2", "で", 122, errorDetailPanel.Bottom + 20, 30, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);
                Label instruct3 = createLabel.CreateLabelsInPanel(dialogPanel, "item3", "補充", 152, errorDetailPanel.Bottom + 20, 50, 40, Color.Transparent, Color.Red, 12, false, ContentAlignment.MiddleLeft);
                Label instruct4 = createLabel.CreateLabelsInPanel(dialogPanel, "item4", "となっている硬貨、紙幣をを補充してください。", 202, errorDetailPanel.Bottom + 20, dialogPanel.Width - 160, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);
                Label instruct5 = createLabel.CreateLabelsInPanel(dialogPanel, "item5", "補充が終わりましたらエラーリセットを押してください。", 70, errorDetailPanel.Bottom + 60, dialogPanel.Width - 140, 40, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);

                Button powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "powerBtn", "", dialogPanel.Right - 200, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(powerBtn);

                powerBtn.MouseDown += new MouseEventHandler(this.ShutDownApp);

                Button errCancelBtn = createButton.CreateButtonWithImage(constants.resetButton, "errCancelBtn", "", 50, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(errCancelBtn);


                errCancelBtn.Click += new EventHandler(this.MainResetComModule);

                if (dialogForm.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();
                }
                else
                {
                    dialogForm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_6", ex.ToString());
                return;
            }
        }

        public void ShowErrorDetailConfirmMessageOut(string errCode = "00", string errCode2 = "00", string errCode3 = "")
        {
            try
            {
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;


                string errTitle = "エラー発生中に終了を選択しました。";
                Color errTitleBackColor = Color.FromArgb(255, 255, 0, 0);


                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errTitle, 70, 20, dialogPanel.Width - 140, dialogPanel.Height / 6, errTitleBackColor, Color.White, dialogPanel.Height / 21);
                TitleLb = dialogTitle;

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImageOut))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);
                pBGlobal = pBox;

                Panel errorDetailPanel = null;

                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                string errCodeStr = "";
                string errContent = "復帰を押してもエラーが解消されない場合は本体付属のエラー対応表より下記のエラーコードに対応する処置をするか、メーカーにエラーコードをお問い合わせください。";

                if (errCode != "00" || errCode2 != "00")
                {
                    errContent = "復帰を押してもエラーが解消されない場合は本体付属のエラー対応表より下記のエラーコードに対応する処置をするか、メーカーにエラーコードをお問い合わせください。";
                    if (errCode != "00" && errCode2 != "00" && errCode != "" && errCode2 != "")
                    {
                        errCodeStr = " （コード01：" + errCode + "   コード02：" + errCode2 + ")";
                    }
                }
                else if (errCode3 != "")
                {
                    errContent = printType + constants.printErrMsg[int.Parse(errCode3)];
                }

                Label dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errContent, 30, errorDetailPanel.Bottom + 20, dialogPanel.Width * 2 / 3 - 50, 150, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleLeft);
                ContentLb = dialogInstruction;

                Label dialogInstruction_1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction_2", errCodeStr, 30, dialogInstruction.Bottom + 5, dialogPanel.Width * 2 / 3 - 50, 30, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleCenter);


                Label dialogInstruction_2 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", "下記のボタンを押すと電源が切れます。", 20, dialogPanel.Bottom - 80, dialogPanel.Width * 2 / 3 - 30, 50, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);

                Button powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "powerBtn", "", dialogPanel.Right - 200, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(powerBtn);

                powerBtn.MouseDown += new MouseEventHandler(this.ShutDownApp);

                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();

                }
                else
                {
                    dialogForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_7", ex.ToString());
                return;
            }

        }

        public bool bExit = false;
        private void ErrPowerApplication(object sender, EventArgs e)
        {
            try
            {
                
                Button tempBtn = (Button)sender;
                string[] errCodeArr = tempBtn.Name.Split('_');

                //this.ExitForError();
                if (mainMenu != null)
                {
                    if (errCodeArr[0] == "unitBtn")
                    {
                        if (DialogFormGlobal != null)
                        {
                            DialogFormGlobal.Close();
                            DialogFormGlobal.Hide();
                            DialogFormGlobal = null;
                        }
                        ShowErrorDetailConfirmMessageOut(errCodeArr[1], errCodeArr[2]);
                    }
                    else
                    {
                        if (printDialogForm != null)
                        {
                            printDialogForm.Close();
                            printDialogForm.Hide();
                            printDialogForm = null;
                        }
                        PowerApplication();
                        Shutdown_windows();
                    }
                }
                else if (saleScreenGlobal != null)
                {

                    if (errCodeArr[0] == "unitBtn")
                    {
                        if (DialogFormGlobal != null)
                        {
                            DialogFormGlobal.Close();
                            DialogFormGlobal.Hide();
                            DialogFormGlobal = null;
                        }
                    }
                    else
                    {
                        if (printDialogForm != null)
                        {
                            printDialogForm.Close();
                            printDialogForm.Hide();
                            printDialogForm = null;
                        }

                    }
                    constants.SaveLogData("messageDialog_8_*", "saleScreen error close");
                    saleScreenGlobal.bBackShow = true;

                    saleScreenGlobal.BackShow();
                }
                bExit = true;
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_8", ex.ToString());
                return;
            }

        }


        public void ShowOtherErrorMessage(string errorMsg1, string errorMsg2)
        {
            try
            {
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;
                //mainMenu.formType = "error_detail_out";

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errorMsg1, 100, 20, dialogPanel.Width - 200, dialogPanel.Height / 6, Color.FromArgb(255, 0, 88, 150), Color.FromArgb(255, 255, 242, 75), dialogPanel.Height / 14);
                TitleLb = dialogTitle;

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImageOut))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);
                pBGlobal = pBox;

                Panel errorDetailPanel = null;
                string errorDetail = "";

                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                errorDetail = errorMsg2;

                Label dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errorDetail, 20, errorDetailPanel.Bottom + 5, dialogPanel.Width * 2 / 3 - 30, 250, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleLeft);
                ContentLb = dialogInstruction;

                Button errCancelBtn = createButton.CreateButtonWithImage(constants.rectRedButton, "errCancelBtn", "エラーリセット", dialogPanel.Right - 200, dialogPanel.Bottom - 80, 150, 40, 0, 1, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(errCancelBtn);

                errCancelBtn.Click += new EventHandler(this.BackShow);
                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();

                }
                else
                {
                    dialogForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_9", ex.ToString());
                return;
            }

        }

        RoundedFormDialog printDialogForm = null;
        /**
         * @params
         * title: printer error title
         * type: printer type: usbPrinter=>0, networkPrinter=>1
         * content_index: err index: ex: connect error=>0, paper out=>1...
         * errTime: err occure time: normal status checking time=>0, printing status checking time=>1
         * */
        public void ShowPrintErrorMessage(string title, int type, int content_index, int errTime = 0, int errLocation = 0)
        {
            try
            {
                this.Width = width;
                this.Height = height;

                content_index_param = content_index;
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;

                dialogForm.Deactivate += new EventHandler(this.DeactiveForm);

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;
                printDialogForm = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;

                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                string errTitle = title + "プリンタを点検して下さい";
                Color errTitleBackColor = Color.FromArgb(255, 108, 108, 108);

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errTitle, 70, 20, dialogPanel.Width - 140, dialogPanel.Height / 6, errTitleBackColor, Color.White, dialogPanel.Height / 20);

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImage))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);

                Panel errorDetailPanel = null;
                string errorDetail = "";
                Color high_fontColor = Color.Black;

                Label dialogInstruction;
                Label dialogInstruction_2;
                Button errCancelBtn;
                Button powerBtn;
                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                errorDetail = "点検終了後【復帰】アイコンを\nクリックしてください";

                dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errorDetail, 20, errorDetailPanel.Bottom + 15, dialogPanel.Width * 2 / 3 - 30, 50, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

                errCancelBtn = createButton.CreateButtonWithImage(constants.resetButton, "errCancelBtn_" + type + "_" + errTime + "_" + errLocation, "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(errCancelBtn);

                dialogInstruction_2 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", "【終了】アイコンを長押しすると\n開始画面に戻ります", 20, errCancelBtn.Bottom + 30, dialogPanel.Width * 2 / 3 - 30, 60, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleCenter);

                powerBtn = createButton.CreateButtonWithImage(constants.shutdownButton, "printBtn_" + content_index + "_" + errLocation, "", dialogPanel.Width * 1 / 3 - 70, dialogInstruction_2.Bottom + 5, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);

                printType = title;
                dialogPanel.Controls.Add(powerBtn);


                powerBtn.Click += new EventHandler(this.ErrPowerApplication);


                errCancelBtn.Click += new EventHandler(this.ResetPrint);

                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    if(DialogFormGlobal != null)
                    {
                        DialogFormGlobal.TopLevel = false;
                        DialogFormGlobal.TopMost = false;
                    }
                    dialogForm.ShowDialog();

                }
                else
                {
                    if (DialogFormGlobal != null)
                    {
                        DialogFormGlobal.TopLevel = false;
                        DialogFormGlobal.TopMost = false;
                    }
                    dialogForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_10", ex.ToString());
                return;
            }
        }

        public void ShowPrintErrorMessageOther(int content_index)
        {
            try
            {
                RoundedFormDialog dialogForm = new RoundedFormDialog();
                dialogForm.Size = new Size(width / 2, height / 2);
                dialogForm.StartPosition = FormStartPosition.CenterParent;
                dialogForm.WindowState = FormWindowState.Normal;
                dialogForm.BackColor = Color.White;
                dialogForm.ControlBox = false;
                dialogForm.FormBorderStyle = FormBorderStyle.None;
                dialogForm.AutoScroll = false;
                dialogForm.radiusValue = 50;
                //mainMenu.formType = "error_detail_out";

                dialogForm.TopMost = true;
                dialogForm.TopLevel = true;

                DialogFormGlobal = dialogForm;
                string dialogTitleText = constants.dialogTitle;
                string dialogInstructionText = constants.dialogInstruction;


                string errTitle = "サーマルプリンタを点検して下さい。";
                Color errTitleBackColor = Color.FromArgb(255, 255, 0, 0);


                Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
                using(Bitmap img = new Bitmap(constants.roundedFormImage))
                {
                    dialogPanel.BackgroundImage = new Bitmap(img);
                }
                dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

                Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", errTitle, 70, 20, dialogPanel.Width - 140, dialogPanel.Height / 6, errTitleBackColor, Color.White, dialogPanel.Height / 21);
                TitleLb = dialogTitle;

                PictureBox pBox = new PictureBox();
                pBox.Width = dialogPanel.Width / 3 - 30;
                pBox.Height = dialogPanel.Height * 2 / 3 - 50;
                pBox.BackColor = Color.Transparent;
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Location = new Point(dialogPanel.Width * 2 / 3 + 10, dialogTitle.Bottom + 10);
                using(Bitmap img = new Bitmap(constants.errImageOut))
                {
                    pBox.Image = new Bitmap(img);
                }
                dialogPanel.Controls.Add(pBox);
                pBGlobal = pBox;

                Panel errorDetailPanel = null;

                errorDetailPanel = createPanel.CreateSubPanel(dialogPanel, 30, dialogTitle.Bottom + 10, dialogPanel.Width * 2 / 3 - 80, 0, BorderStyle.None, Color.FromArgb(255, 215, 228, 189));

                string errCodeStr = "";

                string errContent = constants.printErrMsg[content_index];


                Label dialogInstruction = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", errContent, 30, errorDetailPanel.Bottom + 20, dialogPanel.Width * 2 / 3 - 50, 150, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleLeft);
                ContentLb = dialogInstruction;

                Label dialogInstruction_1 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction_2", errCodeStr, 30, dialogInstruction.Bottom + 5, dialogPanel.Width * 2 / 3 - 50, 30, Color.Transparent, Color.Black, 14, false, ContentAlignment.MiddleCenter);


                Label dialogInstruction_2 = createLabel.CreateLabelsInPanel(dialogPanel, "dialogInstruction", "", 20, dialogPanel.Bottom - 80, dialogPanel.Width * 2 / 3 - 30, 50, Color.Transparent, Color.Black, 12, false, ContentAlignment.MiddleLeft);

                Button powerBtn = createButton.CreateButtonWithImage(constants.backStaticButton, "powerBtn", "", dialogPanel.Right - 200, dialogPanel.Bottom - 80, 150, 50, 0, 20, dialogPanel.Height / 30, FontStyle.Regular, Color.White, ContentAlignment.MiddleCenter, 1);
                dialogPanel.Controls.Add(powerBtn);

                powerBtn.MouseDown += new MouseEventHandler(this.BackShow);

                if (this.InvokeRequired)
                {
                    var owner = Application.OpenForms["Form1"];
                    dialogForm.Load += delegate {
                        // NOTE: just as a workaround for the Owner bug!!
                        Control.CheckForIllegalCrossThreadCalls = false;
                        dialogForm.Owner = owner;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        owner.BeginInvoke(new Action(() => owner.Enabled = false));

                    };
                    dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                        if (!ea.Cancel)
                        {
                            owner.Invoke(new Action(() => owner.Enabled = true));
                            dialogForm.Owner = null;
                        }
                    });
                    dialogForm.ShowDialog();

                }
                else
                {
                    dialogForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_10", ex.ToString());
                return;
            }
        }

        public void ShutDownApp(object sender, EventArgs e)
        {
            if (DialogFormGlobal != null)
            {
                DialogFormGlobal.Close();
                DialogFormGlobal.Hide();
                DialogFormGlobal = null;
            }
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                if (key.GetValue("checkingState") != null)
                {
                    key.SetValue("checkingState", 0);
                }
                else
                {
                    key.SetValue("checkingState", 0);
                }
                key.SetValue("userRole", "0");
                key.SetValue("HDCheck", true);

            }
            if (mainMenu != null)
            {
                mainMenu.HDChecking = false;
                mainMenu.CloseComport();
                mainMenu = null;
            }

            constants.SaveLogData("messageDialog_11", "shutdown");

            Shutdown_windows();
        }

        public void RestartApp(object sender, EventArgs e)
        {
            if (DialogFormGlobal != null)
            {
                DialogFormGlobal.Close();
                DialogFormGlobal.Dispose();
                DialogFormGlobal = null;
            }
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("checkingState", 0);
                key.SetValue("userRole", "0");
                key.SetValue("HDCheck", true);

            }
            if (mainMenu != null)
            {
                mainMenu.HDChecking = false;
                mainMenu.CloseComport();
                mainMenu = null;
            }

            constants.SaveLogData("messageDialog_12", "restart");
            Restart_windows();
        }



        public void Shutdown_windows()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("isAdmin", false);
            }

            Process.Start("shutdown", "/s /t 0");
        }

        public void Restart_windows()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            if (key != null)
            {
                key.SetValue("isAdmin", false);
            }
            Process.Start("shutdown", "/r /t 0");
        }

        private void ResetPrint(object sender, EventArgs e)
        {
            try
            {
                printDialogForm.Close();
                printDialogForm.Visible = false;
                printDialogForm = null;
                Button tempBtn = (Button)sender;
                int printType = int.Parse(tempBtn.Name.Split('_')[1]);
                int errTime = int.Parse(tempBtn.Name.Split('_')[2]);
                int errLocation = int.Parse(tempBtn.Name.Split('_')[3]);

                if (errLocation == 1)
                {
                    if (errTime == 1)
                    {
                        if (saleScreenGlobal.InvokeRequired)
                        {
                            saleScreenGlobal.Invoke((MethodInvoker)delegate
                            {
                                if (printType == 0)
                                {
                                    saleScreenGlobal.USBPrint();
                                }
                                else
                                {
                                    saleScreenGlobal.KichenPrint();
                                }
                            });
                        }
                        else
                        {
                            if (printType == 0)
                            {
                                saleScreenGlobal.USBPrint();
                            }
                            else
                            {
                                saleScreenGlobal.KichenPrint();
                            }
                        }
                    }
                }

                if (DialogFormGlobal != null)
                {
                    DialogFormGlobal.Activate();
                    DialogFormGlobal.TopMost = true;
                    DialogFormGlobal.TopLevel = true;
                }
                    
            }
            catch (Exception ex)
            {
                constants.SaveLogData("messageDialog_12", ex.ToString());
                return;
            }
        }

        private void BackShow(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
            DialogFormGlobal.Visible = false;
            DialogFormGlobal = null;
            if (errorHandler != null && saleScreenGlobal != null)
            {
                saleScreenGlobal.OrderCancelRun();
            }
        }

        private void ResetComModule(object sender, EventArgs e)
        {
            if (DialogFormGlobal != null)
            {
                bExit = false;
                if(saleScreenGlobal != null)
                    saleScreenGlobal.bBackShow = false;
                DialogFormGlobal.Dispose();
                DialogFormGlobal = null;

                if (printDialogForm != null)
                {
                    printDialogForm.Activate();
                    printDialogForm.TopMost = true;
                    printDialogForm.TopLevel = true;
                }
            }
        }

        private void MainResetComModule(object sender, EventArgs e)
        {
            if(DialogFormGlobal != null)
            {
                DialogFormGlobal.Visible = false;
                DialogFormGlobal.Close();
                DialogFormGlobal = null;
            }
            if (mainMenu != null)
                mainMenu.ResetModule();
        }
        private void BackShowMenuReading(object sender, EventArgs e)
        {
            if( DialogFormGlobal != null)
            {
                DialogFormGlobal.Close();
                DialogFormGlobal = null;
            }
            menuReading.BackShowStart();
        }

        private void CategoryPrintView(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
            categoryList.PrintPreview_click();
        }

        private void GroupPrintView(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
            groupList.Btnprintpreview_Click();
        }

        public void ShowDBErrorMessage()
        {
            Form dialogForm = new Form();
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.FromArgb(255, 255, 153, 204));

            Label messageLabel1 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", constants.dbErrorTitle, 50, 30, mainPanel.Width - 100, (mainPanel.Height - 150) / 2, Color.Red, Color.White, constants.fontSizeBig, false, ContentAlignment.MiddleCenter);
            messageLabel1.Padding = new Padding(30, 0, 30, 0);
            Label messageLabel2 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel2", constants.dbErrorContent, 50, messageLabel1.Bottom, mainPanel.Width - 100, (mainPanel.Height - 190) * 1 / 2, Color.Transparent, Color.Black, constants.fontSizeMedium, false, ContentAlignment.MiddleCenter);
            messageLabel2.Padding = new Padding(30, 0, 30, 0);


            Button closeButton = createButton.CreateButton(constants.noStr, "closeButton", mainPanel.Width / 2 - 75, mainPanel.Height - 100, 150, 50, Color.Red, Color.Transparent, 0, 1);
            closeButton.ForeColor = Color.White;
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
            //dialogForm.Show();
        }

        public void CancelOrderMessage(object sender, EventArgs e)
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            DialogFormGlobal = dialogForm;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.White);

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", constants.cancelErrorMessage, 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.White, Color.Black, 18, false, ContentAlignment.MiddleCenter);
            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.yesStr, mainPanel.Width / 2 - 75, mainPanel.Height - 100, 150, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);

            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        public void OrderOutMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", constants.orderOutMsg, 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.confirmLabel, mainPanel.Width / 2 - 50, mainPanel.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        Thread sumThreadmm;
        public void SummingMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 4);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", "ただいま締め処理を行っています。\n暫くお待ちください。", 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);
            dialogForm.FormClosed += new FormClosedEventHandler(CloseFormHandle);

            //Button closeButton = createButton.CreateButtonWithImage(Image.FromFile(constants.rectBlueButton), "closeButton", constants.confirmLabel, mainPanel.Width / 2 - 50, mainPanel.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            //mainPanel.Controls.Add(closeButton);
            //closeButton.Click += new EventHandler(this.BackShow);

            sumThreadmm = new Thread(ClosingProc);
            sumThreadmm.SetApartmentState(ApartmentState.STA);
            sumThreadmm.Start();

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        private void CloseFormHandle(object sender, FormClosedEventArgs e)
        {
            if(sumThreadmm != null)
            {
                sumThreadmm.Abort();
                sumThreadmm = null;
            }
        }

        DBClass dbClass = new DBClass();

        private void ClosingProc()
        {
            Thread.Sleep(2000);
            dbClass.ClosingProcessWork();
            Thread.Sleep(2000);

            if (DialogFormGlobal.InvokeRequired)
            {
                DialogFormGlobal.Invoke((MethodInvoker)delegate
                {
                    DialogFormGlobal.Close();
                    DialogFormGlobal.Visible = false;
                    DialogFormGlobal = null;
                });
            }
            else
            {
                DialogFormGlobal.Close();
                DialogFormGlobal.Visible = false;
                DialogFormGlobal = null;
            }
        }

        public void LogSaveMessage(string msg)
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 4);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", msg, 40, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.confirmLabel, mainPanel.Width / 2 - 50, mainPanel.Height - 70, 100, 40, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        public void RestEmptyMessage()
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 4);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel", constants.restEmptyMessage, 50, 0, mainPanel.Width - 100, mainPanel.Height - 100, Color.Transparent, Color.Black, 18, false, ContentAlignment.MiddleCenter);

            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.confirmLabel, mainPanel.Width / 2 - 50, mainPanel.Height - 80, 100, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        public void BackupRestore(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            backupIndicator = btnTemp.Name;
            string message1 = constants.backupLabel;
            string message2 = constants.backupDialogText;
            if (backupIndicator == "restoreBtn")
            {
                message1 = constants.restoreLabel;
                message2 = constants.restoreDialogText;
            }
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel1 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", message1, 50, 10, mainPanel.Width / 2, 60, Color.Transparent, Color.Red, 18, false, ContentAlignment.BottomCenter);
            messageLabel1.Padding = new Padding(30, 0, 30, 0);
            Label messageLabel2 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel2", message2, 50, (mainPanel.Height - 100) / 5, mainPanel.Width - 100, (mainPanel.Height - 100) * 4 / 5, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
            messageLabel2.Padding = new Padding(30, 0, 30, 0);

            Button runButton = createButton.CreateButtonWithImage(constants.rectRedButton, "runButton", constants.yesStr, mainPanel.Width / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(runButton);
            runButton.Click += new EventHandler(this.RunBackupRestore);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.cancelButtonText, mainPanel.Width * 5 / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        public void BackupRestoreError(string title, string errors)
        {
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width * 2 / 5, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;

            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;

            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }
            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label messageLabel1 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", title, 50, 20, mainPanel.Width / 2 + 30, 60, Color.Transparent, Color.Red, 18, false, ContentAlignment.BottomCenter);
            messageLabel1.Padding = new Padding(30, 0, 30, 0);

            Label messageLabel2 = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel2", errors, 50, (mainPanel.Height - 100) / 5, mainPanel.Width - 100, (mainPanel.Height - 100) * 4 / 5, Color.Transparent, Color.Black, 16, false, ContentAlignment.MiddleLeft);
            messageLabel2.Padding = new Padding(30, 0, 30, 0);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", "OK", mainPanel.Width * 3 / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            if (this.InvokeRequired)
            {
                var owner = Application.OpenForms["Form1"];
                dialogForm.Load += delegate {
                    // NOTE: just as a workaround for the Owner bug!!
                    Control.CheckForIllegalCrossThreadCalls = false;
                    dialogForm.Owner = owner;
                    Control.CheckForIllegalCrossThreadCalls = true;
                    owner.BeginInvoke(new Action(() => owner.Enabled = false));

                };
                dialogForm.FormClosing += new FormClosingEventHandler((s, ea) => {
                    if (!ea.Cancel)
                    {
                        owner.Invoke(new Action(() => owner.Enabled = true));
                        dialogForm.Owner = null;
                    }
                });
                dialogForm.ShowDialog();

            }
            else
            {
                dialogForm.ShowDialog();
            }
        }

        private void RunBackupRestore(object sender, EventArgs e)
        {
            DialogFormGlobal.Close();
            if (backupIndicator == "backupBtn")
            {
                backRestore.DataBackup();
            }
            else
            {
                backRestore.DataRestore();
            }
        }

        PasswordInput passwordInput = null;
        private void CloseErrorDialog(object sender, EventArgs e)
        {
            Button temp = (Button)sender;
            passwordInput = new PasswordInput();
            passwordInput.InitMessageDialog(this);

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            passwordInput.CreateNumberInputDialog("errorDialog", temp.Name);
            passwordInput.dialogFormGlobal.ShowDialog();
        }

        /** 
         * Sale page Close when clicked F1
         **/
        private void FormKeyReturnEventHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                DialogFormGlobal.Close();
                if (saleScreenGlobal != null)
                {
                    saleScreenGlobal.BackShow();
                }
            }
        }

        public void GetPassword(string objectName, string passwords)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinRegistry");
            string pwd = "";
            if (key != null && key.GetValue("POSPassword") != null)
            {
                pwd = key.GetValue("POSPassword").ToString();
            }
            else if (key == null)
            {
                pwd = "";
            }
            if(passwordInput != null && passwordInput.dialogFormGlobal != null)
                passwordInput.dialogFormGlobal.Close();
            if (pwd == passwords)
            {
                if (this.DialogFormGlobal.InvokeRequired)
                {
                    this.DialogFormGlobal.Invoke((MethodInvoker)delegate
                    {
                        this.DialogFormGlobal.Close();
                    });
                }
                else
                {
                    this.DialogFormGlobal.Close();
                }

            }
        }

    }
}
