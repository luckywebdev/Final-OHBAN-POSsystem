using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class NumberInput
    {
        readonly int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        readonly int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton createButton = new CustomButton();
        Form dialogFormGlobal = null;
        TextBox inputValueGlobal = null;
        SoldoutSetting1 soldoutSetting1Global = null;
        ProductItemManagement productGlobal = null;
        Label alertLabelGlobal = null;
        string objectNameGlobal = "";
        string objectHandlerNameGlobal = "";

        public void InitSoldoutSetting(SoldoutSetting1 sendHandler)
        {
            soldoutSetting1Global = sendHandler;
        }
        public void InitProductManage(ProductItemManagement sendHandler)
        {
            productGlobal = sendHandler;
        }


        public void CreateNumberInputDialog(string objectName, int limitAmount, string objectHandlerName)
        {
            objectHandlerNameGlobal = objectHandlerName;
            objectNameGlobal = objectName;
            Form dialogForm = new Form();
            dialogForm.Size = new Size(width / 4, height * 2 / 5 + 30);
            dialogForm.BackColor = Color.White;
            dialogForm.StartPosition = FormStartPosition.CenterParent;
            dialogForm.WindowState = FormWindowState.Normal;
            dialogForm.ControlBox = false;
            dialogForm.FormBorderStyle = FormBorderStyle.None;
            dialogForm.TopMost = true;
            dialogForm.TopLevel = true;
            dialogFormGlobal = dialogForm;

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.FixedSingle, Color.FromArgb(255, 245, 219, 203));

            TextBox inputValueShow = new TextBox();
            inputValueShow.Location = new Point(mainPanel.Width / 6, 30);
            inputValueShow.Size = new Size(mainPanel.Width * 2 / 3, 50);
            inputValueShow.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            inputValueShow.BorderStyle = BorderStyle.None;
            inputValueShow.TextAlign = HorizontalAlignment.Right;
            mainPanel.Controls.Add(inputValueShow);
            inputValueShow.TextChanged += new EventHandler(this.TextChangeHandler);
            inputValueGlobal = inputValueShow;
            if(objectName == "soldoutSetting1")
            {
                Label notifyLabel = createLabel.CreateLabelsInPanel(mainPanel, "notifyLabel", "限定数を入力 0～200 \n 0:無制限", mainPanel.Width / 9, inputValueShow.Bottom + 10, mainPanel.Width * 7 / 9, 40, Color.Transparent, Color.Violet, 12, false, ContentAlignment.TopLeft);
                inputValueShow.Text = limitAmount.ToString();
                inputValueShow.SelectionStart = inputValueShow.Text.Length;
                inputValueShow.SelectionLength = 0;

                Label alertLabel = createLabel.CreateLabelsInPanel(mainPanel, "alertLabel", "設定範囲を超えています。", mainPanel.Width / 9, notifyLabel.Bottom + 5, mainPanel.Width * 7 / 9, 30, Color.Transparent, Color.Red, 12, false, ContentAlignment.TopCenter);
                alertLabelGlobal = alertLabel;
                alertLabel.Hide();
            }
            else if(objectName == "productManagement")
            {
                inputValueShow.Text = limitAmount.ToString();
                inputValueShow.SelectionStart = inputValueShow.Text.Length;
                inputValueShow.SelectionLength = 0;
            }

            Panel keyboardPanel = createPanel.CreateSubPanel(mainPanel, mainPanel.Width / 9, inputValueShow.Bottom + 70, mainPanel.Width * 7 / 9, mainPanel.Height - inputValueShow.Bottom - 100, BorderStyle.None, Color.Transparent);
            string btnImage = constants.keyboardButtonImage;

            for (int k = 3; k >= 0; k--)
            {
                if(k != 0)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        int keyValue = 3 * k - (2 - m);
                        Button numberKeyButton = createButton.CreateButtonWithImage(btnImage, keyValue.ToString(), keyValue.ToString(), keyboardPanel.Width * m / 3, keyboardPanel.Height * (3 - k) / 4, keyboardPanel.Width / 3, keyboardPanel.Height / 4, 1, 1, 18, FontStyle.Bold, Color.Black);
                        keyboardPanel.Controls.Add(numberKeyButton);
                        numberKeyButton.Click += new EventHandler(this.InputValueAdd);
                    }
                }
                else
                {
                    Button numberKeyButton = createButton.CreateButtonWithImage(btnImage, "0", "0", 0, keyboardPanel.Height * (3 - k) / 4, keyboardPanel.Width / 3, keyboardPanel.Height / 4, 1, 1, 18, FontStyle.Bold, Color.Black);
                    keyboardPanel.Controls.Add(numberKeyButton);
                    numberKeyButton.Click += new EventHandler(this.InputValueAdd);

                    Button numberKeyButton_del = createButton.CreateButtonWithImage(btnImage, "Del", "Del", keyboardPanel.Width / 3, keyboardPanel.Height * (3 - k) / 4, keyboardPanel.Width / 3, keyboardPanel.Height / 4, 1, 1, 18, FontStyle.Bold, Color.Black);
                    keyboardPanel.Controls.Add(numberKeyButton_del);
                    numberKeyButton_del.Click += new EventHandler(this.InputValueAdd);

                    Button numberKeyButton_ok = createButton.CreateButtonWithImage(btnImage, "Ok", "Ok", keyboardPanel.Width * 2 / 3, keyboardPanel.Height * (3 - k) / 4, keyboardPanel.Width / 3, keyboardPanel.Height / 4, 1, 1, 18, FontStyle.Bold, Color.Black);
                    keyboardPanel.Controls.Add(numberKeyButton_ok);
                    numberKeyButton_ok.Click += new EventHandler(this.InputValueAdd);
                }
            }

            dialogForm.ShowDialog();
        }

        private void GetFocus(object sender, EventArgs e)
        {
            TextBox tbBox = (TextBox)sender;
            inputValueGlobal = tbBox;
            inputValueGlobal.SelectionLength = 0;

        }

        private void TextChangeHandler(object sender, EventArgs e)
        {
            TextBox textCtl = (TextBox)sender;
            if(objectNameGlobal == "soldoutSetting1")
            {
                if (textCtl.Text != "" && int.Parse(textCtl.Text) > 200)
                {
                    alertLabelGlobal.Show();
                }
                else
                {
                    if(alertLabelGlobal != null)
                    {
                        alertLabelGlobal.Hide();
                    }
                }
            }
        }

        private void InputValueAdd(object sender, EventArgs e)
        {
            Button keyLabel = (Button)sender;
            string keyText = keyLabel.Name;
            if(keyText != "Del" && keyText != "Ok")
            {
                int selectionIndex = inputValueGlobal.SelectionStart;
                inputValueGlobal.Text = inputValueGlobal.Text.Insert(selectionIndex, keyText);
                inputValueGlobal.Focus();
                inputValueGlobal.SelectionStart = selectionIndex + 1;
                inputValueGlobal.SelectionLength = 0;

            }
            else
            {
                if (keyText == "Del")
                {
                    inputValueGlobal.Text = "";
                }
                else if(inputValueGlobal.Text != "")
                {
                    switch (objectNameGlobal)
                    {
                        case "soldoutSetting1":
                            string sendText = int.Parse(inputValueGlobal.Text).ToString();
                            if (int.Parse(inputValueGlobal.Text) > 200)
                                sendText = "0";

                            soldoutSetting1Global.SetLimitationValue(sendText);
                            dialogFormGlobal.Close();
                            break;
                        case "productManagement":
                            string sendTexts = int.Parse(inputValueGlobal.Text).ToString();
                            productGlobal.SetPrice(sendTexts);
                            dialogFormGlobal.Close();
                            break;
                    }
                }
            }
        }


    }
}
