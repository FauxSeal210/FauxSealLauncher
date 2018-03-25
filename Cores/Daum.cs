using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FauxSealLauncher.Cores
{
    public class Daum : Core
    {
        string loginURI = "https://logins.daum.net/accounts/loginform.do?url=http%3A%2F%2Flostsaga.game.daum.net";
        string mainURI = "http://lostsaga.game.daum.net/main/main.asp";
        string playURI = "http://lostsaga.game.daum.net/play/playUrl.asp";

        string id, pw;

        Action callback;

        public Daum(WebControl webControl) : base(webControl)
        {
            webControl.DocumentReady -= onFinishLoading;
            webControl.DocumentReady += onFinishLoading;
        }

        public override string GetServer()
        {
            return "daum";
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
                Thread.Sleep(1500);
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#id", id));
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").val(\"{1}\");", "#inputPwd", pw));
                webControl.ExecuteJavascript(string.Format("$(\"{0}\").click();", "#loginBtn"));
            }
            else if (Regex.Match(e.OriginalString, @"http://lostsaga.game.daum.net/intro/\d+/\d+_intro.asp").Success)
            {
                callback.Invoke();
            }
            else if (e.OriginalString.Equals("http://game.daum.net/bridge/?url=http%3A%2F%2Flostsaga.game.daum.net%2Fmain%2Fmain.asp"))
            {
                Thread.Sleep(1000);
                webControl.ExecuteJavascript(string.Format("location.href = \"{0}\";", playURI));
            }
            else if (e.OriginalString.Equals(playURI))
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
