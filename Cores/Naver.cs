using System;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FauxSealLauncher.Cores
{
    public class Naver : Core
    {
        string loginURI = "https://nid.naver.com/nidlogin.login?url=http%3A%2F%2Flostsaga.playnetwork.co.kr";
        string mainURI = "http://lostsaga.playnetwork.co.kr/main/main.asp";
        string playURI = "http://lostsaga.playnetwork.co.kr/play/playUrl.asp";

        string id, pw;

        Action callback;

        public Naver(WebControl webControl) : base(webControl)
        {
            webControl.LoadingFrameComplete -= onFinishLoading;
            webControl.LoadingFrameComplete += onFinishLoading;
        }

        public override string GetServer()
        {
            return "naver";
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
                webControl.ExecuteJavascript(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "id", id));
                webControl.ExecuteJavascript(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "pw", pw));
                webControl.ExecuteJavascript(string.Format("document.getElementsByClassName(\"{0}\")[0].click();", "btn_global"));
            }
            else if (Regex.Match(e.OriginalString, @"http://lostsaga.playnetwork.co.kr/intro/\d+/\d+_intro.asp").Success)
            {
                callback.Invoke();
            }
            else if (e.OriginalString.Equals("http://game.naver.com/game/mirror.nhn?gurl=http%3A%2F%2Flostsaga.playnetwork.co.kr%2Fmain%2Fmain.asp&gameId=P_LOSA"))
            {
                webControl.ExecuteJavascript(string.Format("location.href = \"{0}\";", playURI));
            }
            else if (e.OriginalString.Equals(playURI))
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
