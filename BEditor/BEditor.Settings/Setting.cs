﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace BEditor
{
    [DataContract]
    public class Settings : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields

        private static readonly PropertyChangedEventArgs clipHeightArgs = new(nameof(ClipHeight));
        private static readonly PropertyChangedEventArgs darkModeArgs = new(nameof(UseDarkMode));
        private static readonly PropertyChangedEventArgs autoBackUpArgs = new(nameof(AutoBackUp));
        private static readonly PropertyChangedEventArgs lastTimeFolderArgs = new(nameof(LastTimeFolder));
        private static readonly PropertyChangedEventArgs lastTimeNumArgs = new(nameof(LastTimeNum));
        private static readonly PropertyChangedEventArgs widthOf1FrameArgs = new(nameof(WidthOf1Frame));
        private static readonly PropertyChangedEventArgs enableErrorLogArgs = new(nameof(EnableErrorLog));
        private static readonly PropertyChangedEventArgs backgroundColorArgs = new(nameof(BackgroundColor));
        private static readonly PropertyChangedEventArgs langArgs = new(nameof(Language));
        private static readonly PropertyChangedEventArgs stackLimitArgs = new(nameof(StackLimit));
        private int clipHeight = 25;
        private bool darkMode = true;
        private bool autoBackUp = true;
        private string lastTimeFolder = "";
        private int lastTimeNum = 0;
        private int widthOf1Frame = 5;
        private bool enableErrorLog = false;
        private ObservableCollection<string>? enablePlugins;
        private ObservableCollection<string>? disablePlugins;
        private ObservableCollection<string>? includeFonts;
        private string? backgroundColor;
        private ObservableCollection<string>? mostRecentlyUsedList;
        private string? language;
        private uint stackLimit = 1048576;

        #endregion


        static Settings()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "user", "settings.json");
            if (!File.Exists(path))
            {
                Default = new Settings();
                Serialize.SaveToFile(Default, path);
            }
            else
            {
                Default = Serialize.LoadFromFile<Settings>(path) ?? new Settings();
            }
        }
        private Settings() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Properties

        public static Settings Default { get; }
        [DataMember]
        public int ClipHeight
        {
            get => clipHeight;
            set => SetValue(value, ref clipHeight, clipHeightArgs);
        }
        [DataMember]
        public bool UseDarkMode
        {
            get => darkMode;
            set => SetValue(value, ref darkMode, darkModeArgs);
        }
        [DataMember]
        public bool AutoBackUp
        {
            get => autoBackUp;
            set => SetValue(value, ref autoBackUp, autoBackUpArgs);
        }
        [DataMember]
        public string LastTimeFolder
        {
            get => lastTimeFolder ??= "";
            set => SetValue(value, ref lastTimeFolder, lastTimeFolderArgs);
        }
        [DataMember]
        public int LastTimeNum
        {
            get => lastTimeNum;
            set => SetValue(value, ref lastTimeNum, lastTimeNumArgs);
        }
        [DataMember]
        public int WidthOf1Frame
        {
            get => widthOf1Frame;
            set => SetValue(value, ref widthOf1Frame, widthOf1FrameArgs);
        }
        [DataMember]
        public bool EnableErrorLog
        {
            get => enableErrorLog;
            set => SetValue(value, ref enableErrorLog, enableErrorLogArgs);
        }
        [DataMember]
        public ObservableCollection<string> EnablePlugins
        {
            get => enablePlugins ??= new();
            set => enablePlugins = value;
        }
        [DataMember]
        public ObservableCollection<string> DisablePlugins
        {
            get => disablePlugins ??= new();
            set => disablePlugins = value;
        }
        [DataMember]
        public ObservableCollection<string> MostRecentlyUsedList
        {
            get => mostRecentlyUsedList ??= new();
            private set => mostRecentlyUsedList = new(value.Where(file => File.Exists(file)));
        }
        [DataMember]
        public ObservableCollection<string> IncludeFontDir
        {
            get
            {
                if (includeFonts is null)
                {
                    includeFonts = new();

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        includeFonts.Add($"{user}\\AppData\\Local\\Microsoft\\Windows\\Fonts");
                        includeFonts.Add("C:\\Windows\\Fonts");
                    }
                }

                return includeFonts;
            }
            set => includeFonts = value;
        }
        [DataMember]
        public string BackgroundColor
        {
            get => backgroundColor ??= "#ff000000";
            set
            {
                if (!IsHTMLColor(value)) return;

                SetValue(value, ref backgroundColor, backgroundColorArgs);
            }
        }
        [DataMember]
        public string Language
        {
            get => language ??= CultureInfo.CurrentCulture.Name;
            set => SetValue(value, ref language, langArgs);
        }
        [DataMember]
        public uint StackLimit
        {
            get => stackLimit;
            set => SetValue(value, ref stackLimit, stackLimitArgs);
        }
        public ExtensionDataObject? ExtensionData { get; set; }

        #endregion

        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        private void SetValue<T1>(T1 src, ref T1 dst, PropertyChangedEventArgs args)
        {
            if (src == null || !src.Equals(dst))
            {
                dst = src;
                RaisePropertyChanged(args);
            }
        }
        public void Save() => Serialize.SaveToFile(this, Path.Combine(AppContext.BaseDirectory, "user", "settings.json"));
        private static bool IsHTMLColor(string color)
        {
            try
            {
                color = "0x" + color.Replace("#", "");

                var argb = Convert.ToUInt32(color, 16);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}