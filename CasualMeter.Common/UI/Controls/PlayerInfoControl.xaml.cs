using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tera.DamageMeter;

namespace CasualMeter.Common.UI.Controls
{
    /// <summary>
    /// Interaction logic for PlayerInfoControl.xaml
    /// </summary>
    public partial class PlayerInfoControl
    {
        public static readonly DependencyProperty PlayerInfoProperty =
        DependencyProperty.Register("PlayerInfo", typeof(PlayerInfo),
            typeof(PlayerInfoControl), new UIPropertyMetadata(null));

        public PlayerInfo PlayerInfo
        {
            get { return (PlayerInfo)GetValue(PlayerInfoProperty); }
            set { SetValue(PlayerInfoProperty, value); }
        }

        public PlayerInfoControl()
        {
            InitializeComponent();
        }
    }
}
