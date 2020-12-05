﻿using System;

using BEditor.Drawing;
using BEditor.Drawing.Pixel;

namespace BEditor.Core.Media.Encoder
{
    public abstract class VideoEncoder : IDisposable
    {
        public VideoEncoder(string fileName, int fps, int width, int height)
        {
            FileName = fileName;
            Fps = fps;
            Width = width;
            Height = height;
        }

        public string FileName { get; }
        public int Fps { get; }
        public int Width { get; set; }
        public int Height { get; set; }

        public abstract void Write(Image<BGRA32> image);

        public abstract void Dispose();
    }
}
