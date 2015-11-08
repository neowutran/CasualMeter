// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lunyx.Common.UI.Wpf;
using Tera.Game;
using Tera.Game.Messages;

namespace Tera.DamageMeter
{
    public class DamageTracker
    {
        public delegate void PlayerInfoEnumerableChanged(object source, EventArgs e);
        public event PlayerInfoEnumerableChanged OnPlayerInfoEnumerableChanged;

        public ThreadSafeObservableCollection<PlayerInfo> StatsByUser { get; set; }
        
        public DateTime? FirstAttack { get; private set; }
        public DateTime? LastAttack { get; private set; }
        public TimeSpan? Duration => LastAttack - FirstAttack;

        public SkillStats TotalDealt { get; private set; }
        public SkillStats TotalReceived { get; private set; }

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
                LastAttack = skillResult.Time;

                if (FirstAttack == null)
                    FirstAttack = skillResult.Time;
            }

            foreach (var playerStat in StatsByUser)
            {   //force update of calculated dps metrics
                playerStat.UpdateStats();
            }
            OnPlayerInfoEnumerableChanged?.Invoke(this, new EventArgs());
        }

        private SkillStats StatsChange(SkillResult message)
        {
            var result = new SkillStats();
            if (message.Amount == 0)
                return result;

            result.Damage = message.Damage;
            result.Heal = message.Heal;

            if (!message.IsHeal)
            {
                result.Hits++;
                if (message.IsCritical)
                    result.Crits++;
            }

            return result;
        }

        public long Dps(long damage)
        {
            var durationInSeconds = (Duration ?? TimeSpan.Zero).TotalSeconds;
            if (durationInSeconds < 1)
                durationInSeconds = 1;
            var dps = damage / durationInSeconds;
            return (long)dps;
        }
    }
}
