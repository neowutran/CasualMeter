using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Conductors.Messages;
using CasualMeter.Common.Formatters;
using CasualMeter.Common.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using Lunyx.Common.UI.Wpf;
using Tera;
using Tera.DamageMeter;
using Tera.Data;
using Tera.Game;
using Tera.Game.Messages;
using Tera.Sniffing;

namespace CasualMeter
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly TeraSniffer _teraSniffer;
        private TeraData _teraData;
        private MessageFactory _messageFactory;
        private EntityTracker _entityTracker;
        private PlayerTracker _playerTracker;

        public ShellViewModel()
        {
            _teraSniffer = new TeraSniffer(BasicTeraData.Servers);
            _teraSniffer.MessageReceived += HandleMessageReceived;
            _teraSniffer.NewConnection += HandleNewConnection;

            CasualMessenger.Instance.Messenger.Register<PastePlayerStatsMessage>(this, PasteStats);
            CasualMessenger.Instance.Messenger.Register<ResetPlayerStatsMessage>(this, Reset);
            CasualMessenger.Instance.Messenger.Register<ExitMessage>(this, Exit);
        }

        #region Properties

        public BasicTeraData BasicTeraData
        {
            get { return GetProperty(getDefault: () => SettingsHelper.Instance.BasicTeraData); }
            set { SetProperty(value); }
        }

        public Server Server
        {
            get { return GetProperty<Server>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<DamageTracker> ArchivedDamageTrackers
        {
            get { return GetProperty(getDefault: () => new ThreadSafeObservableCollection<DamageTracker>()); }
            set { SetProperty(value); }
        }

        public DamageTracker DamageTracker
        {
            get { return GetProperty<DamageTracker>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<PlayerInfo> PlayerStats
        {
            get { return GetProperty<ThreadSafeObservableCollection<PlayerInfo>>(); }
            set { SetProperty(value); }
        }
        #endregion

        #region Commands
        public RelayCommand ExitCommand
        {
            get { return GetProperty<RelayCommand>(getDefault: () => new RelayCommand(PrepareExit)); }
            set { SetProperty(value); }
        }
        #endregion

        public void Initialize()
        {
            //start sniffing
            _teraSniffer.Enabled = true;
        }

        private void HandleNewConnection(Server server)
        {
            Server = server;
            _teraData = BasicTeraData.DataForRegion(server.Region);

            _entityTracker = new EntityTracker();
            _playerTracker = new PlayerTracker(_entityTracker);
            _messageFactory = new MessageFactory(_teraData.OpCodeNamer);

            Reset(null);
            DamageTracker = DamageTracker ?? new DamageTracker();
        }

        private void Reset(ResetPlayerStatsMessage message)
        {
            if (Server == null) return;

            if (message != null && message.ShouldSaveCurrent && !DamageTracker.IsArchived && DamageTracker.StatsByUser.Count > 0)
            {
                DamageTracker.IsArchived = true;
                ArchivedDamageTrackers.Add(DamageTracker);
            }

            DamageTracker = new DamageTracker();

            //update properties
            PlayerStats = DamageTracker.StatsByUser;
        }

        private void HandleMessageReceived(Message obj)
        {
            var message = _messageFactory.Create(obj);
            _entityTracker.Update(message);

            var skillResultMessage = message as EachSkillResultServerMessage;
            if (skillResultMessage != null && !skillResultMessage.IsUseless &&//stuff like warrior DFA
                (DamageTracker.FirstAttack != null || !skillResultMessage.IsHeal) &&//only record first hit is it's a damage hit (heals occurring outside of fights)
                !(skillResultMessage.Target.Equals(skillResultMessage.Source) && !skillResultMessage.IsHeal))//disregard damage dealt to self (gunner self destruct)
            {   
                var skillResult = new SkillResult(skillResultMessage, _entityTracker, _playerTracker, _teraData.SkillDatabase);
                DamageTracker.Update(skillResult);
            }
        }
        
        private void PasteStats(PastePlayerStatsMessage obj)
        {
            if (DamageTracker == null) return;

            var playerStatsSequence = DamageTracker.StatsByUser.OrderByDescending(playerStats => playerStats.Dealt.Damage).TakeWhile(x => x.Dealt.Damage > 0);
            const int maxLength = 300;

            var sb = new StringBuilder();
            bool first = true;

            foreach (var playerInfo in playerStatsSequence)
            {
                var placeHolder = new PlayerStatsFormatter(playerInfo, FormatHelpers.Invariant);
                var playerText = first ? "" : " | ";

                playerText += placeHolder.Replace("{Name} {DPS} {DamagePercent}");

                if (sb.Length + playerText.Length > maxLength)
                    break;

                sb.Append(playerText);
                first = false;
            }
            
            if (sb.Length > 0)
            {
                var text = sb.ToString();
                //copy to clipboard in case user wants to paste outside of Tera
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(text));
                if (ProcessHelper.Instance.IsTeraActive)
                    //send text input to Tera
                    ProcessHelper.Instance.SendString(text);
            }
        }

        private void PrepareExit()
        {
            CasualMessenger.Instance.Messenger.Send(new PrepareExitMessage());
        }

        private void Exit(ExitMessage message)
        {
            Environment.Exit(0);
        }
    }
}
