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
            DataContext = ViewModel = new ShellViewModel(); //temp
            ShellViewModel.Initialize();

            //load window position
            Left = SettingsHelper.Instance.Settings.WindowLeft;
            Top = SettingsHelper.Instance.Settings.WindowTop;

            CasualMessenger.Instance.Messenger.Register<PrepareExitMessage>(this, PrepareClose);
        }

        private void PrepareClose(PrepareExitMessage message)
        {
            SettingsHelper.Instance.Settings.WindowLeft = Left;
            SettingsHelper.Instance.Settings.WindowTop = Top;
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
                    var v = new SkillBreakdownView(vm);
                    v.Show();
                }
            }
        }
    }
}
