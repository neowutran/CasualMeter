using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CasualMeter.Conductors;
using CasualMeter.Helpers;
using Lunyx.Common;

namespace CasualMeter
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        private ProcessInfo.WinEventDelegate dele;//leave this here to prevent garbage collection

        public ShellView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            //set cursor
            Cursor = new Cursor(new MemoryStream(Properties.Resources.Arrow));

            //listen to window focus changed event
            dele = (OnFocusedWindowChanged);
            ProcessInfo.RegisterWindowFocusEvent(dele);

            //initialize helpers
            SettingsHelper.Instance.Initialize();
            HotkeyHelper.Instance.Initialize();

            //subscribe to messages
            CasualMessenger.Instance.Messenger.Register<WindowVisibilityMessage>(this, m => SetVisibility(m.IsVisible));
        }

        private void SetVisibility(bool isVisible)
        {
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnFocusedWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //if (!TabEnabled)
            //{
            //    if (IsLoLActive())
            //        this.Visibility = Visibility.Visible;
            //    else
            //        this.Visibility = Visibility.Hidden;
            //}
            
        }

        /// <summary>
        /// Attach to MouseDown event of controls to allow moving of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
