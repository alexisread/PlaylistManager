using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PlaylistManager
{
    // http://stackoverflow.com/questions/15475047/button-pressed-change-color-by-conditionwpf
    public class ButtonThing : DependencyObject
    {
        public static object GetBusy(DependencyObject obj)
        {
            return (object)obj.GetValue(Tag2Property);
        }
        public static void SetBusy(DependencyObject obj, object value)
        {
            obj.SetValue(Tag2Property, value);
        }
        public static readonly DependencyProperty Tag2Property = DependencyProperty.RegisterAttached("Busy", typeof(object), typeof(ButtonThing), new PropertyMetadata(null));
    }
}
