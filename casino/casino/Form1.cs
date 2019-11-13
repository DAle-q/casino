using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//добавить глобальные значения.!!
namespace casino
{
    public partial class Form1 : Form
    {
        IWebDriver Browser;
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        Timer timer = new Timer();
        List<IWebElement> Temp;
        int turns = 20;
        double[] array = new double[20];

        public Form1()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 2;
            dataGridView1.Rows.Add(100);
            dataGridView1.Columns[0].Name = "№";
            dataGridView1.Columns[1].Name = "Results";
            for (int l = 0; l < 100; l++)
                dataGridView1[0, l].Value = l;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.ColumnCount = 2;
            //dataGridView2.Columns[0].ValueType = typeof(double);
            dataGridView2.Rows.Add(100);
            dataGridView2.Columns[0].Name = "Turn's back";
            dataGridView2.Columns[1].Name = "10x+";
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenQA.Selenium.Chrome.ChromeOptions options = new OpenQA.Selenium.Chrome.ChromeOptions();
            options.AddArguments(@"user-data-dir=C:\Users\DAle\AppData\Local\Google\Chrome\User Data\Default");
            Browser = new OpenQA.Selenium.Chrome.ChromeDriver(options);
            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(300);
            Browser.Navigate().GoToUrl("https://evoplay.games/portfolio/high-striker/");
            System.Threading.Thread.Sleep(1000);
            Cursor.Position = new Point(925, 861);
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            System.Threading.Thread.Sleep(100);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            System.Threading.Thread.Sleep(300);
            Cursor.Position = new Point(800, 620);
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            System.Threading.Thread.Sleep(100);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            System.Threading.Thread.Sleep(2000);
            Browser.SwitchTo().Frame(0);
            Temp = Browser.FindElements(By.CssSelector("#app table tbody b")).ToList();
            //List<IWebElement> Results = Browser.FindElements(By.XPath("//*[@id='app']/div/div/div[5]/div/div[2]/table/tbody/tr[1]/td[1]/b")).ToList();
            //var i = 0;
            //double[] array = new double[19];
            int i = 0;
            foreach (var element in Temp)
            {
                dataGridView1[1, i].Value = GetValue(element);
                array[i] = Convert.ToDouble(dataGridView1[1, i].Value);
                GlobalTurns(Convert.ToDouble(dataGridView1[1, i].Value));
                //dataGridView1[0, i].Style.BackColor = Color.Red;
                i++;
            }
            Post();
            timer.Interval = 4900; // 5000 (5 сек)
            timer.Start();
            timer.Tick += button2_Click;

        }
        private double GetValue(IWebElement element)
        {
            var temp = Convert.ToString(element.Text);
            temp = temp.Replace('.', ',');
            temp = temp.Replace('x', ' ');
            return Convert.ToDouble(temp);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            List<IWebElement> Results = Browser.FindElements(By.CssSelector("#app table tbody b")).ToList();
            if (GetValue(Results[0]) == Convert.ToDouble(dataGridView1[1, 0].Value))
                if (GetValue(Results[1]) == Convert.ToDouble(dataGridView1[1, 1].Value))
                    if (GetValue(Results[2]) == Convert.ToDouble(dataGridView1[1, 2].Value))
                        textBox1.Text = "Nothing new" + GetValue(Results[0]);
                    else
                    {
                        RefreshValues();
                        Post();
                        GlobalTurns(Convert.ToDouble(dataGridView1[1, 0].Value));
                        turns++;
                    }
                else
                {
                    RefreshValues();
                    Post();
                    GlobalTurns(Convert.ToDouble(dataGridView1[1, 0].Value));
                    turns++;
                }
            else
            {
                RefreshValues();
                Post();
                GlobalTurns(Convert.ToDouble(dataGridView1[1, 0].Value));
                turns++;
            }
            

            //проверяем существование списка заданий
            /*var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(1));
            //ждем появление элемента
            try
            {
                var temp1 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.PartialLinkText("Next round in")));
                for (var k = 99; k > 0; k--)
                    dataGridView1[0, k].Value = dataGridView1[0, k - 1].Value;
                dataGridView1[0, 0].Value = GetValue(Browser.FindElement(By.CssSelector("#app table tbody b")));
                //IWebElement order = wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Next round in")));
            }
            catch
            {
                textBox1.Text = "empty";
            };
            Browser.FindElement(By.PartialLinkText("Next round in"));
            for (var k = 99; k > 0; k--)
                dataGridView1[0, k].Value = dataGridView1[0, k - 1].Value;
            dataGridView1[0, 0].Value = GetValue(Browser.FindElement(By.CssSelector("#app table tbody b"))); */
        }
        private double Average(int x)
        {
            double sum = 0;
            for (int k = 0; k < x; k++)
                if (Convert.ToDouble(dataGridView1[1, k].Value) < 10)
                    sum += Convert.ToDouble(dataGridView1[1, k].Value);
                else
                    sum += 10;
            return Math.Round((sum / x),3);

        }
        private int Count(double x)
        {
            int quantity = 0;
            for (int k = 0; k < 99; k++)
                if (Convert.ToDouble(dataGridView1[1, k].Value) >= x)
                    quantity += 1;
            return quantity;
        }
        private void Post()
        {
            textBox2.Text = Convert.ToString(Average(20));
            textBox3.Text = Convert.ToString(Average(30));
            textBox4.Text = Convert.ToString(Average(50));
            textBox5.Text = Convert.ToString(Average(100));
            textBox7.Text = Convert.ToString(Count(1.5));
            textBox8.Text = Convert.ToString(Count(2));
            textBox9.Text = Convert.ToString(Count(3));
            textBox10.Text = Convert.ToString(Count(4));
            textBox11.Text = Convert.ToString(Count(5));
            textBox6.Text = Convert.ToString(turns);
        }
        private void RefreshValues()
        {
            for (int k = 99; k > 0; k--)
            {
                dataGridView1[1, k].Value = dataGridView1[1, k - 1].Value;
                if(Convert.ToDouble(dataGridView1[1,k].Value) >=2.0)
                    dataGridView1[1, k].Style.BackColor = Color.LightGreen;
                else if(Convert.ToDouble(dataGridView1[1, k].Value) >= 10.0)
                    dataGridView1[1, k].Style.BackColor = Color.Orange;
                else
                    dataGridView1[1, k].Style.BackColor = Color.White;
            }
            /* (int k = 99; k > 3; k--)
            {
                if (Convert.ToDouble(dataGridView1[1, k].Value) >= 10.0)
                    dataGridView1[1, k].Value = 10;
            }*/
            dataGridView1[1, 0].Value = GetValue(Browser.FindElement(By.CssSelector("#app table tbody b")));
            if (Convert.ToDouble(dataGridView1[1, 0].Value) >= 10)
            {
                for (int k = 99; k > 0; k--)
                {
                    dataGridView2[1, k].Value = dataGridView2[1, k - 1].Value;
                    dataGridView2[0, k].Value = dataGridView2[0, k - 1].Value;
                    if (Convert.ToDouble(dataGridView2[1, k].Value) >= 100.0)
                        dataGridView2[1, k].Style.BackColor = Color.RoyalBlue;
                    else
                        dataGridView2[1, k].Style.BackColor = Color.White;
                    dataGridView2[1, k].Value = dataGridView2[1, k - 1].Value;
                }                
                dataGridView2[1, 0].Value = dataGridView1[1, 0].Value;
                //dataGridView1[1, 0].Value = 10;
                dataGridView2[0, 0].Value = 1; 
            }
            for (int k = 0; k < 99; k++)
            {
                if (Convert.ToDouble(dataGridView2[0, k].Value) > 0)
                    dataGridView2[0, k].Value = Convert.ToDouble(dataGridView2[0, k].Value) + 1;
            }
        }
        private void GlobalTurns(double value)
        {
            if (value >= 1.5)
            {
                textBox12.Text = Convert.ToString(Convert.ToDouble(textBox12.Text) + 1);
                label12.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox12.Text) / turns / 0.666, 3)) + "%";
                if (value >= 2)
                {
                    textBox13.Text = Convert.ToString(Convert.ToDouble(textBox13.Text) + 1);
                    label13.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox13.Text) / turns / 0.50, 3)) + "%";
                    if (value >= 3)
                    {
                        textBox14.Text = Convert.ToString(Convert.ToDouble(textBox14.Text) + 1);
                        label14.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox14.Text) / turns / 0.333, 3)) + "%";
                        if (value >= 4)
                        {
                            textBox15.Text = Convert.ToString(Convert.ToDouble(textBox15.Text) + 1);
                            label15.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox15.Text) / turns / 0.25, 3)) + "%";
                            if (value >= 5)
                            {
                                textBox16.Text = Convert.ToString(Convert.ToDouble(textBox16.Text) + 1);
                                label16.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox16.Text) / turns / 0.2, 3)) + "%";
                            }
                        }
                        
                    }
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Browser.Quit();
        }
    }
}
