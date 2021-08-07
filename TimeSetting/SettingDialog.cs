using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSetting
{
    public partial class SettingDialog : Form
    {
        int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        CreatePanel createPanel = new CreatePanel();
        CreateLabel createLabel = new CreateLabel();
        CreateButton createButton = new CreateButton();
        TimeSetting timeHandlerGlobal = null;
        Label currentYearGlobal = null;
        Label currentMonthGlobal = null;
        Label currentDayGlobal = null;
        Label currentHourGlobal = null;
        Label currentMinuteGlobal = null;

        public SettingDialog()
        {
            InitializeComponent();
            string dialogFormImage = @"resources\\dialogpanel.png";
            this.Size = new Size(width / 3, height / 2);
            this.AutoSize = false;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            this.ControlBox = false;
            this.TopLevel = true;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            using (Bitmap img = new Bitmap(dialogFormImage))
            {
                this.BackgroundImage = new Bitmap(img);
            }

            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void InitTimeSetting(TimeSetting timeHandler)
        {
            timeHandlerGlobal = timeHandler;
        }


        public void DateSetting()
        {

            string upBtn = @"resources\\upBtn.png";
            string downBtn = @"resources\\downBtn.png";

            DateTime now = DateTime.Now;

            Panel dialogPanel = createPanel.CreateMainPanel(this, 0, 0, this.Width, this.Height, BorderStyle.None, Color.Transparent);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", "日付設定", dialogPanel.Width, 50, dialogPanel.Width - 30, 35, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);

            Panel yearPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width / 10 - 20, 110, dialogPanel.Width * 2 / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextYear = createButton.CreateButtonWithImage(Image.FromFile(upBtn), "nextYear", "", yearPanel.Width / 4 + 25 / 2, 30, yearPanel.Width / 2 - 25, yearPanel.Width / 2 - 35, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextYear.FlatAppearance.BorderColor = Color.White;
            nextYear.FlatAppearance.BorderSize = 0;
            yearPanel.Controls.Add(nextYear);
            nextYear.Click += new EventHandler(this.YearSelect);

            Label currentYear = createLabel.CreateLabelsInPanel(yearPanel, "currentYear", (now.Year).ToString(), 0, yearPanel.Width / 2 - 5, yearPanel.Width, yearPanel.Height - yearPanel.Width - 20, Color.White, Color.Black, 28, false, ContentAlignment.MiddleCenter);
            currentYearGlobal = currentYear;

            Button prevYear = createButton.CreateButtonWithImage(Image.FromFile(downBtn), "prevYear", "", yearPanel.Width / 4 + 25 / 2, currentYear.Bottom, yearPanel.Width / 2 - 25, yearPanel.Width / 2 - 35, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            yearPanel.Controls.Add(prevYear);
            prevYear.FlatAppearance.BorderColor = Color.White;
            prevYear.FlatAppearance.BorderSize = 0;
            prevYear.Click += new EventHandler(this.YearSelect);

            int currentMonthValue = now.Month;

            Panel monthPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width * 1 / 2 - 20, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);
            Button nextMonth = createButton.CreateButtonWithImage(Image.FromFile(upBtn), "nextMonth", "", 10, 30, monthPanel.Width - 20, monthPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextMonth.FlatAppearance.BorderColor = Color.White;
            nextMonth.FlatAppearance.BorderSize = 0;
            monthPanel.Controls.Add(nextMonth);
            nextMonth.Click += new EventHandler(this.MonthSelect);

            Label currentMonth = createLabel.CreateLabelsInPanel(monthPanel, "currentMonth", currentMonthValue.ToString("00"), 0, monthPanel.Width, monthPanel.Width, monthPanel.Height - 2 * monthPanel.Width - 30, Color.White, Color.Black, 28, false, ContentAlignment.MiddleCenter);
            currentMonthGlobal = currentMonth;

            Button prevMonth = createButton.CreateButtonWithImage(Image.FromFile(downBtn), "prevMonth", "", 10, currentMonth.Bottom, monthPanel.Width - 20, monthPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            monthPanel.Controls.Add(prevMonth);
            prevMonth.FlatAppearance.BorderColor = Color.White;
            prevMonth.FlatAppearance.BorderSize = 0;
            prevMonth.Click += new EventHandler(this.MonthSelect);


            int currentDayValue = now.Day;

            Panel dayPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width * 7 / 10 - 10, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);
            Button nextDay = createButton.CreateButtonWithImage(Image.FromFile(upBtn), "nextDay", "", 10, 30, dayPanel.Width - 20, dayPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextDay.FlatAppearance.BorderColor = Color.White;
            nextDay.FlatAppearance.BorderSize = 0;
            dayPanel.Controls.Add(nextDay);
            nextDay.Click += new EventHandler(this.DaySelect);

            Label currentDay = createLabel.CreateLabelsInPanel(dayPanel, "currentDay", currentDayValue.ToString("00"), 0, dayPanel.Width, dayPanel.Width, dayPanel.Height - 2 * dayPanel.Width - 30, Color.White, Color.Black, 28, false, ContentAlignment.MiddleCenter);
            currentDayGlobal = currentDay;

            Button prevDay = createButton.CreateButtonWithImage(Image.FromFile(downBtn), "prevHour", "", 10, currentDay.Bottom, dayPanel.Width - 20, dayPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            dayPanel.Controls.Add(prevDay);
            prevDay.FlatAppearance.BorderColor = Color.White;
            prevDay.FlatAppearance.BorderSize = 0;
            prevDay.Click += new EventHandler(this.DaySelect);

            string soldoutButtonImage = @"resources/soldoutbutton.png";

            Image backImage = Image.FromFile(soldoutButtonImage);

            Button setButton = createButton.CreateButtonWithImage(backImage, "setDateButton", "OK", dialogPanel.Width - 100, dialogPanel.Height - 70, 80, 30, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);

            dialogPanel.Controls.Add(setButton);

            setButton.Click += new EventHandler(this.SetDate);

        }
        public void TimeSetting()
        {
            string upBtn = @"resources\\upBtn.png";
            string downBtn = @"resources\\downBtn.png";

            DateTime now = DateTime.Now;

            Panel dialogPanel = createPanel.CreateMainPanel(this, 0, 0, this.Width, this.Height, BorderStyle.None, Color.Transparent);

            Label titleLabel = createLabel.CreateLabelsInPanel(dialogPanel, "titleLabel", "時間設定", 30, 50, dialogPanel.Width - 30, 35, Color.Transparent, Color.Black, 22, false, ContentAlignment.MiddleLeft);

            int currentHourValue = now.Hour;
            int currentMinuteValue = now.Minute;

            Panel houurPanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width / 5, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextHour = createButton.CreateButtonWithImage(Image.FromFile(upBtn), "nextHour", "", 10, 30, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextHour.FlatAppearance.BorderColor = Color.White;
            nextHour.FlatAppearance.BorderSize = 0;
            houurPanel.Controls.Add(nextHour);
            nextHour.Click += new EventHandler(this.HourMinuteSelect);

            Label currentHour = createLabel.CreateLabelsInPanel(houurPanel, "currentHour", currentHourValue.ToString("00"), 0, houurPanel.Width, houurPanel.Width, houurPanel.Height - 2 * houurPanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentHourGlobal = currentHour;

            Button prevHour = createButton.CreateButtonWithImage(Image.FromFile(downBtn), "prevHour", "", 10, currentHour.Bottom, houurPanel.Width - 20, houurPanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            houurPanel.Controls.Add(prevHour);
            prevHour.FlatAppearance.BorderColor = Color.White;
            prevHour.FlatAppearance.BorderSize = 0;
            prevHour.Click += new EventHandler(this.HourMinuteSelect);


            Panel MinutePanel = createPanel.CreateSubPanel(dialogPanel, dialogPanel.Width * 3 / 5 + 10, 110, dialogPanel.Width / 5 - 10, dialogPanel.Height * 4 / 5 - 100, BorderStyle.None, Color.Transparent);

            Button nextMinute = createButton.CreateButtonWithImage(Image.FromFile(upBtn), "nextMinute", "", 10, 30, MinutePanel.Width - 20, MinutePanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            nextMinute.FlatAppearance.BorderColor = Color.White;
            nextMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(nextMinute);
            nextMinute.Click += new EventHandler(this.HourMinuteSelect);

            Label currentMinute = createLabel.CreateLabelsInPanel(MinutePanel, "currentMinute", currentMinuteValue.ToString("00"), 0, MinutePanel.Width, MinutePanel.Width, MinutePanel.Height - 2 * MinutePanel.Width - 30, Color.White, Color.Black, 32, false, ContentAlignment.MiddleCenter);
            currentMinuteGlobal = currentMinute;

            Button prevMinute = createButton.CreateButtonWithImage(Image.FromFile(downBtn), "prevMinute", "", 10, currentMinute.Bottom, MinutePanel.Width - 20, MinutePanel.Width - 30, 0, 1, 14, FontStyle.Regular, Color.Black, ContentAlignment.MiddleCenter, 1);
            prevMinute.BackgroundImageLayout = ImageLayout.Stretch;
            prevMinute.FlatAppearance.BorderColor = Color.White;
            prevMinute.FlatAppearance.BorderSize = 0;
            MinutePanel.Controls.Add(prevMinute);
            prevMinute.Click += new EventHandler(this.HourMinuteSelect);

            string soldoutButtonImage = @"resources/soldoutbutton.png";
            Image backImage = Image.FromFile(soldoutButtonImage);

            Button setButton = createButton.CreateButtonWithImage(backImage, "setTimeButton", "OK", dialogPanel.Width - 110, dialogPanel.Height - 70, 80, 40, 1, 10, 14, FontStyle.Bold, Color.White, ContentAlignment.MiddleCenter, 1);
            dialogPanel.Controls.Add(setButton);

            setButton.Click += new EventHandler(this.SetDate);

        }

        public void YearSelect(object sender, EventArgs e)
        {
            Button tempBtn = (Button)sender;
            int currentYear = int.Parse(currentYearGlobal.Text);
            if (tempBtn.Name == "nextYear")
            {
                currentYear++;
            }
            else
            {
                currentYear--;
            }
            currentYearGlobal.Text = currentYear.ToString();
        }
        public void MonthSelect(object sender, EventArgs e)
        {
            Button tempBtn = (Button)sender;
            int currentMonth = int.Parse(currentMonthGlobal.Text);
            if (tempBtn.Name == "nextMonth")
            {
                if (currentMonth == 12)
                {
                    currentMonth = 1;
                }
                else
                {
                    currentMonth++;
                }

            }
            else
            {
                if (currentMonth == 1)
                {
                    currentMonth = 12;
                }
                else
                {
                    currentMonth--;
                }
            }
            currentMonthGlobal.Text = currentMonth.ToString("00");
        }
        public void DaySelect(object sender, EventArgs e)
        {
            int currentYear = int.Parse(currentYearGlobal.Text);
            int currentMonth = int.Parse(currentMonthGlobal.Text);
            int endDate = DateTime.DaysInMonth(currentYear, currentMonth);

            int currentDay = int.Parse(currentDayGlobal.Text);
            Button tempBtn = (Button)sender;

            if (tempBtn.Name == "nextDay")
            {

                if (currentDay == endDate)
                {
                    currentDay = 1;
                }
                else
                {
                    currentDay++;
                }
            }
            else
            {
                if (currentDay == 1)
                {
                    currentDay = endDate;
                }
                else
                {
                    currentDay--;
                }
            }
            currentDayGlobal.Text = currentDay.ToString("00");
        }
        public void HourMinuteSelect(object sender, EventArgs e)
        {
            Button lTemp = (Button)sender;
            int endValue = 59;
            int currentValue = int.Parse(currentMinuteGlobal.Text);

            if (lTemp.Name == "nextHour")
            {
                endValue = 23;
                currentValue = int.Parse(currentHourGlobal.Text);
                if (currentValue >= endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if (lTemp.Name == "prevHour")
            {
                currentValue = int.Parse(currentHourGlobal.Text);
                if (currentValue == 0)
                {
                    currentValue = 23;
                }
                else
                {
                    currentValue--;
                }
            }
            else if (lTemp.Name == "nextMinute")
            {
                endValue = 59;
                if (currentValue == endValue)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue++;
                }
            }
            else if (lTemp.Name == "prevMinute")
            {
                if (currentValue == 0)
                {
                    currentValue = 59;
                }
                else
                {
                    currentValue--;
                }
            }

            if (lTemp.Name == "nextHour" || lTemp.Name == "prevHour")
            {
                currentHourGlobal.Text = this.DateTimeFormat(currentValue);
            }
            else
            {
                currentMinuteGlobal.Text = this.DateTimeFormat(currentValue);
            }

        }

        private string DateTimeFormat(int val)
        {
            if (val < 10)
            {
                return "0" + val.ToString();
            }
            return val.ToString();
        }

        public void SetDate(object sender, EventArgs e)
        {
            Button btnTemp = (Button)sender;
            if (btnTemp.Name == "setDateButton")
            {
                string currentYear = currentYearGlobal.Text;
                string currentMonth = currentMonthGlobal.Text;
                string currentDay = currentDayGlobal.Text;
                timeHandlerGlobal.SetVal("setDate", currentYear + "_" + currentMonth + "_" + currentDay);
            }
            else if (btnTemp.Name == "setTimeButton")
            {
                string currentHour = currentHourGlobal.Text;
                string currentMinute = currentMinuteGlobal.Text;
                timeHandlerGlobal.SetVal("setTime", currentHour + "_" + currentMinute);
            }
        }



    }
}
