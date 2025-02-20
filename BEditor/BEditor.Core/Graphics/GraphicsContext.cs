﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BEditor.Core.Graphics
{
    public unsafe sealed class GraphicsContext : BaseGraphicsContext
    {
        private readonly GameWindow GameWindow;
        private static GraphicsContext current;
        private readonly object lockobject = new object();

        public GraphicsContext(int width, int height) : base(width, height)
        {
            lock (lockobject)
            {
                GameWindow = new GameWindow(width, height);
                //GameWindow = new GameWindow(
                //    GameWindowSettings.Default,
                //    new()
                //    {
                //        Size = new(width, height),
                //        StartVisible = false,
                //        Flags = ContextFlags.Offscreen,
                //        API = ContextAPI.OpenGL
                //    });

                Initialize();
            }
        }

        public override void MakeCurrent()
        {
            if (current?.Equals(this) ?? false) return;

            current = this;

            lock (lockobject)
            {
                try
                {
                    GameWindow.MakeCurrent();
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }

        public override void SwapBuffers()
        {
            lock (lockobject)
            {
            GameWindow.SwapBuffers();
            }
        }

        public override void Resize(int width, int height, bool Perspective = false, float x = 0, float y = 0, float z = 1024, float tx = 0, float ty = 0, float tz = 0, float near = 0.1F, float far = 20000)
        {
            lock (lockobject)
            {
                base.Resize(width, height, Perspective, x, y, z, tx, ty, tz, near, far);
                GameWindow.Size = new(width, height);
            }
        }

        public override void Dispose()
        {
            lock (lockobject)
            {
                base.Dispose();
                GameWindow.Dispose();
            }
        }

        //public static GraphicsContext Default { get; } = new GraphicsContext(1, 1);
    }
}
