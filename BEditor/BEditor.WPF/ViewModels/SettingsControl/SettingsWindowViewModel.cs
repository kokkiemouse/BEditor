﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using BEditor.Views.SettingsControl;
using BEditor.Views.SettingsControl.General;
using BEditor.Views.SettingsControl.Plugins;
using BEditor.Core.Properties;
using MaterialDesignThemes.Wpf;
using BEditor.Core.Data;
using System.ComponentModel;
using Reactive.Bindings;

namespace BEditor.ViewModels.SettingsControl
{
    public class SettingsWindowViewModel : BasePropertyChanged
    {
        private static readonly PropertyChangedEventArgs Args = new(nameof(ViewControl));
        private object viewControl;

        public SettingsWindowViewModel()
        {
            TreeSelectCommand.Subscribe(obj =>
            {
                if (obj is TreeViewChild child)
                {
                    ViewControl = child.Control;
                }
            });
            UnloadedCommand.Subscribe(_ => Settings.Default.Save());


            #region General

            var general = new TreeViewChild()
            {
                Text = Resources.General,
                PackIconKind = PackIconKind.Settings,
                Control = new Root()
            };

            //外観
            general.TreeViewChildren.Add(new TreeViewChild()
            {
                Text = Resources.Appearance,
                PackIconKind = PackIconKind.WindowMaximize,
                Control = new Appearance()
            });

            //その他
            general.TreeViewChildren.Add(new TreeViewChild()
            {
                Text = Resources.Others,
                PackIconKind = PackIconKind.DotsVertical,
                Control = new Other()
            });

            #endregion

            #region Project

            var project = new TreeViewChild()
            {
                Text = Resources.Project,
                PackIconKind = PackIconKind.File,
                Control = new ProjectSetting()
            };

            #endregion

            #region Plugins

            var plugins = new TreeViewChild()
            {
                Text = Resources.Plugins,
                PackIconKind = PackIconKind.Puzzle
            };

            plugins.TreeViewChildren.Add(new TreeViewChild()
            {
                Text = Resources.InstalledPlugins,
                Control = new InstalledPlugins()
            });

            plugins.TreeViewChildren.Add(new TreeViewChild()
            {
                Text = Resources.DisabledPlugins,
                Control = new DisabledPlugins()
            });

            #endregion

            #region AppInfo

            var appInfo = new TreeViewChild()
            {
                Text = Resources.Infomation,
                PackIconKind = PackIconKind.Information,
                Control = new AppInfo()
            };

            #endregion

            TreeViewProperty.Add(general);
            TreeViewProperty.Add(project);
            TreeViewProperty.Add(plugins);
            TreeViewProperty.Add(appInfo);
        }

        /// <summary>
        /// 表示されているコントロール
        /// </summary>
        public object ViewControl { get => viewControl; set => SetValue(value, ref viewControl, Args); }


        public ReactiveCommand<object> TreeSelectCommand { get; } = new();
        public ReactiveCommand<object> UnloadedCommand { get; } = new();

        public ObservableCollection<TreeViewChild> TreeViewProperty { get; set; } = new ObservableCollection<TreeViewChild>();
    }

    public class TreeViewChild
    {

        public string Text { get; set; }
        public PackIconKind PackIconKind { get; set; } = PackIconKind.None;
        public ObservableCollection<TreeViewChild> TreeViewChildren { get; set; } = new ObservableCollection<TreeViewChild>();
        public object Control { get; set; }
    }
}
