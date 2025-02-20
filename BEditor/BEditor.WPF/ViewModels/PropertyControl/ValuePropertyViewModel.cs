﻿using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

using BEditor.Core.Command;
using BEditor.Core.Data.Control;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Models;
using BEditor.Models.Extension;
using BEditor.Views.PropertyControls;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using CommandManager = BEditor.Core.Command.CommandManager;

namespace BEditor.ViewModels.PropertyControl
{
    public class ValuePropertyViewModel
    {
        private float oldvalue;

        public ValuePropertyViewModel(ValueProperty property)
        {
            Property = property;
            Metadata = property.ObserveProperty(p => p.PropertyMetadata)
                .ToReadOnlyReactiveProperty();

            Reset.Subscribe(() => CommandManager.Do(new ValueProperty.ChangeValueCommand(Property, Property.PropertyMetadata.DefaultValue)));
            Bind.Subscribe(() =>
            {
                var window = new BindSettings()
                {
                    DataContext = new BindSettingsViewModel<float>(Property)
                };
                window.ShowDialog();
            });
            GotFocus.Subscribe(_ => oldvalue = Property.Value);
            LostFocus.Subscribe(e =>
            {
                if (float.TryParse(e, out float _out))
                {
                    Property.Value = oldvalue;

                    CommandManager.Do(new ValueProperty.ChangeValueCommand(Property, _out));
                }
            });
            PreviewMouseWheel.Subscribe(e =>
            {
                if (e.text.IsKeyboardFocused && float.TryParse(e.text.Text, out var val))
                {
                    int v = 10;//定数増え幅

                    if (Keyboard.IsKeyDown(Key.LeftShift)) v = 1;

                    val += e.e.Delta / 120 * v;

                    Property.Value = Property.InRange(val);

                    AppData.Current.Project.PreviewUpdate(Property.GetParent2());

                    e.e.Handled = true;

                    Debug.WriteLine(e.text.Text);
                }
            });
            TextChanged.Subscribe(text =>
            {
                if (float.TryParse(text, out var val))
                {
                    Property.Value = Property.InRange(val);

                    AppData.Current.Project.PreviewUpdate(Property.GetParent2());

                    Debug.WriteLine(text);
                }
            });
        }

        public ReadOnlyReactiveProperty<ValuePropertyMetadata> Metadata { get; }
        public ValueProperty Property { get; }
        public ReactiveCommand Reset { get; } = new();
        public ReactiveCommand Bind { get; } = new();
        public ReactiveCommand<string> GotFocus { get; } = new();
        public ReactiveCommand<string> LostFocus { get; } = new();
        public ReactiveCommand<string> TextChanged { get; } = new();
        public ReactiveCommand<(TextBox text, MouseWheelEventArgs e)> PreviewMouseWheel { get; } = new();
    }
}
