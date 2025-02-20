﻿using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Primitive.Objects;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Core.Data.Property;
using BEditor.Core.Extensions;
using BEditor.Core.Properties;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace BEditor.Core.Data.Primitive.Effects.PrimitiveImages
{
    [DataContract]
    public class Clipping : ImageEffect
    {
        public static readonly EasePropertyMetadata TopMetadata = new(Resources.Top, 0, float.NaN, 0);
        public static readonly EasePropertyMetadata BottomMetadata = new(Resources.Bottom, 0, float.NaN, 0);
        public static readonly EasePropertyMetadata LeftMetadata = new(Resources.Left, 0, float.NaN, 0);
        public static readonly EasePropertyMetadata RightMetadata = new(Resources.Right, 0, float.NaN, 0);
        public static readonly CheckPropertyMetadata AdjustCoordinatesMetadata = new(Resources.Adjust_coordinates);

        public Clipping()
        {
            Top = new(TopMetadata);
            Bottom = new(BottomMetadata);
            Left = new(LeftMetadata);
            Right = new(RightMetadata);
            AdjustCoordinates = new(AdjustCoordinatesMetadata);
        }

        public override string Name => Resources.Clipping;
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Top,
            Bottom,
            Left,
            Right,
            AdjustCoordinates
        };
        [DataMember(Order = 0)]
        public EaseProperty Top { get; private set; }
        [DataMember(Order = 1)]
        public EaseProperty Bottom { get; private set; }
        [DataMember(Order = 2)]
        public EaseProperty Left { get; private set; }
        [DataMember(Order = 3)]
        public EaseProperty Right { get; private set; }
        [DataMember(Order = 4)]
        public CheckProperty AdjustCoordinates { get; private set; }

        public override void Render(EffectRenderArgs<Image<BGRA32>> args)
        {
            var top = (int)Top.GetValue(args.Frame);
            var bottom = (int)Bottom.GetValue(args.Frame);
            var left = (int)Left.GetValue(args.Frame);
            var right = (int)Right.GetValue(args.Frame);
            var img = args.Value;

            if (AdjustCoordinates.IsChecked && Parent.Effect[0] is ImageObject image)
            {
                image.Coordinate.CenterX.Optional += -(right / 2) + (left / 2);
                image.Coordinate.CenterY.Optional += -(top / 2) + (bottom / 2);
            }

            if (img.Width <= left + right || img.Height <= top + bottom)
            {
                img.Dispose();
                args.Value = new(1, 1, default(BGRA32));
                return;
            }

            int width = img.Width - left - right;
            int height = img.Height - top - bottom;
            int x = left;
            int y = top;

            var img1 = img[new Rectangle(x, y, width, height)];
            img.Dispose();

            args.Value = img1;
        }
        public override void Loaded()
        {
            base.Loaded();
            Top.ExecuteLoaded(TopMetadata);
            Bottom.ExecuteLoaded(BottomMetadata);
            Left.ExecuteLoaded(LeftMetadata);
            Right.ExecuteLoaded(RightMetadata);
            AdjustCoordinates.ExecuteLoaded(AdjustCoordinatesMetadata);
        }
        public override void Unloaded()
        {
            base.Unloaded();
            foreach (var pr in Children)
            {
                pr.Unloaded();
            }
        }
    }
}
