﻿using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data;
using BEditor.Core.Data.Primitive.Effects;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Core.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace TestPlugin2
{
    [DataContract]
    public class TestEffect : ImageEffect
    {
        public static readonly CheckPropertyMetadata CheckMetadata = new("チェックボックス");

        public TestEffect()
        {
            Check = new(CheckMetadata);
        }

        public override string Name => nameof(TestEffect);
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Check
        };
        [DataMember]
        public CheckProperty Check { get; private set; }

        public override void Render(EffectRenderArgs<Image<BGRA32>> args)
        {
            if (Check.Value)
                args.Value.Blur(50);
        }
        public override void Loaded()
        {
            base.Loaded();
            Check.ExecuteLoaded(CheckMetadata);
        }
        public override void Unloaded()
        {
            base.Unloaded();
            Check.Unloaded();
        }
    }
}
