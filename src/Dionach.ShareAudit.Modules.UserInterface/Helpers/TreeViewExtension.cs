using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dionach.ShareAudit.Modules.UserInterface.Helpers
{
    public static class TreeViewExtension
    {
        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.RegisterAttached(
           "SelectItemOnRightClick",
           typeof(bool),
           typeof(TreeViewExtension),
           new UIPropertyMetadata(false, OnSelectItemOnRightClickChanged));

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetSelectItemOnRightClick(DependencyObject d)
        {
            return (bool)d.GetValue(SelectItemOnRightClickProperty);
        }

        public static void SetSelectItemOnRightClick(DependencyObject d, bool value)
        {
            d.SetValue(SelectItemOnRightClickProperty, value);
        }

        public static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private static void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private static void OnSelectItemOnRightClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool selectItemOnRightClick = (bool)e.NewValue;

            if (d is TreeView treeView)
            {
                if (selectItemOnRightClick)
                {
                    treeView.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
                }
                else
                {
                    treeView.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
                }
            }
        }
    }
}
