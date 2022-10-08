using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing.Printing;

namespace ParserAccountsInOk
{
    public partial class Form1 : Form
    {
        private static IWebDriver _chromeDriver;
        private static String _url;
        int x;
        int counter = 0;
        string page_source;
        private const bool v = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _url = "https://ok.ru/olegmaigov/members"; //"https://ok.ru/olegmaigov/members";
            _chromeDriver = new ChromeDriver();
            _chromeDriver.Navigate().GoToUrl(_url);
            timer3.Enabled = true;
            label3.Text = DateTime.Now.ToString();
        }

        //Скроллбар в низ
        void downScroll()
        {
            label6.Text = "";
            ++x;
            label1.Text = x.ToString();
            _chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_chromeDriver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            Thread.Sleep(1000);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (x == 11)
            {
                timer3.Enabled = false;
                timer1.Enabled = false;
                timer4.Enabled = true;
                x = 0;
                label1.Text = x.ToString();
                timer2.Enabled = true;
                PoiskLink();
            }
        }
        private void ProgressChanged()
        {
            counter++;
            switch (counter % 6)
            {
                case 0: label6.Text = "Идёт поиск."; break;
                case 1: label6.Text = "Идёт поиск.."; break;
                case 2: label6.Text = "Идёт поиск..."; break;
                case 3: label6.Text = "Идёт поиск...."; break;
                case 4: label6.Text = "Идёт поиск....."; break;
                case 5: label6.Text = "Идёт поиск......"; break;
            }
        }
        void PoiskLink()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                IList <IWebElement> links = _chromeDriver.FindElements(By.TagName("a"));
            foreach (var link in links)
            {
                if (link.Text == "Показать ещё")
                {
                    link.Click();
                    new Thread(() => { BeginInvoke(new Action(() => timer3.Enabled = true)); timer4.Enabled = false; }).Start();
                    return;
                }
            }
            {
                new Thread(() => { BeginInvoke(new Action(() => copyHTML())); }).Start();
                new Thread(() => { BeginInvoke(new Action(() => timer2.Enabled = false)); }).Start();
                return;
            }
        }).Start();
    }

        



        void parsUrl()
        {
            //string link =textBox1.Text;
            
                MatchCollection match = Regex.Matches(page_source, @"\""/profile(.*?)\?st\.(.*?)""");//@"\""/profile(.*?)"""

            foreach (Match m in match)
            {
                textBox2.Text += "https://ok.ru/profile" + m.Groups[1].Value + Environment.NewLine;
            }

            textBox2.Lines = textBox2.Lines.Distinct().ToArray(); //удаляем дубли
            label5.Text = DateTime.Now.ToString();
           
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (x == 4)
            {
                timer3.Enabled = false;
                timer4.Enabled = true;
                PoiskLink();
                x = 0;
                label1.Text = x.ToString();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //Скроллбар в низ
            downScroll();

        }

        void copyHTML()
        {
            timer2.Enabled = false;
            
                page_source = _chromeDriver.PageSource;
            //textBox1.Text = page_source;
           
             Thread.Sleep(1000);
         

            parsUrl();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             page_source = _chromeDriver.PageSource;
             textBox1.Text = page_source;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            parsUrl();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            ProgressChanged();
        }
    }
}