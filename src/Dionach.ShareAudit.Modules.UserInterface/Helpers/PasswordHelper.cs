using System.Windows;
using System.Windows.Controls;

namespace Dionach.ShareAudit.Modules.UserInterface.Helpers
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, OnAttachChanged));

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);

        public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);

        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

        private static void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
            {
                return;
            }

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }

            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);
    }
}
