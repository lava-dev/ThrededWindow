using OtherThreadWindow.Help;
using System;
using System.Windows;
using System.Windows.Input;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace OtherThreadWindow.BaseWindow
{
    public class CustomWindow : CustomDialogWindow
    {
        private Visibility _normalizeWindowBtnVisibility;
        private Visibility _maximizeWindowBtnVisibility;

        public Visibility MaximizeWindowBtnVisibility
        {
            get { return _maximizeWindowBtnVisibility; }
            set
            {
                if (_maximizeWindowBtnVisibility != value)
                {
                    _maximizeWindowBtnVisibility = value;
                    OnPropertyChanged(nameof(MaximizeWindowBtnVisibility));
                }
            }
        }

        public Visibility NormalizeWindowBtnVisibility
        {
            get { return _normalizeWindowBtnVisibility; }
            set
            {
                if (_normalizeWindowBtnVisibility != value)
                {
                    _normalizeWindowBtnVisibility = value;
                    OnPropertyChanged(nameof(NormalizeWindowBtnVisibility));
                }
            }
        }

        public CustomWindow()
        {
            this.SourceInitialized += Window_OnSourceInitialized;
            this.PreviewMouseMove += Window_OnPreviewMouseMove;
            this.SizeChanged += Window_OnSizeChanged;
        }

        public void Window_OnSourceInitialized(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeWindowBtnVisibility = Visibility.Collapsed;
                NormalizeWindowBtnVisibility = Visibility.Visible;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MaximizeWindowBtnVisibility = Visibility.Visible;
                NormalizeWindowBtnVisibility = Visibility.Collapsed;
            }
        }

        public void Window_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        public void Window_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStateHelper.SetWindowMaximized(this);
                WindowStateHelper.BlockStateChange = true;

                var screen = ScreenFinder.FindAppropriateScreen(this);
                if (screen != null)
                {
                    Top = screen.WorkingArea.Top;
                    Left = screen.WorkingArea.Left;
                    Width = screen.WorkingArea.Width;
                    Height = screen.WorkingArea.Height;
                }

                ShowRestoreDownButton();
            }
            else
            {
                if (WindowStateHelper.BlockStateChange)
                {
                    WindowStateHelper.BlockStateChange = false;
                    return;
                }

                WindowStateHelper.UpdateLastKnownNormalSize(Width, Height);
                WindowStateHelper.UpdateLastKnownLocation(Top, Left);
            }
        }

        public override void OnDragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (WindowStateHelper.IsMaximized)
            {
                WindowStateHelper.SetWindowSizeToNormal(this, true);
                ShowMaximumWindowButton();

                DragMove();
            }
            else
            {
                DragMove();
            }

            WindowStateHelper.UpdateLastKnownLocation(Top, Left);
        }

        public void ButtonMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void ButtonMaximize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            ShowRestoreDownButton();
        }

        public void ShowRestoreDownButton()
        {
            MaximizeWindowBtnVisibility = Visibility.Collapsed;
            NormalizeWindowBtnVisibility = Visibility.Visible;
        }

        public void ShowMaximumWindowButton()
        {
            NormalizeWindowBtnVisibility = Visibility.Collapsed;
            MaximizeWindowBtnVisibility = Visibility.Visible;
        }

        public void ButtonRestoreDown_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMaximumWindowButton();
            WindowStateHelper.SetWindowSizeToNormal(this);
        }

        private void SwitchState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        ShowRestoreDownButton();
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowStateHelper.SetWindowSizeToNormal(this);
                        ShowMaximumWindowButton();
                        break;
                    }
            }
        }

        public void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((ResizeMode == ResizeMode.CanResize) ||
                (ResizeMode == ResizeMode.CanResizeWithGrip))
            {
                SwitchState();
                return;
            }
        }

        ~CustomWindow()
        {
            this.SourceInitialized -= Window_OnSourceInitialized;
            this.PreviewMouseMove -= Window_OnPreviewMouseMove;
            this.SizeChanged -= Window_OnSizeChanged;
        }
    }
}
