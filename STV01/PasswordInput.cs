using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    class PasswordInput
    {
        readonly int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        readonly int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        Constant constants = new Constant();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CustomButton createButton = new CustomButton();
        public Form dialogFormGlobal = null;
        TextBox inputValueGlobal = null;
        MainMenu mainMenuGlobal = null;
        SaleScreen saleScreenGlobal = null;
        MessageDialog messageDialogGlobal = null;

        string objectNameGlobal = "";
        string objectHandlerNameGlobal = "";

        public PasswordInput()
        {
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

            Panel mainPanel = createPanel.CreateMainPanel(dialogForm, 0, 0, dialogForm.Width, dialogForm.Height, BorderStyle.None, Color.FromArgb(255, 245, 219, 203));

            Label inputTitle = createLabel.CreateLabelsInPanel(mainPanel, "inputTitle", constants.passwordInputTitle, 0, 0, mainPanel.Width, 60, Color.Transparent, Color.Black, 22, false, ContentAlignment.BottomCenter);

            TextBox inputValueShow = new TextBox();
            inputValueShow.Location = new Point(mainPanel.Width / 6, 70);
            inputValueShow.Size = new Size(mainPanel.Width * 2 / 3, 50);
            inputValueShow.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            inputValueShow.BorderStyle = BorderStyle.None;
            inputValueShow.TextAlign = HorizontalAlignment.Right;
            mainPanel.Controls.Add(inputValueShow);
            inputValueGlobal = inputValueShow;

            Panel keyboardPanel = createPanel.CreateSubPanel(mainPanel, mainPanel.Width / 9, inputValueShow.Bottom + 10, mainPanel.Width * 7 / 9, mainPanel.Height - inputValueShow.Bottom - 40, BorderStyle.None, Color.Transparent);

            for (int k = 3; k >= 0; k--)
            {
                if (k != 0)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        int keyValue = 3 * (3 - k) + (1 + m);
                        string btnImage = constants.keyboardButtonImage;
                        Button numberKeyButton = createButton.CreateButtonWithImage(btnImage, keyValue.ToString(), keyValue.ToString(), keyboardPanel.Width * m / 3, keyboardPanel.Height * (3 - k) / 4, keyboardPanel.Width / 3, keyboardPanel.Height / 4, 1, 1, 18, FontStyle.Bold, Color.Black);
                        keyboardPanel.Controls.Add(numberKeyButton);
                        numberKeyButton.Click += new EventHandler(this.InputValueAdd);
                    }
                }
                else
                {
                    string btnImage = constants.keyboardButtonImage;
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
        }

        public void InitMainMenu(MainMenu sendHandler)
        {
            mainMenuGlobal = sendHandler;
        }
        public void InitSaleScreen(SaleScreen sendHandler)
        {
            saleScreenGlobal = sendHandler;
        }
        public void InitMessageDialog(MessageDialog sendHandler)
        {
            messageDialogGlobal = sendHandler;
        }

        public void CreateNumberInputDialog(string objectName, string objectHandlerName)
        {
            objectHandlerNameGlobal = objectHandlerName;
            objectNameGlobal = objectName;
        }

        private void GetFocus(object sender, EventArgs e)
        {
            TextBox tbBox = (TextBox)sender;
            inputValueGlobal = tbBox;
            inputValueGlobal.SelectionLength = 0;

        }

        private void InputValueAdd(object sender, EventArgs e)
        {
            Button keyLabel = (Button)sender;
            string keyText = keyLabel.Name;
            if (keyText != "Del" && keyText != "Ok")
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
                else
                {
                    string sendText = inputValueGlobal.Text;

                    switch (objectNameGlobal)
                    {
                        case "maintenance":
                            mainMenuGlobal.GetPassword(objectNameGlobal, sendText);
                            break;
                        case "readingmenu":
                            mainMenuGlobal.GetPassword(objectNameGlobal, sendText);
                            break;
                        case "errorDialog":
                            messageDialogGlobal.GetPassword(objectNameGlobal, sendText);
                            break;
                    }
                }
            }
        }

    }
}
