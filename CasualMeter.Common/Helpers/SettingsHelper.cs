using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using CasualMeter.Common.Conductors;
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
        
        public static SettingsHelper Instance => Lazy.Value;

        private SettingsHelper()
        {
            _classIcons = new Dictionary<PlayerClass, Image>();
            BasicTeraData = new BasicTeraData();
            LoadClassIcons();
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
        }

        public Image GetImage(PlayerClass @class)
        {
            return _classIcons[@class];
        }
    }
}
