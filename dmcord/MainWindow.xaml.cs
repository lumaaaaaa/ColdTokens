using MaterialDesignColors;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace coldtokens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        // static variables
        public static PaletteHelper paletteHelper = new PaletteHelper();
        public static IBaseTheme darkTheme = Theme.Dark;
        public static IBaseTheme lightTheme = Theme.Light;
        public static bool isSettingsOpen = false;
        public static bool running = false;
        List<string> proxies = new List<string>();
        List<string> tokens = new List<string>();
        public static String proxylist = null;
        public static String tokenlist = null;
        public static String sendBody = "";
        public static String proxyType = "";
        public static int currentToken = 0;

        public MainWindow()
        {
            InitializeComponent();
            startIt.Content = "Start";
        }

        public static String betweenStrings(String text, String start, String end)
        {
            try
            {
                int p1 = text.IndexOf(start) + start.Length;
                int p2 = text.IndexOf(end, p1);
                if (end == "") return (text.Substring(p1));
                else return text.Substring(p1, p2 - p1);
            }
            catch (Exception e)
            {
                return "failed";
            }
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            progression.Value = 0;
            if (isSettingsOpen)
            {
                prefIcon.Kind = PackIconKind.Settings;
                title.Text = "ColdTokens. - v0.0.1";
                if (useLightTheme.IsChecked == true)
                {
                    ITheme defaulttheme = Theme.Create(lightTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
                    paletteHelper.SetTheme(defaulttheme);
                }
                else
                {
                    ITheme defaulttheme = Theme.Create(darkTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
                    paletteHelper.SetTheme(defaulttheme);
                }
                startIt.Visibility = Visibility.Visible;
                scrollBox.Visibility = Visibility.Visible;
                build.Visibility = Visibility.Hidden;
                useLightTheme.Visibility = Visibility.Hidden;
                useProxies.Visibility = Visibility.Hidden;
                openTokens.Visibility = Visibility.Hidden;
                channelID.Visibility = Visibility.Hidden;
                message.Visibility = Visibility.Hidden;
                joiner.Visibility = Visibility.Hidden;
                startJoin.Visibility = Visibility.Hidden;
                inviteCode.Visibility = Visibility.Hidden;
                verifier.Visibility = Visibility.Hidden;
                startVerify.Visibility = Visibility.Hidden;
                channelIDReact.Visibility = Visibility.Hidden;
                messageIDReact.Visibility = Visibility.Hidden;
                emojiChar.Visibility = Visibility.Hidden;
                if (tokens.Count > 0)
                {
                    Output.Text = "Ready to start...";
                }
                else
                {

                }
                isSettingsOpen = false;
            }
            else
            {
                setDefaultTheme();
                title.Text = "Settings.";
                startIt.Visibility = Visibility.Hidden;
                scrollBox.Visibility = Visibility.Hidden;
                build.Visibility = Visibility.Visible;
                useLightTheme.Visibility = Visibility.Visible;
                useProxies.Visibility = Visibility.Visible;
                openTokens.Visibility = Visibility.Visible;
                channelID.Visibility = Visibility.Visible;
                message.Visibility = Visibility.Visible;
                joiner.Visibility = Visibility.Visible;
                verifier.Visibility = Visibility.Visible;
                isSettingsOpen = true;
            }
        }
        public void setDefaultTheme()
        {
            if (useLightTheme.IsChecked == true)
            {
                ITheme defaulttheme = Theme.Create(lightTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
                paletteHelper.SetTheme(defaulttheme);
            }
            else
            {
                ITheme defaulttheme = Theme.Create(darkTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
                paletteHelper.SetTheme(defaulttheme);
            }
        }
        private void useLightTheme_Checked(object sender, RoutedEventArgs e)
        {
            ITheme defaulttheme = Theme.Create(lightTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
            paletteHelper.SetTheme(defaulttheme);
        }

        private void useLightTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            ITheme defaulttheme = Theme.Create(darkTheme, SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue], SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue]);
            paletteHelper.SetTheme(defaulttheme);
        }

        private void useProxies_Checked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
                proxylist = openFileDialog.FileName;
            if (proxylist == null)
            {
                snackbar.MessageQueue.Enqueue(
                    "Failed to load proxy list.",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));
            }
            else
            {
                proxies = File.ReadAllLines(proxylist).ToList();
                for (int i = 0; i < proxies.Count; i++)
                {
                    proxies[i] = proxies[i].Replace("@", ":");
                }
                if (proxies[0].Split(":").Length == 2)
                {
                    proxyType = "ip:port";
                    snackbar.MessageQueue.Enqueue(
                    "Proxy list loaded successfully. " + proxies.Count + " proxies locked and loaded!",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));
                }
                else if (proxies[0].Split(":").Length == 4)
                {
                    if (proxies[0].Split(":")[2].Contains("."))
                    {
                        proxyType = "user:pass:ip:port";
                        snackbar.MessageQueue.Enqueue(
                        "Proxy list loaded successfully. " + proxies.Count + " proxies locked and loaded!",
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(3));
                    }
                    else if(proxies[0].Split(":")[0].Contains("."))
                    {
                        proxyType = "ip:port:user:pass";
                        snackbar.MessageQueue.Enqueue(
                        "Proxy list loaded successfully. " + proxies.Count + " proxies locked and loaded!",
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(3));
                    }
                }
                else
                {
                    proxies = null;
                    snackbar.MessageQueue.Enqueue(
                    "Proxy list is in unknown format. Only 'ip:port', 'user:pass@ip:port', and 'ip:port:user:pass' are supported.",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));
                }

            }
        }

        private void openTokens_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
                tokenlist = openFileDialog.FileName;
            if (tokenlist == null)
            {
                snackbar.MessageQueue.Enqueue(
                    "Failed to load token list.",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));
            }
            else
            {
                tokens = File.ReadAllLines(tokenlist).ToList();
                Output.Text += "\n" + tokens.Count + " tokens added!";
                snackbar.MessageQueue.Enqueue(
                    "Token list loaded successfully. " + tokens.Count + " tokens locked and loaded!",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));
                startIt.IsEnabled = true;
                joiner.IsEnabled = true;
                verifier.IsEnabled = true;
                if (tokens[0].Contains(":"))
                {
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        tokens[i] = tokens[i].Substring(tokens[i].LastIndexOf(":") + 1);
                        Debug.WriteLine(tokens[i]);
                    }
                }
            }
        }
        private void join(object sender, RoutedEventArgs e)
        {
            progression.Value = 0;
            title.Text = "Joiner.";
            prefIcon.Kind = PackIconKind.Home;
            startIt.Visibility = Visibility.Hidden;
            startJoin.Visibility = Visibility.Visible;
            startJoin.IsEnabled = true;
            scrollBox.Visibility = Visibility.Visible;
            inviteCode.Visibility = Visibility.Visible;
            Output.Text = "";
            Output.Text += "\nJoiner module active.";
            build.Visibility = Visibility.Hidden;
            useLightTheme.Visibility = Visibility.Hidden;
            useProxies.Visibility = Visibility.Hidden;
            openTokens.Visibility = Visibility.Hidden;
            channelID.Visibility = Visibility.Hidden;
            message.Visibility = Visibility.Hidden;
            joiner.Visibility = Visibility.Hidden;
            verifier.Visibility = Visibility.Hidden;
        }

        private void react(object sender, RoutedEventArgs e)
        {
            progression.Value = 0;
            title.Text = "React Verifier.";
            prefIcon.Kind = PackIconKind.Home;
            startIt.Visibility = Visibility.Hidden;
            startVerify.Visibility = Visibility.Visible;
            startVerify.IsEnabled = true;
            channelIDReact.Visibility = Visibility.Visible;
            messageIDReact.Visibility = Visibility.Visible;
            emojiChar.Visibility = Visibility.Visible;
            scrollBox.Visibility = Visibility.Visible;
            inviteCode.Visibility = Visibility.Hidden;
            Output.Text = "";
            Output.Text += "\nReaction verify module active.";
            build.Visibility = Visibility.Hidden;
            useLightTheme.Visibility = Visibility.Hidden;
            useProxies.Visibility = Visibility.Hidden;
            openTokens.Visibility = Visibility.Hidden;
            channelID.Visibility = Visibility.Hidden;
            message.Visibility = Visibility.Hidden;
            joiner.Visibility = Visibility.Hidden;
            verifier.Visibility = Visibility.Hidden;
        }

        private void startJoinThread(object sender, RoutedEventArgs e)
        {
            string code = inviteCode.Text;

            Output.Text = "";
            Output.Text += "\nJoining discord.gg/" + code + " on " + tokens.Count + " tokens...";
            startJoin.IsEnabled = false;
            preferences.IsEnabled = false;
            Task.Run(() => join(code));
        }

        private void startVerifyThread(object sender, RoutedEventArgs e)
        {
            var enc = new UTF8Encoding(true, false);
            string channelIDV = channelIDReact.Text;
            string messageIDV = messageIDReact.Text;
            string emoji = emojiChar.Text;
            string emojiBytes = BitConverter.ToString(enc.GetBytes(emoji));
            emojiBytes = emojiBytes.Replace("-", "%");

            Output.Text = "";
            Output.Text += "\nReacting to message " + messageIDV + " on " + tokens.Count + " tokens...";
            startJoin.IsEnabled = false;
            preferences.IsEnabled = false;
            Task.Run(() => react("https://discord.com/api/v9/channels/" + channelIDV + "/messages/" + messageIDV + "/reactions/%" + emojiBytes + "/%40me"));
        }
        private String GetNextProxyinQueue()
        {
            lock (proxies)
            {
                if (proxies.Count > 0)
                {
                    String temp = proxies[0];
                    proxies.Remove(temp);
                    return temp;
                }
                else
                {
                    proxies = File.ReadAllLines(proxylist).ToList();
                    String temp = proxies[0];
                    proxies.Remove(temp);
                    return temp;
                }
            }
        }
        private async Task react(string url)
        {
            int successfulReacts = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                HttpWebRequest getCookies = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com"));
                string localproxy = GetNextProxyinQueue();
                switch(proxyType)
                {
                    case null:
                        break;
                    case "ip:port":
                        getCookies.Proxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        break;
                    case "user:pass:ip:port":
                        WebProxy sendProxy = new WebProxy("http://" + localproxy.Split(":")[2] + ":" + localproxy.Split(":")[3]);
                        sendProxy.Credentials = new NetworkCredential(localproxy.Split(":")[0], localproxy.Split(":")[1]);
                        getCookies.Proxy = sendProxy;
                        break;
                    case "ip:port:user:pass":
                        WebProxy newProxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        newProxy.Credentials = new NetworkCredential(localproxy.Split(":")[2], localproxy.Split(":")[3]);
                        getCookies.Proxy = newProxy;
                        break;
                }
                getCookies.Method = "GET";
                HttpWebResponse cookieResponse = null;
                try
                {
                    cookieResponse = (HttpWebResponse)getCookies.GetResponse();
                    using (StreamReader reader = new StreamReader(cookieResponse.GetResponseStream()))
                    {

                    }
                }
                catch (WebException ex)
                {

                }
                catch (NullReferenceException ex)
                {

                }
                string dcfduid = cookieResponse.Headers["Set-Cookie"].After("__dcfduid=").Before(";");
                string sdcfduid = cookieResponse.Headers["Set-Cookie"].After("__sdcfduid=").Before(";");
                string completeCookie = "__dcfduid=" + dcfduid + "; " + "__sdcfduid=" + sdcfduid + "; " + "locale=us";
                HttpWebRequest reactToMsg = (HttpWebRequest)WebRequest.Create(new Uri(url));
                switch (proxyType)
                {
                    case null:
                        break;
                    case "ip:port":
                        reactToMsg.Proxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        break;
                    case "user:pass:ip:port":
                        WebProxy sendProxy = new WebProxy("http://" + localproxy.Split(":")[2] + ":" + localproxy.Split(":")[3]);
                        sendProxy.Credentials = new NetworkCredential(localproxy.Split(":")[0], localproxy.Split(":")[1]);
                        reactToMsg.Proxy = sendProxy;
                        break;
                    case "ip:port:user:pass":
                        WebProxy newProxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        newProxy.Credentials = new NetworkCredential(localproxy.Split(":")[2], localproxy.Split(":")[3]);
                        reactToMsg.Proxy = newProxy;
                        break;
                }
                reactToMsg.Method = "PUT";
                reactToMsg.Headers["Connection"] = "keep-alive";
                reactToMsg.Accept = "*/*";
                reactToMsg.Headers["Accept-Encoding"] = "gzip, deflate, br";
                reactToMsg.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                reactToMsg.Headers["X-Debug-Options"] = "bugReporterEnabled";
                reactToMsg.Headers["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRGlzY29yZCBDbGllbnQiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfdmVyc2lvbiI6IjEuMC45MDAzIiwib3NfdmVyc2lvbiI6IjEwLjAuMTkwNDIiLCJvc19hcmNoIjoieDY0Iiwic3lzdGVtX2xvY2FsZSI6ImVuLVVTIiwiY2xpZW50X2J1aWxkX251bWJlciI6MTEwNDUxLCJjbGllbnRfZXZlbnRfc291cmNlIjpudWxsfQ==";
                reactToMsg.Headers["X-Discord-Locale"] = "en-US";
                reactToMsg.Headers["Authorization"] = tokens[i];
                reactToMsg.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9003 Chrome/91.0.4472.164 Electron/13.4.0 Safari/537.36";
                reactToMsg.Headers["Sec-Fetch-Site"] = "same-origin";
                reactToMsg.Headers["Sec-Fetch-Mode"] = "cors";
                reactToMsg.Headers["Sec-Fetch-Dest"] = "empty";
                reactToMsg.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                reactToMsg.Headers["Origin"] = "https://discord.com";
                reactToMsg.ContentLength = 0;
                reactToMsg.Referer = "https://discord.com/channels/@me";
                reactToMsg.Headers["Cookie"] = completeCookie;
                HttpWebResponse reactResponse = null;
                string content = null;
                try
                {
                    reactResponse = (HttpWebResponse)reactToMsg.GetResponse();
                    using (StreamReader reader = new StreamReader(reactResponse.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {

                }
                catch (NullReferenceException ex)
                {

                }
                if (content == null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nFailed to react on a token!"; }));
                }
                else
                {
                    successfulReacts++;
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nReacted on a token."; }));
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { progression.Value = ((double)(i + 1) / (double)tokens.Count) * 100; }));
            }
            Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nFinished verifying on " + successfulReacts + " tokens..."; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { startVerify.IsEnabled = true; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { preferences.IsEnabled = true; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { progression.Value = 100; }));
        }

        private async Task join(string inviteCode)
        {
            int successfulJoins = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                HttpWebRequest getCookies = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com"));
                string localproxy = GetNextProxyinQueue();
                switch (proxyType)
                {
                    case null:
                        break;
                    case "ip:port":
                        getCookies.Proxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        break;
                    case "user:pass:ip:port":
                        WebProxy sendProxy = new WebProxy("http://" + localproxy.Split(":")[2] + ":" + localproxy.Split(":")[3]);
                        sendProxy.Credentials = new NetworkCredential(localproxy.Split(":")[0], localproxy.Split(":")[1]);
                        getCookies.Proxy = sendProxy;
                        break;
                    case "ip:port:user:pass":
                        WebProxy newProxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        newProxy.Credentials = new NetworkCredential(localproxy.Split(":")[2], localproxy.Split(":")[3]);
                        getCookies.Proxy = newProxy;
                        break;
                }
                getCookies.Method = "GET";
                HttpWebResponse cookieResponse = null;
                try
                {
                    cookieResponse = (HttpWebResponse)getCookies.GetResponse();
                    using (StreamReader reader = new StreamReader(cookieResponse.GetResponseStream()))
                    {

                    }
                }
                catch (WebException ex)
                {

                }
                catch (NullReferenceException ex)
                {

                }
                string dcfduid = cookieResponse.Headers["Set-Cookie"].After("__dcfduid=").Before(";");
                string sdcfduid = cookieResponse.Headers["Set-Cookie"].After("__sdcfduid=").Before(";");
                string completeCookie = "__dcfduid=" + dcfduid + "; " + "__sdcfduid=" + sdcfduid + "; " + "locale=us";
                HttpWebRequest joinServer = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com/api/v9/invites/" + inviteCode));
                switch (proxyType)
                {
                    case null:
                        break;
                    case "ip:port":
                        joinServer.Proxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        break;
                    case "user:pass:ip:port":
                        WebProxy sendProxy = new WebProxy("http://" + localproxy.Split(":")[2] + ":" + localproxy.Split(":")[3]);
                        sendProxy.Credentials = new NetworkCredential(localproxy.Split(":")[0], localproxy.Split(":")[1]);
                        joinServer.Proxy = sendProxy;
                        break;
                    case "ip:port:user:pass":
                        WebProxy newProxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                        newProxy.Credentials = new NetworkCredential(localproxy.Split(":")[2], localproxy.Split(":")[3]);
                        joinServer.Proxy = newProxy;
                        break;
                }
                joinServer.Method = "POST";
                joinServer.Headers["Connection"] = "keep-alive";
                joinServer.Accept = "*/*";
                joinServer.Headers["Accept-Encoding"] = "*/*";
                joinServer.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                joinServer.Headers["X-Debug-Options"] = "bugReporterEnabled";
                joinServer.Headers["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRGlzY29yZCBDbGllbnQiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfdmVyc2lvbiI6IjEuMC45MDAzIiwib3NfdmVyc2lvbiI6IjEwLjAuMTkwNDIiLCJvc19hcmNoIjoieDY0Iiwic3lzdGVtX2xvY2FsZSI6ImVuLVVTIiwiY2xpZW50X2J1aWxkX251bWJlciI6MTEwNDUxLCJjbGllbnRfZXZlbnRfc291cmNlIjpudWxsfQ==";
                joinServer.Headers["X-Discord-Locale"] = "en-US";
                joinServer.Headers["Authorization"] = tokens[i];
                joinServer.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9003 Chrome/91.0.4472.164 Electron/13.4.0 Safari/537.36";
                joinServer.Headers["Sec-Fetch-Site"] = "same-origin";
                joinServer.Headers["Sec-Fetch-Mode"] = "cors";
                joinServer.Headers["Sec-Fetch-Dest"] = "empty";
                joinServer.Referer = "https://discord.com/channels/@me";
                joinServer.Headers["Cookie"] = completeCookie;
                HttpWebResponse joinResponse = null;
                string content = null;
                try
                {
                    joinResponse = (HttpWebResponse)joinServer.GetResponse();
                    using (StreamReader reader = new StreamReader(joinResponse.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {

                }
                catch (NullReferenceException ex)
                {

                }
                if (content == null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nFailed to join on a token!"; }));
                }
                else
                {
                    successfulJoins++;
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                    Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nJoined on a token."; }));
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { progression.Value = ((double)(i + 1) / (double)tokens.Count) * 100; }));
            }
            Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nFinished joining discord.gg/" + inviteCode + " on " + successfulJoins + " tokens..."; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { startJoin.IsEnabled = true; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { preferences.IsEnabled = true; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { progression.Value = 100; }));
        }

        private void start(object sender, RoutedEventArgs e)
        {
            if (startIt.Content == "Start")
            {
                string text = channelID.Text;
                running = true;
                preferences.IsEnabled = false;
                startIt.Content = "Stop";
                Task.Run(() => checkForJoins(text));
            }
            else
            {
                running = false;
                preferences.IsEnabled = true;
                startIt.Content = "Start";
            }

        }
        private async Task checkForJoins(string targetChannel)
        {
            HttpWebRequest getCookies = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com"));
            string localproxy = GetNextProxyinQueue();
            switch (proxyType)
            {
                case null:
                    break;
                case "ip:port":
                    getCookies.Proxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                    break;
                case "user:pass:ip:port":
                    WebProxy sendProxy = new WebProxy("http://" + localproxy.Split(":")[2] + ":" + localproxy.Split(":")[3]);
                    sendProxy.Credentials = new NetworkCredential(localproxy.Split(":")[0], localproxy.Split(":")[1]);
                    getCookies.Proxy = sendProxy;
                    break;
                case "ip:port:user:pass":
                    WebProxy newProxy = new WebProxy("http://" + localproxy.Split(":")[0] + ":" + localproxy.Split(":")[1]);
                    newProxy.Credentials = new NetworkCredential(localproxy.Split(":")[2], localproxy.Split(":")[3]);
                    getCookies.Proxy = newProxy;
                    break;
            }
            getCookies.Method = "GET";
            HttpWebResponse cookieResponse = null;
            string id = null;
            HttpWebResponse monitorResponse = null;
            int monitorCode = 200;
            string content = null;
            try
            {
                cookieResponse = (HttpWebResponse)getCookies.GetResponse();
                using (StreamReader reader = new StreamReader(cookieResponse.GetResponseStream()))
                {

                }
            }
            catch (WebException ex)
            {

            }
            catch (NullReferenceException ex)
            {

            }
            string dcfduid = cookieResponse.Headers["Set-Cookie"].After("__dcfduid=").Before(";");
            string sdcfduid = cookieResponse.Headers["Set-Cookie"].After("__sdcfduid=").Before(";");
            string completeCookie = "__dcfduid=" + dcfduid + "; " + "__sdcfduid=" + sdcfduid + "; " + "locale=us";
            while (running && monitorCode == 200)
            {
                HttpWebResponse getChannelResponse = null;
                string channelContent = null;
                HttpWebRequest monitor = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com/api/v9/channels/" + targetChannel + "/messages?limit=1"));
                string monitorProxy = GetNextProxyinQueue();
                switch (proxyType)
                {
                    case null:
                        break;
                    case "ip:port":
                        monitor.Proxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                        break;
                    case "user:pass:ip:port":
                        WebProxy sendProxy = new WebProxy("http://" + monitorProxy.Split(":")[2] + ":" + monitorProxy.Split(":")[3]);
                        sendProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[0], monitorProxy.Split(":")[1]);
                        monitor.Proxy = sendProxy;
                        break;
                    case "ip:port:user:pass":
                        WebProxy newProxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                        newProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[2], monitorProxy.Split(":")[3]);
                        monitor.Proxy = newProxy;
                        break;
                }
                monitor.Method = "GET";
                monitor.Headers["Connection"] = "keep-alive";
                monitor.Accept = "*/*";
                monitor.Headers["Accept-Encoding"] = "*/*";
                monitor.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                monitor.Headers["X-Debug-Options"] = "bugReporterEnabled";
                monitor.Headers["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRGlzY29yZCBDbGllbnQiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfdmVyc2lvbiI6IjEuMC45MDAzIiwib3NfdmVyc2lvbiI6IjEwLjAuMTkwNDIiLCJvc19hcmNoIjoieDY0Iiwic3lzdGVtX2xvY2FsZSI6ImVuLVVTIiwiY2xpZW50X2J1aWxkX251bWJlciI6MTEwNDUxLCJjbGllbnRfZXZlbnRfc291cmNlIjpudWxsfQ==";
                monitor.Headers["X-Discord-Locale"] = "en-US";
                monitor.Headers["Authorization"] = tokens[currentToken];
                monitor.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9003 Chrome/91.0.4472.164 Electron/13.4.0 Safari/537.36";
                monitor.Headers["Sec-Fetch-Site"] = "same-origin";
                monitor.Headers["Sec-Fetch-Mode"] = "cors";
                monitor.Headers["Sec-Fetch-Dest"] = "empty";
                monitor.Referer = "https://discord.com/channels/@me";
                monitor.Headers["Cookie"] = completeCookie;
                try
                {
                    monitorResponse = (HttpWebResponse)monitor.GetResponse();
                    using (StreamReader reader = new StreamReader(monitorResponse.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {

                }
                catch (NullReferenceException ex)
                {

                }
                if (monitorResponse.StatusCode == HttpStatusCode.OK)
                {
                    monitorCode = 200;
                }
                else
                {
                    monitorCode = 400;
                }
                if (id == null)
                {
                    id = content.After("\"id\": \"").Before("\",");
                }
                else if (id != content.After("\"id\": \"").Before("\","))
                {
                    Thread.Sleep(105);
                    id = content.After("\"id\": \"").Before("\",");
                    HttpWebRequest getChannel = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com/api/v9/users/@me/channels"));
                    switch (proxyType)
                    {
                        case null:
                            break;
                        case "ip:port":
                            getChannel.Proxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                            break;
                        case "user:pass:ip:port":
                            WebProxy sendProxy = new WebProxy("http://" + monitorProxy.Split(":")[2] + ":" + monitorProxy.Split(":")[3]);
                            sendProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[0], monitorProxy.Split(":")[1]);
                            getChannel.Proxy = sendProxy;
                            break;
                        case "ip:port:user:pass":
                            WebProxy newProxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                            newProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[2], monitorProxy.Split(":")[3]);
                            getChannel.Proxy = newProxy;
                            break;
                    }
                    getChannel.Method = "POST";
                    getChannel.Headers["Connection"] = "keep-alive";
                    getChannel.Accept = "*/*";
                    getChannel.Headers["Accept-Encoding"] = "*/*";
                    getChannel.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                    getChannel.Headers["X-Debug-Options"] = "bugReporterEnabled";
                    getChannel.Headers["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRGlzY29yZCBDbGllbnQiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfdmVyc2lvbiI6IjEuMC45MDAzIiwib3NfdmVyc2lvbiI6IjEwLjAuMTkwNDIiLCJvc19hcmNoIjoieDY0Iiwic3lzdGVtX2xvY2FsZSI6ImVuLVVTIiwiY2xpZW50X2J1aWxkX251bWJlciI6MTEwNDUxLCJjbGllbnRfZXZlbnRfc291cmNlIjpudWxsfQ==";
                    getChannel.Headers["X-Discord-Locale"] = "en-US";
                    getChannel.Headers["X-Context-Properties"] = "e30=";
                    getChannel.Headers["Authorization"] = tokens[currentToken];
                    getChannel.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9003 Chrome/91.0.4472.164 Electron/13.4.0 Safari/537.36";
                    getChannel.Headers["Sec-Fetch-Site"] = "same-origin";
                    getChannel.Headers["Sec-Fetch-Mode"] = "cors";
                    getChannel.Headers["Sec-Fetch-Dest"] = "empty";
                    getChannel.Referer = "https://discord.com/channels/@me";
                    getChannel.Headers["Cookie"] = completeCookie;
                    getChannel.ContentType = "application/json";
                    var postData = "{\"recipients\":[\"" + id + "\"]}";
                    var data = Encoding.ASCII.GetBytes(postData);
                    getChannel.ContentLength = data.Length;
                    using (var stream = getChannel.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    try
                    {
                        getChannelResponse = (HttpWebResponse)getChannel.GetResponse();
                        using (StreamReader reader = new StreamReader(getChannelResponse.GetResponseStream()))
                        {
                            channelContent = reader.ReadToEnd();
                        }
                    }
                    catch (WebException ex)
                    {

                    }
                    catch (NullReferenceException ex)
                    {

                    }
                    string dmChannelID = betweenStrings(channelContent, "{\"id\": \"", "\", \"type\"");
                    Debug.WriteLine(channelContent);
                    Thread.Sleep(112);
                    HttpWebRequest sendDM = (HttpWebRequest)WebRequest.Create(new Uri("https://discord.com/api/v9/channels/" + dmChannelID + "/messages"));
                    switch (proxyType)
                    {
                        case null:
                            break;
                        case "ip:port":
                            sendDM.Proxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                            break;
                        case "user:pass:ip:port":
                            WebProxy sendProxy = new WebProxy("http://" + monitorProxy.Split(":")[2] + ":" + monitorProxy.Split(":")[3]);
                            sendProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[0], monitorProxy.Split(":")[1]);
                            sendDM.Proxy = sendProxy;
                            break;
                        case "ip:port:user:pass":
                            WebProxy newProxy = new WebProxy("http://" + monitorProxy.Split(":")[0] + ":" + monitorProxy.Split(":")[1]);
                            newProxy.Credentials = new NetworkCredential(monitorProxy.Split(":")[2], monitorProxy.Split(":")[3]);
                            sendDM.Proxy = newProxy;
                            break;
                    }
                    sendDM.Method = "POST";
                    sendDM.Headers["Connection"] = "keep-alive";
                    sendDM.Accept = "*/*";
                    sendDM.Headers["Accept-Encoding"] = "*/*";
                    sendDM.Headers["Accept-Language"] = "en-US,zh-Hans-CN;q=0.9,pa;q=0.8,bn-IN;q=0.7";
                    sendDM.Headers["X-Debug-Options"] = "bugReporterEnabled";
                    sendDM.Headers["X-Super-Properties"] = "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRGlzY29yZCBDbGllbnQiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfdmVyc2lvbiI6IjEuMC45MDAzIiwib3NfdmVyc2lvbiI6IjEwLjAuMTkwNDIiLCJvc19hcmNoIjoieDY0Iiwic3lzdGVtX2xvY2FsZSI6ImVuLVVTIiwiY2xpZW50X2J1aWxkX251bWJlciI6MTEwNDUxLCJjbGllbnRfZXZlbnRfc291cmNlIjpudWxsfQ==";
                    sendDM.Headers["X-Discord-Locale"] = "en-US";
                    sendDM.Headers["Authorization"] = tokens[currentToken];
                    sendDM.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9003 Chrome/91.0.4472.164 Electron/13.4.0 Safari/537.36";
                    sendDM.Headers["Sec-Fetch-Site"] = "same-origin";
                    sendDM.Headers["Sec-Fetch-Mode"] = "cors";
                    sendDM.Headers["Sec-Fetch-Dest"] = "empty";
                    sendDM.Referer = "https://discord.com/channels/@me";
                    sendDM.Headers["Cookie"] = completeCookie;
                    sendDM.ContentType = "application/json";
                    string postData2;
                    HttpWebResponse DMresponse = null;
                    postData2 = "{\"content\":\"" + sendBody + "\",\"tts\": false}";
                    var data2 = Encoding.ASCII.GetBytes(postData2);
                    sendDM.ContentLength = data2.Length;
                    using (var stream = sendDM.GetRequestStream())
                    {
                        stream.Write(data2, 0, data2.Length);
                    }
                    try
                    {
                        DMresponse = (HttpWebResponse)sendDM.GetResponse();
                    }
                    catch (WebException ex)
                    {

                    }
                    catch (NullReferenceException ex)
                    {

                    }
                    if (DMresponse.StatusCode == HttpStatusCode.OK)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                        Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nDM sent to user '" + dmChannelID + "'!"; }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text = ""; }));
                        Application.Current.Dispatcher.Invoke(new Action(() => { Output.Text += "\nFailed to DM user '" + dmChannelID + "'."; }));
                    }
                }
                Thread.Sleep(2500);
            }
        }

        private void message_LostFocus(object sender, RoutedEventArgs e)
        {
            sendBody = message.Text;
        }

    }
}
