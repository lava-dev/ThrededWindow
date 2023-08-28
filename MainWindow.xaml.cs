using OtherThreadWindow.BaseWindow;
using OtherThreadWindow.EkgWindow;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace OtherThreadWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomWindow
    {
        public MainWindow()
        {
            Current = this;

            InitializeComponent();

            LocationChanged += WindowLocationChangedHandler;
        }

        private void WindowLocationChangedHandler(object sender, EventArgs e)
        {
            GetEkgSettings();
        }

        public static MainWindow Current { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new DialogWindow();
            dialogWindow.Width = 900;
            dialogWindow.Height = 600;
            dialogWindow.Owner = this;
            dialogWindow.Title = "Custom Dialog Window Title";
            
            dialogWindow.ShowDialog();
            { 
                // dialog window closed
                // do whatever after
            }
        }

        private void GetEkgSettings()
        {
            if (EkgManager.Current == null) return;
            var content = (FrameworkElement)this.GridForOtherThreadWindow;
            var currentPosition = content.PointToScreen(new Point(0, 0));
            var widthFactor = GetWindowsScalingWidth();
            var heightFactor = GetWindowsScalingHeight();

            EkgManager.Current.Settings["Left"] = currentPosition.X / widthFactor;
            EkgManager.Current.Settings["Top"] = currentPosition.Y / heightFactor;
            EkgManager.Current.Settings["Width"] = content.ActualWidth * widthFactor;
            EkgManager.Current.Settings["Height"] = content.ActualHeight * heightFactor;
            EkgManager.Current.WindowSettingsChanged();
        }

        internal static double GetWindowsScalingWidth()
        {
            return Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
        }

        internal static double GetWindowsScalingHeight()
        {
            return Screen.PrimaryScreen.Bounds.Height / SystemParameters.PrimaryScreenHeight;
        }

        private void MainWindoLoaded(object sender, RoutedEventArgs e)
        {
            var content = (FrameworkElement)this.GridForOtherThreadWindow;
            var currentPosition = content.PointToScreen(new Point(0, 0));
            var widthFactor = GetWindowsScalingWidth();
            var heightFactor = GetWindowsScalingHeight();

            var settings = new Dictionary<string, double>();

            settings["Left"] = currentPosition.X / widthFactor;
            settings["Top"] = currentPosition.Y / heightFactor;
            settings["Width"] = content.ActualWidth * widthFactor;
            settings["Height"] = content.ActualHeight * heightFactor;

            var ekg = new EkgManager(settings, true);
        }

        private void MainRootActivated(object sender, EventArgs e)
        {
            if (EkgManager.Current == null) return;

            EkgActivator();
        }

        private void MainRootStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                // remove ekg
                if (EkgManager.Current == null) return;
                EkgManager.Current.KillEkg();
            }
            else
            {
                // make ekg
                if (EkgManager.Current != null && EkgManager.Current.EkgController != null) return;
                EkgManager.Current.MakeEkgOnDifferentThread();
                if (this.WindowState != WindowState.Maximized)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        this.Dispatcher.Invoke(() =>
                        {
                            this.Left += 0.1;
                        });
                    });
                }
            }
        }

        private void EkgActivator(bool deatach = false)
        {
            if (EkgManager.Current == null) return;
            if (!deatach)
            {
                EkgManager.Current.WindowActivated();
            }
        }

        private void ContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GetEkgSettings();
        }

        /// <summary>
        /// Spacially for check that MainWindow stuck but EKG still works.
        /// </summary>
        private void MakeYourMainWindowBusy(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5000);
        }

        ~MainWindow()
        {
            LocationChanged -= WindowLocationChangedHandler;
        }
    }
}