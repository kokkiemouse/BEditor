﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using BEditor.Core.Plugin;

namespace BEditor.ViewModels.Converters
{
    public class PluginNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IPlugin plugin)
            {
                var assembly = plugin.GetType().Assembly;
                var name = assembly.GetName();
                return $"{name.Name} {name.Version}";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PluginAuthorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IPlugin plugin)
            {
                var assembly = plugin.GetType().Assembly;

                var attributes = assembly.CustomAttributes.ToArray();

                var a = Array.Find(attributes, a => a.AttributeType == typeof(AssemblyCompanyAttribute));

                return a.ConstructorArguments.FirstOrDefault().Value;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
