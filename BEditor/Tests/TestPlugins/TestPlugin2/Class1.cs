﻿using System;
using System.Collections.Generic;

using BEditor.Core.Data;
using BEditor.Core.Plugin;

namespace TestPlugin2
{
    public partial class TestPlugin2 : ICustomMenuPlugin, IEffects
    {
        public string PluginName => nameof(TestPlugin2);
        public string Description => nameof(TestPlugin2);
        public IEnumerable<EffectMetadata> Effects => new EffectMetadata[]
        {
            new()
            {
                Type = typeof(TestEffect),
                CreateFunc = () => new TestEffect(),
                Name = nameof(TestEffect)
            }
        };

        public SettingRecord Settings { get; set; } = new Setting(0, 0.1f, "文字");
    }
}
