using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VoicemeeterOsdProgram.UiControls.Helpers;

public class ListBoxSelectedKeysAttachedProperty
{
    public static readonly DependencyProperty SelectedKeysProperty =
        DependencyProperty.RegisterAttached("SelectedKeys", typeof(IList),
        typeof(ListBoxSelectedKeysAttachedProperty),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        new PropertyChangedCallback(OnSelectedKeysChanged)));

    public static IList GetSelectedKeys(DependencyObject d)
    {
        return (IList)d.GetValue(SelectedKeysProperty);
    }

    public static void SetSelectedKeys(DependencyObject d, IList value)
    {
        d.SetValue(SelectedKeysProperty, value);
    }

    private static void OnSelectedKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var lb = (ListBox)d;
        lb.SelectionChanged -= OnSelectionChangedKeys;
        lb.SelectionChanged += OnSelectionChangedKeys;

        var source = lb.Items.SourceCollection;
        if (source is not null)
        {
            Dictionary<object, object> sourceDict = new();
            foreach (var item in source)
            {
                var key = item.GetType().GetProperty("Key")?.GetValue(item);
                if (key is null) continue;

                sourceDict.Add(key, item);
            }

            if (e.NewValue is IEnumerable newVals)
            {
                foreach (var item in newVals)
                {
                    if (!sourceDict.ContainsKey(item)) continue;

                    var selItem = sourceDict[item];
                    lb.SelectedItems.Add(selItem);
                }
            }
        }
    }

    private static void OnSelectionChangedKeys(object sender, SelectionChangedEventArgs e)
    {
        var listbox = (ListBox)sender;
        List<object> modelSelectedItems = new();

        if (listbox.SelectedItems is not null)
        {
            foreach (var item in listbox.SelectedItems)
            {
                var key = item.GetType().GetProperty("Key")?.GetValue(item);
                if (key is null) continue;

                modelSelectedItems.Add(key);
            }
        }
        SetSelectedKeys(listbox, modelSelectedItems);
    }
}
