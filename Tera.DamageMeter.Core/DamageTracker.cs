// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lunyx.Common.UI.Wpf;
using Nicenis.ComponentModel;
using Tera.Game;
using Tera.Game.Messages;

namespace Tera.DamageMeter
{
    public class DamageTracker : PropertyObservable
    {
        public ThreadSafeObservableCollection<PlayerInfo> StatsByUser
        {
            get { return GetProperty<ThreadSafeObservableCollection<PlayerInfo>>(); }
            set { SetProperty(value); }
        }

        public bool IsArchived
        {
            get { return GetProperty<bool>(getDefault: () => false); }
            set { SetProperty(value); }
        }

        public DateTime? FirstAttack
        {
            get { return GetProperty<DateTime?>(); }
            set { SetProperty(value, 
                onChanged: e =>
                {
                    if (LastAttack != null && e.NewValue != null) Duration = (DateTime) LastAttack - (DateTime) e.NewValue;
                });
            }
        }

        public DateTime? LastAttack
        {
            get { return GetProperty<DateTime?>(); }
            set
            {
                SetProperty(value,
                    onChanged: e =>
                    {
                        if (e.NewValue != null && FirstAttack != null) Duration = (DateTime)e.NewValue - (DateTime)FirstAttack;
                    });
            }
        }

        public TimeSpan Duration
        {
            get { return GetProperty<TimeSpan>(getDefault: () => TimeSpan.Zero); }
            set { SetProperty(value); }
        }

        public SkillStats TotalDealt
        {
            get { return GetProperty<SkillStats>(); }
            set { SetProperty(value); }
        }

        public SkillStats TotalReceived
        {
            get { return GetProperty<SkillStats>(); }
            set { SetProperty(value); }
        }
        
        public DamageTracker()
        {
            StatsByUser = new ThreadSafeObservableCollection<PlayerInfo>();
            TotalDealt = new SkillStats();
            TotalReceived = new SkillStats();
        }

        private PlayerInfo GetOrCreate(Player player)
        {
            PlayerInfo playerStats = StatsByUser.FirstOrDefault(pi => pi.Player.Equals(player));
            if (playerStats == null)
            {
                playerStats = new PlayerInfo(player, this);
                StatsByUser.Add(playerStats);
            }
            return playerStats;
        }

        public void Update(SkillResult skillResult)
        {
            if (IsArchived) return;//prevent archived trackers from accidentally recording stats

            if (skillResult.SourcePlayer != null)
            {
                var playerStats = GetOrCreate(skillResult.SourcePlayer);
                var statsChange = StatsChange(skillResult);
                playerStats.Dealt.Add(statsChange);
                playerStats.LogSkillUsage(skillResult);
                TotalDealt.Add(statsChange);
            }

            //don't care about damage received since it's useless
            //if (skillResult.TargetPlayer != null)
            //{
            //    var playerStats = GetOrCreate(skillResult.TargetPlayer);
            //    var statsChange = StatsChange(skillResult);
            //    playerStats.Received.Add(statsChange);
            //    TotalReceived.Add(statsChange);
            //}

            if (skillResult.SourcePlayer != null && (skillResult.Damage > 0) && (skillResult.Source.Id != skillResult.Target.Id))
            {
                if (FirstAttack == null)
                    FirstAttack = skillResult.Time;

                LastAttack = skillResult.Time;
            }

            foreach (var playerStat in StatsByUser)
            {   //force update of calculated dps metrics
                playerStat.UpdateStats();
            }
        }

        private SkillStats StatsChange(SkillResult message)
        {
            var result = new SkillStats();
            if (message.Amount == 0)
                return result;

            result.Damage = message.Damage;
            result.Heal = message.Heal;

            result.Hits++;
            if (message.IsCritical)
                result.Crits++;
            
            return result;
        }

        public long Dps(long damage)
        {
            return Dps(damage, Duration);
        }

        public long Dps(long damage, TimeSpan duration)
        {
            var durationInSeconds = duration.TotalSeconds;
            if (durationInSeconds < 1)
                durationInSeconds = 1;
            var dps = damage / durationInSeconds;
            return (long)dps;
        }
    }
}
