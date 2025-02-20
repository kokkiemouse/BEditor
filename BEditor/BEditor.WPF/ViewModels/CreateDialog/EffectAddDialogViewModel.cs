﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Core.Command;
using BEditor.Core.Data;
using BEditor.Models;

using Reactive.Bindings;

namespace BEditor.ViewModels.CreateDialog
{
    public class EffectAddDialogViewModel
    {
        public EffectAddDialogViewModel()
        {
            AddCommand.Subscribe(() =>
            {
                var effectinstance = Type.Value.CreateFunc?.Invoke() ?? Activator.CreateInstance(Type.Value.Type) as EffectElement;

                TargetClip.Value.CreateAddCommand(effectinstance).Execute();
            });
        }

        public ReactiveProperty<Scene> Scene { get; } = new(AppData.Current.Project.SceneList[0]);
        public ReactiveProperty<ClipData> TargetClip { get; } = new();
        public ReactiveProperty<EffectMetadata> Type { get; } = new();
        public ReactiveCommand AddCommand { get; } = new();
    }
}
