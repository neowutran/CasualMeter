using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Entities;
using Newtonsoft.Json;
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
        private readonly Dictionary<PlayerClass, string> _classIcons;
        
        public static SettingsHelper Instance => Lazy.Value;

        private static readonly string ConfigPath = Path.Combine(Path.GetDirectoryName(typeof(SettingsHelper).Assembly.Location) ?? string.Empty, "config");
        private static readonly string ConfigFilePath = Path.Combine(ConfigPath, "settings.json");

        public Settings Settings { get; set; }

        private SettingsHelper()
        {
            _classIcons = new Dictionary<PlayerClass, string>();
            BasicTeraData = new BasicTeraData();
            LoadClassIcons();
            Load();
        }

        public void Initialize()
        {
            //empty method to ensure initialization
        }

        private void LoadClassIcons()
        {
            var directory = Path.Combine(BasicTeraData.ResourceDirectory, @"class-icons");
            foreach (var playerClass in (PlayerClass[]) Enum.GetValues(typeof (PlayerClass)))
            {
                var filename = Path.Combine(directory, playerClass.ToString().ToLowerInvariant() + ".png");
                _classIcons.Add(playerClass, filename);
            }
        }

        private void Load()
        {
            if (File.Exists(ConfigFilePath))
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(ConfigFilePath));
            else
            {
                Settings = new Settings();
                Save();
            }

            //todo: initialize missing settings that weren't loaded
        }

        public void Save()
        {
            Directory.CreateDirectory(ConfigPath);
            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        public string GetImage(PlayerClass @class)
        {
            return _classIcons[@class];
        }
    }
}
