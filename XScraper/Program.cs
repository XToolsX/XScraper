using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

namespace XScraper
{
    class Program
    {
        const int MF_BYCOMMAND = 0x00000000;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [STAThread]
        static void Main(string[] args)
        {
            Console.WindowHeight = 22;
            Console.WindowWidth = 105;

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

            Init();
        }
        public static void SetDefaultFore()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        public static void SetTextFore()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        public static void AddStringNum(string s, int num, bool newline = false)
        {
            var startstring = "[";
            if (newline == true) startstring = "\n[";
            SetTextFore();
            Console.Write(startstring);
            SetDefaultFore();
            Console.Write(num.ToString());
            SetTextFore();
            Console.Write("] ");
            Console.Write(s);
        }
        public static void AddStringNum(string s, string num, bool newline = false)
        {
            var startstring = "[";
            if (newline == true) startstring = "\n[";
            SetTextFore();
            Console.Write(startstring);
            SetDefaultFore();
            Console.Write(num.ToString());
            SetTextFore();
            Console.Write("] ");
            Console.Write(s);
        }
        private static void NewChoice(string text)
        {
            SetTextFore();
            Console.Write(text);
            SetDefaultFore();
            Console.Write(": ");
            SetTextFore();
        }
        private static void Proxy(string proxy)
        {
            Console.Clear();
            SetDefaultFore();
            MakeXLogo();
            Console.Write("\n\n");

            if (proxy == "list")
            {
                Console.WriteLine("ALL, US, GB, DE, CA, NL");
                WaitForInput();
            }
            else
            {
                string url = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=" + proxy.ToLower();

                NewChoice("Timeout[max - 10000]");

                string timeoutentry = ReadKeys(s => { StringToDouble(s, 1, 10000); return true; });
                int timeout = StringToDoubleRaw(timeoutentry);
                url += "&timeout=" + timeout.ToString();

                NewChoice("\nCountry");

                string country = Console.ReadLine();
                country = country.ToLower();
                url += "&country=" + country;


                if (proxy.ToLower() == "http")
                {
                    NewChoice("SSL[all, yes, no]");
                    string ssl = Console.ReadLine();
                    ssl = ssl.ToLower();
                    url += "&ssl=" + ssl;

                    NewChoice("Anonymity[all, elite, anonymous, transparent]");
                    string anonymity = Console.ReadLine();
                    anonymity = anonymity.ToLower();
                    url += "&anonymity=" + anonymity;
                }
                AddStringNum("Scraping Proxies", "-", true);
                var result = string.Empty;
                using (var webClient = new WebClient())
                {
                    result = webClient.DownloadString(url);
                }
                var proxycount = result.Split('\n').Length;
                AddStringNum("Scraped " + proxycount.ToString() + " " + proxy + " Proxies", "-", true);
                NewChoice("\nSave or Copy To Clipboard[s or c]");
                string outputoption = Console.ReadLine();
                if(outputoption == "s")
                {
                    File.WriteAllText(Path.GetFullPath(proxy.ToLower() + "_proxies.txt"), result);
                    AddStringNum("Saved " + proxycount.ToString() + " " + proxy + " Proxies To " + proxy.ToLower() + "_proxies.txt", "-");
                    WaitForInput();
                } else if(outputoption == "c")
                {
                    Clipboard.SetText(result);
                    AddStringNum("Copied " + proxycount.ToString() + " " + proxy + " Proxies To Clipboard", "-");
                    WaitForInput();
                }
            }
        }
        private static void WaitForInput()
        {
            SetTextFore();
            Console.WriteLine("\nPress Any Key To Continue...");
            Console.ReadKey();
            Init();
        }
        private static void MakeOptions()
        {
            Console.Write("                                         ╔═══════════════════════╗\n");
            Console.Write("                                         ║  ");
            AddStringNum("HTTP Proxies", 1);
            SetDefaultFore();
            Console.Write("     ║\n");

            Console.Write("                                         ║  ");
            AddStringNum("SOCKS4 Proxies", 2);
            SetDefaultFore();
            Console.Write("   ║\n");

            Console.Write("                                         ║  ");
            AddStringNum("SOCKS5 Proxies", 3);
            SetDefaultFore();
            Console.Write("   ║\n");

            Console.Write("                                         ║  ");
            AddStringNum("Country List", 4);
            SetDefaultFore();
            Console.Write("     ║\n");

            Console.Write("                                         ╚═══════════════════════╝\n\n");

            SetTextFore();
            Console.Write("Choice");
            SetDefaultFore();
            Console.Write(": ");
            SetTextFore();

            string entry = ReadKeys(s => { StringToDouble(s, 1, 4); return true; });

            int option = StringToDoubleRaw(entry);
            switch(option)
            {
                case 1:
                    Proxy("HTTP");
                    break;
                case 2:
                    Proxy("SOCKS4");
                    break;
                case 3:
                    Proxy("SOCKS5");
                    break;
                case 4:
                    Proxy("list");
                    break;
            }
        }
        public static void Init()
        {
            Console.Title = "XScraper | 1x1x1x1 | XTools";
            Console.Clear();
            SetDefaultFore();
            MakeXLogo();
            Console.Write("\n\n");
            MakeOptions();
        }
        private static int StringToDoubleRaw(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (FormatException)
            {
                return int.Parse(s + '0');
            }
        }
        private static int StringToDouble(string s, int min, int max)
        {
            try
            {
                int test = int.Parse(s);
                if (test <= max && test >= min)
                {
                    return int.Parse(s);
                }
                else
                {
                    throw new InvalidOperationException("Number is not part of options");
                }
            }
            catch (FormatException)
            {
                // handle trailing E and +/- signs
                return int.Parse(s + '0');
            }
            // anything else will be thrown as an exception
        }

        private static string ReadKeys(Predicate<string> check)
        {
            string valid = "0";

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    return valid;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    valid = valid.Substring(0, (valid.Length - 1));
                    Console.Write("\b \b");
                }
                else
                {
                    bool isValid = false;
                    char keyChar = key.KeyChar;
                    string candidate = valid + keyChar;
                    try
                    {
                        isValid = check(candidate);
                    }
                    catch (Exception)
                    {
                    }

                    if (isValid)
                    {
                        Console.Write(keyChar);
                        valid = candidate;
                    }
                }
            }
        }
        public static void MakeXLogo()
        {
            Console.Write(@"                      
                                              .--:         ..:::.  
                                              .:---:     .::--:    
                                                .:---:..----:      
                                                  .:------:        
                                                   .-----:         
                                                 .:---:----:       
                                               .:---:   :----.     
                                              :---:       :---.    
                                            .---:           ::.    
                                           :-:                    ");
        }
    }
}
