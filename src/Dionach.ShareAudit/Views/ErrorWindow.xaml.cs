using System.Windows;

namespace Dionach.ShareAudit.Views
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string errorText)
        {
            InitializeComponent();
            _errorTextBox.Text = errorText;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
