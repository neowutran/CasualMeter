using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using Tera;
using Tera.DamageMeter;
using Tera.Data;
using Tera.Game;
using Tera.Game.Messages;
using Tera.Sniffing;

namespace CasualMeter.Common.Helpers
{
    public sealed class SettingsHelper
    {
        private static readonly Lazy<SettingsHelper> Lazy = new Lazy<SettingsHelper>(() => new SettingsHelper());
        
        public readonly BasicTeraData BasicTeraData;

        private readonly Dictionary<PlayerClass, Image> _classIcons;

        private TeraSniffer _teraSniffer;
        private static readonly BasicTeraData _basicTeraData;
        private static TeraData _teraData;
        private MessageFactory _messageFactory;
        private EntityTracker _entityTracker;
        private DamageTracker _damageTracker;
        private Server _server;
        private PlayerTracker _playerTracker;

        //todo: keyboard hook



        public static SettingsHelper Instance => Lazy.Value;

        private SettingsHelper()
        {
            _classIcons = new Dictionary<PlayerClass, Image>();
            BasicTeraData = new BasicTeraData();
            LoadClassIcons();

            _teraSniffer = new TeraSniffer(BasicTeraData.Servers);
            _teraSniffer.MessageReceived += HandleMessageReceived;
            _teraSniffer.NewConnection += HandleNewConnection;
        }

        private void LoadClassIcons()
        {
            var directory = Path.Combine(BasicTeraData.ResourceDirectory, @"class-icons");
            foreach (var playerClass in (PlayerClass[]) Enum.GetValues(typeof (PlayerClass)))
            {
                var filename = Path.Combine(directory, playerClass.ToString().ToLowerInvariant() + ".png");
                using (var image = Image.FromFile(filename))
                {
                    _classIcons.Add(playerClass, image);
                }
            }
        }

        public void Initialize()
        {
            _teraSniffer.Enabled = true;
        }

        public string ServerName { get; set; }

        public Image GetImage(PlayerClass @class)
        {
            return _classIcons[@class];
        }

        private void HandleNewConnection(Server server)
        {
            ServerName = $"{server.Name}";
            _server = server;
            _teraData = BasicTeraData.DataForRegion(server.Region);
            _entityTracker = new EntityTracker();
            _playerTracker = new PlayerTracker(_entityTracker);
            _damageTracker = new DamageTracker();
            _messageFactory = new MessageFactory(_teraData.OpCodeNamer);
        }

        private void HandleMessageReceived(Message obj)
        {
            var message = _messageFactory.Create(obj);
            _entityTracker.Update(message);

            var skillResultMessage = message as EachSkillResultServerMessage;
            if (skillResultMessage != null)
            {
                var skillResult = new SkillResult(skillResultMessage, _entityTracker, _playerTracker, _teraData.SkillDatabase);
                _damageTracker.Update(skillResult);
            }
        }

    }
}
