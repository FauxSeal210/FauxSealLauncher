using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Windows.Controls;

namespace FauxSealLauncher.Cores
{
    public abstract class Core
    {
        protected WebControl webControl;

        public Core(WebControl webControl)
        {
            this.webControl = webControl;
        }

        public abstract void StartGame();

        public abstract void LogIn(string id, string pw, Action action);

        public abstract string GetServer();
    }
}
