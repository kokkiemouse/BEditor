﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

using BEditor.Core.Command;
using BEditor.Core.Data;
using BEditor.Core.Data.Property;
using BEditor.Media;

namespace BEditor.Core.Command
{
    public static class ExtensionCommand
    {
        public static void Execute(this IRecordCommand command) => CommandManager.Do(command);

        public static IRecordCommand CreateCheckCommand(this EffectElement self, bool value) => new EffectElement.CheckCommand(self, value);
        public static IRecordCommand CreateUpCommand(this EffectElement self) => new EffectElement.UpCommand(self);
        public static IRecordCommand CreateDownCommand(this EffectElement self) => new EffectElement.DownCommand(self);


        public static IRecordCommand CreateRemoveCommand(this ClipData self, EffectElement effect) => new EffectElement.RemoveCommand(effect);
        public static IRecordCommand CreateAddCommand(this ClipData self, EffectElement effect) => new EffectElement.AddCommand(effect, self);
        public static IRecordCommand CreateMoveCommand(this ClipData self, Frame to, int tolayer) => new ClipData.MoveCommand(self, to, tolayer);
        public static IRecordCommand CreateMoveCommand(this ClipData self, Frame to, Frame from, int tolayer, int fromlayer) => new ClipData.MoveCommand(self, to, from, tolayer, fromlayer);
        public static IRecordCommand CreateLengthChangeCommand(this ClipData elf, Frame start, Frame end) => new ClipData.LengthChangeCommand(elf, start, end);

        public static IRecordCommand CreateAddCommand(this Scene self, Frame addframe, int layer, ObjectMetadata metadata, out ClipData generatedClip)
        {
            _ = self ?? throw new ArgumentNullException(nameof(self));
            _ = (Frame.Zero > addframe) ? throw new ArgumentOutOfRangeException(nameof(addframe)) : addframe;
            _ = (0 > layer) ? throw new ArgumentOutOfRangeException(nameof(layer)) : layer;
            _ = metadata ?? throw new ArgumentNullException(nameof(metadata));

            //新しいidを取得
            int idmax = self.NewId;

            //描画情報
            var list = new ObservableCollection<EffectElement>
                {
                    (EffectElement)(metadata.CreateFunc?.Invoke() ?? Activator.CreateInstance(metadata.Type))
                };

            //オブジェクトの情報
            generatedClip = new ClipData(idmax, list, addframe, addframe + 180, metadata.Type, layer, self);

            return RecordCommand.Create(
                generatedClip,
                clip =>
                {
                    var scene = clip.Parent;
                    clip.Loaded();
                    scene.Add(clip);
                    scene.SetCurrentClip(clip);
                },
                clip =>
                {
                    var scene = clip.Parent;
                    scene.Remove(clip);
                    clip.Unloaded();

                    //存在する場合
                    if (scene.SelectNames.Exists(x => x == clip.Name))
                    {
                        scene.SelectItems.Remove(clip);

                        if (scene.SelectName == clip.Name)
                        {
                            scene.SelectItem = null;
                        }
                    }
                });
        }
        static readonly PropertyInfo ClipDataID = typeof(ClipData).GetProperty(nameof(ClipData.Id));
        public static IRecordCommand CreateAddCommand(this Scene self, ClipData clip)
        {
            //オブジェクトの情報
            clip.Parent = self;
            ClipDataID.SetValue(clip, self.NewId);

            return RecordCommand.Create(
                clip,
                clip =>
                {
                    var scene = clip.Parent;
                    clip.Loaded();
                    scene.Add(clip);
                    scene.SetCurrentClip(clip);
                },
                clip =>
                {
                    var scene = clip.Parent;
                    scene.Remove(clip);
                    clip.Unloaded();

                    //存在する場合
                    if (scene.SelectNames.Exists(x => x == clip.Name))
                    {
                        scene.SelectItems.Remove(clip);

                        if (scene.SelectName == clip.Name)
                        {
                            scene.SelectItem = null;
                        }
                    }
                });
        }
        public static IRecordCommand CreateRemoveCommand(this Scene self, ClipData clip) => new ClipData.RemoveCommand(clip);
        public static IRecordCommand CreateRemoveLayerCommand(this Scene self, int layer) => new Scene.RemoveLayer(self, layer);

        public static void ExecuteLoaded(this PropertyElement property, PropertyElementMetadata metadata)
        {
            property.Loaded();
            property.PropertyMetadata = metadata;
        }
        public static void ExecuteLoaded<T>(this PropertyElement<T> property, T metadata) where T : PropertyElementMetadata
        {
            property.Loaded();
            property.PropertyMetadata = metadata;
        }
    }
}
