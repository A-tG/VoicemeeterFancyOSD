﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class PanelMarginSetter
    {
        public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty = DependencyProperty.
            RegisterAttached("Margin", typeof(Thickness), typeof(PanelMarginSetter), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not Panel panel) return;

            panel.Loaded += new RoutedEventHandler(PanelLoaded);
        }

        static void PanelLoaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as Panel;
            foreach (var child in panel.Children)
            {
                if (child is not FrameworkElement fe) continue;

                fe.Margin = GetMargin(panel);
            }

        }
    }


}
