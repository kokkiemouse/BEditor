﻿using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Property;
using BEditor.Core.Extensions;
using BEditor.Core.Properties;

namespace BEditor.Core.Data.Primitive.Properties.PrimitiveGroup
{
    [DataContract]
    public sealed class Angle : ExpandGroup
    {
        public static readonly EasePropertyMetadata AngleXMetadata = new(Resources.AngleX);
        public static readonly EasePropertyMetadata AngleYMetadata = new(Resources.AngleY);
        public static readonly EasePropertyMetadata AngleZMetadata = new(Resources.AngleZ);

        public Angle(PropertyElementMetadata metadata) : base(metadata)
        {
            AngleX = new(AngleXMetadata);
            AngleY = new(AngleYMetadata);
            AngleZ = new(AngleZMetadata);
        }

        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            AngleX,
            AngleY,
            AngleZ
        };
        [DataMember(Order = 0)]
        public EaseProperty AngleX { get; private set; }
        [DataMember(Order = 1)]
        public EaseProperty AngleY { get; private set; }
        [DataMember(Order = 2)]
        public EaseProperty AngleZ { get; private set; }

        public override void Loaded()
        {
            base.Loaded();
            AngleX.ExecuteLoaded(AngleXMetadata);
            AngleY.ExecuteLoaded(AngleYMetadata);
            AngleZ.ExecuteLoaded(AngleZMetadata);
        }
        public override void Unloaded()
        {
            base.Unloaded();
            AngleX.Unloaded();
            AngleY.Unloaded();
            AngleZ.Unloaded();
        }
    }
}
