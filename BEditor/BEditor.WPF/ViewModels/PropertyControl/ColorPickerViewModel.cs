﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Media;

using BEditor.Core.Command;
using BEditor.Core.Data;
using BEditor.Core.Data.Control;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.ViewModels.CustomControl;

using Reactive.Bindings;

namespace BEditor.ViewModels.PropertyControl
{
    public class ColorPickerViewModel : BasePropertyChanged
    {
        private static readonly PropertyChangedEventArgs brushArgs = new(nameof(Brush));

        public ColorPickerViewModel(ColorProperty property)
        {
            Property = property;
            property.PropertyChanged += (s, e) => RaisePropertyChanged(brushArgs);
            Command.Subscribe(x => CommandManager.Do(new ColorProperty.ChangeColorCommand(Property, new(x.Item1, x.Item2, x.Item3, x.Item4))));
            Reset.Subscribe(() => CommandManager.Do(new ColorProperty.ChangeColorCommand(Property, Property.PropertyMetadata.DefaultColor)));
        }

        public static ObservableCollection<ColorList> ColorList { get; } = new();
        public ColorProperty Property { get; }
        public ReactiveCommand<(byte, byte, byte, byte)> Command { get; } = new();
        public ReactiveCommand Reset { get; } = new();
        public SolidColorBrush Brush => new(Color.FromArgb(Property.Color.A, Property.Color.R, Property.Color.G, Property.Color.B));
    }
}
