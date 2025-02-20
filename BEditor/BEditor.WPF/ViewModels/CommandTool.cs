﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;

using Microsoft.Xaml.Behaviors;

using EventTrigger = Microsoft.Xaml.Behaviors.EventTrigger;
using BEditor.Core.Data;
using Reactive.Bindings;

namespace BEditor.ViewModels
{
    public static class CommandTool
    {
        public static EventTrigger CreateEvent(string eventname, ICommand command)
        {
            EventTrigger trigger = new EventTrigger
            {
                EventName = eventname
            };

            InvokeCommandAction action = new()
            {
                Command = command
            };

            trigger.Actions.Add(action);

            return trigger;
        }

        public static EventTrigger CreateEvent(string eventname, ICommand command, IValueConverter converter, object ConverterParameter)
        {
            EventTrigger trigger = new EventTrigger
            {
                EventName = eventname
            };

            InvokeCommandAction action = new InvokeCommandAction()
            {
                Command = command,
                EventArgsConverter = converter,
                EventArgsConverterParameter = ConverterParameter
            };


            trigger.Actions.Add(action);

            return trigger;
        }

        public static EventTrigger CreateEvent(string eventname, ICommand command, object commandparam)
        {
            EventTrigger trigger = new EventTrigger
            {
                EventName = eventname
            };

            InvokeCommandAction action = new InvokeCommandAction()
            {
                Command = command,
                CommandParameter = commandparam
            };


            trigger.Actions.Add(action);

            return trigger;
        }
    }


    public class EventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (parameter, (EventArgs)value);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public static EventArgsConverter Converter = new EventArgsConverter();
    }

    public class MousePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MouseEventArgs a)
            {
                return a.GetPosition((IInputElement)parameter);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public static MousePositionConverter Converter = new MousePositionConverter();
    }

    public class ClipTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type clipType)
            {
                if (clipType == ClipType.Video)
                {
                    return PackIconKind.Movie;
                }
                else if (clipType == ClipType.Image)
                {
                    return PackIconKind.Image;
                }
                else if (clipType == ClipType.Text)
                {
                    return PackIconKind.TextBox;
                }
                else if (clipType == ClipType.Figure)
                {
                    return PackIconKind.Shape;
                }
                else if (clipType == ClipType.Figure)
                {
                    return PackIconKind.RoundedCorner;
                }
                else if (clipType == ClipType.Camera)
                {
                    return PackIconKind.Videocam;
                }
                else if (clipType == ClipType.GL3DObject)
                {
                    return PackIconKind.Cube;
                }
                else if (clipType == ClipType.Scene)
                {
                    return PackIconKind.MovieOpen;
                }
                else
                {
                    return PackIconKind.None;
                }
            }
            else
            {
                return PackIconKind.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconKind kind)
            {
                return ToClipType(kind);
            }

            return ClipType.Video;
        }

        public static Type ToClipType(PackIconKind kind)
        {
            if (kind == PackIconKind.Movie) return ClipType.Video;
            else if (kind == PackIconKind.Image) return ClipType.Image;
            else if (kind == PackIconKind.TextBox) return ClipType.Text;
            else if (kind == PackIconKind.Shape) return ClipType.Figure;
            else if (kind == PackIconKind.Videocam) return ClipType.Camera;
            else if (kind == PackIconKind.Cube) return ClipType.GL3DObject;
            else if (kind == PackIconKind.MovieOpen) return ClipType.Scene;

            return ClipType.Video;
        }
        public static ObjectMetadata ToClipMetadata(PackIconKind kind)
        {
            if (kind == PackIconKind.Movie) return ClipType.VideoMetadata;
            else if (kind == PackIconKind.Audio) return ClipType.AudioMetadata;
            else if (kind == PackIconKind.Image) return ClipType.ImageMetadata;
            else if (kind == PackIconKind.TextBox) return ClipType.TextMetadata;
            else if (kind == PackIconKind.Shape) return ClipType.FigureMetadata;
            else if (kind == PackIconKind.RoundedCorner) return ClipType.RoundRectMetadata;
            else if (kind == PackIconKind.Videocam) return ClipType.CameraMetadata;
            else if (kind == PackIconKind.Cube) return ClipType.GL3DObjectMetadata;
            else if (kind == PackIconKind.MovieOpen) return ClipType.SceneMetadata;

            return ClipType.VideoMetadata;
        }
    }
}
