﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor.Core.Command;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Views.PropertyControls;

using Microsoft.WindowsAPICodePack.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace BEditor.ViewModels.PropertyControl
{
    public class FolderPropertyViewModel
    {
        public FolderPropertyViewModel(FolderProperty property)
        {
            Property = property;
            Metadata = property.ObserveProperty(p => p.PropertyMetadata)
                .ToReadOnlyReactiveProperty();

            Command.Subscribe(x =>
            {
                var file = OpenDialog();

                if (file != null)
                {
                    CommandManager.Do(new FolderProperty.ChangeFolderCommand(Property, file));
                }
            });
            Reset.Subscribe(() => CommandManager.Do(new FolderProperty.ChangeFolderCommand(Property, Property.PropertyMetadata.Default)));
            Bind.Subscribe(() =>
            {
                var window = new BindSettings()
                {
                    DataContext = new BindSettingsViewModel<string>(Property)
                };
                window.ShowDialog();
            });
        }

        public ReadOnlyReactiveProperty<FolderPropertyMetadata> Metadata { get; }
        public FolderProperty Property { get; }
        public ReactiveCommand Command { get; } = new();
        public ReactiveCommand Reset { get; } = new();
        public ReactiveCommand Bind { get; } = new();

        private static string OpenDialog()
        {
            // ダイアログのインスタンスを生成
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };


            // ダイアログを表示する
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
