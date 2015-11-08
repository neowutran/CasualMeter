using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using CasualMeter.ViewModels;
using Tera.DamageMeter;

namespace CasualMeter.Views
{
    /// <summary>
    /// Interaction logic for SkillBreakdownView.xaml
    /// </summary>
    public partial class SkillBreakdownView
    {
        public SkillBreakdownViewModel SkillBreakdownViewModel => ViewModel as SkillBreakdownViewModel;

        public SkillBreakdownView(SkillBreakdownViewModel viewModel)
        {
            InitializeComponent();

            DataContext = ViewModel = viewModel;

            //manually switch first view, then register for all future updates
            SwitchView(SkillBreakdownViewModel?.SelectedCollectionView.Key.ToString());
            CasualMessenger.Instance.Messenger.Register<UpdateSkillBreakdownViewMessage>(this, ViewModel,
                UpdateSkillBreakdownView);
            CasualMessenger.Instance.Messenger.Register<ScrollPlayerStatsMessage>(this, ViewModel,
                ScrollPlayerStats);
        }

        private void ScrollPlayerStats(ScrollPlayerStatsMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                if (SkillBreakdownViewModel?.SelectedCollectionView.Key == SkillViewType.FlatView)
                {
                    if (SkillResultsGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(SkillResultsGrid, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            scroll?.ScrollToEnd();
                        }
                    }
                }
            });
        }

        private void UpdateSkillBreakdownView(UpdateSkillBreakdownViewMessage message)
        {
            Dispatcher.Invoke(() => SwitchView(message.ViewKey));
        }

        private void SwitchView(string viewKey)
        {
            var resource = LayoutGrid.Resources[viewKey];

            var view = resource as CollectionViewSource;
            if (view == null) return;

            SkillResultsGrid.Columns.Clear();
            SkillResultsGrid.DataContext = view;
            switch (viewKey)
            {   //assign columns
                case nameof(SkillViewType.FlatView):
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Time",
                        Binding = new Binding(nameof(SkillResult.Time)),
                        SortDirection = ListSortDirection.Ascending
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Skill Id",
                        Binding = new Binding(nameof(SkillResult.SkillId))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Skill Name",
                        Binding = new Binding(nameof(SkillResult.SkillName))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Amount",
                        Binding = new Binding(nameof(SkillResult.Amount))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Is Crit?",
                        Binding = new Binding(nameof(SkillResult.IsCritical))
                    });
                    break;
                case nameof(SkillViewType.AggregatedSkillIdView):
                case nameof(SkillViewType.AggregatedSkillNameView):
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Skill",
                        Binding = new Binding(nameof(AggregatedSkillResult.DisplayName))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Hits",
                        Binding = new Binding(nameof(AggregatedSkillResult.Hits))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Amount",
                        Binding = new Binding(nameof(AggregatedSkillResult.Amount)),
                        SortDirection = ListSortDirection.Descending
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Crit Rate",
                        Binding = new Binding(nameof(AggregatedSkillResult.CritRate))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Highest Crit",
                        Binding = new Binding(nameof(AggregatedSkillResult.HighestCrit))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Lowest Crit",
                        Binding = new Binding(nameof(AggregatedSkillResult.LowestCrit))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Average Crit",
                        Binding = new Binding(nameof(AggregatedSkillResult.AverageCrit))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Average White",
                        Binding = new Binding(nameof(AggregatedSkillResult.AverageWhite))
                    });
                    SkillResultsGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Damage Percent",
                        Binding = new Binding(nameof(AggregatedSkillResult.DamagePercent))
                    });
                    break;
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel = null;
            Close();
        }
    }
}
