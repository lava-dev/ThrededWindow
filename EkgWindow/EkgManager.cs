using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

namespace OtherThreadWindow.EkgWindow
{
    internal class EkgManager
    {
        private bool _isStarting;

        internal EkgManager(Dictionary<string, double> settings, bool isStarting)
        {
            Current = this;
            Settings = settings;
            _isStarting = isStarting;

            MakeEkgOnDifferentThread();
        }

        public static EkgManager Current { get; set; }
        public Dictionary<string, double> Settings { get; set; }
        public EkgController EkgController { get; private set; }

        internal void MakeEkgOnDifferentThread()
        {
            try
            {
                Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }
            catch (Exception ex)
            {
                // log it or handle
            }
        }

        private void ThreadStartingPoint()
        {
            try
            {
                EkgController = new EkgController();

                UpdateWindowSettings();
                if (_isStarting)
                {
                    Thread.Sleep(1900);

                    MainWindow.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.Current.Topmost = true;
                        MainWindow.Current.Topmost = false;
                        MainWindow.Current.Activate();
                    });

                    _isStarting = false;
                }

                EkgController.Show();

                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception ex)
            {
                // log it or handle
            }
        }

        private void UpdateWindowSettings()
        {
            EkgController.Width = Settings["Width"];
            EkgController.Height = Settings["Height"];
            EkgController.Left = Settings["Left"];
            EkgController.Top = Settings["Top"];
        }

        internal void WindowSettingsChanged()
        {
            try
            {
                if (EkgController == null) return;
                EkgController.Dispatcher.Invoke(() => UpdateWindowSettings());
            }
            catch (Exception ex)
            {
                // log it or handle
            }
        }

        internal void KillEkg()
        {
            try
            {
                EkgController.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                {
                    EkgController.Close();
                    if (EkgController != null)
                    {
                        EkgController.Dispose();
                    }
                }));

                EkgController = null;
            }
            catch (Exception ex)
            {
                // log it or handle
            }
        }

        internal void WindowActivated()
        {
            if (_isStarting) return;

            try
            {
                if (EkgController == null) return;
                if (EkgController.Dispatcher.CheckAccess())
                {
                    EkgController.Show();
                }
                else
                {
                    EkgController.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                    {
                        EkgController.Topmost = true;
                        EkgController.Topmost = false;
                        EkgController.Focus();
                    }));
                }
            }
            catch (Exception ex)
            {
                // log it or handle
            }
        }
    }
}
