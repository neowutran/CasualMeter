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
        private DamageTracker _damageTracker;
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
        public BasicTeraData BasicTeraData => SettingsHelper.Instance.BasicTeraData;

        public Server Server
        {
            get { return GetProperty<Server>(); }
            set { SetProperty(value); }
        }

        public ThreadSafeObservableCollection<PlayerInfo> PlayerStats
        {
            get { return GetProperty<ThreadSafeObservableCollection<PlayerInfo>>(); }
            set { SetProperty(value); }
        }

        public DateTime? FirstAttack
        {
            get { return GetProperty<DateTime?>(); }
            set { SetProperty(value); }
        }

        public DateTime? LastAttack
        {
            get { return GetProperty<DateTime?>(); }
            set { SetProperty(value, onChanged: e => Duration = _damageTracker.Duration ?? TimeSpan.Zero); }
        }

        public TimeSpan Duration
        {
            get { return GetProperty<TimeSpan>(getDefault: () => new TimeSpan(0)); }
            set { SetProperty(value); }
        }

        public long TotalDealt
        {
            get { return GetProperty<long>(getDefault: () => 0); }
            set { SetProperty(value); }
        }
        #endregion

        #region Commands
        public RelayCommand ExitCommand
        {
            get { return GetProperty<RelayCommand>(); }
            set { SetProperty(value); }
        }
        #endregion

        public void Initialize()
        {
            //initalize commands
            ExitCommand = new RelayCommand(PrepareExit);

            //start sniffing
            _teraSniffer.Enabled = true;
        }

        private void HandleNewConnection(Server server)
        {
            if (_damageTracker != null)
                _damageTracker.OnPlayerInfoEnumerableChanged -= UpdateProperties;

            Server = server;
            _teraData = BasicTeraData.DataForRegion(server.Region);

            _entityTracker = new EntityTracker();
            _playerTracker = new PlayerTracker(_entityTracker);
            _messageFactory = new MessageFactory(_teraData.OpCodeNamer);

            //manually trigger initial update and subscribe to future changes
            Reset(null);
            _damageTracker = _damageTracker ?? new DamageTracker();
            _damageTracker.OnPlayerInfoEnumerableChanged += UpdateProperties;
        }

        private void Reset(ResetPlayerStatsMessage message)
        {
            if (Server == null) return;

            if (message != null && message.ShouldSaveCurrent)
            {
                //todo: save current encounter
            }

            _damageTracker = new DamageTracker();

            //update properties
            PlayerStats = _damageTracker.StatsByUser;
            UpdateProperties(null, null);
        }

        private void HandleMessageReceived(Message obj)
        {
            var message = _messageFactory.Create(obj);
            _entityTracker.Update(message);

            var skillResultMessage = message as EachSkillResultServerMessage;
            if (skillResultMessage != null && !skillResultMessage.IsUseless)
            {
                var skillResult = new SkillResult(skillResultMessage, _entityTracker, _playerTracker, _teraData.SkillDatabase);
                _damageTracker.Update(skillResult);
            }
        }

        private void UpdateProperties(object source, EventArgs e)
        {
            FirstAttack = _damageTracker.FirstAttack;
            LastAttack = _damageTracker.LastAttack;
            TotalDealt = _damageTracker.TotalDealt.Damage;
        }

        private void PasteStats(PastePlayerStatsMessage obj)
        {
            if (_damageTracker == null) return;

            var playerStatsSequence = _damageTracker.StatsByUser.OrderByDescending(playerStats => playerStats.Dealt.Damage).TakeWhile(x => x.Dealt.Damage > 0);
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

            var text = sb.ToString();
            ProcessHelper.Instance.SendString(text);
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
