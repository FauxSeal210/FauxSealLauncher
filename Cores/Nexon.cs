using System;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FauxSealLauncher.Cores
{
    public class Nexon : Core
    {
        string loginURI = "https://nxlogin.nexon.com/common/login.aspx?redirect=http%3a%2f%2flostsaga.nexon.com";
        string mainURI = "http://lostsaga.nexon.com/main/main.asp";
        string playURI = "http://lostsaga.nexon.com/play/playUrl.asp";

        string id, pw;

        Action callback = null;

        public Nexon(WebControl webControl) : base(webControl)
        {
            webControl.LoadingFrameComplete += onFinishLoading;
            webControl.LoadingFrameComplete -= onFinishLoading;
        }

        public override string GetServer()
        {
            return "nexon";
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
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#txtNexonID", id));
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#txtPWD", pw));
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").click();", "#btnLogin", id));
            }
            else if (Regex.Match(e.OriginalString, @"http://lostsaga.nexon.com/intro/\d+/\d+_intro.asp").Success)
            {
                callback.Invoke();
            }
            else if (e.OriginalString.Equals(mainURI))
            {
                webControl.ExecuteJavascript("GameStart();");
            }
            else if (e.OriginalString.Equals(playURI))
            {
                if (webControl.IsDocumentReady)
                {
                    string launchURI = webControl.ExecuteJavascriptWithResult(string.Format("$(\"{0}\").attr(\"{1}\");", "#playgame", "href"));
                    try
                    {
                        Process.Start(launchURI);
                        webControl.LoadingFrameComplete -= onFinishLoading;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

    }
}
