using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Text.RegularExpressions;

namespace ParserAccountsInOk
{
    public partial class Form1 : Form
    {
        private static IWebDriver _chromeDriver;
        private static string _url;
        int j, x;
        string str = "-------------";
        char[] ProgressTmp;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _url = "https://ok.ru/olegmaigov/members";
            _chromeDriver = new ChromeDriver();
            _chromeDriver.Navigate().GoToUrl(_url);
            timer3.Enabled = true;
            label3.Text = DateTime.Now.ToString();
        }

        //Скроллбар в низ
        void downScroll()
        {
            label6.Text = "";
            x++;
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

        void PoiskLink()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                IList<IWebElement> links = _chromeDriver.FindElements(By.TagName("a"));
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
                    new Thread(() => { BeginInvoke(new Action(() => timer4.Enabled = false)); parsUrl(); label6.Text = "Идёт поиск ссылок"; }).Start();
                    new Thread(() => { BeginInvoke(new Action(() => timer2.Enabled = false)); }).Start();
                    return;
                }
            }).Start();
        }
        void parsUrl()
        {
            {
                Thread t = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    label6.Invoke((MethodInvoker)delegate
                    {
                        label6.Text = "Идёт поиск ссылок";
                    });
                    List<string> people = new List<string>();
                    MatchCollection match = Regex.Matches(_chromeDriver.PageSource, @"\""/profile(.*?)\?st\.(.*?)""");//@"\""/profile(.*?)"""
                    foreach (Match m in match)
                    {
                        people.Add("https://ok.ru/profile" + m.Groups[1].Value);
                    }
                    for (int i = 0; i < people.Count; i++)
                    {
                        textBox1.Invoke((MethodInvoker)delegate
                        {
                            textBox1.Text += people[i].ToString() + "\r\n";
                        });
                    }
                    textBox1.Invoke((MethodInvoker)delegate
                    {
                        textBox1.Lines = textBox1.Lines.Distinct().ToArray(); //удаляем дубли
                    });
                }
                ); t.Start();
                t.Join();
            }
            new Thread(() => {
                BeginInvoke(new Action(() =>
            label6.Text = "Ссылки добавлены."));
                label5.Text = DateTime.Now.ToString();
            }).Start();
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

        public void SetProgress(Label _Label, int Progres)
        {
            char tmp;
            ProgressTmp = str.ToCharArray();
            tmp = ProgressTmp[Progres];
            ProgressTmp[Progres] = '\0';
            _Label.Text = "Идёт поиск" + new string(ProgressTmp);
            ProgressTmp[Progres] = tmp;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (j > 12)
            {
                j = 0;
            }
            SetProgress(label6, j);
            j++;
        }
    }
}