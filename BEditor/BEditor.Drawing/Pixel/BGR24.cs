﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BEditor.Drawing.Pixel
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [PixelFormat(3)]
    public struct BGR24 : IPixel<BGR24>, IPixelConvertable<BGRA32>, IPixelConvertable<RGBA32>, IPixelConvertable<RGB24>
    {
        public byte B;
        public byte G;
        public byte R;

        public BGR24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public readonly BGR24 Blend(BGR24 foreground) => foreground;
        public void ConvertFrom(BGRA32 src)
        {
            B = src.B;
            G = src.G;
            R = src.R;
        }
        public void ConvertFrom(RGBA32 src)
        {
            B = src.B;
            G = src.G;
            R = src.R;
        }
        public void ConvertFrom(RGB24 src)
        {
            B = src.B;
            G = src.G;
            R = src.R;
        }
        public readonly void ConvertTo(out BGRA32 dst)
        {
            dst = new(R, G, B, 255);
        }
        public readonly void ConvertTo(out RGBA32 dst)
        {
            dst = new(R, G, B, 255);
        }
        public readonly void ConvertTo(out RGB24 dst)
        {
            dst = new(R, G, B);
        }
    }
}
