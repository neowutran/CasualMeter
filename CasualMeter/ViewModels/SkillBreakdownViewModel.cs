using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Conductors.Messages;
using Lunyx.Common.UI.Wpf;
using Nicenis.ComponentModel;
using Tera.DamageMeter;

namespace CasualMeter.ViewModels
{
    public class SkillBreakdownViewModel : ViewModelBase
    {
        public ThreadSafeObservableCollection<ComboBoxEntity> ComboBoxEntities
        {
            get { return GetProperty<ThreadSafeObservableCollection<ComboBoxEntity>>(); }
            set { SetProperty(value); }
        }

        public ComboBoxEntity SelectedCollectionView
        {
            get { return GetProperty<ComboBoxEntity>(); }
            set { SetProperty(value, onChanged: OnSelectedViewChanged); }
        }

        private void OnSelectedViewChanged(IPropertyValueChangedEventArgs<ComboBoxEntity> e)
        {
            if (e?.NewValue?.Key == null) return;
            CasualMessenger.Instance.UpdateSkillBreakdownView(this, e.NewValue.Key.ToString());
        }
        
        public PlayerInfo PlayerInfo
        {
            get { return GetProperty<PlayerInfo>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<SkillResult> SkillLog
        {
            get { return GetProperty<ThreadSafeObservableCollection<SkillResult>>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<AggregatedSkillResult> AggregatedSkillLogById
        {
            get { return GetProperty<ThreadSafeObservableCollection<AggregatedSkillResult>>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<AggregatedSkillResult> AggregatedSkillLogByName
        {
            get { return GetProperty<ThreadSafeObservableCollection<AggregatedSkillResult>>(); }
            set { SetProperty(value); }
        }
        
        public SkillBreakdownViewModel(PlayerInfo playerInfo)
        {
            ComboBoxEntities = new ThreadSafeObservableCollection<ComboBoxEntity>
            {
                new ComboBoxEntity(SkillViewType.FlatView, "Flat View"),
                new ComboBoxEntity(SkillViewType.AggregatedSkillIdView, "Aggregate by Id"),
                new ComboBoxEntity(SkillViewType.AggregatedSkillNameView, "Aggregate by Name")
            };

            SelectedCollectionView = ComboBoxEntities.First(cbe => cbe.Key == SkillViewType.FlatView);

            PlayerInfo = playerInfo;
            SkillLog = PlayerInfo.SkillLog;

            //subscribe to future changes and invoke manually
            SkillLog.CollectionChanged += (sender, args) =>
            {
                PopulateAggregatedSkillLogs();
                CasualMessenger.Instance.Messenger.Send(new ScrollPlayerStatsMessage(), this);
            };
            
            PopulateAggregatedSkillLogs();
        }

        private void PopulateAggregatedSkillLogs()
        {
            AggregatedSkillLogById = new ThreadSafeObservableCollection<AggregatedSkillResult>();
            AggregatedSkillLogByName = new ThreadSafeObservableCollection<AggregatedSkillResult>();

            var aggregatedById =(from s in SkillLog
                                 group s by s.SkillId into grps
                                 select new AggregatedSkillResult
                                 {
                                     DisplayName = grps.FirstOrDefault()?.SkillName ?? "Unknown Skill",
                                     Amount = grps.Sum(g => g.Amount),
                                     Hits = grps.Count(),
                                     CritRate = (double)grps.Count(g => g.IsCritical) / grps.Count(),
                                     HighestCrit = grps.Any(g => g.IsCritical) ? grps.Where(g => g.IsCritical).Max(g => g.Amount) : 0,
                                     LowestCrit = grps.Any(g => g.IsCritical) ? grps.Where(g => g.IsCritical).Min(g => g.Amount) : 0,
                                     AverageCrit = grps.Any(g => g.IsCritical) ? Convert.ToInt64(grps.Where(g => g.IsCritical).Average(g => g.Amount)) : 0,
                                     AverageWhite = grps.Any(g => !g.IsCritical) ? Convert.ToInt64(grps.Where(g => !g.IsCritical).Average(g => g.Amount)) : 0,
                                     DamagePercent = (long)grps.Sum(g => g.Amount) / SkillLog.Sum(s => s.Amount)
                                 }).ToList();

            var aggregatedByName =  (from s in SkillLog
                                     group s by s.SkillName into grps
                                     select new AggregatedSkillResult
                                     {
                                         DisplayName = grps.Key,
                                         Amount = grps.Sum(g => g.Amount),
                                         Hits = grps.Count(),
                                         CritRate = (double)grps.Count(g => g.IsCritical) / grps.Count(),
                                         HighestCrit = grps.Any(g => g.IsCritical) ? grps.Where(g => g.IsCritical).Max(g => g.Amount) : 0,
                                         LowestCrit = grps.Any(g => g.IsCritical) ? grps.Where(g => g.IsCritical).Min(g => g.Amount) : 0,
                                         AverageCrit = grps.Any(g => g.IsCritical) ? Convert.ToInt64(grps.Where(g => g.IsCritical).Average(g => g.Amount)) : 0,
                                         AverageWhite = grps.Any(g => !g.IsCritical) ? Convert.ToInt64(grps.Where(g => !g.IsCritical).Average(g => g.Amount)) : 0,
                                         DamagePercent = (long)grps.Sum(g => g.Amount) / SkillLog.Sum(s => s.Amount)
                                     }).ToList();

            FillSkillLog(AggregatedSkillLogById, aggregatedById);
            FillSkillLog(AggregatedSkillLogByName, aggregatedByName);
        }

        private void FillSkillLog(ThreadSafeObservableCollection<AggregatedSkillResult> skillLog,
            IEnumerable<AggregatedSkillResult> results)
        {
            foreach (var result in results)
                skillLog.Add(result);
        }
    }

    public class ComboBoxEntity
    {
        public ComboBoxEntity(SkillViewType key, string value)
        {
            Key = key;
            Value = value;
        }
        public SkillViewType Key { get; set; }
        public string Value { get; set; }
    }

    public enum SkillViewType
    {
        FlatView = 1,
        AggregatedSkillIdView = 2,
        AggregatedSkillNameView = 4
    }
}
