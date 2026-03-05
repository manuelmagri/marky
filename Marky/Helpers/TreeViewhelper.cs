using System.Windows;
using System.Windows.Controls;

namespace Marky.Helpers
{
    public static class TreeViewHelper
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(object),
            typeof(TreeViewHelper),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemChanged)
        );


        public static object GetSelectedItem(DependencyObject obj) {
            return obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value) {
            obj.SetValue(SelectedItemProperty, value);
        }


        public static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            if (obj is TreeView treeView) {
                treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;
                treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            }
        }

        public static void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView)
            {
                SetSelectedItem(treeView, e.NewValue);
            }
        }

    }
}
