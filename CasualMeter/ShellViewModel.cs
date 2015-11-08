using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CasualMeter.Common.Conductors;
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
        }

        #region Properties
        public BasicTeraData BasicTeraData => SettingsHelper.Instance.BasicTeraData;

        public Server Server
        {
            get { return GetProperty<Server>(); }
            set { SetProperty(value); }
        }

        public IEnumerable<PlayerInfo> PlayerInfoCollection
        {
            get { return GetProperty<IEnumerable<PlayerInfo>>(); }
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
            set { SetProperty(value, onChanged: e => Duration = _damageTracker.Duration ?? new TimeSpan(0)); }
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
            ExitCommand = new RelayCommand(Exit);

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
            _damageTracker = new DamageTracker();
            _messageFactory = new MessageFactory(_teraData.OpCodeNamer);

            //manually trigger initial update and subscribe to future changes
            UpdateProperties(null, null);
            _damageTracker.OnPlayerInfoEnumerableChanged += UpdateProperties;
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

            PlayerInfoCollection = _damageTracker;
        }

        private void Exit()
        {
            Environment.Exit(0);
        }
    }
}
