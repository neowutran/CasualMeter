using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Conductors.Messages;
using CasualMeter.Common.Helpers;
using CasualMeter.Common.UI.Controls;
using CasualMeter.ViewModels;
using CasualMeter.Views;
using Lunyx.Common;
using Lunyx.Common.UI.Wpf.Extensions;
using Tera.DamageMeter;

namespace CasualMeter
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        public ShellViewModel ShellViewModel => ViewModel as ShellViewModel;

        public ShellView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            //ensure initialization of helpers
            SettingsHelper.Instance.Initialize();
            HotkeyHelper.Instance.Initialize();

            //initialize viewmodel
            DataContext = ViewModel = new ShellViewModel();
            ShellViewModel.Initialize();

            //load window position
            Left = SettingsHelper.Instance.Settings.WindowLeft;
            Top = SettingsHelper.Instance.Settings.WindowTop;
            OpacityScaleSlider.Value = SettingsHelper.Instance.Settings.Opacity;
            UiScaleSlider.Value = SettingsHelper.Instance.Settings.UiScale;

            CasualMessenger.Instance.Messenger.Register<PrepareExitMessage>(this, PrepareClose);
        }

        private void PrepareClose(PrepareExitMessage message)
        {
            SettingsHelper.Instance.Settings.WindowLeft = Left;
            SettingsHelper.Instance.Settings.WindowTop = Top;
            SettingsHelper.Instance.Settings.Opacity = OpacityScaleSlider.Value;
            SettingsHelper.Instance.Settings.UiScale = UiScaleSlider.Value;
            SettingsHelper.Instance.Save();

            CasualMessenger.Instance.Messenger.Send(new ExitMessage());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PrepareClose(null);

            base.OnClosing(e);
        }

        private void SkillInfo_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var control = sender as PlayerInfoControl;
                var playerInfo = control?.PlayerInfo;
                if (playerInfo != null)
                {
                    var vm = new SkillBreakdownViewModel(playerInfo);
                    var v = new SkillBreakdownView(vm)
                    {
                        Owner = this
                    };
                    var ownedWindows = OwnedWindows.Cast<Window>().Where(w => w.IsVisible).ToList();
                    if (!ownedWindows.Any())
                    {
                        v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                    else
                    {
                        v.WindowStartupLocation = WindowStartupLocation.Manual;
                        v.Left = ownedWindows.Max(w => w.Left) + 27;
                        v.Top = ownedWindows.Max(w => w.Top) + 27;
                    }
                    v.Show();
                }
            }
        }
    }
}
