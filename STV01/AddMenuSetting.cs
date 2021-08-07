using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STV01
{
    public partial class AddMenuSetting : Form
    {
        Constant constants = new Constant();
        CustomButton customButton = new CustomButton();
        CreateTextBox createTextBox = new CreateTextBox();
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        Panel titleListPn = null;
        Panel optionListPn = null;
        TextBox titleTxtBx = null;
        TextBox optionTxtBx = null;
        ProductInfoSetting prG = null;
        int productid = 0;
        int selectedTitle = -1;
        int selectedValue = -1;

        public AddMenuSetting(int productId)
        {
            InitializeComponent();
            this.Size = new Size(600, 500);
            using(Bitmap img = new Bitmap(constants.dialogFormImage))
            {
                this.BackgroundImage = new Bitmap(img);
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.ControlBox = false;
            this.TopLevel = true;
            this.TopMost = true;

            productid = productId;

            Panel leftPn = createPanel.CreateMainPanel(this, 0, 0, this.Width / 2 - 10, this.Height, BorderStyle.None, Color.Transparent);
            Panel rightPn = createPanel.CreateMainPanel(this, this.Width / 2 + 10, 0, this.Width / 2 - 10, this.Height, BorderStyle.None, Color.Transparent);

            Label titleLb = createLabel.CreateLabelsInPanel(leftPn, "titleLb", "タイトル", 20, 50, leftPn.Width - 40, 30, Color.Transparent, Color.Black, 18);

            titleTxtBx = createTextBox.CreateTextBoxs_panel(leftPn, "titleTxtBx", 30, titleLb.Bottom + 10, 150, 30, 18, BorderStyle.FixedSingle);

            Button saveBtn = customButton.CreateButtonWithImage(constants.addButton, "saveBtn", "", titleTxtBx.Right + 5, titleLb.Bottom + 13, 50, 30, 0, 1, 12, FontStyle.Bold, Color.White);
            leftPn.Controls.Add(saveBtn);
            saveBtn.Click += new EventHandler(SaveOptionTitle);
            titleListPn = createPanel.CreateSubPanel(leftPn, 0, titleTxtBx.Bottom + 10, leftPn.Width, leftPn.Height - titleTxtBx.Bottom - 80, BorderStyle.None, Color.Transparent);


            Label optionLb = createLabel.CreateLabelsInPanel(rightPn, "optionLb", "種　類", 20, 50, rightPn.Width - 40, 30, Color.Transparent, Color.Black, 18);

            optionTxtBx = createTextBox.CreateTextBoxs_panel(rightPn, "optionTxtBx", 20, optionLb.Bottom + 10, 150, 30, 18, BorderStyle.FixedSingle);

            Button saveBtn_2 = customButton.CreateButtonWithImage(constants.addButton, "saveBtn_2", "", optionTxtBx.Right + 5, optionLb.Bottom + 13, 50, 30, 0, 1, 12, FontStyle.Bold, Color.White);
            rightPn.Controls.Add(saveBtn_2);
            saveBtn_2.Click += new EventHandler(SaveOptionValue);
            optionListPn = createPanel.CreateSubPanel(rightPn, 0, optionTxtBx.Bottom + 10, rightPn.Width, rightPn.Height - optionTxtBx.Bottom - 80, BorderStyle.None, Color.Transparent);

            OptionTitleLoad();

            Button backBtn = customButton.CreateButtonWithImage(constants.backStaticButton, "backBtn", "", rightPn.Width - 120, optionListPn.Bottom + 10, 90, 40, 0, 1, 12, FontStyle.Bold, Color.White);
            rightPn.Controls.Add(backBtn);
            backBtn.Click += new EventHandler(BackShow);

        }

        public void InitialProductInfo(ProductInfoSetting pr)
        {
            prG = pr;
        }

        List<TextBox> titleTB = new List<TextBox>();
        List<TextBox> valueTB = new List<TextBox>();

        private void OptionTitleLoad()
        {
            titleTB.Clear();
            titleListPn.Controls.Clear();
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            string qry = "SELECT * FROM " + constants.tbNames[15] + " WHERE productID=@productID ORDER BY id";
            sqlite_cmd.CommandText = qry;
            sqlite_cmd.Parameters.AddWithValue("productID", productid);
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            int k = 0;
            while (sqlite_datareader.Read())
            {
                if (!sqlite_datareader.IsDBNull(0))
                {
                    int id = sqlite_datareader.GetInt32(0);
                    string optionTitle = sqlite_datareader.GetString(2);
                    Color fClr = Color.Black;
                    if(selectedTitle == id)
                    {
                        fClr = Color.Red;
                    }

                    FlowLayoutPanel titleFlowPn = createPanel.CreateFlowLayoutPanel(titleListPn, 30, 10 + k * 35, 120, 30, Color.Transparent, new Padding(0));
                    titleFlowPn.Name = "titleFlowPn_" + id + "_" + k;
                    titleFlowPn.Click += new EventHandler(SelectOptionTitle);

                    TextBox titleLb = createTextBox.CreateTextBoxs_panel(titleFlowPn, "titleLb_" + id + "_" + k, 0, 0, 110, 25, 12, BorderStyle.None, optionTitle);
                    titleLb.Enabled = false;
                    titleLb.TextAlign = HorizontalAlignment.Left;
                    titleTB.Add(titleLb);

                    Button editBtn = customButton.CreateButtonWithImage(constants.updateButton, "title_" + id + "_" + k + "_edit", "", titleFlowPn.Right + 5, 10 + k * 35, 50, 30, 0, 1, 10, FontStyle.Regular, Color.White);
                    titleListPn.Controls.Add(editBtn);
                    editBtn.Click += new EventHandler(EditOption);

                    Button delBtn = customButton.CreateButtonWithImage(constants.deleteButton, "title_" + id, "", editBtn.Right + 5, 10 + k * 35, 50, 30, 0, 1, 10, FontStyle.Regular, Color.White);
                    titleListPn.Controls.Add(delBtn);
                    delBtn.Click += new EventHandler(DeleteOption);
                    k++;
                }
            }
            sqlite_datareader.Close();
            sqlite_datareader = null;
            sqlite_cmd.Dispose();
            sqlite_cmd = null;
            sqlite_conn.Dispose();
            sqlite_conn = null;
        }

        private void OptionValueLoad()
        {
            valueTB.Clear();
            optionListPn.Controls.Clear();
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            SQLiteCommand sqlite_cmd = null;
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                string qry = "SELECT * FROM " + constants.tbNames[16] + " WHERE productID=@productID AND optionTitle=@optionTitle ORDER BY id";
                sqlite_cmd.CommandText = qry;
                sqlite_cmd.Parameters.AddWithValue("productID", productid);
                sqlite_cmd.Parameters.AddWithValue("optionTitle", selectedTitle);
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                int k = 0;
                while (sqlite_datareader.Read())
                {
                    if (!sqlite_datareader.IsDBNull(0))
                    {
                        int id = sqlite_datareader.GetInt32(0);
                        string optionValue = sqlite_datareader.GetString(3);
                        Color fClr = Color.Black;
                        TextBox valueLb = createTextBox.CreateTextBoxs_panel(optionListPn, "valueLb" + id, 30, 13 + k * 35, 100, 25, 12, BorderStyle.None, optionValue);
                        valueLb.Enabled = false;
                        valueLb.TextAlign = HorizontalAlignment.Left;
                        valueTB.Add(valueLb);

                        Button editBtn = customButton.CreateButtonWithImage(constants.updateButton, "value_" + id + "_" + k + "_edit", "", valueLb.Right + 5, 10 + k * 35, 50, 30, 0, 1, 10, FontStyle.Regular, Color.White);
                        optionListPn.Controls.Add(editBtn);
                        editBtn.Click += new EventHandler(EditOption);

                        Button delBtn = customButton.CreateButtonWithImage(constants.deleteButton, "value_" + id, "", editBtn.Right + 5, 10 + k * 35, 50, 30, 0, 1, 10, FontStyle.Regular, Color.White);
                        optionListPn.Controls.Add(delBtn);
                        delBtn.Click += new EventHandler(DeleteOption);
                        k++;
                    }
                }
                sqlite_datareader.Close();
                sqlite_datareader = null;
                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                if(sqlite_cmd != null)
                {
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                    sqlite_conn.Dispose();
                    sqlite_conn = null;
                }
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        private void SelectOptionTitle(object sender, EventArgs e)
        {
            FlowLayoutPanel tempLb = (FlowLayoutPanel)sender;
            selectedTitle = int.Parse(tempLb.Name.Split('_')[1]);
            int index = int.Parse(tempLb.Name.Split('_')[2]);
            foreach (TextBox tbx in titleTB)
            {
                if(int.Parse(tbx.Name.Split('_')[1]) == selectedTitle)
                {
                    tbx.ForeColor = Color.Red;
                    tbx.BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    tbx.ForeColor = Color.Gray;
                    tbx.BorderStyle = BorderStyle.None;
                }
            }
            OptionValueLoad();
        }

        private void EditOption(object sender, EventArgs e)
        {
            Button tempBtn = (Button)sender;
            string optionType = tempBtn.Name.Split('_')[0];
            int id = int.Parse(tempBtn.Name.Split('_')[1]);
            int index = int.Parse(tempBtn.Name.Split('_')[2]);
            string flag = tempBtn.Name.Split('_')[3];
            if(flag == "edit")
            {
                if (optionType == "title")
                {
                    selectedTitle = id;
                    titleTB[index].Enabled = true;
                    titleTB[index].BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    selectedValue = id;
                    valueTB[index].Enabled = true;
                    valueTB[index].BorderStyle = BorderStyle.FixedSingle;
                }
                tempBtn.Name = optionType + "_" + id + "_" + index + "_update";
            }
            else
            {
                if (optionType == "title")
                {
                    selectedTitle = id;
                    titleTB[index].Enabled = false;
                    //titleTB[index].BorderStyle = BorderStyle.None;
                    UpdateOptionTitle(index);
                }
                else
                {
                    selectedValue = id;
                    valueTB[index].Enabled = false;
                    //valueTB[index].BorderStyle = BorderStyle.None;
                    UpdateOptionValue(index);
                }
                tempBtn.Name = optionType + "_" + id + "_" + index + "_edit";
           }
        }

        private void DeleteOption(object sender, EventArgs e)
        {
            Button tempBtn = (Button)sender;
            string optionType = tempBtn.Name.Split('_')[0];
            int id = int.Parse(tempBtn.Name.Split('_')[1]);
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteCommand sqlite_cmd = null;
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                string qry = "";
                if(optionType == "title")
                {
                    qry = "DELETE FROM " + constants.tbNames[15] + " WHERE id=@id";
                    sqlite_cmd.CommandText = qry;
                    sqlite_cmd.Parameters.AddWithValue("@id", id);
                    sqlite_cmd.ExecuteNonQuery();

                    qry = "DELETE FROM " + constants.tbNames[16] + " WHERE optionTitle=@optionTitle";
                    sqlite_cmd.CommandText = qry;
                    sqlite_cmd.Parameters.AddWithValue("@optionTitle", id);
                    sqlite_cmd.ExecuteNonQuery();
                    selectedTitle = -1;
                    OptionTitleLoad();
                }
                else
                {
                    qry = "DELETE FROM " + constants.tbNames[16] + " WHERE id=@id";
                    sqlite_cmd.CommandText = qry;
                    sqlite_cmd.Parameters.AddWithValue("@id", id);
                    sqlite_cmd.ExecuteNonQuery();
                    selectedValue = -1;
                    OptionValueLoad();
                }

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                if (sqlite_cmd != null)
                {
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                }
                sqlite_conn.Dispose();
                sqlite_conn = null;
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        private void SaveOptionTitle(object sender, EventArgs e)
        {
            Button tempTB = (Button)sender;
            string optionTitleNew = titleTxtBx.Text;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }
            
            SQLiteCommand sqlite_cmd = null;
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                string qry = "";
                qry = "INSERT INTO " + constants.tbNames[15] + " (productID, optionTitle) VALUES (@productID, @optionTitleNew)";
                sqlite_cmd.CommandText = qry;
                sqlite_cmd.Parameters.AddWithValue("@productID", productid);
                sqlite_cmd.Parameters.AddWithValue("@optionTitleNew", optionTitleNew);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
                titleTxtBx.Text = "";
                selectedTitle = -1;
                selectedValue = -1;
                OptionTitleLoad();
            }
            catch(Exception ex)
            {
                if(sqlite_cmd != null)
                {
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                }
                sqlite_conn.Dispose();
                sqlite_conn = null;
                Console.WriteLine(ex.ToString());
                return;
            }

        }

        private void SaveOptionValue(object sender, EventArgs e)
        {
            Button tempTB = (Button)sender;
            if(selectedTitle > 0)
            {
                string optionValueNew = optionTxtBx.Text;
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection(constants.dbName);
                if (sqlite_conn.State == ConnectionState.Closed)
                {
                    sqlite_conn.Open();
                }

                SQLiteCommand sqlite_cmd = null;
                try
                {
                    sqlite_cmd = sqlite_conn.CreateCommand();
                    string qry = "";
                    qry = "INSERT INTO " + constants.tbNames[16] + " (productID, optionTitle, optionValue) VALUES (@productID, @optionTitle, @optionValueNew)";
                    sqlite_cmd.CommandText = qry;
                    sqlite_cmd.Parameters.AddWithValue("@productID", productid);
                    sqlite_cmd.Parameters.AddWithValue("@optionTitle", selectedTitle);
                    sqlite_cmd.Parameters.AddWithValue("@optionValueNew", optionValueNew);
                    sqlite_cmd.ExecuteNonQuery();

                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                    sqlite_conn.Dispose();
                    sqlite_conn = null;
                    optionTxtBx.Text = "";
                    selectedValue = -1;
                    OptionValueLoad();
                }
                catch (Exception ex)
                {
                    if (sqlite_cmd != null)
                    {
                        sqlite_cmd.Dispose();
                        sqlite_cmd = null;
                    }
                    sqlite_conn.Dispose();
                    sqlite_conn = null;
                    Console.WriteLine(ex.ToString());
                    return;
                }
            }

        }

        private void UpdateOptionTitle(int index)
        {
            string optionTitleNew = titleTB[index].Text;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteCommand sqlite_cmd = null;
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                string qry = "";
                qry = "UPDATE " + constants.tbNames[15] + " SET optionTitle=@optionTitleNew WHERE id=@selectedID";
                sqlite_cmd.CommandText = qry;
                sqlite_cmd.Parameters.AddWithValue("@optionTitleNew", optionTitleNew);
                sqlite_cmd.Parameters.AddWithValue("@selectedID", selectedTitle);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                if (sqlite_cmd != null)
                {
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                }
                sqlite_conn.Dispose();
                sqlite_conn = null;
                Console.WriteLine(ex.ToString());
                return;
            }

        }

        private void UpdateOptionValue(int index)
        {
            string optionValueNew = valueTB[index].Text;
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection(constants.dbName);
            if (sqlite_conn.State == ConnectionState.Closed)
            {
                sqlite_conn.Open();
            }

            SQLiteCommand sqlite_cmd = null;
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                string qry = "UPDATE " + constants.tbNames[16] + " SET optionValue=@optionValueNew WHERE id=@selectedID";
                sqlite_cmd.CommandText = qry;
                sqlite_cmd.Parameters.AddWithValue("@optionValueNew", optionValueNew);
                sqlite_cmd.Parameters.AddWithValue("@selectedID", selectedValue);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Dispose();
                sqlite_cmd = null;
                sqlite_conn.Dispose();
                sqlite_conn = null;
            }
            catch (Exception ex)
            {
                if (sqlite_cmd != null)
                {
                    sqlite_cmd.Dispose();
                    sqlite_cmd = null;
                }
                sqlite_conn.Dispose();
                sqlite_conn = null;
                Console.WriteLine(ex.ToString());
                return;
            }

        }



        private void BackShow(object sender, EventArgs e)
        {
            //this.Close();
            //this.Dispose();
            prG.CloseAddmitionalMenuDialog();
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


    }
}
