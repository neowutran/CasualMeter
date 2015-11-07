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
        public TeraData TeraData;

        private readonly Dictionary<PlayerClass, Image> _images;
        private DamageTracker _damageTracker;
        private EntityTracker _entityRegistry;
        private MessageFactory _messageFactory;
        private PlayerTracker _playerTracker;
        private Server _server;
        private TeraSniffer _teraSniffer;

        //todo: keyboard hook



        public static SettingsHelper Instance => Lazy.Value;

        private SettingsHelper()
        {
            _images = new Dictionary<PlayerClass, Image>();
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
                    _images.Add(playerClass, image);
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
            return _images[@class];
        }

        private void HandleNewConnection(Server server)
        {
            ServerName = $"{server.Name}";
            _server = server;
            TeraData = BasicTeraData.DataForRegion(server.Region);
            _entityRegistry = new EntityTracker();
            _playerTracker = new PlayerTracker(_entityRegistry);
            _damageTracker = new DamageTracker(_entityRegistry, _playerTracker, TeraData.SkillDatabase);
            _messageFactory = new MessageFactory(TeraData.OpCodeNamer);
        }

        private void HandleMessageReceived(Message obj)
        {
            var message = _messageFactory.Create(obj);
            _entityRegistry.Update(message);

            var skillResultMessage = message as EachSkillResultServerMessage;
            if (skillResultMessage != null)
            {
                _damageTracker.Update(skillResultMessage);
            }
        }

    }
}
