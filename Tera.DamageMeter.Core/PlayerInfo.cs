// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Linq;
using Lunyx.Common.UI.Wpf;
using Tera.Game;

namespace Tera.DamageMeter
{
    public class PlayerInfo : INotifyPropertyChanged
    {

        public DamageTracker Tracker { get; private set; }

        public Player Player { get; }

        public string Name { get { return Player.Name; } }
        public PlayerClass Class { get { return Player.Class; } }

        public ThreadSafeObservableCollection<SkillResult> SkillLog { get; private set; }

        public DateTime EncounterStartTime => Tracker.FirstAttack ?? DateTime.Now;

        public SkillStats Received { get; private set; }
        public SkillStats Dealt { get; private set; }

        //used for sorting collection
        public long Damage => Dealt.Damage;
        public long Heal => Dealt.Heal;

        public double DamageFraction { get { return (double)Dealt.Damage / Tracker.TotalDealt.Damage; } }
        public long Dps { get { return Tracker.Dps(Dealt.Damage); } }
        public double CritFraction { get { return (double) Dealt.Crits/Dealt.Hits; } }

        public long PersonalDps
        {
            get
            {
                var firstOrDefault = SkillLog.FirstOrDefault(s => s.Damage > 0);
                var lastOrDefault = SkillLog.LastOrDefault(s => s.Damage > 0);
                if (firstOrDefault != null && lastOrDefault != null)
                    return Tracker.Dps(Dealt.Damage, lastOrDefault.Time - firstOrDefault.Time);
                return Tracker.Dps(Dealt.Damage, TimeSpan.Zero);
            }
        }

        public PlayerInfo(Player user, DamageTracker tracker)
        {
            Tracker = tracker;
            Player = user;
            Received = new SkillStats();
            Dealt = new SkillStats();
            SkillLog = new ThreadSafeObservableCollection<SkillResult>();

            Dealt.PropertyChanged += DealtOnPropertyChanged;
        }

        public void LogSkillUsage(SkillResult result)
        {
            SkillLog.Add(result);
        }

        public void UpdateStats()
        {
            OnPropertyChanged(nameof(DamageFraction));
            OnPropertyChanged(nameof(Dps));
            OnPropertyChanged(nameof(PersonalDps));
        }

        private void DealtOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Dealt.Damage), StringComparison.OrdinalIgnoreCase))
            {
                OnPropertyChanged(nameof(Damage));
                UpdateStats();
            }
            else if (e.PropertyName.Equals(nameof(Dealt.Heal), StringComparison.OrdinalIgnoreCase))
            {
                OnPropertyChanged(nameof(Heal));
            }
            else if (e.PropertyName.Equals(nameof(Dealt.Hits), StringComparison.OrdinalIgnoreCase))
            {
                OnPropertyChanged(nameof(CritFraction));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            var other = obj as PlayerInfo;
            return Player.PlayerId.Equals(other?.Player.PlayerId);
        }

        public override int GetHashCode()
        {
            return Player.PlayerId.GetHashCode();
        }
    }
}
