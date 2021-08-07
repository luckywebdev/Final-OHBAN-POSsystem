using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace STV01
{
    class OrderDialog: Form
    {
        readonly int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        readonly int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton createButton = new CustomButton();
        public RoundedFormDialog DialogFormGlobal;
        SaleScreen pSaleScreen = null;

        public void InitValue(SaleScreen saleHandler)
        {
            pSaleScreen = saleHandler;

        }
        public Form DialogFormGlobal_2 = null;
        int rowCounter = 0;
        public void ShowTicketingDetail(string[] orderProductNameArray, string[] orderProductPriceArray, string[] orderAmountArray, int orderRowNum)
        {
            rowCounter = orderRowNum;
            //Panel panelTemp = (Panel)sender;
            RoundedFormDialog dialogForm = new RoundedFormDialog();
            dialogForm.Size = new Size(width / 2, 300 + 30 * orderRowNum);
            dialogForm.StartPosition = FormStartPosition.Manual;
            dialogForm.Location = new Point(width / 4, height / 3 - 100 - 15 * orderRowNum);
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.BackColor = Color.White;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.AutoScroll = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.radiusValue = 50;
            DialogFormGlobal = dialogForm;
            DialogFormGlobal.TopLevel = true;
            string dialogTitleText = constants.orderDialogRunText;
            string dialogInstructionText = constants.dialogInstruction;


            Panel dialogPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);
            using (Bitmap img = new Bitmap(constants.roundedFormImage))
            {
                dialogPanel.BackgroundImage = new Bitmap(img);
            }

            dialogPanel.BackgroundImageLayout = ImageLayout.Stretch;

            Label dialogTitle = createLabel.CreateLabelsInPanel(dialogPanel, "dialogTitle", dialogTitleText, 30, 20, dialogPanel.Width - 60, 50, Color.FromArgb(255, 0, 88, 150), Color.FromArgb(255, 255, 242, 75), 30);

            int totalAmount = 0;
            int totalPrice = 0;
            for(int k = 0; k < orderRowNum; k++)
            {
                int orderPrice = int.Parse(orderProductPriceArray[k]) * int.Parse(orderAmountArray[k]);
                totalPrice += orderPrice;
                totalAmount += int.Parse(orderAmountArray[k]);
                FlowLayoutPanel orderRowPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 20, dialogTitle.Bottom + 20 + k * 30, dialogPanel.Width - 40, 30, Color.Transparent, new Padding(dialogPanel.Width / 15, 0, 0, 0));
               // orderRowPanel.BorderStyle = BorderStyle.FixedSingle;
                Label prdNameLabel = createLabel.CreateLabels(orderRowPanel, "productName_" + k, orderProductNameArray[k], 0, 0, orderRowPanel.Width / 3, orderRowPanel.Height, Color.Transparent, Color.Black, orderRowPanel.Height / 3, false, ContentAlignment.MiddleLeft);
                Label prdAmountLabel = createLabel.CreateLabels(orderRowPanel, "productAmount_" + k, "x" + orderAmountArray[k], orderRowPanel.Width / 3 + 10, 0, orderRowPanel.Width / 5 - 5, orderRowPanel.Height, Color.Transparent, Color.Black, orderRowPanel.Height / 3, false, ContentAlignment.MiddleRight);
                Label prdPriceLabel = createLabel.CreateLabels(orderRowPanel, "productPrice_" + k, orderPrice.ToString() + constants.unit, 0, prdAmountLabel.Right + 5, orderRowPanel.Width / 3 - 10, orderRowPanel.Height, Color.Transparent, Color.Black, orderRowPanel.Height / 3, false, ContentAlignment.MiddleRight);
            }

            PictureBox pB = new PictureBox();
            pB.Location = new Point(0, dialogTitle.Bottom + 20 + orderRowNum * 30);
            pB.Size = new Size(dialogPanel.Width, 5);
            dialogPanel.Controls.Add(pB);
            Bitmap image = new Bitmap(pB.Size.Width, pB.Size.Height);
            Graphics g = Graphics.FromImage(image);
            g.DrawLine(new Pen(Color.FromArgb(255, 142, 133, 118), 3), 0, 2, dialogPanel.Width, 2);
            pB.Image = image;

            FlowLayoutPanel totalRowPanel = createPanel.CreateFlowLayoutPanel(dialogPanel, 0, pB.Bottom + 10, dialogPanel.Width, 50, Color.Transparent, new Padding(dialogPanel.Width / 15, 0, 0, 0));

            Label totalNameLabel = createLabel.CreateLabels(totalRowPanel, "totalName", constants.sumLabel, 0, 0, totalRowPanel.Width / 3, totalRowPanel.Height, Color.Transparent, Color.Black, totalRowPanel.Height / 3, false, ContentAlignment.MiddleLeft);
            Label totalAmountLabel = createLabel.CreateLabels(totalRowPanel, "totalAmount", totalAmount + constants.amountUnit1, totalRowPanel.Width / 3 + 10, 0, totalRowPanel.Width / 5 - 5, totalRowPanel.Height, Color.Transparent, Color.Black, totalRowPanel.Height / 3, false, ContentAlignment.MiddleRight);
            Label totalPriceLabel = createLabel.CreateLabels(totalRowPanel, "totalPrice", totalPrice.ToString() + constants.unit, 0, totalAmountLabel.Right + 5, totalRowPanel.Width / 3 - 10, totalRowPanel.Height, Color.Transparent, Color.Black, totalRowPanel.Height / 3, false, ContentAlignment.MiddleRight);

            Button ticketingButton = createButton.CreateButtonWithImage(constants.cancelButton, "ticketingButton", constants.ticketingButtonText, dialogPanel.Width / 2 - 85, dialogPanel.Height - 90, 160, 60, 0, 20, 20, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 2);
            dialogPanel.Controls.Add(ticketingButton);
            ticketingButton.Click += new EventHandler(this.TicketingRun);

            Button productCancelBtn = createButton.CreateButtonWithImage(constants.rectBlueButton, "productCancelBtn", constants.backText, dialogPanel.Width - 170, dialogPanel.Height - 85, 120, 55, 0, 20, 18, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            dialogPanel.Controls.Add(productCancelBtn);

            productCancelBtn.Click += new EventHandler(this.BackShow);

            dialogForm.ShowDialog();

        }

        public void DeleteDialog(int selectedIndex, string prdName, int prdAmount)
        {
            RoundedFormDialog dialogForm = new RoundedFormDialog();
            dialogForm.Size = new Size(width / 3, height / 4);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.BackColor = Color.White;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.AutoScroll = false;
            dialogForm.radiusValue = 50;
            DialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            using (Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }

            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            string msgText = prdName + " " + prdAmount.ToString() + " 品 \n削除しますか？";
            Label messageLabel = createLabel.CreateLabelsInPanel(mainPanel, "messageLabel1", msgText, 50, 50, mainPanel.Width - 100, (mainPanel.Height - 100) / 2, Color.Transparent, Color.Red, 18, false, ContentAlignment.MiddleCenter);
            messageLabel.Padding = new Padding(30, 0, 30, 0);

            Button delButton = createButton.CreateButtonWithImage(constants.rectRedButton, "del_" + selectedIndex.ToString(), constants.deleteText, mainPanel.Width / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(delButton);
            delButton.Click += new EventHandler(this.OrderDelete);

            Button closeButton = createButton.CreateButtonWithImage(constants.rectBlueButton, "closeButton", constants.backText, mainPanel.Width * 5 / 8, mainPanel.Height - 100, mainPanel.Width / 4, 50, 0, 1, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            mainPanel.Controls.Add(closeButton);
            closeButton.Click += new EventHandler(this.BackShow);

            dialogForm.ShowDialog();

        }

        public void TicketingRunDialog()
        {

            RoundedFormDialog dialogForm = new RoundedFormDialog();
            dialogForm.Size = new Size(width / 3 + 100, height / 3);
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            //dialogForm.StartPosition = FormStartPosition.Manual;
            //dialogForm.Location = new Point(width / 3 - 50, height / 2 + 50 + 15 * rowCounter);
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.BackColor = Color.White;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.AutoScroll = false;
            dialogForm.TopLevel = true;
            dialogForm.TopMost = true;

            dialogForm.radiusValue = 50;
            //dialogForm.Owner = pSaleScreen.Owner;
            DialogFormGlobal_2 = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.Transparent);

            using (Bitmap img = new Bitmap(constants.ticketingImage))
            {
                mainPanel.BackgroundImage = new Bitmap(img);
            }

            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;

            dialogForm.Shown += new EventHandler(TicketingRuning);

            dialogForm.ShowDialog();
        }

        private void TicketingRun(object sender, EventArgs e)
        {
            //DialogFormGlobal.Close();
            //DialogFormGlobal.Visible = false;
            //DialogFormGlobal = null;

            TicketingRunDialog();
        }

        private void TicketingRuning(object sender, EventArgs e)
        {
            //DialogFormGlobal.Close();
            pSaleScreen.Ticketing();
        }

        private void OrderDelete(object sender, EventArgs e)
        {
            Button tempBtn = (Button)sender;
            string[] btnName = tempBtn.Name.Split('_');
            int selectedIndex = int.Parse(btnName[1]);

            pSaleScreen.OrderDeleteRun(selectedIndex);
            DialogFormGlobal.Close();
        }

        private void BackShow(object send, EventArgs e)
        {
            DialogFormGlobal.Close();
            DialogFormGlobal.Visible = false;
            DialogFormGlobal = null;
        }

    }
}
