using System;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FauxSealLauncher.Cores
{
    public class HanGame : Core
    {
        string mainURI = "http://lostsaga.hangame.com/main/main.asp";
        string loginURI = "https://id.hangame.com/wlogin.nhn?popup=false&adult=false&nxtURL=http%3A//lostsaga.hangame.com/main/main.asp";
        string playURI = "http://lostsaga.hangame.com/play/playUrl.asp";

        string id, pw;

        Action callback = null;

        public HanGame(WebControl webControl) : base(webControl)
        {
            webControl.LoadingFrameComplete -= onFinishLoading;
            webControl.LoadingFrameComplete += onFinishLoading;
        }

        public override string GetServer()
        {
            return "hangame";
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
                webControl.ExecuteJavascript(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "turtle2", id));
                webControl.ExecuteJavascript(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "earthworm2", pw));
                webControl.ExecuteJavascript(string.Format("document.getElementById(\"{0}\").click();", "btnLoginImg"));
            }
            else if (Regex.Match(e.OriginalString, @"http://lostsaga.hangame.com/intro/\d+/\d+_intro.asp").Success)
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
