using System;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FauxSealLauncher.Cores
{
    public class MGame : Core
    {
        string loginURI = "http://www.mgame.com/ulnauth/login_form_big.mgame?tu=http://lostsaga.mgame.com";
        string mainURI = "http://lostsaga.mgame.com/main/main.asp";
        string playURI = "http://lostsaga.mgame.com/play/playUrl.asp";

        string id, pw;

        Action callback = null;

        public MGame(WebControl webControl) : base(webControl)
        {
            webControl.DocumentReady -= onFinishLoading;
            webControl.DocumentReady += onFinishLoading;
        }

        public override string GetServer()
        {
            return "mgame";
        }

        public override void LogIn(string id, string pw, Action action)
        {
            this.callback = action;
            this.id = id;
            this.pw = pw;
            webControl.Source = loginURI.ToUri();
        }

        public override void StartGame()
        {
            webControl.Source = mainURI.ToUri();
        }

        private void onFinishLoading(object sender, UrlEventArgs e)
        {
            if (e.OriginalString.Equals(loginURI))
            {
                Thread.Sleep(1000);
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#_mgid_enc", id));
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#_mgpwd_enc", pw));
                webControl.ExecuteJavascript("chkSubmit();");
            }
            else if (Regex.Match(e.OriginalString, @"http://lostsaga.mgame.com/intro/\d+/\d+_intro.asp").Success)
            {
                callback.Invoke();
            }
            else if (e.OriginalString.Equals(mainURI))
            {
                Thread.Sleep(1000);
                webControl.ExecuteJavascript("GameStart();");
            }
            else if (e.OriginalString.Equals(playURI))
            {
                Thread.Sleep(1500);
                if (webControl.IsDocumentReady)
                {
                    string launchURI = webControl.ExecuteJavascriptWithResult(string.Format("$(\"{0}\").attr(\"{1}\");", "#playgame", "href"));
                    try
                    {
                        Process.Start(launchURI);
                        webControl.DocumentReady -= onFinishLoading;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
