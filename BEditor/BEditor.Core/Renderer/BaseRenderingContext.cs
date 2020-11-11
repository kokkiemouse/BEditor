﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Core.Media;
using BEditor.Core.Data;
using BEditor.Core.Data.ObjectData;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

#if !OldOpenTK
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
#else

#endif
using Color = BEditor.Core.Media.Color;
using BEditor.Core.Data.PropertyData.Default;
using Image = BEditor.Core.Media.Image;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;

namespace BEditor.Core.Renderer {
    public unsafe class BaseRenderingContext : IBaseRenderingContext {
        protected int Color;
        protected int Depth;
        protected int FBO;
        protected int Renderbuffer;
        protected GameWindow GameWindow;


        public BaseRenderingContext(int width, int height) {
            Width = width;
            Height = height;

            GameWindow = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings() {
                Flags = ContextFlags.Offscreen | ContextFlags.Default,
                Size = new Vector2i(width, height)
            });
            GLFW.HideWindow(GameWindow.WindowPtr);
            Initialize();
        }


        public virtual int Width { get; private set; }
        public virtual int Height { get; private set; }
        public float Aspect => ((float)Width) / ((float)Height);
        public bool IsInitialized { get; private set; }


        public virtual void UnMakeCurrent() {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }
        public virtual void MakeCurrent() {
            //GameWindow.MakeCurrent();
            if (IsInitialized) {
                // fbo 有効化
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO);
            }
        }
        public void SwapBuffers() {
            GameWindow.SwapBuffers();
        }
        public virtual void Dispose() {
            if (IsInitialized) {
                GL.DeleteTexture(Color);
                GL.DeleteTexture(Depth);
                GL.Ext.DeleteFramebuffer(FBO);
                GL.Ext.DeleteRenderbuffer(Renderbuffer);
            }
        }
        /// <summary>
        /// カメラを設定
        /// </summary>
        /// <param name="Perspective">遠近法を使うか</param>
        /// <param name="x">CameraX</param>
        /// <param name="y">CameraY</param>
        /// <param name="z">CameraZ</param>
        /// <param name="tx">TargetX</param>
        /// <param name="ty">TargetY</param>
        /// <param name="tz">TargetZ</param>
        /// <param name="near">ZNear</param>
        /// <param name="far">ZFar</param>
        public virtual void Clear(bool Perspective = false, float x = 0, float y = 0, float z = 1024, float tx = 0, float ty = 0, float tz = 0, float near = 0.1F, float far = 20000) {
            MakeCurrent();

            if (Perspective) {
                //視体積の設定
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, near, far);//描画範囲

                GL.LoadMatrix(ref proj);
            }
            else {
                GL.MatrixMode(MatrixMode.Projection);
                //視体積の設定
                Matrix4 proj = Matrix4.CreateOrthographic(Width, Height, near, far);
                GL.LoadMatrix(ref proj);
            }

            GL.MatrixMode(MatrixMode.Modelview);

            //視界の設定
            Matrix4 look = Matrix4.LookAt(new Vector3(x, y, z), new Vector3(tx, ty, tz), Vector3.UnitY);
            GL.LoadMatrix(ref look);


            //法線の自動調節
            GL.Enable(EnableCap.Normalize);
            //アンチエイリアス
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Enable(EnableCap.PointSmooth);

            GL.Hint(HintTarget.FogHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            if (Perspective) {
                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Lighting);
            }
            else {
                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Lighting);
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public virtual void Resize(int width, int height, bool Perspective = false, float x = 0, float y = 0, float z = 1024, float tx = 0, float ty = 0, float tz = 0, float near = 0.1F, float far = 20000) {
            MakeCurrent();

            Width = width;
            Height = height;

            if (IsInitialized) {
                GL.DeleteTexture(Color);
                GL.DeleteTexture(Depth);
                GL.Ext.DeleteFramebuffer(FBO);
                GL.Ext.DeleteRenderbuffer(Renderbuffer);
            }

            #region RenderBuffer FBO
            //https://ryo1miya.hatenablog.com/entry/2013/09/05/001047
            // renderbuffer object の生成
            GL.Ext.GenRenderbuffers(1, out Renderbuffer);
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, Renderbuffer);

            // 第二引数は 色 GL_RGB, GL_RGBA, デプス値 GL_DEPTH_COMPONENT, ステンシル値 GL_STENCIL_INDEX　などを指定できる
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Rgba16, width, height);

            //色
            GL.GenTextures(1, out Color);
            GL.BindTexture(TextureTarget.Texture2D, Color);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //深度
            GL.GenTextures(1, out Depth);
            GL.BindTexture(TextureTarget.Texture2D, Depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Width, Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //FBO
            GL.Ext.GenFramebuffers(1, out FBO);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, Color, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, Depth, 0);


            // renderbufferをframebuffer objectに結びつける
            // 第二引数は GL_COLOR_ATTACHMENTn_EXT, GL_DEPTH_ATTACHMENT_EXT, GL_STENCIL_ATTACHMENT_EXT など
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.RenderbufferExt, Renderbuffer);

            //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);


            //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO);

            #endregion

            //ビューポートの設定
            GL.Viewport(0, 0, width, height);

            if (Perspective) {
                //視体積の設定
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, near, far);//描画範囲

                GL.LoadMatrix(ref proj);
            }
            else {
                GL.MatrixMode(MatrixMode.Projection);
                //視体積の設定
                Matrix4 proj = Matrix4.CreateOrthographic(Width, Height, near, far);
                GL.LoadMatrix(ref proj);
            }

            GL.MatrixMode(MatrixMode.Modelview);

            //視界の設定
            Matrix4 look = Matrix4.LookAt(new Vector3(x, y, z), new Vector3(tx, ty, tz), Vector3.UnitY);
            GL.LoadMatrix(ref look);


            //法線の自動調節
            GL.Enable(EnableCap.Normalize);
            //アンチエイリアス
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Enable(EnableCap.PointSmooth);

            GL.Hint(HintTarget.FogHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            if (Perspective) {
                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Lighting);
            }
            else {
                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Lighting);
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        protected void Initialize() {
            Resize(Width, Height);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);

            IsInitialized = true;
        }

        internal void DrawImage(Image img, ClipData data, int frame) {
            if (img == null) return;
            ImageObject drawObject = (ImageObject)data.Effect[0];

            float alpha = (float)(drawObject.Blend.Alpha.GetValue(frame) / 100);

            float scale = (float)(drawObject.Zoom.Scale.GetValue(frame) / 100);
            float scalex = (float)(drawObject.Zoom.ScaleX.GetValue(frame) / 100) * scale;
            float scaley = (float)(drawObject.Zoom.ScaleY.GetValue(frame) / 100) * scale;
            float scalez = (float)(drawObject.Zoom.ScaleZ.GetValue(frame) / 100) * scale;

            Point3 coordinate = new Point3(x: drawObject.Coordinate.X.GetValue(frame),
                                             y: drawObject.Coordinate.Y.GetValue(frame),
                                             z: drawObject.Coordinate.Z.GetValue(frame));

            Point3 center = new Point3(x: drawObject.Coordinate.CenterX.GetValue(frame),
                                         y: drawObject.Coordinate.CenterY.GetValue(frame),
                                         z: drawObject.Coordinate.CenterZ.GetValue(frame));


            float nx = drawObject.Angle.AngleX.GetValue(frame);
            float ny = drawObject.Angle.AngleY.GetValue(frame);
            float nz = drawObject.Angle.AngleZ.GetValue(frame);

            Color ambient = drawObject.Material.Ambient.GetValue(frame);
            Color diffuse = drawObject.Material.Diffuse.GetValue(frame);
            Color specular = drawObject.Material.Specular.GetValue(frame);
            float shininess = drawObject.Material.Shininess.GetValue(frame);
            Color color = drawObject.Blend.Color.GetValue(frame);
            color.ScA *= alpha;

            MakeCurrent();
            Graphics.Paint(coordinate, nx, ny, nz, center, () => Graphics.DrawImage(img, scalex, scaley, scalez, color, ambient, diffuse, specular, shininess), Blend.BlentFunc[drawObject.Blend.BlendType.Index]);
        }

    }
}
