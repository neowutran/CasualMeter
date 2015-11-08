using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Helpers;
using Lunyx.Common;
using Lunyx.Common.UI.Wpf;
using Lunyx.Common.UI.Wpf.Controls;

namespace CasualMeter.Common.UI.Controls
{
    public class CasualMeterWindow : ClickThroughWindow
    {
        private ProcessInfo.WinEventDelegate dele;//leave this here to prevent garbage collection

        protected ViewModelBase ViewModel { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //set cursor
            Cursor = new Cursor(new MemoryStream(Properties.Resources.Arrow));

            //listen to window focus changed event
            dele = (OnFocusedWindowChanged);
            ProcessInfo.RegisterWindowFocusEvent(dele);   
        }

        /// <summary>
        /// Attach to MouseDown event of controls to allow moving of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MoveControl(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void SetVisibility(bool isVisible)
        {
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnFocusedWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            SetVisibility(ProcessHelper.Instance.IsTeraActive);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //unregister messages
            CasualMessenger.Instance.Messenger.Unregister(ViewModel);
            CasualMessenger.Instance.Messenger.Unregister(this);

            ViewModel = null;
        }
    }
}
