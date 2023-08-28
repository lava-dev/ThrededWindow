using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OtherThreadWindow.BaseWindow
{
    public class OtherThreadWindowBase : Window, IDisposable
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        internal WindowInteropHelper _wndHelper;

        public OtherThreadWindowBase()
        {
            Loaded += OtherThreadWindowBase_Loaded;
        }

        private void OtherThreadWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            _wndHelper = new WindowInteropHelper(this);
            int exStyle = GetWindowLong(_wndHelper.Handle, GWL_EXSTYLE);
            SetWindowLong(_wndHelper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr window, int index);

        public void Dispose()
        {
            try
            {
                Loaded -= OtherThreadWindowBase_Loaded;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                // log it
            }
        }
    }
}
