using System;
using System.Windows;
using System.Windows.Input;

using FauxSealLauncher.Cores;
using System.IO;
using IWshRuntimeLibrary;

namespace FauxSealLauncher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {      
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            if (System.IO.File.Exists(path + "\\server.txt"))
            {
                string server = System.IO.File.ReadAllText(path + "\\server.txt");
                if (server.Equals("naver"))
                {
                    Naver.IsChecked = true;
                }
                else if (server.Equals("daum"))
                {
                    Daum.IsChecked = true;
                }
                else if (server.Equals("nexon"))
                {
                    Nexon.IsChecked = true;
                }
                else if (server.Equals("mgame"))
                {
                    MGame.IsChecked = true;
                }
                else if (server.Equals("hangame"))
                {
                    HanGame.IsChecked = true;
                }
            }
            this.KeyDown += new KeyEventHandler(Key_Down);
            MessageBox.Show("제작자 : FauxSeal210 (로스트사가 타임게이트)", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            ID.Focus();
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            Core core = null;
            if (Nexon.IsChecked.Value)
            {
                core = new Nexon(webControl);
            }
            else if (Naver.IsChecked.Value)
            {
                core = new Naver(webControl);
            }
            else if (Daum.IsChecked.Value)
            {
                core = new Daum(webControl);
            }
            else if (MGame.IsChecked.Value)
            {
                core = new MGame(webControl);
            }
            else if (HanGame.IsChecked.Value)
            {
                core = new HanGame(webControl);
            }

            if (core == null)
            {
                MessageBox.Show("로스트사가 서버를 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Favorite.IsChecked.Value)
            {
                SetFavorite(core.GetServer());
            }

            core.LogIn(ID.Text, PW.Password, () =>
            {
                LoGin.IsEnabled = false;
                Naver.IsEnabled = false;
                Daum.IsEnabled = false;
                Nexon.IsEnabled = false;
                MGame.IsEnabled = false;
                HanGame.IsEnabled = false;
                Start.IsEnabled = true;
                Favorite.IsEnabled = false;
                MessageBox.Show("로그인에 성공하였습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            });         
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Core core = null;
            if (Nexon.IsChecked.Value)
            {
                core = new Nexon(webControl);
            }
            else if (Naver.IsChecked.Value)
            {
                core = new Naver(webControl);
            }
            else if (Daum.IsChecked.Value)
            {
                core = new Daum(webControl);
            }
            else if (MGame.IsChecked.Value)
            {
                core = new MGame(webControl);
            }
            else if (HanGame.IsChecked.Value)
            {
                core = new HanGame(webControl);
            }
            core.StartGame();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CreateShortcut();
            MessageBox.Show("바탕화면에 바로가기를 생성했습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/FauxSeal210/FauxSealLauncher");
        }

        private void CreateShortcut()
        {
            string link = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "로스트사가 런쳐.lnk";
            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;
            shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
            shortcut.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
            shortcut.Save();
        }

        private void SetFavorite(string server)
        {
            string path = Directory.GetCurrentDirectory();
            System.IO.File.WriteAllText(path + "\\server.txt", server);
        }        
    }
}
